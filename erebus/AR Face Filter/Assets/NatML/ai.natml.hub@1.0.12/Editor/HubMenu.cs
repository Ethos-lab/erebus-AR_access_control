/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub.Editor {

    using UnityEditor;
    using Internal;

    internal static class HubMenu {

        private const int BasePriority = -100;
        
        [MenuItem(@"NatML/Hub " + HubSettings.Version, false, BasePriority)]
        private static void Version () { }

        [MenuItem(@"NatML/Hub " + HubSettings.Version, true, BasePriority)]
        private static bool DisableVersion () => false;

        [MenuItem(@"NatML/Get Access Key", false, BasePriority + 1)]
        private static void OpenAccessKey () => Help.BrowseURL(@"https://hub.natml.ai/profile");

        [MenuItem(@"NatML/Upgrade Billing Plan", false, BasePriority + 2)]
        public static void OpenBilling () => Help.BrowseURL(@"https://hub.natml.ai/billing");

        [MenuItem(@"NatML/Join Discord Community", false, BasePriority + 3)]
        private static void OpenDiscord () => Help.BrowseURL(@"https://discord.com/invite/y5vwgXkz2f");
    }
}