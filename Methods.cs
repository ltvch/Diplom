using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CApp1
{
    public static class Methods
    {
        /// <summary>
        /// Метод "прогонки"
        /// </summary>
        /// <param name="PBeg"></param>
        /// <param name="Pend"></param>
        /// <param name="Ap"></param>
        /// <param name="Bp"></param>
        /// <param name="Cp"></param>
        /// <param name="Fp"></param>
        /// <param name="alfap"></param>
        /// <param name="betap"></param>
        public static void Progonka(int PBeg, int Pend, ref double[] Ap, ref double[] Bp, ref double[] Cp, ref double[] Fp, ref double[] alfap, ref double[] betap)
        {
            int[] DTP = new int[399];
            double zn;

            alfap[PBeg] = Bp[PBeg] / Cp[PBeg];
            betap[PBeg] = Fp[PBeg] / Cp[PBeg];

            for (int ip = PBeg; ip < Pend; ip++)
            {
                zn = Cp[ip] = alfap[ip] * Ap[ip];
                alfap[ip + 1] = Bp[ip] / zn;
                betap[ip + 1] = (Ap[ip] * betap[ip] + Fp[ip]) / zn;

                Debug.WriteLine("alfar is {0}", alfap[ip + 1]);
                Debug.WriteLine("betap is {0}", betap);
                Debug.WriteLine("zn in {0}", zn);
            }
            //Console.WriteLine(zn);

        }

        /// <summary>
        /// физ св-ва трубной стенки
        /// </summary>
        public static void HeatFizLance( int k, int m, int n, ref  double[,] tt, ref double[,] lamst, ref double[,] cest)
        {
            for (int ip = k; k < m; k++)
            {
                for (int jp = 0; jp < n; jp++)
                {
                    lamst[ip, jp] = 50.6 - 3.0 * (tt[ip, jp] - 100.0) / 100.0;
                    cest[ip, jp] = 469.0 + 1.01 * (tt[ip, jp] - 100.0) / 4.0;

                    //Console.WriteLine("l = " + lamst + "\nc = " + cest);
                    Debug.WriteLine("l = " + lamst[ip,jp] + "\nc = " + cest[ip,jp]);
                }
                Debug.WriteLine("lampst[ip, jp] is {0}", lamst[ip, 30]);
            }

            Debug.WriteLine("lampst[i, j] is {0}", lamst[20, 20]);
        }

        /// <summary>
        ///физические свойства СО 
        /// </summary>
        public static void CO(double tt, ref double lamCO, ref double njuCO, ref double PrCO)
        {
            lamCO = 0.0249 + tt * 5.681e-5;
            njuCO = 1.55E-6 + tt * 1.677E-7;
            PrCO = 0.72;
            //Console.WriteLine("lamCO = " + lamCO + "\nnjuCO = " + njuCO + "\nPrCO = " + PrCO);
        }

        /// <summary>
        /// физ св-ва воды
        /// </summary>
        public static void H2O(double t1, double t2, double nju, double Pr, double Prst, double lam)
        {
            nju = 1.306 * Math.E - 6 + (t1 - 10) + 1E-6 * (0.295 - 1.306) / 90;
            Pr = 9.52 + (t1 - 10) * (1.75 - 9.52) / 90;
            if (t2 <= 100)
            {
                Prst = 9.52 + (t2 - 10) * (1.75 - 9.52) / 90;
            }
            else if (t2 > 100 && t2 <= 250)
            {
                Prst = 1.75 + (t2 - 100) * (0.86 - 1.75) / 150;
            }
            else if (t2 > 250)
            {
                Prst = 0.86 + (t2 - 250) * (6.79 - 0.86) / 120;
            }
            lam = 0.574 + (t1 - 10) * (0.683 - 0.574);

            Console.WriteLine("nju = " + nju + "\nPr = " + Pr + "\nlam = " + lam);
        }

        /// <summary>
        /// физ св-ва воздуха 
        /// </summary>
        public static void Vozduh(double tt, double lamv, double njuv, double Prv)
        {
            lamv = (tt * 7.83E2 - 2 - Math.Sqrt(tt) * 3.025E-5 + 24.144) / 1.0E3;
            njuv = (Math.Sqrt(tt) * 7.28E-5 + tt * 9.17E-2 + 12.762) / 1.0E6;
            Prv = 0.7;

            Console.WriteLine("Lamv = " + lamv + "\nnjuv = " + njuv + "\nPrv = " + Prv);
        }

        /// <summary>
        /// коэф свободной конвекции
        /// </summary>
        public static double Alfa_KS(double lamsr, double nju, double Prsr, double tsr, double ll, double tt)
        {
            double bet, Gr, Nu;
            double Alfa_ks;

            Nu = 0; //условное значение

            if ((tt - tsr) < 1 * Math.E - 6)
            {
                Alfa_ks = 0;
                //Application.Exit; // выходим не считаем
            }

            bet = 1 / (0.5 * (tt + tsr) + 273.0);
            Gr = 9.81 * ll * ll * ll * bet * (tt - tsr / Math.Sqrt(nju));

            if ((Gr * Prsr) < 1 * Math.E - 3)
            {
                Nu = 0.45;
            }
            if (((Gr * Prsr) < 1 * Math.E - 3) && ((Gr * Prsr) < 5E2))
            {
                Nu = 1.18 * Math.Exp(0.25 * Math.Log(Gr * Prsr));
            }
            if ((Gr * Prsr) >= 2 * Math.E * 7)
            {
                Nu = 0.54 * Math.Exp(1.0 / 3.0 * Math.Log(Gr * Prsr));
            }

            Alfa_ks = Nu * lamsr / ll;

            return Alfa_ks;
        }

        /// <summary>
        /// /Alfar
        /// </summary>
        /// <param name="ep"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public static double Alfar(double ep, double t1, double t2)
        {
            double C0 = 5.67; //постоянная Стефана - Больцмана
            double Qr;
            double alfar;
            if ((t1 - t2) < 1E-2)
            {
                alfar = 0;
            }

            Qr = C0 * ep * Math.Sqrt(Math.Sqrt(0.01 * (t1 + 273))) - Math.Sqrt(Math.Sqrt(0.01 * (t2 + 273)));
            alfar = Qr / (t1 - t2);
            return alfar;
        }

        /// <summary>
        /// эффективная теплопроводность
        /// </summary>        
        public static double Lamef(double lam1, double lam2)
        {
            double lamef = 2 * lam1 - lam2 / (lam1 + lam2);
            return lamef;
        }

        /// <summary>
        /// Stop signal
        /// </summary>
        /// <param name="iBeg"></param>
        /// <param name="iEnd"></param>
        /// <param name="jBeg"></param>
        /// <param name="jEnd"></param>
        /// <param name="t0"></param>
        /// <param name="tnew"></param>
        /// <returns></returns>
        public static bool Stop_signal(int iBeg, int iEnd, int jBeg, int jEnd, double[,] t0, double[,] tnew)
        {
            int ip, jp;
            bool stop_signal = false;

            for (ip = iBeg; iBeg <= iEnd; iBeg++)
            {
                for (jp = jBeg; jBeg <= jEnd; jBeg++)
                {
                    if ((t0[ip, jp] - tnew[ip, jp]) > 0.0001)
                    {
                        stop_signal = false;
                    }
                    else stop_signal = true;
                }
            }
            return stop_signal;
        }
    }
}
