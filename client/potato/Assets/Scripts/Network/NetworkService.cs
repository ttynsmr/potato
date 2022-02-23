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
                return new Session(new TcpClient(ip, port));
            }

            public Session Session => session;

            private void Start()
            {

            }

            private void Update()
            {

            }

            private Session session;
        }
    }
}