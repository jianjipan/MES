using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MMIS
{
    public class OrderState:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }

        private string P_Order_State;   //加工订单状态
        private string D_Order_State;   //检测订单状态
        private string A1_Order_State;   //拧螺丝订单状态
        private string A2_Order_State;   //装配订单状态
        public string P_Order
        {
            get { return P_Order_State; }
            set
            {
                if (P_Order_State != value)
                {
                    P_Order_State = value;
                }
                OnPropertyChanged("P_Order");
            }
        }
        public string D_Order
        {
            get { return D_Order_State; }
            set
            {
                if (D_Order_State != value)
                {
                    D_Order_State = value;
                }
                OnPropertyChanged("D_Order");
            }
        }
        public string A1_Order
        {
            get { return A1_Order_State; }
            set
            {
                if (A1_Order_State != value)
                {
                    A1_Order_State = value;
                }
                OnPropertyChanged("A1_Order");
            }
        }
        public string A2_Order
        {
            get { return A2_Order_State; }
            set
            {
                if (A2_Order_State != value)
                {
                    A2_Order_State = value;
                }
                OnPropertyChanged("A2_Order");
            }
        }
    }
}
