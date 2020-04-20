using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMIS
{
    class ConfigClass
    {
        private static object lock0 = new object();
        private static string server_ipadd;  //f服务器IP地址
        private static int server_port;    //服务器端口号
        private static string AGV_ipadd;   //AGV的IP地址
        private static int AGV_port;  //AGV的端口号

        private static string WH_ipadd;   //立库的IP地址
        private static int WH_port;   //立库的端口号

        private static string Pro_M_ipadd;   //加工机器的IP地址

        private static string Assembly_ipadd;  //轴承压装机器的IP地址

        private static string Detection_ipadd;   //检测机的IP地址

        private static string WHManngerSYS_Password;  //立库管理系统密码

        public static string SERVER_IPADD
        {
            get { return server_ipadd; }
            set { server_ipadd = value; }
        }

        public static int SERVER_PORT
        {
            get { return server_port; }
            set { server_port = value; }
        }
        public static string AGV_IPADD
        {
            get { return AGV_ipadd; }
            set { AGV_ipadd = value; }   
        }
        public static int AGV_PORT
        {
            get { return AGV_port; }
            set { AGV_port = value; }
        }
        public static string WH_IPADD
        {
            get { return WH_ipadd; }
            set { WH_ipadd = value; }
        }
        public static int WH_PORT
        {
            get { return WH_port; }
            set { WH_port = value; }
        }
        public static string ASSEMBLY_IPADD
        {
            get { return Assembly_ipadd; }
            set { Assembly_ipadd = value; }
        }
        public static string PRO_M_IPADD
        {
            get { return Pro_M_ipadd; }
            set { Pro_M_ipadd = value; }
        }
        public static string DETECTION_IPADD
        {
            get { return Detection_ipadd; }
            set { Detection_ipadd = value; }
        }
        public static string WHMANNGERSYS_PASSWORD
        {
            get { return WHManngerSYS_Password; }
            set { WHManngerSYS_Password = value; }
        }

        public static string[] SystemState=new string[]{"系统异常","系统正常，未启动","系统正常，已启动","系统手动模式"};
        public static string[] Tray_State = new string[] { "无托盘", "有托盘" };
        public static string[] Machine_State = new string[] { "空闲", "作业中" };

        //托盘类型
        public const int Tray_Empty = 0;   //空货架
        public const int Tray_A0 = 1;     //加工空托盘A0
        public const int Tray_A1 = 2;  //加工毛坯托盘A1 
        public const int Tray_A2 = 3;   //加工成品托盘A2  
        public const int Tray_A3 = 4;   //加工检测托盘A3
        public const int Tray_A4 = 5;   //轴承压装毛坯托盘A4
        public const int Tray_A5 = 6;  //轴承压装完成托盘A5
        public const int Tray_A6 = 7;  //轴承压装空托盘A6
        public const int Tray_B0 = 8;  //拧螺钉空托盘B0
        public const int Tray_B1 = 9;  //拧螺钉打钉托盘B1
        public const int Tray_B2 = 10;  //螺钉完成托盘B2
        public const int Tray_C = 11;  //轴承托盘C
        public const int Tray_D = 12;  //螺钉托盘D

        //地图号
        public const int WHOutlibArea1 = 29;  //立库出库区
        public const int WHOutlibArea2 = 34;   //侧边出库区
        public const int WHInlibArea = 32;   //立库入库区
        public const int ProcessArea1 = 25;   //加工区1
        public const int ProcessArea2 = 23;   //加工区2
        public const int AssemblyArea1 = 11;  //装配区1(拧螺丝)
        public const int AssemblyArea2 = 13;  //装配区2(轴承压装)
        public const int DetectionArea1 = 0;  //检测区1
        public const int DetectionArea2 = 6;   //检测区2
        public const int HandUpArea = 18;    //人工上料区
        public const int HandDownArea = 20;   //人工下料区
        public const int ChargingArea = 200;  //充电区

        //实现地图号与其字面意义的映射
        public static string GetMapString(int MapID)
        {
            string MapString = "";
            switch (MapID)
            {
                case 29:
                    MapString = "立库出库区处";
                    break;
                case 34:
                    MapString = "侧边出库区处";
                    break;
                case 32:
                    MapString = "立库入库区处";
                    break;
                case 25:
                    MapString = "加工区1处";
                    break;
                case 23:
                    MapString = "加工区2处";
                    break;
                case 11:
                    MapString = "拧螺丝处";
                    break;
                case 13:
                    MapString = "轴承压装处";
                    break;
                case 0:
                    MapString = "检测区1处";
                    break;
                case 6:
                    MapString = "检测区2处";
                    break;
                case 18:
                    MapString = "人工上料区处";
                    break;
                case 20:
                    MapString = "人工下料区处";
                    break;

            }
            return MapString;
        }

        //实现托盘号与其代号的映射
        public static string GetTrayString(int TrayID)
        {
            lock (lock0)
            {
                string TrayString = "";
                switch (TrayID)
                {
                    case 1:
                        TrayString = "A0";
                        break;
                    case 2:
                        TrayString = "A1";
                        break;
                    case 3:
                        TrayString = "A2";
                        break;
                    case 4:
                        TrayString = "A3";
                        break;
                    case 5:
                        TrayString = "B1";
                        break;
                    case 6:
                        TrayString = "B2";
                        break;
                    case 7:
                        TrayString = "B0";
                        break;
                    case 8:
                        TrayString = "C0";
                        break;
                    case 9:
                        TrayString = "C1";
                        break;
                    case 10:
                        TrayString = "C2";
                        break;
                    case 11:
                        TrayString = "D";
                        break;
                    case 12:
                        TrayString = "E";
                        break;
                }
                return TrayString;
            }
        }

    }
}
