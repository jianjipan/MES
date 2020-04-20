using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMIS
{
    public class LogInfoHelp
    {
        /// <summary>
        /// 日志类型
        /// </summary>
        public enum LOG_TYPE
        {
            LOG_FAIL = 0,    //致命错误
            LOG_ERROR,       //一般错误
            LOG_EXCEPTION,   //异常
            LOG_WARN,        //警告
            LOG_INFO,        //一般信息
        }

        private static object lock0 = new object();
        private StreamWriter LogFile = null;
        private static LogInfoHelp _instance = null;
        private string LogFilePath = null;

        public static LogInfoHelp GetInstance()
        {
            if (null == _instance)
            {
                _instance = new LogInfoHelp();
            }

            return _instance;
        }
        private LogInfoHelp() { }

        /// <summary>
        /// 创建日志文件
        /// </summary>
        public void CreateLogFile()
        {
            //获取运行程序的路径
            string logFilePath = AppDomain.CurrentDomain.BaseDirectory;
            string logFileName = (DateTime.Now.Year).ToString() + '-'
                + (DateTime.Now.Month).ToString() + '-' + (DateTime.Now.Day).ToString() + "_Log.log";
            logFilePath += "logFile\\";
            if (!System.IO.Directory.Exists(logFilePath))
            {
                System.IO.Directory.CreateDirectory(logFilePath);
            }
            this.LogFilePath = logFilePath + logFileName;
        }

        /// <summary>
        /// 信息写入日志
        /// </summary>
        /// <param name="strMsg"></param>
        public void WriteInfoToLogFile(string strLogInfo, LOG_TYPE logType)
        {
            lock (lock0)
            {
                LogFile = new StreamWriter(LogFilePath, true);//文件保存位置
                string strlogInfo = null;
                switch (logType)
                {
                    case LOG_TYPE.LOG_FAIL:
                        {
                            strlogInfo = String.Format("[{0}] 致命错误:{1}", DateTime.Now.ToString(), strLogInfo);
                        }
                        break;

                    case LOG_TYPE.LOG_ERROR:
                        {
                            strlogInfo = String.Format("[{0}] 一般错误:{1}", DateTime.Now.ToString(), strLogInfo);
                        }
                        break;

                    case LOG_TYPE.LOG_EXCEPTION:
                        {
                            strLogInfo = String.Format("[{0}] 异常:{1}", DateTime.Now.ToString(), strLogInfo);
                        }
                        break;

                    case LOG_TYPE.LOG_WARN:
                        {
                            strLogInfo = String.Format("[{0}] 警告:{1}", DateTime.Now.ToString(), strLogInfo);
                        }
                        break;

                    case LOG_TYPE.LOG_INFO:
                        {
                            strLogInfo = String.Format("[{0}] 一般信息:{1}", DateTime.Now.ToString(), strLogInfo);
                        }
                        break;
                }
                LogFile.WriteLine(strLogInfo);
                LogFile.Close();
            }
        }
    }
}
