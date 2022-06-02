using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Potato
{
    namespace Network
    {
        namespace Protocol
        {
            public class Payload
            {
                public Payload(PayloadHeader header)
                {
                    Header = header;
                    SetBufferSize(Header.PayloadSize);
                }

                private void SetBufferSize(int size)
                {
                    Array.Resize(ref buffer, size);
                }

                public PayloadHeader Header { get; private set; }

                public byte[] GetBuffer() { return buffer; }

                private byte[] buffer;
            }
        }
    }
}
