using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MMIS
{
    public class KuweiNumber:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }

        private string quanxianMode = "";
        public string QUANXIANMODE
        {
            get { return quanxianMode; }
            set
            {
                if (quanxianMode != value)
                {
                    quanxianMode = value;
                }
                OnPropertyChanged("QUANXIANMODE");
            }
        }

        private int tray_Empty;   //空托盘
        private int tray_A0;    
        private int tray_A1;   
        private int tray_A2;
        private int tray_A3;
        private int tray_A4;
        private int tray_A5;
        private int tray_A6;
        private int tray_B0;
        private int tray_B1;
        private int tray_B2;
        private int tray_C;
        private int tray_D;

        public int Tray_Empty
        {
            get { return tray_Empty; }
            set
            {
                if (tray_Empty != value)
                {
                    tray_Empty = value;
                }
                OnPropertyChanged("Tray_Empty");
            }
        }
        public int Tray_A0
        {
            get { return tray_A0; }
            set
            {
                if (tray_A0 != value)
                {
                    tray_A0 = value;
                }
                OnPropertyChanged("Tray_A0");
            }
        }
        public int Tray_A1
        {
            get { return tray_A1; }
            set
            {
                if (tray_A1 != value)
                {
                    tray_A1 = value;
                }
                OnPropertyChanged("Tray_A1");
            }
        }
        public int Tray_A2
        {
            get { return tray_A2; }
            set
            {
                if (tray_A2 != value)
                {
                    tray_A2 = value;
                }
                OnPropertyChanged("Tray_A2");
            }
        }
        public int Tray_A3
        {
            get { return tray_A3; }
            set
            {
                if (tray_A3 != value)
                {
                    tray_A3 = value;
                }
                OnPropertyChanged("Tray_A3");
            }
        }
        public int Tray_A4
        {
            get { return tray_A4; }
            set
            {
                if (tray_A4 != value)
                {
                    tray_A4 = value;
                }
                OnPropertyChanged("Tray_A4");
            }
        }
        public int Tray_A5
        {
            get { return tray_A5; }
            set
            {
                if (tray_A5 != value)
                {
                    tray_A5 = value;
                }
                OnPropertyChanged("Tray_A5");
            }
        }
        public int Tray_A6
        {
            get { return tray_A6; }
            set
            {
                if (tray_A6 != value)
                {
                    tray_A6 = value;
                }
                OnPropertyChanged("Tray_A6");
            }
        }
        public int Tray_B0
        {
            get { return tray_B0; }
            set
            {
                if (tray_B0 != value)
                {
                    tray_B0 = value;
                }
                OnPropertyChanged("Tray_B0");
            }
        }
        public int Tray_B1
        {
            get { return tray_B1; }
            set
            {
                if (tray_B1 != value)
                {
                    tray_B1 = value;
                }
                OnPropertyChanged("Tray_B1");
            }
        }
        public int Tray_B2
        {
            get { return tray_B2; }
            set
            {
                if (tray_B2 != value)
                {
                    tray_B2 = value;
                }
                OnPropertyChanged("Tray_B2");
            }
        }
        public int Tray_C
        {
            get { return tray_C; }
            set
            {
                if (tray_C != value)
                {
                    tray_C = value;
                }
                OnPropertyChanged("Tray_C");
            }
        }
        public int Tray_D
        {
            get { return tray_D; }
            set
            {
                if (tray_D != value)
                {
                    tray_D = value;
                }
                OnPropertyChanged("Tray_D");
            }
        }


    }
}
