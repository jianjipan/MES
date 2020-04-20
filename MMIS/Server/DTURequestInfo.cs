using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperSocket.SocketBase.Protocol;

namespace MMIS
{
    public class DTURequestInfo:IRequestInfo
    {
        //实现IRequestInfo接口
        public string Key { get; set; }

        //命令字(1代表加工区PLC发来的消息，2代表检测区PLC发来的消息，3代表装配区PLC发来的消息)
        public byte CmdKey { get; set; }

        //平台心跳
        public bool HeadBeat { get; set; }

        //系统状态
        public byte SystemState { get; set; }

        //系统控制  收到该信号值为1时不急停，0为否则急停
        private byte systemcontrol = 1;
        public byte SystemControl { get { return systemcontrol; } set { systemcontrol = value; } }

        //各机构状态
        public byte PartState { get; set; }

        //需要AGV执行的命令
        public byte AGVExecuteCmd { get; set; }

        //毛胚托盘以取完，需要MEMS将空托盘入库
        public byte Raw_Tray_Empty { get; set; }

        //人工上料AGV对接台RFID识别托盘类型代号
        public byte Manual_Up_RFID { get; set; }

        public float Manual_Up_CD_Number { get; set; }  //人工上料AGV对接台RFID识别托盘工件相关参数若D托盘，则为轴承数量，若E托盘，则为螺钉数量

        public float Manual_Down_CD_Number { get; set; }  //人工下料AGV对接台RFID识别托盘工件相关参数若D托盘，则为轴承数量，若E托盘，则为螺钉数量

        public float Manual_Down_Para1 { get; set; }  //人工下料AGV对接台RFID识别托盘工件相关参数检测轴直径数值1

        public float Manual_Down_Para2 { get; set; }  //人工下料AGV对接台RFID识别托盘工件相关参数检测轴直径数值2

        public float Manual_Down_Para3 { get; set; }  //人工下料AGV对接台RFID识别托盘工件相关参数检测轴直径数值3

        public float Manual_Down_Para4 { get; set; }  //人工下料AGV对接台RFID识别托盘工件相关参数检测轴直径数值4

        public float Manual_Down_Para5 { get; set; }  //人工下料AGV对接台RFID识别托盘工件相关参数检测轴直径数值5

        //人工下料AGV对接台RFID识别托盘类型代号
        public byte Manual_Down_RFID { get; set; }

        //加工区AGV对接台1RFID识别托盘类型代号
        public byte Process_Area1_RFID { get; set; }

        //加工区AGV对接台2RFID识别托盘类型代号
        public byte Process_Area2_RFID { get; set; }

        //检测区AGV对接台1RFID识别托盘类型代号
        public byte Detection_Area1_RFID { get; set; }

        //检测区AGV对接台1RFID识别托盘相关参数1（A3托盘会有这个数组）
        public float Detection_Area1_Parameter1 { get; set; }
        //检测区AGV对接台1RFID识别托盘相关参数2（A3托盘会有这个数组）
        public float Detection_Area1_Parameter2 { get; set; }
        //检测区AGV对接台1RFID识别托盘相关参数3（A3托盘会有这个数组）
        public float Detection_Area1_Parameter3 { get; set; }
        //检测区AGV对接台1RFID识别托盘相关参数4（A3托盘会有这个数组）
        public float Detection_Area1_Parameter4 { get; set; }
        //检测区AGV对接台1RFID识别托盘相关参数5（A3托盘会有这个数组）
        public float Detection_Area1_Parameter5 { get; set; }

        //检测区AGV对接台2RFID识别托盘类型代号
        public byte Detection_Area2_RFID { get; set; }

        //检测区AGV对接台2RFID识别托盘相关参数1（A3托盘会有这个数组）
        public float Detection_Area2_Parameter1 { get; set; }
        //检测区AGV对接台2RFID识别托盘相关参数2（A3托盘会有这个数组）
        public float Detection_Area2_Parameter2 { get; set; }
        //检测区AGV对接台2RFID识别托盘相关参数3（A3托盘会有这个数组）
        public float Detection_Area2_Parameter3 { get; set; }
        //检测区AGV对接台2RFID识别托盘相关参数4（A3托盘会有这个数组）
        public float Detection_Area2_Parameter4 { get; set; }
        //检测区AGV对接台2RFID识别托盘相关参数5（A3托盘会有这个数组）
        public float Detection_Area2_Parameter5 { get; set; }

        //装配区输送台1入口RFID识别托盘类型代号
        public byte Assembly_Area1_RFID { get; set; }

        //装配区输送台2入口RFID识别托盘类型代号
        public byte Assembly_Area2_RFID { get; set; }



    }
}
