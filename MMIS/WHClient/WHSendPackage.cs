using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMIS
{
    class WHSendPackage
    {
        public ushort Head { get; set; }  //消息头
        public int SerialNumber { get; set; } //流水号
        public ushort WHPostion { get; set; }  //库位号
        public ushort TrayID { get; set; }   //托盘类型代号
    }
}
