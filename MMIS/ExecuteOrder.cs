using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Threading;

namespace MMIS
{
    class ExecuteOrder
    {
        public static bool ExecuteFlag = false;   //执行订单的表示符，由执行订单的按钮来控制
        public static bool HandAndDownFlag = false;    //人工上下料订单的标识符，由人工上下料按钮来控制
        public static bool WHCmdInlibNum = true;   //立库的指令条数，默认为true
        public static bool RunExecuteAGVCmdThread = true;   //运行执行AGV指令表的线程
        public static bool SendWHCmdEnable = true;    //允许发送立库指令的允许位,立库接收到消息后置为false，完成后置为true
        public static bool SendAGVCmdEnable = true;   //允许发送AGV指令的允许位，收到消息后置为flase，将指令表中的通信状态改过来后置为true。
        public static void ExecuteAGVCmd(object obj)    //执行AGV指令表（不断查询数据库AGV指令表中有没有未发送给AGV的指令，然后将其发送出去）
        {
            DataBaseHandle db = obj as DataBaseHandle;
            SqlConnection con = new SqlConnection();
            while (RunExecuteAGVCmdThread)
            {
                if ((ExecuteFlag == true||HandAndDownFlag==true)&&SendAGVCmdEnable==true)  //开始执行订单
                {
                    db.SelectAGVCmd(con);   //查询AGV需要发送的指令
                }
                Thread.Sleep(TimeSpan.FromSeconds(3));       //3s查询一次
            }
            RunExecuteAGVCmdThread = true;
        }

        public static bool RunExecuteWHCmdThread = true;   //运行立库指令表的线程
        public static void ExecuteWHCmd(object obj)       // 执行立库指令表（不断查询数据库AGV指令表中有没有未发送给AGV的指令，然后将其发送出去）
        {
            DataBaseHandle db = obj as DataBaseHandle;
            SqlConnection con = new SqlConnection();
            while (RunExecuteWHCmdThread)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));    //3s查询一次
                if ((ExecuteFlag==true||HandAndDownFlag==true)&&SendWHCmdEnable==true)       //开始执行订单
                {
                    db.SelectWHCmd(con);   //查询立库需要发送的指令
                }       
            }
            RunExecuteWHCmdThread = true;
        }       
    }
}
