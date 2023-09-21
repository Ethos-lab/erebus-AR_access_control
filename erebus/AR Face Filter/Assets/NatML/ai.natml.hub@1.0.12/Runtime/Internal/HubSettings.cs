/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub.Internal {

    using System;
    using UnityEngine;

    /// <summary>
    /// NatML Hub settings.
    /// </summary>
    public sealed class HubSettings : ScriptableObject {

        #region --Data--
        [SerializeField, HideInInspector]
        internal string accessKey = string.Empty;
        [SerializeField, HideInInspector]
        internal User user = null;
        #endregion


        #region --Client API--
        /// <summary>
        /// NatML access key.
        /// </summary>
        public string AccessKey => accessKey;

        /// <summary>
        /// Current NatML user.
        /// </summary>
        public User User => !string.IsNullOrEmpty(user?.username) ? user : null;

        /// <summary>
        /// NatML Hub settings for this project.
        /// </summary>
        public static HubSettings Instance {
            get => settings;
            internal set => OnUpdateSettings?.Invoke(settings = value);
        }

        /// <summary>
        /// Event raised when the settings have been updated by the user.
        /// This event is only raised in the Editor, in the following cases:
        /// 1. Editor startup, after the user account is fetched.
        /// 2. NatML access key is updated by the user in Project Settings.
        /// </summary>
        public static event Action<HubSettings> OnUpdateSettings;
        #endregion


        #region --Operations--
        public const string API = @"ai.natml.hub";
        public const string Version = @"1.0.12";
        public const string EditorBundle = @"com.unity3d.UnityEditor5.x";
        internal static HubSettings settings; // setting this does _not_ trigger `OnUpdateSettings`

        void OnEnable () => settings = this;
        #endregion
    }
}