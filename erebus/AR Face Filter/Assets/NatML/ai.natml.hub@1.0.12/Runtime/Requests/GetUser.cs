/* 
*   NatML Hub
*   Copyright (c) 2022 NatML Inc. All rights reserved.
*/

namespace NatML.Hub.Requests {

    using System;

    [Serializable]
    public sealed class GetUserRequest : GraphRequest {

        public GetUserRequest () : base(@"
            query {
                user {
                    email
                    username
                    billing { plan }
                }
            }
        ") { }
    }

    [Serializable]
    public sealed class GetUserResponse : GraphResponse {

        public ResponseData data;

        [Serializable]
        public sealed class ResponseData {
            public User user;
        }
    }
}