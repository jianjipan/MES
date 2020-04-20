using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMIS
{
    public class DataTransform
    {
        private static object lock0 = new object();
        private static object lock1 = new object();
        private static object lock2 = new object();
        private static object lock3 = new object();
        private static object lock4 = new object();

        public static byte[] floatToByte(float value)
        {
            lock (lock0)
            {
                byte[] cmd = new byte[4];
                cmd=BitConverter.GetBytes(value);
                Array.Reverse(cmd, 0, 4);
                return cmd;
            }
        }
        
        /// <summary>
        /// int转byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] intToByte(int value)
        {
            lock (lock1)
            {
                byte[] cmd = new byte[4];
                cmd[3] = (byte)((value >> 24) & 0xFF);
                cmd[2] = (byte)((value >> 16) & 0xFF);
                cmd[1] = (byte)((value >> 8) & 0xFF);
                cmd[0] = (byte)(value & 0xFF);
                return cmd;
            }
        }
        ///byte 转int
        public static int bytesToInt(byte[] cmd, int offset)
        {
            lock (lock2)
            {
                int value;
                value = (int)((cmd[offset] & 0xFF)
                    | ((cmd[offset + 1] & 0xFF) << 8)
                    | ((cmd[offset + 2] & 0xFF) << 16)
                    | ((cmd[offset + 3] & 0xFF) << 24));
                return value;
            }
        }

        ///short转byte
        public static byte[] shortToByte(ushort value)
        {
            lock (lock3)
            {
                byte[] cmd = new byte[2];
                cmd[1] = (byte)((value & 0xFF00) >> 8);
                cmd[0] = (byte)(value & 0xFF);
                return cmd;
            }
        }

        ///byte 转 short
        public static ushort bytesToUshort(byte[] cmd, int offset)
        {
            lock (lock4)
            {
                ushort value;
                value = (ushort)((cmd[offset] & 0xFF)
                    | ((cmd[offset + 1] & 0xFF) << 8));
                return value;
            }
        }
    }
}
