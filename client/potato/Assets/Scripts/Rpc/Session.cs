using System.Collections;
using System.Collections.Generic;
using Potato.Network.Protocol;
using UnityEngine;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace Potato
{
    namespace Network
    {
        public class Session
        {

            public Action<Session> OnDisconnected { get; set; }
            public Action<Payload> OnPayloadReceoved { get; set; }

            private CancellationTokenSource tokenSource = new CancellationTokenSource();
            private Task receieverTask;

            private TcpClient client;

            public Session(TcpClient client)
            {
                client.NoDelay = true;
                client.ReceiveBufferSize = 256 * 1024;
                this.client = client;
            }

            public void Disconnect()
            {
                Debug.Log("disconnecting");
                tokenSource?.Cancel();
                receieverTask?.Wait();
                client.Close();
                client.Dispose();
                Debug.Log("disconnected");
            }

            public void SendPayload(Payload payload)
            {
                // Debug.Log("sent: " + payload.GetBuffer().Length + " bytes");
                client.GetStream().Write(payload.GetBuffer(), 0, payload.GetBuffer().Length);
            }

            public void Start()
            {
                receieverTask = Task.Run(() =>
                {
                    while (!tokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            byte[] headerBuffer = new byte[PayloadHeader.Size];
                            //Debug.Log($"waiting read header {PayloadHeader.Size}bytes");
                            var readHeaderSize = client.GetStream().ReadAsync(headerBuffer, 0, PayloadHeader.Size, tokenSource.Token);
                            readHeaderSize.Wait(tokenSource.Token);
                            if (!client.Connected || readHeaderSize.Result == 0)
                            {
                                tokenSource.Cancel();
                                break;
                            }
                            Debug.Assert(readHeaderSize.Result == PayloadHeader.Size);

                            Payload payload = new Payload
                            {
                                Header = PayloadHeader.Deserialize(headerBuffer)
                            };
                            payload.SetBufferSize(payload.Header.payloadSize);
                            //Debug.Log($"waiting rayload header {payload.Header.payloadSize}bytes");
                            var readSize = client.GetStream().ReadAsync(payload.GetBuffer(), PayloadHeader.Size, payload.Header.payloadSize, tokenSource.Token);
                            readSize.Wait(tokenSource.Token);
                            if (!client.Connected || readSize.Result == 0)
                            {
                                tokenSource.Cancel();
                                break;
                            }
                            Debug.Assert(readSize.Result == payload.Header.payloadSize);

                            OnPayloadReceoved?.Invoke(payload);
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                }, tokenSource.Token);
                receieverTask.ContinueWith((task) => {
                    Debug.Log($"receieverTask was {task.Status}");
                    OnDisconnect();
                });
            }

            private void OnDisconnect()
            {
                OnDisconnected?.Invoke(this);
            }

            public void Update()
            {
            }
        }
    }
}
