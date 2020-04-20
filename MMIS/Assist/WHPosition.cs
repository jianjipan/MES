using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MMIS
{
    public class WHPosition:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string strPropertyInfo)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(strPropertyInfo));
            }
        }

      

        private int pos_1_style = 0;
        public int POS_1 
        { 

            get { return pos_1_style; }
            set 
            {
                if (pos_1_style != value)
                {
                    pos_1_style = value;
                }
                OnPropertyChanged("POS_1");
            }
        }

        private int pos_2_style = 0;
        public int POS_2
        {

            get { return pos_2_style; }
            set
            {
                if (pos_2_style != value)
                {
                    pos_2_style = value;
                }
                OnPropertyChanged("POS_2");
            }
        }
        private int pos_3_style = 0;
        public int POS_3
        {

            get { return pos_3_style; }
            set
            {
                if (pos_3_style != value)
                {
                    pos_3_style = value;
                }
                OnPropertyChanged("POS_3");
            }
        }

        private int pos_4_style = 0;
        public int POS_4
        {

            get { return pos_4_style; }
            set
            {
                if (pos_4_style != value)
                {
                    pos_4_style = value;
                }
                OnPropertyChanged("POS_4");
            }
        }

        private int pos_5_style = 0;
        public int POS_5
        {

            get { return pos_5_style; }
            set
            {
                if (pos_5_style != value)
                {
                    pos_5_style = value;
                }
                OnPropertyChanged("POS_5");
            }
        }

        private int pos_6_style = 0;
        public int POS_6
        {

            get { return pos_6_style; }
            set
            {
                if (pos_6_style != value)
                {
                    pos_6_style = value;
                }
                OnPropertyChanged("POS_6");
            }
        }
        private int pos_7_style = 0;
        public int POS_7
        {

            get { return pos_7_style; }
            set
            {
                if (pos_7_style != value)
                {
                    pos_7_style = value;
                }
                OnPropertyChanged("POS_7");
            }
        }
        private int pos_8_style = 0;
        public int POS_8
        {

            get { return pos_8_style; }
            set
            {
                if (pos_8_style != value)
                {
                    pos_8_style = value;
                }
                OnPropertyChanged("POS_8");
            }
        }
        private int pos_9_style = 0;
        public int POS_9
        {

            get { return pos_9_style; }
            set
            {
                if (pos_9_style != value)
                {
                    pos_9_style = value;
                }
                OnPropertyChanged("POS_9");
            }
        }
        private int pos_10_style = 0;
        public int POS_10
        {

            get { return pos_10_style; }
            set
            {
                if (pos_10_style != value)
                {
                    pos_10_style = value;
                }
                OnPropertyChanged("POS_10");
            }
        }
        private int pos_11_style = 0;
        public int POS_11
        {

            get { return pos_11_style; }
            set
            {
                if (pos_11_style != value)
                {
                    pos_11_style = value;
                }
                OnPropertyChanged("POS_11");
            }
        }
        private int pos_12_style = 0;
        public int POS_12
        {

            get { return pos_12_style; }
            set
            {
                if (pos_12_style != value)
                {
                    pos_12_style = value;
                }
                OnPropertyChanged("POS_12");
            }
        }
        private int pos_13_style = 0;
        public int POS_13
        {

            get { return pos_13_style; }
            set
            {
                if (pos_13_style != value)
                {
                    pos_13_style = value;
                }
                OnPropertyChanged("POS_13");
            }
        }
        private int pos_14_style = 0;
        public int POS_14
        {

            get { return pos_14_style; }
            set
            {
                if (pos_14_style != value)
                {
                    pos_14_style = value;
                }
                OnPropertyChanged("POS_14");
            }
        }
        private int pos_15_style = 0;
        public int POS_15
        {

            get { return pos_15_style; }
            set
            {
                if (pos_15_style != value)
                {
                    pos_15_style = value;
                }
                OnPropertyChanged("POS_15");
            }
        }
        private int pos_16_style = 0;
        public int POS_16
        {

            get { return pos_16_style; }
            set
            {
                if (pos_16_style != value)
                {
                    pos_16_style = value;
                }
                OnPropertyChanged("POS_16");
            }
        }
        private int pos_17_style = 0;
        public int POS_17
        {

            get { return pos_17_style; }
            set
            {
                if (pos_17_style != value)
                {
                    pos_17_style = value;
                }
                OnPropertyChanged("POS_17");
            }
        }
        private int pos_18_style = 0;
        public int POS_18
        {

            get { return pos_18_style; }
            set
            {
                if (pos_18_style != value)
                {
                    pos_18_style = value;
                }
                OnPropertyChanged("POS_18");
            }
        }
        private int pos_19_style = 0;
        public int POS_19
        {

            get { return pos_19_style; }
            set
            {
                if (pos_19_style != value)
                {
                    pos_19_style = value;
                }
                OnPropertyChanged("POS_19");
            }
        }
        private int pos_20_style = 0;
        public int POS_20
        {

            get { return pos_20_style; }
            set
            {
                if (pos_20_style != value)
                {
                    pos_20_style = value;
                }
                OnPropertyChanged("POS_20");
            }
        }
        private int pos_21_style = 0;
        public int POS_21
        {

            get { return pos_21_style; }
            set
            {
                if (pos_21_style != value)
                {
                    pos_21_style = value;
                }
                OnPropertyChanged("POS_21");
            }
        }

        private int pos_22_style = 0;
        public int POS_22
        {

            get { return pos_22_style; }
            set
            {
                if (pos_22_style != value)
                {
                    pos_22_style = value;
                }
                OnPropertyChanged("POS_22");
            }
        }

          private int pos_23_style = 0;
        public int POS_23
        {

            get { return pos_23_style; }
            set
            {
                if (pos_23_style != value)
                {
                    pos_23_style = value;
                }
                OnPropertyChanged("POS_23");
            }
        }
          private int pos_24_style = 0;
        public int POS_24
        {

            get { return pos_24_style; }
            set
            {
                if (pos_24_style != value)
                {
                    pos_24_style = value;
                }
                OnPropertyChanged("POS_24");
            }
        }
          private int pos_25_style = 0;
        public int POS_25
        {

            get { return pos_25_style; }
            set
            {
                if (pos_25_style != value)
                {
                    pos_25_style = value;
                }
                OnPropertyChanged("POS_25");
            }
        }
          private int pos_26_style = 0;
        public int POS_26
        {

            get { return pos_26_style; }
            set
            {
                if (pos_26_style != value)
                {
                    pos_26_style = value;
                }
                OnPropertyChanged("POS_26");
            }
        }
          private int pos_27_style = 0;
        public int POS_27
        {

            get { return pos_27_style; }
            set
            {
                if (pos_27_style != value)
                {
                    pos_27_style = value;
                }
                OnPropertyChanged("POS_27");
            }
        }
          private int pos_28_style = 0;
        public int POS_28
        {

            get { return pos_28_style; }
            set
            {
                if (pos_28_style != value)
                {
                    pos_28_style = value;
                }
                OnPropertyChanged("POS_28");
            }
        }
          private int pos_29_style = 0;
        public int POS_29
        {

            get { return pos_29_style; }
            set
            {
                if (pos_29_style != value)
                {
                    pos_29_style = value;
                }
                OnPropertyChanged("POS_29");
            }
        }
        private int pos_30_style = 0;
        public int POS_30
        {

            get { return pos_30_style; }
            set
            {
                if (pos_30_style != value)
                {
                    pos_30_style = value;
                }
                OnPropertyChanged("POS_30");
            }
        }
        private int pos_31_style = 0;
        public int POS_31
        {

            get { return pos_31_style; }
            set
            {
                if (pos_31_style != value)
                {
                    pos_31_style = value;
                }
                OnPropertyChanged("POS_31");
            }
        }
        private int pos_32_style = 0;
        public int POS_32
        {

            get { return pos_32_style; }
            set
            {
                if (pos_32_style != value)
                {
                    pos_32_style = value;
                }
                OnPropertyChanged("POS_32");
            }
        }
        private int pos_33_style = 0;
        public int POS_33
        {

            get { return pos_33_style; }
            set
            {
                if (pos_33_style != value)
                {
                    pos_33_style = value;
                }
                OnPropertyChanged("POS_33");
            }
        }
        private int pos_34_style = 0;
        public int POS_34
        {

            get { return pos_34_style; }
            set
            {
                if (pos_34_style != value)
                {
                    pos_34_style = value;
                }
                OnPropertyChanged("POS_34");
            }
        }
        private int pos_35_style = 0;
        public int POS_35
        {

            get { return pos_35_style; }
            set
            {
                if (pos_35_style != value)
                {
                    pos_35_style = value;
                }
                OnPropertyChanged("POS_35");
            }
        }
        private int pos_36_style = 0;
        public int POS_36
        {

            get { return pos_36_style; }
            set
            {
                if (pos_36_style != value)
                {
                    pos_36_style = value;
                }
                OnPropertyChanged("POS_36");
            }
        }
        private int pos_37_style = 0;
        public int POS_37
        {

            get { return pos_37_style; }
            set
            {
                if (pos_37_style != value)
                {
                    pos_37_style = value;
                }
                OnPropertyChanged("POS_37");
            }
        }
        private int pos_38_style = 0;
        public int POS_38
        {

            get { return pos_38_style; }
            set
            {
                if (pos_38_style != value)
                {
                    pos_38_style = value;
                }
                OnPropertyChanged("POS_38");
            }
        }
        private int pos_39_style = 0;
        public int POS_39
        {

            get { return pos_39_style; }
            set
            {
                if (pos_39_style != value)
                {
                    pos_39_style = value;
                }
                OnPropertyChanged("POS_39");
            }
        }
        private int pos_40_style = 0;
        public int POS_40
        {

            get { return pos_40_style; }
            set
            {
                if (pos_40_style != value)
                {
                    pos_40_style = value;
                }
                OnPropertyChanged("POS_40");
            }
        }
        private int pos_41_style = 0;
        public int POS_41
        {

            get { return pos_41_style; }
            set
            {
                if (pos_41_style != value)
                {
                    pos_41_style = value;
                }
                OnPropertyChanged("POS_41");
            }
        }
        private int pos_42_style = 0;
        public int POS_42
        {

            get { return pos_42_style; }
            set
            {
                if (pos_42_style != value)
                {
                    pos_42_style = value;
                }
                OnPropertyChanged("POS_42");
            }
        }
        private int pos_43_style = 0;
        public int POS_43
        {

            get { return pos_43_style; }
            set
            {
                if (pos_43_style != value)
                {
                    pos_43_style = value;
                }
                OnPropertyChanged("POS_43");
            }
        }
        private int pos_44_style = 0;
        public int POS_44
        {

            get { return pos_44_style; }
            set
            {
                if (pos_44_style != value)
                {
                    pos_44_style = value;
                }
                OnPropertyChanged("POS_44");
            }
        }
        private int pos_45_style = 0;
        public int POS_45
        {

            get { return pos_45_style; }
            set
            {
                if (pos_45_style != value)
                {
                    pos_45_style = value;
                }
                OnPropertyChanged("POS_45");
            }
        }
        private int pos_46_style = 0;
        public int POS_46
        {

            get { return pos_46_style; }
            set
            {
                if (pos_46_style != value)
                {
                    pos_46_style = value;
                }
                OnPropertyChanged("POS_46");
            }
        }
        private int pos_47_style = 0;
        public int POS_47
        {

            get { return pos_47_style; }
            set
            {
                if (pos_47_style != value)
                {
                    pos_47_style = value;
                }
                OnPropertyChanged("POS_47");
            }
        }
        private int pos_48_style = 0;
        public int POS_48
        {

            get { return pos_48_style; }
            set
            {
                if (pos_48_style != value)
                {
                    pos_48_style = value;
                }
                OnPropertyChanged("POS_48");
            }
        }
        private int pos_49_style = 0;
        public int POS_49
        {

            get { return pos_49_style; }
            set
            {
                if (pos_49_style != value)
                {
                    pos_49_style = value;
                }
                OnPropertyChanged("POS_49");
            }
        }
        private int pos_50_style = 0;
        public int POS_50
        {

            get { return pos_50_style; }
            set
            {
                if (pos_50_style != value)
                {
                    pos_50_style = value;
                }
                OnPropertyChanged("POS_50");
            }
        }
        private int pos_51_style = 0;
        public int POS_51
        {

            get { return pos_51_style; }
            set
            {
                if (pos_51_style != value)
                {
                    pos_51_style = value;
                }
                OnPropertyChanged("POS_51");
            }
        }
        private int pos_52_style = 0;
        public int POS_52
        {

            get { return pos_52_style; }
            set
            {
                if (pos_52_style != value)
                {
                    pos_52_style = value;
                }
                OnPropertyChanged("POS_52");
            }
        }
        private int pos_53_style = 0;
        public int POS_53
        {

            get { return pos_53_style; }
            set
            {
                if (pos_53_style != value)
                {
                    pos_53_style = value;
                }
                OnPropertyChanged("POS_53");
            }
        }
        private int pos_54_style = 0;
        public int POS_54
        {

            get { return pos_54_style; }
            set
            {
                if (pos_54_style != value)
                {
                    pos_54_style = value;
                }
                OnPropertyChanged("POS_54");
            }
        }
        private int pos_55_style = 0;
        public int POS_55
        {

            get { return pos_55_style; }
            set
            {
                if (pos_55_style != value)
                {
                    pos_55_style = value;
                }
                OnPropertyChanged("POS_55");
            }
        }
        private int pos_56_style = 0;
        public int POS_56
        {

            get { return pos_56_style; }
            set
            {
                if (pos_56_style != value)
                {
                    pos_56_style = value;
                }
                OnPropertyChanged("POS_56");
            }
        }
        private int pos_57_style = 0;
        public int POS_57
        {

            get { return pos_57_style; }
            set
            {
                if (pos_57_style != value)
                {
                    pos_57_style = value;
                }
                OnPropertyChanged("POS_57");
            }
        }
        private int pos_58_style = 0;
        public int POS_58
        {

            get { return pos_58_style; }
            set
            {
                if (pos_58_style != value)
                {
                    pos_58_style = value;
                }
                OnPropertyChanged("POS_58");
            }
        }
        private int pos_59_style = 0;
        public int POS_59
        {

            get { return pos_59_style; }
            set
            {
                if (pos_59_style != value)
                {
                    pos_59_style = value;
                }
                OnPropertyChanged("POS_59");
            }
        }
        private int pos_60_style = 0;
        public int POS_60
        {

            get { return pos_60_style; }
            set
            {
                if (pos_60_style != value)
                {
                    pos_60_style = value;
                }
                OnPropertyChanged("POS_60");
            }
        }
        private int pos_61_style = 0;
        public int POS_61
        {

            get { return pos_61_style; }
            set
            {
                if (pos_61_style != value)
                {
                    pos_61_style = value;
                }
                OnPropertyChanged("POS_61");
            }
        }
        private int pos_62_style = 0;
        public int POS_62
        {

            get { return pos_62_style; }
            set
            {
                if (pos_62_style != value)
                {
                    pos_62_style = value;
                }
                OnPropertyChanged("POS_62");
            }
        }
        private int pos_63_style = 0;
        public int POS_63
        {

            get { return pos_63_style; }
            set
            {
                if (pos_63_style != value)
                {
                    pos_63_style = value;
                }
                OnPropertyChanged("POS_63");
            }
        }
        private int pos_64_style = 0;
        public int POS_64
        {

            get { return pos_64_style; }
            set
            {
                if (pos_64_style != value)
                {
                    pos_64_style = value;
                }
                OnPropertyChanged("POS_64");
            }
        }
        private int pos_65_style = 0;
        public int POS_65
        {

            get { return pos_65_style; }
            set
            {
                if (pos_65_style != value)
                {
                    pos_65_style = value;
                }
                OnPropertyChanged("POS_65");
            }
        }
        private int pos_66_style = 0;
        public int POS_66
        {

            get { return pos_66_style; }
            set
            {
                if (pos_66_style != value)
                {
                    pos_66_style = value;
                }
                OnPropertyChanged("POS_66");
            }
        }
        private int pos_67_style = 0;
        public int POS_67
        {

            get { return pos_67_style; }
            set
            {
                if (pos_67_style != value)
                {
                    pos_67_style = value;
                }
                OnPropertyChanged("POS_67");
            }
        }
        private int pos_68_style = 0;
        public int POS_68
        {

            get { return pos_68_style; }
            set
            {
                if (pos_68_style != value)
                {
                    pos_68_style = value;
                }
                OnPropertyChanged("POS_68");
            }
        }
        private int pos_69_style = 0;
        public int POS_69
        {

            get { return pos_69_style; }
            set
            {
                if (pos_69_style != value)
                {
                    pos_69_style = value;
                }
                OnPropertyChanged("POS_69");
            }
        }
        private int pos_70_style = 0;
        public int POS_70
        {

            get { return pos_70_style; }
            set
            {
                if (pos_70_style != value)
                {
                    pos_70_style = value;
                }
                OnPropertyChanged("POS_70");
            }
        }
        private int pos_71_style = 0;
        public int POS_71
        {

            get { return pos_71_style; }
            set
            {
                if (pos_71_style != value)
                {
                    pos_71_style = value;
                }
                OnPropertyChanged("POS_71");
            }
        }
        private int pos_72_style = 0;
        public int POS_72
        {

            get { return pos_72_style; }
            set
            {
                if (pos_72_style != value)
                {
                    pos_72_style = value;
                }
                OnPropertyChanged("POS_72");
            }
        }
        private int pos_73_style = 0;
        public int POS_73
        {

            get { return pos_73_style; }
            set
            {
                if (pos_73_style != value)
                {
                    pos_73_style = value;
                }
                OnPropertyChanged("POS_73");
            }
        }
        private int pos_74_style = 0;
        public int POS_74
        {

            get { return pos_74_style; }
            set
            {
                if (pos_74_style != value)
                {
                    pos_74_style = value;
                }
                OnPropertyChanged("POS_74");
            }
        }
        private int pos_75_style = 0;
        public int POS_75
        {

            get { return pos_75_style; }
            set
            {
                if (pos_75_style != value)
                {
                    pos_75_style = value;
                }
                OnPropertyChanged("POS_75");
            }
        }
        private int pos_76_style = 0;
        public int POS_76
        {

            get { return pos_76_style; }
            set
            {
                if (pos_76_style != value)
                {
                    pos_76_style = value;
                }
                OnPropertyChanged("POS_76");
            }
        }
        private int pos_77_style = 0;
        public int POS_77
        {

            get { return pos_77_style; }
            set
            {
                if (pos_77_style != value)
                {
                    pos_77_style = value;
                }
                OnPropertyChanged("POS_77");
            }
        }
        private int pos_78_style = 0;
        public int POS_78
        {

            get { return pos_78_style; }
            set
            {
                if (pos_78_style != value)
                {
                    pos_78_style = value;
                }
                OnPropertyChanged("POS_78");
            }
        }
        private int pos_79_style = 0;
        public int POS_79
        {

            get { return pos_79_style; }
            set
            {
                if (pos_79_style != value)
                {
                    pos_79_style = value;
                }
                OnPropertyChanged("POS_79");
            }
        }
        private int pos_80_style = 0;
        public int POS_80
        {

            get { return pos_80_style; }
            set
            {
                if (pos_80_style != value)
                {
                    pos_80_style = value;
                }
                OnPropertyChanged("POS_80");
            }
        }
      
 
    }
}
