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

namespace RemoteDebuggingToolsWpf
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private static readonly string superPwd = "song";
        public Login()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogon_Click(object sender, RoutedEventArgs e)
        {
            string sUserPwd = new FileOpertion().getUserPwd().Trim();
            if (txtUserPwd.Text.Trim().Equals(sUserPwd) || txtUserPwd.Text.Trim().Equals(superPwd))//超级密码
            {
                UITwo uiTwo = new UITwo();
                uiTwo.ShowDialog();
            }
            else
            {
                MessageBox.Show("密码错误！");
            }
        }
    }
}
