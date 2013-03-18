using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using StackExchange.Profiling;

namespace GitNinja.Gallery.Web.Controllers
{
  public class GitController : Controller
  {
    [HttpPost]
    public ActionResult UploadPack(string dojo, string repo)
    {
      if (!GitNinja.IsValidRepository(dojo, repo)) return HttpNotFound();
      return new GitStreamResult("{0} --stateless-rpc .", "upload-pack", GitNinja.GetRepositoryPath(dojo, repo));
    }

    [HttpPost]
    public ActionResult ReceivePack(string dojo, string repo)
    {
      if (!GitNinja.IsValidRepository(dojo, repo)) return HttpNotFound();
      return new GitStreamResult("{0} --stateless-rpc .", "receive-pack", GitNinja.GetRepositoryPath(dojo, repo));
    }

    public ActionResult GetInfoRefs(string dojo, string repo, string service)
    {
      if (!GitNinja.IsValidRepository(dojo, repo)) return HttpNotFound();
      return new GitCommandResult("{0} --stateless-rpc --advertise-refs .", GetService(service), GitNinja.GetRepositoryPath(dojo, repo));
    }

    private static string GetService(string service)
    {
      return (!string.IsNullOrWhiteSpace(service) && service.StartsWith("git-"))
        ? service.Substring(4)
        : null;
    }
  }

  public static class GitUtilities
  {
    public static Encoding DefaultEncoding
    {
      get { return Encoding.GetEncoding(28591); }
    }

    public static string Execute(string gitArgs, string repositoryPath)
    {
      using (var git = Start(gitArgs, repositoryPath))
      {
        var result = git.StandardOutput.ReadToEnd();
        git.WaitForExit();
        return result;
      }
    }

    //todo: refactor this horrible method
    public static Process Start(string gitArgs, string repositoryPath, bool redirectInput = false)
    {
      var processStartInfo = new ProcessStartInfo
      {
        FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Git", "bin", "git.exe"),
        Arguments = gitArgs,
        WorkingDirectory = repositoryPath,
        RedirectStandardInput = redirectInput,
        StandardOutputEncoding = DefaultEncoding,
        RedirectStandardOutput = true,
        RedirectStandardError = false,
        UseShellExecute = false,
        CreateNoWindow = true,
      };
      Process process = null;
      IDisposable trace = null;
      IDisposable traceClosure = null;
      try
      {
        var returnProcess = process = new Process { EnableRaisingEvents = true, StartInfo = processStartInfo };
        process.Exited += (s, e) => { if (traceClosure != null) traceClosure.Dispose(); };
        try
        {
          var profiler = MiniProfiler.Current;
          if (profiler != null) traceClosure = trace = profiler.Step(string.Concat("git ", gitArgs));
          process.Start();
          trace = process = null;
          return returnProcess;
        }
        finally
        {
          if (trace != null) trace.Dispose();
        }
      }
      finally
      {
        if (process != null) process.Dispose();
      }
    }
  }

  public class GitStreamResult : ActionResult
  {
    private readonly string commandFormat;
    private readonly string action;
    private readonly string repoPath;

    public GitStreamResult(string commandFormat, string action, string repoPath)
    {
      if (string.IsNullOrEmpty(commandFormat))
        throw new ArgumentNullException("commandFormat");
      if (string.IsNullOrEmpty(action))
        throw new ArgumentNullException("action");
      if (string.IsNullOrEmpty(repoPath))
        throw new ArgumentNullException("repoPath");

      this.commandFormat = commandFormat;
      this.action = action;
      this.repoPath = repoPath;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      var response = context.HttpContext.Response;
      var request = context.HttpContext.Request;
      var realRequest = System.Web.HttpContext.Current.Request;
      response.ContentType = string.Format("application/x-git-{0}-result", action);
      response.BufferOutput = false;
      using (var gitProcess = GitUtilities.Start(string.Format(commandFormat, action), repoPath, true))
      {
        var readThread = new Thread(() =>
        {
          var readBuffer = new byte[4096];
          Stream wrapperStream = null;
          try
          {
            var input = realRequest.GetBufferlessInputStream();
            if (request.Headers["Content-Encoding"] == "gzip")
              input = wrapperStream = new GZipStream(input, CompressionMode.Decompress);
            int readCount;
            while ((readCount = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
              gitProcess.StandardInput.BaseStream.Write(readBuffer, 0, readCount);
          }
          finally
          {
            if (wrapperStream != null) wrapperStream.Dispose();
          }
          gitProcess.StandardInput.Close();
        });
        readThread.Start();
        var writeBuffer = new byte[4096];
        int writeCount;
        byte[] copy = null;
        while ((writeCount = gitProcess.StandardOutput.BaseStream.Read(writeBuffer, 0, writeBuffer.Length)) > 0)
        {
          if (copy == null || copy.Length != writeCount)
            copy = new byte[writeCount];
          for (var i = 0; i < writeCount; i++)
            copy[i] = writeBuffer[i];
          response.BinaryWrite(copy);
        }
        readThread.Join();
        gitProcess.WaitForExit();
        if (gitProcess.ExitCode != 0)
        {
          response.StatusCode = 500;
          response.SubStatusCode = gitProcess.ExitCode;
        }
      }
    }
  }

  public class GitCommandResult : ActionResult
  {
    private readonly string commandFormat;
    private readonly string service;
    private readonly string repositoryPath;

    public GitCommandResult(string commandFormat, string service, string repositoryPath)
    {
      if (string.IsNullOrWhiteSpace(commandFormat)) throw new ArgumentNullException("commandFormat");
      if (string.IsNullOrWhiteSpace(service)) throw new ArgumentNullException("service");
      if (string.IsNullOrWhiteSpace(repositoryPath)) throw new ArgumentNullException("repositoryPath");

      this.commandFormat = commandFormat;
      this.service = service;
      this.repositoryPath = repositoryPath;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      var response = context.HttpContext.Response;
      var commandResult = GitUtilities.Execute(string.Format(commandFormat, service), repositoryPath);
      var commandData = GitUtilities.DefaultEncoding.GetBytes(commandResult);
      response.StatusCode = 200;
      response.ContentType = string.Format("application/x-git-{0}-advertisement", service);
      response.BinaryWrite(PacketFormat(string.Format("# service=git-{0}\n", service)));
      response.BinaryWrite(PacketFlush());
      response.BinaryWrite(commandData);
    }

    private static byte[] PacketFormat(string packet)
    {
      return GitUtilities.DefaultEncoding.GetBytes((packet.Length + 4).ToString("X").ToLower().PadLeft(4, '0') + packet);
    }

    private static byte[] PacketFlush()
    {
      return new[] { (byte)'0', (byte)'0', (byte)'0', (byte)'0' };
    }
  }

  public class GitException : Exception
  {
    public GitException(string command, string workingDir, int exitCode, string errorOutput)
      : base("Fatal error executing git command in '" + workingDir + "'." + Environment.NewLine + errorOutput)
    {
      Command = command;
      ExitCode = exitCode;
      ErrorOutput = errorOutput;
    }
    public string Command { get; private set; }
    public int ExitCode { get; private set; }
    public string ErrorOutput { get; private set; }
  }
}
