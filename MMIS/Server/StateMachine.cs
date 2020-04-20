using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;

namespace MMIS
{
    class StateMachine
    {
        public delegate void SendWHMsgDelegate(WHSendPackage package);
        public  static SendWHMsgDelegate sendWHMsgDel;

        //P代表加工区的状态
        private static bool P_OnlyOnce1 = false;   //防止因为重复读取状态导致重复向数据表中插入指令的标志位
        private static bool P_OnlyOnce2 = false;
        private static bool P_OnlyOnce3 = false;
        private static bool P_OnlyOnce4 = false;
        private static bool P_OnlyOnce5 = false;
        private static bool P_OnlyOnce6 = false;
        //状态
        public static byte P_System_State { get; private set; }    //加工区系统状态 0：系统异常 1：系统正常，未启动 2：系统正常，已启动 3：系统手动模式
        public static byte P_System_Control { get; private set; }  //系统控制  0 急停 1 不急停
        public static int P_Manual_Up_Area { get; private set; }  //人工上料AGV对接台状态 0：无托盘，1：有托盘
        public static int P_Manual_Down_Area { get; private set; }  //人工下料AGV对接台状态 0：无托盘，1：有托盘
        public static int P_Process_Area1 { get; private set; }    //加工区AGV对接台1状态 0：无托盘，1：有托盘
        public static int P_Process_Area2 { get; private set; }   //加工区AGV对接台2状态 0：无托盘，1：有托盘
        public static int P_Mazak1_State { get; private set; }  //轴、端盖加工机床1状态（Mazak机床）0：空闲，1：正在加工
        public static int P_Mazak2_State { get; private set; }  //轴、端盖加工机床2状态（Mazak机床）0：空闲，1：正在加工
        public static int P_Big_State { get; private set; }   //壳体加工机床状态（大机床）0：空闲，1：正在加工
        public static int P_Robot_State { get; private set; }  //加工区机器人状态 0：空闲，1：作业中
        //动作
        public static int P_Raw_Tray_Empty { get; private set; } //立库出口2位置，待加工毛坯托盘上所有工件已取完，MES需通知AGV将此处空托盘取走入库。0=无动作，1=工件已全部取完，直到MES返回此处空托盘已被取走，才将该位复位=0
        public static int P_AGV_Manual_Up { get; private set; }  //人工上料AGV对接台 0：无动作，1：AGV到此取托盘
        public static int P_AGV_Manual_Down { get; private set; } //人工下料AGV对接台 0：无动作，1：AGV到此取托盘
        public static int P_AGV_Process1 { get; private set; }  // 加工区AGV对接台1 0：无动作，1：AGV到此取托盘
        public static int P_AGV_Process2 { get; private set; }  //加工区AGV对接台2 0：无动作，1：AGV到此取托盘
        //RFID
        public static byte P_Manual_Up_RFID { get; private set; }  //人工上料AGV对接台RFID识别托盘类型代号
        public static byte P_Manual_Down_RFID { get; private set; }  //人工下料AGV对接台RFID识别托盘类型代号
        public static byte P_Process1_RFID { get; private set; }  //加工区AGV对接台1 RFID识别托盘类型代号
        public static byte P_Process2_RFID { get; private set; }  //加工区AGV对接台2 RFID识别托盘类型代号

        //D代表检测区的状态
        private static bool D_OnlyOnce1 = false;
        private static bool D_OnlyOnce2 = false;
        private static bool D_OnlyOnce3 = false;
        private static bool D_OnlyOnce4 = false;
        //状态
        public static byte D_System_State { get; private set; }   //检测区系统状态 0：系统异常 1：系统正常，未启动 2：系统正常，已启动 3：系统手动模式
        public static int D_Detection_Area1 { get; private set; } //检测区AGV对接台1状态 0：无托盘，1：有托盘
        public static int D_Detection_Area2 { get; private set; } //检测区AGV对接台2状态 0：无托盘，1：有托盘
        public static int D_Marking_State { get; private set; }  //激光打标机工作状态 0：空闲，1：正在打标
        public static int D_Robot_State { get; private set; }  //检测区机器人状态 0：空闲，1：作业中
        //动作
        public static int D_AGV_Detection1 { get; private set; }  //检测区AGV对接台1 0：无动作，1：AGV到此取托盘
        public static float D_AGV_Detection1_Para1 { get; private set; }   //检测参数1
        public static float D_AGV_Detection1_Para2 { get; private set; }   //检测参数2
        public static float D_AGV_Detection1_Para3 { get; private set; }   //检测参数3
        public static float D_AGV_Detection1_Para4 { get; private set; }   //检测参数4
        public static float D_AGV_Detection1_Para5 { get; private set; }   //检测参数5
        public static int D_AGV_Detection2 { get; private set; }  //检测区AGV对接台2 0：无动作，1：AGV到此取托盘
        public static float D_AGV_Detection2_Para1 { get; private set; }   //检测参数1
        public static float D_AGV_Detection2_Para2 { get; private set; }   //检测参数2
        public static float D_AGV_Detection2_Para3 { get; private set; }   //检测参数3
        public static float D_AGV_Detection2_Para4 { get; private set; }   //检测参数4
        public static float D_AGV_Detection2_Para5 { get; private set; }   //检测参数5
        //RFID
        public static byte D_Detection1_RFID { get; private set; }  //检测区AGV对接台1 RFID识别托盘类型代号
        public static byte D_Detection2_RFID { get; private set; }  //检测区AGV对接台2 RFID识别托盘类型代号

        //A代表装配区
        private static bool A_OnlyOnce1 = false;
        private static bool A_OnlyOnce2 = false;
        private static bool A_OnlyOnce3 = false;
        private static bool A_OnlyOnce4 = false;
        //状态
        public static byte A_System_State { get; private set; }   //装配区系统状态 0：系统异常 1：系统正常，未启动 2：系统正常，已启动 3：系统手动模式
        public static int A_Assembly_Area1 { get; private set; } //装配区输送台1入口状态（拧螺钉输送台） 0：无托盘，1：有托盘
        public static int A_Assembly_Area2 { get; private set; }  //装配区输送台2入口状态（轴承压装输送台） 0：无托盘，1：有托盘
        public static int A_Robot_State { get; private set; }  //装配区机器人状态0：空闲，1：作业中
        public static int A_Cor_State { get; private set; }  //拧螺钉直角坐标机器人状态 0：空闲，1：作业中
        public static int A_Assembly_InArea1 { get; private set; }   //装配区输送台1内侧状态（拧螺钉输送台）0：无托盘，1：有托盘（只有装配输送台内侧和入口都没有托盘的时候，才能让AGV来放托盘）
        public static int A_Assembly_InArea2 { get; private set; }   //装配区输送台2内侧状态（拧螺钉输送台）0：无托盘，1：有托盘（只有装配输送台内侧和入口都没有托盘的时候，才能让AGV来放托盘）
        //动作
        public static int A_AGV_Assembly1 { get; private set; }  //装配区输送台1 0：无动作，1：AGV到此取托盘
        public static int A_AGV_Assembly2 { get; private set; }  //装配区输送台2 0：无动作，1：AGV到此取托盘
        //RFID
        public static int A_Assembly1_RFID { get; private set; }  //装配区输送台1入口RFID识别托盘类型代号
        public static int A_Assembly2_RFID { get; private set; }  //装配区输送台2入口RFID识别托盘类型代号

        //数据
        public static int UP_C_D_Number { get; private set; }   // 人工上料 若为D托盘，则为轴承数量、若为C托盘，则为螺钉数量


        public static int Down_C_D_Number { get; private set; } //人工下料 若为D托盘，则为轴承数量、若为C托盘，则为螺钉数量
        public static float Down_RFID_Para1 { get; private set; }   //人工下料AGV对接台RFID识别托盘工件相关参数1
        public static float Down_RFID_Para2 { get; private set; }   //人工下料AGV对接台RFID识别托盘工件相关参数2
        public static float Down_RFID_Para3 { get; private set; }   //人工下料AGV对接台RFID识别托盘工件相关参数3
        public static float Down_RFID_Para4 { get; private set; }   //人工下料AGV对接台RFID识别托盘工件相关参数4
        public static float Down_RFID_Para5 { get; private set; }   //人工下料AGV对接台RFID识别托盘工件相关参数5

        //将加工区的状态赋给上述属性
        public static void Get_P_State(DTURequestInfo requestinfo)
        {
            //状态
            P_System_State = requestinfo.SystemState;
            P_Manual_Up_Area = (requestinfo.PartState & 1) == 1 ? 1 : 0;
            P_Manual_Down_Area = (requestinfo.PartState & 2) == 2 ? 1 : 0;
            P_Process_Area1 = (requestinfo.PartState & 4) == 4 ? 1 : 0;
            P_Process_Area2 = (requestinfo.PartState & 8) == 8 ? 1 : 0;
            P_Mazak1_State = (requestinfo.PartState & 16) == 16 ? 1 : 0;
            P_Mazak2_State = (requestinfo.PartState & 32) == 32 ? 1 : 0;
            P_Big_State = (requestinfo.PartState & 64) == 64 ? 1 : 0;
            P_Robot_State = (requestinfo.PartState & 128) == 128 ? 1 : 0;
            //系统控制
            P_System_Control = requestinfo.SystemControl;
            if (P_System_State != 2)   //如果不是系统正常，已启动这个状态的话，就退出
            {
                return;
            }
          
            //动作
            P_Raw_Tray_Empty = (requestinfo.Raw_Tray_Empty & 1) == 1 ? 1 : 0;

            P_AGV_Manual_Up = (requestinfo.AGVExecuteCmd & 1) == 1 ? 1 : 0;
            P_AGV_Manual_Down = (requestinfo.AGVExecuteCmd & 2) == 2 ? 1 : 0;
            P_AGV_Process1 = (requestinfo.AGVExecuteCmd & 4) == 4 ? 1 : 0;
            P_AGV_Process2 = (requestinfo.AGVExecuteCmd & 8) == 8 ? 1 : 0;
            //RFID
            P_Manual_Up_RFID = requestinfo.Manual_Up_RFID;
            P_Manual_Down_RFID = requestinfo.Manual_Down_RFID;
            P_Process1_RFID = requestinfo.Process_Area1_RFID;
            if (P_Process1_RFID == ConfigClass.Tray_A2 && P_OnlyOnce5 == false)
            {
                SqlConnection con = new SqlConnection();
                DataBaseHandle db = new DataBaseHandle();
                db.SaveData(con, "加工订单", ConfigClass.GetTrayString(P_Process1_RFID));
                P_OnlyOnce5 = true;
            }
            if (P_Process1_RFID != ConfigClass.Tray_A2)
            {
                P_OnlyOnce5 = false;
            }
            P_Process2_RFID = requestinfo.Process_Area2_RFID;
            if (P_Process2_RFID == ConfigClass.Tray_A2 && P_OnlyOnce6 == false)
            {
                SqlConnection con = new SqlConnection();
                DataBaseHandle db = new DataBaseHandle();
                db.SaveData(con, "加工订单", "缺省",ConfigClass.GetTrayString(P_Process2_RFID));
                P_OnlyOnce6 = true;
            }
            if (P_Process2_RFID != ConfigClass.Tray_A2)
            {
                P_OnlyOnce6 = false;
            }

            //数据
            UP_C_D_Number = (int)requestinfo.Manual_Up_CD_Number;
            Down_C_D_Number = (int)requestinfo.Manual_Down_CD_Number;
            Down_RFID_Para1 = requestinfo.Manual_Down_Para1;
            Down_RFID_Para2 = requestinfo.Manual_Down_Para2;
            Down_RFID_Para3 = requestinfo.Manual_Down_Para3;
            Down_RFID_Para4 = requestinfo.Manual_Down_Para4;
            Down_RFID_Para5 = requestinfo.Manual_Down_Para5;

            //如果动作值为1，则通知AGV(向AGV表中插入命令),且不能重复插入
            if (P_Raw_Tray_Empty == 1 && P_OnlyOnce1==false)
            {
                WHSendPackage package = new WHSendPackage();
                package.Head = 29000;
                package.TrayID = 0;
                package.WHPostion = 0;
                package.SerialNumber = 0;
                if (MainWindow.WHDISCONNECTED == false)
                {
                    sendWHMsgDel(package);
                    P_OnlyOnce1 = true;
                }
            }
            if (P_Raw_Tray_Empty == 0)
            {
                P_OnlyOnce1 = false;
            }
            if (P_AGV_Process1 == 1 && P_OnlyOnce2 == false)
            {
                InformAGV(ConfigClass.ProcessArea1);
                P_OnlyOnce2 = true;
            }
            if (P_AGV_Process1 == 0)
            {
                P_OnlyOnce2 = false;
            }
            if (P_AGV_Process2 == 1 && P_OnlyOnce3 == false)
            {
                InformAGV(ConfigClass.ProcessArea2);
                P_OnlyOnce3 = true;
            }
            if (P_AGV_Process2 == 0)
            {
                P_OnlyOnce3 = false;
            }
            if (P_AGV_Manual_Up == 1 && P_OnlyOnce4 == false)
            {
                InformAGV(ConfigClass.HandUpArea,P_Manual_Up_RFID);
                P_OnlyOnce4 = true;
            }
            if (P_AGV_Manual_Up == 0)
            {
                P_OnlyOnce4 = false;
            }
            
        }
        
        //将检测区的状态赋给上述属性
        public static void Get_D_State(DTURequestInfo requestinfo)
        {
            //状态
            D_System_State = requestinfo.SystemState;
            D_Detection_Area1 = (requestinfo.PartState & 1) == 1 ? 1 : 0;
            D_Detection_Area2 = (requestinfo.PartState & 2) == 2 ? 1 : 0;
            D_Marking_State = (requestinfo.PartState & 4) == 4 ? 1 : 0;
            D_Robot_State = (requestinfo.PartState & 8) == 8 ? 1 : 0;
            if (D_System_State != 2)   //如果不是系统正常，已启动这个状态的话，就退出
            {
                return;
            }
            //动作
            D_AGV_Detection1 = (requestinfo.AGVExecuteCmd & 1) == 1 ? 1 : 0;
            D_AGV_Detection2 = (requestinfo.AGVExecuteCmd & 2) == 2 ? 1 : 0;
            //RFID
            D_Detection1_RFID = requestinfo.Detection_Area1_RFID;
            if (D_Detection1_RFID == ConfigClass.Tray_A3 && D_OnlyOnce3 == false)
            {
                D_AGV_Detection1_Para1 = requestinfo.Detection_Area1_Parameter1;
                D_AGV_Detection1_Para2 = requestinfo.Detection_Area1_Parameter2;
                D_AGV_Detection1_Para3 = requestinfo.Detection_Area1_Parameter3;
                D_AGV_Detection1_Para4 = requestinfo.Detection_Area1_Parameter4;
                D_AGV_Detection1_Para5 = requestinfo.Detection_Area1_Parameter5;
                SqlConnection con = new SqlConnection();
                DataBaseHandle db = new DataBaseHandle();
                db.SaveData(con, "检测订单", ConfigClass.GetTrayString(D_Detection1_RFID),
                    ConfigClass.GetTrayString(D_Detection2_RFID), D_AGV_Detection1_Para1, D_AGV_Detection1_Para2,
                    D_AGV_Detection1_Para3, D_AGV_Detection1_Para4, D_AGV_Detection1_Para5);
                D_OnlyOnce3 = true;
            }
            if (D_Detection1_RFID != ConfigClass.Tray_A3)
            {
                D_OnlyOnce3 = false;
            }
            D_Detection2_RFID = requestinfo.Detection_Area2_RFID;
            if (D_Detection2_RFID ==ConfigClass.Tray_A3&&D_OnlyOnce4==false)
            {
                D_AGV_Detection2_Para1 = requestinfo.Detection_Area2_Parameter1;
                D_AGV_Detection2_Para2 = requestinfo.Detection_Area2_Parameter2;
                D_AGV_Detection2_Para3 = requestinfo.Detection_Area2_Parameter3;
                D_AGV_Detection2_Para4 = requestinfo.Detection_Area2_Parameter4;
                D_AGV_Detection2_Para5 = requestinfo.Detection_Area2_Parameter5;
                SqlConnection con = new SqlConnection();
                DataBaseHandle db = new DataBaseHandle();
                db.SaveData(con, "检测订单", ConfigClass.GetTrayString(D_Detection1_RFID),
                    ConfigClass.GetTrayString(D_Detection2_RFID), D_AGV_Detection1_Para1, D_AGV_Detection1_Para2,
                    D_AGV_Detection1_Para3, D_AGV_Detection1_Para4, D_AGV_Detection1_Para5);
                D_OnlyOnce4 = true;
            }
            if (D_Detection2_RFID != ConfigClass.Tray_A3)
            {
                D_OnlyOnce4 = false;
            }
            //如果动作值为1，则通知AGV(向AGV表中插入命令)且不能重复插入
            if (D_AGV_Detection1 == 1 && D_OnlyOnce1 == false)
            {
                InformAGV(ConfigClass.DetectionArea1);
                D_OnlyOnce1 = true;
            }
            if (D_AGV_Detection1 == 0)
            {
                D_OnlyOnce1 = false;
            }
            if (D_AGV_Detection2 == 1 && D_OnlyOnce2 == false)
            {
                InformAGV(ConfigClass.DetectionArea2);
                D_OnlyOnce2 = true;
            }
            if (D_AGV_Detection2 == 0)
            {
                D_OnlyOnce2 = false;
            }
        }

        //将装配区的状态赋给上述属性
        public static void Get_A_State(DTURequestInfo requestinfo)
        {
            //状态
            A_System_State = requestinfo.SystemState;
            A_Assembly_Area1 = (requestinfo.PartState & 1) == 1 ? 1 : 0;
            A_Assembly_Area2 = (requestinfo.PartState & 2) == 2 ? 1 : 0;
            A_Robot_State = (requestinfo.PartState & 4) == 4 ? 1 : 0;
            A_Cor_State = (requestinfo.PartState & 8) == 8 ? 1 : 0;
            A_Assembly_InArea1 = (requestinfo.PartState & 16) == 16 ? 1 : 0;
            A_Assembly_InArea2 = (requestinfo.PartState & 32) == 32 ? 1 : 0;
            if (A_System_State != 2)   //如果不是系统正常，已启动这个状态的话，就退出
            {
                return;
            }
            //动作
            A_AGV_Assembly1 = (requestinfo.AGVExecuteCmd & 1) == 1 ? 1 : 0;
            A_AGV_Assembly2 = (requestinfo.AGVExecuteCmd & 2) == 2 ? 1 : 0;
            //RFID
            A_Assembly1_RFID = requestinfo.Assembly_Area1_RFID;
            if (A_Assembly1_RFID == ConfigClass.Tray_B2 && A_OnlyOnce3 == false)
            {
                SqlConnection con = new SqlConnection();
                DataBaseHandle db = new DataBaseHandle();
                db.SaveData(con, "拧螺丝订单", ConfigClass.GetTrayString(A_Assembly1_RFID));
                A_OnlyOnce3 = true;
            }
            if (A_Assembly1_RFID != ConfigClass.Tray_B2)
            {
                A_OnlyOnce3 = false;
            }
            A_Assembly2_RFID = requestinfo.Assembly_Area2_RFID;
            if (A_Assembly2_RFID == ConfigClass.Tray_B2 && A_OnlyOnce4== false)
            {
                SqlConnection con = new SqlConnection();
                DataBaseHandle db = new DataBaseHandle();
                db.SaveData(con, "拧螺丝订单","缺省",ConfigClass.GetTrayString(A_Assembly1_RFID));
                A_OnlyOnce4 = true;
            }
            if (A_Assembly2_RFID != ConfigClass.Tray_B2)
            {
                A_OnlyOnce4 = false;
            }
            //如果动作值为1，则通知AGV(向AGV表中插入命令)且不能重复插入
            if (A_AGV_Assembly1 == 1 && A_OnlyOnce1 == false)
            {
                InformAGV(ConfigClass.AssemblyArea1);
                A_OnlyOnce1 = true;
            }
            if (A_AGV_Assembly1 == 0)
            {
                A_OnlyOnce1 = false;
            }
            if (A_AGV_Assembly2 == 1 && A_OnlyOnce2 == false)
            {
                InformAGV(ConfigClass.AssemblyArea2);
                A_OnlyOnce2 = true;
            }
            if (A_AGV_Assembly2 == 0)
            {
                A_OnlyOnce2 = false;
            }
        }

        private static void InformAGV(ushort StartPoint)
        {
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            db.InsertAGVCmd(con, StartPoint, ConfigClass.WHInlibArea);
        }
        private static void InformAGV(ushort StartPoint,byte TrayStyle)
        {
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            db.InsertAGVCmd(con, StartPoint, ConfigClass.WHInlibArea,TrayStyle);
        }
    }
}
