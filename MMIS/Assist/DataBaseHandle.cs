using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace MMIS
{
    class DataBaseHandle
    {
        /// <summary>
        /// 查询AGV中通信状态【未收到】的状态，返给指定参数，通知AGV发送函数将其发送出去
        /// </summary>
        public delegate void AGVDelegate(SendPackage package);
        public AGVDelegate SelectAGVCmdEvent;

        /// <summary>
        /// 查询立库中通信状态【未收到】的状态，返给指定参数，通知立库发送函数将其发送出去
        /// </summary>
        public delegate void WHDelegate(WHSendPackage package);
        public WHDelegate SelectWHCmdEvent;
   
        /// <summary>
        /// 打开数据库  
        /// </summary>
        /// <param name="con">数据库连接实例</param>
        public void openDatabase(SqlConnection con)
        {
            con.ConnectionString= "Data Source=.;Initial Catalog=Electronic;Integrated Security=true";
            try
            {
                con.Open();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("数据库打开失败" + ex.ToString());
            }
        }
     
        /*********************************************************数据库AGV表有关的操作******************************************************/

        #region  AGV表有关的操作


        /// <summary>
        /// 自动生成任务号  用于AGV指令表
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private int Auto_TaskID(SqlConnection con)
        {
            int TaskID;
            string sql_select = "SELECT Max(TaskID)FROM AGVCmdTable";  //获取AGV指令表中任务号的最大值
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            if (Convert.ToString(cmd_select.ExecuteScalar()) != "")
            {
                int preTaskID = Convert.ToInt32(cmd_select.ExecuteScalar());  //获取最后一条记录的编号
                TaskID = preTaskID + 1;
            }
            else
            {
                TaskID = 1000;
            }
            return TaskID;
        }

        /// <summary>
        /// 向AGV数据表中插入命令
        /// </summary>
        /// <param name="con"></param>
        /// <param name="StartMapPoint">AGV地图起点</param>
        /// <param name="EndMapPoint">AGV地图终点</param>
        public void InsertAGVCmd(SqlConnection con, ushort StartMapPoint, ushort EndMapPoint,int TrayStyle=-1)
        {
            try
            {
                openDatabase(con);
                string sql_insert = "INSERT INTO AGVCmdTable VALUES(@TaskID,@StartMapPoint,@EndMapPoint,@WorkState,@ComState,@TrayStyle,@Priority)";
                SqlCommand cmd_insert = new SqlCommand(sql_insert, con);
                SqlParameter para0 = new SqlParameter("@TaskID", Auto_TaskID(con));
                cmd_insert.Parameters.Add(para0);
                SqlParameter para1 = new SqlParameter("@StartMapPoint", StartMapPoint.ToString());
                cmd_insert.Parameters.Add(para1);
                SqlParameter para2 = new SqlParameter("@EndMapPoint", EndMapPoint.ToString());
                cmd_insert.Parameters.Add(para2);
                string workstate="未完成";
                SqlParameter para3 = new SqlParameter("@WorkState", workstate);
                cmd_insert.Parameters.Add(para3);
                string comstate = "未收到";
                SqlParameter para4 = new SqlParameter("@ComState", comstate);
                cmd_insert.Parameters.Add(para4);
                SqlParameter para5 = new SqlParameter("@TrayStyle", TrayStyle);
                cmd_insert.Parameters.Add(para5);
                int priority = 0;
                if (StartMapPoint == ConfigClass.WHOutlibArea1)
                {
                     priority = 1;   //托盘从出库口取货的优先级要高
                 
                }
                if (StartMapPoint == ConfigClass.WHOutlibArea2 && EndMapPoint == ConfigClass.WHInlibArea)
                {
                     priority = 1;   //托盘侧边出库口出库口取货的优先级要高
                }
                SqlParameter para6 = new SqlParameter("@Priority", priority);
                cmd_insert.Parameters.Add(para6);
                cmd_insert.ExecuteNonQuery();
                con.Close();
            }
            catch(SqlException ex)
            {
                MessageBox.Show("插入AGV指令失败，请检查数据库是否断开" + ex.ToString());
            }
        }

        /// <summary>
        /// 选择AGV指令表中未通讯完成的指令，将其发给AGV
        /// </summary>
        /// <param name="con"></param>
        public void SelectAGVCmd(SqlConnection con)
        {
            try
            {
                openDatabase(con);
                string sql_select = "SELECT TaskID,StartMapPoint,EndMapPoint FROM AGVCmdTable WHERE WorkState='未完成' AND ComState='未收到' ORDER BY Priority DESC,TaskID ASC";
                SqlCommand cmd_select = new SqlCommand(sql_select, con);
                SqlDataReader reader = cmd_select.ExecuteReader();
                if (reader.Read())
                {
                    SendPackage package = new SendPackage();
                    package.TaskID= Convert.ToInt32(reader["TaskID"].ToString());
                    package.MapID1 = Convert.ToUInt16(reader["StartMapPoint"].ToString());
                    package.MapID2 = Convert.ToUInt16(reader["EndMapPoint"].ToString());
                    if (ExecuteOrder.SendAGVCmdEnable)
                    {
                        SelectAGVCmdEvent(package);  //触发事件
                    }
                }
                reader.Close();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("查询AGV通讯状态失败，请检查数据库是否断开" + ex.ToString());
            }
        }

        /// <summary>
        /// 当收到AGV的任务已收到消息后，更新AGV指令表中ComState的状态，为【已收到】
        /// </summary>
        /// <param name="con"></param>
        /// <param name="TaskID">任务号</param>
        public void UpdateAGVComState(SqlConnection con, int TaskID)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE AGVCmdTable SET ComState='已收到' WHERE TaskID='" + TaskID + "'";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("更新AGV通信状态失败，请检查数据库是否断开" + ex.ToString());
            }

        }

        /// <summary>
        /// 当AGV任务完成后，返回任务号，根据任务号将AGV指令表中的工作状态置为【完成】
        /// </summary>
        /// <param name="con"></param>
        /// <param name="TaskID">任务号</param>
        public void UpdateAGVWorkState(SqlConnection con, int TaskID)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE AGVCmdTable SET WorkState='完成' WHERE TaskID=" + TaskID + "";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("更新AGV工作状态失败，请检查数据库是否断开" + ex.ToString());
            }
        }

        /// <summary>
        /// 关闭AGV按钮后或关闭软件时，将AGV指令表中的通讯状态全部转换为【已收到】
        /// 防止下次开启AGV按钮或启动软件时，AGV突然执行上次的动作。
        /// </summary>
        /// <param name="con"></param>
        public void ResetAGVComState(SqlConnection con)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE AGVCmdTable SET ComState='已收到'";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch(SqlException ex)
            {
                MessageBox.Show("重置AGV通信状态失败，请检查数据库是否断开" + ex.ToString());
            }

        }

        /// <summary>
        /// 查询AGV的起始点 和终点位置
        /// </summary>
        /// <param name="con"></param>
        /// <param name="TaskID">任务号</param>
        public int SelectAGVStartEndPoint(SqlConnection con, int TaskID,ref int EndPoint)
        {
            int StartPoint=0;
            openDatabase(con);
            string sql_select = "SELECT StartMapPoint,EndMapPoint FROM AGVCmdTable WHERE TaskID=" + TaskID + "";
            SqlCommand cmd_select = new SqlCommand(sql_select,con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                StartPoint = Convert.ToInt32(reader["StartMapPoint"]);
                EndPoint = Convert.ToInt32(reader["EndMapPoint"]);
            }
            reader.Close();
            con.Close();
            return StartPoint;
        }

        public int SelectTrayStyleByTaskID(SqlConnection con, int TaskID)
        {
            int TrayStyle=-1;
            openDatabase(con);
            string sql_select = "SELECT TrayStyle FROM AGVCmdTable WHERE TaskID=" + TaskID + "";
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                TrayStyle = Convert.ToInt32(reader["TrayStyle"]);
            }
            reader.Close();
            con.Close();
            return TrayStyle;
        }

        //当重置按钮按下时，需要删除AGV指令表中的未收到和未完成指令
        public void DeleteAGVCmdTable(SqlConnection con)
        {
            try
            {
                openDatabase(con);
                string sql_delete = "DELETE FROM AGVCmdTable WHERE ComState='未收到' OR WorkState='未完成'";
                SqlCommand cmd_delete = new SqlCommand(sql_delete, con);
                cmd_delete.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("删除AGV指令未收到和未完成指令失败，请检查数据库是否断开连接" + ex.ToString());
            }
        }

        #endregion


        /**********************************************与立库相关的数据库操作**************************************************************/

        #region 立库数据表操作


        /// <summary>
        /// 自动生成流水号  用于立库指令表
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private int Auto_SerialNumber(SqlConnection con)
        {
            int SerialNumber;
            string sql_select = "SELECT Max(SerialNumber)FROM WHCmdTable";  //获取立库指令表中任务号的最大值
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            if (Convert.ToString(cmd_select.ExecuteScalar()) != "")
            {
                int preSerialNumber = Convert.ToInt32(cmd_select.ExecuteScalar());  //获取最后一条记录的编号
                SerialNumber = preSerialNumber + 1;
            }
            else
            {
                SerialNumber = 1000;
            }
            return SerialNumber;
        }


        /// <summary>
        /// 向数据库插入立库动作指令（动作包括 出库 入库 侧边出库）
        /// </summary>
        /// <param name="con"></param>
        /// <param name="Style"></param>
        /// <param name="Action"></param>
        public void InsertWHCmd(SqlConnection con, string Style,string WHAction,int TrayStyle)
        {
            try
            {
                openDatabase(con);
                int position = JudgePosition(con, TrayStyle, WHAction);
                if (position != 0)    //如果有对应要求的库位号
                {
                    string sql_insert = "INSERT INTO WHCmdTable VALUES(@SerialNumber,@OrderStyle,@WHAction,@WHTrayStyle,@WHPosition,@WorkState,@ComState)";
                    SqlCommand cmd_insert = new SqlCommand(sql_insert, con);
                    SqlParameter para0 = new SqlParameter("@SerialNumber", Auto_SerialNumber(con));
                    cmd_insert.Parameters.Add(para0);
                    SqlParameter para1 = new SqlParameter("@OrderStyle", Style);
                    cmd_insert.Parameters.Add(para1);
                    SqlParameter para2 = new SqlParameter("@WHAction", WHAction);
                    cmd_insert.Parameters.Add(para2);
                    SqlParameter para3 = new SqlParameter("@WHTrayStyle", TrayStyle);
                    cmd_insert.Parameters.Add(para3);
                    SqlParameter para4 = new SqlParameter("@WHPosition", position);
                    cmd_insert.Parameters.Add(para4);
                    string workstate = "未完成";
                    SqlParameter para5 = new SqlParameter("@WorkState", workstate);
                    cmd_insert.Parameters.Add(para5);
                    string comstate = "未收到";
                    SqlParameter para6 = new SqlParameter("@ComState", comstate);
                    cmd_insert.Parameters.Add(para6);
                    cmd_insert.ExecuteNonQuery();
                }
                else
                {
                    MessageBox.Show("库位中没有满足此条件的托盘，无法插入库位指令表");
                }
                con.Close();
            }
            catch(SqlException ex)
            {
                MessageBox.Show("插入立库指令失败，请检查数据库是否断开" + ex.ToString());
            }
        }

        /// <summary>
        /// 根据出库的类型和立库的动作来判断需要出或者入什么样的库位
        /// </summary>
        /// <param name="con"></param>
        /// <param name="Style">出库类型</param>
        /// <param name="WHAction">动作</param>
        /// <returns></returns>
        private int JudgePosition(SqlConnection con, int TrayStyle,string WHAction)
        {
            int position = 0;  //需要出库或入库的ID号
            int OperateStyle = -1;   //默认为-1
            if (WHAction == "出库" || WHAction == "侧边出库")
            {
                OperateStyle = TrayStyle;
            }
            if (WHAction == "入库")   //如果是入库订单，应该入没有任何托盘的库位Empty（对应数据表CargoStyle的0）
            {
                OperateStyle = ConfigClass.Tray_Empty;
            }
            string sql_select = "SELECT CargoID FROM CargoInfo WHERE CargoStyle="+OperateStyle+" AND SelectedState=0 ORDER BY CargoID";
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                position = Convert.ToInt32(reader["CargoID"]);                          
            }
            reader.Close();
            //选中好货位ID后，将其选中状态置为1【已选中】
            string sql_update = "UPDATE CargoInfo SET SelectedState=1 WHERE CargoID=" + position + "";
            SqlCommand cmd_update = new SqlCommand(sql_update, con);
            cmd_update.ExecuteNonQuery();   
            return position;
        }

        /// <summary>
        /// 选择立库指令表中通信状态【未收到】的指令，并通知立库函数发送指令
        /// </summary>
        /// <param name="con"></param>
        public void SelectWHCmd(SqlConnection con)
        {
            try
            {
                openDatabase(con);
                string sql_select = "SELECT SerialNumber,WHAction,WHTrayStyle,WHPosition FROM WHCmdTable WHERE WorkState='未完成'AND ComState='未收到' ORDER BY SerialNumber";
                SqlCommand cmd_select = new SqlCommand(sql_select, con);
                SqlDataReader reader = cmd_select.ExecuteReader();
                if (reader.Read())
                {
                    WHSendPackage package = new WHSendPackage();
                    string WHAction = reader["WHAction"].ToString().Trim();
                    if (WHAction == "出库")
                    {
                        package.Head = 10003;
                    }
                    if (WHAction == "侧边出库")
                    {
                        package.Head = 30003;
                    }
                    if (WHAction == "入库")
                    {
                        package.Head = 20003;
                    }
                    package.SerialNumber = Convert.ToInt32(reader["SerialNumber"].ToString());
                    package.TrayID = (ushort)Convert.ToInt32(reader["WHTrayStyle"].ToString());
                    package.WHPostion =(ushort)Convert.ToInt32(reader["WHPosition"].ToString());
                    if (ExecuteOrder.SendWHCmdEnable)
                    {
                        SelectWHCmdEvent(package);   //触发事件
                    }
                }
                reader.Close();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("查询立库表通讯状态失败，请检查数据库是否断开" + ex.ToString());
            }
        }

        /// <summary>
        /// 收到立库发来的消息确认后，更新立库指令表中的通讯状态为【已收到】
        /// </summary>
        /// <param name="con"></param>
        /// <param name="SerialNumber">流水号</param>
        public void UpdateWHComState(SqlConnection con, int SerialNumber)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE WHCmdTable SET ComState='已收到' WHERE SerialNumber='" + SerialNumber + "'";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("更新立库表通信状态失败，请检查数据库是否断开" + ex.ToString());
            }

        }

        /// <summary>
        /// 收到立库操作完成的消息后，更新立库指令表中的工作状态为【完成】
        /// </summary>
        /// <param name="con"></param>
        /// <param name="SerialNumber"></param>
        public void UpdateWHWorkState(SqlConnection con, int SerialNumber)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE WHCmdTable SET WorkState='完成' WHERE SerialNumber=" + SerialNumber + "";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("更新立库表工作状态失败，请检查数据库是否断开" + ex.ToString());
            }
        }


        /// <summary>
        /// 重置立库指令表  关闭立库按钮后或关闭软件时，将立库指令表中的通讯状态全部转换为【已收到】
        /// 防止下次开启立库按钮或启动软件时,立库突然执行上次的动作。
        /// </summary>
        /// <param name="con"></param>
        public void ResetWHComState(SqlConnection con)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE WHCmdTable SET ComState='已收到'";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("重置立库通信状态失败，请检查数据库是否断开" + ex.ToString());
            }

        }

        /// <summary>
        /// 立库出库完成后，向数据表中查询订单类型和托盘类型信息，以便做出判断通知AGV取往何处
        /// </summary>
        /// <param name="con"></param>
        /// <param name="SerialNumber"></param>
        public int SelectOTbySerialNumber(SqlConnection con, int SerialNumber,ref string OrderStyle)
        {
              int WHTrayStyle = -1;
              openDatabase(con);
              string sql_select = "SELECT OrderStyle,WHTrayStyle FROM WHCmdTable WHERE SerialNumber=" + SerialNumber + "";
              SqlCommand cmd_select = new SqlCommand(sql_select, con);
              SqlDataReader reader = cmd_select.ExecuteReader();
              if (reader.Read())
              {
                  OrderStyle = reader["OrderStyle"].ToString().Trim();
                  WHTrayStyle = Convert.ToInt32(reader["WHTrayStyle"]);
              }
              reader.Close();
              con.Close();
              return WHTrayStyle;
        }

        //当重置按钮按下时，需要删除立库指令表中所有未发送和未完成的指令
        public void DeleteWHCmdTable(SqlConnection con)
        {
            try
            {
                openDatabase(con);
                string sql_delete = "DELETE FROM WHCmdTable WHERE ComState='未收到' OR WorkState='未完成'";
                SqlCommand cmd_delete = new SqlCommand(sql_delete, con);
                cmd_delete.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("订单表指令删除失败，请检查数据库是否断开连接" + ex.ToString());
            }
        }

        //查询立库指令表中的入库指令是否小于等于1条，如果大于1条，则将AGV挂起
        public bool SelectWHCmdNum(SqlConnection con)
        {
            bool WHCMDNUMFlag = false;
            openDatabase(con);
            string sql_select = "SELECT COUNT(*) FROM WHCmdTable WHERE ComState='未收到' AND WHAction='入库'";
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                int num = Convert.ToInt32(reader[0].ToString());
                reader.Close();
                if (num > 0)   //AGV挂起
                {
                    WHCMDNUMFlag = true;
                    con.Close();
                    return WHCMDNUMFlag;
                }        
            }
            con.Close();
            return WHCMDNUMFlag;

        }

        #endregion


        /**************************************************与数据库订单表有关的操作******************************************************/

        #region 订单表操作

        /// <summary>
        /// 自动生成订单号  用于订单数据表
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        private string Auto_OrderID(SqlConnection con)
        {
            string OrderID;
            string sql_select = "SELECT Max(OrderID)FROM OrderTable";   //获取表中订单号的最大值
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            if (Convert.ToString(cmd_select.ExecuteScalar()) != "")  //如果表中有记录
            {
                string strID = Convert.ToString(cmd_select.ExecuteScalar()); //获取最后一条记录的编号
                OrderID = strID.Substring(0, 1) + Convert.ToString(Convert.ToUInt64(strID.Substring(1)) + 1); //自增1
            }
            else
            {
                OrderID = "D1000";
            }
            return OrderID;
        }


        /// <summary>
        /// 向数据库OrderTable表中插入下订单数据
        /// </summary>
        /// <param name="con">数据库连接实例</param>
        /// <param name="orderstyle">订单类型</param>
        /// <param name="ordernum">订单数量</param>
        public void InserOrderData(SqlConnection con, string orderstyle, int ordernum)
        {
            try
            {
                openDatabase(con);
                string sql_insert = "INSERT INTO OrderTable VALUES(@OrderID,@OrderTime,@OrderStyle,@OrderNum,@OrderCompleteNum,@OutlibNum,@PreOutlibNum,@OrderState)";
                int number = 0;
                SqlCommand cmd_insert = new SqlCommand(sql_insert, con);
                SqlParameter para0 = new SqlParameter("@OrderID", Auto_OrderID(con));
                cmd_insert.Parameters.Add(para0);
                SqlParameter para1 = new SqlParameter("@OrderTime", DateTime.Now.ToString());
                cmd_insert.Parameters.Add(para1);
                SqlParameter para2 = new SqlParameter("@OrderStyle", orderstyle);
                cmd_insert.Parameters.Add(para2);
                SqlParameter para3 = new SqlParameter("@OrderNum", ordernum);
                cmd_insert.Parameters.Add(para3);
                SqlParameter para4 = new SqlParameter("@OrderCompleteNum", number);
                cmd_insert.Parameters.Add(para4);
                SqlParameter para5 = new SqlParameter("OutlibNum", number);
                cmd_insert.Parameters.Add(para5);
                SqlParameter para6 = new SqlParameter("PreOutlibNUm", number);
                cmd_insert.Parameters.Add(para6);
                string orderstate="未完成";
                SqlParameter para7 = new SqlParameter("@OrderState", orderstate);
                cmd_insert.Parameters.Add(para7);
                cmd_insert.ExecuteNonQuery();
                con.Close();
                MessageBox.Show("下单成功");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("插入订单数据失败，请检查数据库是否断开" + ex.ToString());
            }
        }

        /// <summary>
        /// 每次向立库插入一条出库指令后，PreOutlibNum就增加1
        /// </summary>
        /// <param name="con"></param>
        /// <param name="OrderStyle"></param>
        public void UpdatePreOutlibNum(SqlConnection con,string OrderStyle)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE OrderTable SET PreOutlibNum=PreOutlibNum+1 WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("更新订单预出库数据失败，请检查数据库是否断开" + ex.ToString());
            }
        }


        /// <summary>
        /// 判断一下预出库数量是否小于订单数量
        /// </summary>
        /// <param name="con"></param>
        /// <param name="OrderStyle"></param>
        /// <returns></returns>
        public bool JudgePreOutlibNum(SqlConnection con,string OrderStyle)
        {
            bool IsCanOutlib = false;
            openDatabase(con);
            string sql_select = "SELECT OrderNum,PreOutlibNum FROM OrderTable WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                int ordernum = Convert.ToInt32(reader["OrderNum"].ToString());
                int preoutlibnum = Convert.ToInt32(reader["PreOutlibNum"].ToString());
                reader.Close();
                if (ordernum > preoutlibnum)
                {
                    IsCanOutlib = true;
                    con.Close();
                    return IsCanOutlib;
                }
            }
            con.Close();
            return IsCanOutlib;
        }

        /// <summary>
        /// 查询数据库OrderTable表中指定订单的数量
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        public int SelectOrderNum(SqlConnection con,string OrderStyle)
        {
            int num = 0;
            try
            {
                openDatabase(con);
                string sql_select = "SELECT OrderNum FROM OrderTable WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
                SqlCommand cmd_select = new SqlCommand(sql_select, con);
                SqlDataReader reader = cmd_select.ExecuteReader();
                if (reader.Read())
                {
                    num = Convert.ToInt32(reader["OrderNum"].ToString());
                }
                reader.Close();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("数据库查询失败，请检查数据库是否断开" + ex.ToString());
            }
            return num;
        }

        /// <summary>
        /// 查询是否存在未完成的指定订单类型的订单
        /// </summary>
        /// <param name="con"></param>
        /// <param name="OrderStyle"></param>
        public int IsExistOrderStyle(SqlConnection con, string OrderStyle)
        {
            int num = 0;
            openDatabase(con);
            string sql_select = "SELECT COUNT(*) FROM OrderTable WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                num = Convert.ToInt32(reader[0].ToString());
            }
            reader.Close();
            con.Close();
            return num;
        }

        /// <summary>
        /// 立库每次入库完成后，更新订单表的完成数量+1，然后比较订单表中完成订单数量是否等于订单数量，若是，将订单状态置为已完成
        /// </summary>
        /// <param name="con"></param>
        /// <param name="OrderID"></param>
        public void UpdateOrderNum(SqlConnection con, string OrderStyle)
        {
            openDatabase(con);
            string sql_update = "UPDATE OrderTable SET OrderCompleteNum=OrderCompleteNum+1 WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
            SqlCommand cmd_update = new SqlCommand(sql_update, con);
            cmd_update.ExecuteNonQuery();
            string sql_select = "SELECT OrderNum,OrderCompleteNum FROM OrderTable WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                int ordernum = Convert.ToInt32(reader["OrderNum"].ToString());
                int completenum = Convert.ToInt32(reader["OrderCompleteNum"].ToString());
                reader.Close();
                if (ordernum<=completenum)
                {
                    string sql_update1 = "UPDATE OrderTable SET OrderState='完成' WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
                    SqlCommand cmd_update1 = new SqlCommand(sql_update1, con);
                    cmd_update1.ExecuteNonQuery();
                }
            }
            con.Close();     
        }


        /// <summary>
        /// 判断订单完成数量是否小于订单数量，
        /// </summary>
        /// <param name="con"></param>
        /// <param name="OrderStyle"></param>
        /// <returns></returns>
        public bool JudgeCompleteNum(SqlConnection con, string OrderStyle)
        {
            bool IsCanOutlib = false;
            openDatabase(con);
            string sql_select = "SELECT OrderNum,OrderCompleteNum FROM OrderTable WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                int ordernum = Convert.ToInt32(reader["OrderNum"].ToString());
                int completenum = Convert.ToInt32(reader["OrderCompleteNum"].ToString());
                reader.Close();
                if (ordernum > completenum)
                {
                    IsCanOutlib = true;
                    con.Close();
                    return IsCanOutlib;
                }
            }
            con.Close();
            return IsCanOutlib;
        }

        /// <summary>
        /// 立库每次出库完成后，将订单表的相应订单的出库数量+1，
        /// </summary>
        /// <param name="con"></param>
        /// <param name="OrderStyle"></param>
        /// <returns></returns>
        public void  UpdateOutlibNum(SqlConnection con, string OrderStyle)
        {       
            openDatabase(con);
            string sql_update = "UPDATE OrderTable SET OutlibNum=OutlibNum+1 WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
            SqlCommand cmd_update = new SqlCommand(sql_update, con);
            cmd_update.ExecuteNonQuery();        
            con.Close();
        }

        /// <summary>
        /// 判断出库数量是否小于订单数量，
        /// </summary>
        /// <param name="con"></param>
        /// <param name="OrderStyle"></param>
        /// <returns></returns>
        public bool JudgeOutlib(SqlConnection con, string OrderStyle)
        {
            bool IsCanOutlib = false;
            openDatabase(con);
            string sql_select = "SELECT OrderNum,OutlibNum FROM OrderTable WHERE OrderStyle='" + OrderStyle + "' AND OrderState='未完成'";
            SqlCommand cmd_select = new SqlCommand(sql_select, con);
            SqlDataReader reader = cmd_select.ExecuteReader();
            if (reader.Read())
            {
                int ordernum = Convert.ToInt32(reader["OrderNum"].ToString());
                int outlibnum = Convert.ToInt32(reader["OutlibNum"].ToString());
                reader.Close();
                if (ordernum > outlibnum)
                {
                    IsCanOutlib = true;
                    con.Close();
                    return IsCanOutlib;
                }
            }
            con.Close();
            return IsCanOutlib;
        }

        //当重置按钮按下时，需要更新所有未完成订单为终止执行。
        public void UpdateUnCompleteOrder(SqlConnection con)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE OrderTable SET OrderState='终止执行' WHERE OrderState='未完成'";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch(SqlException ex)
            {
                MessageBox.Show("更新未完成订单失败，请检查数据库是否断开" + ex.ToString());
            }
        }


        #endregion

        /****************************************与数据库货位表有关的操作*****************************************************************************/
        #region 货位表操作
        /// <summary>
        /// 更新库位数组，以便于将其显示在屏幕上用
        /// </summary>
        /// <param name="con">数据库连接实例</param>
        public void UpdateKuweiArray(SqlConnection con)
        {
            if (con.State != ConnectionState.Open)
            {
                openDatabase(con);
            }
            int[] position=new int[13];
            for (int i = 0; i < 13; i++)
            {
                string sql_select = "SELECT COUNT(*) AS Number FROM CargoInfo WHERE CargoStyle=" + i + "";
                SqlCommand cmd_select = new SqlCommand(sql_select, con);
                SqlDataReader Dr = cmd_select.ExecuteReader();
                if (Dr.Read())
                {
                    position[i] = Convert.ToInt32(Dr["Number"].ToString());
                }
                Dr.Close();
            }
            con.Close();
            MainWindow.kuweinumber.Tray_Empty = position[0];
            MainWindow.kuweinumber.Tray_A0 = position[1];
            MainWindow.kuweinumber.Tray_A1 = position[2];
            MainWindow.kuweinumber.Tray_A2 = position[3];
            MainWindow.kuweinumber.Tray_A3 = position[4];
            MainWindow.kuweinumber.Tray_A4 = position[5];
            MainWindow.kuweinumber.Tray_A5 = position[6];
            MainWindow.kuweinumber.Tray_A6 = position[7];
            MainWindow.kuweinumber.Tray_B0 = position[8];
            MainWindow.kuweinumber.Tray_B1 = position[9];
            MainWindow.kuweinumber.Tray_B2 = position[10];
            MainWindow.kuweinumber.Tray_C = position[11];
            MainWindow.kuweinumber.Tray_D = position[12];
        }

        /// <summary>
        /// 库位数组，以便于将其显示在在KuweiShow界面上
        /// </summary>
        /// <param name="con"></param>
        public DataTable GetKuweiArray(SqlConnection con)
        {
            openDatabase(con);
            string sql_select = "SELECT CargoStyle FROM CargoInfo";
            SqlDataAdapter da = new SqlDataAdapter(sql_select, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }

        /// <summary>
        /// KuweiShow保存修改按钮的数据库操作
        /// </summary>
        /// <param name="con"></param>
        /// <param name="position"></param>
        /// <param name="traystyle"></param>
        public void SaveKuweiArray(SqlConnection con, int position,int traystyle)
        {
            try
            {
                openDatabase(con);
                string sql_update = "UPDATE CargoInfo SET CargoStyle=" + traystyle + " WHERE CargoID=" + position + "";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("将库位信息保存到数据库失败，请检查数据库是否断开连接"+ex.ToString());
            }
        }

        /// <summary>
        /// 每次出库完成后，要更新库位表
        /// （根据流水号得到出库的货位ID，然后将此ID号对应的货位类型置为0(无托盘) 然后将选中状态置为0（未选中））
        /// </summary>
        /// <param name="con"></param>
        /// <param name="SerialNumber"></param>
        public void UpdateCargoInfo(SqlConnection con, int SerialNumber)
        {
            try
            {
                openDatabase(con);
                string sql_select = "SELECT WHPosition FROM WHCmdTable WHERE SerialNumber=" + SerialNumber + "";
                SqlCommand cmd_select = new SqlCommand(sql_select, con);
                SqlDataReader reader = cmd_select.ExecuteReader();
                if (reader.Read())
                {
                    
                    int position = Convert.ToInt32(reader["WHPosition"].ToString());  //得到货位ID
                    reader.Close();
                    //更新货位表
                    string sql_update = "UPDATE CargoInfo SET CargoStyle=0,SelectedState=0 WHERE CargoID=" + position + "";
                    SqlCommand cmd_update = new SqlCommand(sql_update, con);
                    cmd_update.ExecuteNonQuery();
                }
              
                con.Close();
                UpdateKuweiArray(con);    //更新库位表，以便于将其显示屏幕上
            }
            catch (SqlException ex)
            {
                MessageBox.Show("更新货位信息表失败，请检查数据库连接是否断开" + ex.ToString());
            }
        }

        /// <summary>
        /// 每次入库完成后，更新库位表（根据流水号得出入库的货位ID，
        /// 然后根据得到的托盘类型，将货位类型置为相应的类型。选中状态置为0(未选中))
        /// </summary>
        /// <param name="con"></param>
        /// <param name="SerialNumber"></param>
        /// <param name="TrayStyle"></param>
        public void UpdateCargoInfo(SqlConnection con, int SerialNumber, ushort TrayStyle)
        {
            try
            {
                openDatabase(con);
                string sql_select = "SELECT WHPosition FROM WHCmdTable WHERE SerialNumber=" + SerialNumber + "";
                SqlCommand cmd_select = new SqlCommand(sql_select, con);
                SqlDataReader reader = cmd_select.ExecuteReader();
                if (reader.Read())
                {
                    int position = Convert.ToInt32(reader["WHPosition"].ToString());  //得到货位ID
                    reader.Close();
                    //更新货位表
                    string sql_update = "UPDATE CargoInfo SET CargoStyle=" + (int)TrayStyle + ",SelectedState=0 WHERE CargoID=" + position + "";
                    SqlCommand cmd_update = new SqlCommand(sql_update, con);
                    cmd_update.ExecuteNonQuery();
                }          
                con.Close();
                UpdateKuweiArray(con);    //更新库位表，以便于将其显示在屏幕上。
            }
            catch (SqlException ex)
            {
                MessageBox.Show("更新货位信息表失败，请检查数据库连接是否断开" + ex.ToString());
            }
        }

        //当重置按钮按下时，将货位表中的货位选中状态置为0 未选中
        public void UpdateSelectState(SqlConnection con)
        {
            try
            {
                openDatabase(con);
                int selectState=0;
                string sql_update = "UPDATE CargoInfo SET SelectedState=" + selectState + " WHERE SelectedState='1'";
                SqlCommand cmd_update = new SqlCommand(sql_update, con);
                cmd_update.ExecuteNonQuery();
                con.Close();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("更新货位表选中状态失败，请检查数据库是否断开" + ex.ToString());
            }
        }

        #endregion
        /******************************************************与检测信息表有关的操作**********************************************************/
        /// <summary>
        /// 收到各个区域的托盘等信息后，将RFID或者其他参数保存
        /// </summary>
        /// <param name="con"></param>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <param name="value3"></param>
        /// <param name="value4"></param>
        /// <param name="value5"></param>
        public void SaveData(SqlConnection con, string WorkArea, string TrayInArea1="缺省", string TrayInArea2="缺省", float value1 = 0, float value2 = 0, float value3 = 0, float value4 = 0, float value5 = 0)
        {
            openDatabase(con);
            string sql_insert = "INSERT INTO DetectionTable VALUES(@DetectionTime,@WorkArea,@TrayInArea1,@TrayInArea2,@para1,@para2,@para3,@para4,@para5)";
            SqlCommand cmd_insert = new SqlCommand(sql_insert, con);
            SqlParameter para0 = new SqlParameter("@DetectionTime", DateTime.Now.ToString());
            cmd_insert.Parameters.Add(para0);
            SqlParameter para1 = new SqlParameter("@WorkArea", WorkArea);
            cmd_insert.Parameters.Add(para1);
            SqlParameter para2 = new SqlParameter("@TrayInArea1", TrayInArea1);
            cmd_insert.Parameters.Add(para2);
            SqlParameter para3 = new SqlParameter("@TrayInArea2", TrayInArea2);
            cmd_insert.Parameters.Add(para3);
            SqlParameter para4 = new SqlParameter("para1", value1);
            cmd_insert.Parameters.Add(para4);
            SqlParameter para5 = new SqlParameter("para2", value2);
            cmd_insert.Parameters.Add(para5);
            SqlParameter para6 = new SqlParameter("para3", value3);
            cmd_insert.Parameters.Add(para6);
            SqlParameter para7 = new SqlParameter("para4", value4);
            cmd_insert.Parameters.Add(para7);
            SqlParameter para8 = new SqlParameter("para5", value5);
            cmd_insert.Parameters.Add(para8);
            cmd_insert.ExecuteNonQuery();
            con.Close();
        }

        public DataTable SearchOrderData(SqlConnection con, string startTime, string endTime, string bookingStyle)
        {
            openDatabase(con);
            string sql_select = "";
            if (bookingStyle != "全部订单")
            {
                string sql_select1 = "SELECT OrderID,OrderTime,OrderStyle,OrderNum,OrderCompleteNum,OrderState FROM OrderTable WHERE OrderTime>'" + startTime + "' and OrderTime<'" + endTime + "' and OrderStyle='" + bookingStyle + "'";
                sql_select = sql_select1;
            }
            else
            {
                string sql_select2 = "SELECT OrderID,OrderTime,OrderStyle,OrderNum,OrderCompleteNum,OrderState FROM OrderTable WHERE OrderTime>'" + startTime + "' and OrderTime<'" + endTime + "'";
                sql_select = sql_select2;
            }
            SqlDataAdapter da = new SqlDataAdapter(sql_select, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
        public DataTable SearchDetectionData(SqlConnection con, string startTime, string endTime)
        {
            openDatabase(con);
            string sql_select = "SELECT DetectionTime,WorkArea,TrayInArea1,TrayInArea2,para1,para2,para3,para4,para5 FROM DetectionTable WHERE DetectionTime>'"+startTime+"' and DetectionTime<'"+endTime+"'";
            SqlDataAdapter da = new SqlDataAdapter(sql_select, con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            return dt;
        }
    }
}
