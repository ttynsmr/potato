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
                        byte[] headerBuffer = new byte[PayloadHeader.Size];
                        var readHeaderSize = client.GetStream().Read(headerBuffer, 0, PayloadHeader.Size);
                        Debug.Assert(readHeaderSize == PayloadHeader.Size);

                        Payload payload = new Payload
                        {
                            Header = PayloadHeader.Deserialize(headerBuffer)
                        };
                        payload.SetBufferSize(payload.Header.payloadSize);
                        var readSize = client.GetStream().Read(payload.GetBuffer(), PayloadHeader.Size, payload.Header.payloadSize);
                        Debug.Assert(readSize == payload.Header.payloadSize);

                        var rpc = rpcs.Find(x => x.ContractId == payload.Header.contract_id
                         && x.RpcId == payload.Header.rpc_id);

                        if (rpc != null)
                        {
                            queue.Enqueue(() => { rpc.ReceievePayload(payload); });
                        }
                    }
                }, tokenSource.Token);
                receieverTask.ContinueWith(((task) => { Debug.Log($"receieverTask was {task.Status}"); }));
            }

            public void Update()
            {
                while (queue.TryDequeue(out var action))
                {
                    action();
                }
            }

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            Task receieverTask;
            private ConcurrentQueue<Action> queue = new ConcurrentQueue<Action>();

            private List<Torikime.IRpc> rpcs;

            private TcpClient client;
        }
    }
}
