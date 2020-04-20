using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMIS
{
    class DTUSendPackage
    {
        
        private static byte systemstate = 1; //系统状态 默认情况下系统正常
        public static byte SystemState
        {
            get { return systemstate; }
            set { systemstate = value; }
        }

        private static byte hand_Order_Enable = 1;

        //加工区PLC

        public static byte Raw_Tray_Out { get; set; }  //待加工毛坯托盘已出库到位信号，
        public static byte Inform_P_Raw_Tray { get; set; }   //通知加工区机器人，毛坯托盘已经取走
        //取
        public static byte AGV_Manual_Up_GET { get; set; }  //人工上料AGV对接台取托盘完成
        public static byte AGV_Manual_Down_GET { get; set; } //人工下料AGV对接台取托盘完成
        public static byte AGV_Process1_GET { get; set; }  //加工区AGV对接台1取托盘完成
        public static byte AGV_Process2_GET { get; set; }  //加工区AGV对接台2取托盘完成

        //放
        public static byte AGV_Manual_Up_PUT { get; set; }  //人工上料AGV对接台放托盘完成
        public static byte AGV_Manual_Down_PUT { get; set; }  //人工下料AGV对接台放托盘完成
        public static byte AGV_Process1_PUT { get; set; }  //加工区AGV对接台1放托盘完成
        public static byte AGV_Process2_PUT { get; set; }  //加工区AGV对接台2放托盘完成

        //订单控制
        public static byte P_Order_Enable { get; set; }  //加工订单允许
        public static byte Hand_Order_Enabel { get { return hand_Order_Enable; } set { hand_Order_Enable = value; } }  //人工上下料订单允许,初始情况下是允许(1)
        public static byte OrderControl { get; set; }  //订单控制  人工上料AGV对接台RFID写码该位=1且保持5s脉冲
        public static byte Maunal_Up_RFID { get; set; }   //人工上料AGV对接台RFID写码托盘类型代号
        public static float Manual_Up_RFID_Para { get; set; }  //人工上料AGV对接台RFID写码托盘工件相关参数

        //检测区PLC

        //订单控制
        public static byte D_Order_Enable { get; set; }  //检测订单允许

        //系统控制  当收到加工区发来的急停信号后，将该值变为0  意味着急停
        private static byte D_system_control = 1;   // 0=急停，1=不急停
        public static byte D_System_Control { get { return D_system_control; } set { D_system_control = value; } }  
        //取
        public static byte AGV_Detection1_GET { get; set; }
        public static byte AGV_Detection2_GET { get; set; }
        //放
        public static byte AGV_Detection1_PUT { get; set; }
        public static byte AGV_Detection2_PUT { get; set; }

        //装配区PLC

        //订单控制
        public static byte A1_Order_Enable { get; set; }   //拧螺钉订单允许
        public static byte A2_Order_Enable { get; set; }   //轴承压装订单允许

        //系统控制  系统控制  当收到加工区发来的急停信号后，将该值变为0  意味着急停
        private static byte A_system_control = 1;  // 0=急停，1=不急停
        public static byte A_System_Control { get { return A_system_control; } set { A_system_control = value; } }  
        //取
        public static byte AGV_Assembly1_GET { get; set; }
        public static byte AGV_Assembly2_GET { get; set; }
        //放
        public static byte AGV_Assembly1_PUT { get; set; }
        public static byte AGV_Assembly2_PUT { get; set; }

        public void SendMessage_P(DTUSession session)
        {
            byte[] data = new byte[150];
            data[0] = 0x05; data[1] = 0x0A; data[2] = 0x05; data[3] = 0x0A;//消息头标识                               
            data[4] = 0x80;   //通讯标志位
            data[5] = 0x02; data[6] = 0x0A; data[7] = 0x00; data[8] = 0x01;//命令字            
            data[9] = 0x00; data[10] = 0x82;//数据长度 130 固定值
            data[11] = 0x00; data[12] = 0x01; //包序 固定值      
            data[13] = 0x01;   //最后一包  固定值
            data[20] = 0x29; data[21] = 0xFF;  //平台心跳
            data[23] = SystemState;   //系统状态
            data[24] = (byte)((P_Order_Enable)
                | (Hand_Order_Enabel << 1)
                | (OrderControl << 3));  //订单控制
            data[39] = (byte)((Raw_Tray_Out) | (Inform_P_Raw_Tray << 1));
            data[41] = (byte)((AGV_Manual_Up_GET)
                | (AGV_Manual_Down_GET << 1)
                | (AGV_Process1_GET << 2)
                | (AGV_Process2_GET << 3)
                | (AGV_Manual_Up_PUT << 4)
                | (AGV_Manual_Down_PUT << 5)
                | (AGV_Process1_PUT << 6)
                | (AGV_Process2_PUT << 7));
            data[42] = Maunal_Up_RFID;
            byte[] cmd = new byte[4];
            cmd = DataTransform.floatToByte(Manual_Up_RFID_Para);
            data[46] = cmd[0];
            data[47] = cmd[1];
            data[48] = cmd[2];
            data[49] = cmd[3];
            for (int i = 20; i < 150; i++)  //消息体校验字
            {
                data[14] += data[i];
            }
            for (int i = 0; i < 19; i++)  //消息头校验字
            {
                data[19] += data[i];
            }
            session.Send(data, 0, 150);
        }

        public void SendMessage_D(DTUSession session)
        {
            byte[] data = new byte[150];
            data[0] = 0x05; data[1] = 0x0A; data[2] = 0x05; data[3] = 0x0A;//消息头标识                               
            data[4] = 0x80;   //通讯标志位
            data[5] = 0x02; data[6] = 0x0A; data[7] = 0x00; data[8] = 0x02;//命令字            
            data[9] = 0x00; data[10] = 0x82;//数据长度 130 固定值
            data[11] = 0x00; data[12] = 0x01; //包序 固定值      
            data[13] = 0x01;   //最后一包  固定值
            data[20] = 0x28; data[21] = 0xEE;  //平台心跳
            data[23] = SystemState;   //系统状态
            data[24] = (byte)(D_Order_Enable);  //检测订单允许
            data[25] = (byte)(D_System_Control);  //系统控制
            data[41] = (byte)((AGV_Detection1_GET)
                | (AGV_Detection2_GET << 1)
                | (AGV_Detection1_PUT << 2)
                | (AGV_Detection2_PUT << 3));
            for (int i = 20; i < 150; i++)  //消息体校验字
            {
                data[14] += data[i];
            }
            for (int i = 0; i < 19; i++)  //消息头校验字
            {
                data[19] += data[i];
            }
            session.Send(data, 0, 150);
        }
        public void SendMessage_A(DTUSession session)
        {
            byte[] data = new byte[150];
            data[0] = 0x05; data[1] = 0x0A; data[2] = 0x05; data[3] = 0x0A;//消息头标识                               
            data[4] = 0x80;   //通讯标志位
            data[5] = 0x02; data[6] = 0x0A; data[7] = 0x00; data[8] = 0x03;//命令字            
            data[9] = 0x00; data[10] = 0x82;//数据长度 130 固定值
            data[11] = 0x00; data[12] = 0x01; //包序 固定值      
            data[13] = 0x01;   //最后一包  固定值
            data[20] = 0x27; data[21] = 0xDD;  //平台心跳
            data[23] = SystemState;   //系统状态
            data[24] = (byte)((A1_Order_Enable) | (A2_Order_Enable << 1));
            data[25] = (byte)(A_System_Control);
            data[41] = (byte)((AGV_Assembly1_GET)
                | (AGV_Assembly2_GET << 1)
                | (AGV_Assembly1_PUT << 2)
                | (AGV_Assembly2_PUT << 3));
            for (int i = 20; i < 150; i++)  //消息体校验字
            {
                data[14] += data[i];
            }
            for (int i = 0; i < 19; i++)  //消息头校验字
            {
                data[19] += data[i];
            }
            session.Send(data, 0, 150);
        }
    }
}
