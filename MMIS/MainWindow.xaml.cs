using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SuperSocket.SocketBase;
using SuperSocket.ClientEngine;
using System.Net;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Threading;
using System.Data;



namespace MMIS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DTUServer dtuServer = new DTUServer();   //定义Socket服务器套接字
        public static EasyClient AGVclient = new EasyClient();   //定义AGV的Socket客服端套接字
        public static EasyClient WHClient = new EasyClient();    //定义WH（WareHouse）的Socket客服端套接字     

        private DispatcherTimer StateTimer1 = new DispatcherTimer();  //人工上下料信息的状态控制。
        public static int AGVOperState = 0; //0代表现在没有操作  //1代表自动执行订单操作 2代表人工上下料操作 3代表手动操作
        public static int WHOperState = 0; //0代表现在没有操作  //1代表自动执行订单操作，2代表人工上下料操作 3代表手动操作
        Thread AGVExecuteThread = null;  //AGV订单执行线程
        Thread WHExecuteThread = null;   //立库订单执行线程
        public static KuweiNumber kuweinumber = new KuweiNumber();
        public static OrderState orderstate = new OrderState();
        public static AGVUIAHandle agvUIHandle = new AGVUIAHandle();
        public static WHUIHandle whUIHandle = new WHUIHandle();
        public static ServerUIHandle serverUIhandle = new ServerUIHandle();

        public static bool WHMatchingProcess1 = false;   //为true表示立库已经匹配了加工区出库1的位置不能在出此方面的库位
        public static bool WHMatchingProcess2 = false;   //为true表示立库已经匹配了加工区出库1的位置不能在出此方面的库位
        public static bool WHMatchingDetection1 = false;
        public static bool WHMatchingDetection2 = false;
        public static bool WHMatchingAssembly1 = false;
        public static bool WHMatchingAssembly2 = false;

        public static bool AGVMatchingProcess1 = false;   //为true表示立库已通知AGV前往此区域（加工区1）不能再通知它前往这里了
        public static bool AGVMatchingProcess2 = false;
        public static bool AGVMatchingDetection1 = false;
        public static bool AGVMatchingDetection2 = false;
        public static bool AGVMatchingAssembly1 = false;
        public static bool AGVMatchingAssembly2 = false;

        public static bool P_Order_Enable = false;   //为true时允许向立库指令表中插入订单
        public static bool D_Order_Enable = false;   //为true时允许向立库指令表中插入订单
        public static bool A1_Order_Enable = false;   //为true时允许向立库指令表中插入订单
        public static bool A2_Order_Enable = false;   //为true时允许向立库指令表中插入订单

        public static bool firstCome = false;    //第一次执行订单后置为true，知道所有订单执行完后置为false 以后每次下订单就不用在重置flag，防止影响运行中的标识符值

        public static WHHandInfo whHandInfo = new WHHandInfo();   //手动操作是立库的出入库信息。
   
        
        private static object lock0 = new object();   //立库发送函数的锁
        private static object lock1 = new object();   //AGV发送函数的锁

       // private bool EsSignalOnlyOneFlag=false;   //立库的解急停信号只能发一次

        public MainWindow()
        {
            InitializeComponent();
            this.LoggedInUser = Environment.UserName;
            this.Designation = DateTime.Now.Date.ToLongDateString();
            LinkControl();
            Initial();
        }

        #region 将属性与控件绑定起来
        private void LinkControl()
        {
            //关于统计各种托盘数量的按钮绑定
            lb_Tary_A0.DataContext = kuweinumber;
            lb_Tary_A1.DataContext = kuweinumber;
            lb_Tary_A2.DataContext = kuweinumber;
            lb_Tary_A3.DataContext = kuweinumber;
            lb_Tary_A4.DataContext = kuweinumber;
            lb_Tary_A5.DataContext = kuweinumber;
            lb_Tary_A6.DataContext = kuweinumber;
            lb_Tary_B0.DataContext = kuweinumber;
            lb_Tary_B1.DataContext = kuweinumber;
            lb_Tary_B2.DataContext = kuweinumber;
            lb_Tary_C.DataContext = kuweinumber;
            lb_Tary_D.DataContext = kuweinumber;
            lb_Tary_Empty.DataContext = kuweinumber;

            //权限模式
            quanxianinfo.DataContext = kuweinumber;

            //关于各种订单是否在执行的按钮绑定
            lb_P_State.DataContext = orderstate;
            lb_D_State.DataContext = orderstate;
            lb_A1_State.DataContext = orderstate;
            lb_A2_State.DataContext = orderstate;

            //关于AGV的绑定
            lb_AGV_com_state.DataContext = agvUIHandle;
            lb_AGV_pos_state.DataContext = agvUIHandle;
            btn_AGV_Start_Close.DataContext = agvUIHandle;

            //关于立库的绑定
            lb_W_com_state.DataContext = whUIHandle;
            lb_W_oper_state.DataContext = whUIHandle;
            btn_WH_Start_Close.DataContext = whUIHandle;

            //关于服务器加工区控件的绑定
            lb_P_com_state.DataContext = serverUIhandle;
            lb_P_sys_state.DataContext = serverUIhandle;
            lb_P_Manualdown_Area.DataContext = serverUIhandle;
            lb_P_Manualup_Area.DataContext = serverUIhandle;
            lb_P_Area1.DataContext = serverUIhandle;
            lb_P_Area2.DataContext = serverUIhandle;
            lb_P_Mazak1_State.DataContext = serverUIhandle;
            lb_P_Mazak2_State.DataContext = serverUIhandle;
            lb_P_Big_State.DataContext = serverUIhandle;
            lb_P_Robot_State.DataContext = serverUIhandle;
            lb_Other_info.DataContext = serverUIhandle;

            //关于服务器检测区控件的绑定
            lb_D_com_state.DataContext = serverUIhandle;
            lb_D_sys_state.DataContext = serverUIhandle;
            lb_D_Area1_state.DataContext = serverUIhandle;
            lb_D_Area2_state.DataContext = serverUIhandle;
            lb_D_Marking_state.DataContext = serverUIhandle;
            lb_D_Robot_state.DataContext = serverUIhandle;

            //关于服务器装配区控件的绑定
            lb_A_com_state.DataContext = serverUIhandle;
            lb_A_sys_state.DataContext = serverUIhandle;
            lb_A_Cor_state.DataContext = serverUIhandle;
            lb_A_Robot_state.DataContext = serverUIhandle;
            lb_A_Area1_state.DataContext = serverUIhandle;
            lb_A_Area2_state.DataContext = serverUIhandle;

           


        }

        private void Initial()
        {
            agvUIHandle.AGV_BUTTON = "连接AGV";
            agvUIHandle.AGV_COM_STATE = "未连接";
            agvUIHandle.AGV_POS_ACTION = "空";
            whUIHandle.WH_BUTTON = "连接立库";
            whUIHandle.WH_COM_STATE = "未连接";
            whUIHandle.WH_OPER_STATE = "空";

            serverUIhandle.P_COM_STATE = "空";
            serverUIhandle.OTHER_INFO = "其他信息";
            serverUIhandle.P_AREA1 = "空";
            serverUIhandle.P_AREA2 = "空";
            serverUIhandle.P_BIG_ROBOT = "空";
            serverUIhandle.P_MANUALDOWN_AREA = "空";
            serverUIhandle.P_MANUALUP_AREA = "空";
            serverUIhandle.P_MAZAK1_ROBOT = "空";
            serverUIhandle.P_MAZAK2_ROBOT = "空";
            serverUIhandle.P_ROBOT = "空";
            serverUIhandle.P_SYS_STATE = "空";

            serverUIhandle.D_COM_STATE = "空";
            serverUIhandle.D_SYS_STATE = "空";
            serverUIhandle.D_AREA1 = "空";
            serverUIhandle.D_AREA2 = "空";
            serverUIhandle.D_MARKING_ROBOT = "空";
            serverUIhandle.D_ROBOT = "空";

            serverUIhandle.A_COM_STATE = "空";
            serverUIhandle.A_SYS_STATE = "空";
            serverUIhandle.A_AREA1 = "空";
            serverUIhandle.A_AREA2 = "空";
            serverUIhandle.A_COR_ROBOT = "空";
            serverUIhandle.A_ROBOT = "空";

            kuweinumber.QUANXIANMODE = "普通用户模式";

        }
        #endregion

        #region 调试用客户端

        #endregion

        #region 服务器处理函数
        private  void StartSocketServer()
        {  
            //Setup the appServer
            if (!dtuServer.Setup(ConfigClass.SERVER_IPADD,ConfigClass.SERVER_PORT))
            {
                string info="服务器不可以正确配置端口，请检查" + ConfigClass.SERVER_PORT + "端口是否占用";
                MessageBox.Show(info);
                LogInfoHelp.GetInstance().WriteInfoToLogFile(info,LogInfoHelp.LOG_TYPE.LOG_ERROR);
                return;
            }
            dtuServer.NewRequestReceived += dtuServer_NewRequestReceived;
            dtuServer.NewSessionConnected +=dtuServer_NewSessionConnected;
            dtuServer.SessionClosed += dtuServer_SessionClosed;
            //Try to start the appServer
            if (!dtuServer.Start())
            {
                MessageBox.Show("不可以启动服务器，请检查网络配置");
                LogInfoHelp.GetInstance().WriteInfoToLogFile("不可以启动服务器，请检查网络配置", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                return;
            }
            lb_show_state.Content = "成功启动服务器";
            LogInfoHelp.GetInstance().WriteInfoToLogFile("成功启动服务器", LogInfoHelp.LOG_TYPE.LOG_INFO);
            StateMachine.sendWHMsgDel += this.WHSendMsg;
        }

        private void CloseSocketServer()    //关闭服务器
        {
            //close the appServer
            dtuServer.Stop();
            dtuServer.Dispose();
            lb_show_state.Content = "已关闭服务器";
            LogInfoHelp.GetInstance().WriteInfoToLogFile("已关闭服务器", LogInfoHelp.LOG_TYPE.LOG_INFO);
        }
        private void dtuServer_NewRequestReceived(DTUSession session, DTURequestInfo requestInfo)  //新的消息请求
        {
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.PRO_M_IPADD)  //加工区发来的消息
            {
                if (requestInfo.CmdKey == 1)
                {
                    StateMachine.Get_P_State(requestInfo);  //将参数赋给状态机
                    if (StateMachine.P_System_Control == 0)   //加工区发来急停信号
                    {
                        this.lb_show_state.Dispatcher.Invoke(new Action(delegate { lb_show_state.Content = "系统处于急停状态"; }));
                        DTUSendPackage.A_System_Control = 0;   //通知装配PLC急停
                        DTUSendPackage.D_System_Control = 0;   //通知检测PLC急停
                        WHSendPackage WHpackage = new WHSendPackage();
                       // EsSignalOnlyOneFlag = false;  //解急停只发一次标志
                        WHpackage.Head = 19000;
                        WHpackage.TrayID = 0;
                        WHpackage.WHPostion = 0;
                        WHpackage.SerialNumber = 0;
                        if (MainWindow.WHDISCONNECTED == false)
                        {
                            WHSendMsg(WHpackage);          //立库急停
                        }
                        SendPackage AGVpackage = new SendPackage();
                        AGVpackage.Head = 10012;
                        AGVpackage.ESSignal = 5;   //急停                                                                                                                     
                        if (MainWindow.AGVDISCONNECTED == false)
                        {
                            AGVSendESMsg(AGVpackage);
                        }
                    }
                    if (StateMachine.P_System_Control == 1)
                    {
                        DTUSendPackage.A_System_Control = 1;   //解急停
                        DTUSendPackage.D_System_Control = 1;   //解急停
                    }
                    //状态显示                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     

                    serverUIhandle.P_SYS_STATE = ConfigClass.SystemState[StateMachine.P_System_State];
                    serverUIhandle.P_MANUALUP_AREA = ConfigClass.Tray_State[StateMachine.P_Manual_Up_Area];
                    serverUIhandle.P_MANUALDOWN_AREA = ConfigClass.Tray_State[StateMachine.P_Manual_Down_Area];
                    serverUIhandle.P_AREA1 = ConfigClass.Tray_State[StateMachine.P_Process_Area1];
                    serverUIhandle.P_AREA2 = ConfigClass.Tray_State[StateMachine.P_Process_Area2];
                    serverUIhandle.P_MAZAK1_ROBOT = ConfigClass.Machine_State[StateMachine.P_Mazak1_State];
                    serverUIhandle.P_MAZAK2_ROBOT = ConfigClass.Machine_State[StateMachine.P_Mazak2_State];
                    serverUIhandle.P_BIG_ROBOT = ConfigClass.Machine_State[StateMachine.P_Big_State];
                    serverUIhandle.P_ROBOT = ConfigClass.Machine_State[StateMachine.P_Robot_State];

                    if (StateMachine.P_Manual_Up_RFID == ConfigClass.Tray_C || StateMachine.P_Manual_Up_RFID == ConfigClass.Tray_D)
                    {
                         serverUIhandle.OTHER_INFO="上料数量："+StateMachine.UP_C_D_Number.ToString();
                    }
                    if (StateMachine.P_Manual_Down_RFID == ConfigClass.Tray_C || StateMachine.P_Manual_Down_RFID == ConfigClass.Tray_D)
                    {
                         serverUIhandle.OTHER_INFO= "下料数量：" + StateMachine.Down_C_D_Number.ToString();
                    }
                    if (StateMachine.P_Manual_Down_RFID == ConfigClass.Tray_A3)
                    {                 
                          serverUIhandle.OTHER_INFO= "A3 1:" + StateMachine.Down_RFID_Para1 + " 2:" + StateMachine.Down_RFID_Para2 + " 3:" +
                                StateMachine.Down_RFID_Para3 + " 4:" + StateMachine.Down_RFID_Para4 + " 5:" + StateMachine.Down_RFID_Para5;

                    }

                    DTUSendPackage package = new DTUSendPackage();
                    package.SendMessage_P(session);
                }
            }
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.DETECTION_IPADD)  //检测区发来的消息
            {
                if (requestInfo.CmdKey == 2)
                {
                    StateMachine.Get_D_State(requestInfo); //将参数赋给状态机
                    //状态显示
                    serverUIhandle.D_SYS_STATE = ConfigClass.SystemState[StateMachine.D_System_State];
                    serverUIhandle.D_AREA1 = ConfigClass.Tray_State[StateMachine.D_Detection_Area1];
                    serverUIhandle.D_AREA2 = ConfigClass.Tray_State[StateMachine.D_Detection_Area2];
                    serverUIhandle.D_MARKING_ROBOT = ConfigClass.Machine_State[StateMachine.D_Marking_State];
                    serverUIhandle.D_ROBOT = ConfigClass.Machine_State[StateMachine.D_Robot_State];

                    DTUSendPackage package = new DTUSendPackage();
                    package.SendMessage_D(session);

                }
            }
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.ASSEMBLY_IPADD)   //装配区发来的消息
            {
                if (requestInfo.CmdKey == 3)
                {
                    StateMachine.Get_A_State(requestInfo);  //将参数赋给状态机
                    //状态显示
                    serverUIhandle.A_SYS_STATE = ConfigClass.SystemState[StateMachine.A_System_State];
                    serverUIhandle.A_AREA1 = "外侧" + ConfigClass.Tray_State[StateMachine.A_Assembly_Area1] + "  内侧" + ConfigClass.Tray_State[StateMachine.A_Assembly_InArea1];
                    serverUIhandle.A_AREA2 = "外侧" + ConfigClass.Tray_State[StateMachine.A_Assembly_Area2] + "  内侧" + ConfigClass.Tray_State[StateMachine.A_Assembly_InArea2];
                    serverUIhandle.A_ROBOT = ConfigClass.Machine_State[StateMachine.A_Robot_State];
                    serverUIhandle.A_COR_ROBOT = ConfigClass.Machine_State[StateMachine.A_Cor_State];

                    DTUSendPackage package = new DTUSendPackage();
                    package.SendMessage_A(session);

                }
            }
        }

        private void dtuServer_NewSessionConnected(DTUSession session)   //新的连接请求
        {
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.PRO_M_IPADD)
            {
                serverUIhandle.P_COM_STATE = "连接成功";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("加工区PLC连接成功", LogInfoHelp.LOG_TYPE.LOG_INFO);
            }
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.DETECTION_IPADD)
            {
                serverUIhandle.D_COM_STATE = "连接成功";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("检测区PLC连接成功", LogInfoHelp.LOG_TYPE.LOG_INFO);
            }
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.ASSEMBLY_IPADD)
            {
                serverUIhandle.A_COM_STATE = "连接成功";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("装配区PLC连接成功", LogInfoHelp.LOG_TYPE.LOG_INFO);
            }
        }

        private void dtuServer_SessionClosed(DTUSession session, CloseReason reason)  //有客户端关闭了
        {
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.PRO_M_IPADD)
            {
                serverUIhandle.P_COM_STATE = "断开连接";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("加工区PLC断开连接", LogInfoHelp.LOG_TYPE.LOG_WARN);
            }
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.DETECTION_IPADD)
            {
                serverUIhandle.D_COM_STATE = "断开连接";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("检测区PLC断开连接", LogInfoHelp.LOG_TYPE.LOG_WARN);
            }
            if (session.RemoteEndPoint.Address.ToString() == ConfigClass.ASSEMBLY_IPADD)
            {
                serverUIhandle.A_COM_STATE = "断开连接";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("装配区PLC断开连接", LogInfoHelp.LOG_TYPE.LOG_WARN);
            }
        }

        #endregion

        #region AGV客户端处理函数
        public static bool AGVDISCONNECTED = true;   // 如果客户端已断开，则禁止再向服务器发送消息
        private void AGVStartSocketClient()
        {
            IPEndPoint endPort=new IPEndPoint(IPAddress.Parse(ConfigClass.AGV_IPADD),ConfigClass.AGV_PORT);

            AGVclient.Error +=new EventHandler<ErrorEventArgs>(AGVClient_Error);
            AGVclient.Initialize(new SocketRequestFilter(), (request) =>
                {
                    AGVOnMessageReceive(request);
                });
            AGVclient.Connected += new EventHandler(AGVClient_Connected);
            AGVclient.Closed +=new EventHandler(AGVClient_Closed);
            AGVclient.ConnectAsync(endPort);       
        }

        private void AGVCloseSocketClient()
        {
            AGVclient.Close();
        }

        private void AGVClient_Error(object sender, ErrorEventArgs e)
        {         
            agvUIHandle.AGV_COM_STATE = "连接错误";
            LogInfoHelp.GetInstance().WriteInfoToLogFile("AGV连接出错", LogInfoHelp.LOG_TYPE.LOG_WARN);
            ExecuteOrder.RunExecuteAGVCmdThread = false;   //停止AGV执行线程
            //重置AGV指令表
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            db.ResetAGVComState(con);
            AGVDISCONNECTED = true;
        }

        private void AGVClient_Closed(object sender, EventArgs e)
        {
            ExecuteOrder.RunExecuteAGVCmdThread = false;   //停止AGV执行线程                
            AGVclient.Error -= new EventHandler<ErrorEventArgs>(AGVClient_Error);
            AGVclient.Connected -= new EventHandler(AGVClient_Connected);
            AGVclient.Closed -= new EventHandler(AGVClient_Closed);
            //重置AGV指令表
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            AGVDISCONNECTED = true;
            Thread.Sleep(TimeSpan.FromSeconds(3));   //等待AGV执行线程结束
            agvUIHandle.AGV_COM_STATE = "连接断开";
            agvUIHandle.AGV_BUTTON = "连接AGV";
            LogInfoHelp.GetInstance().WriteInfoToLogFile("AGV连接断开", LogInfoHelp.LOG_TYPE.LOG_WARN);
        }

        private void AGVClient_Connected(object sender, EventArgs e)
        {
            agvUIHandle.AGV_COM_STATE = "连接成功";
            agvUIHandle.AGV_BUTTON = "AGV断开";
            LogInfoHelp.GetInstance().WriteInfoToLogFile("AGV连接成功", LogInfoHelp.LOG_TYPE.LOG_INFO);
            AGVDISCONNECTED = false;         
            //启动AGV执行线程    
            DataBaseHandle db = new DataBaseHandle();
            db.SelectAGVCmdEvent = this.AGVSendMsg;     //将AGV发送消息函数注册到查询AGV【通讯状态】字段的事件中
            AGVExecuteThread = new Thread(new ParameterizedThreadStart(ExecuteOrder.ExecuteAGVCmd));
            AGVExecuteThread.Start(db);
            AGVExecuteThread.IsBackground = true;
        }

        //收到消息      
        private void AGVOnMessageReceive(PackageInfo package)
        {
            AGVMsgHandle msghandle = new AGVMsgHandle(package);
            msghandle.UDDelegate += this.CurrentUpDownComplete;
            msghandle.sendESDelegate = this.AGVSendESMsg;
            msghandle.SelectMode(AGVOperState);
        }
        //向AGV发送急停信号
        private void AGVSendESMsg(SendPackage package)
        {
            List<byte> byteSource = new List<byte>();
            byteSource.AddRange(DataTransform.shortToByte(package.Head));
            byteSource.AddRange(DataTransform.shortToByte(package.CarID));
            byteSource.AddRange(DataTransform.shortToByte(package.ESSignal));
            byte[] data = byteSource.ToArray();
            byte CKCode = 0;
            for (int i = 0; i < data.Length; i++)
            {
                CKCode += data[i];
            }
            byteSource.Add(CKCode); //校验码
            byte[] senddata = byteSource.ToArray();
            SendAGVMsg(senddata,package);
        }
        //向AGV发送消息
        private void AGVSendMsg(SendPackage package)     //向AGV发送消息
        {
            List<byte> byteSource = new List<byte>();
            byteSource.AddRange(DataTransform.shortToByte(package.Head));
            byteSource.AddRange(DataTransform.intToByte(package.TaskID));
            byteSource.AddRange(DataTransform.shortToByte(package.TaskStepID));
            byteSource.AddRange(DataTransform.shortToByte(package.TaskType));
            byteSource.AddRange(DataTransform.shortToByte(package.ProductType));
            byteSource.AddRange(DataTransform.shortToByte(package.Priority));
            byteSource.AddRange(DataTransform.shortToByte(package.CarType));
            byteSource.AddRange(DataTransform.shortToByte(package.CarID));
            byteSource.AddRange(DataTransform.shortToByte(package.NextTask));
            byteSource.AddRange(DataTransform.shortToByte(package.Parameter));
            byteSource.AddRange(DataTransform.shortToByte(package.MapID1));
            byteSource.AddRange(DataTransform.shortToByte(package.MapID2));
            byte[] data = byteSource.ToArray();
            byte CKCode = 0;
            for (int i = 0; i < data.Length; i++)
            {
                CKCode += data[i];
            }
            byteSource.Add(CKCode); //校验码
            byte[] senddata = byteSource.ToArray();
            SendAGVMsg(senddata,package);     
        }
        //发送AGV消息
        private void SendAGVMsg(byte[] data,SendPackage package)
        {
            lock (lock1)
            {
                if (!AGVDISCONNECTED)
                {
                    AGVclient.Send(data);
                    string info = "向AGV发送指令：头" + package.Head + " 任务号" + package.TaskID + " 取货点" + package.MapID1 + " 卸货点" + package.MapID2+" 急停信号"+package.ESSignal;
                    LogInfoHelp.GetInstance().WriteInfoToLogFile(info, LogInfoHelp.LOG_TYPE.LOG_INFO);
                }
                else
                {
                    MessageBox.Show("AGV通讯错误，请检查AGV是否连接或者由于某种原因断开");
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("AGV通讯错误，请检查AGV是否连接或者由于某种原因断开", LogInfoHelp.LOG_TYPE.LOG_WARN);
                }
            }
        }
        #endregion

        #region WH客户端处理函数
        public static bool WHDISCONNECTED = true;   //如果客户端已断开，则禁止再向服务器发送消息
        private void WHStartSocketClient()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ConfigClass.WH_IPADD), ConfigClass.WH_PORT);
            WHClient.Error += new EventHandler<ErrorEventArgs>(WHClient_Error);
            WHClient.Initialize(new WHSocketRequestFilter(), (request) =>
            {
                WHOnMessageReceive(request);
            });
            WHClient.Connected += new EventHandler(WHClient_Connected);
            WHClient.Closed += new EventHandler(WHClient_Closed);
            WHClient.ConnectAsync(endPoint);
        }
        private void WHCloseSocketClient()
        {         
            WHClient.Close();
        }
        private void WHClient_Error(object sender,EventArgs e)
        {
            whUIHandle.WH_COM_STATE = "连接错误";
            LogInfoHelp.GetInstance().WriteInfoToLogFile("立库连接错误", LogInfoHelp.LOG_TYPE.LOG_ERROR);
            ExecuteOrder.RunExecuteWHCmdThread = false;   //停止立库执行线程
            //重置立库指令表
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            db.ResetWHComState(con);
            WHDISCONNECTED = true;
        }
        private void WHClient_Closed(object sender,EventArgs e)
        {
            ExecuteOrder.RunExecuteWHCmdThread = false;   //停止立库执行线程  
            WHClient.Error -= new EventHandler<ErrorEventArgs>(WHClient_Error);
            WHClient.Connected -= new EventHandler(WHClient_Connected);
            WHClient.Closed -= new EventHandler(WHClient_Closed);
            //重置立库指令表
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            WHDISCONNECTED = true;
            Thread.Sleep(TimeSpan.FromSeconds(3));
            whUIHandle.WH_COM_STATE = "连接断开";
            whUIHandle.WH_BUTTON = "连接立库";
            LogInfoHelp.GetInstance().WriteInfoToLogFile("立库连接断开", LogInfoHelp.LOG_TYPE.LOG_WARN);

         
        }
        private void WHClient_Connected(Object sender, EventArgs e)
        {          
            WHDISCONNECTED = false;
            //启动立库执行线程
            DataBaseHandle db = new DataBaseHandle();
            db.SelectWHCmdEvent = this.WHSendMsg;    //将立库发送消息函数注册到查询立库【通讯状态】字段的事件中
            WHExecuteThread = new Thread(new ParameterizedThreadStart(ExecuteOrder.ExecuteWHCmd));
            WHExecuteThread.Start(db);
            WHExecuteThread.IsBackground = true;
            whUIHandle.WH_COM_STATE = "连接成功";
            whUIHandle.WH_BUTTON = "立库断开";
            LogInfoHelp.GetInstance().WriteInfoToLogFile("立库连接成功", LogInfoHelp.LOG_TYPE.LOG_INFO);
        }
        //收到立库的消息 
        private void WHOnMessageReceive(WHPackageInfo package)
        {
            WHMsgHandle msgHandle = new WHMsgHandle(package);
            msgHandle.OCDelegate += this.AllOrderCompleted;
            msgHandle.UDDelegate += this.CurrentUpDownComplete;
            msgHandle.SelectMode(MainWindow.WHOperState);
        }
        //向立库发送消息
        private void WHSendMsg(WHSendPackage package)
        {
            lock (lock0)
            {
                List<byte> byteSource = new List<byte>();
                byteSource.AddRange(DataTransform.shortToByte(package.Head));
                byteSource.AddRange(DataTransform.intToByte(package.SerialNumber));
                byteSource.AddRange(DataTransform.shortToByte(package.WHPostion));
                byteSource.AddRange(DataTransform.shortToByte(package.TrayID));
                byte[] data = byteSource.ToArray();
                byte CKCode = 0;
                for (int i = 0; i < data.Length; i++)
                {
                    CKCode += data[i];
                }
                byteSource.Add(CKCode);  //校验码
                byte[] sendData = byteSource.ToArray();
                if (!WHDISCONNECTED)
                {
                    WHClient.Send(sendData);
                    string info="向立库发送指令：头"+package.Head+" 流水号"+package.SerialNumber+" 库位号"+package.WHPostion+" 托盘ID"+package.TrayID;
                    LogInfoHelp.GetInstance().WriteInfoToLogFile(info, LogInfoHelp.LOG_TYPE.LOG_INFO);
                }
                else
                {
                    MessageBox.Show("立库通讯错误，请检查立库是否连接或者由于某种原因断开");
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("立库通讯错误，请检查立库是否连接或者由于某种原因断开", LogInfoHelp.LOG_TYPE.LOG_WARN);
                }
            }
        }
        #endregion

        //所有订单完成后的动作
        private void AllOrderCompleted()
        {
            //所有订单都完成了
            //对下订单和执行订单两个按钮解禁。
            MainWindow.D_Order_Enable = false;
            MainWindow.P_Order_Enable = false;
            MainWindow.A1_Order_Enable = false;
            MainWindow.A2_Order_Enable = false;
            MainWindow.firstCome = false;   //对执行订单按钮的第一次进入标识符进行解禁。
            MainWindow.AGVOperState = 0;  //将自动操作恢复为默认
            MainWindow.WHOperState = 0;   //将自动操作恢复为默认
            ExecuteOrder.ExecuteFlag = false;   //停止向数据库查询AGV和立库要发送的指令
            ExecuteOrder.SendAGVCmdEnable = false;
            ExecuteOrder.SendWHCmdEnable = false;
            LogInfoHelp.GetInstance().WriteInfoToLogFile("所有订单执行完毕", LogInfoHelp.LOG_TYPE.LOG_INFO);
        }

        //当前上料订单完成后的动作,恢复上下料按钮功能，允许进行其他订单操作
        private void CurrentUpDownComplete()
        {
            MainWindow.AGVOperState = 0;  //将上下料操作恢复为默认
            MainWindow.WHOperState = 0;  //将上下料操作恢复为默认
            btn_Hand_Up_Tray.IsEnabled = true;
            btn_Hand_Down_Tray.IsEnabled = true;
            ExecuteOrder.SendAGVCmdEnable = false;
            ExecuteOrder.SendWHCmdEnable = false;
            ExecuteOrder.HandAndDownFlag = false;  //停止向数据库查询AGV和立库要发送的指令
        }

        # region 配置文件
        private void InitialConfig()
        {
            //读取配置文件
            ReadWriteXml.GetConfigInfo();
            //读取库位
            DataBaseHandle database = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            //database.UpdateKuweiSuccess += this.CalculateKuwei;  //将库位显示注册到已初始化库位成功事件中
            database.UpdateKuweiArray(con);
               
        }

        private void SaveConfig()
        {
            //保存配置文件
            ReadWriteXml.SetConfigTaskID();
        }

        #endregion
        
        #region AGV和立库的连接按钮事件
        /// AGV按钮启动关闭事件
        private void btn_AGV_Start_Close_Click(object sender, RoutedEventArgs e)
        {
            if (btn_AGV_Start_Close.Content.ToString()=="连接AGV")
            {
                AGVOperState = 0;  ////将AGV的操作模式置为默认。
                AGVStartSocketClient();   //启动AGV客户端   
            }
            if (btn_AGV_Start_Close.Content.ToString() == "AGV断开")
            {
                AGVCloseSocketClient();   //关闭AGV客户端
            }
        }
        /// 立库按钮启动关闭事件
        private void btn_WH_Start_Close_Click(object sender, RoutedEventArgs e)
        {
            if (btn_WH_Start_Close.Content.ToString() == "连接立库")
            {
                WHOperState = 0;  //将立库的操作模式置为默认。
                WHStartSocketClient();   //启动立库客户端
                Thread.Sleep(TimeSpan.FromSeconds(1.5));
                WHCloseSocketClient();
                Thread.Sleep(TimeSpan.FromSeconds(1.5));
                WHStartSocketClient();   //启动  //这里启动两次才能连上我也不是很清楚
            }
            if (btn_WH_Start_Close.Content.ToString() == "立库断开")
            {
                WHCloseSocketClient();   //关闭立库客户端
            }
        }

        #endregion

        # region Window Events
        /// Window Close
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            ExecuteOrder.RunExecuteAGVCmdThread = false;
            ExecuteOrder.RunExecuteWHCmdThread = false;
            CloseSocketServer();  //关闭服务器
            WHCloseSocketClient();  //关闭WH客户端
            AGVCloseSocketClient();  //关闭AGV客户端
            SaveConfig();
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            //删除AGV指令表中未完成的指令
            db.DeleteAGVCmdTable(con);
            //删除立库中未完成的指令
            db.DeleteWHCmdTable(con);
            //将库位选中状态置为0
            db.UpdateSelectState(con);
            //删除订单表中未完成的订单
            db.UpdateUnCompleteOrder(con);
            LogInfoHelp.GetInstance().WriteInfoToLogFile("窗口关闭", LogInfoHelp.LOG_TYPE.LOG_INFO);
            this.Close();         
        }

        /// Window Minimize Events
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// 窗口加载函数
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogInfoHelp.GetInstance().CreateLogFile();   //创建log文件。
            InitialConfig();    //从配置文件和数据库中读取配置信息
            StartSocketServer();   //启动服务器   
            ResetFunction();  
            LogInfoHelp.GetInstance().WriteInfoToLogFile("窗口加载成功",LogInfoHelp.LOG_TYPE.LOG_INFO);
        }

        #endregion

        #region 订单事件
        //执行订单按钮
        private void btn_Execute_order_Click(object sender, RoutedEventArgs e)
        {
            if (AGVDISCONNECTED == true)
            {
                MessageBox.Show("AGV未连接，请连接AGV");
                return;
            }
            if (WHDISCONNECTED == true)
            {
                MessageBox.Show("立库未连接，请连接立库");
                return;
            }
            if (MainWindow.AGVOperState != 1 && MainWindow.AGVOperState != 0 && MainWindow.WHOperState != 1 && MainWindow.WHOperState != 0)
            {
                MessageBox.Show("有正在执行的人工上下料事件或者手动操作事件，请稍后再试");
                return;
            }
            if (btn_Execute_order.Content.ToString() == "执行订单")
            {
                MainWindow.WHOperState = 1;
                MainWindow.AGVOperState = 1;
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();

                #region 加工订单

                if (cb_OrderStyle.Text == "加工订单")
                {            
                    if (orderstate.P_Order != "")
                    {
                        MessageBox.Show("当前加工订单正在执行中，不允许重复执行订单");
                        return;
                    }
                    int P_Order = db.IsExistOrderStyle(con, "加工订单");
                    if (P_Order > 0)
                    {
                        orderstate.P_Order = "加工订单正在执行";
                        DTUSendPackage.P_Order_Enable = 1;
                        DTUSendPackage.Hand_Order_Enabel = 0;
                    }
                    else
                    {
                        WHOperState = 0;
                        AGVOperState = 0;
                        orderstate.P_Order = "";
                        DTUSendPackage.P_Order_Enable = 0;
                        DTUSendPackage.Hand_Order_Enabel = 1;
                        MessageBox.Show("请先下单");
                        return;
                    }
                    if (firstCome == false)
                    {
                        ExecuteOrder.ExecuteFlag = true;
                        ExecuteOrder.SendWHCmdEnable = true;
                        ExecuteOrder.SendAGVCmdEnable = true;
                        firstCome = true;
                    }
                    if (P_Order > 0)
                    {
                        if (StateMachine.P_Process_Area1 == 0 && MainWindow.WHMatchingProcess1 == false && StateMachine.P_System_State == 2)
                        {
                            db.InsertWHCmd(con, "加工订单", "侧边出库", ConfigClass.Tray_A1);   //侧边出库加工毛坯托盘A1
                            db.InsertWHCmd(con, "加工订单", "出库", ConfigClass.Tray_A0);    //出库加工空托盘A0
                            db.UpdatePreOutlibNum(con, "加工订单");
                            LogInfoHelp.GetInstance().WriteInfoToLogFile("向立库插入加工订单", LogInfoHelp.LOG_TYPE.LOG_INFO);
                            WHMatchingProcess1 = true;
                            P_Order_Enable = true;
                            return;
                        }
                        else if (StateMachine.P_Process_Area2 == 0 && MainWindow.WHMatchingProcess2 == false && StateMachine.P_System_State == 2)
                        {
                            db.InsertWHCmd(con, "加工订单", "侧边出库", ConfigClass.Tray_A1);   //侧边出库加工毛坯托盘A1
                            db.InsertWHCmd(con, "加工订单", "出库", ConfigClass.Tray_A0);    //出库加工空托盘A0
                            db.UpdatePreOutlibNum(con, "加工订单");
                            LogInfoHelp.GetInstance().WriteInfoToLogFile("向立库插入加工订单", LogInfoHelp.LOG_TYPE.LOG_INFO);
                            WHMatchingProcess2 = true;
                            P_Order_Enable = true;
                            return;
                        }
                        else
                        {
                            WHOperState = 0;
                            AGVOperState = 0;
                            orderstate.P_Order = "";
                            DTUSendPackage.P_Order_Enable = 0;
                            DTUSendPackage.Hand_Order_Enabel = 1;
                            MessageBox.Show("执行订单任务失败，请检查是否加工区两个区域都有托盘或者加工区PLC状态未处于【系统正常 已启动】状态");
                        }
                        return;
                    }
                }

                #endregion

                #region 检测订单

                if (cb_OrderStyle.Text == "检测订单")
                {          
                    if (orderstate.D_Order !="")
                    {
                        MessageBox.Show("当前检测订单正在执行中，不允许重复执行订单");
                        return;
                    }
                    int D_Order = db.IsExistOrderStyle(con, "检测订单");
                    if (D_Order > 0)
                    {
                        orderstate.D_Order = "检测订单正在执行";
                        DTUSendPackage.D_Order_Enable = 1;
                    }
                    else
                    {
                        WHOperState = 0;
                        AGVOperState = 0;
                        orderstate.D_Order = "";
                        DTUSendPackage.D_Order_Enable = 0;
                        MessageBox.Show("请先下单");
                        return;
                    }
                    if (firstCome == false)
                    {
                        ExecuteOrder.ExecuteFlag = true;
                        ExecuteOrder.SendWHCmdEnable = true;
                        ExecuteOrder.SendAGVCmdEnable = true;
                        firstCome = true;
                    }
                    if (D_Order > 0)
                    {
                        if (StateMachine.D_Detection_Area1 == 0 && MainWindow.WHMatchingDetection1 == false && StateMachine.D_System_State == 2)
                        {
                            db.InsertWHCmd(con, "检测订单", "出库", ConfigClass.Tray_A2);   //出库加工成品托盘A2托盘
                            db.UpdatePreOutlibNum(con, "检测订单");
                            LogInfoHelp.GetInstance().WriteInfoToLogFile("向立库插入检测订单", LogInfoHelp.LOG_TYPE.LOG_INFO);
                            WHMatchingDetection1 = true;
                            D_Order_Enable = true;
                            return;
                        }
                        else if (StateMachine.D_Detection_Area2 == 0 && MainWindow.WHMatchingDetection2 == false && StateMachine.D_System_State == 2)
                        {
                            db.InsertWHCmd(con, "检测订单", "出库", ConfigClass.Tray_A2);   //出库加工成品托盘A2托盘
                            db.UpdatePreOutlibNum(con, "检测订单");
                            LogInfoHelp.GetInstance().WriteInfoToLogFile("向立库插入检测订单", LogInfoHelp.LOG_TYPE.LOG_INFO);
                            WHMatchingDetection2 = true;
                            D_Order_Enable = true;
                            return;
                        }
                        else
                        {
                            WHOperState = 0;
                            AGVOperState = 0;
                            orderstate.D_Order = "";
                            DTUSendPackage.D_Order_Enable = 0;
                            MessageBox.Show("执行订单任务失败，请检查是否检测区两个区域都有托盘或者检测区PLC状态未处于【系统正常 已启动】状态");

                        }
                    }
                }

                #endregion

                #region 拧螺丝订单
                if (cb_OrderStyle.Text == "拧螺丝订单")
                {              
                    if (orderstate.A1_Order != "")
                    {
                        MessageBox.Show("当前拧螺丝订单正在执行中，不允许重复执行订单");
                        return;
                    }
                    int A1_Order = db.IsExistOrderStyle(con, "拧螺丝订单");
                    if (A1_Order > 0)
                    {
                        orderstate.A1_Order = "拧螺丝订单正在执行";
                        DTUSendPackage.A1_Order_Enable = 1;
                    }
                    else
                    {
                        AGVOperState = 0;
                        WHOperState = 0;
                        orderstate.A1_Order = "";
                        DTUSendPackage.A1_Order_Enable = 0;
                        MessageBox.Show("请先下单");
                        return;
                    }
                    if (firstCome == false)
                    {
                        ExecuteOrder.ExecuteFlag = true;
                        ExecuteOrder.SendWHCmdEnable = true;
                        ExecuteOrder.SendAGVCmdEnable = true;
                        firstCome = true;
                    }
                    if (A1_Order > 0)
                    {
                        if (StateMachine.A_Assembly_Area1 == 0 && StateMachine.A_Assembly_InArea1 == 0 && MainWindow.WHMatchingAssembly1 == false && StateMachine.A_System_State == 2)
                        {
                            db.InsertWHCmd(con, "拧螺丝订单", "出库", ConfigClass.Tray_B1);  //出库拧螺钉打钉托盘B1
                            db.UpdatePreOutlibNum(con, "拧螺丝订单");
                            LogInfoHelp.GetInstance().WriteInfoToLogFile("向立库插入拧螺丝订单", LogInfoHelp.LOG_TYPE.LOG_INFO);
                            WHMatchingAssembly1 = true;
                            A1_Order_Enable = true;
                            return;
                        }
                        else
                        {
                            AGVOperState = 0;
                            WHOperState = 0;
                            orderstate.A1_Order = "";
                            DTUSendPackage.A1_Order_Enable = 0;
                            MessageBox.Show("执行订单任务失败，请检查是否拧螺丝区有托盘或者装配区PLC状态未处于【系统正常 已启动】状态");
                        }
                    }

                }
                #endregion

                #region 轴承压装订单

                if (cb_OrderStyle.Text == "轴承压装订单")
                {
                    if (orderstate.A2_Order != "")
                    {
                        MessageBox.Show("当前轴承压装订单正在执行，不允许重复执行订单");
                        return;
                    }
                    int A2_Order = db.IsExistOrderStyle(con, "轴承压装订单");
                    if (A2_Order > 0)
                    {
                        orderstate.A2_Order = "轴承压装订单正在执行";
                        DTUSendPackage.A2_Order_Enable = 1;
                    }
                    else
                    {
                        AGVOperState = 0;
                        WHOperState = 0;
                        orderstate.A2_Order = "";
                        DTUSendPackage.A2_Order_Enable = 0;
                        MessageBox.Show("请先下单");
                        return;
                    }
                    if (firstCome == false)
                    {
                        ExecuteOrder.ExecuteFlag = true;
                        ExecuteOrder.SendWHCmdEnable = true;
                        ExecuteOrder.SendAGVCmdEnable = true;
                        firstCome = true;
                    }
                    if (A2_Order > 0)
                    {
                        if (StateMachine.A_Assembly_Area2 == 0 && StateMachine.A_Assembly_InArea2 == 0 && MainWindow.WHMatchingAssembly2 == false && StateMachine.A_System_State == 2)
                        {
                            db.InsertWHCmd(con, "轴承压装订单", "出库", ConfigClass.Tray_A4);  //出库轴承压装毛坯托盘A4
                            db.UpdatePreOutlibNum(con, "轴承压装订单");
                            LogInfoHelp.GetInstance().WriteInfoToLogFile("向立库插入轴承压装订单", LogInfoHelp.LOG_TYPE.LOG_INFO);
                            WHMatchingAssembly2 = true;
                            A2_Order_Enable = true;
                            return;
                        }
                        else
                        {
                            AGVOperState = 0;
                            WHOperState = 0;
                            orderstate.A2_Order = "";
                            DTUSendPackage.A2_Order_Enable = 0;
                            MessageBox.Show("执行订单任务失败，请检查是否轴承压装区有托盘或者装配区PLC状态未处于【系统正常 已启动】状态");
                        }
                    }
                  #endregion


                }
            }
        }

        /// 下订单事件
        private void btn_Booking_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection();
            DataBaseHandle db = new DataBaseHandle();
            string orderstyle=cb_OrderStyle.Text;
            int ordernum=Convert.ToInt32(cb_OrderNum.Text);

            #region 下加工订单
            //如果是加工订单，要判断订单数量是否小于等于空托盘和毛胚托盘
            if (orderstyle == "加工订单")
            {
                int number = db.IsExistOrderStyle(con, orderstyle);
                if (number == 0)
                {
                    if (ordernum <= kuweinumber.Tray_A0 && ordernum <= kuweinumber.Tray_A1)
                    {
                        db.InserOrderData(con, orderstyle, ordernum);
                        LogInfoHelp.GetInstance().WriteInfoToLogFile("下加工订单"+ordernum, LogInfoHelp.LOG_TYPE.LOG_INFO);
                    }
                    else
                    {
                        if (kuweinumber.Tray_A0 <= kuweinumber.Tray_A1)
                        {
                            MessageBox.Show("库位中的空托盘和毛坯托盘数量不足，实际库中数量为" + kuweinumber.Tray_A0);
                        }
                        else
                        {
                            MessageBox.Show("库位中的加工空托盘A0和加工毛坯托盘A1数量不足，实际库中数量为" + kuweinumber.Tray_A1);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("已有加工订单在执行中，请等待本次加工订单完成后再次下单");
                }
                return;
            }

            #endregion

            #region 下检测订单
            //如果是检测订单，要判断订单数量是否小于等于加工成品托盘
            if (orderstyle == "检测订单")
            {
                int number = db.IsExistOrderStyle(con, orderstyle);
                if (number == 0)
                {
                    if (ordernum <= kuweinumber.Tray_A2)
                    {
                        db.InserOrderData(con, orderstyle, ordernum);
                        LogInfoHelp.GetInstance().WriteInfoToLogFile("下检测订单" + ordernum, LogInfoHelp.LOG_TYPE.LOG_INFO);
                    }
                    else
                    {
                        MessageBox.Show("库位中的加工成品托盘A2数量不足，实际库中数量为" + kuweinumber.Tray_A2);
                    }
                }
                else
                {
                    MessageBox.Show("已有检测订单在执行中，请等待本次检测订单完成后再次下单");
                }
                return;
            }

            #endregion

            #region 下轴承压装订单
            //如果是轴承压装订单，要判断订单数量是否小于等于轴承压装毛坯托盘A4
            if (orderstyle == "轴承压装订单")
            {
                int number = db.IsExistOrderStyle(con, orderstyle);
                if (number == 0)
                {
                    if (ordernum <= kuweinumber.Tray_A4)
                    {
                        db.InserOrderData(con, orderstyle, ordernum);
                        LogInfoHelp.GetInstance().WriteInfoToLogFile("下轴承压装订单" + ordernum, LogInfoHelp.LOG_TYPE.LOG_INFO);
                    }
                    else
                    {
                        MessageBox.Show("库位中的轴承压装毛坯托盘A4数量不足，实际库中数量为" + kuweinumber.Tray_A4);
                    }
                }
                else
                {
                    MessageBox.Show("已有轴承压装订单在执行中，请等待本次轴承压装订单完成后再次下单");
                }
                return;
            }

            #endregion

            #region 下拧螺丝订单
            //如果是拧螺丝订单，要判断订单数量是否小于等于拧螺丝打钉托盘B1
            if (orderstyle == "拧螺丝订单")
            {
                int number = db.IsExistOrderStyle(con, orderstyle);
                if (number == 0)
                {
                    if (ordernum <= kuweinumber.Tray_B1)
                    {
                        db.InserOrderData(con, orderstyle, ordernum);
                        LogInfoHelp.GetInstance().WriteInfoToLogFile("下拧螺丝订单" + ordernum, LogInfoHelp.LOG_TYPE.LOG_INFO);
                    }
                    else
                    {
                        MessageBox.Show("库位中的拧螺丝打钉托盘B1数量不足，实际库中数量为" + kuweinumber.Tray_B1);
                    }
                }
                else
                {
                    MessageBox.Show("已有拧螺丝订单在执行中，请等待本次拧螺丝订单完成后再次下单");
                }
                return;
            }

            #endregion
        }

        #endregion

        #region  人工上下料事件

        /// <summary>
        /// 人工上料的按钮事件，根据选择的料盘型号和参数来改变状态将其
        /// 返给控制台。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Hand_Up_Tray_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.WHOperState == 0 && MainWindow.AGVOperState == 0)
            {               
                if (AGVDISCONNECTED == true)
                {
                    MessageBox.Show("AGV未连接，请连接AGV");
                    return;
                }
                if (WHDISCONNECTED == true)
                {
                    MessageBox.Show("立库未连接，请连接立库");
                    return;
                }
                DTUSendPackage.Hand_Order_Enabel = 1;  //人工上下料订单允许
                DTUSendPackage.OrderControl = 1;  //订单控制
                DTUSendPackage.Maunal_Up_RFID = (byte)(Hand_Up_TrayStyle.SelectedIndex + 1);
                if (Hand_Up_TrayStyle.SelectedIndex == 11 || Hand_Up_TrayStyle.SelectedIndex == 10)
                {
                    if (Tray_parameter.Text == "")
                    {
                        MessageBox.Show("请填写托盘参数");
                        return;
                    }
                    DTUSendPackage.Manual_Up_RFID_Para = Convert.ToSingle(Tray_parameter.Text);
                }
                //将这种状态保持5s的线程，然后置0
                Thread t = new Thread(delegate()
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                        SetOriginNull();
                    });

                MainWindow.WHOperState = 2;  
                MainWindow.AGVOperState = 2;
                LogInfoHelp.GetInstance().WriteInfoToLogFile("开启人工上下料模式", LogInfoHelp.LOG_TYPE.LOG_INFO);
                btn_Hand_Down_Tray.IsEnabled = false;  //按钮使能失效
                btn_Hand_Up_Tray.IsEnabled = false;
                //这里不需要了，由加工区PLC来通知什么时候插入命令。
                //向AGV表中插入命令，通知其来取货
                //DataBaseHandle db = new DataBaseHandle();
                //SqlConnection con = new SqlConnection();
                //起点是人工上料区，终点是立库入库区
                //db.InsertAGVCmd(con, ConfigClass.HandUpArea, ConfigClass.WHInlibArea, Hand_Up_TrayStyle.SelectedIndex + 1);
                ExecuteOrder.SendAGVCmdEnable = true;
                ExecuteOrder.SendWHCmdEnable = true;
                ExecuteOrder.HandAndDownFlag = true;
            }
            else
            {
                MessageBox.Show("有订单或者手动操作事件可能正在执行，请稍后再试");
            }
        }
        private void SetOriginNull()
        {
            DTUSendPackage.OrderControl = 0;
            DTUSendPackage.Maunal_Up_RFID = 0;
            DTUSendPackage.Manual_Up_RFID_Para = 0;
        }

        private void btn_Hand_Down_Tray_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AGVOperState == 0 && MainWindow.WHOperState == 0)
            {                
                if (AGVDISCONNECTED == true)
                {
                    MessageBox.Show("AGV未连接，请连接AGV");
                    return;
                }
                if (WHDISCONNECTED == true)
                {
                    MessageBox.Show("立库未连接，请连接立库");
                    return;
                }
                LogInfoHelp.GetInstance().WriteInfoToLogFile("开启人工上下料模式", LogInfoHelp.LOG_TYPE.LOG_INFO);
                MainWindow.WHOperState = 2;
                MainWindow.AGVOperState = 2;
                btn_Hand_Down_Tray.IsEnabled = false;  //按钮使能失效
                btn_Hand_Up_Tray.IsEnabled = false;
                //向立库表插入命令，通知其出货
                DTUSendPackage.Hand_Order_Enabel = 1;  //人工上下料订单允许
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                int TrayStyle = Hand_Down_TrayStyle.SelectedIndex + 1;
                db.InsertWHCmd(con, "人工订单", "出库", TrayStyle);
                ExecuteOrder.SendAGVCmdEnable = true;
                ExecuteOrder.SendWHCmdEnable = true;
                ExecuteOrder.HandAndDownFlag = true;
            }
            else
            {
                MessageBox.Show("有订单或者手动操作事件可能正在执行，请稍后再试");
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// Set Logged-In User
        /// </summary>
        public string LoggedInUser
        {
            get
            {
                return this.txtUser.Text;
            }
            set
            {
                this.txtUser.Text = value;
            }
        }

        public string Designation
        {
            get
            {
                return this.txtDesignation.Text;
            }
            set
            {
                this.txtDesignation.Text = value;
            }
        }

        #endregion

        #region 手动操作
        private void btn_Hand_WH_OK_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.WHOperState == 0)
            {               
                if (WHDISCONNECTED == true)
                {
                    MessageBox.Show("立库未连接，请连接立库");
                    return;
                }
                if (Hand_Position.Text == "")
                {
                    MessageBox.Show("请选择库位号");
                    return;
                }
                string action = Hand_Action_Style.Text;
                WHSendPackage package = new WHSendPackage();
                if (action == "出库")
                {
                    package.Head = 10003;
                }
                if (action == "入库")
                {
                    package.Head = 20003;
                }
                if (action == "侧边出库")
                {
                    package.Head = 30003;
                }
                LogInfoHelp.GetInstance().WriteInfoToLogFile("开启手动操作模式", LogInfoHelp.LOG_TYPE.LOG_INFO);
                package.SerialNumber = 10000;  //手动操作立库的专有流水号
                package.WHPostion = Convert.ToUInt16(Hand_Position.Text);
                if (package.WHPostion > 80 || package.WHPostion < 1)
                {
                    MessageBox.Show("请输入正确的库位，库位范围在1-80内");
                    return;
                }
                if (package.WHPostion == 44 || package.WHPostion == 45)
                {
                    MessageBox.Show("输入库位为禁用库位，禁止输入");
                    return;
                }
                package.TrayID = (ushort)(Hand_Tray_Style.SelectedIndex + 1);
                WHOperState = 3;   //手动操作
                WHSendMsg(package); //发送包
                whHandInfo.Position = package.WHPostion;
                whHandInfo.TrayStyle = package.TrayID;
            }
            else
            {
                MessageBox.Show("有订单或者人工上下料事件正在执行，请您稍后再试");
            }        
        }

        private void btn_Hand_AGV_OK_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.AGVOperState == 0)
            {
                if (AGVDISCONNECTED == true)
                {
                    MessageBox.Show("AGV未连接，请连接AGV");
                    return;
                }             
                int taskID = ReadWriteXml.ReadAndUpdateTaskID();
                if (taskID > 0)
                {
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("开启手动操作模式", LogInfoHelp.LOG_TYPE.LOG_INFO);
                    SendPackage package = new SendPackage();
                    package.TaskID = taskID;
                    package.MapID1 = Convert.ToUInt16(Hand_Map_Start.Text);
                    package.MapID2 = Convert.ToUInt16(Hand_Map_End.Text);
                    AGVOperState = 3;  //手动操作
                    AGVSendMsg(package);
                }
            }
            else
            {
                MessageBox.Show("有订单或者人工上下料事件正在执行，请您稍后再试");
            }
        }

        # endregion

        #region 历史纪录

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            string startTime = StartDatePicker1.SelectedDate.ToString();
            string endTime = EndDatePicker2.SelectedDate.ToString();
            if (startTime != "" && endTime != "" && startTime.Equals(endTime) == false)
            {
                string booking_style = cb_BookingStyle.Text;
                string searching_style = cb_SearchStyle.Text;
                if (searching_style == "订单数据")
                {
                    SqlConnection con = new SqlConnection();
                    DataBaseHandle db = new DataBaseHandle();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("OrderID", typeof(string));
                    dt.Columns.Add("OrderTime", typeof(string));
                    dt.Columns.Add("OrderStyle", typeof(string));
                    dt.Columns.Add("OrderNum", typeof(int));
                    dt.Columns.Add("OrderCompleteNum", typeof(int));
                    dt.Columns.Add("OrderState", typeof(string));
                    dt = db.SearchOrderData(con, startTime, endTime, booking_style);
                    dg_Order.ItemsSource = dt.DefaultView;
                }
                if (searching_style == "托盘数据")
                {
                    SqlConnection con = new SqlConnection();
                    DataBaseHandle db = new DataBaseHandle();
                    DataTable dt = new DataTable();
                    dt.Columns.Add("DetectionTime", typeof(string));
                    dt.Columns.Add("WorkArea", typeof(string));
                    dt.Columns.Add("TrayInArea1", typeof(string));
                    dt.Columns.Add("TrayInArea2", typeof(string));
                    dt.Columns.Add("FirstPara", typeof(float));
                    dt.Columns.Add("SecondPara", typeof(float));
                    dt.Columns.Add("ThirdPara", typeof(float));
                    dt.Columns.Add("FourPara", typeof(float));
                    dt.Columns.Add("FivePara", typeof(float));
                    dt = db.SearchDetectionData(con, startTime, endTime);
                    dg_detection.ItemsSource = dt.DefaultView;
                }
            }
            else
            {
                MessageBox.Show("请正确选择日期期间!!!");
            }
        }
        private void btn_export_data_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel exportexcel = new ExportToExcel();
            string searching_style = cb_SearchStyle.Text;
            if (searching_style == "托盘数据")
            {
                string title = "托盘数据";
                bool issucess = exportexcel.Export(dg_detection, title);
                if (issucess)
                {
                    MessageBox.Show("导出成功");
                }
                else
                {
                    MessageBox.Show("导出失败");
                }
            }
            else
            {
                string title = "订单数据";
                bool issucess = exportexcel.Export(dg_Order, title);
                if (issucess)
                {
                    MessageBox.Show("导出成功");
                }
                else
                {
                    MessageBox.Show("导出失败");
                }
            }
        }
        #endregion

        #region 库位管理按钮

        private void btn_Kuwei_mannger_Click(object sender, RoutedEventArgs e)
        {
            if (kuweinumber.QUANXIANMODE == "管理员模式")
            {
                KuweiShow win = new KuweiShow();
                win.Show();
            }
            if (kuweinumber.QUANXIANMODE == "普通用户模式")
            {
                LoginWindow win1 = new LoginWindow(ConfigClass.WHMANNGERSYS_PASSWORD,2);
                win1.Show();
            }
           
        }

        # endregion

        #region 重置按钮
        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            string hintString = "确定重置所有操作吗？此操作会使当前所有操作信息丢失。同时在进行此操作前请确保所有工作区都恢复到初始状态";
            MessageBoxResult result = MessageBox.Show(hintString, "重置提示", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
            if (result == MessageBoxResult.OK)
            {     
                ResetFunction();
                LogInfoHelp.GetInstance().WriteInfoToLogFile("重置按钮被按下", LogInfoHelp.LOG_TYPE.LOG_INFO);
            }
            else
            {
                //什么也不做
            }

        }

        private void ResetFunction()
        {
            //操作模式切换为默认
            MainWindow.AGVOperState = 0;
            MainWindow.WHOperState = 0;
            //重置执行订单按钮的第一次进入标识符
            MainWindow.firstCome = false;
            //重置生产概况中AGV和立库的执行状态
            agvUIHandle.AGV_POS_ACTION = "空";
            whUIHandle.WH_OPER_STATE = "空";
            //将订单执行状态恢复为空
            orderstate.P_Order = "";
            orderstate.D_Order = "";
            orderstate.A1_Order = "";
            orderstate.A2_Order = "";
            //自动模式下的按钮恢复初始状态
            btn_Execute_order.IsEnabled = true;
            btn_Booking.IsEnabled = true;
            btn_Execute_order.Content = "执行订单";
            //人工上下料模式的按钮恢复为初始状态
            btn_Hand_Down_Tray.IsEnabled = true;
            btn_Hand_Up_Tray.IsEnabled = true;
            //停止向立库和AGV指令表中查询指令
            ExecuteOrder.HandAndDownFlag = false;
            ExecuteOrder.ExecuteFlag = false;
            ExecuteOrder.SendAGVCmdEnable = true;
            ExecuteOrder.SendWHCmdEnable = true;
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            db.UpdateUnCompleteOrder(con);
            db.DeleteWHCmdTable(con);
            db.DeleteAGVCmdTable(con);
            db.UpdateSelectState(con);
            WHMatchingProcess1 = false;   //为true表示立库已经匹配了加工区出库1的位置不能在出此方面的库位
            WHMatchingProcess2 = false;   //为true表示立库已经匹配了加工区出库1的位置不能在出此方面的库位
            WHMatchingDetection1 = false;
            WHMatchingDetection2 = false;
            WHMatchingAssembly1 = false;
            WHMatchingAssembly2 = false;

            AGVMatchingAssembly1 = false;
            AGVMatchingAssembly2 = false;
            AGVMatchingDetection1 = false;
            AGVMatchingDetection2 = false;
            AGVMatchingProcess1 = false;
            AGVMatchingProcess2 = false;

            A1_Order_Enable = false;
            A2_Order_Enable = false;
            P_Order_Enable = false;
            D_Order_Enable = false;

            DTUSendPackage.Inform_P_Raw_Tray = 0;
            DTUSendPackage.Raw_Tray_Out = 0;
            DTUSendPackage.AGV_Manual_Down_GET = 0;
            DTUSendPackage.AGV_Manual_Down_PUT = 0;
            DTUSendPackage.AGV_Manual_Up_GET = 0;
            DTUSendPackage.AGV_Manual_Up_PUT = 0;
            DTUSendPackage.AGV_Process1_GET = 0;
            DTUSendPackage.AGV_Process1_PUT = 0;
            DTUSendPackage.AGV_Process2_GET = 0;
            DTUSendPackage.AGV_Process2_PUT = 0;
            DTUSendPackage.P_Order_Enable = 0;
            DTUSendPackage.Hand_Order_Enabel = 1;
            DTUSendPackage.OrderControl = 0;
            DTUSendPackage.D_Order_Enable = 0;
            DTUSendPackage.AGV_Detection1_GET = 0;
            DTUSendPackage.AGV_Detection1_PUT = 0;
            DTUSendPackage.AGV_Detection2_GET = 0;
            DTUSendPackage.AGV_Detection2_PUT = 0;
            DTUSendPackage.A1_Order_Enable = 0;
            DTUSendPackage.A2_Order_Enable = 0;
            DTUSendPackage.AGV_Assembly1_GET = 0;
            DTUSendPackage.AGV_Assembly1_PUT = 0;
            DTUSendPackage.AGV_Assembly2_GET = 0;
            DTUSendPackage.AGV_Assembly2_PUT = 0;
        }

        #endregion

        #region 急停区域

        /// <summary>
        /// 立库急停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_WH_ES_Click(object sender, RoutedEventArgs e)
        {
            WHSendPackage WHpackage = new WHSendPackage();
            WHpackage.Head = 19000;
            WHpackage.TrayID = 0;
            WHpackage.WHPostion = 0;
            WHpackage.SerialNumber = 0;
            WHSendMsg(WHpackage);
        }

        /// <summary>
        /// AGV急停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AGV_ES_Click(object sender, RoutedEventArgs e)
        {
            SendPackage pac = new SendPackage();
            pac.Head = 10012;
            pac.ESSignal = 5;  //急停
            AGVSendESMsg(pac);
        }

        /// <summary>
        /// AGV解急停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AGV_NES_Click(object sender, RoutedEventArgs e)
        {
            SendPackage pac = new SendPackage();
            pac.Head = 10012;
            pac.ESSignal = 6;  //解急停
            AGVSendESMsg(pac);
        }
        #endregion


        # region 用户权限
        private void btn_User_mannger_Click(object sender, RoutedEventArgs e)
        {
            UserWindow user = new UserWindow();
            user.Show();
        }
        # endregion









    }
}
