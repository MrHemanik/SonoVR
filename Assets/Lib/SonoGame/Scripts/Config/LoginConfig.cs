using mKit;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SonoGame
{
    [System.Serializable]
    public class LoginConfig
    {
        public string userListPath = @"Users/study-users.txt";

        public TextAsset userListAsset;

        public bool requireUserLogin;
        public bool requirePassword;
        public bool allowAdminLogin;
        public bool allowGuestLogin;

        public string adminRole = "admin";
        public string userRole = "user";
        public string guestLogin = "Guest";
        public string adminPwd = "fh15";

        [Header("UI Settings")]
        public bool showUsername;

        internal List<string> userNames;
        internal List<string> adminNames;
        internal Credentials guestCredentials;
        internal List<Credentials> userCredentials;

        internal LoginConfig()
        {
            guestCredentials = new Credentials(guestLogin, "", AccountRole.Guest);
        }

        internal AccountRole GetRoleByString(string role)
        {
            AccountRole result = AccountRole.None;

            if (role == userRole)
            {
                result = AccountRole.User;
            }
            else if (role == adminRole)
            {
                result = AccountRole.Admin;
            }

            return result;
        }

        /// <summary>
        /// Validate role (of imported users).
        /// Note: Guest role cannot login by name currently,
        /// only by button: <see cref="LoginManager.OnGuestLoginButton"/>
        /// </summary>
        /// <param name="cred"></param>
        /// <returns>validation success</returns>
        internal bool ValidateRole(Credentials cred)
        {
            return cred.Role == AccountRole.User || (cred.Role == AccountRole.Admin && allowAdminLogin);
        }

        internal void LoadUsers()
        {
            string[] lines = new string[0];

            if (AppConfig.current.loginConfig.userListAsset != null)
            {
                lines = AppConfig.current.loginConfig.userListAsset.text.Split('\n');
            }
            else if (File.Exists(HelperLib.StreamingAssetsPath + userListPath))
            {
                Debug.Log("LoginConfig: reading users from " + userListPath);
                lines = File.ReadAllLines(HelperLib.StreamingAssetsPath + userListPath);
            }

#if UNITY_EDITOR
            Debug.Log("LoginConfig: found " + lines.Length + " user items");
#endif

            userNames = new List<string>();
            adminNames = new List<string>();
            userCredentials = new List<Credentials>();

            if (allowGuestLogin)
            {
                userNames.Add(guestLogin);
            }

            foreach (var l in lines)
            {
                bool added = false;

                string[] tokens = l.Replace("\r", "").Split(';');

                if (tokens.Length >= 2)
                {
                    string username = tokens[0].Trim(' ', '\t');
                    string password = tokens[1].Trim(' ', '\t');
                    string roleStr = (tokens.Length >= 3) ? tokens[2] : "user";
                    AccountRole role = GetRoleByString(roleStr);

                    if (username != "" && password != "" && role != AccountRole.None && username != guestLogin)
                    {
                        userNames.Add(username);
                        userCredentials.Add(new Credentials(username, password, role));
                        added = true;

                        if (role == AccountRole.Admin)
                        {
                            adminNames.Add(username);
                        }

                        //Debug.Log("Added login: " + username + " (" + role + ")");
                    }
                }

                if (!added)
                {
#if UNITY_EDITOR
                    Debug.LogError("LoginManager userList ignoring line: " + l);
#else
                    Debug.LogWarning("LoginManager userList ignoring line: " + l);
#endif

                }
            } // end foreach
        }
    }

}
