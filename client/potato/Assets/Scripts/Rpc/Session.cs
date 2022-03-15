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
            public Session(TcpClient client)
            {
                client.NoDelay = true;
                this.client = client;
                rpcs = Torikime.RpcBuilder.Build(this);
            }

            public void Disconnect()
            {
                Debug.Log("disconnecting");
                tokenSource.Cancel();
                receieverTask.Wait();
                rpcs.Clear();
                client.Close();
                client.Dispose();
                Debug.Log("disconnected");
            }

            public void SendPayload(Payload payload)
            {
                // Debug.Log("sent: " + payload.GetBuffer().Length + " bytes");
                client.GetStream().Write(payload.GetBuffer(), 0, payload.GetBuffer().Length);
            }

            public T GetRpc<T>() where T : Torikime.IRpc
            {
                return (T)rpcs.Find(x => x is T);
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
                            var readSize = client.GetStream().ReadAsync(payload.GetBuffer(), PayloadHeader.Size, payload.Header.payloadSize, tokenSource.Token);
                            readSize.Wait(tokenSource.Token);
                            if (!client.Connected || readSize.Result == 0)
                            {
                                tokenSource.Cancel();
                                break;
                            }
                            Debug.Assert(readSize.Result == payload.Header.payloadSize);

                            var rpc = rpcs.Find(x => x.ContractId == payload.Header.contract_id
                             && x.RpcId == payload.Header.rpc_id);

                            if (rpc.ContractId == Torikime.Diagnosis.PingPong.Rpc.StaticContractId
                            && rpc.RpcId == Torikime.Diagnosis.PingPong.Rpc.StaticRpcId)
                            {
                                try
                                {
                                    rpc.ReceievePayload(payload);
                                }
                                catch (Exception)
                                {
                                    if (!client.Connected)
                                    {
                                        tokenSource.Cancel();
                                        break;
                                    }
                                }
                                continue;
                            }

                            if (rpc != null)
                            {
                                queue.Enqueue(() => { rpc.ReceievePayload(payload); });
                            }
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
                while (queue.TryDequeue(out var action))
                {
                    action();
                }
            }

            public Action<Session> OnDisconnected { get; set; }

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task receieverTask;
            private ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();


            private List<Torikime.IRpc> rpcs;

            private TcpClient client;
        }
    }
}
