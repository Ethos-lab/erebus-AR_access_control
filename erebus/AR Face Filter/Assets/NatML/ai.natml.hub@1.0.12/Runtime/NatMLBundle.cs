/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub {

    using System;

    /// <summary>
    /// Specify a final app bundle identifier that will be used to validate app tokens.
    ///
    /// This attribute is intended for use when the app identifier in Unity is different from the final
    /// app identifier at runtime, like when Unity is embedded within another app. NatML API's always 
    /// validate app tokens against the app identifier at runtime, so this attribute can be used to 
    /// override the app identifier used for validation.
    ///
    /// On web platforms, the final web URL of the app should be used. This corresponds to the 
    /// `window.location.hostname` variable in the browser.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class NatMLBundleAttribute : Attribute {

        #region --Client API--
        /// <summary>
        /// App bundle identifier.
        /// </summary>
        public readonly string identifier;

        /// <summary>
        /// Specify an app bundle override.
        /// </summary>
        /// <param name="identifier">Final app bundle identifier.</param>
        public NatMLBundleAttribute (string identifier) => this.identifier = identifier;
        #endregion
    }
}