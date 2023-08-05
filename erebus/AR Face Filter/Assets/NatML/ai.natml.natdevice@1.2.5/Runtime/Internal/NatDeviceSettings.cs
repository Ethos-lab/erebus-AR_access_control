/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Internal {

    using UnityEngine;

    internal sealed class NatDeviceSettings : ScriptableObject {

        #region --Data--
        [SerializeField, HideInInspector]
        internal string token = string.Empty;
        #endregion


        #region --Client API--
        /// <summary>
        /// Get or set the NatDevice app token.
        /// </summary>
        internal string Token => !string.IsNullOrEmpty(token) ? token : null;

        /// <summary>
        /// NatDevice settings for this project.
        /// </summary>
        public static NatDeviceSettings Instance { get; internal set; }
        #endregion


        #region --Operations--
        public const string API = @"ai.natml.natdevice";
        public const string Version = @"1.2.5";

        void OnEnable () => Instance = this;
        #endregion
    }
}