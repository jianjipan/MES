using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MMIS
{
    public class WHUIHandle:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }

        private string wh_button;
        private string wh_com_state;
        private string wh_oper_state;

        public string WH_BUTTON
        {
            get { return wh_button; }
            set
            {
                if (wh_button != value)
                {
                    wh_button = value;
                }
                OnPropertyChanged("WH_BUTTON");
            }
        }

        public string WH_COM_STATE
        {
            get { return wh_com_state; }
            set
            {
                if (wh_com_state != value)
                {
                    wh_com_state = value;
                }
                OnPropertyChanged("WH_COM_STATE");
            }
        }

        public string WH_OPER_STATE
        {
            get { return wh_oper_state; }
            set
            {
                if (wh_oper_state != value)
                {
                    wh_oper_state = value;
                }
                OnPropertyChanged("WH_OPER_STATE");
            }
        }
    }
}
