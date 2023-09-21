/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub.Requests {

    using System;

    [Serializable]
    public sealed class GetUploadURLRequest : GraphRequest {

        public Variables variables;

        public GetUploadURLRequest (Input input) : base(@"
            query ($input: UploadURLInput!) {
                uploadURL (input: $input)
            }
        ") => this.variables = new Variables { input = input };

        [Serializable]
        public sealed class Variables {
            public Input input;
        }

        [Serializable]
        public sealed class Input {
            public string name;
            public string type;
        }
    }

    [Serializable]
    public sealed class GetUploadURLResponse : GraphResponse {

        public ResponseData data;

        [Serializable]
        public sealed class ResponseData {
            public string uploadURL;
        }
    }
}