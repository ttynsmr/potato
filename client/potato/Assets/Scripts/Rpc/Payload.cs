using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Potato
{
    namespace Network
    {
        namespace Protocol
        {
            public enum Meta : byte
            {
                Request = 0,
                Response = 1,
                Notification = 2
            };

            public class PayloadHeader
            {
                public ushort payloadSize = 0;
                public byte version = 0;
                public byte meta = 0;
                public ushort contract_id = 0;
                public ushort rpc_id = 0;

                public static int Size => sizeof(ushort) + sizeof(byte) + sizeof(byte) + sizeof(ushort) + sizeof(ushort);

                public PayloadHeader Deserialize(byte[] data)
                {
                    payloadSize = BitConverter.ToUInt16(data, 0);
                    version = data[2];
                    meta = data[3];
                    contract_id = BitConverter.ToUInt16(data, 4);
                    rpc_id = BitConverter.ToUInt16(data, 6);
                    return this;
                }
            };

            public class Payload
            {
                public Payload()
                {
                    buffer = new byte[PayloadHeader.Size];
                }

                public void SetBufferSize(int size)
                {
                    Array.Resize(ref buffer, PayloadHeader.Size + size);
                }

                public PayloadHeader GetHeader()
                {
                    return new PayloadHeader().Deserialize(buffer);
                }

                // std::byte* getPayloadData() { return &buffer[sizeof(PayloadHeader)]; };
                // const std::byte* getPayloadData() const { return &buffer[sizeof(PayloadHeader)]; };

                public byte[] GetBuffer() { return buffer; }

                private byte[] buffer;
            }
        }
    }
}
