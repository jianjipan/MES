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

namespace MMIS
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private string password;
        private int mode;
        public LoginWindow(string password,int mode)  //mode为1表示从权限按钮进入的，mode为2表示从立库管理按钮进入的
        {
            InitializeComponent();
            this.password = password;
            this.mode = mode;

        }

        //登录按钮
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (mode == 1)
            {
                if (User.Text == "dldz" && Password.Password == password)
                {
                    MainWindow.kuweinumber.QUANXIANMODE = "管理员模式";
                    this.Close();
                }
                else
                {
                    MessageBox.Show("用户名或密码输入不正确，请重新输入");
                }
                return;
            }
            if (mode == 2)
            {
                if (User.Text == "dldz" && Password.Password == password)
                {
                    KuweiShow kuweishow = new KuweiShow();
                    kuweishow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("用户名或密码输入不正确，请重新输入");
                }
            }
        }

        //退出按钮
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
