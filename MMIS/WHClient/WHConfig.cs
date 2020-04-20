using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMIS
{
    public class WHConfig
    {
        //三维数组,等同于定义2个5行7列得数组
        private static int[] KuweiState = new int[80];
        //索引器
        public  int this[int i]
        {
            get { return KuweiState[i]; }
            set { KuweiState[i] = value; }
        }
        public static List<int> Tray_Empty = new List<int>();  //加工空托盘A0
        public static List<int> Tray_A0 = new List<int>();  //加工空托盘A0
        public static List<int> Tray_A1 = new List<int>();  //加工毛胚托盘A1
        public static List<int> Tray_A2 = new List<int>();  //加工成品托盘A2
        public static List<int> Tray_A3 = new List<int>();  //加工检测托盘A3
        public static List<int> Tray_A4 = new List<int>();  //轴承压装毛胚托盘A4
        public static List<int> Tray_A5 = new List<int>();   //轴承压装完成托盘A5
        public static List<int> Tray_A6 = new List<int>();   //轴承压装空托盘A6
        public static List<int> Tray_B0 = new List<int>();   //拧螺丝空托盘B0
        public static List<int> Tray_B1 = new List<int>();   //拧螺丝打钉托盘B1
        public static List<int> Tray_B2 = new List<int>();   //螺丝完成托盘B2
        public static List<int> Tray_C = new List<int>();    //轴承托盘C
        public static List<int> Tray_D = new List<int>();    //螺钉托盘D 
    }
}
