/* 
*   NatDevice
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Devices.Outputs {

    using System;

    /// <summary>
    /// Media device output which consumes sample buffers from media devices.
    /// </summary>
    public interface IMediaOutput<TSampleBuffer> : IDisposable {
        
        /// <summary>
        /// Update the output with a new sample buffer.
        /// </summary>
        /// <param name="sampleBuffer">Sample buffer.</param>
        void Update (TSampleBuffer sampleBuffer);
    }
}