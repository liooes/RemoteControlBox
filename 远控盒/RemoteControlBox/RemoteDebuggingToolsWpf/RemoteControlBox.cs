using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDebuggingToolsWpf
{
    public class RemoteControlBox
    {
        BoxCMD bcmd = new BoxCMD();
        SerialPortHelper sph;
        FileOpertion fo = new FileOpertion();
        public bool isBusy = false;
        /// <summary>
        /// 额定电压
        /// </summary>
        public string sRatedVoltage { get; set; }

        /// <summary>
        /// 额定电流
        /// </summary>
        public string sRatedCurrent { get; set; }

        /// <summary>
        /// 当前电压
        /// </summary>
        public string sCurrentVoltage { get; set; }

        /// <summary>
        /// 当前电流
        /// </summary>
        public string sCurrentCurrent { get; set; }

        /// <summary>
        /// 设备地址
        /// </summary>
        public string sBoxAddress { get; set; }

        /// <summary>
        /// 设置电压
        /// </summary>
        public string sSettingVoltage { get; set; }

        /// <summary>
        /// 设置电流
        /// </summary>
        public string sSettingCurrent { get; set; }

        /// <summary>
        /// 软启动时间
        /// </summary>
        public string sSoftStartTime { get; set; }

        /// <summary>
        /// 启动停止状态
        /// </summary>
        public bool sStartStopState { get; set; }

        /// <summary>
        /// 稳压稳流状态  true稳压  false稳流
        /// </summary>
        public bool sRSFState { get; set; }

        /// <summary>
        /// 通讯状态
        /// </summary>
        public bool sCommState { get; set; }

        /// <summary>
        /// 初始化串口参数
        /// </summary>
        /// <param name="portName">串口号</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="DataBits">数据位</param>
        /// <param name="parity">校验位 0奇 1偶 2无</param>
        /// <param name="stopBits">停止位 0=停止位1  1=停止位1.5    2=停止位2</param>
        public RemoteControlBox(string portName, string baudRate, string dataBits, int parity, int stopBits)
        { 
            sph = new SerialPortHelper(portName, baudRate, dataBits, parity, stopBits); 
        }

        /// <summary>
        /// 获取额定电压、电流
        /// </summary>
        public void getRatedVoltageCurrent()
        {
            FileOpertion fo = new FileOpertion();
            this.sRatedVoltage = fo.getUserSetRatedVoltage();
            this.sRatedCurrent = fo.getUserSetRatedCurrent();
        }

        /// <summary>
        /// 读取当前电压电流
        /// </summary>
        public void getCurrentVoltageCurrent()
        {
            isBusy = true;
            try
            {
                byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bcmd.bCurrentVoltageCurrent);
                string sSendState = sph.sp_sendData(cmd);//发送读取当前电压电流命令
                sph.sp_DataReceive();//读取应答

                //将两位电压值转换为十进制、公式运算得出当前电压值
                byte[] brev = sph.strspRevData;
                if (brev.Length == 9)
                {
                    byte[] bv = new byte[2];//电压值、十六进制
                    bv[0] = brev[3];
                    bv[1] = brev[4];
                    //电压数据位拼接、转十进制
                    string shexVoltage = StringHandle.getStringFromBytes(bv); 
                    int iv = StringHandle.formatHexStringTo10(shexVoltage);//十进制当前电压值

                    //用户设定的额定电压
                    float fUserVoltage = float.Parse(fo.getUserSetRatedVoltage());
                    //当前电压
                    string sCVoltage = ((fUserVoltage * iv) / 4096).ToString("f2");
                    //设置当前电压值
                    this.sCurrentVoltage = sCVoltage;


                    //当前电流
                    byte[] bc = new byte[2];//电流值、十六进制
                    bc[0] = brev[5];
                    bc[1] = brev[6];
                    //电流数据位拼接、转十进制
                    string shexCurrent = StringHandle.getStringFromBytes(bc);
                    int ic = StringHandle.formatHexStringTo10(shexCurrent);//十进制当前电流值 

                    //用户设定的额定电流
                    float fUserCurrent = float.Parse(fo.getUserSetRatedCurrent());
                    //当前电流
                    string sCurrent = ((fUserCurrent * ic) / 4096).ToString("f2");
                    //设置当前电流值
                    this.sCurrentCurrent = sCurrent;
                }

               
            }
            catch { }
            isBusy = false;
        }

        /// <summary>
        /// 获取远控盒运行状态
        /// </summary>
        public void getBoxState()
        {
            isBusy = true;
            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bcmd.bReadRunState);

            string sSendState = sph.sp_sendData(cmd);//发送获取远控盒运行状态命令
            sph.sp_DataReceive();//读取应答 
            byte[] brev = sph.strspRevData;

            string sState = brev[3].ToString();   //第三位 设备运行状态 0运行    1停止
            int iState = int.Parse(sState);//十进制当前运行状态值
            if (iState == 0)
                this.sStartStopState = true;
            else
                this.sStartStopState = false;

            isBusy = false;
        }

        /// <summary>
        /// 切换稳压
        /// </summary>
        public void switchRegulator()
        {
            isBusy = true;
            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bcmd.bSwitchRegulator);
            string sSendState = sph.sp_sendData(cmd);//发送切换稳压命令
            sph.sp_DataReceive();//读取应答 
            byte[] brev = sph.strspRevData;

            bool bIsEqual = StringHandle.CheckArrayEqual(cmd, brev);//判断发送与接收是否相等

            if (bIsEqual)
            {
                this.sRSFState = true;//切换成功后设置状态
            }
            isBusy = false;
        }

        /// <summary>
        /// 切换稳流
        /// </summary>
        public void switchSteadyFlow()
        {
            isBusy = true;
            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bcmd.bSwitchSteadyFlow);
            string sSendState = sph.sp_sendData(cmd);//发送切换稳压命令
            sph.sp_DataReceive();//读取应答 
            byte[] brev = sph.strspRevData;

            bool bIsEqual = StringHandle.CheckArrayEqual(cmd, brev);//判断发送与接收是否相等
            if (bIsEqual)
            {
                this.sRSFState = false;//切换成功后设置状态
            }
            isBusy = false;
        }

        /// <summary>
        /// 启动远控盒
        /// </summary>
        public void startBox()
        {
            isBusy = true;
            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bcmd.bStartBox);
            string sSendState = sph.sp_sendData(cmd);//发送启动远控盒命令
            sph.sp_DataReceive();//读取应答 
            byte[] brev = sph.strspRevData;

            bool bIsEqual = StringHandle.CheckArrayEqual(cmd, brev);//判断发送与接收是否相等
            if (bIsEqual)
            {
                this.sStartStopState = true;//切换成功后设置状态
            }
            isBusy = false;
        }

        /// <summary>
        /// 停止远控盒
        /// </summary>
        public void stopBox()
        {
            isBusy = true;
            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bcmd.bStopBox);
            string sSendState = sph.sp_sendData(cmd);//发送停止远控盒命令
            sph.sp_DataReceive();//读取应答 
            byte[] brev = sph.strspRevData;

            bool bIsEqual = StringHandle.CheckArrayEqual(cmd, brev);//判断发送与接收是否相等
            if (bIsEqual)
            {
                this.sStartStopState = false;//切换成功后设置状态
            }
            isBusy = false;
        }

        /// <summary>
        /// 设置远控盒硬件地址
        /// </summary>
        /// <param name="sAddress"></param>
        public void setBoxAddress(string sAddress)
        {
            if (sAddress != null)
                this.sBoxAddress = sAddress;
        }

        /// <summary>
        /// 获取通讯状态
        /// </summary>
        public void getBoxCommState()
        {
            isBusy = true;

            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bcmd.bReadRunState);
            sph.strspRevData[0] = 255;
            string sSendState = sph.sp_sendData(cmd);//发送获取远控盒运行状态命令
            sph.sp_DataReceive();//读取应答 
            byte[] brev = sph.strspRevData;

            if (brev != null && brev[0] == cmd[0])//并且CRC校验成功
                this.sCommState = true;
            else
                this.sCommState = false;
            
            isBusy = false;
        }

        /// <summary>
        /// 写入设定电压
        /// </summary>
        /// <param name="sVoltage">设定电压</param>
        public void setSetVoltage(string sVoltage)
        {
            isBusy = true;
            //写入的数据需要公式转换
            int i = Convert.ToInt32((4096 * float.Parse(sVoltage)) / float.Parse(fo.getUserSetRatedVoltage()));
        
            //固定位+数据位
           byte[] bfd = StringHandle.mergeFixedData(bcmd.bWriteSetVoltage, i.ToString());
            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bfd);

            string sSendState = sph.sp_sendData(cmd);//发送写入设定电压命令
            sph.sp_DataReceive();//读取应答
            byte[] brev = sph.strspRevData;//读取到的数据
            bool bIsEqual = StringHandle.CheckArrayEqual(cmd, brev);//判断发送与接收是否相等
             
            isBusy = false;
        }

        /// <summary>
        /// 写入设定电流
        /// </summary>
        /// <param name="sCurrent">设定电流</param>
        public void setSetCurrent(string sCurrent)
        {
            isBusy = true; 
            //写入的数据需要公式转换
            int i = Convert.ToInt32((4096 * float.Parse(sCurrent)) / float.Parse(fo.getUserSetRatedCurrent())); 

            //固定位+数据位
            byte[] bfd = StringHandle.mergeFixedData(bcmd.bWriteSetCurrent, i.ToString());
            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bfd);

            string sSendState = sph.sp_sendData(cmd);//发送写入设定电压命令
            sph.sp_DataReceive();//读取应答
            byte[] brev = sph.strspRevData;//读取到的数据
            bool bIsEqual = StringHandle.CheckArrayEqual(cmd, brev);//判断发送与接收是否相等
             
            isBusy = false;
        }

        /// <summary>
        /// 写入软启动时间
        /// </summary>
        /// <param name="sSST">软启动时间</param>
        public void setSoftStartTime(string sSST)
        {
            isBusy = true;
            //固定位+数据位
            byte[] bfd = StringHandle.mergeFixedData(bcmd.bWriteSoftStartTime, sSST);
            byte[] cmd = StringHandle.formatCMD(StringHandle.formatStringTobyte(this.sBoxAddress), bfd);

            string sSendState = sph.sp_sendData(cmd);//发送写入设定电压命令
            sph.sp_DataReceive();//读取应答
            byte[] brev = sph.strspRevData;//读取到的数据
            bool bIsEqual = StringHandle.CheckArrayEqual(cmd, brev);//判断发送与接收是否相等

            isBusy = false;
        }
    }
}
