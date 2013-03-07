using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using GitNinja.Gallery.Web.Models;

namespace GitNinja.Gallery.Web.Helpers
{
    public static class Ldap
    {
        private const string Root = "LDAP://group-ldap.rwe.com:389/";
        private const string GroupBaseDn = "OU=Gruppen,OU=Infrastruktur,DC=group,DC=rwe,DC=com";
        private const string AlternateGroupBaseDn = "OU=Gruppen,OU=RWE Trading,OU=Gesellschaften,DC=group,DC=rwe,DC=com";
        private const string UserBaseDn = "DC=group,DC=rwe,DC=com";

        private static readonly DirectoryEntry GroupBase = new DirectoryEntry(Root + GroupBaseDn, Username, Password);
        private static readonly DirectoryEntry AlternateGroupBase = new DirectoryEntry(Root + AlternateGroupBaseDn, Username, Password);
        private static readonly DirectoryEntry UserBase = new DirectoryEntry(Root + UserBaseDn, Username, Password);

        private static readonly string[] GroupProps = new[] { "member", "samaccountname", "whencreated", "description" };
        private static readonly string[] UserProps = new[] { "samaccountname", "displayname", "mail" };

        public static string Username { get { return "SRV_RWEST_TC_MGR" /*ConfigurationManager.AppSettings.Get("LdapBindUsername")*/; } }
        public static string Password { get { return "k5w5z5Ih0R4OJZR" /*ConfigurationManager.AppSettings.Get("LdapBindPassword")*/; } }

        public static User ToUser(this SearchResult searchResult)
        {
            var user = new User();
            try { user.Id = searchResult.GetStringProperty("samaccountname").ToLowerInvariant(); }
            catch { user.Id = null; }
            try { user.Forename = searchResult.GetStringProperty("displayname").Split(',').Last().Trim(); }
            catch { user.Forename = null; }
            try { user.Surname = searchResult.GetStringProperty("displayname").Split(',').First().Trim(); }
            catch { user.Surname = null; }
            try { user.Email = searchResult.GetStringProperty("mail").ToLowerInvariant(); }
            catch { user.Email = null; }
            try { user.LdapPath = searchResult.Path; }
            catch { user.LdapPath = null; }
            return user;
        }

        public static Group ToGroup(this SearchResult searchResult)
        {
            var group = new Group();
            try { group.Id = searchResult.GetStringProperty("samaccountname"); }
            catch { group.Id = null; }
            try { group.Created = searchResult.GetDateTimeProperty("whencreated"); }
            catch { group.Created = null; }
            try { group.Isp = searchResult.GetStringProperty("description"); }
            catch { group.Isp = null; }
            try { group.LdapPath = searchResult.Path; }
            catch { group.LdapPath = null; }
            return group;
        }

        public static Group GetGroup(string id)
        {
            Group group = null;
            var groupSearch = new DirectorySearcher(GroupBase, string.Format("(cn={0})", id), GroupProps);
            var groupResult = groupSearch.FindOne();
            if (groupResult == null)
            {
                groupSearch = new DirectorySearcher(AlternateGroupBase, string.Format("(cn={0})", id), GroupProps);
                groupResult = groupSearch.FindOne();
            }
            if (groupResult != null)
            {
                group = groupResult.ToGroup();
                group.AddRange(groupResult.Properties["member"].Cast<string>()
                    .Select(userDn => new DirectoryEntry(Root + userDn, Username, Password))
                    .Select(entry => GetUserSearcher(entry).FindOne().ToUser()));
            }
            return group;
        }

        private static DirectorySearcher GetUserSearcher(DirectoryEntry entry)
        {
            var searcher = new DirectorySearcher(entry);
            searcher.PropertiesToLoad.AddRange(UserProps);
            return searcher;
        }

        private static SearchResult FindUser(string id)
        {
            if (id.Contains('\\'))
                id = id.Split('\\').Last();
            var search = new DirectorySearcher(UserBase, string.Format("(&(samaccountname={0}))", id), UserProps);
            return search.FindOne();
        }

        public static IEnumerable<Group> FindGroups(params string[] patterns)
        {
            var sb = new StringBuilder();
            if (patterns.Length > 1)
                sb.Append("(|");
            foreach (var pattern in patterns)
                sb.AppendFormat("(cn=*{0}*)", pattern);
            if (patterns.Length > 1)
                sb.Append(")");
            var groupSearch = new DirectorySearcher(GroupBase, sb.ToString(), GroupProps);
            return (groupSearch.FindAll().Cast<SearchResult>().Where(groupResult => groupResult != null).Select(groupResult => groupResult.ToGroup()));
        }

        public static User GetUser(string id)
        {
            return FindUser(id).ToUser();
        }

        public static IEnumerable<string> GetUserGroups(string userId)
        {
            return ((IEnumerable)new DirectoryEntry(FindUser(userId).Path, Username, Password).Invoke("Groups"))
            .Cast<object>().Select(x => new DirectoryEntry(x)).Select(x => x.Name.Substring(3));
        }

        public static string GetStringProperty(this SearchResult searchResult, string propertyName)
        {
            return searchResult.Properties[propertyName].Cast<string>().First();
        }

        public static DateTime GetDateTimeProperty(this SearchResult searchResult, string propertyName)
        {
            return searchResult.Properties[propertyName].Cast<DateTime>().First();
        }
    }
}