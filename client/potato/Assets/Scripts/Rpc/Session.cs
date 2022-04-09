using Potato.Network.Protocol;
using UnityEngine;
using System.Net.Sockets;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Potato
{
    namespace Network
    {
        public class Session
        {

            public Action<Session> OnDisconnectedCallback { get; set; }
            public Action<Payload> OnPayloadReceoved { get; set; }

            private CancellationTokenSource tokenSource = new CancellationTokenSource();
            private UniTask receieverTask;

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
                tokenSource.Cancel();
                //receieverTask.AsTask().Wait();
                Debug.Log(receieverTask.ToString());
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
                receieverTask = UniTask.RunOnThreadPool(async () =>
                {
                    while (!tokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            byte[] headerBuffer = new byte[PayloadHeader.Size];
                            //Debug.Log($"waiting read header {PayloadHeader.Size}bytes");
                            var readHeaderSize = await client.GetStream().ReadAsync(headerBuffer, 0, PayloadHeader.Size, tokenSource.Token);
                            if (!client.Connected || readHeaderSize == 0 || tokenSource.IsCancellationRequested)
                            {
                                tokenSource.Cancel();
                                await OnDisconnect();
                                break;
                            }
                            Debug.Assert(readHeaderSize == PayloadHeader.Size);

                            Payload payload = new Payload
                            {
                                Header = PayloadHeader.Deserialize(headerBuffer)
                            };
                            payload.SetBufferSize(payload.Header.payloadSize);
                            //Debug.Log($"waiting rayload header {payload.Header.payloadSize}bytes");
                            var readSize = await client.GetStream().ReadAsync(payload.GetBuffer(), PayloadHeader.Size, payload.Header.payloadSize, tokenSource.Token);
                            if (!client.Connected || readSize == 0 || tokenSource.IsCancellationRequested)
                            {
                                tokenSource.Cancel();
                                await OnDisconnect();
                                break;
                            }
                            Debug.Assert(readSize == payload.Header.payloadSize);

                            OnPayloadReceoved?.Invoke(payload);
                        }
                        catch (OperationCanceledException e)
                        {
                            Debug.Log(e.ToString());
                            Debug.Log($"receieverTask was {receieverTask.Status}");
                            await OnDisconnect();
                        }
                        catch(Exception e)
                        {
                            Debug.LogException(e);
                            Debug.Log($"receieverTask was {receieverTask.Status}");
                            await OnDisconnect();
                        }
                    }
                }, true, tokenSource.Token).ContinueWith(async () => {
                    Debug.Log($"receieverTask was {receieverTask.Status}");
                    await OnDisconnect();
                });
            }

            private async UniTask OnDisconnect()
            {
                await UniTask.SwitchToMainThread();
                OnDisconnectedCallback?.Invoke(this);
                await UniTask.SwitchToThreadPool();
            }

            public void Update()
            {
            }
        }
    }
}
