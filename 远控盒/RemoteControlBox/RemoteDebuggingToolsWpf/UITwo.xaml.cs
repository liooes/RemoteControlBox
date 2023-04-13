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
    /// UITwo.xaml 的交互逻辑
    /// </summary>
    public partial class UITwo : Window
    {
        FileOpertion fo = new FileOpertion();
        public UITwo()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 载入用户值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            loadRateVoltageCurrent();
        }

        /// <summary>
        /// 载入额定电压电流
        /// </summary>
        void loadRateVoltageCurrent()
        {
            txtRateVoltage.Text = fo.getUserSetRatedVoltage();
            txtRateCurrent.Text = fo.getUserSetRatedCurrent();
        }

        /// <summary>
        /// 修改额定电压、额定电流、用户密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetData_Click(object sender, RoutedEventArgs e)
        {
            string sratevoltage = txtRateVoltage.Text.Trim();
            string sratecurrent = txtRateCurrent.Text.Trim();
            string suserOddpwd = txtUserPwdOne.Text.Trim();
            string suserNewpwd = txtUserPwdTwo.Text.Trim();



            //修改额定电压额定电流
            setRateVoltageCurrent(sratevoltage, sratecurrent);
            // 修改用户密码
            setUserPwd(suserOddpwd, suserNewpwd);
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="suserOddpwd">密码1</param>
        /// <param name="suserNewpwd">密码2</param>
        private void setUserPwd(string suserOddpwd, string suserNewpwd)
        {
            if (suserOddpwd == suserNewpwd)
            {
                fo.setUserPwd(suserNewpwd);
                ShowMsgHandle.ShowMsg("密码修改成功！");
            }
            else
            {
                ShowMsgHandle.ShowMsg("两次密码输入不一致！");
            }
        }

        /// <summary>
        /// 修改额定电压额定电流
        /// </summary>
        /// <param name="sratevoltage"></param>
        /// <param name="sratecurrent"></param>
        private void setRateVoltageCurrent(string sratevoltage, string sratecurrent)
        {
            if (!string.IsNullOrWhiteSpace(sratevoltage) || !string.IsNullOrWhiteSpace(sratecurrent))
            {
                if (!(float.Parse(sratevoltage) >= 65535 || float.Parse(sratecurrent) >= 65535))
                {
                    fo.setRatedVoltage(sratevoltage);
                    fo.setSetRatedCurrent(sratecurrent);
                    ShowMsgHandle.ShowMsg("修改额定电压额定电流成功");
                }
                else
                {
                    ShowMsgHandle.ShowMsg("额定电压，额定电流不能大于65535");
                }
            }
            else
            {
                ShowMsgHandle.ShowMsg("额定电压，额定电流不能为空或空白");
            }
        }

        /// <summary>
        /// 返回登陆界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBarkToMain_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
