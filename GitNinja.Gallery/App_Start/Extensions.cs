using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace GitNinja.Gallery.App_Start
{
  public static class StringExtensions
  {
    public static string ToMd5Hash(this string raw)
    {
      return string.IsNullOrWhiteSpace(raw)
        ? new string('0', 32)
        : string.Join(string.Empty, new MD5CryptoServiceProvider().ComputeHash(Encoding.ASCII.GetBytes(raw.Trim().ToLower())).Select(x => x.ToString("x2")));
    }
  }

  public static class HtmlHelperExtensions
  {
    /// <summary>
    /// Creates HTML for an <c>img</c> element that presents a Gravatar icon.
    /// </summary>
    /// <param name="html">The <see cref="HtmlHelper"/> upon which this extension method is provided.</param>
    /// <param name="email">The email address used to identify the icon.</param>
    /// <param name="size">An optional parameter that specifies the size of the square image in pixels.</param>
    /// <param name="rating">An optional parameter that specifies the safety level of allowed images.</param>
    /// <param name="defaultImage">An optional parameter that controls what image is displayed for email addresses that don't have associated Gravatar icons.</param>
    /// <param name="htmlAttributes">An optional parameter holding additional attributes to be included on the <c>img</c> element.</param>
    /// <returns>An HTML string of the <c>img</c> element that presents a Gravatar icon.</returns>
    public static string Gravatar(this HtmlHelper html,
                                  string email,
                                  int? size = null,
                                  GravatarRating rating = GravatarRating.Default,
                                  GravatarDefaultImage defaultImage = GravatarDefaultImage.MysteryMan,
                                  object htmlAttributes = null)
    {
      var url = new StringBuilder("http://www.gravatar.com/avatar/", 90);
      url.Append(email.ToMd5Hash());

      var isFirst = true;
      Action<string, string> addParam = (p, v) =>
      {
        url.Append(isFirst ? '?' : '&');
        isFirst = false;
        url.Append(p);
        url.Append('=');
        url.Append(v);
      };

      if (size != null)
      {
        if (size < 1 || size > 512)
          throw new ArgumentOutOfRangeException("size", size, "Must be null or between 1 and 512, inclusive.");
        addParam("s", size.Value.ToString(CultureInfo.InvariantCulture));
      }

      if (rating != GravatarRating.Default)
        addParam("r", rating.ToString().ToLower());

      switch (defaultImage)
      {
        case GravatarDefaultImage.Http404:
          addParam("d", "404");
          break;
        case GravatarDefaultImage.Identicon:
          addParam("d", "identicon");
          break;
        case GravatarDefaultImage.MonsterId:
          addParam("d", "monsterid");
          break;
        case GravatarDefaultImage.MysteryMan:
          addParam("d", "mm");
          break;
        case GravatarDefaultImage.Wavatar:
          addParam("d", "wavatar");
          break;
      }

      var tag = new TagBuilder("img");
      tag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
      tag.Attributes.Add("src", url.ToString());

      if (size != null)
      {
        tag.Attributes.Add("width", size.ToString());
        tag.Attributes.Add("height", size.ToString());
      }
      return tag.ToString(TagRenderMode.SelfClosing);
    }
  }

  public enum GravatarRating
  {
    /// <summary>
    /// The default value as specified by the Gravatar service.  That is, no rating value is specified
    /// with the request.  At the time of authoring, the default level was <see cref="G"/>.
    /// </summary>
    Default,

    /// <summary>
    /// Suitable for display on all websites with any audience type.  This is the default.
    /// </summary>
    G,

    /// <summary>
    /// May contain rude gestures, provocatively dressed individuals, the lesser swear words, or mild violence.
    /// </summary>
    Pg,

    /// <summary>
    /// May contain such things as harsh profanity, intense violence, nudity, or hard drug use.
    /// </summary>
    R,

    /// <summary>
    /// May contain hardcore sexual imagery or extremely disturbing violence.
    /// </summary>
    X
  }

  public enum GravatarDefaultImage
  {
    /// <summary>
    /// The default value image.  That is, the image returned when no specific default value is included
    /// with the request.  At the time of authoring, this image is the Gravatar icon.
    /// </summary>
    Default,

    /// <summary>
    /// Do not load any image if none is associated with the email hash, instead return an HTTP 404 (File Not Found) response.
    /// </summary>
    Http404,

    /// <summary>
    /// A simple, cartoon-style silhouetted outline of a person (does not vary by email hash).
    /// </summary>
    MysteryMan,

    /// <summary>
    /// A geometric pattern based on an email hash.
    /// </summary>
    Identicon,

    /// <summary>
    /// A generated 'monster' with different colors, faces, etc.
    /// </summary>
    MonsterId,

    /// <summary>
    /// Generated faces with differing features and backgrounds.
    /// </summary>
    Wavatar
  }
}