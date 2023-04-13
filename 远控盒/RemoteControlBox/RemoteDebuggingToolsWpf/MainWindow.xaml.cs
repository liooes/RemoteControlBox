using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RemoteDebuggingToolsWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        FileOpertion fo = new FileOpertion();
        RemoteControlBox rcb;
        bool isBusy = false;
        /// <summary>
        /// 设置当前电压电流委托
        /// </summary>
        /// <param name="sCurrentVoltage">当前电压</param>
        /// <param name="sCurrentCurrent">当前电流</param>
        delegate void degsetCurrentVoltageCurrentCallBack(string sCurrentVoltage, string sCurrentCurrent);

        /// <summary>
        /// 设置当前通讯状态委托
        /// </summary>
        /// <param name="commState">通讯状态</param>
        delegate void degsetCurrentCommStateCallBack(bool commState);


        //声明
        degsetCurrentVoltageCurrentCallBack setCurrentVoltageCurrentCallback;
        degsetCurrentCommStateCallBack setCurrentCommStateCallback;

        Thread thSetVoltageCurrend;//设置电压电流线程
        Thread thSetCommState;//设置通讯状态线程

        public MainWindow()
        {
            InitializeComponent();
        }

        #region 操作窗口

        /// <summary>
        /// 最大化最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMaxMin_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
                this.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// 最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMini_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Minimized;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            rcb = null;
            this.Close();
        }

        /// <summary>
        /// 移动窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gTiele_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
        }

        #endregion

        #region 载入串口值
        /// <summary>
        /// 载入所有串口
        /// </summary>
        void loadSerialPort()
        {
            string[] sPort = SerialPort.GetPortNames();
            for (int i = 0; i < sPort.Length; i++)
                cbxComPort.Items.Add(sPort[i]);
        }

        /// <summary>
        /// 载入波特率
        /// </summary>
        void loadBaudrate()
        {
            string[] sbaudRate = new string[] { "4800", "9600", "19200", "38400", "43000" };
            for (int i = 0; i < sbaudRate.Length; i++)
                cbxBaudrate.Items.Add(sbaudRate[i]);

        }

        /// <summary>
        /// 载入校验位
        /// </summary>
        void loadParity()
        {
            string[] sparity = new string[] { "奇", "偶", "无" };
            for (int i = 0; i < sparity.Length; i++)
                cbxParity.Items.Add(sparity[i]);

        }

        /// <summary>
        /// 载入停止位
        /// </summary>
        void loadStopBits()
        {
            string[] sparity = new string[] { "1", "1.5", "2" };
            for (int i = 0; i < sparity.Length; i++)
                cbxStopBits.Items.Add(sparity[i]);

        }

        /// <summary>
        /// 载入选中项
        /// </summary>
        void loadSelectedData()
        {
            try
            {
                cbxBaudrate.SelectedIndex = 1;
                cbxComPort.SelectedIndex = 0;
                cbxParity.SelectedIndex = 2;
                cbxStopBits.SelectedIndex = 2;
            }
            catch { }
        }

        #endregion

        #region 操作颜色
        /// <summary>
        /// 获取绿色
        /// </summary>
        /// <param name="icolor"></param>
        /// <returns></returns>
        SolidColorBrush getGreen()
        {
            Color cgreen = Color.FromRgb(34, 177, 76);
            return new SolidColorBrush(cgreen);

        }

        /// <summary>
        /// 获取红色
        /// </summary>
        /// <param name="icolor"></param>
        /// <returns></returns>
        SolidColorBrush getRed()
        {
            Color cred = Color.FromRgb(237, 28, 36);
            return new SolidColorBrush(cred);

        }

        #endregion

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenSerialPort_Click(object sender, RoutedEventArgs e)
        {
            string sport = cbxComPort.SelectedValue.ToString();
            string ibaudrate = cbxBaudrate.SelectedValue.ToString();
            string sdatabits = "8";
            int iparity = cbxParity.SelectedIndex;
            int istopbits = cbxStopBits.SelectedIndex;

            if (btnOpenSerialPort.Content.Equals("打开串口"))
            {
                btnOpenSerialPort.Content = "关闭串口";
                //初始化远控盒对象设置串口参数
                 instantiationRCB(sport, ibaudrate, sdatabits, iparity, istopbits); 

                //初始化远控盒
                initRCB();
                //实时刷新当前电压当前电流
                refreshCurrentVoltageCurrent();
                //实时刷新通讯状态
                refreshCommState();
            }
            else
            {
                btnOpenSerialPort.Content = "打开串口";
                rcb.sCommState = false;
                loadCurrentCommState(rcb.sCommState);
                //释放远控盒和读取线程对象
                disposeThreadAndRCB();
            }
        }

        /// <summary>
        /// 实例化远控盒对象
        /// </summary>
        /// <param name="scom"></param>
        /// <param name="sbaudrate">波特率</param>
        /// <param name="databits">数据位</param>
        /// <param name="sparity">校验位 0奇 1偶 2无</param>
        /// <param name="sstopbits">停止位 0=停止位1  1=停止位1.5    2=停止位2</param>
        void instantiationRCB(string scom, string sbaudrate, string idatabits, int iparity, int istopbits)
        {
            rcb = new RemoteControlBox(scom, sbaudrate, idatabits, iparity, istopbits);
        }

        /// <summary>
        /// 释放远控盒和读取线程对象
        /// </summary>
        private void disposeThreadAndRCB()
        {
            try
            {
                rcb = null;
                thSetCommState.Abort();
                thSetVoltageCurrend.Abort();
                thSetCommState = null;
                thSetVoltageCurrend = null;
            }
            catch { }
        }

        /// <summary>
        /// 实时刷新通讯状态
        /// </summary>
        private void refreshCommState()
        {
            //实时刷新通讯状态
            setCurrentCommStateCallback = new degsetCurrentCommStateCallBack(loadCurrentCommState);
            thSetCommState = new Thread(new ThreadStart(setCurrentCommState));
            thSetCommState.IsBackground = true;
            thSetCommState.Start();
        }

        /// <summary>
        /// 实时刷新当前电压当前电流
        /// </summary>
        private void refreshCurrentVoltageCurrent()
        {
            setCurrentVoltageCurrentCallback = new degsetCurrentVoltageCurrentCallBack(loadCurrentVoltageCurrent);
            thSetVoltageCurrend = new Thread(new ThreadStart(setCurrentVoltageCurrent));
            thSetVoltageCurrend.IsBackground = true;
            thSetVoltageCurrend.Start();
        }

        /// <summary>
        /// 回调刷新通讯状态
        /// </summary>
        private void setCurrentCommState()
        {
            while (true)
            { 
                Thread.Sleep(100);
                if (rcb != null && !rcb.isBusy)
                {
                    rcb.isBusy = true;
                    rcb.getBoxCommState();
                    this.Dispatcher.Invoke(setCurrentCommStateCallback, rcb.sCommState);
                    rcb.isBusy = false;
                }
             
            }
        }

        /// <summary>
        /// 回调设置当前电压电流
        /// </summary>
        private void setCurrentVoltageCurrent()
        {
            while (true)
            { 
                Thread.Sleep(100);
                if (rcb != null && !rcb.isBusy)
                {
                    rcb.isBusy = true;
                    rcb.getCurrentVoltageCurrent();
                    this.Dispatcher.Invoke(setCurrentVoltageCurrentCallback, rcb.sCurrentVoltage, rcb.sCurrentCurrent);
                    rcb.isBusy = false;
                }
                
            }
        }

        /// <summary>
        /// 获取校验位
        /// </summary>
        /// <param name="sparity"></param>
        /// <returns>错误返回无</returns>
        Parity getParity(string sparity)
        {
            Parity p = Parity.None;
            switch (sparity)
            {
                case "奇": { p = Parity.Odd; } break;
                case "偶": { p = Parity.Even; } break;
                case "无": { p = Parity.None; } break;
            }
            return p;
        }

        /// <summary>
        /// 获取停止位
        /// </summary>
        /// <param name="sparity"></param>
        /// <returns>错误返回1</returns>
        StopBits getStopBits(string sparity)
        {
            StopBits s = StopBits.One;
            switch (sparity)
            {
                case "1": { s = StopBits.One; } break;
                case "1.5": { s = StopBits.OnePointFive; } break;
                case "2": { s = StopBits.Two; } break;
            }
            return s;
        }

        /// <summary>
        /// 初始化远控盒
        /// </summary>
        private void initRCB()
        {

            try
            {
                //设置地址
                rcb.setBoxAddress(txtDevAddress.Text);

                //载入额定电压额定电流值
                string sratedVoltage = fo.getUserSetRatedVoltage();
                string sratedCurrent = fo.getUserSetRatedCurrent();
                loadRatedVoltageCurrent(sratedVoltage, sratedCurrent);
                //停止远控盒
                rcb.stopBox();
                loadRCBState(rcb.sStartStopState);
                //切换稳压
                rcb.switchRegulator();
                rcb.sRSFState = false;
                loadRSFState(rcb.sRSFState);
                //写入设定电压设定电流软起动时间
                string svoltage = txtSetVoltage.Text.Trim();
                string scurrent = txtSetCurrent.Text.Trim();
                string ssoftstarttime = txtSoftStartTime.Text.Trim();
                rcb.setSetVoltage(svoltage);
                rcb.setSetCurrent(scurrent);
                rcb.setSoftStartTime(ssoftstarttime);
            }
            catch {
                rcb.sCommState = false;
                loadCurrentCommState(rcb.sCommState);
            }
        }

        /// <summary>
        /// 载入当前电压当前电流
        /// </summary>
        void loadCurrentVoltageCurrent(string scurrentVoltage, string scurrentcurrent)
        {
            txtCurrentVoltage.Text = scurrentVoltage.Substring(0, scurrentVoltage.Length - 1) +"V";
            txtCurrentCurrent.Text = scurrentcurrent.Substring(0, scurrentcurrent.Length - 1) +"A";

        }

        /// <summary>
        /// 载入稳压稳流状态
        /// </summary>
        /// <param name="brsf"></param>
        void loadRSFState(bool brsf)
        {
            if (!brsf)
                txtRSFState.Text = "稳压";
            else
                txtRSFState.Text = "稳流";
        }

        /// <summary>
        /// 载入远控盒状态
        /// </summary>
        /// <param name="bstate">true启动 false停止</param>
        void loadRCBState(bool bstate)
        {
            if (bstate)
                txtStartStopState.Text = "启动";
            else
                txtStartStopState.Text = "停止";
        }

        /// <summary>
        /// 载入额定电压电流
        /// </summary>
        void loadRatedVoltageCurrent(string sratedVoltage, string sratedCurrent)
        {
            txtRatedVoltage.Text = sratedVoltage+"V";
            txtRatedCurrent.Text = sratedCurrent+"A";
        }

        /// <summary>
        /// 载入当前通讯状态
        /// </summary>
        /// <param name="bcommstate">状态</param>
        void loadCurrentCommState(bool bcommstate)
        { 
            if (bcommstate)
            {
                txtCommState.Text = "通讯成功";
                txtCommState.Background = getGreen();
            }
            else
            {
                txtCommState.Text = "通讯失败";
                txtCommState.Background = getRed();
            }

        }


        /// <summary>
        /// 载入串口值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void main_Loaded(object sender, RoutedEventArgs e)
        {
            loadSerialPort();
            loadBaudrate();
            loadParity();
            loadStopBits();
            loadSelectedData();
        }

        /// <summary>
        /// 设置设备地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSetDevAddress_Click(object sender, RoutedEventArgs e)
        {
            string sdevaddress = txtDevAddress.Text;
            rcb.setBoxAddress(sdevaddress);
            ShowMsgHandle.ShowMsg("设置完成！");
        }

        /// <summary>
        /// 停止启动远控盒
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStartStopBox_Click(object sender, RoutedEventArgs e)
        {

            int i = 0;
            while (i == 0)
            {
                if (!rcb.isBusy)
                {
                    if (btnStartStopBox.Content.Equals("启动"))
                    {
                        btnStartStopBox.Content = "停止";
                        rcb.startBox();
                    }
                    else
                    {
                        btnStartStopBox.Content = "启动";
                        rcb.stopBox();
                    }

                    loadRCBState(rcb.sStartStopState);//成功后切换状态 
                    i++;
                }
            }
        }

        /// <summary>
        /// 切换稳压稳流
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSwitchRSF_Click(object sender, RoutedEventArgs e)
        {
            int i = 0;
            while (i == 0)
            {
                if (!rcb.isBusy)
                {
                    bool bRSF = rcb.sRSFState;//稳压稳流状态
                    if (bRSF)
                    {
                        rcb.switchSteadyFlow();
                    }
                    else
                    {
                        rcb.switchRegulator();
                    }
                    //切换完成之后载入状态
                    loadRSFState(bRSF);
                    i++;
                }
            }
        }

        /// <summary>
        /// 登陆界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenUITwo_Click(object sender, RoutedEventArgs e)
        {
            Login lg = new Login();
            if (lg.ShowDialog() != null)
            {
                //载入额定电压额定电流值
                string sratedVoltage = fo.getUserSetRatedVoltage();
                string sratedCurrent = fo.getUserSetRatedCurrent();
                loadRatedVoltageCurrent(sratedVoltage, sratedCurrent);
            }
        }

        /// <summary>
        /// 写入设定电压电流软启动时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSetVCSST_Click(object sender, RoutedEventArgs e)
        {

            int i = 0;
            while (i == 0)
            {
                if (!rcb.isBusy)
                {
                    string ssetVoltage = txtSetVoltage.Text.Trim();
                    string ssetCurrent = txtSetCurrent.Text.Trim();
                    string ssst = txtSoftStartTime.Text.Trim();

                    rcb.setSetVoltage(ssetVoltage);
                    rcb.setSetCurrent(ssetCurrent);
                    rcb.setSoftStartTime(ssst);
                    i++;
                }
            }

            MessageBox.Show("设置完成！");
        }

        /// <summary>
        /// 关闭窗口释放资源
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            disposeThreadAndRCB();//释放串口、线程资源
            base.OnClosed(e);
        }
    }
}
