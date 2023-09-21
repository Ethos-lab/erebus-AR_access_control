/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub.Editor {

    using System.Collections.Generic;
    using UnityEditor;

    internal static class HubSettingsProvider {

        [SettingsProvider]
        public static SettingsProvider CreateProvider () => new SettingsProvider(@"Project/NatML", SettingsScope.Project) {
            label = @"NatML",
            guiHandler = searchContext => {
                EditorGUILayout.LabelField(@"NatML Account", EditorStyles.boldLabel);
                HubProjectSettings.instance.AccessKey = EditorGUILayout.TextField(@"Access Key", HubProjectSettings.instance.AccessKey);
            },
            deactivateHandler = HubProjectSettings.instance.UpdateSettings,
            keywords = new HashSet<string>(new[] { @"NatML", @"NatCorder", @"NatDevice", @"NatShare", @"Hub", @"NatML Hub" }),
        };
    }
}
