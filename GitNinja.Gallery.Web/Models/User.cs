using System;
using System.Collections.Generic;
using System.Linq;
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

        public string DisplayName { get { return string.Format("{0} {1}", Forename, Surname); } }

        public static User Current { get { return Ldap.GetUser(HttpContext.Current.User.Identity.Name); } }
    }

    public class Group : List<User>
    {
        public string Id { get; set; }

        public string LdapPath { get; set; }

        public DateTime? Created { get; set; }

        public string Isp { get; set; }
    }
}