using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ProtoBase;

namespace MMIS
{
    class WHSocketRequestFilter:FixedHeaderReceiveFilter<WHPackageInfo>
    {
        //消息头长度
        protected const int headsize=2;

        //构造函数
        public WHSocketRequestFilter()
            : base(headsize)
        {
 
        }

        //根据头得到数据头长度
        protected override int GetBodyLengthFromHeader(IBufferStream bufferStream, int length)
        {
            int datalength = 0;
            byte[] headByte = new byte[2];
            bufferStream.Read(headByte, 0, 2);
            int head = DataTransform.bytesToUshort(headByte, 0);
            if (head == 10004 || head == 20004 || head == 30004 || head == 40004||head==30000)
            {
                datalength = 6;
                return datalength;
            }
            if (head == 10005 || head == 20005 || head == 30005 || head == 40005)
            {
                datalength = 8;
                return datalength;
            }
            return datalength;
        }

        //解析数据
        public override WHPackageInfo ResolvePackage(IBufferStream bufferStream)
        {
            //byte CKCode = 0;  //校验位
            WHPackageInfo datapackage = new WHPackageInfo();
            List<byte> byteData = new List<byte>();
            byteData.Add(bufferStream.Buffers.Last().Array[0]);    //消息头
            byteData.Add(bufferStream.Buffers.Last().Array[1]);
            byteData.AddRange(bufferStream.Buffers.Last().Array.Skip(headsize).Take(GetBodyLengthFromHeader(bufferStream, 0)).ToArray()); //数据
            byte[] data = byteData.ToArray();
            //for (int i = 0; i < data.Length - 1; i++)  //校验前n-1个数据
            //{
            //    CKCode += data[i];
            //}
            if(true)   //始终校验通过
            {
                if (data.Length == 8)
                {
                    datapackage.Head = DataTransform.bytesToUshort(data, 0);  //头
                    datapackage.SerialNumber = DataTransform.bytesToInt(data, 2);  //流水号
                    datapackage.InlibMatchingID = DataTransform.bytesToUshort(data, 6);   //入库匹配号
                }
                else if (data.Length == 10)
                {
                    datapackage.Head = DataTransform.bytesToUshort(data, 0);  //头
                    datapackage.SerialNumber = DataTransform.bytesToInt(data, 2);  //流水号
                    datapackage.TrayStyle = DataTransform.bytesToUshort(data, 6);      //托盘类型代号
                    datapackage.OutlibMatchingID = DataTransform.bytesToUshort(data, 8);   //出库匹配号
                }
                else
                {
                    datapackage.Head = 0;  //头
                    datapackage.SerialNumber = 0;  //流水号
                    datapackage.TrayStyle = 0;      //托盘类型代号
                }             
            }
            return datapackage;
        }
    }
}
