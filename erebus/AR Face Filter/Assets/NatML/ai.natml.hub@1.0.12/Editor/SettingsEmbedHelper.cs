/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub.Editor {

    using System.IO;
    using UnityEditor;
    using UnityEditor.Build.Reporting;
    using Internal;

    internal sealed class SettingsEmbedHelper : BuildEmbedHelper<HubSettings> {

        private const string CachePath = @"Assets/NMLBuildCache";

        protected override HubSettings[] CreateEmbeds (BuildReport report) {
            Directory.CreateDirectory(CachePath);
            var settings = HubProjectSettings.CreateSettings();
            AssetDatabase.CreateAsset(settings, $"{CachePath}/NatMLHub.asset");
            return new [] { settings };
        }

        protected override void ClearEmbeds (BuildReport report) {
            base.ClearEmbeds(report);
            AssetDatabase.DeleteAsset(CachePath);
        }
    }
}