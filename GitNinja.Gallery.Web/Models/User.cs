using System;
using System.Collections.Generic;
using System.Web;
using GitNinja.Gallery.Web.Helpers;

namespace GitNinja.Gallery.Web.Models
{
  public class User
  {
    public string Id { get; set; }

    public string LdapPath { get; set; }

    public string Email { get; set; }

    public string Forename { get; set; }

    public string Surname { get; set; }

    public string DisplayName { get { return (!string.IsNullOrWhiteSpace(Forename) && !string.IsNullOrWhiteSpace(Surname)) ? string.Format("{0} {1}", Forename, Surname) : Id; } }


    public static User Current
    {
      get
      {
        User user;
        try { user = Ldap.GetUser(HttpContext.Current.User.Identity.Name); }
        catch { user = new User { Id = HttpContext.Current.User.Identity.Name }; }
        return user;
      }
    }
  }

  public class Group : List<User>
  {
    public string Id { get; set; }

    public string LdapPath { get; set; }

    public DateTime? Created { get; set; }

    public string Isp { get; set; }
  }
}