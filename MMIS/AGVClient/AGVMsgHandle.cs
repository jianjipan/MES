using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Threading;
using System.Threading;

namespace MMIS
{
    class AGVMsgHandle
    {
        PackageInfo package = new PackageInfo();
      //  private bool flag1 = true;   //防止定时器重复触发的标识符
        private DispatcherTimer StateTimer = new DispatcherTimer();  //AGV的状态定时器
        Thread InformWHThread = null;   //通知立库出库的线程
        private bool InformWHThreadState = false;   //线程的状态
        public delegate void UpDownCompleteDelegate();  //当前上下料订单完成后的触发事件
        public UpDownCompleteDelegate UDDelegate;
        public delegate void SendESDelegate(SendPackage package);
        public SendESDelegate sendESDelegate;   //向AGV发送挂起和解挂得事件
        public AGVMsgHandle(PackageInfo package)
        {
            this.package = package;     
        }
        public void SelectMode(int AGVOperState)
        {
            if (AGVOperState == 3)   //手动操作的处理方法
            {
                HandOperate();
            }
            if (AGVOperState == 2)
            {
                ManualOperate();
            }
            if (AGVOperState == 1)                            //自动操作的处理方法
            {
                AutoOperate();
            }
        }
        //手动操作AGV时收到消息的处理方法
        private void HandOperate()
        {
            if (package.Head == 10003)   //收到消息
            {
                MainWindow.agvUIHandle.AGV_POS_ACTION = "AGV已收到消息";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("AGV已收到消息", LogInfoHelp.LOG_TYPE.LOG_INFO);
                return;
            }
            if (package.Head == 10005)  //执行指令完成
            {
                if (package.TaskStepID == 0)   //执行取货任务完成
                {
                    MainWindow.agvUIHandle.AGV_POS_ACTION = "AGV取货动作完毕";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("AGV取货动作完毕", LogInfoHelp.LOG_TYPE.LOG_INFO);
                }
                if (package.TaskStepID == 1)  //执行卸货任务完成
                {
                    MainWindow.agvUIHandle.AGV_POS_ACTION = "AGV卸货动作完毕";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("AGV卸货动作完毕", LogInfoHelp.LOG_TYPE.LOG_INFO);
                    MainWindow.AGVOperState = 0;
                    return;
                }
            }
        }

        //执行订单（自动操作）AGV时收到消息的处理方法
        private void AutoOperate()
        {
            if (package.Head == 10003)   //收到消息
            {
                ExecuteOrder.SendAGVCmdEnable = false;
                MainWindow.agvUIHandle.AGV_POS_ACTION = "AGV已收到消息";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("AGV已收到消息", LogInfoHelp.LOG_TYPE.LOG_INFO);
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateAGVComState(con, package.TaskID);   //更新AGV指令表中的通讯状态为【已收到】
                return;
            }
            if (package.Head == 10005)  //执行指令完成
            {
                if (package.TaskStepID == 0)   //执行取货任务完成
                {
                    //得到AGV的起点和终点
                    int StartPoint, EndPoint = 0;
                    DataBaseHandle db = new DataBaseHandle();
                    SqlConnection con = new SqlConnection();
                    StartPoint = db.SelectAGVStartEndPoint(con, package.TaskID, ref EndPoint);
                    MainWindow.agvUIHandle.AGV_POS_ACTION = ConfigClass.GetMapString(StartPoint) + "取货完毕";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile(MainWindow.agvUIHandle.AGV_POS_ACTION, LogInfoHelp.LOG_TYPE.LOG_INFO);
                    //如过是送往立库入库区来的，向加工PLC  装配PLC  检测PLC发布当前状态
                    //AGV在各个机构区域取托盘完毕
                    if (EndPoint == ConfigClass.WHInlibArea)
                    {
                        PublishAGVState(StartPoint, EndPoint);
                        Thread t = new Thread(delegate()
                        {                 
                            Thread.Sleep(new TimeSpan(0, 0, 5));
                            SetNull();
                        });
                        t.Start();
                        t.Priority = ThreadPriority.AboveNormal;
                        t.IsBackground = true;
                        //判断立库表指令是否超过两条，如果超过两条则挂起，否则不挂起
                        Thread th = new Thread(delegate()
                            {
                                SendPackage pac = new SendPackage();
                                while (true)
                                {
                                    bool IsNoPause = db.SelectWHCmdNum(con);  //查询立库指令表是否具备AGV挂起的条件
                                    if (IsNoPause == false)   //如果指令没超过一条
                                    {
                                        pac.Head = 10012;
                                        pac.ESSignal = 6;   //解急停
                                        sendESDelegate(pac);
                                        break;
                                    }
                                    //如果超过两条
                                    pac.Head = 10012;
                                    pac.ESSignal = 5;  //急停
                                    sendESDelegate(pac);
                                    Thread.Sleep(TimeSpan.FromSeconds(3));
                                }
                            });
                        th.Start();
                        th.Priority = ThreadPriority.AboveNormal;
                        th.IsBackground = true;
                    }
                    return;
                }
                if (package.TaskStepID == 1)  //执行卸货任务完成
                {

                    DataBaseHandle db = new DataBaseHandle();
                    SqlConnection con = new SqlConnection();
                    //得到AGV的起点和终点
                    int StartPoint, EndPoint = 0;
                    StartPoint = db.SelectAGVStartEndPoint(con, package.TaskID, ref EndPoint);
                    MainWindow.agvUIHandle.AGV_POS_ACTION = ConfigClass.GetMapString(EndPoint) + "卸货完毕";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile(MainWindow.agvUIHandle.AGV_POS_ACTION, LogInfoHelp.LOG_TYPE.LOG_INFO);
                    db.UpdateAGVWorkState(con, package.TaskID);   //更新AGV指令表中的通讯状态为【完成】   
                    ExecuteOrder.SendAGVCmdEnable = true;
                    //如果是从立库出库区来的，向加工PLC  装配PLC  检测PLC发布当前状态

                    //AGV在各个机构区域放托盘完毕
                    if (StartPoint == ConfigClass.WHOutlibArea1)
                    {
                       //发布AGV的状态给各个PLC
                        PublishAGVState(StartPoint, EndPoint);                  
                        //AGV在各工作区卸货完成后，检查是否还有订单,并且检查每个工作区是否有空闲，
                        //如果两个条件都满足，我们就通知立库继续出库（向立库指令表中插入出库信息）
                        //这里用线程，是为了防止接收到多次一样的消息从而重复通知立库的bug。
                        if (InformWHThreadState == false)  //防止线程重复创建
                        {
                            InformWHThread = new Thread(new ParameterizedThreadStart(CheckAndInformWH));
                            InformWHThread.IsBackground = true;
                            InformWHThreadState = true;
                            InformWHThread.Start(EndPoint);
                            InformWHThread.Priority = ThreadPriority.AboveNormal;
                        }
                        return;
                    }
                    //如过是送往立库入库区来的，则向立库表中插入入库指令信息
                    if (EndPoint == ConfigClass.WHInlibArea)
                    {
                        //AGV在立库入库区卸货完成，然后向立库表中插入入库指令信息
                        if (InformWHThreadState == false)  //防止线程重复创建
                        {
                            InformWHThread = new Thread(new ParameterizedThreadStart(InformWHInlib));
                            InformWHThread.IsBackground = true;
                            InformWHThreadState = true;
                            InformWHThread.Start(StartPoint);
                            InformWHThread.Priority = ThreadPriority.AboveNormal;
                        }
                        return;

                    }
                }
            }
        }

        //执行人工上下料时AGV收到消息的处理方法
        private void ManualOperate()
        {
            if (package.Head == 10003)   //收到消息
            {
                ExecuteOrder.SendAGVCmdEnable = false;
                MainWindow.agvUIHandle.AGV_POS_ACTION = "AGV已收到消息";
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateAGVComState(con, package.TaskID);   //更新AGV指令表中的通讯状态为【已收到】
                return;
            }
            if (package.Head == 10005)  //执行指令完成
            {
                if (package.TaskStepID == 0)   //执行取货任务完成
                {
                    //得到AGV的起点和终点
                    int StartPoint, EndPoint = 0;
                    DataBaseHandle db = new DataBaseHandle();
                    SqlConnection con = new SqlConnection();
                    StartPoint = db.SelectAGVStartEndPoint(con, package.TaskID, ref EndPoint);
                    MainWindow.agvUIHandle.AGV_POS_ACTION = ConfigClass.GetMapString(StartPoint) + "取货完毕";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile(MainWindow.agvUIHandle.AGV_POS_ACTION, LogInfoHelp.LOG_TYPE.LOG_INFO);
                }
                if (package.TaskStepID == 1)
                {
                    DataBaseHandle db = new DataBaseHandle();
                    SqlConnection con = new SqlConnection();
                    //得到AGV的起点和终点
                    int StartPoint, EndPoint = 0;
                    StartPoint = db.SelectAGVStartEndPoint(con, package.TaskID, ref EndPoint);
                    MainWindow.agvUIHandle.AGV_POS_ACTION = ConfigClass.GetMapString(EndPoint) + "卸货完毕";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile(MainWindow.agvUIHandle.AGV_POS_ACTION, LogInfoHelp.LOG_TYPE.LOG_INFO);
                    db.UpdateAGVWorkState(con, package.TaskID);   //更新AGV指令表中的通讯状态为【完成】
                    ExecuteOrder.SendAGVCmdEnable = true;
                    //AGV的起点是立库人工上料区,终点是立库入库区，则是人工上料任务，此时通知立库入库
                    if (StartPoint == ConfigClass.HandUpArea && EndPoint == ConfigClass.WHInlibArea)
                    {
                        //这里用线程，是为了防止接收到多次一样的消息从而重复通知立库的bug。
                        if (InformWHThreadState == false)  //防止线程重复创建
                        {
                            InformWHThread = new Thread(new ParameterizedThreadStart(InformInlib));
                            InformWHThread.IsBackground = true;
                            InformWHThreadState = true;
                            InformWHThread.Start(package.TaskID);
                            InformWHThread.Priority = ThreadPriority.AboveNormal;
                        }
                        return;
                    }
                    //AGV的起点是立库出库区，终点是人工下料区，则是人工下料任务，则AGV完成完成了任务后什么也不需要干了
                    if (StartPoint == ConfigClass.WHOutlibArea1 && EndPoint == ConfigClass.HandDownArea)
                    {
                        //当前人工下料订单完成 恢复上下料的按钮功能。
                        UDDelegate();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 人工上料订单，AGV动作执行完毕后，通知立库入库
        /// </summary>
        /// <param name="TaskID"></param>
        private void InformInlib(object TaskID)
        {
            int taskID = -1;
            if (TaskID != null)
            {
                taskID = Convert.ToInt32(TaskID);
            }
            Thread.Sleep(TimeSpan.FromSeconds(2));  ////等待几秒，防止重复接收消息导致重复向立库指令表中插入指令
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            int TrayStyle = db.SelectTrayStyleByTaskID(con, taskID);
            db.InsertWHCmd(con, "人工订单", "入库", TrayStyle);
            InformWHThreadState = false;
        }

        /// <summary>
        ///  //AGV在各工作区卸货完成后，检查是否还有订单（如果多于或大于2个，则通知立库出库，有1个说明有一个当前正在操作）,并且检查每个工作区是否有空闲，
        //如果两个条件都满足，我们就通知立库继续出库（向立库指令表中插入出库信息）
        /// </summary>
        private void CheckAndInformWH(object endpoint)
        {
            int EndPoint = 0;
            if (endpoint != null)
            {
                EndPoint = Convert.ToInt32(endpoint);
            }
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            Thread.Sleep(TimeSpan.FromSeconds(5));  // 发布AGV的状态给各个PLC保持5s
            SetNull();    
            if (StateMachine.P_Process_Area1 == 0&&MainWindow.WHMatchingProcess1==false&&StateMachine.P_System_State==2&&MainWindow.P_Order_Enable==true)
            {
                bool P_Outlib_Can = db.JudgeOutlib(con, "加工订单")&&db.JudgePreOutlibNum(con,"加工订单");  //判断加工订单是否还可以出库
                if (P_Outlib_Can == true)
                {
                    db.InsertWHCmd(con, "加工订单", "侧边出库", ConfigClass.Tray_A1);   //侧边出库加工毛坯托盘A1
                    db.InsertWHCmd(con, "加工订单", "出库", ConfigClass.Tray_A0);    //出库加工空托盘A0
                    db.UpdatePreOutlibNum(con, "加工订单");
                    MainWindow.WHMatchingProcess1 = true;
                    RefreshWHMatchingWorkArea(EndPoint);
                    RefreshAGVMatchingWorkArea(EndPoint);
                    InformWHThreadState = false;
                    return;
                }
            }
            if (StateMachine.D_Detection_Area1 == 0&&MainWindow.WHMatchingDetection1==false&&StateMachine.D_System_State==2&&MainWindow.D_Order_Enable==true)
            {
                bool D_Outlib_Can = db.JudgeOutlib(con, "检测订单")&&db.JudgePreOutlibNum(con,"检测订单");  //判断检测订单是否还可以出库
                if (D_Outlib_Can == true)
                {
                    db.InsertWHCmd(con, "检测订单", "出库", ConfigClass.Tray_A2);   //出库加工成品托盘A2托盘
                    db.UpdatePreOutlibNum(con, "检测订单");
                    MainWindow.WHMatchingDetection1 = true;
                    RefreshWHMatchingWorkArea(EndPoint);
                    RefreshAGVMatchingWorkArea(EndPoint);
                    InformWHThreadState = false;
                    return;
                }
            }
            if (StateMachine.A_Assembly_Area1 == 0 && StateMachine.A_Assembly_InArea1 == 0&&MainWindow.WHMatchingAssembly1==false&&StateMachine.A_System_State==2&&MainWindow.A1_Order_Enable==true)
            {
                bool A1_Outlib_Can = db.JudgeOutlib(con, "拧螺丝订单")&&db.JudgePreOutlibNum(con,"拧螺丝订单");  //判断拧螺丝订单是否还可以出库
                if (A1_Outlib_Can == true)
                {
                    db.InsertWHCmd(con, "拧螺丝订单", "出库", ConfigClass.Tray_B1);  //出库拧螺钉打钉托盘B1
                    db.UpdatePreOutlibNum(con, "拧螺丝订单");
                    MainWindow.WHMatchingAssembly1 = true;
                    RefreshWHMatchingWorkArea(EndPoint);
                    RefreshAGVMatchingWorkArea(EndPoint);
                    InformWHThreadState = false;
                    return;
                }
            }
            if (StateMachine.A_Assembly_Area2 == 0 && StateMachine.A_Assembly_InArea2 == 0&&MainWindow.WHMatchingAssembly2==false&&StateMachine.A_System_State==2&&MainWindow.A2_Order_Enable==true)
            {
                bool A2_Outlib_Can = db.JudgeOutlib(con, "轴承压装订单")&&db.JudgePreOutlibNum(con,"轴承压装订单");  //判断轴承压装订单是否还可以出库
                if (A2_Outlib_Can==true)
                {
                    db.InsertWHCmd(con, "轴承压装订单", "出库", ConfigClass.Tray_A4);  //出库轴承压装毛坯托盘A4
                    db.UpdatePreOutlibNum(con, "轴承压装订单");
                    MainWindow.WHMatchingAssembly2 = true;
                    RefreshWHMatchingWorkArea(EndPoint);
                    RefreshAGVMatchingWorkArea(EndPoint);
                    InformWHThreadState = false;
                    return;
                }
            }
            if (StateMachine.P_Process_Area2 == 0&&MainWindow.WHMatchingProcess2==false&&StateMachine.P_System_State==2&&MainWindow.P_Order_Enable==true)
            {
                bool P_Outlib_Can = db.JudgeOutlib(con, "加工订单") && db.JudgePreOutlibNum(con, "加工订单");  //判断加工订单是否还可以出库
                if (P_Outlib_Can == true)
                {
                    db.InsertWHCmd(con, "加工订单", "侧边出库", ConfigClass.Tray_A1);   //侧边出库加工毛坯托盘A1
                    db.InsertWHCmd(con, "加工订单", "出库", ConfigClass.Tray_A0);    //出库加工空托盘A0
                    db.UpdatePreOutlibNum(con, "加工订单");
                    MainWindow.WHMatchingProcess2 = true;
                    RefreshWHMatchingWorkArea(EndPoint);
                    RefreshAGVMatchingWorkArea(EndPoint);
                    InformWHThreadState = false;
                    return;
                }
            }
            if (StateMachine.D_Detection_Area2 == 0&&MainWindow.WHMatchingDetection2==false&&StateMachine.D_System_State==2&&MainWindow.D_Order_Enable==true)
            {
                bool D_Outlib_Can = db.JudgeOutlib(con, "检测订单") && db.JudgePreOutlibNum(con, "检测订单");  //判断检测订单是否还可以出库
                if (D_Outlib_Can == true)
                {
                    db.InsertWHCmd(con, "检测订单", "出库", ConfigClass.Tray_A2);  //出库加工成品托盘A2托盘
                    db.UpdatePreOutlibNum(con, "检测订单");
                    MainWindow.WHMatchingDetection2 = true;
                    RefreshWHMatchingWorkArea(EndPoint);
                    RefreshAGVMatchingWorkArea(EndPoint);
                    InformWHThreadState = false;
                    return;
                }
            }
            //上诉if条件都进不去的话，依然要刷新立库和AGV的匹配状态
            RefreshWHMatchingWorkArea(EndPoint);
            RefreshAGVMatchingWorkArea(EndPoint);
        }

        //AGV在立库入库区卸货完成，然后向立库表中插入入库指令信息
        private void InformWHInlib(object startpoint)
        {
            int StartPoint = 0;
            if (startpoint != null)
            {
                StartPoint = Convert.ToInt32(startpoint);
            }
            Thread.Sleep(TimeSpan.FromSeconds(4));   //等待几秒，防止重复接收消息导致重复向立库指令表中插入指令
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            if (StartPoint == ConfigClass.WHOutlibArea2)
            {
                db.InsertWHCmd(con, "入库订单", "入库", ConfigClass.Tray_A0);  //入库加工空托盘
                InformWHThreadState = false;
                return;
            }
            if (StartPoint == ConfigClass.ProcessArea1 || StartPoint == ConfigClass.ProcessArea2)
            {
                db.InsertWHCmd(con, "入库订单", "入库", ConfigClass.Tray_A2); //入库加工成品托盘A2
                InformWHThreadState = false;
                return;
            }
            if (StartPoint == ConfigClass.DetectionArea1 || StartPoint == ConfigClass.DetectionArea2)
            {
                db.InsertWHCmd(con, "入库订单", "入库", ConfigClass.Tray_A3);  //入库加工检测托盘A3
                InformWHThreadState = false;
                return;
            }
            if (StartPoint == ConfigClass.AssemblyArea1)
            {
                db.InsertWHCmd(con, "入库订单", "入库", ConfigClass.Tray_B2);   //入库螺钉完成托盘B2
                InformWHThreadState = false;
                return;
            }
            if (StartPoint == ConfigClass.AssemblyArea2)
            {
                db.InsertWHCmd(con, "入库订单", "入库", ConfigClass.Tray_A5);  //轴承压装完成托盘A5
                InformWHThreadState = false;
                return;
            }
        }

        private void RefreshWHMatchingWorkArea(int EndPoint)
        {
            switch (EndPoint)
            {
                case ConfigClass.ProcessArea1:
                    MainWindow.WHMatchingProcess1 = false;
                    break;
                case ConfigClass.ProcessArea2:
                    MainWindow.WHMatchingProcess2 = false;
                    break;
                case ConfigClass.DetectionArea1:
                    MainWindow.WHMatchingDetection1 = false;
                    break;
                case ConfigClass.DetectionArea2:
                    MainWindow.WHMatchingDetection2 = false;
                    break;
                case ConfigClass.AssemblyArea1:
                    MainWindow.WHMatchingAssembly1 = false;
                    break;
                case ConfigClass.AssemblyArea2:
                    MainWindow.WHMatchingAssembly2 = false;
                    break;
            }
        }


        private void RefreshAGVMatchingWorkArea(int EndPoint)
        {
            switch (EndPoint)
            {
                case ConfigClass.ProcessArea1:
                    MainWindow.AGVMatchingProcess1 = false;
                    break;
                case ConfigClass.ProcessArea2:
                    MainWindow.AGVMatchingProcess2 = false;
                    break;
                case ConfigClass.DetectionArea1:
                    MainWindow.AGVMatchingDetection1 = false;
                    break;
                case ConfigClass.DetectionArea2:
                    MainWindow.AGVMatchingDetection2 = false;
                    break;
                case ConfigClass.AssemblyArea1:
                    MainWindow.AGVMatchingAssembly1 = false;
                    break;
                case ConfigClass.AssemblyArea2:
                    MainWindow.AGVMatchingAssembly2 = false;
                    break;
            }
        }


        /// <summary>
        /// 向加工PLC  装配PLC  检测PLC发布当前状态
        /// </summary>
        /// <param name="StartPoint"></param>
        /// <param name="EndPoint"></param>
        private void PublishAGVState(int StartPoint, int EndPoint)
        {
            if (StartPoint == ConfigClass.WHOutlibArea1)   //AGV一个动作结束后，从出库区前往目标区完成
            {
                switch (EndPoint)
                {
                    case ConfigClass.ProcessArea1:
                        DTUSendPackage.AGV_Process1_PUT = 1;
                        break;
                    case ConfigClass.ProcessArea2:
                        DTUSendPackage.AGV_Process2_PUT = 1;
                        break;
                    case ConfigClass.AssemblyArea1:
                        DTUSendPackage.AGV_Assembly1_PUT = 1;
                        break;
                    case ConfigClass.AssemblyArea2:
                        DTUSendPackage.AGV_Assembly2_PUT = 1;
                        break;
                    case ConfigClass.DetectionArea1:
                        DTUSendPackage.AGV_Detection1_PUT = 1;
                        break;
                    case ConfigClass.DetectionArea2:
                        DTUSendPackage.AGV_Detection2_PUT = 1;
                        break;
                }
            }
            if (EndPoint == ConfigClass.WHInlibArea)   //终点是立库入库区，AGV从各个机器取到了托盘
            {
                switch (StartPoint)
                {
                    case ConfigClass.WHOutlibArea2:
                        DTUSendPackage.Inform_P_Raw_Tray = 1;  ////通知加工区机器人，毛坯托盘已经取走
                        DTUSendPackage.Raw_Tray_Out = 0;    //通知加工区机器人，加工区已没有毛坯托盘
                        break;
                    case ConfigClass.ProcessArea1:
                        DTUSendPackage.AGV_Process1_GET = 1;
                        break;
                    case ConfigClass.ProcessArea2:
                        DTUSendPackage.AGV_Process2_GET = 1;
                        break;
                    case ConfigClass.AssemblyArea1:
                        DTUSendPackage.AGV_Assembly1_GET = 1;
                        break;
                    case ConfigClass.AssemblyArea2:
                        DTUSendPackage.AGV_Assembly2_GET = 1;
                        break;
                    case ConfigClass.DetectionArea1:
                        DTUSendPackage.AGV_Detection1_GET = 1;
                        break;
                    case ConfigClass.DetectionArea2:
                        DTUSendPackage.AGV_Detection2_GET = 1;
                        break;
                }
            }
        }

        /// <summary>
        /// 重置AGV状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetNull()
        {
            DTUSendPackage.Inform_P_Raw_Tray = 0;
            DTUSendPackage.AGV_Manual_Up_GET = 0;
            DTUSendPackage.AGV_Manual_Up_PUT = 0;
            DTUSendPackage.AGV_Manual_Down_GET = 0;
            DTUSendPackage.AGV_Manual_Down_PUT = 0;
            DTUSendPackage.AGV_Process1_GET = 0;
            DTUSendPackage.AGV_Process1_PUT = 0;
            DTUSendPackage.AGV_Process2_GET = 0;
            DTUSendPackage.AGV_Process2_PUT = 0;
            DTUSendPackage.AGV_Detection1_GET = 0;
            DTUSendPackage.AGV_Detection1_PUT = 0;
            DTUSendPackage.AGV_Detection2_GET = 0;
            DTUSendPackage.AGV_Detection2_PUT = 0;
            DTUSendPackage.AGV_Assembly1_GET = 0;
            DTUSendPackage.AGV_Assembly1_PUT = 0;
            DTUSendPackage.AGV_Assembly2_GET = 0;
            DTUSendPackage.AGV_Assembly2_PUT = 0;      
        }
    }
}
