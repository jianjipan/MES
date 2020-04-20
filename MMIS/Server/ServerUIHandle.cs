using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MMIS
{
    public class ServerUIHandle:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }

        /******************************加工区UI字符串*************/
        #region
        //加工区通信状态
        private string p_com_state;
        public string P_COM_STATE
        {
            get { return p_com_state; }
            set
            {
                if (p_com_state != value)
                {
                    p_com_state = value;
                }
                OnPropertyChanged("P_COM_STATE");
            }
        }
        //加工区系统状态
        private string p_sys_state;
        public string P_SYS_STATE
        {
            get { return p_sys_state; }
            set
            {
                if (p_sys_state != value)
                {
                    p_sys_state = value;
                }
                OnPropertyChanged("P_SYS_STATE");
            }
        }
        //人工上料区  有无托盘
        private string p_manualup_area;
        public string P_MANUALUP_AREA
        {
            get { return p_manualup_area; }
            set 
            {
                if (p_manualup_area != value)
                {
                    p_manualup_area = value;
                }
                OnPropertyChanged("P_MANUALUP_AREA");
            }
        }

        //人工下料区
        private string p_manualdown_area;
        public string P_MANUALDOWN_AREA
        {
            get { return p_manualdown_area; }
            set
            {
                if (p_manualdown_area != value)
                {
                    p_manualdown_area = value;
                }
                OnPropertyChanged("P_MANUALDOWN_AREA");
            }
        }

        //加工区1
        private string p_area1;
        public string P_AREA1
        {
            get { return p_area1; }
            set
            {
                if (p_area1 != value)
                {
                    p_area1 = value;
                }
                OnPropertyChanged("P_AREA1");
            }
        }
        //加工区2
        private string p_area2;
        public string P_AREA2
        {
            get { return p_area2; }
            set
            {
                if (p_area2 != value)
                {
                    p_area2 = value;
                }
                OnPropertyChanged("P_AREA2");
            }
        }

        //Mazak机床1 工作状态
        private string p_mazak1_robot;
        public string P_MAZAK1_ROBOT
        {
            get { return p_mazak1_robot; }
            set {
                if (p_mazak1_robot != value)
                {
                    p_mazak1_robot = value;
                }
                OnPropertyChanged("P_MAZAK1_ROBOT");
            }
        }
        //Mazak机床2 工作状态
        private string p_mazak2_robot;
        public string P_MAZAK2_ROBOT
        {
            get { return p_mazak2_robot; }
            set
            {
                if (p_mazak2_robot != value)
                {
                    p_mazak2_robot = value;
                }
                OnPropertyChanged("P_MAZAK2_ROBOT");
            }
        }

        //大机床 工作状态
        private string p_big_robot;
        public string P_BIG_ROBOT
        {
            get { return p_big_robot; }
            set
            {
                if (p_big_robot != value)
                {
                    p_big_robot = value;
                }
                OnPropertyChanged("P_BIG_ROBOT");
            }
        }

        //加工区机器人
        private string p_robot;
        public string P_ROBOT
        {
            get { return p_robot; }
            set
            {
                if (p_robot != value)
                {
                    p_robot = value;
                }
                OnPropertyChanged("P_ROBOT");
            }
        }

        //其他信息
        private string other_info;
        public string OTHER_INFO
        {
            get { return other_info; }
            set
            {
                if (other_info != value)
                {
                    other_info = value;
                }
                OnPropertyChanged("OTHER_INFO");
            }
        }
        #endregion

        /***************************检测区UI字符串***********************/
        #region
        //检测区通信状态
        private string d_com_state;
        public string D_COM_STATE
        {
            get { return d_com_state; }
            set
            {
                if (d_com_state != value)
                {
                    d_com_state = value;
                }
                OnPropertyChanged("D_COM_STATE");
            }
        }
        //检测区系统状态
        private string d_sys_state;
        public string D_SYS_STATE
        {
            get { return d_sys_state; }
            set
            {
                if (d_sys_state != value)
                {
                    d_sys_state = value;
                }
                OnPropertyChanged("D_SYS_STATE");
            }
        }

        //检测区1状态
        private string d_area1;
        public string D_AREA1
        {
            get { return d_area1; }
            set
            {
                if (d_area1 != value)
                {
                    d_area1 = value;
                }
                OnPropertyChanged("D_AREA1");
            }
        }

        //检测区2状态
        private string d_area2;
        public string D_AREA2
        {
            get { return d_area2; }
            set
            {
                if (d_area2 != value)
                {
                    d_area2 = value;
                }
                OnPropertyChanged("D_AREA2");
            }
        }

        //打标机器人
        private string d_marking_robot;
        public string D_MARKING_ROBOT
        {
            get { return d_marking_robot; }
            set
            {
                if (d_marking_robot != value)
                {
                    d_marking_robot = value;
                }
                OnPropertyChanged("D_MARKING_ROBOT");
            }
        }

        //检测区机器人
        private string d_robot;
        public string D_ROBOT
        {
            get { return d_robot; }
            set
            {
                if (d_robot != value)
                {
                    d_robot = value;
                }
                OnPropertyChanged("D_ROBOT");
            }
        }
        #endregion

        /****************************装配区UI字符串***********************/

        //装配区通信状态
        private string a_com_state;
        public string A_COM_STATE
        {
            get { return a_com_state; }
            set
            {
                if (a_com_state != value)
                {
                    a_com_state = value;
                }
                OnPropertyChanged("A_COM_STATE");
            }
        }
        //装配区系统状态
        private string a_sys_state;
        public string A_SYS_STATE
        {
            get { return a_sys_state; }
            set
            {
                if (a_sys_state != value)
                {
                    a_sys_state = value;
                }
                OnPropertyChanged("A_SYS_STATE");
            }
        }

        //区域1
        private string a_area1;
        public string A_AREA1
        {
            get { return a_area1; }
            set
            {
                if (a_area1 != value)
                {
                    a_area1 = value;
                }
                OnPropertyChanged("A_AREA1");
            }
        }

        //区域2
        private string a_area2;
        public string A_AREA2
        {
            get { return a_area2; }
            set
            {
                if (a_area2 != value)
                {
                    a_area2 = value;
                }
                OnPropertyChanged("A_AREA2");
            }
        }

        //装配区机器人
        private string a_robot;
        public string A_ROBOT
        {
            get { return a_robot; }
            set
            {
                if (a_robot != value)
                {
                    a_robot = value;
                }
                OnPropertyChanged("A_ROBOT");
            }
        }

        //直角坐标机器人
        private string a_cor_robot;
        public string A_COR_ROBOT
        {
            get { return a_cor_robot; }
            set
            {
                if (a_cor_robot != value)
                {
                    a_cor_robot = value;
                }
                OnPropertyChanged("A_COR_ROBOT");
            }
        }
    }
}
