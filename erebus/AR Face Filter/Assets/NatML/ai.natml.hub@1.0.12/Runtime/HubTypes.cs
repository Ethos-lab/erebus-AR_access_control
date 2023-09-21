/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub {

    using System;

    #region --Enumerations--
    /// <summary>
    /// Development framework.
    /// </summary>
    public static class Framework {
        /// <summary>
        /// Unity Engine.
        /// </summary>
        public const string Unity = "UNITY";
    }

    /// <summary>
    /// Graph format.
    /// </summary>
    public static class GraphFormat {
        /// <summary>
        /// Apple CoreML.
        /// </summary>
        public const string CoreML = @"COREML";
        /// <summary>
        /// Open Neural Network Exchange.
        /// </summary>
        public const string ONNX = @"ONNX";
        /// <summary>
        /// TensorFlow Lite.
        /// </summary>
        public const string TensorFlowLite = @"TFLITE";
    }

    /// <summary>
    /// Device platform.
    /// </summary>
    public static class Platform {
        /// <summary>
        /// Android.
        /// </summary>
        public const string Android = @"ANDROID";
        /// <summary>
        /// iOS.
        /// </summary>
        public const string iOS = @"IOS";
        /// <summary>
        /// Linux.
        /// </summary>
        public const string Linux = @"LINUX";
        /// <summary>
        /// macOS.
        /// </summary>
        public const string macOS = @"MACOS";
        /// <summary>
        /// Browser or other Web platform.
        /// </summary>
        public const string Web = @"WEB";
        /// <summary>
        /// Windows.
        /// </summary>
        public const string Windows = @"WINDOWS";
    }

    /// <summary>
    /// Predictor status.
    /// </summary>
    public static class PredictorStatus {
        /// <summary>
        /// Predictor is a draft.
        /// Predictor can only be viewed and used by author.
        /// </summary>
        public const string Draft = @"DRAFT";
        /// <summary>
        /// Predictor is pending review.
        /// Predictor can only be viewed and used by author.
        /// </summary>
        public const string Pending = @"PENDING";
        /// <summary>
        /// Predictor is in review.
        /// Predictor can be viewed and used by owner or NatML predictor review team.
        /// </summary>
        public const string Review = @"REVIEW";
        /// <summary>
        /// Predictor has been published.
        /// Predictor viewing and fetching permissions are dictated by the access mode.
        /// </summary>
        public const string Published = @"PUBLISHED";
        /// <summary>
        /// Predictor is archived.
        /// Predictor can be viewed but cannot be used by anyone including owner.
        /// </summary>
        public const string Archived = @"ARCHIVED";
    }

    /// <summary>
    /// Upload URL type.
    /// </summary>
    public static class UploadType {
        /// <summary>
        /// Model graph.
        /// </summary>
        public const string Graph = @"GRAPH";

        [Obsolete(@"Deprecated in Hub 1.0.9")]
        public const string Feature = @"FEATURE";
    }
    #endregion


    #region --Structs--
    [Serializable]
    public sealed class Normalization {
        public float[] mean;
        public float[] std;
    }

    [Serializable]
    public sealed class AudioFormat {
        public int sampleRate;
        public int channelCount;
    }

    [Serializable]
    public sealed class Session {
        public string id;
        public Predictor predictor;
        public string platform;
        public string graph;
        public string format;
        public int flags;
    }

    [Serializable]
    public sealed class Predictor {
        public string tag;
        public string status;
        public string aspectMode;
        public string[] labels;
        public Normalization normalization;
        public AudioFormat audioFormat;

        [Obsolete(@"Deprecated in Hub 1.0.9")]
        public string type;
    }

    [Serializable]
    public sealed class User {
        public string email;
        public string username;
        public Billing billing;
    }

    [Serializable]
    public sealed class Billing {
        public string plan;
    }
    #endregion


    #region --DEPRECATED--
    [Obsolete(@"Deprecated in Hub 1.0.9")]
    public static class DataType {
        public const string Float32 = @"FLOAT32";
        public const string Float64 = @"FLOAT64";
        public const string Int8 = @"INT8";
        public const string Int16 = @"INT16";
        public const string Int32 = @"INT32";
        public const string Int64 = @"INT64";
        public const string UInt8 = @"UINT8";
        public const string UInt16 = @"UINT16";
        public const string UInt32 = @"UINT32";
        public const string UInt64 = @"UINT64";
        public const string String = @"STRING";
        public const string Image = @"IMAGE";
        public const string Video = @"VIDEO";
        public const string Audio =  @"AUDIO";
        public const string Binary = @"BINARY";
    }

    [Obsolete(@"Deprecated in Hub 1.0.9")]
    public static class PredictorType {
        public const string Edge = @"EDGE";
        public const string Cloud = @"CLOUD";
    }

    [Obsolete(@"Deprecated in Hub 1.0.9")]
    public static class PredictionStatus {
        public const string Waiting = @"WAITING";
        public const string Processing = @"PROCESSING";
        public const string Completed = @"COMPLETED";
    }

    [Serializable, Obsolete(@"Deprecated in Hub 1.0.9")]
    public sealed class Prediction {
        public string id;
        public string status;
        public Feature[] results;
        public string error;
    }

    [Serializable, Obsolete(@"Deprecated in Hub 1.0.9")]
    public sealed class Feature {
        public string type;
        public string data;
        public int[] shape;
    }
    #endregion
}