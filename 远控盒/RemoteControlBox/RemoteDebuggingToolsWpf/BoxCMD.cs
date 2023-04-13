using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDebuggingToolsWpf
{
    public class BoxCMD
    {
        /// <summary>
        /// 读取运行状态
        /// </summary>
        public byte[] bReadRunState = { 0x02, 0x00, 0x00, 0x00, 0x01 };

        /// <summary>
        ///读取当前电压电流
        /// </summary>
        public byte[] bCurrentVoltageCurrent = { 0x04, 0x00, 0x00, 0x00, 0x02 };

        /// <summary>
        ///停止远控盒
        /// </summary>
        public byte[] bStopBox = { 0x05, 0x00, 0x00, 0xFF, 0x00 };

        /// <summary>
        ///启动远控盒
        /// </summary>
        public byte[] bStartBox = { 0x05, 0x00, 0x00, 0x00, 0xFF };

        /// <summary>
        ///写入设定电压
        /// </summary>
        public byte[] bWriteSetVoltage = { 0x06, 0x00, 0x00 };

        /// <summary>
        ///写入设定电流
        /// </summary>
        public byte[] bWriteSetCurrent = { 0x06, 0x00, 0x01 };

        /// <summary>
        ///写入软启动时间
        /// </summary>
        public byte[] bWriteSoftStartTime = { 0x06, 0x00, 0x02 };

        /// <summary>
        ///切换稳压
        /// </summary>
        public byte[] bSwitchRegulator = { 0x05, 0x00, 0x01, 0xFF, 0x00 };

        /// <summary>
        ///切换稳流
        /// </summary>
        public byte[] bSwitchSteadyFlow = { 0x05, 0x00, 0x01, 0x00, 0xFF };
    }
}
