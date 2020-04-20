using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace MMIS
{
    //将库位表中，各库位的托盘样式的值转换为字符串
    class WHPositionConvert:IValueConverter
    {
        //当值从绑定源传播给绑定目标时，调用方法Convert
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            int pos_value = (int)value;
            string pos_string = "";
            if (pos_value == -2)
            {
                pos_string = "禁";
                return pos_string;
            }
            if (pos_value == -10)
            {
                pos_string="空";
                return pos_string;
            }
            if (pos_value == 1)
            {
                pos_string="A0";
                return pos_string;
            }
            if (pos_value == 2)
            {
                pos_string = "A1";
                return pos_string;
            }
            if (pos_value == 3)
            {
                pos_string = "A2";
                return pos_string;
            }
            if (pos_value == 4)
            {
                pos_string = "A3";
                return pos_string;
            }
            if (pos_value == 5)
            {
                pos_string = "B1";
                return pos_string;
            }
            if (pos_value == 6)
            {
                pos_string = "B2";
                return pos_string;
            }
            if (pos_value == 7)
            {
                pos_string = "B0";
                return pos_string;
            }
            if (pos_value == 8)
            {
                pos_string = "C0";
                return pos_string;
            }
            if (pos_value == 9)
            {
                pos_string = "C1";
                return pos_string;
            }
            if (pos_value == 10)
            {
                pos_string = "C2";
                return pos_string;
            }
            if (pos_value == 11)
            {
                pos_string = "D";
                return pos_string;
            }
            if (pos_value == 12)
            {
                pos_string = "E";
                return pos_string;
            }
            else
            {
                pos_string = "";
                return pos_string;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string pos_string = value as string;
            int pos_value;
            if (pos_string == "禁")
            {
                pos_value = -2;
                return pos_value;
            }
            if (pos_string == "空")
            {
                pos_value = -10;
                return pos_value;
            }
            if (pos_string == "A0")
            {
                pos_value = 1;
                return pos_value;
            }
            if (pos_string == "A1")
            {
                pos_value = 2;
                return pos_value;
            }
            if (pos_string == "A2")
            {
                pos_value = 3;
                return pos_value;
            }
            if (pos_string == "A3")
            {
                pos_value = 4;
                return pos_value;
            }
            if (pos_string == "B1")
            {
                pos_value = 5;
                return pos_value;
            }
            if (pos_string == "B2")
            {
                pos_value = 6;
                return pos_value;
            }
            if (pos_string == "B0")
            {
                pos_value = 7;
                return pos_value;
            }
            if (pos_string == "C0")
            {
                pos_value = 8;
                return pos_value;
            }
            if (pos_string == "C1")
            {
                pos_value = 9;
                return pos_value;
            }
            if (pos_string == "C2")
            {
                pos_value = 10;
                return pos_value;
            }
            if (pos_string == "D")
            {
                pos_value = 11;
                return pos_value;
            }
            if (pos_string == "E")
            {
                pos_value = 12;
                return pos_value;
            }
            return DependencyProperty.UnsetValue;
        }

    }

    class WHPositionTOColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            int pos_value = (int)value;
            string pos_color = "";
            if (pos_value == -2)
            {
                pos_color = "Black";
                return pos_color;
            }
            if (pos_value == -10)
            {
                pos_color = "PaleGreen";
                return pos_color;
            }
            if (pos_value == 1)
            {
                pos_color = "Olive";
                return pos_color;
            }
            if (pos_value == 2)
            {
                pos_color = "Red";
                return pos_color;
            }
            if (pos_value == 3)
            {
                pos_color = "Chocolate";
                return pos_color;
            }
            if (pos_value == 4)
            {
                pos_color = "SkyBlue";
                return pos_color;
            }
            if (pos_value == 5)
            {
                pos_color = "IndianRed";
                return pos_color;
            }
            if (pos_value == 6)
            {
                pos_color = "Lime";
                return pos_color;
            }
            if (pos_value == 7)
            {
                pos_color = "Pink";
                return pos_color;
            }
            if (pos_value == 8)
            {
                pos_color = "HotPink";
                return pos_color;
            }
            if (pos_value == 9)
            {
                pos_color = "DarkRed";
                return pos_color;
            }
            if (pos_value == 10)
            {
                pos_color = "Brown";
                return pos_color;
            }
            if (pos_value == 11)
            {
                pos_color = "LightBlue";
                return pos_color;
            }
            if (pos_value == 12)
            {
                pos_color = "Indigo";
                return pos_color;
            }
            else
            {
                pos_color = "Transparent";
                return pos_color;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
