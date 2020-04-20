using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;

namespace MMIS
{
    /// <summary>
    /// KuweiShow.xaml 的交互逻辑
    /// </summary>
    public partial class KuweiShow : Window
    {
        public KuweiShow()
        {
            InitializeComponent();
        }
        WHPosition position = new WHPosition();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {       
            Link();
            //读取数据库CargoInfo的数据
            Get_db_CargoInfo();
        }
        //读取数据库CargoInfo的数据
        private void Get_db_CargoInfo()
        {
            DataBaseHandle db = new DataBaseHandle();
            SqlConnection con = new SqlConnection();
            DataTable dt = db.GetKuweiArray(con);
            for (int i = 0; i < 80; i++)
            {
                if (Convert.ToInt32(dt.Rows[i][0]) == 0)
                {
                    dt.Rows[i][0] = -10;   //代表-10
                }
            }
            position.POS_1 = Convert.ToInt32(dt.Rows[0][0]);
            position.POS_2 = Convert.ToInt32(dt.Rows[1][0]);
            position.POS_3 = Convert.ToInt32(dt.Rows[2][0]);
            position.POS_4 = Convert.ToInt32(dt.Rows[3][0]);
            position.POS_5 = Convert.ToInt32(dt.Rows[4][0]);
            position.POS_6 = Convert.ToInt32(dt.Rows[5][0]);
            position.POS_7 = Convert.ToInt32(dt.Rows[6][0]);
            position.POS_8 = Convert.ToInt32(dt.Rows[7][0]);
            position.POS_9 = Convert.ToInt32(dt.Rows[8][0]);
            position.POS_10 = Convert.ToInt32(dt.Rows[9][0]);
            position.POS_11 = Convert.ToInt32(dt.Rows[10][0]);
            position.POS_12 = Convert.ToInt32(dt.Rows[11][0]);
            position.POS_13 = Convert.ToInt32(dt.Rows[12][0]);
            position.POS_14 = Convert.ToInt32(dt.Rows[13][0]);
            position.POS_15 = Convert.ToInt32(dt.Rows[14][0]);
            position.POS_16 = Convert.ToInt32(dt.Rows[15][0]);
            position.POS_17 = Convert.ToInt32(dt.Rows[16][0]);
            position.POS_18 = Convert.ToInt32(dt.Rows[17][0]);
            position.POS_19 = Convert.ToInt32(dt.Rows[18][0]);
            position.POS_20 = Convert.ToInt32(dt.Rows[19][0]);
            position.POS_21 = Convert.ToInt32(dt.Rows[20][0]);
            position.POS_22 = Convert.ToInt32(dt.Rows[21][0]);
            position.POS_23 = Convert.ToInt32(dt.Rows[22][0]);
            position.POS_24 = Convert.ToInt32(dt.Rows[23][0]);
            position.POS_25 = Convert.ToInt32(dt.Rows[24][0]);
            position.POS_26 = Convert.ToInt32(dt.Rows[25][0]);
            position.POS_27 = Convert.ToInt32(dt.Rows[26][0]);
            position.POS_28 = Convert.ToInt32(dt.Rows[27][0]);
            position.POS_29 = Convert.ToInt32(dt.Rows[28][0]);
            position.POS_30 = Convert.ToInt32(dt.Rows[29][0]);
            position.POS_31 = Convert.ToInt32(dt.Rows[30][0]);
            position.POS_32 = Convert.ToInt32(dt.Rows[31][0]);
            position.POS_33 = Convert.ToInt32(dt.Rows[32][0]);
            position.POS_34 = Convert.ToInt32(dt.Rows[33][0]);
            position.POS_35 = Convert.ToInt32(dt.Rows[34][0]);
            position.POS_36 = Convert.ToInt32(dt.Rows[35][0]);
            position.POS_37 = Convert.ToInt32(dt.Rows[36][0]);
            position.POS_38 = Convert.ToInt32(dt.Rows[37][0]);
            position.POS_39 = Convert.ToInt32(dt.Rows[38][0]);
            position.POS_40 = Convert.ToInt32(dt.Rows[39][0]);
            position.POS_41 = Convert.ToInt32(dt.Rows[40][0]);
            position.POS_42 = Convert.ToInt32(dt.Rows[41][0]);
            position.POS_43 = Convert.ToInt32(dt.Rows[42][0]);
            position.POS_44 = Convert.ToInt32(dt.Rows[43][0]);
            position.POS_45 = Convert.ToInt32(dt.Rows[44][0]);
            position.POS_46 = Convert.ToInt32(dt.Rows[45][0]);
            position.POS_47 = Convert.ToInt32(dt.Rows[46][0]);
            position.POS_48 = Convert.ToInt32(dt.Rows[47][0]);
            position.POS_49 = Convert.ToInt32(dt.Rows[48][0]);
            position.POS_50 = Convert.ToInt32(dt.Rows[49][0]);
            position.POS_51 = Convert.ToInt32(dt.Rows[50][0]);
            position.POS_52 = Convert.ToInt32(dt.Rows[51][0]);
            position.POS_53 = Convert.ToInt32(dt.Rows[52][0]);
            position.POS_54 = Convert.ToInt32(dt.Rows[53][0]);
            position.POS_55 = Convert.ToInt32(dt.Rows[54][0]);
            position.POS_56 = Convert.ToInt32(dt.Rows[55][0]);
            position.POS_57 = Convert.ToInt32(dt.Rows[56][0]);
            position.POS_58 = Convert.ToInt32(dt.Rows[57][0]);
            position.POS_59 = Convert.ToInt32(dt.Rows[58][0]);
            position.POS_60 = Convert.ToInt32(dt.Rows[59][0]);
            position.POS_61 = Convert.ToInt32(dt.Rows[60][0]);
            position.POS_62 = Convert.ToInt32(dt.Rows[61][0]);
            position.POS_63 = Convert.ToInt32(dt.Rows[62][0]);
            position.POS_64 = Convert.ToInt32(dt.Rows[63][0]);
            position.POS_65 = Convert.ToInt32(dt.Rows[64][0]);
            position.POS_66 = Convert.ToInt32(dt.Rows[65][0]);
            position.POS_67 = Convert.ToInt32(dt.Rows[66][0]);
            position.POS_68 = Convert.ToInt32(dt.Rows[67][0]);
            position.POS_69 = Convert.ToInt32(dt.Rows[68][0]);
            position.POS_70 = Convert.ToInt32(dt.Rows[69][0]);
            position.POS_71 = Convert.ToInt32(dt.Rows[70][0]);
            position.POS_72 = Convert.ToInt32(dt.Rows[71][0]);
            position.POS_73 = Convert.ToInt32(dt.Rows[72][0]);
            position.POS_74 = Convert.ToInt32(dt.Rows[73][0]);
            position.POS_75 = Convert.ToInt32(dt.Rows[74][0]);
            position.POS_76 = Convert.ToInt32(dt.Rows[75][0]);
            position.POS_77 = Convert.ToInt32(dt.Rows[76][0]);
            position.POS_78 = Convert.ToInt32(dt.Rows[77][0]);
            position.POS_79 = Convert.ToInt32(dt.Rows[78][0]);
            position.POS_80 = Convert.ToInt32(dt.Rows[79][0]);
        }

        //把UI控件和属性类联系起来。
        private void Link()
        {
            this.Position_1.DataContext = position;
            this.Position_2.DataContext = position;
            this.Position_3.DataContext = position;
            this.Position_4.DataContext = position;
            this.Position_5.DataContext = position;
            this.Position_6.DataContext = position;
            this.Position_7.DataContext = position;
            this.Position_8.DataContext = position;
            this.Position_9.DataContext = position;
            this.Position_10.DataContext = position;
            this.Position_11.DataContext = position;
            this.Position_12.DataContext = position;
            this.Position_13.DataContext = position;
            this.Position_14.DataContext = position;
            this.Position_15.DataContext = position;
            this.Position_16.DataContext = position;
            this.Position_17.DataContext = position;
            this.Position_18.DataContext = position;
            this.Position_19.DataContext = position;
            this.Position_20.DataContext = position;
            this.Position_21.DataContext = position;
            this.Position_22.DataContext = position;
            this.Position_23.DataContext = position;
            this.Position_24.DataContext = position;
            this.Position_25.DataContext = position;
            this.Position_26.DataContext = position;
            this.Position_27.DataContext = position;
            this.Position_28.DataContext = position;
            this.Position_29.DataContext = position;
            this.Position_30.DataContext = position;
            this.Position_31.DataContext = position;
            this.Position_32.DataContext = position;
            this.Position_33.DataContext = position;
            this.Position_34.DataContext = position;
            this.Position_35.DataContext = position;
            this.Position_36.DataContext = position;
            this.Position_37.DataContext = position;
            this.Position_38.DataContext = position;
            this.Position_39.DataContext = position;
            this.Position_40.DataContext = position;
            this.Position_41.DataContext = position;
            this.Position_42.DataContext = position;
            this.Position_43.DataContext = position;
            this.Position_44.DataContext = position;
            this.Position_45.DataContext = position;
            this.Position_46.DataContext = position;
            this.Position_47.DataContext = position;
            this.Position_48.DataContext = position;
            this.Position_49.DataContext = position;
            this.Position_50.DataContext = position;
            this.Position_51.DataContext = position;
            this.Position_52.DataContext = position;
            this.Position_53.DataContext = position;
            this.Position_54.DataContext = position;
            this.Position_55.DataContext = position;
            this.Position_56.DataContext = position;
            this.Position_57.DataContext = position;
            this.Position_58.DataContext = position;
            this.Position_59.DataContext = position;
            this.Position_60.DataContext = position;
            this.Position_61.DataContext = position;
            this.Position_62.DataContext = position;
            this.Position_63.DataContext = position;
            this.Position_64.DataContext = position;
            this.Position_65.DataContext = position;
            this.Position_66.DataContext = position;
            this.Position_67.DataContext = position;
            this.Position_68.DataContext = position;
            this.Position_69.DataContext = position;
            this.Position_70.DataContext = position;
            this.Position_71.DataContext = position;
            this.Position_72.DataContext = position;
            this.Position_73.DataContext = position;
            this.Position_74.DataContext = position;
            this.Position_75.DataContext = position;
            this.Position_76.DataContext = position;
            this.Position_77.DataContext = position;
            this.Position_78.DataContext = position;
            this.Position_79.DataContext = position;
            this.Position_80.DataContext = position;
        }


        //保存修改按钮事件
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (xiugai_position.Text == "")
                {
                    MessageBox.Show("请选择库位");
                    return;
                }
                if (Convert.ToInt32(xiugai_position.Text) > 80 || Convert.ToInt32(xiugai_position.Text) < 1)
                {
                    MessageBox.Show("请选择正确的库位");
                    return;
                }
                if (Convert.ToInt32(xiugai_position.Text) == 44 || Convert.ToInt32(xiugai_position.Text) == 45)
                {
                    MessageBox.Show("该库位为禁用库位，请选择其他库位");
                    return;
                }
                int position = Convert.ToInt32(xiugai_position.Text);
                int TrayStyle = xiugai_traystyle.SelectedIndex;
                DataBaseHandle db = new DataBaseHandle();
                SqlConnection con = new SqlConnection();
                db.SaveKuweiArray(con, position, TrayStyle);
                db.UpdateKuweiArray(con);
                Get_db_CargoInfo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("用户非法输入，请输入数字"+ex.ToString());
            }
        }
    }
}
