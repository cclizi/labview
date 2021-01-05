using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TEST_MDI_DEMO
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true; // 将当前窗体设为父窗体
            Form2 ParentObj1 = new Form2();//创建窗体对象
            ParentObj1.MdiParent = this; // 将窗体设置为当前父窗体的子窗体
            ParentObj1.Show();//显示窗体
        }
    }
}
