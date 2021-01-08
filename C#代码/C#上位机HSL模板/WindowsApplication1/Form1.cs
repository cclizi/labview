using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WindowsApplication1.DAO;
using System.IO;
using System.Security.AccessControl;
using HslControls.Charts;
using HZH_Controls.Controls;
using HZH_Controls.Forms;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

//1.ZLGCAN系列接口卡信息的数据类型。
public struct VCI_BOARD_INFO 
{ 
	public UInt16  hw_Version;
    public UInt16  fw_Version;
    public UInt16 dr_Version;
    public UInt16 in_Version;
    public UInt16 irq_Num;
    public byte can_Num;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst=20)] public byte []str_Serial_Num;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
    public byte[] str_hw_Type;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Reserved;
}


/////////////////////////////////////////////////////
//2.定义CAN信息帧的数据类型。
unsafe public struct VCI_CAN_OBJ  //使用不安全代码
{
    public uint ID;
    public uint TimeStamp;
    public byte TimeFlag;
    public byte SendType;
    public byte RemoteFlag;//是否是远程帧
    public byte ExternFlag;//是否是扩展帧
    public byte DataLen;

    public fixed byte Data[8];

    public fixed byte Reserved[3];

}
////2.定义CAN信息帧的数据类型。
//public struct VCI_CAN_OBJ 
//{
//    public UInt32 ID;
//    public UInt32 TimeStamp;
//    public byte TimeFlag;
//    public byte SendType;
//    public byte RemoteFlag;//是否是远程帧
//    public byte ExternFlag;//是否是扩展帧
//    public byte DataLen;
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
//    public byte[] Data;
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
//    public byte[] Reserved;

//    public void Init()
//    {
//        Data = new byte[8];
//        Reserved = new byte[3];
//    }
//}

//3.定义CAN控制器状态的数据类型。
public struct VCI_CAN_STATUS 
{
    public byte ErrInterrupt;
    public byte regMode;
    public byte regStatus;
    public byte regALCapture;
    public byte regECCapture;
    public byte regEWLimit;
    public byte regRECounter;
    public byte regTECounter;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Reserved;
}

//4.定义错误信息的数据类型。
public struct VCI_ERR_INFO 
{
    public UInt32 ErrCode;
    public byte Passive_ErrData1;
    public byte Passive_ErrData2;
    public byte Passive_ErrData3;
    public byte ArLost_ErrData;
}

//5.定义初始化CAN的数据类型
public struct VCI_INIT_CONFIG 
{
    public UInt32 AccCode;
    public UInt32 AccMask;
    public UInt32 Reserved;
    public byte Filter;
    public byte Timing0;
    public byte Timing1;
    public byte Mode;
}

//6.定义发送数据格式
public struct FS_Data
{
    public UInt16 Data_1;
    public UInt16 Data_2;
    public UInt16 Data_3;
    public UInt16 Data_4;

}



public struct CHGDESIPANDPORT 
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] szpwd;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] szdesip;
    public Int32 desport;

    public void Init()
    {
        szpwd = new byte[10];
        szdesip = new byte[20];
    }
}

///////// new add struct for filter /////////
//typedef struct _VCI_FILTER_RECORD{
//    DWORD ExtFrame;	//是否为扩展帧
//    DWORD Start;
//    DWORD End;
//}VCI_FILTER_RECORD,*PVCI_FILTER_RECORD;
public struct VCI_FILTER_RECORD
{
    public UInt32 ExtFrame;
    public UInt32 Start;
    public UInt32 End;
}


namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        const int VCI_PCI5121 = 1;
        const int VCI_PCI9810 = 2;
        const int VCI_USBCAN1 = 3;
        const int VCI_USBCAN2 = 4;
        const int VCI_USBCAN2A = 4;
        const int VCI_PCI9820 = 5;
        const int VCI_CAN232 = 6;
        const int VCI_PCI5110 = 7;
        const int VCI_CANLITE = 8;
        const int VCI_ISA9620 = 9;
        const int VCI_ISA5420 = 10;
        const int VCI_PC104CAN = 11;
        const int VCI_CANETUDP = 12;
        const int VCI_CANETE = 12;
        const int VCI_DNP9810 = 13;
        const int VCI_PCI9840 = 14;
        const int VCI_PC104CAN2 = 15;
        const int VCI_PCI9820I = 16;
        const int VCI_CANETTCP = 17;
        const int VCI_PEC9920 = 18;
        const int VCI_PCI5010U = 19;
        const int VCI_USBCAN_E_U = 20;
        const int VCI_USBCAN_2E_U = 21;
        const int VCI_PCI5020U = 22;
        const int VCI_EG20T_CAN = 23;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceType"></param>
        /// <param name="DeviceInd"></param>
        /// <param name="Reserved"></param>
        /// <returns></returns>

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport("controlcan.dll")]
        //static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        unsafe static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, byte* pData);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        //[DllImport("controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);

        //static UInt32 m_devtype = 4;//USBCAN2
        static UInt32 m_devtype = 21;//USBCAN-2e-u
        //usb-e-u 波特率
        static UInt32[] GCanBrTab = new UInt32[10]{
                    0x060003, 0x060004, 0x060007,
                        0x1C0008, 0x1C0011, 0x160023,
                        0x1C002C, 0x1600B3, 0x1C00E0,
                        0x1C01C1
                };
        ////////////////////////////////////////
        const UInt32 STATUS_OK = 1;

        UInt32 m_bOpen = 0;
        UInt32 m_devind = 0;
        UInt32 m_canind = 0;

        //开关定义
        uint a = 0; //充电
        uint b = 0; //放电
        uint c = 0; //主继电器
        uint d = 0; //加热继电器
        uint m = 0; //主动放电

        //曲线定义
        //创建数据队列
        public Queue<float> x1 = new Queue<float>(); //外部输出电压
        public Queue<float> x2 = new Queue<float>(); //外部输出电流
        public Queue<float> x3 = new Queue<float>(); //外部输出功率
        public Queue<float> x4 = new Queue<float>(); //外部输入电流
        public Queue<float> x5 = new Queue<float>(); //DC输出电压
        public Queue<float> x6 = new Queue<float>(); //DC输出电流
        public Queue<float> x7 = new Queue<float>(); //DC输入电压
        public Queue<float> x8 = new Queue<float>(); //DC温度
        public Queue<float> x9 = new Queue<float>(); //内部Vbat
        public Queue<float> x10 = new Queue<float>(); //内部Vin
        public Queue<float> x11 = new Queue<float>(); //内部Ibat
        public Queue<float> x12 = new Queue<float>(); //内部Vbus
        public Queue<float> x13 = new Queue<float>(); //内部输出功率
        public Queue<float> x14 = new Queue<float>(); //内部IA
        public Queue<float> x15 = new Queue<float>(); //内部IB
        public Queue<float> x16 = new Queue<float>(); //内部Iout
        public Queue<float> x17 = new Queue<float>(); //内部Vout
        public Queue<float> x18 = new Queue<float>(); //内部温度1
        public Queue<float> x19 = new Queue<float>(); //内部温度2
        public Queue<float> x20 = new Queue<float>(); //内部温度3
        public Queue<float> x21 = new Queue<float>(); //内部温度4
        public Queue<float> x22 = new Queue<float>(); //内部保留
        public Queue<float> n = new Queue<float>();  //Y轴


        //定义数据保存文件路径
        String SavePath = @"D:\2020\TTRLDC\48V\" + DateTime.Now.ToLongDateString().ToString()+ DateTime.Now.ToString("hh-mm-ss")+ ".txt";
        String SavePath1 = @"D:\2020\TTRLDC\NBData\" + DateTime.Now.ToLongDateString().ToString() + DateTime.Now.ToString("hh-mm-ss") + ".txt";
        String SavePath2 = @"D:\2020\TTRLDC\WBData\" + DateTime.Now.ToLongDateString().ToString() + DateTime.Now.ToString("hh-mm-ss") + ".txt";
        VCI_CAN_OBJ[] m_recobj = new VCI_CAN_OBJ[50];

        UInt32[] m_arrdevtype = new UInt32[20];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox_DevIndex.SelectedIndex = 0;
            comboBox_CANIndex.SelectedIndex = 0;
            textBox_AccCode.Text = "00000000";
            textBox_AccMask.Text = "FFFFFFFF";
            textBox_Time0.Text = "00";
            textBox_Time1.Text = "14";
            comboBox_Filter.SelectedIndex = 1;
            comboBox_Mode.SelectedIndex = 0;


            comboBox_e_u_baud.SelectedIndex = 0;//1000Kbps
            comboBox_e_u_Filter.SelectedIndex = 2;//禁用
            textBox_e_u_startid.Text = "1";
            textBox_e_u_endid.Text = "ff";

            //
            Int32 curindex = 0;
            comboBox_devtype.Items.Clear();
            curindex = comboBox_devtype.Items.Add("USBCAN_1");
            m_arrdevtype[curindex] = VCI_USBCAN1;

            curindex = comboBox_devtype.Items.Add("USBCAN_E_U");
            m_arrdevtype[curindex] = VCI_USBCAN_E_U;

            curindex = comboBox_devtype.Items.Add("USBCAN_2E_U");
            m_arrdevtype[curindex] = VCI_USBCAN_2E_U;

            curindex = comboBox_devtype.Items.Add("PCI5010U");
            m_arrdevtype[curindex] = VCI_PCI5010U;

            curindex = comboBox_devtype.Items.Add("PCI5020U");
            m_arrdevtype[curindex] = VCI_PCI5020U;

            comboBox_devtype.SelectedIndex = 4;
            comboBox_devtype.MaxDropDownItems = comboBox_devtype.Items.Count;


            WriteTestFlagFile(@"D:\2020\TTRLDC\48V\", SavePath); //判断文件是否存在
            WriteTestFlagFile(@"D:\2020\TTRLDC\NBData\",SavePath1);
            WriteTestFlagFile(@"D:\2020\TTRLDC\WBData\", SavePath2);
            Console.WriteLine("创建文件");

            timer_boxing.Start();

        }
      
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (m_bOpen == 1)
            {
                VCI_CloseDevice(m_devtype, m_devind);
            }
        }
        //连接CAN
        unsafe private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 1)
            {
                VCI_CloseDevice(m_devtype, m_devind);
                m_bOpen = 0;
                //记录数据开启
                timer_DataSave.Start();
            }
            else
            {
                //记录数据停止
                timer_DataSave.Stop();
                m_devtype = m_arrdevtype[comboBox_devtype.SelectedIndex];

                m_devind = (UInt32)comboBox_DevIndex.SelectedIndex;
                m_canind = (UInt32)comboBox_CANIndex.SelectedIndex;
                if (VCI_OpenDevice(m_devtype, m_devind, 0) == 0)
                {
                    MessageBox.Show("打开设备失败,请检查设备类型和设备索引号是否正确", "错误",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                //USB-E-U 代码
                UInt32 baud;
                baud = GCanBrTab[comboBox_e_u_baud.SelectedIndex];
                if (VCI_SetReference(m_devtype, m_devind, m_canind, 0, (byte*)&baud) != STATUS_OK)
                {

                    MessageBox.Show("设置波特率错误，打开设备失败!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    VCI_CloseDevice(m_devtype, m_devind);
                    return;
                }
                //滤波设置


                //////////////////////////////////////////////////////////////////////////

                m_bOpen = 1;
                VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                config.AccCode = System.Convert.ToUInt32("0x" + textBox_AccCode.Text, 16);
                config.AccMask = System.Convert.ToUInt32("0x" + textBox_AccMask.Text, 16);
                config.Timing0 = System.Convert.ToByte("0x" + textBox_Time0.Text, 16);
                config.Timing1 = System.Convert.ToByte("0x" + textBox_Time1.Text, 16);
                config.Filter = (Byte)comboBox_Filter.SelectedIndex;
                config.Mode = (Byte)comboBox_Mode.SelectedIndex;
                VCI_InitCAN(m_devtype, m_devind, m_canind, ref config);

                //////////////////////////////////////////////////////////////////////////
                Int32 filterMode = comboBox_e_u_Filter.SelectedIndex;
                if (2 != filterMode)//不是禁用
                {
                    VCI_FILTER_RECORD filterRecord = new VCI_FILTER_RECORD();
                    filterRecord.ExtFrame = (UInt32)filterMode;
                    filterRecord.Start = System.Convert.ToUInt32("0x" + textBox_e_u_startid.Text, 16);
                    filterRecord.End = System.Convert.ToUInt32("0x" + textBox_e_u_endid.Text, 16);
                    //填充滤波表格
                    VCI_SetReference(m_devtype, m_devind, m_canind, 1, (byte*)&filterRecord);
                    //使滤波表格生效
                    byte tm = 0;
                    if (VCI_SetReference(m_devtype, m_devind, m_canind, 2, &tm) != STATUS_OK)
                    {
                        MessageBox.Show("设置滤波失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        VCI_CloseDevice(m_devtype, m_devind);
                        return;
                    }
                }
                //////////////////////////////////////////////////////////////////////////
            }
            buttonConnect.Text = m_bOpen == 1 ? "断开" : "连接";
            timer_rec.Enabled = m_bOpen == 1 ? true : false;
        }
        //显示信息
        uint y1 = 0;
        unsafe private void Timer_rec(object sender, EventArgs e)
        {
            UInt32 res = new UInt32();
            res = VCI_GetReceiveNum(m_devtype, m_devind, m_canind);
            if (res == 0)
                return;
            //res = VCI_Receive(m_devtype, m_devind, m_canind, ref m_recobj[0],50, 100);

            /////////////////////////////////////
            //UInt32 con_maxlen = 50;
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)res);


            res = VCI_Receive(m_devtype, m_devind, m_canind, pt, res, 100);
            ////////////////////////////////////////////////////////

            String str = "";
            for (UInt32 i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));

                str = "接收到数据: ";
                str += "  帧ID:0x" + System.Convert.ToString((Int32)obj.ID, 16);
                str += "  帧格式:";
                if (obj.RemoteFlag == 0)
                    str += "数据帧 ";
                else
                    str += "远程帧 ";
                if (obj.ExternFlag == 0)
                    str += "标准帧 ";
                else
                    str += "扩展帧 ";

                //////////////////////////////////////////
                if (obj.RemoteFlag == 0)
                {
                    str += "数据: ";
                    byte len = (byte)(obj.DataLen % 9);
                    byte j = 0;
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[0], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[1], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[2], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[3], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[4], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[5], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[6], 16);
                    if (j++ < len)
                        str += " " + System.Convert.ToString(obj.Data[7], 16);

                }
                listBox_Info.Items.Add(str);
                IDC_LIST_INFO.Items.Add(str);
                IDC_LIST_INFO.SelectedIndex = IDC_LIST_INFO.Items.Count - 1;
                listBox_Info.SelectedIndex = listBox_Info.Items.Count - 1;
                //判断接受到ID
                switch(System.Convert.ToString((Int32)obj.ID, 16)) 
                {
                    case "0CFF3A7":
                        byte[] guzhang = System.BitConverter.GetBytes(obj.Data[0]);
                        textBox_SR_I.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[1] << 8) + obj.Data[2]) - 10000) * 0.1);
                        textBox_SC_V.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[3] << 8) + obj.Data[4]) - 10000) * 0.1);
                        textBox_SC_I.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[5] << 8) + obj.Data[6]) - 10000) * 0.1);
                        byte[] guzhang1 = System.BitConverter.GetBytes(obj.Data[7]);

                        //判断故障
                        Falgsh1(guzhang);
                        Falgsh2(guzhang1);

                        //外部输出电压，电流，功率，输入电压数据进入队列
                        x1.Enqueue(uint.Parse(textBox_SR_I.Text));
                        x2.Enqueue(uint.Parse(textBox_SC_V.Text));
                        x3.Enqueue(uint.Parse(textBox_SC_I.Text));
                        x4.Enqueue((uint.Parse(textBox_SC_V.Text) * uint.Parse(textBox_SC_I.Text))/1000);
                        y1++;
                        n.Enqueue(y1);
                        break;

                    case "1801a0d1":

                        textBox_Vat.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[0] << 8) + obj.Data[1])-10000)*0.1);
                        textBox_Vin.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[2] << 8) + obj.Data[3]) - 10000) * 0.1);
                        textBox_Ibat.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[4] << 8) + obj.Data[5]) - 10000) * 0.1);
                        textBox_Vbus.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[6] << 8) + obj.Data[7]) - 10000) * 0.1);

                        //进入队列
                        x9.Enqueue(uint.Parse(textBox_Vat.Text));
                        x10.Enqueue(uint.Parse(textBox_Vin.Text));
                        x11.Enqueue(uint.Parse(textBox_Ibat.Text));
                        x12.Enqueue(uint.Parse(textBox_Vbus.Text));
                        y1++;
                        n.Enqueue(y1);
                        break;

                    case "1802a0d1":

                        textBox_AI.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[0] << 8) + obj.Data[1]) - 10000) * 0.1);
                        textBox_BI.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[2] << 8) + obj.Data[3]) - 10000) * 0.1);
                        textBox_Iout.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[4] << 8) + obj.Data[5]) - 10000) * 0.1);
                        textBox_Vout.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[6] << 8) + obj.Data[7]) - 10000) * 0.1);


                        //进入队列
                        x12.Enqueue(uint.Parse(textBox_AI.Text));
                        x13.Enqueue(uint.Parse(textBox_BI.Text));
                        x14.Enqueue(uint.Parse(textBox_Iout.Text));
                        x15.Enqueue(uint.Parse(textBox_Vout.Text));
                        y1++;
                        n.Enqueue(y1);
                        break;

                    case "1803a0d1":

                        textBox_LIC.Text = System.Convert.ToString(Convert.ToUInt16((obj.Data[0] << 8) + obj.Data[1]));
                        textBox_NSR_P.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[2] << 8) + obj.Data[3]) /1000));
                        textBox_NSC_P.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[4] << 8) + obj.Data[5]) /1000));
                        textBox_BL.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[6] << 8) + obj.Data[7])/1000));//保留


                        //波形显示
                        x16.Enqueue(float.Parse(textBox_NSR_P.Text));
                        x17.Enqueue(float.Parse(textBox_NSC_P.Text));
                        
                        y1++;
                        n.Enqueue(y1);  
                        break;

                    case "1804a0d1":

                        textBox_WD1.Text = System.Convert.ToString(Convert.ToUInt16(obj.Data[0])-40);
                        textBox_WD2.Text = System.Convert.ToString((Convert.ToUInt16(obj.Data[1])-40));
                        textBox_WD3.Text = System.Convert.ToString((Convert.ToUInt16(obj.Data[2])-40));
                        textBox_WD4.Text = System.Convert.ToString((Convert.ToUInt16(obj.Data[3] )-40));
                        byte[] guzhang3 = System.BitConverter.GetBytes((obj.Data[4]<<8)+ obj.Data[5]);
                        byte[] guzhang4 = System.BitConverter.GetBytes((obj.Data[6]));
                        byte[] guzhang5 = System.BitConverter.GetBytes((obj.Data[7]));
                        Falgsh3(guzhang3);
                        Falgsh4(guzhang4);
                        Falgsh5(guzhang5);


                        //进入队列
                        x18.Enqueue(uint.Parse(textBox_WD1.Text));
                        x19.Enqueue(uint.Parse(textBox_WD2.Text));
                        x20.Enqueue(uint.Parse(textBox_WD3.Text));
                        x21.Enqueue(uint.Parse(textBox_WD4.Text));
                        y1++;
                        n.Enqueue(y1);
                     
                        break;
                    case "c0ba79a":  //48V

                        textBox_48VSC_V.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[0] << 8) + obj.Data[1]) - 10000) * 0.1);
                        textBox_48VSC_I.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[2] << 8) + obj.Data[3]) - 10000) * 0.1);
                        textBox_48VSR_V.Text = System.Convert.ToString((Convert.ToUInt16((obj.Data[4] << 8) + obj.Data[5]) - 10000) * 0.1);
                        textBox_48VWD.Text = System.Convert.ToString((Convert.ToUInt16(obj.Data[6]) - 40));
                        byte[] guzhang6 = System.BitConverter.GetBytes(obj.Data[7]);
                        //判断故障
                        Falgsh6(guzhang6);
                        //进入队列
                        x5.Enqueue(uint.Parse(textBox_48VSC_V.Text));
                        x6.Enqueue(uint.Parse(textBox_48VSC_I.Text));
                        x7.Enqueue(uint.Parse(textBox_48VSR_V.Text));
                        x8.Enqueue(uint.Parse(textBox_48VWD.Text));
                        y1++;
                        n.Enqueue(y1);
                        break;
                }
               

            }

            Marshal.FreeHGlobal(pt);
        }

        private void button_StartCAN_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 0)
            return;
            VCI_StartCAN(m_devtype, m_devind, m_canind);

        }

        private void button_StopCAN_Click(object sender, EventArgs e)
        {
            if (m_bOpen == 0)
               
            return;
            VCI_ResetCAN(m_devtype, m_devind, m_canind);
        }

        //发送数据
        unsafe private void button_Send_Click(object sender, EventArgs e)
        {
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            //sendobj.Init();
            sendobj.SendType = (byte)0;
            sendobj.RemoteFlag = (byte)1;
            sendobj.ExternFlag = (byte)0;
            sendobj.ID = 0x1829d0f3;
            int len = 8;
            sendobj.DataLen = System.Convert.ToByte(len);
            //把电压电流等值赋给发送数据
            FS_Data fsd = new FS_Data(); //定义4个数据为UINT16
            byte[] fsb = new byte[8]; // 创建1Byte对象
            fsb[0] = System.Convert.ToByte(a);
            fsb[1] = System.Convert.ToByte(b);
            fsb[2] = System.Convert.ToByte(c);
            fsb[3] = System.Convert.ToByte(d);
            fsb[4] = System.Convert.ToByte(m);
            fsb[5] = System.Convert.ToByte(0);
            fsb[6] = System.Convert.ToByte(0);
            fsb[7] = System.Convert.ToByte(0);
            uint aa;
             aa=BitConverter.ToUInt16(fsb,0);

            fsd.Data_1 = UInt16.Parse(textBox_CD_V.Text); //启停
            fsd.Data_2 = (UInt16)((Double.Parse(textBox_FD_I.Text)*10)+10000) ; //放电电流
            fsd.Data_3 = (UInt16)((Double.Parse(textBox_CD_I.Text) * 10) + 10000); //充电电流
            fsd.Data_4 = (UInt16)((Double.Parse(textBox_CD_V.Text) * 10) + 10000); //充电电压
            Console.WriteLine("数据" + fsd.Data_1);
            int i = -1;
            if (i++ < len - 1)
                sendobj.Data[0] = System.Convert.ToByte(0xaa);
            if (i++ < len - 1)
                sendobj.Data[1] = System.Convert.ToByte((uint)fsd.Data_1 & 0xff);
            if (i++ < len - 1)
                sendobj.Data[2] = System.Convert.ToByte((uint)fsd.Data_2 >> 8 & 0xff);
            if (i++ < len - 1)
                sendobj.Data[3] = System.Convert.ToByte((uint)fsd.Data_2 & 0xff);
            if (i++ < len - 1)
                sendobj.Data[4] = System.Convert.ToByte((uint)fsd.Data_3 >> 8 & 0xff);
            if (i++ < len - 1)
                sendobj.Data[5] = System.Convert.ToByte((uint)fsd.Data_3 & 0xff);
            if (i++ < len - 1)
                sendobj.Data[6] = System.Convert.ToByte((uint)fsd.Data_4 >> 8 & 0xff);
            if (i++ < len - 1)
                sendobj.Data[7] = System.Convert.ToByte((uint)fsd.Data_4 & 0xff);
            int nTimeOut = 3000;
            VCI_SetReference(m_devtype, m_devind, m_canind, 4, (byte*)&nTimeOut);
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj, 1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                timer_send.Stop(); //停止循环发送
            }


            uint right, left;
            right = (uint)fsd.Data_1 >> 8 & 0xff;
            left = (uint)fsd.Data_1 & 0xff;
            Console.WriteLine("高八位" + right);
            Console.WriteLine("低八位" + left);
            Console.WriteLine("id" +sendobj.ID);
            Console.WriteLine("开关" +0xaa);
            for (i = 0; i < len; i++)
            {
                Console.WriteLine("数据" + sendobj.Data[i]);
                Console.WriteLine("开关" + fsb[i]);
                //textBox_Vat.Text = System.Convert.ToString((Convert.ToUInt16((sendobj.Data[2] << 8) + sendobj.Data[3]) - 10000) * 0.1);
            }

            Console.Read();


        }

        //发送第二帧
       unsafe private void btn_FS2_Click(object sender, EventArgs e)
        {
            VCI_CAN_OBJ sendobj1 = new VCI_CAN_OBJ();
            //sendobj.Init();
            sendobj1.SendType = (byte)0;
            sendobj1.RemoteFlag = (byte)1;
            sendobj1.ExternFlag = (byte)0;
            sendobj1.ID = 0x182AD0F3;
            int len = 8;
            sendobj1.DataLen = System.Convert.ToByte(len);
            //把电压电流等值赋给发送数据
            FS_Data fsd1 = new FS_Data(); //定义4个数据为UINT16
            fsd1.Data_1 = (UInt16)((Double.Parse(textBox_SCKZ_V.Text) * 10) + 10000); //输出电压
            fsd1.Data_2 = (UInt16)(0); 
            fsd1.Data_3 = (UInt16)(0); 
            fsd1.Data_4 = (UInt16)(0); 
            Console.WriteLine("数据" + fsd1.Data_1);
            int i = -1;
            if (i++ < len - 1)
                sendobj1.Data[0] = System.Convert.ToByte((uint)fsd1.Data_1 >> 8 & 0xff);
            if (i++ < len - 1)
                sendobj1.Data[1] = System.Convert.ToByte((uint)fsd1.Data_1 & 0xff);
            if (i++ < len - 1)
                sendobj1.Data[2] = System.Convert.ToByte((uint)fsd1.Data_2 >> 8 & 0xff);
            if (i++ < len - 1)
                sendobj1.Data[3] = System.Convert.ToByte((uint)fsd1.Data_2 & 0xff);
            if (i++ < len - 1)
                sendobj1.Data[4] = System.Convert.ToByte((uint)fsd1.Data_3 >> 8 & 0xff);
            if (i++ < len - 1)
                sendobj1.Data[5] = System.Convert.ToByte((uint)fsd1.Data_3 & 0xff);
            if (i++ < len - 1)
                sendobj1.Data[6] = System.Convert.ToByte((uint)fsd1.Data_4 >> 8 & 0xff);
            if (i++ < len - 1)
                sendobj1.Data[7] = System.Convert.ToByte((uint)fsd1.Data_4 & 0xff);
            int nTimeOut = 3000;
            VCI_SetReference(m_devtype, m_devind, m_canind, 4, (byte*)&nTimeOut);
            if (VCI_Transmit(m_devtype, m_devind, m_canind, ref sendobj1,1) == 0)
            {
                MessageBox.Show("发送失败", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                timer_send.Stop(); //停止循环发送
            }

            

            uint right, left;
            right = (uint)fsd1.Data_1 >> 8 & 0xff;
            left = (uint)fsd1.Data_1 & 0xff;
            Console.WriteLine("高八位" + right);
            Console.WriteLine("低八位" + left);
            Console.WriteLine("id" + sendobj1.ID);
            for (i = 0; i < len; i++)
            {
                Console.WriteLine("发送数据2" + sendobj1.Data[i]);
                //textBox_Vat.Text = System.Convert.ToString((Convert.ToUInt16((sendobj.Data[2] << 8) + sendobj.Data[3]) - 10000) * 0.1);
            }

            Console.Read();

        }

        //充电请求
        unsafe  private void btn_CD_Click(object sender, EventArgs e)
        {
           
            if ( a == 0)
            {
               
                btn_CD.Text = "关闭";
                a = 1;
            }
            else
            {
               
                btn_CD.Text = "打开";
                a = 0;
            }
            

        }
        //放电请求
        private void btn_FD_Click(object sender, EventArgs e)
        {
            if (b == 0)
            {
                btn_FD.Text = "关闭";
                b = 1;
            }
            else
            {
                btn_FD.Text = "打开";
                b = 0;
            }
        }
        //主动放电
        private void btn_ZDFD_Click(object sender, EventArgs e)
        {
            if (c == 0)
            {
                btn_ZDFD.Text = "关闭";
                c = 1;
            }
            else
            {
                btn_ZDFD.Text = "打开";
                c = 0;
            }
        }
        //主继电器
        private void btn_ZJDQ_Click(object sender, EventArgs e)
        {
            if (d == 0)
            {
                btn_ZJDQ.Text = "关闭";
               d = 1;
            }
            else
            {
                btn_ZJDQ.Text = "打开";
                d = 0;
            }
        }
        //加热继电器
        private void btn_JRJDQ_Click(object sender, EventArgs e)
        {
            if (m == 0)
            {
                btn_JRJDQ.Text = "关闭";
                m = 1;
            }
            else
            {
                btn_JRJDQ.Text = "打开";
                m = 0;
            }
        }
        //循环发送数据
        private void timer_send_Tick(object sender, EventArgs e)
        {
            button_Send_Click(null, null);
            btn_FS2_Click(null, null);
        }
        //开启/关闭循环发送
        private void checkBox_SendData_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SendData.Checked)
            {
                timer_send.Start();
            }
            else
            {
                timer_send.Stop();
            }
            
        }

        //定义判断故障状态
        //外部故障1
        public void Falgsh1(byte[] guzhang)
        {
            if (guzhang[0] == 1)
            {
                Falg_MXDY.BackColor = Color.Red;
                

            }
            else
            {
               Falg_MXDY.BackColor = Color.Black;
            }

            if (guzhang[1] == 1)
            {
                Falg_XM.BackColor = Color.Red;
                

            }
            else
            {
                Falg_XM.BackColor = Color.Black;
              
            }

            if (guzhang[2] == 1)
            {
                Falg_ZJDQZT.BackColor = Color.Red;
                

            }
            else
            {
                Falg_ZJDQZT.BackColor = Color.Black;
            }

            if (guzhang[3] == 1)
            {
                Falg_ZJDQDK.BackColor = Color.Red;
                

            }
            else
            {
                Falg_ZJDQDK.BackColor = Color.Black;
            }

            if (guzhang[4] == 1)
            {
                Falg_ZJDQBH.BackColor = Color.Red;
              

            }
            else
            {
                Falg_ZJDQBH.BackColor = Color.Black;
            }

            if (guzhang[5] == 1)
            {
                Falg_YL1.BackColor = Color.Red;
                

            }
            else
            {
                Falg_YL1.BackColor = Color.Black;
            }
            if (guzhang[6] == 1)
            {
                Falg_YL2.BackColor = Color.Red;
                

            }
            else
            {
                Falg_YL2.BackColor = Color.Black;
            }
            if (guzhang[7] == 1)
            {
                Falg_YL3.BackColor = Color.Red;
               

            }
            else
            {
                Falg_YL3.BackColor = Color.Black;
            }
            
        }

        //外部故障2
        public void Falgsh2(byte[] guzhang)
        {
            if (guzhang[0] == 1)
            {
                Falg_JRZJDQZT.BackColor = Color.Red;
               

            }
            else
            {
                Falg_JRZJDQZT.BackColor = Color.Black;
            }

            if (guzhang[1] == 1)
            {
                Falg_JRZJDQBH.BackColor = Color.Red;
              

            }
            else
            {
                Falg_JRZJDQBH.BackColor = Color.Black;

            }

            if (guzhang[2] == 1)
            {
                Falg_JRZJDQDK.BackColor = Color.Red;
                

            }
            else
            {
                Falg_JRZJDQDK.BackColor = Color.Black;
            }

            if (guzhang[3] == 1)
            {
                Falg_JRFJDQZT.BackColor = Color.Red;
              

            }
            else
            {
                Falg_JRFJDQZT.BackColor = Color.Black;
            }

            if (guzhang[4] == 1)
            {
                Falg_JRFJDQBH.BackColor = Color.Red;
                

            }
            else
            {
                Falg_JRFJDQBH.BackColor = Color.Black;
            }

            if (guzhang[5] == 1)
            {
                Falg_JRFJDQDK.BackColor = Color.Red;
                

            }
            else
            {
                Falg_JRFJDQDK.BackColor = Color.Black;
            }
            if (guzhang[6] == 1)
            {
                Falg_YL4.BackColor = Color.Red;
               

            }
            else
            {
                Falg_YL4.BackColor = Color.Black;
            }
            if (guzhang[7] == 1)
            {
                Falg_YL5.BackColor = Color.Red;
               

            }
            else
            {
                Falg_YL5.BackColor = Color.Black;
            }

        }
        //内部故障3
        public void Falgsh3(byte[] guzhang)
        {
            if (guzhang[0] == 1)
            {
                Falg_OT.BackColor = Color.Red;
               

            }
            else
            {
                Falg_OT.BackColor = Color.Black;
            }

            if (guzhang[1] == 1)
            {
                Falg_OPLV.BackColor = Color.Red;
                

            }
            else
            {
                Falg_OPLV.BackColor = Color.Black;

            }

            if (guzhang[2] == 1)
            {
                Falg_BATLV.BackColor = Color.Red;
                

            }
            else
            {
                Falg_BATLV.BackColor = Color.Black;
            }

            if (guzhang[3] == 1)
            {
                Falg_INLV.BackColor = Color.Red;
                

            }
            else
            {
                Falg_INLV.BackColor = Color.Black;
            }

            if (guzhang[4] == 1)
            {
                Falg_INOV.BackColor = Color.Red;
               

            }
            else
            {
                Falg_INOV.BackColor = Color.Black;
            }

            if (guzhang[5] == 1)
            {
                Falg_OPOV.BackColor = Color.Red;
               

            }
            else
            {
                Falg_OPOV.BackColor = Color.Black;
            }
            if (guzhang[6] == 1)
            {
                Falg_BUSOV.BackColor = Color.Red;
              

            }
            else
            {
                Falg_BUSOV.BackColor = Color.Black;
            }
            if (guzhang[7] == 1)
            {
                Falg_OPOC.BackColor = Color.Red;
               

            }
            else
            {
                Falg_OPOC.BackColor = Color.Black;
            }
            if (guzhang[8] == 1)
            {
                Falg_INOC.BackColor = Color.Red;


            }
            else
            {
                Falg_INOC.BackColor = Color.Black;
            }

            if (guzhang[9] == 1)
            {
                Falg_BPOC.BackColor = Color.Red;


            }
            else
            {
                Falg_BPOC.BackColor = Color.Black;

            }

            if (guzhang[10] == 1)
            {
                Falg_APOC.BackColor = Color.Red;


            }
            else
            {
                Falg_APOC.BackColor = Color.Black;
            }

            if (guzhang[11] == 1)
            {
                Falg_INOVH.BackColor = Color.Red;


            }
            else
            {
                Falg_INOVH.BackColor = Color.Black;
            }

            if (guzhang[12] == 1)
            {
                Falg_BUSOVH.BackColor = Color.Red;


            }
            else
            {
                Falg_BUSOVH.BackColor = Color.Black;
            }

            if (guzhang[13] == 1)
            {
                Falg_OPOVH.BackColor = Color.Red;


            }
            else
            {
                Falg_OPOVH.BackColor = Color.Black;
            }
            if (guzhang[14] == 1)
            {
                Falg_DCOCH.BackColor = Color.Red;


            }
            else
            {
                Falg_DCOCH.BackColor = Color.Black;
            }
            if (guzhang[15] == 1)
            {
                Falg_GATFAULT.BackColor = Color.Red;


            }
            else
            {
                Falg_GATFAULT.BackColor = Color.Black;
            }

        }
        //内部故障4
        public void Falgsh4(byte[] guzhang)
        {
            if (guzhang[0] == 1)
            {
                Falg_OT1.BackColor = Color.Red;
                

            }
            else
            {
                Falg_OT1.BackColor = Color.Black;
            }

            if (guzhang[1] == 1)
            {
                Falg_RELAY1FAULT.BackColor = Color.Red;
                

            }
            else
            {
                Falg_RELAY1FAULT.BackColor = Color.Black;

            }

            if (guzhang[2] == 1)
            {
                Falg_RELAY2FAULT.BackColor = Color.Red;
                

            }
            else
            {
                Falg_RELAY2FAULT.BackColor = Color.Black;
            }

            if (guzhang[3] == 1)
            {
                Falg_NYL4.BackColor = Color.Red;
               

            }
            else
            {
                Falg_NYL4.BackColor = Color.Black;
            }

            if (guzhang[4] == 1)
            {
                Falg_NYL5.BackColor = Color.Red;
               

            }
            else
            {
                Falg_NYL5.BackColor = Color.Black;
            }

            if (guzhang[5] == 1)
            {
                Falg_NYL6.BackColor = Color.Red;
               

            }
            else
            {
                Falg_NYL6.BackColor = Color.Black;
            }
            if (guzhang[6] == 1)
            {
                Falg_NYL7.BackColor = Color.Red;
              

            }
            else
            {
                Falg_NYL7.BackColor = Color.Black;
            }
            if (guzhang[7] == 1)
            {
                Falg_NYL8.BackColor = Color.Red;
               

            }
            else
            {
                Falg_NYL8.BackColor = Color.Black;
            }

        }
        //内部故障5
        public void Falgsh5(byte[] guzhang)
        {
            if (guzhang[0] == 1)
            {
                Falg_RunEnable.BackColor = Color.Red;


            }
            else
            {
                Falg_RunEnable.BackColor = Color.Black;
            }

            if (guzhang[1] == 1)
            {
                Falg_RLY1ON.BackColor = Color.Red;


            }
            else
            {
                Falg_RLY1ON.BackColor = Color.Black;

            }

            if (guzhang[2] == 1)
            {
                Falg_RLY2ON.BackColor = Color.Red;


            }
            else
            {
                Falg_RLY2ON.BackColor = Color.Black;
            }

            if (guzhang[3] == 1)
            {
                Falg_RLY3ON.BackColor = Color.Red;


            }
            else
            {
                Falg_RLY3ON.BackColor = Color.Black;
            }

            if (guzhang[4] == 1)
            {
                Falg_ChargeON.BackColor = Color.Red;


            }
            else
            {
                Falg_ChargeON.BackColor = Color.Black;
            }

            if (guzhang[5] == 1)
            {
                Falg_NYLF.BackColor = Color.Red;


            }
            else
            {
                Falg_NYLF.BackColor = Color.Black;
            }
            if (guzhang[6] == 1)
            {
                Falg_NYLF.BackColor = Color.Red;


            }
            else
            {
                Falg_NYLF.BackColor = Color.Black;
            }
            if (guzhang[7] == 1)
            {
                Falg_NYLH.BackColor = Color.Red;


            }
            else
            {
                Falg_NYLH.BackColor = Color.Black;
            }

        }





        //48V故障
        public void Falgsh6(byte[] guzhang)
        {
            if (guzhang[0] == 1)
            {
                Falg_48V_SCGL.BackColor = Color.Red;
                

            }
            else
            {
                Falg_48V_SCGL.BackColor = Color.Black;
            }

            if (guzhang[1] == 1)
            {
                Falg_48V_SCGY.BackColor = Color.Red;
                

            }
            else
            {
                Falg_48V_SCGY.BackColor = Color.Black;

            }

            if (guzhang[2] == 1)
            {
                Falg_48V_ZJGR.BackColor = Color.Red;
              

            }
            else
            {
                Falg_48V_ZJGR.BackColor = Color.Black;
            }

            if (guzhang[3] == 1)
            {
                Falg_48V_SRGY.BackColor = Color.Red;
              

            }
            else
            {
                Falg_48V_SRGY.BackColor = Color.Black;
            }

            if (guzhang[4] == 1)
            {
                Falg_48V_SRQY.BackColor = Color.Red;
                

            }
            else
            {
                Falg_48V_SRQY.BackColor = Color.Black;
            }

            if (guzhang[5] == 1)
            {
                Falg_48V_SCQY.BackColor = Color.Red;
               

            }
            else
            {
                Falg_48V_SCQY.BackColor = Color.Black;
            }
            if (guzhang[6] == 1)
            {
                Falg_48V_YL.BackColor = Color.Red;
              

            }
            else
            {
                Falg_48V_YL.BackColor = Color.Black;
            }
            if (guzhang[7] == 1)
            {
                Falg_48V_ZT.BackColor = Color.Red;
                

            }
            else
            {
                Falg_48V_ZT.BackColor = Color.Black;
            }

        }
        




        //数据保存

        //判断文件是都存在
        public void WriteTestFlagFile(string FilesPath,string LogPath)
        {
            try
            {
                Console.WriteLine(LogPath);

                if (!Directory.Exists(FilesPath))
                {//如果这个文件不存在，就创建这个文件
                    Directory.CreateDirectory(FilesPath);
                    //创建文件
                    FileStream fs = File.Create(LogPath);
                    fs.Close();
                    Console.WriteLine("路径" + File.Exists(LogPath));
                }
                else
                {
                    if (!File.Exists(LogPath))
                    {
                        //创建文件
                        FileStream fs =  File.Create(LogPath);
                        fs.Close();
                    }
                    Console.WriteLine("路径2" + File.Exists(LogPath));
                   
                }
             
            
            }
            catch (Exception)
            {//写入文件异常，这个地方一般是由于使用了非超级管理员权限向C盘写入了文件，报告权限不够
                if (MessageBox.Show("Run me in the role of the super administrator!", "Information Tip:", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1) == DialogResult.OK)
                {
                    this.Dispose();
                    Application.Exit();
                }
            }
        } 
        
        private void timer_DataSave_Tick(object sender, EventArgs e)
        {
          
           

            // 48V数据写入txt
            String texts = "输出电流: " + textBox_48VSC_I.Text+"输出电压: "+textBox_48VSC_V.Text + "输入电压: " + textBox_48VSR_V.Text + "温度 " + textBox_48VWD.Text;
             StreamWriter w = new StreamWriter(SavePath,true);
              w.WriteLine(texts + "\n");
              w.Flush();
              w.Close();
            //WB输入写入txt
            String texts1 = "Vat:"+textBox_Vat.Text+"Vin:"+textBox_Vin.Text+"Ibat"+textBox_Ibat.Text +"Vbus"+ textBox_Vbus.Text+"A相电流"+textBox_AI.Text + "B相电流" + textBox_BI.Text+
                            "Iout"+textBox_Iout.Text+ "Vout" + textBox_Vout.Text+"温度1"+textBox_WD1.Text + "温度2" + textBox_WD2.Text + "温度3" + textBox_WD3.Text + "温度4" + textBox_WD4.Text+
                            "输入功率"+textBox_NSR_P.Text+"输出功率"+textBox_NSC_P.Text;
            StreamWriter w1 = new StreamWriter(SavePath1, true);
            w1.WriteLine(texts1 + "\n");
            w1.Flush();
            w1.Close();
            //WB输入写入txt
            String texts2 = "输入电流: " + textBox_SR_I.Text + "输出电压: " + textBox_SC_V.Text + "输出电流: " + textBox_SC_I.Text + "输出功率: " + textBox_SC_P.Text;
            StreamWriter w2 = new StreamWriter(SavePath2, true);
            w2.WriteLine(texts2 + "\n");
            w2.Flush();
            w2.Close();
        }
        //波形显示
        private Random r = new Random();
        private void timer_boxing_Tick(object sender, EventArgs e)
        {


            //波形显示
            x16.Enqueue(float.Parse(textBox_NSR_P.Text));
            x17.Enqueue(float.Parse(textBox_NSC_P.Text));

            y1++;
            n.Enqueue(y1);

          if(n.Count >= 100)
            {
                float m = n.Dequeue(); //数组输出X轴
                Console.WriteLine(n.Count);
                //HSL动态数据
                ChartPointCollection points = hslChart1.Series[0].Points;
                points.Add(new HslControls.Charts.ChartPoint(m, m));
                ChartPointCollection points2 = hslChart1.Series[1].Points;
                points2.Add(new HslControls.Charts.ChartPoint(m+10, m-10));
                //Zgraph
                //zGraph_wb.f_reXY();
                //zGraph_wb.f_LoadOnePix(ref m, ref m, Color.Red, 2);
                //zGraph_wb.f_AddPix(ref m, ref m, Color.Blue, 4);

                //HZH
               // ucWaveChart1.AddSource(m.ToString(), m);
                ucWaveChart1.AddSource(m.ToString(), m*10);


                //chart
                //设置X轴名称
                chart1.ChartAreas[0].AxisX.Title = "时间（1格100ms）";
                //外部曲线
                //第一条曲线
                if(x1.Count != 0)
                  chart1.Series[0].Points.AddXY(m, x1.Dequeue());
                //第二条曲线
                if (x2.Count != 0)
                    chart1.Series[1].Points.AddXY(m, x2.Dequeue());
                //第三条曲线
                if (x16.Count != 0)
                    chart1.Series[2].Points.AddXY(m, x16.Dequeue());
                //第四条曲线
                if (x4.Count != 0)
                    chart1.Series[3].Points.AddXY(m, x4.Dequeue());



            }

           

        }
    }
}