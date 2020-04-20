using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Windows.Forms;

namespace MMIS
{
    class ReadWriteXml        //读取配置文件
    {
        public static void GetConfigInfo()
        {
            string filePath = @"..\..\Config\Config.xml";
            try
            {
                if (File.Exists(filePath))   //判断文件是否存在
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);

                    XmlNode xm0 = xmlDoc.GetElementsByTagName("SERVER")[0];
                    ConfigClass.SERVER_IPADD = xm0.FirstChild.InnerText;
                    ConfigClass.SERVER_PORT = Convert.ToInt32(xm0.LastChild.InnerText);

                    XmlNode xm1 = xmlDoc.GetElementsByTagName("AGV")[0];
                    ConfigClass.AGV_IPADD = xm1.FirstChild.InnerText;
                    ConfigClass.AGV_PORT =Convert.ToInt32(xm1.LastChild.InnerText);

                    XmlNode xm2 = xmlDoc.GetElementsByTagName("WareHouse")[0];
                    ConfigClass.WH_IPADD = xm2.FirstChild.InnerText;
                    ConfigClass.WH_PORT = Convert.ToInt32(xm2.LastChild.InnerText);

                    XmlNode xm3 = xmlDoc.GetElementsByTagName("ProcessMachine")[0];
                    ConfigClass.PRO_M_IPADD = xm3.FirstChild.InnerText;

                    XmlNode xm4 = xmlDoc.GetElementsByTagName("DetectionMachine")[0];
                    ConfigClass.DETECTION_IPADD = xm4.FirstChild.InnerText;

                    XmlNode xm5 = xmlDoc.GetElementsByTagName("AssemblyMachine")[0];
                    ConfigClass.ASSEMBLY_IPADD = xm5.FirstChild.InnerText;

                    XmlNode xm6 = xmlDoc.GetElementsByTagName("WHManngerSYS")[0];
                    ConfigClass.WHMANNGERSYS_PASSWORD = xm6.FirstChild.InnerText;
                }
            }
            catch(FileNotFoundException ex)
            {
                MessageBox.Show("配置文件不存在或者出错" + ex.ToString());
            }
        }

        /// <summary>
        /// 文件关闭后将当前的TaskID号写入配置文件中
        /// </summary>
        public static void SetConfigTaskID()      
        {
            string filepath = @"..\..\Config.xml";
            try
            {
                if (File.Exists(filepath))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filepath);

                    //不需要了
                    //XmlNode xn = xmlDoc.GetElementsByTagName("TaskID")[0];
                    //xn.FirstChild.InnerText = ConfigClass.TaskID.ToString();

                    xmlDoc.Save(@"..\..\Config.xml");
                }
            }
            catch (FileNotFoundException  ex)
            {
                MessageBox.Show("配置文件不存在或者出错" + ex.ToString());
            }
        }

        //读取任务号，并加1返回，并保存加1后的值
        public static int ReadAndUpdateTaskID()
        {
            string filePath = @"..\..\Config\Config.xml";
            int TaskID = -1;
            try
            {
                if (File.Exists(filePath))   //判断文件是否存在
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(filePath);

                    XmlNode xm1 = xmlDoc.GetElementsByTagName("TaskID")[0];
                    TaskID = Convert.ToInt32(xm1.FirstChild.InnerText);
                    TaskID = TaskID + 1;
                    xm1.FirstChild.InnerText = TaskID.ToString();
                    xmlDoc.Save(@"..\..\Config.xml");
                }
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show("配置文件不存在或者出错" + e.ToString());
            }
            return TaskID;         
        }
    }
}
