/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub.Requests {

    using System;

    [Serializable, Obsolete(@"Deprecated in Hub 1.0.9")]
    public sealed class RequestPredictionRequest : GraphRequest {

        public Variables variables;

        public RequestPredictionRequest (Input input) : base(@"
            mutation ($input: RequestPredictionInput!) {
                requestPrediction (input: $input) {
                    id
                    status
                    results { data type shape }
                    error
                }
            }
        ") => this.variables = new Variables { input = input };

        [Serializable]
        public sealed class Variables {
            public Input input;
        }

        [Serializable]
        public sealed class Input {
            public string session;
            public Feature[] inputs;
            public bool waitUntilCompleted;
        }
    }

    [Serializable, Obsolete(@"Deprecated in Hub 1.0.9")]
    public sealed class RequestPredictionResponse : GraphResponse {

        public ResponseData data;

        [Serializable]
        public sealed class ResponseData {
            public Prediction requestPrediction;
        }
    }
}