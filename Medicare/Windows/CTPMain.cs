using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommFTDI;
using Medicare.BLE;
using Medicare.PathLoss;
using Medicare.Port;
using Medicare.Setup;
using Medicare.Utility;
using Medicare.Visa;
using Medicare.Windows;

namespace Medicare.Main
{
    enum eCol
    {
        NO = 0,
        TST_ITEM = 1,
        FREQ = 2,
        MEAS,
        RES,
        LOWER,
        UPPER,
        TST
    }
    enum eTestItem
    {
        avgCH0 = 2,
        avgCH19,
        avgCH39,
        diffCH0,
        diffCH19,
        diffCH39,
        OPTIME = 8,
        OP_2M,
        avgCH0_2M,
        avgCH19_2M,
        avgCH39_2M,
        diffCH0_2M,
        diffCH19_2M,
        diffCH39_2M,
        OPTIME_2M,
        MOD,
        df1_avg_CH0,
        df1_avg_C19,
        df1_avg_CH39,
        df2_max_CH0,
        df2_max_CH19,
        df2_max_CH39,
        df2df1_CH0,
        df2df1_CH19,
        df2df1_CH39,
        MODTIME,
        MOD2,
        df1_avg_CH0_2M,
        df1_avg_C19_2M,
        df1_avg_CH39_2M,
        df2_max_CH0_2M,
        df2_max_CH19_2M,
        df2_max_CH39_2M,
        df2df1_CH0_2M,
        df2df1_CH19_2M,
        df2df1_CH39_2M,
        MODTIME_2M,
        CFD,
        ftx_fn_CH0,
        ftx_fn_CH19,
        ftx_fn_CH39,
        f0_fn_CH0,
        f0_fn_CH19,
        f0_fn_CH39,
        f1_f0_CH0,
        f1_f0_CH19,
        f1_f0_CH39,
        fn_fn5_CH0,
        fn_fn5_CH19,
        fn_fn5_CH39,
        CFDTIME,
        CFD2,
        ftx_fn_CH0_2M,
        ftx_fn_CH19_2M,
        ftx_fn_CH39_2M,
        f0_fn_CH0_2M,
        f0_fn_CH19_2M,
        f0_fn_CH39_2M,
        f1_f0_CH0_2M,
        f1_f0_CH19_2M,
        f1_f0_CH39_2M,
        fn_fn5_CH0_2M,
        fn_fn5_CH19_2M,
        fn_fn5_CH39_2M,
        CFDTIME_2M,
        RS,
        RV_PER_CH0,
        RV_PER_CH19,
        RV_PER_CH39,
        RSTIME,
        RS2,
        RV_PER_CH0_2M,
        RV_PER_CH19_2M,
        RV_PER_CH39_2M,
        RSTIME_2M,
        CFOM,
        DF0_MAX_CH0,
        DF0_MAX_CH19,
        DF0_MAX_CH39,
        DF0_MIN_CH0,
        DF0_MIN_CH19,
        DF0_MIN_CH39,
        DF0_AVG_CH0,
        DF0_AVG_CH19,
        DF0_AVG_CH39,
        DF2_MIN_CH0,
        DF2_MIN_CH19,
        DF2_MIN_CH39,
        DF2_AVG_CH0,
        DF2_AVG_CH19,
        DF2_AVG_CH39,
        CFOM_TIME,
        BLE_DISCONNECT,
        TOTAL_RESULT,
        total_tst_time,
        total_run_time
    }
    enum eBLECaseIdx
    {
        OP = 0,
        OP2 = 1,
        MOD,
        MOD2,
        CFD,
        CFD2,
        RS,
        RS2,
        CFOM,
        MAX
    }
    public partial class CTPMain : Form
    {
        public static bool isPuased = false;
        public static bool[] sbOn = new bool[2] { true, true };
        public static int resultVerdictIndex = 0;

        private Button[] statusButton = new Button[(int)ePort.MAX];

        public static eStatus currStatus = eStatus.STOP;

        //Directory
        public static string rootFolder = string.Empty;
        public static string rootFolderName = @"\Model";
        public static string resultFolder = "\\Model\\ZEUS\\Result\\Report";
        public static string logFolder = "\\Model\\ZEUS\\Result\\Log";
        public static string passwordFileName = @"\password.dat";
        public static string setupFileName = @"\Setup.cnf";
        public static string[] modelFolderName = new string[] { @"\ZEUS", @"\ZEUS2", @"\ZEUS3", @"\ZEUS4" };
        public static string btleConfigFolderName = @"\CONFIG\bt_le.cnf";
        public static string configFileName = @"\bt_le.cnf";
        public static string commanderDir = "";
        
        public static string bdAddr = string.Empty;
        public static string swVersion = string.Empty;
        public static bool isGetBDAddr = false;
        public static bool isGetVersion = false;
        public static string prevDutPort = string.Empty;
        public static string prevSBPort = string.Empty;

        public static string flowCtrl = string.Empty;
        public static string[] baudRateValueList = new string[7];

        public static string[] hciTypeValueList = new string[6];
        public static int selectedRFPort = 0;
        //password
        public static string pwd = string.Empty;
        public static bool isVisaOpen = false;
        //baud rate
        public static string baudRateIdx = string.Empty;
        public static string baudRateValue = string.Empty;
        //HCI Type
        public static string hciTypeIdx1 = string.Empty;
        public static string hciTypeIdx2 = string.Empty;
        public static bool isBLE = true;
        public static CTPMain wnd = null;
        public static List<List<string>> rowCategoryList = new List<List<string>>();

        public static IniFile passwordFile = new IniFile();
        public static IniFile setupFile = new IniFile();

        public static IniFile btConfigFile = new IniFile();
        public static string testMethod = string.Empty;
        public static string[] channel = new string[(int)eBLECaseIdx.MAX];
        public static string[] packetNum = new string[(int)eBLECaseIdx.MAX];
        private SerialPortManager sgr = null;
        public static OP op = null;
        public static OP2 op2 = null;
        public static MOD mod = null;
        public static MOD2 mod2 = null;
        public static CFD cfd = null;
        public static CFD2 cfd2 = null;
        public static RS rs = null;
        public static RS2 rs2 = null;
        public static CFOM cfom = null;
        public DateTime stTime = DateTime.MinValue;
        public static float[] testTime = new float[(int)eBLECaseIdx.MAX];
        public static int passCount = 0;
        public static int failCount = 0;
        public static int waitTime = 5;
        public static string[] chFreq = new string[] { "2402", "2440", "2480" };
        public static int prgCount = 1;
        public static int dtmIdx = 0;
        public string finalResult = string.Empty;
        public float tactTime = 0f;
        public float overallTime = 0f;
        public static List<string> lstError = new List<string>();
        public static int startIndex = 0;
        public static int resultIndex = 0;

        //User Firmware Download
        public static bool isUserFirmware = true;
        public static bool isDTMDown = true;
        public static bool isDownload = false;
        public static string dtmFileName = string.Empty;
        public static string userFileName = string.Empty;

        public static string serialNo = string.Empty;
        public static string commander_serial_number = string.Empty;

        private VisaManager vgr = VisaManager.Instance;

        public CTPMain()
        {
            InitializeComponent();
            startButton.Enabled = false;
            for (int i = 0; i < (int)eBLECaseIdx.MAX; i++)
            {
                testTime[i] = 0;
                NewSetupWindow.bleCaseCheckBoxList[i] = false;
            }
            loadCnf();
            loadBtConfig();
            //loadDataGrid();
            loadDataGrid2();

            wnd = this;
            sgr = SerialPortManager.Instance;

            if (!NewSetupWindow.devicePorts[(int)ePort.SB1].Equals("EMPTY")) // SB 연결
            {
                bool isConnected = NewSetupWindow.connectSB("RF1", (int)ePort.SB1);
                NewSetupWindow.isConnect[(int)ePort.SB1] = isConnected;
                if (!isConnected)
                {
                    NewSetupWindow.devicePorts[(int)ePort.SB1] = "EMPTY";
                    sbOn[(int)ePort.SB1] = false;
                }
            }

            if (!NewSetupWindow.devicePorts[(int)ePort.DUT1].Equals("EMPTY")) // dut 연결
            {
                bool isConnected = NewSetupWindow.connectDUT("RF1", (int)ePort.DUT1, 115200);
                //bool isConnected = FTDIDeviceCtrl.DeviceOpen(FDTI_Type.OPEN_TYPE.DESCRIPTION, "dut");
                NewSetupWindow.isConnect[(int)ePort.DUT1] = isConnected;
                if (!isConnected)
                    NewSetupWindow.devicePorts[(int)ePort.DUT1] = "EMPTY";
            }

            isVisaOpen = false;

            //if (Util.IsCheckNetwork())
                isVisaOpen = vgr.openVisaSession(); // mtp300 연결

            if (isVisaOpen)
            {
                mtp300Btn.BackColor = Color.Green;
                NewSetupWindow.isMtp300Connect = true;
            }
            else
            {
                mtp300Btn.BackColor = Color.Red;
                NewSetupWindow.isMtp300Connect = false;
            }

            statusButton[0] = sbBtn1;
            statusButton[2] = dutBtn1;

            Task.Run(() =>      // MTP300 indicator check
            {
                checkDeviceStatus();
            });

            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            timer1.Interval = 1000;
            timer1.Start();

            setTextPgrBar("READY", Color.Black);
            // MES DB Connect
            //DBManager.Instance.connectDB(NewSetupWindow.mesIP, NewSetupWindow.mesDBName, NewSetupWindow.mesID,
            //    NewSetupWindow.mesPwd);
            resetBDaddr();
            resetSWVersion();
            this.Size = new Size(876, 630);

            initColor();

            currStatus = eStatus.READY;
        }

        void initColor()
        {
            InstrumentLabel.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            InstrumentLabel.ForeColor = Color.White;
            ctpVersionLabel.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            ctpVersionLabel.ForeColor = Color.White;
            modelLabel.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            modelLabel.ForeColor = Color.White;
            logoLabel.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            logoLabel.ForeColor = Color.White;

            startButton.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            startButton.ForeColor = Color.White;

            pauseButton.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            pauseButton.ForeColor = Color.White;

            reportButton.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            reportButton.ForeColor = Color.White;

            logButton.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            logButton.ForeColor = Color.White;

            pathlossBtn.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            pathlossBtn.ForeColor = Color.White;

            setupButton.BackColor = Color.FromArgb(0, 54, 105); // R, G, B
            setupButton.ForeColor = Color.White;

            this.BackColor = Color.FromArgb(166, 175, 190);

            upperPanel.BackColor = Color.FromArgb(235, 236, 238);
            infoPanel.BackColor = Color.FromArgb(235, 236, 238);
            msgPanel.BackColor = Color.FromArgb(235, 236, 238);

            instruPanel.BackColor = Color.FromArgb(235, 236, 238);
            buttonPanel.BackColor = Color.FromArgb(235, 236, 238);

            pgrBar.BackColor = Color.FromArgb(255, 242, 204);
            pgrBar.ForeColor = Color.FromArgb(0, 54, 105);
            pgrBar.Style = ProgressBarStyle.Continuous;

        }
        private void resetBDaddr()
        {
            bdAddrLabel.Text = "N/A";
            bdAddr = "";
        }

        private void resetSWVersion()
        {
            userFirmwareText.Text = "N/A";
        }
        protected void OnLoad(object sender, EventArgs e)
        {
            // do stuff before Load-event is raised
            //base.OnLoad(e);
            // do stuff after Load-event was raised
            mainBLEGrid.ClearSelection();
            mainBLEGrid.Refresh();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            SetClock();
        }
        protected void SetClock()
        {
            string str = DateTime.Now.ToString();
            int index = str.IndexOf(" ");

            timeLabel.Text = str.ToString();
        }
        
        private void hideRows()
        {
            for (int i = 0; i < NewSetupWindow.bleCaseCheckBoxList.Length; i++)
            {
                bool isfalse = !NewSetupWindow.bleCaseCheckBoxList[i];

                if (isfalse)
                {
                    int st = -1;
                    int ed = 1;

                    if (i == (int)eBLECaseIdx.OP)
                    {
                        st = (int)eTestItem.avgCH0;
                        ed = (int)eTestItem.OPTIME;
                    }
                    else if (i == (int)eBLECaseIdx.OP2)
                    {
                        st = (int)eTestItem.avgCH0_2M;
                        ed = (int)eTestItem.OPTIME_2M;
                    }

                    for (int r = st; r <= ed; r++)
                        mainBLEGrid.Rows.RemoveAt(r);
                }
            }
            mainBLEGrid.Refresh();
        }
        private void loadDataGrid2()
        {
            rowCategoryList.Clear();
            rowCategoryList.Add(new List<string>() { "Tester Connect" });
            rowCategoryList.Add(new List<string>() { "BLE Output Power", "Pavg_CH0", "Pavg_CH19", "Pavg_CH39", "Ppk-Pav_CH0", "Ppk-Pav_CH19", "Ppk-Pav_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "BLE Output Power 2M", "Pavg_CH0", "Pavg_CH19", "Pavg_CH39", "Ppk-Pav_CH0", "Ppk-Pav_CH19", "Ppk-Pav_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Modulation Characteristics", "df1_avg_CH0", "df1_avg_CH19", "df1_avg_CH39", "df2_max_CH0", "df2_max_CH19", "df2_max_CH39", "df2/df1_CH0", "df2/df1_CH19", "df2/df1_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Modulation Characteristics 2M", "df1_avg_CH0", "df1_avg_CH19", "df1_avg_CH39", "df2_max_CH0", "df2_max_CH19", "df2_max_CH39", "df2/df1_CH0", "df2/df1_CH19", "df2/df1_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Carrier Frequency Offset and Drift", "|ftx_fn|_CH0", "|ftx_fn|_CH19", "|ftx_fn|_CH39", "|f0_fn|_CH0", "|f0_fn|_CH19", "|f0_fn|_CH39", "|f1_f0|_CH0", "|f1_f0|_CH19", "|f1_f0|_CH39", "|fn_fn5|_CH0", "|fn_fn5|_CH19", "|fn_fn5|_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Carrier Frequency Offset and Drift 2M", "|ftx_fn|_CH0", "|ftx_fn|_CH19", "|ftx_fn|_CH39", "|f0_fn|_CH0", "|f0_fn|_CH19", "|f0_fn|_CH39", "|f1_f0|_CH0", "|f1_f0|_CH19", "|f1_f0|_CH39", "|fn_fn5|_CH0", "|fn_fn5|_CH19", "|fn_fn5|_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Receiver Sensitivity", "RV_PER_CH0", "RV_PER_CH19", "RV_PER_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Receiver Sensitivity 2M", "RV_PER_CH0", "RV_PER_CH19", "RV_PER_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Carrier Freq. Offset + Mod Char (preamble) 1M", "DF0_MAX_CH0", "DF0_MAX_CH19", "DF0_MAX_CH39", "DF0_MIN_CH0", "DF0_MIN_CH19", "DF0_MIN_CH39", "DF0_AVG_CH0", "DF0_AVG_CH19", "DF0_AVG_CH39", "DF2_MIN_CH0", "DF2_MIN_CH19", "DF2_MIN_CH39", "DF2_AVG_CH0", "DF2_AVG_CH19", "DF2_AVG_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "BLE DUT Disconnect" });
            rowCategoryList.Add(new List<string>() { "Download User Firmware" });
            rowCategoryList.Add(new List<string>() { "PASS", "Test Time", "Total Time" });

            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < rowCategoryList.Count; j++)
                {
                    if (0 < j && j < 10 && NewSetupWindow.bleCaseCheckBoxList[j - 1] == false)
                        continue;

                    for (int k = 0; k < rowCategoryList[j].Count; k++)
                    {
                        mainBLEGrid.Rows.Add();
                        int cnt = mainBLEGrid.RowCount;

                        if ((int)eCol.NO == k)
                        {
                            mainBLEGrid.Rows[cnt - 1].Cells[(int)eCol.NO].Value = j + 1;
                            mainBLEGrid.Rows[cnt - 1].DefaultCellStyle.BackColor = Color.DarkSeaGreen;
                        }

                        mainBLEGrid.Rows[cnt - 1].Cells[(int)eCol.TST_ITEM].Value = rowCategoryList[j][k];
                    }
                }
            }

            resultIndex = mainBLEGrid.RowCount - 3;
        }
        private void loadDataGrid()
        {
            rowCategoryList.Clear();
            rowCategoryList.Add(new List<string>() { "Tester Connect" });
            rowCategoryList.Add(new List<string>() { "BLE Output Power", "Pavg_CH0", "Pavg_CH19", "Pavg_CH39", "Ppk-Pav_CH0", "Ppk-Pav_CH19", "Ppk-Pav_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "BLE Output Power 2M", "Pavg_CH0", "Pavg_CH19", "Pavg_CH39", "Ppk-Pav_CH0", "Ppk-Pav_CH19", "Ppk-Pav_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Modulation Characteristics", "df1_avg_CH0", "df1_avg_CH19", "df1_avg_CH39", "df2_max_CH0", "df2_max_CH19", "df2_max_CH39", "df2/df1_CH0", "df2/df1_CH19", "df2/df1_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Modulation Characteristics 2M", "df1_avg_CH0", "df1_avg_CH19", "df1_avg_CH39", "df2_max_CH0", "df2_max_CH19", "df2_max_CH39", "df2/df1_CH0", "df2/df1_CH19", "df2/df1_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Carrier Frequency Offset and Drift", "|ftx_fn|_CH0", "|ftx_fn|_CH19", "|ftx_fn|_CH39", "|f0_fn|_CH0", "|f0_fn|_CH19", "|f0_fn|_CH39", "|f1_f0|_CH0", "|f1_f0|_CH19", "|f1_f0|_CH39", "|fn_fn5|_CH0", "|fn_fn5|_CH19", "|fn_fn5|_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Carrier Frequency Offset and Drift 2M", "|ftx_fn|_CH0", "|ftx_fn|_CH19", "|ftx_fn|_CH39", "|f0_fn|_CH0", "|f0_fn|_CH19", "|f0_fn|_CH39", "|f1_f0|_CH0", "|f1_f0|_CH19", "|f1_f0|_CH39", "|fn_fn5|_CH0", "|fn_fn5|_CH19", "|fn_fn5|_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Receiver Sensitivity", "RV_PER_CH0", "RV_PER_CH19", "RV_PER_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Receiver Sensitivity 2M", "RV_PER_CH0", "RV_PER_CH19", "RV_PER_CH39", "Test Time" });
            rowCategoryList.Add(new List<string>() { "Carrier Freq. Offset + Mod Char (preamble) 1M", "DF0_MAX_CH0", "DF0_MAX_CH19", "DF0_MAX_CH39", "DF0_MIN_CH0", "DF0_MIN_CH19", "DF0_MIN_CH39", "DF0_AVG_CH0", "DF0_AVG_CH19", "DF0_AVG_CH39", "DF2_MIN_CH0", "DF2_MIN_CH19", "DF2_MIN_CH39", "DF2_AVG_CH0", "DF2_AVG_CH19", "DF2_AVG_CH39" });
            rowCategoryList.Add(new List<string>() { "BLE DUT Disconnect" });
            rowCategoryList.Add(new List<string>() { "PASS", "Test Time", "Total Time" });

            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < rowCategoryList.Count; j++)
                {
                    for (int k = 0; k < rowCategoryList[j].Count; k++)
                    {
                        mainBLEGrid.Rows.Add();
                        int cnt = mainBLEGrid.RowCount;

                        if ((int)eCol.NO == k)
                        {
                            mainBLEGrid.Rows[cnt - 1].Cells[(int)eCol.NO].Value = j + 1;
                            mainBLEGrid.Rows[cnt - 1].DefaultCellStyle.BackColor = Color.DarkSeaGreen;
                        }

                        mainBLEGrid.Rows[cnt - 1].Cells[(int)eCol.TST_ITEM].Value = rowCategoryList[j][k];
                    }
                }
            }
        }
        public static void loadCnf()
        {
            //root folder
            var currentDir = Directory.GetCurrentDirectory();
            rootFolder = Directory.GetCurrentDirectory() + rootFolderName;
            //read password.dat
            passwordFile.Load(currentDir + passwordFileName);
            pwd = passwordFile["password"]["password"].ToString();
            //read Setup.cnf
            setupFile.Load(currentDir + setupFileName);
            NewSetupWindow.modelName = setupFile["common"]["model_name"].ToString();
            NewSetupWindow.measScenario = Int32.Parse(setupFile["common"]["meas_scenario"].ToString());
            NewSetupWindow.retryNum = Int32.Parse(setupFile["common"]["retry_num"].ToString());
            NewSetupWindow.isScannerEnable =
                setupFile["common"]["scanner_enable"].ToString().Equals("True") == true ? true : false;
            NewSetupWindow.isLogAutoSave = setupFile["common"]["log_autosave"].ToString().Equals("True") == true ? true : false;
            NewSetupWindow.isIterationMode =
                setupFile["common"]["iteration_mode"].ToString().Equals("True") == true ? true : false;
            NewSetupWindow.iterCount = Int32.Parse(setupFile["common"]["iteration_count"].ToString());
            NewSetupWindow.isFailStop = setupFile["common"]["isFailStop"].ToString().Equals("True") ? true : false;
            VisaManager.ip = setupFile["common"]["mtp300_ip"].ToString();

            var list = setupFile["common"]["box_check"].ToString().Split(',');

            bool[] l = new bool[sbOn.Length];
            for (int i = 0; i < 2; i++)
            {
                if (list[i].Equals("True"))
                    l[i] = true;
                else
                    l[i] = false;
            }

            Array.Copy(l, sbOn, sbOn.Length);

            var list2 = setupFile["common"]["box_port"].ToString().Split(',');
            var list3 = setupFile["common"]["dut_port"].ToString().Split(',');
            var list4 = setupFile["common"]["sc_port"].ToString().Split(',');

            string[] strlist = new string[(int)ePort.MAX];
            strlist[0] = list2[0];
            strlist[1] = list2[1];
            strlist[2] = list3[0];
            strlist[3] = list3[1];
            //strlist[4] = "MTP300";
            strlist[4] = list4[0];
            strlist[5] = list4[1];
            //selected_model_idx
            NewSetupWindow.selectedModelIdx = setupFile["common"]["selected_model_idx"].ToString();
            // MES setting
            NewSetupWindow.mesUse = setupFile["common"]["MES_USE"].ToString();
            NewSetupWindow.mesDBName = setupFile["common"]["MES_DB_NAME"].ToString();
            NewSetupWindow.mesIP = setupFile["common"]["MES_IP"].ToString();
            NewSetupWindow.mesPort = setupFile["common"]["MES_PORT"].ToString();
            NewSetupWindow.mesID = setupFile["common"]["MES_ID"].ToString();
            NewSetupWindow.mesPwd = setupFile["common"]["MES_PWD"].ToString();

            isDTMDown = setupFile["common"]["DTM_DOWN"].ToString().Equals("True") ? true : false;
            isUserFirmware = setupFile["common"]["USER_DOWN"].ToString().Equals("True") ? true : false;
            commanderDir = setupFile["common"]["commanderdir"].ToString();
            serialNo = setupFile["common"]["serialno"].ToString();
            dtmFileName = setupFile["common"]["dtmfilename"].ToString();
            userFileName = setupFile["common"]["userfilename"].ToString();

            if (commanderDir == null)
                commanderDir = string.Empty;

            Array.Copy(strlist, NewSetupWindow.devicePorts, strlist.Length);
            prevDutPort = NewSetupWindow.devicePorts[(int)ePort.DUT1];
            prevSBPort = NewSetupWindow.devicePorts[(int)ePort.SB1];

            Console.WriteLine("===== SETUP.cnf =====");
            Console.WriteLine("** Root Folder Dir : " + rootFolder);
            Console.WriteLine("** password dir : " + rootFolder + passwordFileName);
            Console.WriteLine("** setup dir : " + rootFolder + setupFileName);
            Console.WriteLine("** password : " + pwd);
            Console.WriteLine("** Model Name : " + NewSetupWindow.modelName);
            Console.WriteLine("** meas scenario : " + NewSetupWindow.measScenario);
            Console.WriteLine("** retry num : " + NewSetupWindow.retryNum);
            Console.WriteLine("** scanner enable : " + NewSetupWindow.isScannerEnable);
            Console.WriteLine("** log autosave : " + NewSetupWindow.isLogAutoSave);
            Console.WriteLine("** iteration mode: " + NewSetupWindow.isIterationMode);
            Console.WriteLine("** iteration count : " + NewSetupWindow.iterCount);
            Console.WriteLine("** mtp300 IP : " + VisaManager.ip);
            string isfailstop = NewSetupWindow.isFailStop == true ? "ON" : "OFF";
            Console.WriteLine("** is fail stop : " + isfailstop);
            //Console.WriteLine("** selected model idx : " + NewSetupWindow.selectedModelIdx);
            //Console.WriteLine("** MES_USE : " + NewSetupWindow.mesUse);
            //Console.WriteLine("** MES_DB_NAME : " + NewSetupWindow.mesDBName);
            //Console.WriteLine("** MES_IP : " + NewSetupWindow.mesIP);
            //Console.WriteLine("** MES_PORT : " + NewSetupWindow.mesPort);
            //Console.WriteLine("** MES_ID : " + NewSetupWindow.mesID);
            //Console.WriteLine("** MES_PWD : " + NewSetupWindow.mesPwd);
            Console.WriteLine("** DTM FILE NAME : " + dtmFileName);
            Console.WriteLine("** COMMANDER DIR : " + commanderDir);
            Console.WriteLine("** SERIAL NO. : " + serialNo);
            Console.WriteLine("=====================");
        }
        private void checkDeviceStatus()
        {
            while (true)
            {
                Thread.Sleep(300);

                //for (int i = (int)ePort.DUT1; i <= (int)ePort.SC2; i++)
                {
                    if (NewSetupWindow.isConnect[(int)ePort.DUT1])
                        NewSetupWindow.setButtonStatus(statusButton[(int)ePort.DUT1], Color.Green);
                    else
                        NewSetupWindow.setButtonStatus(statusButton[(int)ePort.DUT1], Color.Red);
                }

                if (NewSetupWindow.isConnect[(int)ePort.SB1])
                {
                    if (!sbOn[(int)ePort.SB1])
                        NewSetupWindow.setButtonStatus(sbBtn1, Color.Gray);
                    else
                        NewSetupWindow.setButtonStatus(sbBtn1, Color.Green);
                }
                else
                {
                    NewSetupWindow.setButtonStatus(sbBtn1, Color.Red);
                }

                //if (NewSetupWindow.isConnect[(int)ePort.SB2])
                //{
                //    if (!sbOn[(int)ePort.SB2])
                //        NewSetupWindow.setButtonStatus(sbBtn2, Color.Gray);
                //    else
                //        NewSetupWindow.setButtonStatus(sbBtn2, Color.Green);
                //}
                //else
                //{
                //    NewSetupWindow.setButtonStatus(sbBtn2, Color.Red);
                //}

                //Util.writeLog24();
                //if (!Util.IsCheckNetwork())
                //{
                //    mtp300Btn.BackColor = Color.Red;
                //    NewSetupWindow.isMtp300Connect = false;
                //    VisaManager.Instance.closeVisaSession();
                //}
                //else
                //{
                //    if (VisaManager.Instance.isConnected())
                //    {
                //        mtp300Btn.BackColor = Color.Green;
                //        NewSetupWindow.isMtp300Connect = true;
                //    }
                //    else
                //    {
                //        mtp300Btn.BackColor = Color.Red;
                //        NewSetupWindow.isMtp300Connect = false;
                //    }
                //}

                if (currStatus == eStatus.RUNNING)
                {
                    if (startButton.InvokeRequired)
                    {
                        startButton.BeginInvoke(new Action(() => startButton.Enabled = false));
                    }
                }
                else
                {
                    if (startButton.InvokeRequired)
                    {
                        startButton.BeginInvoke(new Action(() => startButton.Enabled = true));
                    }
                }
            }
        }

        private void rf1ResetButton_Click(object sender, EventArgs e)
        {
            rf1TotalCount.Text = "0";
            rf1PassCount.Text = "0";
            rf1NGCount.Text = "0";
        }

        private void clearMainGrid()
        {
            resultVerdictIndex = 0;

            while (mainBLEGrid.Rows.Count > 0)
                mainBLEGrid.Rows.RemoveAt(mainBLEGrid.Rows.Count - 1);

            mainBLEGrid.Refresh();

            pgrBar.Value = 0;
        }
        private void resetMainGrid()
        {
            resultVerdictIndex = 0;

            while (mainBLEGrid.Rows.Count > 0)
                mainBLEGrid.Rows.RemoveAt(mainBLEGrid.Rows.Count - 1);

            mainBLEGrid.Refresh();

            //loadDataGrid();
            loadDataGrid2();

            pgrBar.Value = 0;
        }
        public static void loadBtConfig()
        {
            try
            {
                baudRateValueList[0] = "2400";
                baudRateValueList[1] = "4800";
                baudRateValueList[2] = "9600";
                baudRateValueList[3] = "19200";
                baudRateValueList[4] = "38400";
                baudRateValueList[5] = "57600";
                baudRateValueList[6] = "115200";

                hciTypeValueList[0] = "NONE";
                hciTypeValueList[1] = "USB";
                hciTypeValueList[2] = "UART";
                hciTypeValueList[3] = "2-wire UART";
                hciTypeValueList[4] = "USB to UART";
                hciTypeValueList[5] = "USB to 2-wire";

                var dir = rootFolder + modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] + btleConfigFolderName;

                btConfigFile.Load(dir);
                baudRateIdx = btConfigFile["common"]["baud_rate"].ToString();
                baudRateValue = baudRateValueList[Int32.Parse(baudRateIdx)];
                hciTypeIdx1 = btConfigFile["common"]["hci_type1"].ToString();
                hciTypeIdx2 = btConfigFile["common"]["hci_type2"].ToString();
                flowCtrl = btConfigFile["common"]["flow_ctrl"].ToString();
                testMethod = btConfigFile["common"]["test_method"].ToString();

                //op
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.OP] =
                    btConfigFile["op"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.OP] = btConfigFile["op"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.OP] = btConfigFile["op"]["npackets"].ToString();
                OP.pavg_up = btConfigFile["op"]["Pavg_up"].ToString();
                OP.pavg_low = btConfigFile["op"]["Pavg_low"].ToString();
                OP.power_diff = btConfigFile["op"]["Power_diff"].ToString();
                //op2
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.OP2] =
                    btConfigFile["op2"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.OP2] = btConfigFile["op2"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.OP2] = btConfigFile["op2"]["npackets"].ToString();
                OP2.pavg_up = btConfigFile["op2"]["Pavg_up"].ToString();
                OP2.pavg_low = btConfigFile["op2"]["Pavg_low"].ToString();
                OP2.power_diff = btConfigFile["op2"]["Power_diff"].ToString();

                //mo
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.MOD] =
                    btConfigFile["mo"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.MOD] = btConfigFile["mo"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.MOD] = btConfigFile["mo"]["npackets"].ToString();
                MOD.df1avgupper = btConfigFile["mo"]["df1avgupper"].ToString();
                MOD.df1avglower = btConfigFile["mo"]["df1avglower"].ToString();
                MOD.df2max = btConfigFile["mo"]["df2max"].ToString();
                MOD.df2avgdf1avg = btConfigFile["mo"]["df2avgdf1avg"].ToString();

                //mo2
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.MOD2] =
                    btConfigFile["mo2"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.MOD2] = btConfigFile["mo2"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.MOD2] = btConfigFile["mo2"]["npackets"].ToString();
                MOD2.df1avgupper = btConfigFile["mo2"]["df1avgupper"].ToString();
                MOD2.df1avglower = btConfigFile["mo2"]["df1avglower"].ToString();
                MOD2.df2max = btConfigFile["mo2"]["df2max"].ToString();
                MOD2.df2avgdf1avg = btConfigFile["mo2"]["df2avgdf1avg"].ToString();

                //cfod
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFD] =
                    btConfigFile["cfod"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.CFD] = btConfigFile["cfod"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.CFD] = btConfigFile["cfod"]["npackets"].ToString();
                CFD.freq_accuracy = btConfigFile["cfod"]["freqacc"].ToString();
                CFD.freq_drift = btConfigFile["cfod"]["freqdrift"].ToString();
                CFD.initial_freq_drift = btConfigFile["cfod"]["initfreqdrift"].ToString();
                CFD.max_drift_rate = btConfigFile["cfod"]["maxdriftrate"].ToString();

                //cfod2
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFD2] =
                    btConfigFile["cfod2"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.CFD2] = btConfigFile["cfod2"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.CFD2] = btConfigFile["cfod2"]["npackets"].ToString();
                CFD2.freq_accuracy = btConfigFile["cfod2"]["freqacc"].ToString();
                CFD2.freq_drift = btConfigFile["cfod2"]["freqdrift"].ToString();
                CFD2.initial_freq_drift = btConfigFile["cfod2"]["initfreqdrift"].ToString();
                CFD2.max_drift_rate = btConfigFile["cfod2"]["maxdriftrate"].ToString();

                //rs
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS] =
                    btConfigFile["rs"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.RS2] = btConfigFile["rs"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.RS2] = btConfigFile["rs"]["npackets"].ToString();
                RS.rs_dirty = btConfigFile["rs"]["dirty"].ToString();
                RS.rs_level = btConfigFile["rs"]["rxlevel"].ToString();
                RS.rs_per = btConfigFile["rs"]["per"].ToString();

                //rs2
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS2] =
                    btConfigFile["rs2"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.RS] = btConfigFile["rs2"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.RS] = btConfigFile["rs2"]["npackets"].ToString();
                RS2.rs_dirty = btConfigFile["rs2"]["dirty"].ToString();
                RS2.rs_level = btConfigFile["rs2"]["rxlevel"].ToString();
                RS2.rs_per = btConfigFile["rs2"]["per"].ToString();
                // cfom
                NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFOM] = btConfigFile["cfom"]["bchecked"].ToString().Equals("True") ? true : false;
                channel[(int)eBLECaseIdx.CFOM] = btConfigFile["cfom"]["channel"].ToString();
                packetNum[(int)eBLECaseIdx.CFOM] = btConfigFile["cfom"]["npackets"].ToString();
                CFOM.maxFreqTolLower = btConfigFile["cfom"]["maxfreqtolLow"].ToString();
                CFOM.maxFreqTolUpper = btConfigFile["cfom"]["maxfreqtolUp"].ToString();
                CFOM.deltaf2avg = btConfigFile["cfom"]["deltaf2avg"].ToString();
                CFOM.deltaf2min = btConfigFile["cfom"]["deltaf2min"].ToString();

                Console.WriteLine("===== bt_le.cnf =====");
                Console.WriteLine("** bt le config folder : " + dir);
                Console.WriteLine("** baud rate : " + baudRateIdx + " : " + baudRateValueList[Int32.Parse(baudRateIdx)]);
                Console.WriteLine("** hci type 1 : " + hciTypeIdx1 + " : " + hciTypeValueList[Int32.Parse(hciTypeIdx1)]);
                Console.WriteLine("** hci type 2 : " + hciTypeIdx2 + " : " + hciTypeValueList[Int32.Parse(hciTypeIdx2)]);
                var fc = flowCtrl.Equals("0") ? "NONE" : "FLOW";
                Console.WriteLine("** flow ctrl : " + flowCtrl + " : " + fc);
                Console.WriteLine("=====================");

                Console.WriteLine("======== OP ========");
                Console.WriteLine("op - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.OP].ToString());
                Console.WriteLine("op - channel : " + channel[(int)eBLECaseIdx.OP].ToString());
                Console.WriteLine("op - npackets : " + packetNum[(int)eBLECaseIdx.OP].ToString());
                Console.WriteLine("op - pavg_up :" + OP.pavg_up);
                Console.WriteLine("op - pavg_low : " + OP.pavg_low);
                Console.WriteLine("op - power_diff : " + OP.power_diff);
                Console.WriteLine("=====================");

                Console.WriteLine("======== OP 2M ========");
                Console.WriteLine("op2 - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.OP2].ToString());
                Console.WriteLine("op2 - channel : " + channel[(int)eBLECaseIdx.OP2].ToString());
                Console.WriteLine("op2 - npackets : " + packetNum[(int)eBLECaseIdx.OP2].ToString());
                Console.WriteLine("op2 - pavg_up :" + OP2.pavg_up);
                Console.WriteLine("op2 - pavg_low : " + OP2.pavg_low);
                Console.WriteLine("op2 - power_diff : " + OP2.power_diff);
                Console.WriteLine("=====================");


                Console.WriteLine("======== MOD ========");
                Console.WriteLine("mo - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.MOD].ToString());
                Console.WriteLine("mo - channel : " + channel[(int)eBLECaseIdx.MOD].ToString());
                Console.WriteLine("mo - npackets : " + packetNum[(int)eBLECaseIdx.MOD].ToString());
                Console.WriteLine("mo - df1avgupper : " + MOD.df1avgupper);
                Console.WriteLine("mo - df1avglower : " + MOD.df1avglower);
                Console.WriteLine("mo - df2max : " + MOD.df2max);
                Console.WriteLine("mo - df2avgdf1avg : " + MOD.df2avgdf1avg);
                Console.WriteLine("=====================");

                Console.WriteLine("======== MOD2 ========");
                Console.WriteLine("mo2 - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.MOD2].ToString());
                Console.WriteLine("mo2 - channel : " + channel[(int)eBLECaseIdx.MOD2].ToString());
                Console.WriteLine("mo2 - npackets : " + packetNum[(int)eBLECaseIdx.MOD2].ToString());
                Console.WriteLine("mo2 - df1avgupper : " + MOD2.df1avgupper);
                Console.WriteLine("mo2 - df1avglower : " + MOD2.df1avglower);
                Console.WriteLine("mo2 - df2max : " + MOD2.df2max);
                Console.WriteLine("mo2 - df2avgdf1avg : " + MOD2.df2avgdf1avg);
                Console.WriteLine("=====================");

                Console.WriteLine("======= CFD =======");
                Console.WriteLine("cfod - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFD].ToString());
                Console.WriteLine("cfod - channel : " + channel[(int)eBLECaseIdx.CFD].ToString());
                Console.WriteLine("cfod - npackets : " + packetNum[(int)eBLECaseIdx.CFD]);
                Console.WriteLine("cfod - freqacc : " + CFD.freq_accuracy);
                Console.WriteLine("cfod - freqdrift : " + CFD.freq_drift);
                Console.WriteLine("cfod - initfreqdrift : " + CFD.initial_freq_drift);
                Console.WriteLine("cfod - maxdriftrate : " + CFD.max_drift_rate);
                Console.WriteLine("=====================");

                Console.WriteLine("======= CFD2 =======");
                Console.WriteLine("cfod2 - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFD2].ToString());
                Console.WriteLine("cfod2 - channel : " + channel[(int)eBLECaseIdx.CFD2].ToString());
                Console.WriteLine("cfod2 - npackets : " + packetNum[(int)eBLECaseIdx.CFD2]);
                Console.WriteLine("cfod2 - freqacc : " + CFD2.freq_accuracy);
                Console.WriteLine("cfod2 - freqdrift : " + CFD2.freq_drift);
                Console.WriteLine("cfod2 - initfreqdrift : " + CFD2.initial_freq_drift);
                Console.WriteLine("cfod2 - maxdriftrate : " + CFD2.max_drift_rate);
                Console.WriteLine("=====================");

                Console.WriteLine("======== RS =========");
                Console.WriteLine("rs - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS].ToString());
                Console.WriteLine("rs - channel : " + channel[(int)eBLECaseIdx.RS].ToString());
                Console.WriteLine("rs - npackets : " + packetNum[(int)eBLECaseIdx.RS]);
                Console.WriteLine("rs - dirty : " + RS.rs_dirty);
                Console.WriteLine("rs - rxlevel : " + RS.rs_level);
                Console.WriteLine("rs - per : " + RS.rs_per);
                Console.WriteLine("=====================");

                Console.WriteLine("======== RS2 =========");
                Console.WriteLine("rs2 - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS2].ToString());
                Console.WriteLine("rs2 - channel : " + channel[(int)eBLECaseIdx.RS2].ToString());
                Console.WriteLine("rs2 - npackets : " + packetNum[(int)eBLECaseIdx.RS2]);
                Console.WriteLine("rs2 - dirty : " + RS2.rs_dirty);
                Console.WriteLine("rs2 - rxlevel : " + RS2.rs_level);
                Console.WriteLine("rs2 - per : " + RS2.rs_per);
                Console.WriteLine("=====================");

                Console.WriteLine("======== CFOM =========");
                Console.WriteLine("cfom - bchecked : " + NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS2].ToString());
                Console.WriteLine("cfom - channel : " + channel[(int)eBLECaseIdx.RS2].ToString());
                Console.WriteLine("cfom - npackets : " + packetNum[(int)eBLECaseIdx.RS2]);
                Console.WriteLine("cfom - maxfreqtolLow : " + CFOM.maxFreqTolLower);
                Console.WriteLine("cfom - maxfreqtolUp : " + CFOM.maxFreqTolUpper);
                Console.WriteLine("cfom - deltaf2avg : " + CFOM.deltaf2avg);
                Console.WriteLine("cfom - deltag2min : " + CFOM.deltaf2min);
                Console.WriteLine("=====================");
            }
            catch (Exception e)
            {
                Util.openPopupOk("LoadBTConfig error : " + e.Message);
            }
        }

        public async Task<bool> setMeasBLE()
        {
            try
            {
                VisaManager vgr = VisaManager.Instance;

                if (!vgr.writeVisa("SYST:PROTO BT", selectedRFPort))
                    return false;

                await Task.Delay(100);
                if (!vgr.writeVisa("SYST:RES", selectedRFPort))
                    return false;
                if (!vgr.writeVisa("*CLS", selectedRFPort))
                    return false;
                if (!vgr.writeVisa("*RST", selectedRFPort))
                    return false;
                if (!vgr.writeVisa("ROUT:MEAS:PORT RF1", selectedRFPort))
                    return false;

                if (!vgr.writeVisa("CONFigure:BT:TYPE 1", selectedRFPort))
                    return false;
                
                if (!vgr.writeVisa("CONFigure:BT:LE:DUT:TEST_METHOD 0", selectedRFPort))
                    return false;
                if (!vgr.writeVisa(string.Format("CONFigure:BT:LE:DUT:HCITYPE 5"), selectedRFPort))
                    return false;
                if (!vgr.writeVisa(string.Format("CONFigure:BT:LE:DUT:UART:BAUDRATE {0}", baudRateValue), selectedRFPort))
                    return false;
                //VisaManager.Instance.writeVisa("CONF:BT:MEAS:P1_PLOSS:CHANnel CH0@1", selectedRFPort);
                //VisaManager.Instance.writeVisa("CONF:BT:MEAS:P2_PLOSS:CHANnel CH0@1", selectedRFPort);
                //VisaManager.Instance.writeVisa("CONF:BT:LE:MEAS:PHY 1", selectedRFPort);
                if (!vgr.writeVisa("CONF:MEAS:RFS:PDET:AUTO 1", selectedRFPort))
                    return false;
                if (!vgr.writeVisa("CONF:MEAS:RFS:REFLevel 4", selectedRFPort))
                    return false;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("setMeasBLE error", Color.Red);
                Util.openPopupOk("setMeasBLE - " + e.Message);
                return false;
            }
            return true;
        }
        private void setUserFirmwareDownload(bool isDw, string swv)
        {
            int r = startIndex + 2;

            if (!isUserFirmware)
            {
                mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = "N/A";
                mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = "N/A";
                mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.BackColor = Color.OrangeRed;
                mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;
                userFirmwareText.Text = "N/A";
                return;
            }

            mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = !isDw ? "FAIL" : "PASS";

            if (!isDw)
            {
                mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = "N/A";
                mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.BackColor = Color.Red;
                mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;
                userFirmwareText.Text = "N/A";
            }
            else
            {
                mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = swv;
                mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.BackColor = Color.Green;
                mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;
                userFirmwareText.Text = swv;
            }

            startIndex++;
        }
        private void setTotalTimeAndResult(bool isFail, DateTime startTime)
        {
            int idx = resultIndex;
            mainBLEGrid.Rows[idx].Cells[(int)eCol.RES].Value =
                isFail == true ? "FAIL" : "PASS";
            finalResult = isFail == true ? "FAIL" : "PASS";

            if (isFail)
            {
                mainBLEGrid.Rows[idx].Cells[(int)eCol.RES].Style.BackColor = Color.Red;
                mainBLEGrid.Rows[idx].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;
            }
            else
            {
                mainBLEGrid.Rows[idx].Cells[(int)eCol.RES].Style.BackColor = Color.Green;
                mainBLEGrid.Rows[idx].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;

            }
            var result = 0f;

            Array.ForEach(testTime, value => result += value);
            tactTime = result;
            mainBLEGrid.Rows[idx + 1].Cells[(int)eCol.RES
            ].Value = result;

            overallTime = (float)(DateTime.Now - startTime).TotalSeconds;
            mainBLEGrid.Rows[idx + 2].Cells[(int)eCol.RES].Value = overallTime;

            // 메인 화면 Test Time 표시
            testTimeText.Text = ((int)overallTime).ToString() + "[SEC]";

            mainBLEGrid.RefreshEdit();
            pgrBar.Value = prgCount += 3;
        }
        private void setCount(bool isfail)
        {
            if (!isfail)
                passCount++;
            else
                failCount++;

            rf1TotalCount.Text = (passCount + failCount + Int32.Parse(rf1TotalCount.Text)).ToString();
            rf1PassCount.Text = (passCount + Int32.Parse(rf1PassCount.Text)).ToString();
            rf1NGCount.Text = (failCount + Int32.Parse(rf1NGCount.Text)).ToString();

            string resultStr = string.Empty;

            resultStr = "rf1 total count : " + rf1TotalCount.Text + " : rf1 pass count : " + rf1PassCount.Text + " : rf1 ng count : " + rf1NGCount.Text;
            Util.addLog(resultStr, selectedRFPort);
            resultStr = "========================================================================\n";
            Util.addLog(resultStr, "-");
            Util.writeLog("RF1");
            pgrBar.Value = 0;
            Util.addLog(resultStr, "-");
            resultStr = "========================================================================\n";
            Util.addLog(resultStr, "-");
            Util.writeLog("RF1");
        }

        public static string passfailstring = "";
        public async Task runTest()
        {
            //arr = new[] { isRF1, isRF2 };

            //Invoke(new Action(async delegate ()
            //{
            for (int i = 0; i < NewSetupWindow.iterCount; i++)
            {
                await Task.Delay(2000);

                if (op == null)
                    op = new OP();
                if (op2 == null)
                    op2 = new OP2();
                if (mod == null)
                    mod = new MOD();
                if (mod2 == null)
                    mod2 = new MOD2();
                if (cfd == null)
                    cfd = new CFD();
                if (cfd2 == null)
                    cfd2 = new CFD2();
                if (rs == null)
                    rs = new RS();
                if (rs2 == null)
                    rs2 = new RS2();
                if (cfom == null)
                    cfom = new CFOM();

                startIndex = 0;
                op.clearResult();
                op2.clearResult();
                mod.clearResult();
                mod2.clearResult();
                cfd.clearResult();
                cfd2.clearResult();
                rs.clearResult();
                rs2.clearResult();
                cfom.clearResult();
                passfailstring = string.Empty;

                currStatus = eStatus.RUNNING;

                if (await initDUT())
                {
                    currStatus = eStatus.STOP;
                    setTextPgrBar("FAIL", Color.Black);
                    return;
                }

                resetMainGrid();
                startIndex = 0;
                stTime = DateTime.Now;
                Array.Clear(testTime, 0, testTime.Length);

                passCount = 0;
                failCount = 0;

                if (!await setMeasBLE()) return;

                // 테스트 준비
                Queue<List<string>> commandList = new Queue<List<string>>();
                int pgrTotalCount = 0;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.OP])
                {
                    List<string> li = new List<string>();
                    OP.opCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 7;
                }
                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.OP2])
                {
                    List<string> li = new List<string>();
                    OP2.opCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 7;
                }

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.MOD])
                {
                    List<string> li = new List<string>();
                    MOD.modCharacteristicsCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 10;
                }

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.MOD2])
                {
                    List<string> li = new List<string>();
                    MOD2.modCharacteristicsCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 10;
                }

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFD])
                {
                    List<string> li = new List<string>();
                    CFD.carrierFreqModCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 13;
                }

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFD2])
                {
                    List<string> li = new List<string>();
                    CFD2.carrierFreqModCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 13;
                }

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS])
                {
                    List<string> li = new List<string>();
                    RS.rsCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 4;
                }

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS2])
                {
                    List<string> li = new List<string>();
                    RS2.rsCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 4;
                }

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFOM])
                {
                    List<string> li = new List<string>();
                    CFOM.cfomCommands(out li);
                    commandList.Enqueue(li);
                    pgrTotalCount += 16;
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }

                if (isStop())
                {
                    return;
                }

                // 테스트 실행
                bool isFail = false;
                pgrBar.Value = 0;
                pgrBar.Maximum = pgrTotalCount + 3;
                prgCount = 0;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.OP])
                {
                    setTextPgrBar("BLE Output Power", Color.Black);

                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await op.runOP(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";


                    if (isPass)
                    {
                        //passCount++;
                    }
                    else
                    {
                        //failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int) ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.OP2])
                {
                    setTextPgrBar("BLE Output Power 2M", Color.Black);

                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await op2.runOP(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";


                    if (isPass)
                    {
                        //passCount++;
                    }
                    else
                    {
                        //failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.MOD])
                {
                    setTextPgrBar("Modulation Characteristics", Color.Black);

                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await mod.runMOD(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";


                    if (isPass)
                    {
                        //passCount++;
                    }
                    else
                    {
                        //failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.MOD2])
                {
                    setTextPgrBar("Modulation Characteristics 2M", Color.Black);

                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await mod2.runMOD(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";


                    if (isPass)
                    {
                        //passCount++;
                    }
                    else
                    {
                        //failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFD])
                {
                    //prgBar.CustomText = "Carrier Frequency Offset and Drift";
                    setTextPgrBar("Carrier Frequency Offset and Drift", Color.Black);
                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await cfd.runCFD(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";


                    if (isPass)
                    {
                        // passCount++;
                    }
                    else
                    {
                        //failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFD2])
                {
                    setTextPgrBar("Carrier Frequency Offset and Drift 2M", Color.Black);

                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await cfd2.runCFD(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";


                    if (isPass)
                    {
                        // passCount++;
                    }
                    else
                    {
                        // failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS])
                {
                    setTextPgrBar("Receiver Sensitivity", Color.Black);

                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await rs.runRS(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";


                    if (isPass)
                    {
                        //passCount++;
                    }
                    else
                    {
                        //failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.RS2])
                {
                    setTextPgrBar("Receiver Sensitivity 2M", Color.Black);

                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await rs2.runRS(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";

                    if (isPass)
                    {
                        //passCount++;
                    }
                    else
                    {
                        //failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (NewSetupWindow.bleCaseCheckBoxList[(int)eBLECaseIdx.CFOM])
                {
                    setTextPgrBar("Carrier Freq. Offset + Mod Char (preamble) 1M", Color.Black);

                    while (isPuased)
                        await Task.Delay(waitTime);

                    bool isPass = await cfom.runCFOM(commandList.Peek());
                    passfailstring += isPass == true ? "PASS|" : "FAIL|";


                    if (isPass)
                    {
                        //passCount++;
                    }
                    else
                    {
                        //failCount++;
                        isFail = true;

                        if (NewSetupWindow.isFailStop)
                        {
                            setTotalTimeAndResult(isFail, stTime);
                            setCount(isFail);
                            Util.saveReport();      // report 파일 생성 및 결과 저장
                            Util.savePassFailReport(isFail);
                            procfailStop();
                            setTextPgrBar("FAIL", Color.Black);
                            return;
                        }
                    }
                    commandList.Dequeue();
                    await Task.Delay(waitTime);
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                turnOffUSB();

                if (isUserFirmware)     // user firmware 다운로드 하기
                {
                    setTextPgrBar("Downloading userfirmware ...", Color.Black);
                    //Util.RunProc("prepare_test.exe", commanderDir, "stop:"+ serialNo);
                    //await Task.Delay(3000);
                    if (!FTDIDeviceCtrl.setOne(serialNo))
                        return;
                    if (!FTDIDeviceCtrl.setTwo(serialNo))
                        return;
                    //// get serial
                    //string path = commanderDir + @"\Flash_commander_24_0206\Flash_commander";
                    //ProcessStartInfo startInfo = new ProcessStartInfo(string.Concat(path, @"\", "commander.exe"));
                    //startInfo.Arguments = "-v";
                    //startInfo.RedirectStandardOutput = true;
                    //startInfo.UseShellExecute = false;
                    //// wrap IDisposable into using (in order to release hProcess) 
                    //commander_serial_number = string.Empty;
                    //using (Process process = new Process())
                    //{
                    //    process.StartInfo = startInfo;
                    //    process.Start();

                    //    // Add this: wait until process does its work
                    //    process.WaitForExit();

                    //    // and only then read the result
                    //    string result = process.StandardOutput.ReadToEnd();
                    //    commander_serial_number = result.Substring(result.IndexOf("SN=") + 3, result.IndexOf("USBAddr=") - result.IndexOf("SN=") - 3);

                    //    Console.WriteLine("commander.exe serial number : " + commander_serial_number);
                    //    Util.addLog("commander.exe serial number : " + commander_serial_number, "-");
                    //}


                    if (sbOn[(int)ePort.SB1])
                    {
                        if (await checkSB())
                        {
                            setTextPgrBar("STOP", Color.Black);
                            return;
                        }
                    }

                    if (isStop()) return;

                    if (userFileName == "")
                    {
                        currStatus = eStatus.STOP;
                        Util.openPopupOk("Please select a User Firmware file in the setup.");
                        isStop();
                        return;
                    }


                    if (!Util.downloadDTM(commander_serial_number, 0, userFileName))
                        return;

                    if (!Util.downloadDTM(commander_serial_number, 1, userFileName))
                        return;

                    if (!Util.downloadDTM(commander_serial_number, 2, userFileName))
                        return;
                    //Util.RunProc("startDownload", commanderDir, userFileName + ":" + serialNo);

                    if (sbOn[(int)ePort.SB1])
                    {
                        if (await checkSB())
                        {
                            setTextPgrBar("STOP", Color.Black);
                            return;
                        }
                    }

                    if (isStop()) return;

                    isGetVersion = true;
                    //isDownload = Util.checkUserFirmWareDownload();
                    isDownload = true;

                    if (sbOn[(int)ePort.SB1])
                    {
                        if (await checkSB())
                        {
                            setTextPgrBar("STOP", Color.Black);
                            return;
                        }
                    }
                    if (isStop()) return;

                    isFail = !(isDownload & !isFail);

                    var p = SerialPortManager.Instance;

                    if (isDownload)     // user firmware 다운로드 성공! sw 버전 가져오기
                    {
                        swVersion = string.Empty;
                        p.openDUT(prevDutPort, 9600, (int)ePort.DUT1);

                        while (swVersion == string.Empty)
                        {
                            setTextPgrBar("Waiting for SW Version...", Color.Black);
                            await Task.Delay(50);
                            if (sbOn[(int)ePort.SB1])
                            {
                                if (await checkSB())
                                {
                                    setTextPgrBar("STOP", Color.Black);
                                    return;
                                }
                            }
                            if (isStop()) return;
                        }

                        while (CTPMain.isGetVersion)
                        {
                            setTextPgrBar("Waiting for SW Version...", Color.Black);
                            await Task.Delay(50);
                            if (sbOn[(int)ePort.SB1])
                            {
                                if (await checkSB())
                                {
                                    setTextPgrBar("STOP", Color.Black);
                                    return;
                                }
                            }
                            if (isStop()) return;
                        }
                    }
                    setUserFirmwareDownload(isDownload, swVersion);
                    if (sbOn[(int)ePort.SB1])
                    {
                        if (await checkSB())
                        {
                            setTextPgrBar("STOP", Color.Black);
                            return;
                        }
                    }
                    if (isStop()) return;
                }

                turnOffUSB();

                //Util.RunProc("end_test", commanderDir, serialNo);
                FTDIDeviceCtrl.setFive(serialNo);

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (turnOnUSB())
                {
                    setTextPgrBar("STOP", Color.Black);
                    return;
                }

                setTotalTimeAndResult(isFail, stTime);
                setCount(isFail);

                Util.saveReport();      // report 파일 생성 및 결과 저장
                Util.savePassFailReport(isFail);
                
                vgr.writeVisa("SYST:ERR?", selectedRFPort);
                vgr.readVisa("SYST:ERR?", selectedRFPort);

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return;
                    }
                }
                if (isStop()) return;

                if (isFail)
                {
                    setTextPgrBar("FAIL", Color.Red);
                }
                else
                {
                    setTextPgrBar("PASS", Color.Green);
                }
            }

            if (sbOn[(int)ePort.SB1])
            {
                SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
            }
            currStatus = eStatus.FINISH;    // 테스트 끝
            await Task.Delay(1000);
            setTextPgrBar("READY",Color.Black);
        }

        private bool turnOnDUT()
        {
            if (!NewSetupWindow.isConnect[(int)ePort.DUT1])
            {
                bool isDut = SerialPortManager.Instance.openDUT(prevDutPort, 115200, (int)ePort.DUT1);
                NewSetupWindow.isConnect[(int)ePort.DUT1] = true;   // 메인 화면 dut 연결 표시 다시 on

                if (!isDut)
                {
                    currStatus = eStatus.STOP;
                    pgrBar.Value = 0;
                    Util.openPopupOk("Please check DUT connection.");
                    return true;
                }
            }
            return false;
        }
        private bool turnOnUSB()
        {
            if (!NewSetupWindow.isConnect[(int)ePort.DUT1])
            {
                bool isDut = SerialPortManager.Instance.openDUT(prevDutPort, 115200, (int)ePort.DUT1);
                //bool isDut = FTDIDeviceCtrl.DeviceOpen(FDTI_Type.OPEN_TYPE.SERIALNUMBER, "A9JSDGRG");
                NewSetupWindow.isConnect[(int)ePort.DUT1] = true;   // 메인 화면 dut 연결 표시 다시 on

                if (!isDut)
                {
                    currStatus = eStatus.STOP;
                    pgrBar.Value = 0;
                    Util.openPopupOk("Please check DUT connection.");
                    return true;
                }
            }

            if (sbOn[(int)ePort.SB1] && !NewSetupWindow.isConnect[(int)ePort.SB1])
            {
                bool isSB = SerialPortManager.Instance.openDUT(prevSBPort, 9600, (int)ePort.SB1);
                NewSetupWindow.isConnect[(int)ePort.SB1] = true;    // 메인 화면 SB 연결 표시 다시 on

                if (!isSB)
                {
                    currStatus = eStatus.STOP;
                    pgrBar.Value = 0;
                    Util.openPopupOk("Please check Shield Box connection.");
                    return true;
                }
            }
            return false;
        }

        private void turnOffUSB()
        {
            NewSetupWindow.isConnect[(int)ePort.DUT1] = false;
            SerialPortManager.Instance.closePort((int)ePort.DUT1);
            //FTDIDeviceCtrl.DeviceClose();

            //NewSetupWindow.isConnect[(int)ePort.SB1] = false;
            //SerialPortManager.Instance.closePort((int)ePort.SB1);
        }
        private void procfailStop()
        {
            currStatus = eStatus.STOP;
            pgrBar.Value = 0;
            setTextPgrBar("FAIL", Color.Red);
            turnOnUSB();
            SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
        }

        public async Task<bool> checkSB()
        {
            string res = string.Empty;
            bool isSB = SerialPortManager.Instance.isSBConnected("RF1");

            if (!isSB)
            {
                NewSetupWindow.connectSB("RF1", (int)ePort.SB1);
            }

            if (!SerialPortManager.Instance.writeSBCommand("lid?\r", "RF1"))
                return true;
            
            await Task.Delay(300);

            res = SerialPortManager.Instance.dutData[(int)ePort.SB1];
            if (!res.Equals("") && res.Length >= 2 && res[0] == 'O' && res[1] == 'P')      // shield box closed?
            {
                NewSetupWindow.setButtonStatus(sbBtn1, Color.Gray);  // Yellow = opened and connected
                setTextPgrBar("STOP", Color.Black);
                currStatus = eStatus.STOP;
                return true;

            }
            return false;
        }
        private async Task<bool> isSBOpen(string selectedRF)
        {
            string res = string.Empty;
            while (true)
            {
                if (currStatus == eStatus.STOP)
                {
                    string resultStr = string.Empty;
                    resultStr = "===============================STOPPED=================================\n";
                    Util.addLog(resultStr, "-");
                    Util.writeLog("RF1");
                    setTextPgrBar("STOP", Color.Red);
                    SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
                    return false;
                }

                if (!SerialPortManager.Instance.writeSBCommand("lid?\r", selectedRF))
                {
                    currStatus = eStatus.STOP;
                    string resultStr = string.Empty;
                    resultStr = "===========================BOX DISCONNECTED============================\n";
                    Util.addLog(resultStr, "-");
                    Util.writeLog("RF1");
                    setTextPgrBar("STOP", Color.Red);
                    SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
                    return false;
                }

                await Task.Delay(500);

                res = SerialPortManager.Instance.dutData[(int)ePort.SB1];
                if (!res.Equals("") && res[0] == 'C')      // shield box closed?
                {
                    NewSetupWindow.setButtonStatus(sbBtn1, Color.Green); // green = closed and connected            
                    break;
                }
                NewSetupWindow.setButtonStatus(sbBtn1, Color.Gray);  // Yellow = opened and connected
                setTextPgrBar("Close Shield Box!", Color.Black);

                //if (!sbOn[(int)ePort.SB1]) // 수동으로 전환
                //{
                //    setTextPgrBar("READY", Color.AliceBlue);
                //    break;
                //}

                Form fc = Application.OpenForms["NewSetupWindow"];

                if (fc != null)
                    break;
            }

            return true;
        }
        private bool isStop()
        {
            if (currStatus == eStatus.STOP)
            {
                string resultStr = string.Empty;
                resultStr = "===============================STOPPED=================================\n";
                Util.addLog(resultStr, "-");
                Util.writeLog("RF1");

                NewSetupWindow.isConnect[(int)ePort.DUT1] = false;
                SerialPortManager.Instance.closePort((int)ePort.DUT1);

                NewSetupWindow.isConnect[(int)ePort.SB1] = false;
                SerialPortManager.Instance.closePort((int)ePort.SB1);

                //Util.RunProc("turnoff.exe", commanderDir, serialNo);
                pgrBar.Value = 0;
                setTextPgrBar("STOP", Color.Red);
                turnOnUSB();
                SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
                return true;
            }

            return false;
        }
        private void startButton_Click(object sender, EventArgs e)
        {
            if (!sbOn[(int)ePort.SB1]) // shield box1 usage check
            {
                return;
            }

            if (currStatus != eStatus.READY)
            {
                Util.openPopupOk("Test in progress...");
                return;
            }

            if (commanderDir == "")
            {
                Util.openPopupOk("Please select Commander path in the Setup Window.");
                return;
            }

            if (serialNo == "")
            {
                Util.openPopupOk("Please Select DUT's serial number in the setting.");
                return;
            }

            if (isDTMDown && dtmFileName == "")
            {
                Util.openPopupOk("Please select a DTM file in the setup.");
                return;
            }
            
            if (isUserFirmware && userFileName == "")
            {
                Util.openPopupOk("Please select a User Firmware file in the setup.");
                return;
            }

            if (sbOn[(int) ePort.SB1] && !NewSetupWindow.isConnect[(int)ePort.SB1])
            {
                Util.openPopupOk("Please check Shield Box connection.");
                setTextPgrBar("READY", Color.AliceBlue);
                currStatus = eStatus.READY;
                SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
                return;
            }

            if (!NewSetupWindow.isMtp300Connect)
            {
                Util.openPopupOk("Please check MTP300 connection.");
                setTextPgrBar("READY", Color.AliceBlue);
                currStatus = eStatus.READY;
                SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
                return;
            }

            if (!NewSetupWindow.isConnect[(int)ePort.DUT1])
            {
                Util.openPopupOk("Please Check DUT connection.");
                return;
            }

            Invoke(new Action(async delegate ()
            {
                turnOnUSB();

                if (sbOn[(int)ePort.SB1]) // shield box1 usage check
                {
                    // 무한 루프
                    while (currStatus != eStatus.STOP)
                    {
                        bool isClosed = await isSBOpen("RF1");

                        Form fc = Application.OpenForms["NewSetupWindow"];

                        if (fc != null)
                        {
                            setTextPgrBar("READY", Color.AliceBlue);
                            break;
                        }

                        if (currStatus == eStatus.STOP)
                        {
                            currStatus = eStatus.READY;
                            return;
                        }

                        if (VisaManager.Instance.isConnected())
                        {
                            await runTest();

                            if (currStatus == eStatus.STOP)
                            {
                                currStatus = eStatus.READY;
                                return;
                            }
                        }

                        if (currStatus == eStatus.FINISH)
                        {
                            currStatus = eStatus.READY;
                        }
                    }

                    if (currStatus == eStatus.STOP)
                    {
                        setTextPgrBar("READY", Color.AliceBlue);
                        currStatus = eStatus.READY;
                    }
                }
            }));
        }

        private async Task<bool> initDUT()
        {
            try
            {
                isGetBDAddr = false;
                isGetVersion = false;

                setTextPgrBar("Testing", Color.Black);
                clearMainGrid();
                resetBDaddr();
                resetSWVersion();
                turnOffUSB();

                //string arg = isDTMDown ? "start" : "stop";

                if (isDTMDown)
                {
                    isGetBDAddr = true;
                    setTextPgrBar("Downloading DTM ...", Color.Black);

                    if (dtmFileName == "")
                    {
                        currStatus = eStatus.STOP;
                        Util.openPopupOk("Please select a DTM file in the setup.");
                        isStop();
                        return true;
                    }
                    //Util.RunProc("prepare_test.exe", commanderDir, dtmFileName + ":" + serialNo);

                    if (!FTDIDeviceCtrl.setOne(serialNo))
                        return true;
                    if (!FTDIDeviceCtrl.setTwo(serialNo))
                        return true;

                    // get serial
                    string path = commanderDir;
                    ProcessStartInfo startInfo = new ProcessStartInfo(string.Concat(path, @"\", "commander.exe"));
                    startInfo.Arguments = "-v";
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    // wrap IDisposable into using (in order to release hProcess) 
                    commander_serial_number = string.Empty;
                    using (Process p = new Process())
                    {
                        p.StartInfo = startInfo;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;

                        p.Start();

                        // Add this: wait until process does its work
                        p.WaitForExit();

                        // and only then read the result
                        string result = p.StandardOutput.ReadToEnd();
                        Console.WriteLine("result : " + result);
                        commander_serial_number = result.Substring(result.IndexOf("SN=") + 3, result.IndexOf("USBAddr=") - result.IndexOf("SN=") - 3);

                        if (commander_serial_number == string.Empty)
                        {
                            Util.openPopupOk("Could not read Commander Serial Number.");
                            return true;
                        }

                        Console.WriteLine("commander.exe serial number : " + commander_serial_number);
                        Util.addLog("commander.exe serial number : " + commander_serial_number, "-");
                    }
                    // download dtm
                    if (!Util.downloadDTM(commander_serial_number, 0, dtmFileName))
                        return true;
                    
                    if (!Util.downloadDTM(commander_serial_number, 1, dtmFileName))
                        return true;
                    
                    if (!Util.downloadDTM(commander_serial_number, 2, dtmFileName))
                        return true;

                }
                else
                {
                    isGetBDAddr = true;
                    //Util.RunProc("prepareNoDTM", commanderDir, serialNo);
                    if (!FTDIDeviceCtrl.setOne(serialNo))
                        return true;
                    if (!FTDIDeviceCtrl.setTwo(serialNo))
                        return true;
                }

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return true;
                    }
                }
                if (isStop()) return true;

                if (prevDutPort == string.Empty)
                {
                    currStatus = eStatus.STOP;
                    pgrBar.Value = 0;
                    Util.openPopupOk("Please check DUT connection.");
                    turnOnUSB();
                    SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
                    return true;
                }
                if (prevSBPort == string.Empty)
                {
                    currStatus = eStatus.STOP;
                    pgrBar.Value = 0;
                    Util.openPopupOk("Please check Shield Box connection.");
                    turnOnUSB();
                    SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
                    return true;
                }

                if (turnOnUSB()) return true;
                //bool isDut = SerialPortManager.Instance.openDUT("COM13", 115200, (int)ePort.DUT1);

                //NewSetupWindow.isConnect[(int)ePort.DUT1] = true;
                DateTime st = DateTime.Now;

                while (bdAddr.Length < 14 && (DateTime.Now - st).TotalSeconds < 25)
                {
                    setTextPgrBar("Getting BD addr", Color.Black);
                    if (sbOn[(int)ePort.SB1])
                    {
                        if (await checkSB())
                        {
                            setTextPgrBar("STOP", Color.Black);
                            return true;
                        }
                    }
                    if (isStop()) return true;
                    await Task.Delay(200);
                }

                isGetBDAddr = false;

                bdAddrLabel.Text = bdAddr;
                
                if (bdAddr == "")
                {
                    Util.openPopupOk("BD address time out.");
                    return true;
                }

                NewSetupWindow.isConnect[(int)ePort.DUT1] = false;
                SerialPortManager.Instance.closePort((int)ePort.DUT1);

                //NewSetupWindow.isConnect[(int)ePort.SB1] = false;
                //SerialPortManager.Instance.closePort((int)ePort.SB1);

                //Util.RunProc("turnoff.exe", commanderDir, serialNo);
                if (!FTDIDeviceCtrl.setFour(serialNo))
                    return true;

                if (sbOn[(int)ePort.SB1])
                {
                    if (await checkSB())
                    {
                        setTextPgrBar("STOP", Color.Black);
                        return true;
                    }
                }

                setTextPgrBar("Preparing...", Color.Black);

                await Task.Delay(1000);
                return false;
            }
            catch (Exception e)
            {
                Util.openPopupOk("Init DUT error - " + e.Message);
                return true;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            //if (currStatus == eStatus.READY) return;
            currStatus = eStatus.STOP;
        }

        private void setupButton_Click(object sender, EventArgs e)
        {
            if (currStatus == eStatus.RUNNING)
            {
                Util.openPopupOk("Test in progress...");
                return;
            }

            var wnd = new LoginWindow();
            wnd.ShowDialog();

            if (wnd.DialogResult == DialogResult.OK)
                new NewSetupWindow().ShowDialog();
        }

        private void logButton_Click(object sender, EventArgs e)
        {
            if (currStatus == eStatus.RUNNING)
            {
                Util.openPopupOk("Test in progress...");
                return;
            }
            Util.openFolder(logFolder);
        }

        private void reportButton_Click(object sender, EventArgs e)
        {
            if (currStatus == eStatus.RUNNING)
            {
                Util.openPopupOk("Test in progress...");
                return;
            }
            Util.openFolder(resultFolder);
        }
        private void pathlossBtn_Click(object sender, EventArgs e)
        {
            if (currStatus == eStatus.RUNNING)
            {
                Util.openPopupOk("Test in progress...");
                return;
            }
            new PathLossSetupWindow().ShowDialog(this);
        }

        public void setTextPgrBar(string s, Color c)
        {
            pgrLabel.Text = s;
            pgrLabel.ForeColor = c;

            if (s == "READY")
            {
                statusMainButton.BackColor = Color.FromArgb(0, 126, 222);
                statusMainButton.ForeColor = Color.White;
                statusMainButton.Text = s;
            }
            else if (s == "Close Shield Box!")
            {
                statusMainButton.BackColor = Color.FromArgb(0, 126, 222);
                statusMainButton.ForeColor = Color.White;
                statusMainButton.Text = s;
            }
            else if (s == "Testing")
            {
                statusMainButton.BackColor = Color.Yellow;
                statusMainButton.ForeColor = Color.Black;
                statusMainButton.Text = s;
            }
            else if (s == "PASS")
            {
                statusMainButton.BackColor = Color.Green;
                statusMainButton.ForeColor = Color.Black;
                statusMainButton.Text = s;
            }
            else if (s == "FAIL")
            {
                statusMainButton.BackColor = Color.Red;
                statusMainButton.ForeColor = Color.Black;
                statusMainButton.Text = s;
            }
            else if (s == "-")
            {
                statusMainButton.BackColor = Color.FromArgb(0, 126, 222);
                statusMainButton.ForeColor = Color.White;
                statusMainButton.Text = s;
            }
            else if (s == "STOP")
            {
                statusMainButton.BackColor = Color.Red;
                statusMainButton.ForeColor = Color.Black;
                statusMainButton.Text = s;
            }
            Refresh();
        }

        private void showProgressList()
        {
            if (this.Size.Width > 876)
            {
                showDetailButton.Text = "\u25B6"; // right arrow   
                this.Size = new Size(876, 630);
                return;
            }

            var wnd = new LoginWindow();
            wnd.ShowDialog();

            if (wnd.DialogResult == DialogResult.OK)
            {
                showDetailButton.Text = "\u25C0"; // left arrow
                this.Size = new Size(1850, 630);
            }
        }

        private void showDetailButton_Click(object sender, EventArgs e)
        {
            if (currStatus == eStatus.RUNNING)
            {
                Util.openPopupOk("Test in progress...");
                return;
            }

            showProgressList();
        }

        private void statusMainButton_Click(object sender, EventArgs e)
        {
            if (sbOn[(int)ePort.SB1])
                return;

            if (currStatus != eStatus.READY)
            {
                Util.openPopupOk("Test in progress...");
                return;
            }

            if (commanderDir == "")
            {
                Util.openPopupOk("Please select Commander path in the Setup Window.");
                return;
            }

            if (serialNo == "")
            {
                Util.openPopupOk("Please Select DUT's serial number in the setting.");
                return;
            }

            if (sbOn[(int)ePort.SB1] && !NewSetupWindow.isConnect[(int)ePort.SB1])
            {
                Util.openPopupOk("Please check Shield Box connection.");
                setTextPgrBar("READY", Color.AliceBlue);
                currStatus = eStatus.READY;
                SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
                return;
            }

            //if (!NewSetupWindow.isMtp300Connect)
            //{
            //    Util.openPopupOk("Please check MTP300 connection.");
            //    setTextPgrBar("READY", Color.AliceBlue);
            //    currStatus = eStatus.READY;
            //    SerialPortManager.Instance.writeSBCommand("open\r", "RF1");
            //    return;
            //}

            if (!NewSetupWindow.isConnect[(int)ePort.DUT1])
            {
                Util.openPopupOk("Please Check DUT connection.");
                return;
            }

            Invoke(new Action(async delegate ()
            {
                turnOnUSB();

                NewSetupWindow.setButtonStatus(sbBtn1, Color.Gray); // Gray = SB is not needed.    

                //if (VisaManager.Instance.isConnected())
                {
                    await runTest();
                }

                currStatus = eStatus.READY;
            }));
        }

        public void keyDownMainBLE(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                mainBLEGrid.ClearSelection();
                e.SuppressKeyPress = true;
            }
        }

        private void CTPMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                statusMainButton_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }
    }
}
