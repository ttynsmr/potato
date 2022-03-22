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

                public static PayloadHeader Deserialize(byte[] data)
                {
                    PayloadHeader header = new PayloadHeader();
                    header.payloadSize = BitConverter.ToUInt16(data, 0);
                    header.version = data[2];
                    header.meta = data[3];
                    header.contract_id = BitConverter.ToUInt16(data, 4);
                    header.rpc_id = BitConverter.ToUInt16(data, 6);
                    return header;
                }

                public void SerializeTo(byte[] buffer)
                {
                    byte[] payloadSizeBuffer = BitConverter.GetBytes(payloadSize);
                    buffer[0] = payloadSizeBuffer[0];
                    buffer[1] = payloadSizeBuffer[1];
                    buffer[2] = version;
                    buffer[3] = meta;
                    byte[] contractIdBuffer = BitConverter.GetBytes(contract_id);
                    buffer[4] = contractIdBuffer[0];
                    buffer[5] = contractIdBuffer[1];
                    byte[] rpcIdBuffer = BitConverter.GetBytes(rpc_id);
                    buffer[6] = rpcIdBuffer[0];
                    buffer[7] = rpcIdBuffer[1];
                }

                public override string ToString()
                {
                    return $"payloadSize:{payloadSize} version:{version} meta:{meta} contract_id:{contract_id} rpc_id:{rpc_id}";
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
                    Header.payloadSize = (ushort)size;
                }

                public PayloadHeader Header { get; set; } = new PayloadHeader();

                public byte[] GetBuffer() { return buffer; }

                private byte[] buffer;
            }
        }
    }
}
