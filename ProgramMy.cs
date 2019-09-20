using System;
using System.Diagnostics;

namespace CApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            #region --- Обьявляем массивы --- 
            double[] tH2O = new double[109];
            double[] A = new double[109];
            double[] alfa = new double[109];
            double[] B = new double[109];
            double[] beta = new double[109];
            double[] C = new double[109];
            double[] F = new double[109];

            double[,] t = new double[109, 31];
            double[,] t1 = new double[109, 31];
            double[,] t2 = new double[109, 31];
            double[,] ce = new double[109, 31];
            // double[,] ce;
            double[,] lam = new double[109, 31];
            double[,] t3 = new double[109, 31];

            int[] arr = new int[5];
            #endregion

            #region --- константы ---
            /// <summary>
            /// константы
            /// </summary> 
            int wps = 300; //общее время процесса
            int wpr = 150; // время продувки сек

            double dz = 0.5; //расчет. шаг
            double dr = 0.001;

            int ro = 7800; // плотность трубной стенки
            double eps = 0.85; // степень черноты ствола
            int lf = 15; // иследуемая длина фурма м

            double Dgv = 3.2; //диаметр горловины м
            double Dfn = 0.219; //наружный диаметр фурмы м
            double Dfv = 0.203; //внутренний диаметр наружной трубы фурмы м
            double Drt = 0.168; //диаметр разделительной трубы фурмы м

            int w = 1; // интервал печати
            int dt = 1; // расч. шаг

            int tint = 20; //температура воды на входе
            int cH2O = 4178; // теплоемкость воды
            int roH2O = 1000; //плотность воды
            int WH2O = 40; // расход воды

            double q1 = 0.93e6;
            double q2 = 0.16e6;
            #endregion

            #region --- переменные ---
            /// <summary>
            /// переменные
            /// </summary>
            bool fl = true, fl1 = true, sig = true; // условное значение

            //char Key; 

            double sss = 0, sss1 = 0, nnn = 0, nnn1 = 0; //условно присвоил значение

            string StartMode, plav = "0", v = "0", s = "2"; //условное значение v, plav, s; StartMode ???

            double alfaH2O = 5.3, vH2O, Sk, Sint, Sext, ReH2O, NuH2O, mn, alfarz = 1, alfarv, alfakv, ee,  //alfaH2O = 5.3, alfarz = 1 условное значение; vH2O не нашел по коду
                   lamv = 100, njuv = 150, Prv = 200, tv = 30.5, lamCO = 0, njuCo = 0, PrCO = 0, wCO, Nu, Re, njuH2O = 12, PrH2O = 18, PrH2Ost = 20, lamH2O = 22, q = 1;
            //tv = 30,5; q=1, lamv = 100, njuv = 150, Prv = 200, njuH2O = 12, PrH2O = 18, PrH2Ost = 20, lamH2O = 22 условное значение

            byte priz = 0; //условно присвоено значение 0

            int K, M, N, i, j;
            #endregion

            N = Convert.ToInt32(Math.Round(lf / dz)) + 1; // N = 31
            K = Convert.ToInt32(Math.Round(0.5 * Dfv / dr)); // K = 101,5
            M = Convert.ToInt32(Math.Truncate(0.5 * Dfn / dr)); // M = 109,5

            Console.WriteLine("--- Расчет температуры стенки верхней фурмы ---");

            Methods.Progonka(10, 100, ref A, ref B, ref C, ref F, ref alfa, ref beta); //работает но не считает

            Console.WriteLine("Methods.Alfar return {0}", Methods.Alfar(1.2, 5.5, 6.3)); //считает

            //Console.WriteLine(Alfa_KS(2.2, 5.6, 3.9, 5.5, 8.3,6.2)); //считает при определенных значениях

            //Vozduh(1.2,1.3,1.4,1.5);//считает

            //H2O(20,25.5,1.5,2.3,3.3,4.5); //считает

            //Console.WriteLine(Lamef(2.5,2.7)); // считает

            //CO(25,2.5,3.3,6.4);//считает

            //HeatFizLance(t,lam,ce,K,M,N);//работает но не считает

            //Console.WriteLine(Stop_signal(5,25,10,50,t,t1));//работает, не понятно с передачей параметров

            //*****************Температурное поле стенки*****************


            Methods.HeatFizLance(K, M, N, ref t, ref lam, ref ce); //"теплофизика трубы"  (отрабатывает)
            Methods.CO(tint, ref lamCO, ref njuCo, ref PrCO); //расчет CO

#if DEBUG
            Console.WriteLine(ce[108, 30]);
            Console.WriteLine(lam[108, 30]);

            Console.WriteLine(lamCO);
            Console.WriteLine(njuCo);
            Console.WriteLine(PrCO);

#endif
            Console.WriteLine(N + " " + K + " " + M);

            for (j = 1; j < N; j++)
            {
                mn = 2 * dt / ((K + 0.25) * dr * ce[K, j] * ro);
                Debug.WriteLine("первый расчет mn ={0} ", mn);//
                A[K] = 0;
                B[K] = mn * (K + 0.5) * Methods.Lamef(lam[K, j], lam[K + 1, j]) / dr;

                #region --- До этого момента переменные попадающие в параметры заполняются ---
                Debug.WriteLine("A[K] is {0}", A[K]);
                Debug.WriteLine("B[K] is {0}", B[K]);
                
                
                #region --- Проверяем есть ли данные в параметрах Alfar ---
                for (int a = 0; a < j; a++)
                {
                    Debug.WriteLine(t[K, j]);
                    Debug.WriteLine(tH2O[N - j + 1]);
                }
                #endregion
                #endregion


                //todo Разобратся почему t[K, j], tH2O[N - j + 1] пустые
                // Они не могут быть пустые !!!

                alfarz = Methods.Alfar(eps, t[K, j], tH2O[N - j + 1]);


                #region --- Проверяем есть ли данные в параметрах Alfar ---
                for (int a = 0; a < j; a++)
                {
                    Debug.WriteLine(t[K, j]);
                    Debug.WriteLine(tH2O[N - j + 1]);
                }
                #endregion

                C[K] = 1 + B[K] + mn * K * (alfarz + alfaH2O);
                F[K] = t[K, j] + mn * K * tH2O[N - j + 1] * (alfarz + alfaH2O);

                Console.WriteLine(A[K] + " " + B[K] + " " + C[K] + " " + F[K] + " ");

                for (i = K + 1; i < M; i++)
                {
                    double d = ce[i, j];
                    // mn = dt / (i * Math.Sqrt(dr) * ce[i, j] * ro);
                    mn = 0.56;//test value
                    A[i] = mn * Methods.Lamef(lam[i, j], lam[i - 1, j]) * (i - 0.5);
                    B[i] = mn * Methods.Lamef(lam[i, j], lam[i + 1, j]) * (i + 0.5); // индекс за пределами массива
                    C[i] = 1 + A[i] + B[i];
                    F[i] = t[i, j];
                    Console.WriteLine("flag1");
                }

                mn = 2 * dt / ((M - 0.25) * dr * ce[M, j] * ro);
                A[M] = mn * (M - 0.5) * Methods.Lamef(lam[M, j], lam[M - 1, j]) / dr; // индекс за пределами массива
                B[M] = 0;

                alfarv = Methods.Alfar(eps, t[M, j], tv);

                if (Convert.ToInt32(v) < wpr)
                {
                    Methods.CO(tv, ref lamCO, ref njuCo, ref PrCO);
                    ee = 0.86 + 0.8 * Math.Exp(0.4 * Math.Log((Dgv - Dfn) / lf)) * Math.Exp(0.2 * Math.Log(Dfn / Dgv));

                    wCO = 600.0 / 60.0;
                    wCO = wCO * (tv + 273.0) / 273.0 / (0.25 * Math.PI * (Math.Sqrt(Dgv) - Math.Sqrt(Dfn)));
                    Re = wCO * (Dgv - Dfn) / njuCo;
                    Nu = 0.021 * Math.Exp(0.8 * Math.Log(Re)) * Math.Exp(0.43 * Math.Log(PrCO));
                    Nu = ee * Nu * (1.0 - 0.45 / (2.4 + PrCO) * Math.Exp(0.16 /
                        Math.Exp(0.15 * Math.Log(PrCO)) * Math.Log(Dfn / Dgv)));

                    alfakv = Nu * lamCO / (Dgv - Dfn);
                }
                else
                {
                    Methods.Vozduh(0.5 * (t[M, j] + tv), lamv, njuv, Prv);
                    alfakv = Methods.Alfa_KS(lamv, njuv, Prv, tv, lf, t[M, j]);
                }
                if (priz == 0)
                {
                    if (j <= 0.65 * N)
                    {
                        q = q2;
                    }
                    else { q = q1; }
                }
                C[M] = 1 + A[M] + mn * M * priz * (alfarv + alfakv);
                F[M] = t[M, j] + mn * M * (priz * tv * (alfarv + alfakv) + (1 - priz) * q); //
                Methods.Progonka(K, M, ref A, ref B, ref C, ref F, ref alfa, ref beta); //Коэфициенты прогонки

                t[M, j] = (F[M] + beta[M] * A[M]) / (C[M] - A[M] * alfa[M]);

                for (i = M; M < K; K++)
                {
                    t[i, j] = alfa[i + 1] * t[i + 1, j] + beta[i + 1];
                    Console.WriteLine("flag3");
                }
            }

            for (i = K; K < M;) //осевое направление
            {
                mn = 2 * dt * Methods.Lamef(lam[i, 1], lam[i, 2]) / Math.Sqrt(dz) * ce[i, 1] * ro;

                A[1] = 0;
                B[1] = mn;
                C[1] = 1 + B[1];
                F[1] = t[i, 1];

                for (j = 2; j < N;)
                {
                    mn = dt / (Math.Sqrt(dz) * ce[i, j] * ro);

                    A[j] = mn * Methods.Lamef(lam[i, j], lam[i, j - 1]);
                    B[j] = mn * Methods.Lamef(lam[i, j], lam[i, j + 1]);
                    C[j] = 1 + A[j] + B[j];
                    F[j] = t[i, j];
                }
                mn = 2 * dt * Methods.Lamef(lam[i, N], lam[i, N - 1]) / (Math.Sqrt(dz) * ce[i, N] * ro); //7str ce[i,n] ???

                A[N] = mn;
                B[N] = 0;
                C[N] = 1 + A[N];
                F[N] = t[i, N];

                Methods.Progonka(1, N, ref A, ref B, ref C, ref F, ref alfa, ref beta);

                t[i, N] = (F[N] + beta[N] * A[N]) / (C[N] - A[N] * alfa[N]);

                for (j = N - 1; j >= 1; j--)
                {
                    t[i, N] = alfa[j + 1] * t[i, j - 1] + beta[j - 1];
                }





                //sss = 0;
                //nnn = 0;
                //sss1 = 0;
                //nnn1 = 0;

                for (j = 1; j < N;)
                {
                    if (j <= 0.65 * N)
                    {
                        sss = sss + t[K, j];
                        nnn = nnn + t[M, j];
                    }
                    else
                    {
                        sss1 = sss1 + t[K, j];
                        nnn1 = nnn1 + t[M, j];
                    }
                }

                sss = sss / (N - 11);//16
                nnn = nnn / (N - 11);//16
                sss1 = sss1 / 11;//16
                nnn1 = nnn1 / 11;//16

                fl = true;
            }

            for (i = K; K < M;)
            {
                if (fl == false)
                {
                    for (j = 1; j < N;)
                    {
                        if (Math.Abs(t[i, j] - t3[i, j]) > 0.1)
                        {
                            fl = false;
                            //break
                        }
                        else
                        {
                            fl = true;
                        }
                    }
                }
            }
            if (Convert.ToInt32(plav) == 2)
            {
                if (Convert.ToInt32(v) == wpr)
                {
                    fl = false;
                }
                if (fl == true && fl1 == false)
                {
                    fl1 = true;
                }
            }
            t3 = t;

            /// <summary>
            /// расчет температуры воды
            /// </summary>
            Sk = 0.025 * Math.PI * (Math.Sqrt(Dfv) - Math.Sqrt(Drt));
            Sint = Math.PI * Drt * dz;
            Sext = Math.PI * Dfv * dz;

            Methods.H2O(0.5 * (tH2O[1] + tH2O[N]), 0.5 * (t[K, 1] + t[K, N]), njuH2O, PrH2O, PrH2Ost, lamH2O);

            ReH2O = WH2O / 3600.0 / Sk * (Dfv - Drt) / njuH2O;

            NuH2O = 0.017 * Math.Exp(0.8 * Math.Log(ReH2O)) * Math.Exp(0.4 * Math.Log(PrH2O)) *
                Math.Exp(0.25 * Math.Log(PrH2O / PrH2Ost)) * Math.Exp(0.18 * Math.Log(Dfv / Drt));

            alfaH2O = NuH2O * lamH2O / (Dfv - Drt);

            tH2O[1] = tint;

            for (j = 2; j < N;)
            {
                tH2O[j] = tH2O[j - 1] + alfaH2O / (cH2O * roH2O * (WH2O / 3600.0)) *
                    (Sint * (tint - tH2O[j - 1]) - Sext * (tH2O[j - 1] - t[K, N - j + 1]));
            }

            bool Flag = true;

            if (Convert.ToInt32(s) >= w && Flag == true) //Flag не существует в данном контексте(создал)
            {
                s = "0";
                Console.WriteLine("Плавка N...." + plav);
            }
            Console.WriteLine("\r\nСкорость воды = " + WH2O / 3600 / Sk);
            Console.WriteLine("\r\nТекущее время = " + Convert.ToInt32(v) / 60 + " мин");
            Console.WriteLine("\r\nAlfa = " + alfaH2O + alfarz);

            if (Convert.ToInt32(v) <= wpr)
            {
                Console.WriteLine("\r\nВремя продувки = " + Convert.ToInt32(v) / 60 + "мин");
            }
            else
            {
                Console.WriteLine("\r\nВремя продувки = " + (Convert.ToInt32(v) - wpr) / 60 + "мин");
            }

            Console.WriteLine("\r\nТемпература среды = " + tv);
            Console.WriteLine("\r\nСредняя температура внутренней стенки = " + sss + ", " + sss1);
            Console.WriteLine("\r\nСредняяя температура наружной стенки = " + nnn + ", " + nnn1);
            Console.WriteLine("\r\n");
            Console.WriteLine("\r\nТЕМПЕРАТУРНОЕ ПОЛЕ СТЕНКИ");

            for (i = K; K < M; K++)
            {
                Console.WriteLine("\r\n" + i);
            }
            Console.WriteLine("\r\n");

            for (j = 1; j < N; j++)
            {
                Console.WriteLine("\r\n" + j);
                for (i = K; K < M; K++)
                {
                    Console.WriteLine("\r\n" + t[i, j]);
                    Console.WriteLine("\r\n" + tH2O[N - j + 1]);
                    Console.WriteLine("\r\n");
                }
                Console.WriteLine("\r\n");
            }

            if (Convert.ToInt32(v) == wpr)
            {
                sig = Methods.Stop_signal(K, M, 1, N, t, t1);
                t1 = t;
            }

            if (Convert.ToInt32(v) == wps)
            {
                v = "0";
            }

            if (sig == true)
            {
                sig = Methods.Stop_signal(K, M, 1, N, t, t2);
                t2 = t;
                if (sig == true)
                {
                    Flag = true;
                }
            }

            Console.ReadKey();
        }
    }
}
