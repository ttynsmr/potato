using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

namespace Potato
{
    namespace Network
    {
        public class NetworkService : MonoBehaviour
        {
            public Session Connect(string ip, int port)
            {
                session = new Session(new TcpClient(ip, port));
                session.OnDisconnectedCallback += OnDisconnected;
                return session;
            }

            public void StartReceive()
            {
                session.Start();
                OnConnectedCallback?.Invoke(session);
            }

            private void OnDisconnected(Session session)
            {
                OnDisconnectedCallback?.Invoke(session);
                this.session = null;
            }

            public Session Session => session;

            private void Start()
            {

            }

            private void Update()
            {
                session?.Update();
            }

            private void OnDestroy()
            {
                session?.Disconnect();
            }

            public long Now {
                get
                {
                    return Math.Max(LastSyncronizedTime, (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds + ServerTimeDifference);
                }
            }

            private long serverTimeDifference;
            public long ServerTimeDifference {
                get { return serverTimeDifference; }
                set
                {
                    serverTimeDifference = value;
                    LastSyncronizedTime = Now;
                }
            }
            public long LastSyncronizedTime { get; private set; }
            public Action<Session> OnConnectedCallback { get; set; }
            public Action<Session> OnDisconnectedCallback { get; set; }

            public readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            private Session session;
        }
    }
}
