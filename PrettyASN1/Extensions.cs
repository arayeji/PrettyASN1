using System.Collections;
using System.Reflection;
using System.Text;

namespace PrettyASN1
{

    public static class Extensions
    {
        public static class ReflectiveEnumerator
        {
            static ReflectiveEnumerator() { }

            public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class, IComparable<T>
            {
                List<T> objects = new List<T>();
                foreach (Type type in
                    Assembly.GetAssembly(typeof(T)).GetTypes()
                    .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
                {
                    objects.Add((T)Activator.CreateInstance(type, constructorArgs));
                }
                objects.Sort();
                return objects;
            }
        }

        public static List<ASNAttribute> GetASNData(this object source, string current)
        {

            MemberInfo[] propertyInfo = source.GetType().GetMember(current.Replace("set_", "").Replace("get_", ""));
            object[] attribute = propertyInfo[0].GetCustomAttributes(typeof(ASNAttribute), true);
            if (attribute.Length > 0)
                return attribute.ToList().Cast<ASNAttribute>().ToList();
            return new List<ASNAttribute>();
        }

        public static ASNAttribute GetSingleASNData(this object source, string current)
        {

            MemberInfo[] propertyInfo = source.GetType().GetMember(current.Replace("set_", "").Replace("get_", ""));
            object[] attribute = propertyInfo[0].GetCustomAttributes(typeof(ASNAttribute), true);
            if (attribute.Length > 0)
                return (ASNAttribute)attribute[0];
            return null;
        }

        public static ASNAttribute GetASNData(this object source, Type type)
        {
            object[] attribute = type.GetCustomAttributes(typeof(ASNAttribute), true);
            if (attribute.Length > 0)
                return (ASNAttribute)attribute[0];
            return null;
        }

        public static ASNAttribute GetASNData(this object source)
        {

            object[] attribute = source.GetType().GetCustomAttributes(typeof(ASNAttribute), true);
            if (attribute.Length > 0)
                return (ASNAttribute)attribute[0];
            return null;
        }
        public static byte[] ToByte(this int X)
        {
            byte[] bt = BitConverter.GetBytes(Convert.ToInt32(X));
            int len = 0;
            int x = 0;
            while (x < bt.Length - 1 && bt[bt.Length - x - 1] == 0x00)
            {
                len++;
                x++;
            }

            byte[] btout = new byte[bt.Length - len];
            for (int y = 0; y < btout.Length; y++)
            {
                btout[btout.Length - y - 1] = bt[y];
            }

            return btout;
        }

        public static byte[] ToByte(this uint X)
        {
            byte[] bt = BitConverter.GetBytes(X);
            Array.Reverse(bt);
            return bt;
        }
        public static string ByteArrayToString(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public static T ToInt<T>(this byte[] bt)
        {
            if (bt.Length > 5)
            {
                return (T)Convert.ChangeType(0, typeof(T));
            }
            else
            if (bt.Length == 5)
            {
                return (T)Convert.ChangeType((((bt[0] & 0xff) << 32) | ((bt[1] & 0xff) << 24) | ((bt[2] & 0xff) << 16) | ((bt[3] & 0xff) << 8) | (bt[4] & 0xff)), typeof(T));
            }
            else
            if (bt.Length == 4)
            {
                return (T)Convert.ChangeType((((bt[0] & 0xff) << 24) | ((bt[1] & 0xff) << 16) | ((bt[2] & 0xff) << 8) | (bt[3] & 0xff)), typeof(T));
            }
            else if (bt.Length == 3)
            {
                return (T)Convert.ChangeType((((bt[0] & 0xff) << 16) | ((bt[1] & 0xff) << 8) | (bt[2] & 0xff)), typeof(T));
            }
            else if (bt.Length == 2)
            {
                return (T)Convert.ChangeType((((bt[0] & 0xff) << 8) | (bt[1] & 0xff)), typeof(T));
            }
            else
            {
                return (T)Convert.ChangeType((bt[0] & 0xff), typeof(T));
            }
        }
        public static long ToInt(this byte[] bt)
        {
            if (bt.Length > 5)
            {
                return 0;
            }
            else
            if (bt.Length == 5)
            {
                return (long)(((bt[0] & 0xff) << 32) | ((bt[1] & 0xff) << 24) | ((bt[2] & 0xff) << 16) | ((bt[3] & 0xff) << 8) | (bt[4] & 0xff));
            }
            else
            if (bt.Length == 4)
            {
                return (long)(((bt[0] & 0xff) << 24) | ((bt[1] & 0xff) << 16) | ((bt[2] & 0xff) << 8) | (bt[3] & 0xff));
            }
            else if (bt.Length == 3)
            {
                return (long)(((bt[0] & 0xff) << 16) | ((bt[1] & 0xff) << 8) | (bt[2] & 0xff));
            }
            else if (bt.Length == 2)
            {
                return (long)(((bt[0] & 0xff) << 8) | (bt[1] & 0xff));
            }
            else
            {
                return (long)(bt[0] & 0xff);
            }
        }
        public static byte[] MirrorBytes(this byte[] bytes)
        {
            string digit = (bytes).ByteArrayToString();
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < digit.Length; x += 2)
            {
                sb.Append(digit[x + 1].ToString() + digit[x].ToString());
            }
            return ToByteArray(sb.ToString());
        }
        public static int ToInt(this BitArray bitArray)
        {

            if (bitArray.Length > 32)
                throw new ArgumentException("Argument length shall be at most 32 bits.");

            int[] array = new int[1];
            bitArray.CopyTo(array, 0);
            return array[0];

        }
        public static byte[] ToByteArray(this String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

    }
}
