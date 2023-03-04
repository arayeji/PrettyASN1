using System;
using System.Text;

namespace PrettyASN1
{
    static class Utils
    {

        public static byte[] IntToByte(uint X, int Len)
        {

            byte[] len3 = new byte[Len];
            if (Len == 1)
                len3[0] = Convert.ToByte(X & 0x0ff);
            else
                if (Len == 2)

            {
                len3[0] = Convert.ToByte(X >> 8 & 0x0ff);
                len3[1] = Convert.ToByte(X & 0x0ff);
            }
            else
                if (Len == 3)

            {
                len3[0] = Convert.ToByte(X >> 16 & 0x0ff);
                len3[1] = Convert.ToByte(X >> 8 & 0x0ff);
                len3[2] = Convert.ToByte(X & 0x0ff);
            }
            else if (Len == 4)
            {
                len3[0] = Convert.ToByte(X >> 24 & 0x0ff);
                len3[1] = Convert.ToByte(X >> 16 & 0x0ff);
                len3[2] = Convert.ToByte(X >> 8 & 0x0ff);
                len3[3] = Convert.ToByte(X & 0x0ff);
            }

            return len3;
        }
        public static byte[] IntToByte(int X, int Len)
        {


            byte[] len3 = new byte[Len];
            if (Len == 1)
                len3[0] = Convert.ToByte(X & 0x0ff);
            else
                if (Len == 2)

            {
                len3[0] = Convert.ToByte(X >> 8 & 0x0ff);
                len3[1] = Convert.ToByte(X & 0x0ff);
            }
            else
                if (Len == 3)

            {
                len3[0] = Convert.ToByte(X >> 16 & 0x0ff);
                len3[1] = Convert.ToByte(X >> 8 & 0x0ff);
                len3[2] = Convert.ToByte(X & 0x0ff);
            }
            else if (Len == 4)

            {
                len3[0] = Convert.ToByte(X >> 24 & 0x0ff);
                len3[1] = Convert.ToByte(X >> 16 & 0x0ff);
                len3[2] = Convert.ToByte(X >> 8 & 0x0ff);
                len3[3] = Convert.ToByte(X & 0x0ff);
            }

            return len3;
        }
        public static string ByteArrayToString(byte[] ba, uint count)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            for (int x = 0; x < count; x++)
                hex.AppendFormat("{0:x2}", ba[x]);
            return hex.ToString();
        }
    }
}
