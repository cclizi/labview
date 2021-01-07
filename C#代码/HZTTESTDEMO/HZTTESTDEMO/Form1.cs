using HZH_Controls.Controls;
using HZH_Controls.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HslControls.Charts;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace HZTTESTDEMO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           
        }
        int A = 0;
        private Random r = new Random();
        private void Form1_Load(object sender, EventArgs e)
        {
            if (FrmDialog.ShowDialog(this, "是否在现实", "窗体", true) == System.Windows.Forms.DialogResult.OK)
            {
                FrmDialog.ShowDialog(this, "这是一个没有取消按钮的测试库", "测试");
            }
            FrmInputs frm = new FrmInputs("动态多输入窗体测试",
               new string[] { "姓名", "电话", "身份证号", "住址" },
               new Dictionary<string, HZH_Controls.TextInputType>() { { "电话", HZH_Controls.TextInputType.Regex }, { "身份证号", HZH_Controls.TextInputType.Regex } },
               new Dictionary<string, string>() { { "电话", "^1\\d{10}$" }, { "身份证号", "^\\d{18}$" } },
               new Dictionary<string, KeyBoardType>() { { "电话", KeyBoardType.UCKeyBorderNum }, { "身份证号", KeyBoardType.UCKeyBorderNum } },
               new List<string>() { "姓名", "电话", "身份证号" });
            frm.ShowDialog(this);
        }

        private void btn_OK_BtnClick(object sender, EventArgs e)
        {

            if (A == 0)
            {
                Random r = new Random();
                int i = r.Next(100, 1000);
                boxingtu.AddSource(i.ToString(), i);
                A = 1;
                
            }
            if (A == 1)
            {
                Random r = new Random();
                int m = r.Next(100, 1000);
                int n = r.Next(10, 100);
                boxingtu.AddSource(m.ToString(), m);
                A = 0;

            }




        }


        private void Btn_Exti_BtnClick(object sender, EventArgs e)
        {
            //直接显示波形
            //for (int i = 0; i < 4; i++)
            //{
            //    double y = r.NextDouble();

            //    boxingtu1.Series[0].Points[i].Y = y;
            //    boxingtu1.Series[3].Points[i].Y = y;
            //}
            //for (int j = 0; j < 2; j++)
            //{
            //    // 修改数据也是这样的轻松
            //    boxingtu1.Series[1].Points[j].X = r.NextDouble();
            //    boxingtu1.Series[1].Points[j].Y = r.NextDouble();
            //    boxingtu1.Series[2].Points[j].X = r.NextDouble();
            //    boxingtu1.Series[2].Points[j].Y = r.NextDouble();
            //}



            ////以时间为坐标显示波形
            DateTime[] datetimes = new DateTime[10000];
            double[] values1 = new double[10000];
            double[] values2 = new double[10000];
            for (int i = 0; i < 10000; i++)
            {
                datetimes[i] = DateTime.Now.AddSeconds(i);
                values1[i] = r.NextDouble() * 100;
                values2[i] = r.Next(20, 30);
            }
            // 只需要使用下面这两个方法，就可以创建时间型坐标轴。
            boxingtu1.AxisX[0].SetLabels(datetimes);
            boxingtu1.AxisX[0].Visible = true;
            boxingtu1.Series[0].SetValuesY(values1);
            boxingtu1.Series[1].SetValuesY(values2);

            cartesianChart1.Series = new LiveCharts.SeriesCollection
            {
                new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0,7),
                        new ObservablePoint(1,3),
                        new ObservablePoint(4,5),
                        new ObservablePoint(4, 7)

                    },
                    PointGeometrySize = 15
                },
                 new LineSeries
                {
                    Values = new ChartValues<ObservablePoint>
                    {
                        new ObservablePoint(0,0),
                        new ObservablePoint(1,1),
                        new ObservablePoint(2,2),
                        new ObservablePoint(3,3)

                    },
                    PointGeometrySize = 15
                }
            };

           
        }
        private Point pt;
        private void lineChart_MouseUp(object sender, MouseEventArgs e)
        {
            pt = e.Location;
        }
    }
}
