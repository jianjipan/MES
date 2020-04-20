using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.Common;
using SuperSocket.Facility.Protocol;

namespace MMIS
{
    public class DTUFixedHeaderReceiveFilter:FixedHeaderReceiveFilter<DTURequestInfo>
    {
        //消息头长度
        protected const int headerSize = 20;

        //构造函数
        public DTUFixedHeaderReceiveFilter()
            : base(headerSize)
        {
           
        }

        //获取消息头
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            int len = 130;
            return len;
        }

        //解析消息
        protected override DTURequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            DTURequestInfo request = new DTURequestInfo();
            List<byte> bytesource = new List<byte>();
            bytesource.AddRange(header.Array);
            bytesource.AddRange(bodyBuffer.CloneRange(offset, length));
            byte[] data = bytesource.ToArray();
            //消息校验
            if (headCKfunc(data))  //校验通过
            {
                //是加工区PLC发来的消息
                if (data[5] == 0x02 && data[6] == 0x0B && data[7] == 0x00 && data[8] == 0x01)
                {
                    if (data[20] == 0x29 && data[21] == 0xFF)  //心跳位校验
                    {
                        request.CmdKey = 1;   //代表加工区PLC发来的命令
                        request.HeadBeat = true;   //心跳正常
                        request.Raw_Tray_Empty = data[39];  //毛胚托盘取完标志
                        request.SystemState = data[23];   //系统状态位
                        request.SystemControl = data[25];   //系统控制位   0急停 1 急停
                        request.PartState = data[40];    //各机构状态
                        request.AGVExecuteCmd = data[41];   //需要AGV取料
                        request.Manual_Up_RFID = data[42];   // 人工上料AGV对接台RFID识别托盘类型代号
                        Array.Reverse(data, 46, 4);
                        request.Manual_Up_CD_Number = BitConverter.ToSingle(data,46);
                        request.Manual_Down_RFID = data[68];  //人工下料AGV对接台RFID识别托盘类型代号
                        Array.Reverse(data, 72, 4);
                        request.Manual_Down_CD_Number = BitConverter.ToSingle(data,72);
                        request.Manual_Down_Para1 = BitConverter.ToSingle(data, 72);
                        Array.Reverse(data, 76, 4);
                        request.Manual_Down_Para2 = BitConverter.ToSingle(data, 76);
                        Array.Reverse(data, 80, 4);
                        request.Manual_Down_Para3 = BitConverter.ToSingle(data, 80);
                        Array.Reverse(data, 84, 4);
                        request.Manual_Down_Para4 = BitConverter.ToSingle(data, 84);
                        Array.Reverse(data, 88, 4);
                        request.Manual_Down_Para5= BitConverter.ToSingle(data, 88);
                        request.Process_Area1_RFID = data[94]; //加工区AGV对接台1RFID识别托盘类型代号
                        request.Process_Area2_RFID = data[120]; //加工区AGV对接台2RFID识别托盘类型代号
                        return request;
                    }
                    else
                    {
                        return request;
                    }
                }
                //是检测区PLC发来的消息
                else if (data[5] == 0x02 && data[6] == 0x0B && data[7] == 0x00 && data[8] == 0x02)
                {
                    if (data[20] == 0x28 && data[21] == 0xEE)
                    {
                        request.CmdKey = 2;
                        request.HeadBeat = true;
                        request.SystemState = data[23];
                        request.PartState = data[40];
                        request.AGVExecuteCmd = data[41];
                        request.Detection_Area1_RFID = data[42];
                        Array.Reverse(data, 46, 4);
                        request.Detection_Area1_Parameter1 = BitConverter.ToSingle(data, 46);
                        Array.Reverse(data, 50, 4);
                        request.Detection_Area1_Parameter2 = BitConverter.ToSingle(data, 50);
                        Array.Reverse(data, 54, 4);
                        request.Detection_Area1_Parameter3 = BitConverter.ToSingle(data, 54);
                        Array.Reverse(data, 58, 4);
                        request.Detection_Area1_Parameter4 = BitConverter.ToSingle(data, 58);
                        Array.Reverse(data, 62, 4);
                        request.Detection_Area1_Parameter5 = BitConverter.ToSingle(data, 62);
                        request.Detection_Area2_RFID = data[68];
                        Array.Reverse(data, 72, 4);
                        request.Detection_Area2_Parameter1 = BitConverter.ToSingle(data, 72);
                        Array.Reverse(data, 76, 4);
                        request.Detection_Area2_Parameter2 = BitConverter.ToSingle(data, 76);
                        Array.Reverse(data, 80, 4);
                        request.Detection_Area2_Parameter3 = BitConverter.ToSingle(data, 80);
                        Array.Reverse(data, 84, 4);
                        request.Detection_Area2_Parameter4 = BitConverter.ToSingle(data, 84);
                        Array.Reverse(data, 88, 4);
                        request.Detection_Area2_Parameter5 = BitConverter.ToSingle(data, 88);
                        return request;
                    }
                    else
                    {
                        return request;
                    }
                }
                //是装配区PLC发来的消息
                else if (data[5] == 0x02 && data[6] == 0x0B && data[7] == 0x00 && data[8] == 0x03)
                {
                    if (data[20] == 0x27 && data[21] == 0xDD)
                    {
                        request.CmdKey = 3;
                        request.HeadBeat = true;
                        request.SystemState = data[23];
                        request.PartState = data[40];
                        request.AGVExecuteCmd = data[41];
                        request.Assembly_Area1_RFID = data[42];
                        request.Assembly_Area2_RFID = data[68];
                        return request;
                    }
                    else
                    {
                        return request;
                    }
                }
                else
                {
                    return request;
                }
            }
            else
            {
                return request;
            }
        }

        /// <summary>
        /// 消息头校验函数
        /// </summary>
        /// <param name="headArray">消息的前20个字节</param>
        /// <returns></returns>
        private bool headCKfunc(byte[] data)  
        {
            if (data[0] == 0x05 && data[1] == 0x0A && data[2] == 0x05 && data[3] == 0x0A)  //消息头标识校验通过
            {
                byte headCKCode = 0;
                for (int i = 0; i < 19; i++)
                {
                    headCKCode += data[i];
                }
                if (headCKCode == data[19])  //消息头校验通过
                {
                    byte bodyCKCode = 0;
                    for (int i = 20; i < 150; i++)
                    {
                        bodyCKCode += data[i];
                    }
                    if (bodyCKCode == data[14])  //消息体校验通过
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


    }
}
    

