using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.ProtoBase;

namespace MMIS
{
    public class WHPackageInfo:IPackageInfo<byte>
    {
        //Flag   //实现IPackageInfo接口
        public byte Flag { get; set; }
        public byte Key
        {
            get { return Flag; }
        }

        //消息头
        public ushort Head { get; set; }

        //流水号
        public int  SerialNumber { get; set; }


        //入库匹配号
        public int InlibMatchingID { get; set; }

        //出库匹配号
        public int OutlibMatchingID { get; set; }

        //托盘类型代号
        public ushort TrayStyle { get; set; }

    }
}
