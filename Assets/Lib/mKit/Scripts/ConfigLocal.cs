using System;
using System.IO;
using UnityEngine;

namespace mKit
{
    /// <summary>
    /// Config helper functions regarding local file system access.
    /// </summary>
    public static class ConfigDataLocal
    {
        public static string configRelativePath = "Config";

        public static string appName = "AppName";

        /// <summary>
        /// Config directory from the (not-user-modifiable) installation directory. 
        /// </summary>
        public static string ConfigPathInstallation
        {
            get
            {
                return mKit.HelperLib.StreamingAssetsPath + configRelativePath + System.IO.Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Sets the application name for use as relative path in <see cref="UserAppDirectory"/>.
        /// </summary>
        /// <param name="appname"></param>
        public static void SetAppName(string appname)
        {
            ConfigDataLocal.appName = appname;
        }

        /// <summary>
        /// User's application data directory
        /// (e.g. %APPDATA%/Sonogame)
        /// </summary>
        public static string UserAppDirectory
        {
            get
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + System.IO.Path.DirectorySeparatorChar + appName + System.IO.Path.DirectorySeparatorChar;
            }

        }

        /// <summary>
        /// Full directory path for user config, conists of <see cref="UserAppDirectory"/> and <see cref="configRelativePath"/> (e.g. config/ ).
        /// </summary>
        public static string UserConfigDirectory
        {
            get
            {
                return UserAppDirectory + configRelativePath + System.IO.Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Try to create the directory <see cref="UserConfigDirectory", if it does not exist.
        /// </summary>
        public static void CreateConfigPathLocal()
        {
            if (!Directory.Exists(UserAppDirectory))
            {
                DirectoryInfo info = Directory.CreateDirectory(UserAppDirectory);

                if (info.Exists)
                {
                    Debug.Log("Created directory: " + UserAppDirectory);
                }
            }

            if (!Directory.Exists(UserConfigDirectory))
            {
                DirectoryInfo info = Directory.CreateDirectory(UserConfigDirectory);

                if (info.Exists)
                {
                    Debug.Log("Created directory: " + UserConfigDirectory);
                }
            }

        }

        /// <summary>
        /// Return true if <see cref="UserConfigDirectory"/> was created.
        /// </summary>
        public static bool UserConfigExists
        {

            get
            {
                if (!Directory.Exists(UserConfigDirectory))
                {
                    try
                    {
                        CreateConfigPathLocal();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error creating directory: " + UserConfigDirectory + " (" + ex.Message + ")");
                    }
                }

                return Directory.Exists(UserConfigDirectory);
            }
        }

    }
}

