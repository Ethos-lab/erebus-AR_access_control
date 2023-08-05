/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices {

    using System;
    using System.Linq;

    /// <summary>
    /// Common criteria used to filter devices.
    /// </summary>
    public static class MediaDeviceCriteria {

        #region --Media Type--
        /// <summary>
        /// Filter for audio devices.
        /// </summary>
        public static readonly Predicate<IMediaDevice> AudioDevice = device => device is AudioDevice;

        /// <summary>
        /// Filter for camera devices.
        /// </summary>
        public static readonly Predicate<IMediaDevice> CameraDevice = device => device is CameraDevice;
        #endregion


        #region --Device Location and Type--
        /// <summary>
        /// Filter for internal devices.
        /// </summary>
        public static readonly Predicate<IMediaDevice> Internal = device => device.location == DeviceLocation.Internal;

        /// <summary>
        /// Filter for external devices.
        /// </summary>
        public static readonly Predicate<IMediaDevice> External = device => device.location == DeviceLocation.External;

        /// <summary>
        /// Filter for default devices for their respective media types.
        /// </summary>
        public static readonly Predicate<IMediaDevice> Default = device => device.defaultForMediaType;
        #endregion


        #region --AudioDevice--
        /// <summary>
        /// Filter for audio devices that perform echo cancellation.
        /// </summary>
        public static readonly Predicate<IMediaDevice> EchoCancellation = device => device is AudioDevice microphone && microphone.echoCancellationSupported;
        #endregion
        

        #region --CameraDevice--
        /// <summary>
        /// Filter for rear-facing camera devices.
        /// </summary>
        public static readonly Predicate<IMediaDevice> RearCamera = device => device is CameraDevice camera && !camera.frontFacing;

        /// <summary>
        /// Filter for front-facing camera devices.
        /// </summary>
        public static readonly Predicate<IMediaDevice> FrontCamera = device => device is CameraDevice camera && camera.frontFacing;

        /// <summary>
        /// Filter for camera devices that can stream depth images.
        /// </summary>
        internal static readonly Predicate<IMediaDevice> DepthCamera = device => device is CameraDevice camera && camera.depthStreamingSupported;
        
        /// <summary>
        /// Filter for camera devices that have a torch unit.
        /// </summary>
        public static readonly Predicate<IMediaDevice> Torch = device => device is CameraDevice camera && camera.torchSupported;
        #endregion


        #region --Utilities--
        /// <summary>
        /// Filter for devices that meet any of the provided criteria.
        /// </summary>
        /// <param name="criteria">Criteria to meet.</param>
        public static Predicate<IMediaDevice> Any (params Predicate<IMediaDevice>[] criteria) => device => criteria.Any(c => c(device));

        /// <summary>
        /// Filter for devices that meet all of the provided criteria.
        /// </summary>
        /// <param name="criteria">Criteria to meet.</param>
        public static Predicate<IMediaDevice> All (params Predicate<IMediaDevice>[] criteria) => device => criteria.All(c => c(device));
        #endregion
    }
}