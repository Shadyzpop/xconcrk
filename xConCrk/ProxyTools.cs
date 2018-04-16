using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xConCrk
{
    public static class ProxyTools
    {
        public static GetType GT = GetType.NONE;
        public static GetMethod GM = GetMethod.Loop;
        public static int numprx = 0;

        public static int GetProxy(int d)
        {
            if (numprx >= d)
                numprx = 0;
            ++numprx;
            return numprx - 1;
        }

        public enum GetType
        {
            NONE,
            HTTP,
            SOCKS,
        }
        public enum GetMethod
        {
            Loop,
            Delete
        }
    }
}
