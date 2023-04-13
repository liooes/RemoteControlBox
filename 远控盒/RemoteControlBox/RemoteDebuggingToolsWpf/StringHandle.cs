using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteDebuggingToolsWpf
{
    public static class StringHandle
    {
        /// <summary>
        /// 十六进制转字符串
        /// </summary>
        /// <param name="pByte"></param>
        /// <returns></returns>
        public static string getStringFromBytes(byte[] pByte)
        {
            string text = "";
            for (int i = 0; i < pByte.Length; i++)
            {
                text = text + pByte[i].ToString("X").PadLeft(2, '0');// + " "
            }
            return text.TrimEnd(new char[]
            {
                ' '
            });
        }

        /// <summary>
        /// 字符串转十六进制
        /// </summary>
        /// <param name="pString"></param>
        /// <returns></returns>
        public static byte[] getBytesFromString(string pString)
        {
            string[] array = pString.Split(new char[]
            {
                ' '
            });
            byte[] array2 = new byte[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array2[i] = Convert.ToByte(Convert.ToInt32(array[i], 16));
            }
            return array2;
        }

        /// <summary>
        /// 十六进制转十进制
        /// </summary>
        /// <param name="str16"></param>
        /// <returns></returns>
        public static string format16To10(string str16)
        {
            return Convert.ToInt32(str16, 16).ToString();
        }

        /// <summary>
        /// 十进制转十六进制
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string format10To16(byte[] b)
        {
            string str16 = "";
            for (int i = 0; i < b.Length; i++)
                str16 += Convert.ToString(b[i], 16);

            return str16;
        }

        /// <summary>
        /// 十六进制字符串转十进制
        /// </summary>
        /// <param name="shex"></param>
        /// <returns></returns>
        public static int formatHexStringTo10(string shex)
        {
         return Convert.ToInt32(shex, 16);
        }

        /// <summary>
        /// 十进制转十六进制
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static string formar10To16Hex(int i)
        {
            return Convert.ToString(i, 16);
        }

        /// <summary>
        /// 字符串转十六进制byte数据类型返回
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static byte formatStringTobyte(string s)
        {
            return Convert.ToByte(Convert.ToInt32(s, 16));
        }

        /// <summary>
        /// 命令连接
        /// </summary>
        /// <returns>拼接后的命令</returns>
        public static byte[] formatCMD(byte bBoxAddress, byte[] bCMD)
        {
            int ilen = bCMD.Length;
            byte[] bNewCMD = new byte[ilen + 1];
            bNewCMD[0] = bBoxAddress;//第0位是硬件地址
            for (int i = 1; i <= ilen; i++)//将数据拼接到新命令里
                bNewCMD[i] = bCMD[i - 1];

            //将生成的CRC拼接到最后两位
            byte[] crc = CRC.CRC16(bNewCMD);
            byte[] bAddCmdCRC = new byte[bNewCMD.Length + crc.Length];
            for (int i = 0; i < bNewCMD.Length; i++)
                bAddCmdCRC[i] = bNewCMD[i];

            bAddCmdCRC[bAddCmdCRC.Length - 2] = crc[1];
            bAddCmdCRC[bAddCmdCRC.Length - 1] = crc[0];

            return bAddCmdCRC;
        }

        /// <summary>
        /// 数组比较是否相等
        /// </summary>
        /// <param name="bt1">数组1</param>
        /// <param name="bt2">数组2</param>
        /// <returns>true:相等，false:不相等</returns>
        public static bool CheckArrayEqual(byte[] bt1, byte[] bt2)
        {
            var len1 = bt1.Length;
            var len2 = bt2.Length;
            if (len1 != len2)
            {
                return false;
            }
            for (var i = 0; i < len1; i++)
            {
                if (bt1[i] != bt2[i])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 从十进制转换到十六进制
        /// </summary>
        /// <param name="ten"></param>
        /// <returns></returns>
        public static string Ten2Hex(string ten)
        {
            ulong tenValue = Convert.ToUInt64(ten);
            ulong divValue, resValue;
            string hex = "";
            do
            {
                //divValue = (ulong)Math.Floor(tenValue / 16);

                divValue = (ulong)Math.Floor((decimal)(tenValue / 16));

                resValue = tenValue % 16;
                hex = tenValue2Char(resValue) + hex;
                tenValue = divValue;
            }
            while (tenValue >= 16);
            if (tenValue != 0)
                hex = tenValue2Char(tenValue) + hex;
            return hex;
        }

        private static string tenValue2Char(ulong ten)
        {
            switch (ten)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    return ten.ToString();
                case 10:
                    return "A";
                case 11:
                    return "B";
                case 12:
                    return "C";
                case 13:
                    return "D";
                case 14:
                    return "E";
                case 15:
                    return "F";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 格式化数据为长度为4、十六进制
        /// </summary>
        /// <returns></returns>
        public static byte[] formatLenFourAndHex(string s)
        {
            string bVoltage = StringHandle.Ten2Hex(s);//十进制转十六进制字符串
            string sNewData = "";
            byte[] bNewData = null;//转换后的值（十六进制）
            //长度不足4补0
            int ivlen = bVoltage.Length;
            if (bVoltage.Length < 4)
            {
                for (int i = 0; i < 4 - ivlen; i++)
                    sNewData = sNewData + "0";
            }
            //长度为4、十六进制的字符串
            sNewData = sNewData + bVoltage;
            if (sNewData.Length == 4)
            {
                return bNewData = StringHandle.strToToHexByte(sNewData);//字符串十六进制转byte[]类型十六进制
            }
            return bNewData;
        }

        /// <summary>
        /// 数组合并
        /// </summary>
        /// <param name="a">固定位</param>
        /// <param name="b">数据位</param>
        /// <returns></returns>
        private static byte[] copybyte(byte[] a, byte[] b)
        {
            byte[] c = new byte[a.Length + b.Length];
            a.CopyTo(c, 0);
            b.CopyTo(c, a.Length);
            return c;
        }

        /// <summary>
        /// 将固定位和数据位合并
        /// </summary>
        /// <param name="sfixed">固定位</param>
        /// <param name="bData">数据位</param>
        /// <returns>合并后的数据</returns>
        public static byte[] mergeFixedData(byte[] sfixed, string bData)
        {
            byte[] bfixed = sfixed;//固定位
            //将十进制转十六进制（命令需要十六进制）
            byte[] bdata = StringHandle.formatLenFourAndHex(bData);//数据位 

            return copybyte(bfixed, bdata);
        }
    }
}
