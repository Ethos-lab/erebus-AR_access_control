/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

//#define NATML_HUB_STAGING

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.Hub.Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo(@"NatML.ML.Tests")]
namespace NatML.Hub {

    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.Networking;
    using Requests;

    /// <summary>
    /// NatML API.
    /// </summary>
    public static class NatMLHub {

        #region --Properties--
        /// <summary>
        /// NatML API URL.
        /// </summary>
        public const string URL =
        #if NATML_HUB_STAGING
        @"https://staging.api.natml.ai/graph";
        #else
        @"https://api.natml.ai/graph";
        #endif

        /// <summary>
        /// Current platform identifier.
        /// </summary>
        public static string CurrentPlatform => Application.platform.ToPlatform();
        #endregion


        #region --Mutations--
        /// <summary>
        /// Get the user info.
        /// </summary>
        /// <param name="accessKey">NatML access key.</param>
        /// <returns>Completed prediction.</returns>
        public static async Task<User> GetUser (string accessKey) {
            var response = await Request<GetUserRequest, GetUserResponse>(new GetUserRequest(), accessKey);
            return response.data.user;
        }

        /// <summary>
        /// Create an application token for use with NatML API's.
        /// </summary>
        /// <param name="input">Application token input.</param>
        /// <param name="accessKey">NatML access key.</param>
        public static async Task<string> CreateAppToken (
            CreateAppTokenRequest.Input input,
            string accessKey
        ) {
            var request = new CreateAppTokenRequest(input);
            var response = await Request<CreateAppTokenRequest, CreateAppTokenResponse>(request, accessKey);
            return response.data.createAppToken;
        }

        /// <summary>
        /// Create a prediction session.
        /// </summary>
        /// <param name="input">Session input.</param>
        /// <param name="accessKey">NatML access key.</param>
        /// <returns></returns>
        public static async Task<Session> CreateSession (CreateSessionRequest.Input input, string accessKey) {
            var request = new CreateSessionRequest(input);
            var response = await Request<CreateSessionRequest, CreateSessionResponse>(request, accessKey);
            return response.data.createSession;
        }

        /// <summary>
        /// Request an upload URL.
        /// </summary>
        /// <param name="name">File name.</param>
        /// <param name="type">Upload type.</param>
        /// <returns>Pre-signed upload URL.</returns>
        public static async Task<string> GetUploadURL (string name, string type = UploadType.Graph) {
            var input = new GetUploadURLRequest.Input {
                type = type,
                name = name,
            };
            var request = new GetUploadURLRequest(input);
            var response = await Request<GetUploadURLRequest, GetUploadURLResponse>(request);
            return response.data.uploadURL;
        }
        #endregion


        #region --Requests---
        /// <summary>
        /// Make a request to the NatML API.
        /// </summary>
        /// <typeparam name="TRequest">NatML API request.</typeparam>
        /// <param name="request">Request.</param>
        /// <param name="accessKey">Access key for requests that require authentication.</param>
        public static async Task Request<TRequest> (
            TRequest request,
            string accessKey = null
        ) where TRequest : GraphRequest => await Request<TRequest, GraphResponse>(request, accessKey);

        /// <summary>
        /// Make a request to the NatML API.
        /// </summary>
        /// <typeparam name="TRequest">NatML API request.</typeparam>
        /// <typeparam name="TResponse">NatML API response.</typeparam>
        /// <param name="request">Request.</param>
        /// <param name="accessKey">Access key for requests that require authentication.</param>
        /// <returns>Response.</returns>
        public static Task<TResponse> Request<TRequest, TResponse> (
            TRequest request,
            string accessKey = null
        ) where TRequest : GraphRequest where TResponse : GraphResponse {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                return RequestUnityWebRequest<TRequest, TResponse>(request, accessKey);
            else
                return RequestNet<TRequest, TResponse>(request, accessKey);
        }

        private static async Task<TResponse> RequestNet<TRequest, TResponse> (
            TRequest request,
            string accessKey = null
        ) where TRequest : GraphRequest where TResponse : GraphResponse {
            var payload = JsonUtility.ToJson(request);
            using var client = new HttpClient();
            using var content = new StringContent(payload, Encoding.UTF8, @"application/json");
            // Add auth token
            var authHeader = !string.IsNullOrEmpty(accessKey) ? new AuthenticationHeaderValue(@"Bearer", accessKey) : null;
            client.DefaultRequestHeaders.Authorization = authHeader;
            // Post
            using var response = await client.PostAsync(URL, content);
            // Parse
            var responseStr = await response.Content.ReadAsStringAsync();
            var responsePayload = JsonUtility.FromJson<TResponse>(responseStr);
            // Return
            if (responsePayload.errors == null)
                return responsePayload;
            else
                throw new InvalidOperationException(responsePayload.errors[0].message);
        }

        private static async Task<TResponse> RequestUnityWebRequest<TRequest, TResponse> (
            TRequest request,
            string accessKey = null
        ) where TRequest : GraphRequest where TResponse : GraphResponse {
            var payload = JsonUtility.ToJson(request);
            using var client = new UnityWebRequest(URL, UnityWebRequest.kHttpVerbPOST) {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload)),
                downloadHandler = new DownloadHandlerBuffer(),
                disposeDownloadHandlerOnDispose = true,
                disposeUploadHandlerOnDispose = true,
            };
            client.SetRequestHeader(@"Content-Type",  @"application/json");
            if (!string.IsNullOrEmpty(accessKey))
                client.SetRequestHeader("Authorization", $"Bearer {accessKey}");
            client.SendWebRequest();
            while (!client.isDone)
                await Task.Yield();
            var responseStr = client.downloadHandler.text;
            var response = JsonUtility.FromJson<TResponse>(responseStr);
            if (response.errors == null)
                return response;
            else
                throw new InvalidOperationException(response.errors[0].message);
        }
        #endregion


        #region --Utilities--
        /// <summary>
        /// Get the platform identifier for a given runtime platform.
        /// </summary>
        public static string ToPlatform (this RuntimePlatform platform) => platform switch {
            RuntimePlatform.Android         => Platform.Android,
            RuntimePlatform.IPhonePlayer    => Platform.iOS,
            RuntimePlatform.LinuxEditor     => Platform.Linux,
            RuntimePlatform.LinuxPlayer     => Platform.Linux,
            RuntimePlatform.OSXEditor       => Platform.macOS,
            RuntimePlatform.OSXPlayer       => Platform.macOS,
            RuntimePlatform.WebGLPlayer     => Platform.Web,
            RuntimePlatform.WindowsEditor   => Platform.Windows,
            RuntimePlatform.WindowsPlayer   => Platform.Windows,
            _                               => null,
        };

        /// <summary>
        /// Get the corresponding graph format for a given platform identifier.
        /// </summary>
        public static string FormatForPlatform (string platform) => platform switch {
            Platform.Android    => GraphFormat.TensorFlowLite,
            Platform.iOS        => GraphFormat.CoreML,
            Platform.macOS      => GraphFormat.CoreML,
            Platform.Web        => GraphFormat.ONNX,
            Platform.Windows    => GraphFormat.ONNX,
            _                   => null,
        };
        #endregion


        #region --DEPRECATED--
        [Obsolete(@"Deprecated in Hub 1.0.9")]
        public static Task<Prediction> RequestPrediction (RequestPredictionRequest.Input input) => default;

        [Obsolete(@"Deprecated in Hub 1.0.9")]
        public static Task<Prediction> WaitForPrediction (string prediction) => default;
        #endregion
    }
}