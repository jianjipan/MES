using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

namespace MMIS
{
    class WHMsgHandle
    {
        WHPackageInfo package = new WHPackageInfo();
        Thread InformAGVThread = null;   //通知AGV的线程
        Thread InformAGVThread1 = null;   //通知AGV去下料区的线程
        Thread UpdateOrderThread = null;   //更新订单表的线程
        public bool InformAGVThreadState = false;   //线程的活动状态，false表示未活动。
        public bool InformAGVThread1State = false;
        public bool UpdateOrderThreadState = false;
        public delegate void OrderCompleteDelegate();    
        public OrderCompleteDelegate OCDelegate;     //当前所有订单完成后触发的事件
        public delegate void UpDownCompleteDelegate();  //当前上下料订单完成后的触发事件
        public UpDownCompleteDelegate UDDelegate;
        public WHMsgHandle(WHPackageInfo package)
        {
            this.package = package;        
        }
        public void SelectMode(int WHOperState)
        {
            if (WHOperState == 3)
            {
                HandOperate();
            }
            if (WHOperState == 2)
            {
                ManualOperate();
            }
            if (WHOperState == 1)
            {
                AutoOperate();
            }
        }
        //手动操作AGV时收到消息的处理方法
        private void HandOperate()
        {           
            if (package.Head == 10004)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "已收到出库指令";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已收到出库指令", LogInfoHelp.LOG_TYPE.LOG_INFO);
                return;
            }
            if (package.Head == 20004)
            {
                if (package.InlibMatchingID == 1)
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "已收到入库指令";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("已收到入库指令", LogInfoHelp.LOG_TYPE.LOG_INFO);
                }
                else
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "入库托盘匹配不成功";
                    MessageBox.Show("入库托盘匹配不成功,请选择正确的托盘类型，或者检查托盘RFID码是否正确，否则RFID读写器可能出现异常");
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("入库托盘匹配不成功", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                }
                return;
            }
            if (package.Head == 30004)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "已收到侧边出库指令";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已收到侧边出库指令", LogInfoHelp.LOG_TYPE.LOG_INFO);
                return;
            }
            if (package.Head == 10005)
            {  
                if(package.OutlibMatchingID==2)
                {
                     MainWindow.whUIHandle.WH_OPER_STATE = "出库托盘匹配不成功";
                     MessageBox.Show("出库托盘匹配不成功，请检查MES系统的托盘位置和立库实际的托盘位置是否一致，否则RFID读写器可能出现异常");
                     LogInfoHelp.GetInstance().WriteInfoToLogFile("出库托盘匹配不成功", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                }
                MainWindow.whUIHandle.WH_OPER_STATE = "已出库";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已出库", LogInfoHelp.LOG_TYPE.LOG_INFO);
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.SaveKuweiArray(con, MainWindow.whHandInfo.Position, 0);   //更新库位表中的库位信息
                db.UpdateKuweiArray(con);
                MainWindow.WHOperState = 0;  //一个动作完成后，将手动动作置为默认
                return;                                
            }
            if (package.Head == 20005)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "已入库";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已入库", LogInfoHelp.LOG_TYPE.LOG_INFO);
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.SaveKuweiArray(con, MainWindow.whHandInfo.Position, MainWindow.whHandInfo.TrayStyle);   //更新库位表中的库位信息
                db.UpdateKuweiArray(con);
                MainWindow.WHOperState = 0; //一个动作完成后，将手动动作置为默认
                return;
            }
            if (package.Head == 30005)
            {
                if (package.OutlibMatchingID == 1)
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "已侧边出库";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("已侧边出库", LogInfoHelp.LOG_TYPE.LOG_INFO);
                    DataBaseHandle db = new DataBaseHandle();
                    SqlConnection con = new SqlConnection();
                    db.SaveKuweiArray(con, MainWindow.whHandInfo.Position, 0);   //更新库位表中的库位信息
                    db.UpdateKuweiArray(con);
                    MainWindow.WHOperState = 0; //一个动作完成后，将手动动作置为默认
                    return;
                }
                else if (package.OutlibMatchingID == 2)
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "侧边托盘匹配不成功";
                    MessageBox.Show("侧边出库托盘匹配不成功，请检查MES系统的托盘位置和立库实际的托盘位置是否一致，否则RFID读写器可能出现异常");
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("侧边托盘匹配不成功", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                }
            }
        }


        //执行订单（自动操作）AGV时收到消息的处理方法
        private void AutoOperate()
        {
            if (package.Head == 30000)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "写码完成";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("写码完成", LogInfoHelp.LOG_TYPE.LOG_INFO);
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.InsertAGVCmd(con, ConfigClass.WHOutlibArea2, ConfigClass.WHInlibArea);
                return;
            }
            if (package.Head == 10004)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "已收到出库指令";
                ExecuteOrder.SendWHCmdEnable = false;
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已收到出库指令", LogInfoHelp.LOG_TYPE.LOG_INFO);
                //更新立库表通信状态为【已收到】
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateWHComState(con, package.SerialNumber);              
                return;
            }

            if (package.Head == 20004)
            {
                if (package.InlibMatchingID == 1)  // 与预期的托盘号一致，匹配成功
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "已收到入库指令";
                    ExecuteOrder.SendWHCmdEnable = false;
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("已收到入库指令", LogInfoHelp.LOG_TYPE.LOG_INFO);
                    //更新立库表通信状态为【已收到】
                    DataBaseHandle db = new DataBaseHandle();
                    SqlConnection con = new SqlConnection();
                    db.UpdateWHComState(con, package.SerialNumber);
                }
                else
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "入库托盘匹配不成功";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("入库托盘匹配不成功", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                    MessageBox.Show("入库托盘匹配不成功,RFID读写器可能出现异常或者出现一些不可预见因素");
                    ExecuteOrder.ExecuteFlag = false;
                    DataBaseHandle db = new DataBaseHandle();
                    SqlConnection con = new SqlConnection();
                    db.DeleteAGVCmdTable(con);
                    db.DeleteWHCmdTable(con);
                }
                return;
            }
            if (package.Head == 30004)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "已收到侧边出库命令";
                ExecuteOrder.SendWHCmdEnable = false;
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已收到侧边出库命令", LogInfoHelp.LOG_TYPE.LOG_INFO);
                //更新立库表通信状态为【已收到】
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateWHComState(con, package.SerialNumber);                
                return;
            }
            if (package.Head == 10005)
            {
                if (package.OutlibMatchingID == 2)
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "出库托盘匹配不成功";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("出库托盘匹配不成功", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                    MessageBox.Show("出库托盘匹配不成功,请检查MES系统的托盘位置和立库实际的托盘位置是否一致，否则RFID读写器可能出现异常");
                    ExecuteOrder.ExecuteFlag = false;
                    DataBaseHandle db1 = new DataBaseHandle();
                    SqlConnection con1 = new SqlConnection();
                    db1.DeleteAGVCmdTable(con1);
                    db1.DeleteWHCmdTable(con1);
                    return;
                }
                MainWindow.whUIHandle.WH_OPER_STATE = "已出库";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已出库", LogInfoHelp.LOG_TYPE.LOG_INFO);
                //更新立库表工作状态为【完成】
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateWHWorkState(con, package.SerialNumber);
                ExecuteOrder.SendWHCmdEnable = true;
                //更新库位表（根据流水号得到出库的货位ID，然后将此ID号对应的货位类型置为0，选中状态置为未选中）
                db.UpdateCargoInfo(con, package.SerialNumber);

                //通知AGV来取货物，并送往指定位置
                //(根据当前出库的托盘类型和订单类型和各个工作区的PLC状态来判断应该将货物送往哪里，然后向AGV指令表中插入数据)

                //防止由于连续接收到相同信息，从而导致连续插入相同的指令以及防止线程重复创建
                if (InformAGVThreadState == false)
                {
                    InformAGVThread = new Thread(new ParameterizedThreadStart(CheckAndInformAGV));
                    InformAGVThread.IsBackground = true;
                    InformAGVThread1State = true;
                    InformAGVThread.Start(package);
                    InformAGVThread.Priority = ThreadPriority.AboveNormal;
                }
                return;
            }
            if (package.Head == 20005)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "已入库";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已入库", LogInfoHelp.LOG_TYPE.LOG_INFO);
                //更新立库表工作状态为【完成】
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateWHWorkState(con, package.SerialNumber);
                ExecuteOrder.SendWHCmdEnable = true;
                //更新库位表（根据流水号得出入库的货位ID，然后根据得到的托盘类型，将货位类型置为相应的类型。选中状态置为未选中）
                db.UpdateCargoInfo(con, package.SerialNumber, package.TrayStyle);

                //如果订单类型是入库订单的话，我们就根据其托盘类型更新订单表 ,然后判断立库中订单表的数量，
                //如果为0停止响应的机器工作
                //不为0的话则通知出库。
                if (UpdateOrderThreadState == false)
                {
                    UpdateOrderThread = new Thread(new ParameterizedThreadStart(UpdateOrderTable));
                    UpdateOrderThread.IsBackground = true;
                    UpdateOrderThreadState = true;
                    UpdateOrderThread.Start(package.SerialNumber);
                    UpdateOrderThread.Priority = ThreadPriority.AboveNormal;
                }
            }

            if (package.Head == 30005)
            {
                if (package.OutlibMatchingID == 2)
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "托盘匹配不成功";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("侧边托盘匹配不成功", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                    MessageBox.Show("侧边出库托盘匹配不成功,请检查MES系统的托盘位置和立库实际的托盘位置是否一致，否则RFID读写器可能出现异常");
                    ExecuteOrder.ExecuteFlag = false;
                    DataBaseHandle db1 = new DataBaseHandle();
                    SqlConnection con1 = new SqlConnection();
                    db1.DeleteAGVCmdTable(con1);
                    db1.DeleteWHCmdTable(con1);
                    return;
                }
                MainWindow.whUIHandle.WH_OPER_STATE = "已侧边出库";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已侧边出库", LogInfoHelp.LOG_TYPE.LOG_INFO);
                //更新立库表工作状态为【完成】
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateWHWorkState(con, package.SerialNumber);
                ExecuteOrder.SendWHCmdEnable = true;
                //更新库位表（根据流水号得到出库的货位ID，然后将此ID号对应的货位类型置为0，选中状态置为未选中）
                db.UpdateCargoInfo(con, package.SerialNumber);

                //通知加工区机器人来取货物
                DTUSendPackage.Raw_Tray_Out = 1;

                return;
            }
        }

        private void ManualOperate()
        {
            if (package.Head == 10004)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "已收到出库指令";       
                ExecuteOrder.SendWHCmdEnable = false;
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已收到出库指令", LogInfoHelp.LOG_TYPE.LOG_INFO);
                //更新立库表通信状态为【已收到】
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateWHComState(con, package.SerialNumber);         
            }

            if (package.Head == 20004)
            {
                if (package.InlibMatchingID == 1)  // 与预期的托盘号一致，匹配成功
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "已收到入库指令";
                    ExecuteOrder.SendWHCmdEnable = false;
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("已收到入库指令", LogInfoHelp.LOG_TYPE.LOG_INFO);
                    //更新立库表通信状态为【已收到】
                    DataBaseHandle db = new DataBaseHandle();
                    SqlConnection con = new SqlConnection();
                    db.UpdateWHComState(con, package.SerialNumber);
                }
                else
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "入库托盘匹配不成功";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("入库托盘匹配不成功", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                    MessageBox.Show("入库托盘匹配不成功,请选择正确的托盘类型，或者检查托盘RFID码是否正确，否则RFID读写器可能出现异常");
                    ExecuteOrder.HandAndDownFlag = false;
                    DataBaseHandle db1 = new DataBaseHandle();
                    SqlConnection con1 = new SqlConnection();
                    db1.DeleteAGVCmdTable(con1);
                    db1.DeleteWHCmdTable(con1);
                }
                return;
            }
            if (package.Head == 10005)
            {
                if (package.OutlibMatchingID == 2)
                {
                    MainWindow.whUIHandle.WH_OPER_STATE = "出库托盘匹配不成功";
                    LogInfoHelp.GetInstance().WriteInfoToLogFile("出库托盘匹配不成功", LogInfoHelp.LOG_TYPE.LOG_ERROR);
                    MessageBox.Show("出库托盘匹配不成功,请检查MES系统的托盘位置和立库实际的托盘位置是否一致，否则RFID读写器可能出现异常");
                    ExecuteOrder.HandAndDownFlag = false;
                    DataBaseHandle db1 = new DataBaseHandle();
                    SqlConnection con1 = new SqlConnection();
                    db1.DeleteAGVCmdTable(con1);
                    db1.DeleteWHCmdTable(con1);
                    return;
                }
                MainWindow.whUIHandle.WH_OPER_STATE = "已出库";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已出库", LogInfoHelp.LOG_TYPE.LOG_INFO);
                //更新立库表工作状态为【完成】
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateWHWorkState(con, package.SerialNumber);
                ExecuteOrder.SendWHCmdEnable = true;
                //更新库位表（根据流水号得到出库的货位ID，然后将此ID号对应的货位类型置为0，选中状态置为未选中）
                db.UpdateCargoInfo(con, package.SerialNumber);

                //通知AGV来取货物，并送往立库下料区
                //(根据当前出库的托盘类型和订单类型和各个工作区的PLC状态来判断应该将货物送往哪里，然后向AGV指令表中插入数据)

                //防止由于连续接收到相同信息，从而导致连续插入相同的指令以及防止线程重复创建
                if (InformAGVThread1State == false)
                {
                    InformAGVThread1 = new Thread(new ParameterizedThreadStart(CheckAndInformAGV1));
                    InformAGVThread1.IsBackground = true;
                    InformAGVThread1State = true;
                    InformAGVThread1.Start(package);
                }
                return;

            }
            if (package.Head == 20005)
            {
                MainWindow.whUIHandle.WH_OPER_STATE = "已入库";
                LogInfoHelp.GetInstance().WriteInfoToLogFile("已入库", LogInfoHelp.LOG_TYPE.LOG_INFO);
                //更新立库表工作状态为【完成】
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.UpdateWHWorkState(con, package.SerialNumber);
                ExecuteOrder.SendWHCmdEnable = true;
                //更新库位表（根据流水号得出入库的货位ID，然后根据得到的托盘类型，将货位类型置为相应的类型。选中状态置为未选中）
                db.UpdateCargoInfo(con, package.SerialNumber, package.TrayStyle);

                //当前上料订单完成事件，恢复上下料的按钮功能。
                UDDelegate();
            }

        }

        private void CheckAndInformAGV1(object obj)
        {
            Thread.Sleep(2);  //防止由于连续接收到相同信息，从而导致连续插入相同的指令
            WHPackageInfo package = obj as WHPackageInfo;
            SqlConnection con = new SqlConnection();
            DataBaseHandle db = new DataBaseHandle();
            string OrderStyle = "";
            int WHTrayStyle = db.SelectOTbySerialNumber(con, package.SerialNumber, ref OrderStyle);
            if (OrderStyle == "人工订单")   //如果是人工订单，则需要将其运送到人口下料区
            {
                db.InsertAGVCmd(con, ConfigClass.WHOutlibArea1, ConfigClass.HandDownArea);
                InformAGVThread1State = false;
                return;
            }
        }
        private void UpdateOrderTable(object obj)
        {
            int SerialNumber = -1;
            if (obj != null)
            {
                SerialNumber = Convert.ToInt32(obj);
            }
            Thread.Sleep(TimeSpan.FromSeconds(4));
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            string OrderStyle = "";
            int WHTrayStyle = db.SelectOTbySerialNumber(con, SerialNumber, ref OrderStyle);
            if (OrderStyle == "入库订单")
            {
                //更新订单表
                if (WHTrayStyle == ConfigClass.Tray_A2)   //入库加工成品托盘A2
                {
                    db.UpdateOrderNum(con, "加工订单");
                    Thread.Sleep(1);
                    CheckWHNumber("加工订单");
                    UpdateOrderThreadState = false;
                    return;
                }
                if (WHTrayStyle == ConfigClass.Tray_A3)  //入库加工检测托盘A3
                {
                    db.UpdateOrderNum(con, "检测订单");
                    Thread.Sleep(1);
                    CheckWHNumber("检测订单");
                    UpdateOrderThreadState = false;
                    return;
                }
                if (WHTrayStyle == ConfigClass.Tray_B2)   //入库螺钉完成托盘
                {
                    db.UpdateOrderNum(con, "拧螺丝订单");
                    Thread.Sleep(1);
                    CheckWHNumber("拧螺丝订单");
                    UpdateOrderThreadState = false;
                    return;
                }
                if (WHTrayStyle == ConfigClass.Tray_A5)  //入库轴承压装完成托盘
                {
                    db.UpdateOrderNum(con, "轴承压装订单");
                    Thread.Sleep(1);
                    CheckWHNumber("轴承压装订单");
                    UpdateOrderThreadState = false;
                    return;
                }
            }           
        }

        private void CheckWHNumber(string OrderStyle)
        {
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            //检查入库数量
            bool P_InlibNum = db.JudgeCompleteNum(con, "加工订单");   //小于订单数量就返回true
            bool D_InlibNum = db.JudgeCompleteNum(con, "检测订单");
            bool A1_InlibNum = db.JudgeCompleteNum(con, "拧螺丝订单");
            bool A2_InlibNum = db.JudgeCompleteNum(con, "轴承压装订单");          
            if (P_InlibNum == false)
            {
                MainWindow.orderstate.P_Order = "";
                DTUSendPackage.P_Order_Enable = 0;
                DTUSendPackage.Hand_Order_Enabel = 1;
                MainWindow.P_Order_Enable = false;
            }
            if (D_InlibNum == false)
            {
                MainWindow.orderstate.D_Order = "";
                DTUSendPackage.D_Order_Enable = 0;
                MainWindow.D_Order_Enable = false;
            }
            if (A1_InlibNum == false)
            {
                MainWindow.orderstate.A1_Order = "";
                DTUSendPackage.A1_Order_Enable = 0;
                MainWindow.A1_Order_Enable = false;
            }
            if (A2_InlibNum == false)
            {
                MainWindow.orderstate.A2_Order = "";
                DTUSendPackage.A2_Order_Enable = 0;
                MainWindow.A2_Order_Enable = false;
            }
            if (P_InlibNum == false && D_InlibNum == false && A1_InlibNum == false && A2_InlibNum == false)
            {
                //所有订单都完成了
                //对下订单和执行订单两个按钮解禁。
                OCDelegate();   //触发事件
            }
            if (OrderStyle == "加工订单")
            {              
                //检查出库数量
                bool P_OutlibNum = db.JudgeOutlib(con, "加工订单");   //小于订单数量就返回true
                bool P_PreOutlibNum = db.JudgePreOutlibNum(con, "加工订单");  //小于订单数量就返回
                //检查是否可以继续出库
                bool P_Can_Outlib = P_InlibNum && P_OutlibNum&&P_PreOutlibNum;  
                if (P_Can_Outlib == true && StateMachine.P_Process_Area1 == 0&&MainWindow.WHMatchingProcess1==false&&StateMachine.P_System_State==2&&MainWindow.P_Order_Enable==true)
                {
                    db.InsertWHCmd(con, "加工订单", "侧边出库", ConfigClass.Tray_A1);   //侧边出库加工毛坯托盘A1
                    db.InsertWHCmd(con, "加工订单", "出库", ConfigClass.Tray_A0);    //出库加工空托盘A0
                    db.UpdatePreOutlibNum(con, "加工订单");
                    MainWindow.WHMatchingProcess1 = true;
                    return;
                }
                if (P_Can_Outlib == true && StateMachine.P_Process_Area2 == 0&&MainWindow.WHMatchingProcess2==false&&StateMachine.P_System_State==2&&MainWindow.P_Order_Enable==true)
                {
                    db.InsertWHCmd(con, "加工订单", "侧边出库", ConfigClass.Tray_A1);   //侧边出库加工毛坯托盘A1
                    db.InsertWHCmd(con, "加工订单", "出库", ConfigClass.Tray_A0);    //出库加工空托盘A0
                    db.UpdatePreOutlibNum(con, "加工订单");
                    MainWindow.WHMatchingProcess2 = true;
                    return;
                }
                return;
            }
            if (OrderStyle == "检测订单")
            {
                //检查出库数量
                bool D_OutlibNum = db.JudgeOutlib(con, "检测订单");
                bool D_PreOutlibNum = db.JudgeOutlib(con, "检测订单");
                //检查是否可以继续出库  
                bool D_Can_Outlib = D_InlibNum && D_OutlibNum&&D_PreOutlibNum;
                if (D_Can_Outlib == true && StateMachine.D_Detection_Area1 == 0&&MainWindow.WHMatchingDetection1==false&&StateMachine.D_System_State==2&&MainWindow.D_Order_Enable==true)
                {
                    db.InsertWHCmd(con, "检测订单", "出库", ConfigClass.Tray_A2);   //出库加工成品托盘A2托盘
                    db.UpdatePreOutlibNum(con, "检测订单");
                    MainWindow.WHMatchingDetection1 = true;
                    return;
                }
                if (D_Can_Outlib == true && StateMachine.D_Detection_Area2 == 0&&MainWindow.WHMatchingDetection2==false&&StateMachine.D_System_State==2&&MainWindow.D_Order_Enable==true)
                {
                    db.InsertWHCmd(con, "检测订单", "出库", ConfigClass.Tray_A2);  //出库加工成品托盘A2托盘
                    db.UpdatePreOutlibNum(con, "检测订单");
                    MainWindow.WHMatchingDetection2 = true;
                    return;
                }
                return;
            }
            if (OrderStyle == "拧螺丝订单")
            {
                //检查出库数量
                bool A1_OutlibNum = db.JudgeOutlib(con, "拧螺丝订单");
                bool A1_PreOutlibNum = db.JudgePreOutlibNum(con, "拧螺丝订单");
                //检查是否可以继续出库  
                bool A1_Can_Outlib = A1_InlibNum && A1_OutlibNum&&A1_PreOutlibNum;
                if (A1_Can_Outlib == true && StateMachine.A_Assembly_Area1 == 0 && StateMachine.A_Assembly_InArea1 == 0&&MainWindow.WHMatchingAssembly1==false&&StateMachine.A_System_State==2&&MainWindow.A1_Order_Enable==true)
                {
                    db.InsertWHCmd(con, "拧螺丝订单", "出库", ConfigClass.Tray_B1);  //出库拧螺钉打钉托盘B1
                    db.UpdatePreOutlibNum(con, "拧螺丝订单");
                    MainWindow.WHMatchingAssembly1 = true;
                    return;
                }
                return;
            }
            if (OrderStyle == "轴承压装订单")
            {
                //检查出库数量
                bool A2_OutlibNum = db.JudgeOutlib(con, "轴承压装订单");
                bool A2_PreOutlibNum=db.JudgePreOutlibNum(con,"轴承压装订单");
                //检查是否可以继续出库  
                bool A2_Can_Outlib = A2_InlibNum && A2_OutlibNum&&A2_PreOutlibNum;
                if (A2_Can_Outlib == true && StateMachine.A_Assembly_Area2 == 0 && StateMachine.A_Assembly_InArea2 == 0&&MainWindow.WHMatchingAssembly2==false&&StateMachine.A_System_State==2&&MainWindow.A2_Order_Enable==true)
                {
                    db.InsertWHCmd(con, "轴承压装订单", "出库", ConfigClass.Tray_A4);  //出库轴承压装毛坯托盘A4
                    db.UpdatePreOutlibNum(con, "轴承压装订单");
                    MainWindow.WHMatchingAssembly2 = true;
                    return;
                }
            }          
        }

        //通知AGV的线程执行的函数
        //通知AGV来取货物，并送往指定位置
        //(根据当前出库的托盘类型和订单类型和各个工作区的PLC状态来判断应该将货物送往哪里，然后向AGV指令表中插入数据)
        private void CheckAndInformAGV(object obj)
        {
            Thread.Sleep(TimeSpan.FromSeconds(4));  //防止由于连续接收到相同信息，从而导致连续插入相同的指令
            WHPackageInfo package = obj as WHPackageInfo;
            SqlConnection con = new SqlConnection();
            DataBaseHandle db = new DataBaseHandle();
            string OrderStyle = "";
            int WHTrayStyle = db.SelectOTbySerialNumber(con, package.SerialNumber, ref OrderStyle);    
            //更新订单表的出库数量
            db.UpdateOutlibNum(con, OrderStyle);
            if (OrderStyle == "加工订单" && WHTrayStyle == ConfigClass.Tray_A0)  //如果是加工订单，且是加工空托盘A0的话
            {
                //AGV应该有从立库出口处前往加工区
                if (StateMachine.P_Process_Area1 == 0&&MainWindow.WHMatchingProcess1==true&&MainWindow.AGVMatchingProcess1==false)
                {
                    db.InsertAGVCmd(con, ConfigClass.WHOutlibArea1, ConfigClass.ProcessArea1);
                    MainWindow.AGVMatchingProcess1 = true;
                    InformAGVThread1State = false;
                    return;
                }
                if (StateMachine.P_Process_Area2 == 0&&MainWindow.WHMatchingProcess2==true&&MainWindow.AGVMatchingProcess2==false)
                {
                    db.InsertAGVCmd(con, ConfigClass.WHOutlibArea1, ConfigClass.ProcessArea2);
                    MainWindow.AGVMatchingProcess2 = true;
                    InformAGVThread1State = false;
                    return;
                }
                return;
            }
            if (OrderStyle == "检测订单" && WHTrayStyle == ConfigClass.Tray_A2) //如果是检测订单，且是加工成品托盘A2
            {
                //AGV应该从立库出口处前往检测区
                if (StateMachine.D_Detection_Area1 == 0&&MainWindow.WHMatchingDetection1==true&&MainWindow.AGVMatchingDetection1==false)
                {
                    db.InsertAGVCmd(con, ConfigClass.WHOutlibArea1, ConfigClass.DetectionArea1);
                    MainWindow.AGVMatchingDetection1 = true;
                    InformAGVThread1State = false;
                    return;
                }
                if (StateMachine.D_Detection_Area2 == 0&&MainWindow.WHMatchingDetection2==true&&MainWindow.AGVMatchingDetection2==false)
                {
                    db.InsertAGVCmd(con, ConfigClass.WHOutlibArea1, ConfigClass.DetectionArea2);
                    MainWindow.AGVMatchingDetection2 = true;
                    InformAGVThread1State = false;
                    return;
                }
                return;
            }
            if (OrderStyle == "拧螺丝订单" && WHTrayStyle == ConfigClass.Tray_B1)  //如果是拧螺丝订单，且是拧螺丝打钉托盘B1
            {
                if (StateMachine.A_Assembly_Area1 == 0 && MainWindow.WHMatchingAssembly1 == true&&MainWindow.AGVMatchingAssembly1==false)
                {
                    //AGV应该从立库出口处前往拧螺丝区（装配区1）
                    db.InsertAGVCmd(con, ConfigClass.WHOutlibArea1, ConfigClass.AssemblyArea1);
                    MainWindow.AGVMatchingAssembly1 = true;
                    InformAGVThread1State = false;
                    return;
                }
            }
            if (OrderStyle == "轴承压装订单" && WHTrayStyle == ConfigClass.Tray_A4)  //如果是轴承压装订单，且是轴承压装毛坯托盘A4
            {
                if (StateMachine.A_Assembly_Area2 == 0 && MainWindow.WHMatchingAssembly2 == true&&MainWindow.AGVMatchingAssembly2==false)
                {
                    //AGV应该从立库出口处前往轴承压装区（装配区2）
                    db.InsertAGVCmd(con, ConfigClass.WHOutlibArea1, ConfigClass.AssemblyArea2);
                    MainWindow.AGVMatchingAssembly2 = true;
                    InformAGVThread1State = false;
                    return;
                }
            }      
        }

    }
}
