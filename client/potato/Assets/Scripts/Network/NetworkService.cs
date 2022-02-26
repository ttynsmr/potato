using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

namespace Potato
{
    namespace Network
    {
        public class NetworkService : MonoBehaviour
        {
            public Session Connect(string ip, int port)
            {
                session = new Session(new TcpClient(ip, port));
                return session;
            }

            public void StartReceive()
            {
                StartCoroutine(session.ReceivePayload());
            }

            public Session Session => session;

            private void Start()
            {

            }

            private void Update()
            {
                session.Update();
            }

            private Session session;
        }
    }
}
