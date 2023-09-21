/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub.Editor {

    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEditor;
    using Internal;

    [FilePath(@"ProjectSettings/NatMLHub.asset", FilePathAttribute.Location.ProjectFolder)]
    internal sealed class HubProjectSettings : ScriptableSingleton<HubProjectSettings> {

        #region --Data--
        [SerializeField]
        private string accessKey;
        #endregion


        #region --Client API--
        /// <summary>
        /// NatML access key.
        /// </summary>
        internal string AccessKey {
            get => accessKey;
            set {
                accessKey = value;
                Save(false);
            }
        }

        /// <summary>
        /// Create NatML settings from the current project settings.
        /// </summary>
        /// <param name="refresh">Whether to force refresh the user info from the NatML Hub API.</param>
        internal static HubSettings CreateSettings (bool refresh = false) {
            // Create settings
            var settings = ScriptableObject.CreateInstance<HubSettings>();
            settings.accessKey = instance.AccessKey;
            // Fetch user
            var userData = SessionState.GetString(UserKey, string.Empty);
            var user = JsonUtility.FromJson<User>(userData);
            if (string.IsNullOrEmpty(userData) || refresh) {
                user = Task.Run(() => NatMLHub.GetUser(instance.AccessKey)).Result;
                SessionState.SetString(UserKey, JsonUtility.ToJson(user));
            }
            settings.user = user;
            // Return
            return settings;
        }
        #endregion


        #region --Operations--
        private static string UserKey => $"{HubSettings.API}.user";

        [InitializeOnLoadMethod]
        private static async void OnLoad () {
            // Populate settings
            var userData = SessionState.GetString(UserKey, string.Empty);
            var firstLoad = string.IsNullOrEmpty(userData);
            var settings = CreateSettings();
            HubSettings.settings = settings;
            // Wait for clients to subscribe to `OnUpdateSettings`
            for (var i = 0; i < 3; ++i)
                await Task.Yield();
            // Trigger `OnUpdateSettings`
            if (firstLoad)
                HubSettings.Instance = settings;
        }

        internal void UpdateSettings () {
            Save(false);
            HubSettings.Instance = CreateSettings(true);
        }
        #endregion
    }
}