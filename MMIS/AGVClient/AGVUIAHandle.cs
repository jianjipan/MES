using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MMIS
{
    public class AGVUIAHandle:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }

        private string agv_button;   //连接AGV的按钮
        private string agv_com_state;  //AGV通信状态
        private string agv_pos_action;   //AGV位置和动作状态

        public string AGV_BUTTON
        {
            get { return agv_button; }
            set
            {
                if (agv_button != value)
                {
                    agv_button = value;
                }
                OnPropertyChanged("AGV_BUTTON");
            }
        }

        public string AGV_COM_STATE
        {
            get { return agv_com_state; }
            set
            {
                if (agv_com_state != value)
                {
                    agv_com_state = value;
                }
                OnPropertyChanged("AGV_COM_STATE");
            }
        }

        public string AGV_POS_ACTION
        {
            get { return agv_pos_action; }
            set
            {
                if (agv_pos_action != value)
                {
                    agv_pos_action = value;
                }
                OnPropertyChanged("AGV_POS_ACTION");
            }
        }
    }
}
