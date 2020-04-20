using System.Linq;
using SuperSocket.ProtoBase;
using System.Collections.Generic;

namespace MMIS
{
    public class SocketRequestFilter:FixedHeaderReceiveFilter<PackageInfo>
    {
        //消息头长度
        protected const int headerSize = 2;

        //构造函数
        public SocketRequestFilter()
            : base(headerSize)
        {
 
        }

        //根据头得到数据的长度
        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            int dataLength = 0;
            byte[] headByte = new byte[2];
            bufferStream.Read(headByte, 0, 2);
            int head = DataTransform.bytesToUshort(headByte, 0);
            if (head == 10003)
            {
                dataLength = 7;
                return dataLength;
            }
            if (head == 10005)
            {
                dataLength = 9;
                return dataLength;
            }
            return dataLength;
        }

        //解析数据
        public override PackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            byte CKCode = 0; //校验位
            PackageInfo datapackage = new PackageInfo();
            List<byte> byteData = new List<byte>();
            byteData.Add(bufferStream.Buffers.Last().Array[0]);   //头（10003/10005）
            byteData.Add(bufferStream.Buffers.Last().Array[1]);
            byteData.AddRange(bufferStream.Buffers.Last().Array.Skip(headerSize).Take(GetBodyLengthFromHeader(bufferStream, 0)).ToArray());//数据
            byte[] data = byteData.ToArray();
            for (int i = 0; i <data.Length-1; i++)  //校验前n-1个数据
            {
                CKCode += data[i];
            }
            if (data[data.Length-1] == CKCode)   //如果校验通过
            {
                if (data.Length == 9)
                {
                    datapackage.Head = DataTransform.bytesToUshort(data, 0);
                    datapackage.TaskID = DataTransform.bytesToInt(data, 2);
                    datapackage.TaskStepID = DataTransform.bytesToUshort(data, 6);
                }
                else if (data.Length == 11)
                {
                    datapackage.Head = DataTransform.bytesToUshort(data, 0);
                    datapackage.TaskID = DataTransform.bytesToInt(data, 2);
                    datapackage.TaskStepID = DataTransform.bytesToUshort(data, 6);
                    datapackage.CarID = DataTransform.bytesToUshort(data, 8);
                }
                else
                {
                    datapackage.Head = 0;
                    datapackage.TaskID = 0;
                    datapackage.TaskStepID = 0;
                    datapackage.CarID = 0;
                }
            }

            return datapackage;
        }

    }
}