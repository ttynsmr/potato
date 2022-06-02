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
            public Action<Payload> OnPayloadReceived { get; set; }

            private CancellationTokenSource tokenSource = new();
            private UniTask receiverTask;

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
                Debug.Log(receiverTask.ToString());
                Debug.Log("disconnected");
                receiverTask.Forget();
            }

            public void SendPayload(Payload payload)
            {
                // Debug.Log("sent: " + payload.GetBuffer().Length + " bytes");
                client.GetStream().Write(BitConverter.GetBytes((ushort)payload.Header.CalculateSize()), 0, 2);
                Debug.Log($"payload header size: {payload.Header.CalculateSize()}");
                var buffer = new byte[payload.Header.CalculateSize()];
                var buf = new Google.Protobuf.CodedOutputStream(buffer);
                payload.Header.WriteTo(buf);
                buf.Flush();
                client.GetStream().Write(buffer, 0, buffer.Length);
                buf.Dispose();
                client.GetStream().Write(payload.GetBuffer(), 0, payload.GetBuffer().Length);
            }

            public void Start()
            {
                receiverTask = UniTask.RunOnThreadPool(async () =>
                {
                    while (!tokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            byte[] payloadSizeBuffer = new byte[2];
                            var readPayloadSizeBufferSize = await client.GetStream().ReadAsync(payloadSizeBuffer, 0, 2, tokenSource.Token);
                            if (!client.Connected || readPayloadSizeBufferSize == 0 || tokenSource.IsCancellationRequested)
                            {
                                await OnDisconnect();
                                break;
                            }
                            Debug.Assert(readPayloadSizeBufferSize == 2);

                            var payloadSize = BitConverter.ToUInt16(payloadSizeBuffer, 0);

                            byte[] headerBuffer = new byte[payloadSize];
                            //Debug.Log($"waiting read header {PayloadHeader.Size}bytes");
                            var readPayloadHeaderSize = await client.GetStream().ReadAsync(headerBuffer, 0, payloadSize, tokenSource.Token);
                            if (!client.Connected || readPayloadHeaderSize == 0 || tokenSource.IsCancellationRequested)
                            {
                                await OnDisconnect();
                                break;
                            }
                            Debug.Assert(readPayloadHeaderSize == payloadSize);

                            var payload = new Payload(PayloadHeader.Parser.ParseFrom(headerBuffer));
                            //Debug.Log($"waiting rayload header {payload.Header.payloadSize}bytes");
                            var readSize = await client.GetStream().ReadAsync(payload.GetBuffer(), 0, payload.Header.PayloadSize, tokenSource.Token);
                            if (!client.Connected || readSize == 0 || tokenSource.IsCancellationRequested)
                            {
                                await OnDisconnect();
                                break;
                            }
                            Debug.Assert(readSize == payload.Header.PayloadSize);

                            OnPayloadReceived?.Invoke(payload);
                        }
                        catch (OperationCanceledException e)
                        {
                            Debug.Log(e.ToString());
                            Debug.Log($"receiverTask was {receiverTask.Status}");
                            await OnDisconnect();
                        }
                        catch(Exception e)
                        {
                            Debug.LogException(e);
                            Debug.Log($"receiverTask was {receiverTask.Status}");
                            await OnDisconnect();
                        }
                    }
                });
            }

            private async UniTask OnDisconnect()
            {
                await UniTask.SwitchToMainThread();
                OnDisconnectedCallback?.Invoke(this);
                client.Close();
                client.Dispose();
                client = null;
                Disconnect();
                await UniTask.SwitchToThreadPool();
            }

            public void Update()
            {
            }
        }
    }
}
