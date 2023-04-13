using SerialPortHelpers;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            RemoteControlBox rcb = new RemoteControlBox("COM4", 9600, 8, Parity.None, StopBits.Two);
            rcb.sBoxAddress = "01";
            rcb.getCurrentVoltageCurrent();
            Console.WriteLine("额定电压：" + rcb.sCurrentVoltage);
            Console.WriteLine("额定电流：" + rcb.sCurrentCurrent);

            //rcb.startBox();
            rcb.stopBox();
            //获取远控盒运行状态
            rcb.getBoxState();
            Console.WriteLine("远控盒运行状态：" + rcb.sStartStopState);

            //切换稳压
            rcb.switchRegulator();
            //切换稳流
            //rcb.switchSteadyFlow();
            Console.WriteLine("远控盒稳压稳流状态：" + rcb.sRSFState);

            //获取通讯状态
            rcb.getBoxCommState();
            Console.WriteLine("远控盒通讯状态：" + rcb.sCommState);

            //rcb.setSetVoltage("12");
            //rcb.setSetCurrent("1000");
            //rcb.setSoftStartTime("30");

            Console.ReadKey();
         }

      
    }
}
