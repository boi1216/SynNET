using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseMiNET.utils
{
    class Utils
    {

        public static byte[] ToBigEndian(int value)
        {
            byte[] retval = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(retval);
            }
            return retval;
        }

        public static byte[] cleanArray(byte[] array)
        {
            List<byte> bytes = new List<byte>();

            foreach(byte buff in array)
            {
                if(buff != 0)
                {
                    bytes.Add(buff);
                }
            }
            return bytes.ToArray();
        }
        
    }
}
