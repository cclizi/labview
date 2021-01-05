using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReadTXTDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //创建全局变量
        String text1;

        private void btn_Read_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "文本文件(*,txt)|*.txt"; //设置文本报错格式
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamReader r = new StreamReader(openFileDialog1.FileName, true);
                text1 = r.ReadToEnd();
                textBox1.Text = text1;
                r.Close();
            }

        }

        private void btn_Write_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "")
            {
                MessageBox.Show("请输入写入内容");
            }
            else
            {
                    saveFileDialog1.Filter = "文本文件(*,txt)|*.txt"; //设置文本报错格式
                    if(saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //实例化写入对象，文件名另存为对话框输入名称
                    StreamWriter w = new StreamWriter(saveFileDialog1.FileName,true);
                    w.WriteLine(textBox1.Text);
                    w.Close();
                    textBox1.Clear();
                }
            }
        }
    }
}
