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
    /// UserWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
        }

        //普通用户权限
        private void CommonUser_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.kuweinumber.QUANXIANMODE = "普通用户模式";
            this.Close();
        }

        //管理员权限
        private void VIPUser_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow(ConfigClass.WHMANNGERSYS_PASSWORD, 1);
            login.Show();
        }

    }
}
