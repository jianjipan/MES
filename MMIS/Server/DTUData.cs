using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMIS
{
    public class DTUData
    {
        public char Head { get; set; }
        public byte Ping { get; set; }
        public ushort Lenght { get; set; }
        public uint FID { get; set; }
        public byte Type { get; set; }
        public uint SID { get; set; }
        public ushort SendCount { get; set; }
        public byte[] Retain { get; set; }
        public byte Check { get; set; }
        public char End { get; set; }

        public override string ToString()
        {
            return string.Format("上行包头:{0},心跳包数据:{1},数据长度:{2},本终端ID:{3},目标类型:{4},转发终端ID:{5},发送包计数：{6},保留字段:{7},异或校验:{8},结束符号:{9}",
                Head, Ping, Lenght, FID, Type, SID, SendCount, Retain, Check, End);
        }
    }
   
}
