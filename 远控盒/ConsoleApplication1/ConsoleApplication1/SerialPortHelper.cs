using System;
using System.IO.Ports;
using System.Threading;

namespace ConsoleApplication1
{
    public class SerialPortHelper
    {
        SerialPort sp = new SerialPort();

        /// <summary>
        /// 存储串口接收数据值
        /// </summary>
        public byte[] strspRevData;

        /// <summary>
        /// 串口号
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; }

        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBits { get; set; }

        /// <summary>
        /// 校验位
        /// </summary>
        public Parity parity { get; set; }

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits stopBits { get; set; }
  
        /// <summary>
        /// 设置串口参数
        /// </summary>
        public void SettingSerialPort()
        {
            sp.PortName = PortName;
            sp.BaudRate = BaudRate;
            sp.DataBits = DataBits;
            sp.Parity = parity;
            sp.StopBits = stopBits;
        }

        /// <summary>
        /// 清除接收缓冲区值
        /// </summary>
        public void ClearSPRecv()
        {
            strspRevData = null;
        }
        
    

          /// <summary>
        /// 设置串口参数
        /// </summary>
        /// <param name="portName">串口号</param>
        /// <param name="BaudRate">波特率</param>
        /// <param name="DataBits">数据位</param>
        /// <param name="parity">校验位    0奇 1偶 2无</param>
        /// <param name="stopBits">停止位  0=停止位1  1=停止位1.5    2=停止位2</param>
        public SerialPortHelper(string portName, int BaudRate, int DataBits, Parity parity, StopBits stopBits)
        {
            sp.PortName = portName;
            sp.BaudRate = BaudRate;
            sp.DataBits = DataBits;
            sp.Parity = parity;
            sp.StopBits = stopBits;
            sp.Handshake = Handshake.None;
            //sp.RtsEnable = true;
        }
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <returns>串口打开状态</returns>
        public bool OpenSerialPort()
        {
            bool result;
            try
            {
                if (!sp.IsOpen)
                {
                    sp.Open();
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        /// <returns>串口状态</returns>
        public bool CloseSerialPort()
        {
            bool result;
            try
            {
                if (sp.IsOpen)
                {
                    sp.Close();
                }
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 数据发送
        /// </summary>
        /// <param name="cmd">具体操作命令(有附加位的byte数组)</param>
        /// <returns>发送状态</returns>
        public string sp_sendData(byte[] cmd)
        {
            try
            {
                if (!sp.IsOpen)
                    sp.Open();
                if (sp.IsOpen)
                {
                    sp.Write(cmd, 0, cmd.Length);
                }
                else
                    return "串口未打开";
            }
            catch (Exception ex)
            {
                return "串口未打开" + ex.Message;
            }
            return "数据发送成功";
        }  

        /// <summary>
        /// 数据接收
        /// </summary>
        public string sp_DataReceive()
        {
            try
            {
                lock (this)
                {
                    if (!sp.IsOpen)
                    {
                        sp.Open();
                    }
                    if (sp.IsOpen)
                    {
                        //for (int i = 0; i < 20; i++)//控制等待时间
                        //{
                            Thread.Sleep(50);
                            int bytesToRead = sp.BytesToRead;//获取在缓冲区的字节数
                            if (bytesToRead > 0)
                            {
                                byte[] array = new byte[bytesToRead];
                                sp.Read(array, 0, bytesToRead);
                                //sp.DiscardInBuffer();
                                sp.Close();
                                strspRevData = array;
                                //break;//将接收数据存储至strspRevData
                            }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "数据接收成功";
        }


    }
}
