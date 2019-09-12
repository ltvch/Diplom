using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFApp1
{
    static class Example
    {
        public static double AlphaR(int ep, int t1, int t2)
        {
            double Alphar;
            double C0 = 5.67;
            double Qr;

            if ((t1 - t2) < Math.E - 2) //не понятная 1е
            {
                Alphar = 0;
            }
            else
            {
                Qr = C0 * ep * (Math.Sqrt(Math.Sqrt(0.01 * (t1 + 273))) - Math.Sqrt(Math.Sqrt(0.01 * (t2 + 273)))); //к чему этот расчет?
                Alphar = Qr / (t1 - t2);
            }
            return Alphar;
        }
    }
}
