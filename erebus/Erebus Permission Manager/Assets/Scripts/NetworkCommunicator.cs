using NetMQ;
using NetMQ.Sockets;
using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace Networking
{
    public class NetworkCommunicator
    {
        private string ipAddr = "*";
        private string portNum = "12345";
        private TimeSpan timeout = default;
        public NetworkCommunicator(TimeSpan timeout, string ipAddr = "*", string portNum = "12345")
        {
            this.ipAddr = ipAddr;
            this.portNum = portNum;
            this.timeout = timeout;
        }
        private byte[] Decompress(byte[] inputBytes)
        {
            using (MemoryStream ouputStream = new MemoryStream())
            {
                using (MemoryStream inputStream = new MemoryStream(inputBytes))
                {
                    using (var decompressor = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        decompressor.CopyTo(ouputStream);
                        return ouputStream.ToArray();
                    }
                }
            }
        }
        public string SendCode(string inputNLString)
        {
            AsyncIO.ForceDotNet.Force();
            using (var socket = new RequestSocket($"tcp://{ipAddr}:{portNum}"))
            {
                try
                {
                    socket.SendFrame(inputNLString);
                    if (socket.TryReceiveFrameString(timeout, out string erebusCodeString))
                        return erebusCodeString;
                    else
                        Debug.Log("Client timeout");
                }
                finally
                {
                    socket.Close();
                    NetMQConfig.Cleanup();
                }
            }
            return null;
        }
    }
}