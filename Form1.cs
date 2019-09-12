using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WFApp1
{
    public partial class Form1 : Form
    {
        //*****расчет температуры стенки верхней фурмы*****

        int[] Vector = new int[109];
        int[,] DimType = new int[109, 31];

        bool Flag, Flag1 = false;

        //*****константы*****

        //Esc = #27
        int wps = 300; //общее время процесса
        int wpr = 150; // время продувки сек

        double dz = 0.5; //расчет. шаг
        double dr = 0.001;

        int ro = 7800; // плотность трубной стенки
        double eps = 0.85; // степень черноты ствола
        int If = 15; // иследуемая длина фурма м

        double Dgv = 3.2; //диаметр горловины м
        double Dfn = 0.219; //наружный диаметр фурмы м
        double Dfv = 0.203; //внутренний диаметр наружной трубы фурмы м
        double Drt = 0.168; //диаметр разделительной трубы фурмы м

        int w = 1; // интервал печати
        int dt = 1; // расч. шаг

        int tint = 20; //температура воді на входе
        int cH2O = 4178; // теплоемкость воды
        int roH2O = 1000; //плотность воды
        int WH2O = 40; // расход воды

        double q1 = 0.93e6;
        double q2 = 0.16e6;

        //*****переменные*****
        bool fl, fl1, sig;

        char Key;

        int sss, sss1, nnn, nnn1;

        string StartMode, plav, v, s;

        int alfaH2O, vH2O, Sk, Sint, Sext, ReH2O, NuH2O, mn, alfarz, alfarv, alfakv, ee,
            lamv, njuv, Prv, tv, LamCO, njuCo, PrCO, wCO, Nu, Re, njuH2O, PrH2O, PrH2Ost, lamH2O, q;

        double K, M, N, i, j;

        byte priz;

        public double Progoonka(double PBeg, double Pend, int Ap, int Bp, int Cp, int Fp, int alfap, int betap)
        {
            int[] DTP = new int[399];
            double ip, zn;



            return betap;
        }



        //*****коэф свободной конвекции*****
        public double Alfa_KS(int lamsr, int nju, int Prsr, int tsr, int ll, int tt)
        {
            double bet, Gr, Nu;
            double Alfa_ks;

            if ((tt - tsr) < Math.E - 6)
            {
                Alfa_ks = 0;
                //Application.Exit; // выходим не считаем
            }

            bet = 1 / (0.5 * (tt + tsr) + 273.0);
            Gr = 9.81 * ll * ll * ll * bet * (tt - tsr / Math.Sqrt(nju));            
            
            if ((Gr * Prsr) < 1E-3)
            {
                Nu = 0.45;
            }
            if (((Gr * Prsr) < 1E-3) && ((Gr * Prsr) < 5E2))
            {
                Nu = 1.18 * Math.Exp(0.25 * Math.Log(Gr * Prsr));
            }
            if ((Gr * Prsr) >= 2E7)
            {
                Nu = 0.54 * Math.Exp(1.0 / 3.0 * Math.Log(Gr * Prsr));
            }

            Alfa_ks = Nu * lamsr / ll;

            return Alfa_ks;
        }

        //*****физ св-ва воздуха*****
        public void Vozduh()
        {
            int tt;
            double lamv, njuv, Prv;

            lamv = (tt * 7.83E2 - 2 - Math.Sqrt(tt) * 3.025E-5 + 24.144) / 1.0E3;
            njuv = (Math.Sqrt(tt) * 7.28E-5 + tt * 9.17E-2 + 12.762) / 1.0E6;
            Prv = 0.7;
        }

        //*****физ св-ва воды*****
        public void H2O(double t1, double t2, double nju, double Pr, double Prst, double lam)
        {
            

            nju = 1.306E - 6 + (t1 - 10) + 1E-6 * (0.295 - 1.306) / 90;
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
        }
        //*****эффективная теплопроводность******
        public double Lamef(double lam1, double lam2)
        {
            double lamef = 2 * lam1 - lam2 / (lam1 + lam2);
            return lamef;
        }
        //
        /// <summary>
        ///физические свойства СО 
        /// </summary>
        public void CO(double tt, double lamCO, double njuCO, double PrCO)
        {
            lamCO = 0.0249 + tt * 5.681E-5;
            njuCO = 1.55E-6 + tt * 1.677E-7;
            PrCO = 0.72;
        }

        //*****физ св-ва трубной стенки*****


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(0, 25);
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

            chart1.ChartAreas[0].AxisY.ScaleView.Zoom(-1, 1);
            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;

            for (int i = 0; i < 25; i++)
            {
                chart1.Series[0].Points.AddXY(i, Math.Sin(i));
            }
        }
    }
}
