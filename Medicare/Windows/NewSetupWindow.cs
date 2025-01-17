using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommFTDI;
using Medicare.BLE;
using Medicare.Main;
using Medicare.Port;
using Medicare.Utility;
using Medicare.Visa;
using Microsoft.Win32;

namespace Medicare.Setup
{
    public enum ePort
    {
        SB1 = 0,
        SB2 = 1,
        DUT1,
        DUT2,
        //MTP300 = 4,
        SC1,
        SC2,
        SERIALNO,
        TEST,
        MAX
    }
    enum ePortStatus
    {
        SCAN_START = 0,
        SCAN_STOP
    }
    public partial class NewSetupWindow : Form
    {
        //[DllImport("kernel32")]
        //private static extern long WritePrivateProfileString(string section, string key, string val, string filepath);

        //[DllImport("kernel32")]
        //private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder reVal,
        //    int size, string filepath);

        //public delegate void setSBButton(string selectedRF, Color c);
        //public static setSBButton setSBButtonCallBack = null;

        public delegate void setResultBtn(string selectedRF, Color c);
        public static setResultBtn setResultButtonCallBack = null;
        public static string[] devicePorts = new string[(int)ePort.MAX];

        public static bool[] isConnect = new bool[] { false, false, false, false, false, false, false, false };
        public static bool isMtp300Connect = false;
        public static string modelName = string.Empty;
        public static int measScenario = -1;
        public static int retryNum = -1;
        public static bool isScannerEnable = false;
        public static bool isLogAutoSave = false;
        public static int iterCount = -1;
        public static bool isFailStop = true;
        public static Button[] connectButtonList = new Button[(int)ePort.MAX];
        public static bool[] bleCaseCheckBoxList = new bool[(int)eBLECaseIdx.MAX];
        public static string selectedModelIdx = "0";
        public static bool isIterationMode = false;
        // MES setting 
        public static string mesUse = string.Empty;
        public static string mesDBName = string.Empty;
        public static string mesIP = string.Empty;
        public static string mesPort = string.Empty;
        public static string mesID = string.Empty;
        public static string mesPwd = string.Empty;

        private List<string> ports = new List<string>();
        private string fileName = "setup.csv";
        private ComboBox[] comboBoxList = new ComboBox[(int)ePort.MAX];
        private ePortStatus portStatus = ePortStatus.SCAN_STOP;
        private static SerialPortManager sgr = null;

        public NewSetupWindow()
        {
            try
            {
                InitializeComponent();

                // load cnf
                //CTPMain.loadCnf();
                CTPMain.loadBtConfig();

                StartPosition = FormStartPosition.CenterScreen;

                tabControl.SelectTab("interfaceTab");

                initColor();
                initInterfaceTab();
                initBleDtmTab();
                initGeneralTab();
                
                sgr = SerialPortManager.Instance;
                
                // FTDI 씨리얼 넘버 drop box 설정
                if (isConnect[(int)ePort.DUT1])
                {
                    foreach (var ftdiDevice in FTDIDeviceCtrl.GetDeviceList())
                    {
                        string str = ftdiDevice.ComPort + ":" + ftdiDevice.SerialNumber;
                        Console.WriteLine(str);
                        serialNoComboBox1.Items.Add(str);
                    }

                    if (serialNoComboBox1.Items.Count == 0 && CTPMain.serialNo != string.Empty)
                    {
                        serialNoComboBox1.Items.Add(devicePorts[(int)ePort.DUT1] + ":" + CTPMain.serialNo);
                        serialNoComboBox1.SelectedIndex = 0;
                        serialNoComboBox1.Enabled = false;
                        isConnect[(int)ePort.SERIALNO] = true;
                    }
                    else if (serialNoComboBox1.Items.Count != 0 && CTPMain.serialNo == string.Empty)
                    {
                        serialNoComboBox1.Enabled = true;
                        isConnect[(int)ePort.SERIALNO] = false;
                    }
                }
                else
                {
                    serialNoComboBox1.Enabled = true;
                    isConnect[(int)ePort.SERIALNO] = false;
                }
                setConnectButtonText(ePort.SERIALNO);

                Task.Run(() =>      // MTP300 indicator check
                {
                    while (true)
                    {
                        Thread.Sleep(300);

                        if (mtp300ConnectButton.InvokeRequired)
                        {
                            if (!VisaManager.Instance.isConnected())
                            {
                                mtp300ConnectButton.BeginInvoke(new Action(() => {
                                    //VisaManager.Instance.closeVisaSession();
                                    isMtp300Connect = false;
                                    CTPMain.isVisaOpen = false;
                                    mtp300ConnectButton.Text = "CONNECT";
                                    mtp300ConnectButton.BackColor = Color.FromArgb(166, 175, 190);
                                }));
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                CTPMain.wnd.setTextPgrBar("Cannot open setting window", Color.Red);
                Util.openPopupOk("Error : " + ex.Message);
            }
        }

        private void initMesTab()
        {
            //mesUseCheckBox.Checked = mesUse.Equals("True");
            //mesDBNameTextBox.Text = mesDBName;
            //mesIPTextBox.Text = mesIP;
            //mesPortTextBox.Text = mesPort;
            //mesIDtextBox.Text = mesID;
            //mesPwdtextBox.Text = mesPwd;

            //mesIPTextBox.Text = "192.168.110.20";
            //mesPortTextBox.Text = "1433";
            //mesIPTextBox.Text = "TMPARK-PC";
            //mesPortTextBox.Text = "3306";
            //mesIDtextBox.Text = "ITMV_KTNG";
            //mesPwdtextBox.Text = "!itm@semi!12";
            //mesPwdtextBox.Text = "1234";

            //bool isConnected = DBManager.Instance.connectDB(mesIPTextBox.Text, dbNameTextBox.Text, mesIDtextBox.Text, mesPwdtextBox.Text);
        }
        private void initInterfaceTab()
        {
            connectButtonList[0] = sbConnectButton1;
            //connectButtonList[1] = sbConnectButton2;
            connectButtonList[2] = dutConnectButton1;
            //connectButtonList[3] = dutConnectButton2;
            //connectButtonList[4] = scannerConnectButton1;
            //connectButtonList[5] = scannerConnectButton2;
            connectButtonList[(int)ePort.SERIALNO] = serialNoSelectButton;

            comboBoxList[0] = sbComboBox1;
            //comboBoxList[1] = sbComboBox2;
            comboBoxList[2] = dutComboBox1;

            comboBoxList[(int)ePort.SERIALNO] = serialNoComboBox1;
            //comboBoxList[3] = dutComboBox2;
            //comboBoxList[4] = scannerComboBox1;
            //comboBoxList[5] = scannerComboBox2;

            scanPort();

            if (!SerialPortManager.Instance.isPortOpen((int)ePort.SB1))
            {
                isConnect[(int)ePort.SB1] = false;
            }

            if (!SerialPortManager.Instance.isPortOpen((int)ePort.DUT1))
            {
                isConnect[(int)ePort.DUT1] = false;
            }

            for (int i = 0; i < sbComboBox1.Items.Count; i++)
            {
                if (sbComboBox1.Items[i].Equals(devicePorts[(int)ePort.SB1]))
                {
                    sbComboBox1.SelectedIndex = i;
                    break;
                }
            }

            //for (int i = 0; i < sbComboBox2.Items.Count; i++)
            //{
            //    if (sbComboBox2.Items[i].Equals(devicePorts[(int)ePort.SB2]))
            //    {
            //        sbComboBox2.SelectedIndex = i;
            //        break;
            //    }
            //}

            for (int i = 0; i < dutComboBox1.Items.Count; i++)
            {
                if (dutComboBox1.Items[i].Equals(devicePorts[(int)ePort.DUT1]))
                {
                    dutComboBox1.SelectedIndex = i;
                    break;
                }
            }

            //for (int i = 0; i < dutComboBox2.Items.Count; i++)
            //{
            //    if (dutComboBox2.Items[i].Equals(devicePorts[(int)ePort.DUT2]))
            //    {
            //        dutComboBox2.SelectedIndex = i;
            //        break;
            //    }
            //}

            //for (int i = 0; i < scannerComboBox1.Items.Count; i++)
            //{
            //    if (scannerComboBox1.Items[i].Equals(devicePorts[(int)ePort.SC1]))
            //    {
            //        scannerComboBox1.SelectedIndex = i;
            //        break;
            //    }
            //}

            //for (int i = 0; i < scannerComboBox2.Items.Count; i++)
            //{
            //    if (scannerComboBox2.Items[i].Equals(devicePorts[(int)ePort.SC2]))
            //    {
            //        scannerComboBox2.SelectedIndex = i;
            //        break;
            //    }
            //}

            //for (int i = (int)ePort.SB1; i <= (int)ePort.DUT1; i++)
            //{
            //if (isConnect[i] && i != (int) ePort.MTP300)
            //setConnectButtonText((ePort)i);
            //}
            setConnectButtonText(ePort.DUT1);
            setConnectButtonText(ePort.SB1);

            if (!VisaManager.ip.Equals(""))
            {
                mtp300TextBox.Text = VisaManager.ip;

                bool isOpen = false;
                if (Util.IsCheckNetwork())
                    isOpen = VisaManager.Instance.openVisaSession();

                if (isOpen)
                {
                    CTPMain.wnd.mtp300Btn.BackColor = Color.Green;
                    //isConnect[(int)ePort.MTP300] = true;
                    isMtp300Connect = true;
                    mtp300ConnectButton.Text = "Connected";
                    mtp300ConnectButton.BackColor = Color.Green;
                }
                else
                {
                    CTPMain.wnd.mtp300Btn.BackColor = Color.Red;
                    //isConnect[(int)ePort.MTP300] = false;
                    isMtp300Connect = false;
                    mtp300ConnectButton.Text = "Connect";
                    mtp300ConnectButton.BackColor = Color.FromArgb(166,175,190);
                }
            }
            else
            {
                CTPMain.wnd.mtp300Btn.BackColor = Color.Red;
                //isConnect[(int)ePort.MTP300] = false;
                isMtp300Connect = false;
                mtp300ConnectButton.Text = "Connect";
                mtp300ConnectButton.BackColor = Color.FromArgb(166,175,190);
            }

            //initGeneralTab();

            //if (isConnect[(int)ePort.MTP300])
            if (isMtp300Connect)
            {
                mtp300ConnectButton.Text = "Connected";
                mtp300ConnectButton.BackColor = Color.Green;
            }
            else
            {
                mtp300ConnectButton.Text = "Connect";
                mtp300ConnectButton.BackColor = Color.FromArgb(166,175,190);
            }

            sbBoxOn1.Checked = false;
            //sbBoxOn2.Checked = false;

            if (CTPMain.sbOn[(int)ePort.SB1])
                sbBoxOn1.Checked = true;

            //if (CTPMain.sbOn[(int)ePort.SB2])
            //    sbBoxOn2.Checked = true;
            bitbangPathLabel.Text = CTPMain.commanderDir == "" ? "N/A" : CTPMain.commanderDir;

            //CTPMain.prevDutPort = devicePorts[(int)ePort.DUT1];
        }
        private void initGeneralTab()
        {
            // General Tab
            runModeComboBox.Items.Add("Infinity");
            runModeComboBox.Items.Add("Iteration");

            failStopComboBox.Items.Add("ON");
            failStopComboBox.Items.Add("OFF");

            for (int i = 0; i < 100; i++)
                iterComboBox.Items.Add((i + 1).ToString());

            // default "infinity"
            //iterComboBox.SelectedIndex = runModeComboBox.SelectedIndex = 0;

            // default iterComboBox visible false
            iterComboBox.Visible = false;

            if (isIterationMode) // iteration mode
            {
                runModeComboBox.SelectedIndex = 1;
                iterComboBox.SelectedIndex = iterCount - 1;
                iterComboBox.Visible = true;
            }
            else  // infinity mode
            {
                runModeComboBox.SelectedIndex = 0;
                iterComboBox.SelectedIndex = 0;
                iterCount = 0;
            }

            // default isFailStop true, "ON"
            //failStopComboBox.SelectedIndex = 0;

            if (isFailStop)
                failStopComboBox.SelectedIndex = 0;
            else
                failStopComboBox.SelectedIndex = 1;

            //modelComboBox.Items.Add("ZEUS");
            //modelComboBox.Items.Add("ZEUS2");
            //modelComboBox.Items.Add("ZEUS3");
            //modelComboBox.Items.Add("ZEUS4");
            //modelComboBox.SelectedIndex = 0;

            userFirmWareCheckBox.Checked = CTPMain.isUserFirmware;
            dtmDownCheckBox.Checked = CTPMain.isDTMDown;
            dtmFilenameLabel.Text = CTPMain.dtmFileName;
            userfirmwareFileNameLabel.Text = CTPMain.userFileName;
        }

        void initColor()
        {
            mtp300Label.BackColor = Color.FromArgb(0, 126, 222);
            mtp300Label.ForeColor = Color.White;

            sbLabel.BackColor = Color.FromArgb(0, 126, 222);
            sbLabel.ForeColor = Color.White;

            dutComponentSetupLabel.BackColor = Color.FromArgb(0, 126, 222);
            dutComponentSetupLabel.ForeColor = Color.White;

            bigbangPathLabel.BackColor = Color.FromArgb(0, 126, 222);
            bigbangPathLabel.ForeColor = Color.White;

            bitbangPathButton.BackColor = Color.FromArgb(0, 126, 222);
            bitbangPathButton.ForeColor = Color.White;


            okButton.BackColor = Color.FromArgb(0, 54, 105);
            okButton.ForeColor = Color.White;


            cancelButton.BackColor = Color.FromArgb(0, 54, 105);
            cancelButton.ForeColor = Color.White;

            dtmInterfaceLabel.BackColor = Color.FromArgb(0, 126, 222);
            dtmInterfaceLabel.ForeColor = Color.White;

            testCasesLabel.BackColor = Color.FromArgb(0, 126, 222);
            testCasesLabel.ForeColor = Color.White;

            mtp300ConnectButton.BackColor = Color.FromArgb(166, 175, 190);
            sbConnectButton1.BackColor = Color.FromArgb(166, 175, 190);
            rescanSBButton.BackColor = Color.FromArgb(166, 175, 190);
            dutConnectButton1.BackColor = Color.FromArgb(166, 175, 190);
            rescanScannerButton.BackColor = Color.FromArgb(166, 175, 190);

            dutScanButton.BackColor = Color.FromArgb(166, 175, 190);
            serialNoSelectButton.BackColor = Color.FromArgb(166, 175, 190);


            opButton.BackColor = Color.FromArgb(166, 175, 190);
            op2Button.BackColor = Color.FromArgb(166, 175, 190);
            moButton.BackColor = Color.FromArgb(166, 175, 190);
            mo2Button.BackColor = Color.FromArgb(166, 175, 190);
            cfodButton.BackColor = Color.FromArgb(166, 175, 190);
            cfod2Button.BackColor = Color.FromArgb(166, 175, 190);
            rsButton.BackColor = Color.FromArgb(166, 175, 190);
            rs2Button.BackColor = Color.FromArgb(166, 175, 190);
            cfomButton.BackColor = Color.FromArgb(166, 175, 190);

            ctpInfoLabel.BackColor = Color.FromArgb(0, 126, 222);
            ctpInfoLabel.ForeColor = Color.White;

            runModeLabel.BackColor = Color.FromArgb(0, 126, 222);
            runModeLabel.ForeColor = Color.White;

            dtmuserfirmwareLabel.BackColor = Color.FromArgb(0, 126, 222);
            dtmuserfirmwareLabel.ForeColor = Color.White;

            newpasswdLabel.BackColor = Color.FromArgb(0, 126, 222);
            newpasswdLabel.ForeColor = Color.White;

            dtmFirmwareChangeButton.BackColor = Color.FromArgb(0, 54, 105);
            dtmFirmwareChangeButton.ForeColor = Color.White;

            userFirmwareChangeButton.BackColor = Color.FromArgb(0, 54, 105);
            userFirmwareChangeButton.ForeColor = Color.White;

            failStopLabel.BackColor = Color.FromArgb(0, 126, 222);
            failStopLabel.ForeColor = Color.White;

            changePwdButton.BackColor = Color.FromArgb(0, 54, 105);
            changePwdButton.ForeColor = Color.White;

            interfaceTab.BackColor = Color.FromArgb(235, 236, 238);
            bleTab.BackColor = Color.FromArgb(235, 236, 238);
            generalTab.BackColor = Color.FromArgb(235, 236, 238);
        }

        private void rescanShieldBoxPorts(object sender, EventArgs e)
        {
            if (portStatus == ePortStatus.SCAN_START)
            {
                Util.openPopupOk("Scanning ...");
                return;
            }

            scanPort();
        }

        private void rescanDUTPorts(object sender, EventArgs e)
        {
            if (portStatus == ePortStatus.SCAN_START)
            {
                Util.openPopupOk("Scanning ...");
                return;
            }

            scanPort();
        }

        private void rescanScannerPorts(object sender, EventArgs e)   // TODO
        {

        }
        private void setConnectButtonText(ePort e)
        {
            if (isConnect[(int)e])
            {
                if (e != ePort.SERIALNO)
                    connectButtonList[(int)e].Text = "Connected";
                else
                {
                    connectButtonList[(int)e].Text = "Selected";
                }

                connectButtonList[(int)e].BackColor = Color.Green;
            }
            else
            {
                if (e != ePort.SERIALNO)
                {
                    comboBoxList[(int)e].SelectedIndex = 0;
                    connectButtonList[(int)e].Text = "Connect";
                }
                else
                    connectButtonList[(int)e].Text = "Select";

                connectButtonList[(int)e].BackColor = Color.FromArgb(166,175,190);
            }
        }
        private void scanPort()
        {
            if (portStatus == ePortStatus.SCAN_START)
            {
                Util.openPopupOk("Scanning ...");
                return;
            }

            portStatus = ePortStatus.SCAN_START;

            sbComboBox1.Items.Clear();
            //sbComboBox2.Items.Clear();
            dutComboBox1.Items.Clear();
            //dutComboBox2.Items.Clear();
            //scannerComboBox1.Items.Clear();
            //scannerComboBox2.Items.Clear();
            ports.Clear();

            using (ManagementClass i_Entity = new ManagementClass("Win32_PnPEntity"))
            {
                foreach (ManagementObject i_Inst in i_Entity.GetInstances())
                {
                    Object o_Guid = i_Inst.GetPropertyValue("ClassGuid");
                    if (o_Guid == null || o_Guid.ToString().ToUpper() != "{4D36E978-E325-11CE-BFC1-08002BE10318}")
                        continue; // Skip all devices except device class "PORTS"

                    String s_Caption = i_Inst.GetPropertyValue("Caption").ToString();
                    String s_DeviceID = i_Inst.GetPropertyValue("PnpDeviceID").ToString();
                    String s_RegPath = "HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Enum\\" + s_DeviceID + "\\Device Parameters";
                    String s_PortName = Registry.GetValue(s_RegPath, "PortName", "").ToString();

                    int s32_Pos = s_Caption.IndexOf(" (COM");
                    if (s32_Pos > 0) // remove COM port from description
                        s_Caption = s_Caption.Substring(0, s32_Pos);
                    ports.Add(s_PortName);
                }
            }
            ports.Sort();

            sbComboBox1.Items.Add("EMPTY");
            //sbComboBox2.Items.Add("EMPTY");
            dutComboBox1.Items.Add("EMPTY");
            //dutComboBox2.Items.Add("EMPTY");
            //scannerComboBox1.Items.Add("EMPTY");
            //scannerComboBox2.Items.Add("EMPTY");

            foreach (var port in ports)
            {
                sbComboBox1.Items.Add(port);
                //sbComboBox2.Items.Add(port);
                dutComboBox1.Items.Add(port);
                //dutComboBox2.Items.Add(port);
                //scannerComboBox1.Items.Add(port);
                //scannerComboBox2.Items.Add(port);
            }

            portStatus = ePortStatus.SCAN_STOP;
        }

        private void save()
        {
            var modelName = CTPMain.modelFolderName[Int32.Parse(selectedModelIdx)].Substring(1);
            CTPMain.setupFile["common"]["model_name"] = modelName;
            CTPMain.setupFile["common"]["meas_scenario"] = measScenario;
            CTPMain.setupFile["common"]["retry_num"] = retryNum;
            CTPMain.setupFile["common"]["scanner_enable"] = isScannerEnable.ToString();
            CTPMain.setupFile["common"]["log_autosave"] = isLogAutoSave.ToString();
            CTPMain.setupFile["common"]["iteration_mode"] = isIterationMode.ToString();
            CTPMain.setupFile["common"]["iteration_count"] = iterCount;
            CTPMain.setupFile["common"]["mtp300_ip"] = VisaManager.ip;

            CTPMain.setupFile["common"]["box_check"] = CTPMain.sbOn[(int)ePort.SB1].ToString() + "," + CTPMain.sbOn[(int)ePort.SB2].ToString();
            CTPMain.setupFile["common"]["box_port"] =
                devicePorts[(int)ePort.SB1].ToString() + "," + devicePorts[(int)ePort.SB2];
            CTPMain.setupFile["common"]["dut_port"] = devicePorts[(int)ePort.DUT1] + "," + devicePorts[(int)ePort.DUT2];
            CTPMain.prevDutPort = devicePorts[(int)ePort.DUT1];
            CTPMain.prevSBPort = devicePorts[(int)ePort.SB1];

            CTPMain.setupFile["common"]["sc_port"] = devicePorts[(int)ePort.SC1] + "," + devicePorts[(int)ePort.SC2];
            CTPMain.setupFile["common"]["isFailStop"] = isFailStop.ToString();

            var currentDir = Directory.GetCurrentDirectory();
            // selected_model_idx
            CTPMain.setupFile["common"]["selected_model_idx"] = selectedModelIdx;
            // Mes Save
            //CTPMain.setupFile["common"]["MES_USE"] = mesUseCheckBox.Checked == true ? "True" : "False";
            //CTPMain.setupFile["common"]["MES_DB_NAME"] = mesDBNameTextBox.Text;
            //CTPMain.setupFile["common"]["MES_IP"] = mesIPTextBox.Text;
            //CTPMain.setupFile["common"]["MES_PORT"] = mesPortTextBox.Text;
            //CTPMain.setupFile["common"]["MES_ID"] = mesIDtextBox.Text;
            //CTPMain.setupFile["common"]["MES_PWD"] = mesPwdtextBox.Text;

            // DTM DOWN
            CTPMain.setupFile["common"]["DTM_DOWN"] = CTPMain.isDTMDown.ToString();
            // USER FIRMWARE
            CTPMain.setupFile["common"]["USER_DOWN"] = CTPMain.isUserFirmware.ToString();
            // bitbang path
            CTPMain.setupFile["common"]["commanderdir"] = CTPMain.commanderDir;
            CTPMain.setupFile["common"]["dtmfilename"] = CTPMain.dtmFileName;
            CTPMain.setupFile["common"]["userfilename"] = CTPMain.userFileName;
            CTPMain.setupFile["common"]["serialno"] = CTPMain.serialNo;
            CTPMain.setupFile.Save(currentDir + CTPMain.setupFileName);
            
            // BT Config 
            CTPMain.btConfigFile["common"]["baud_rate"] = CTPMain.baudRateIdx;
            CTPMain.btConfigFile["common"]["hci_type1"] = CTPMain.hciTypeIdx1;
            CTPMain.btConfigFile["common"]["hci_type2"] = CTPMain.hciTypeIdx2;
            CTPMain.setupFile["common"]["flow_ctrl"] = CTPMain.flowCtrl;
 
            // op
            CTPMain.btConfigFile["op"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.OP].ToString();
            // op2
            CTPMain.btConfigFile["op2"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.OP2].ToString();
            // mo
            CTPMain.btConfigFile["mo"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.MOD].ToString();
            // mo2
            CTPMain.btConfigFile["mo2"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.MOD2].ToString();
            // cfod
            CTPMain.btConfigFile["cfod"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.CFD].ToString();
            // cfod2
            CTPMain.btConfigFile["cfod2"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.CFD2].ToString();
            // rs
            CTPMain.btConfigFile["rs"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.RS].ToString();
            // rs2
            CTPMain.btConfigFile["rs2"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.RS2].ToString();
            // cfom
            CTPMain.btConfigFile["cfom"]["bchecked"] = bleCaseCheckBoxList[(int)eBLECaseIdx.CFOM].ToString();

            CTPMain.btConfigFile.Save(CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(selectedModelIdx)] + CTPMain.btleConfigFolderName);
        }
        public static bool connectDUT(string selectedRF, int idx, int baudrate)
        {
            if (!isConnect[idx])
            {
                if (devicePorts[idx].Equals("EMPTY"))
                {
                    Util.openPopupOk("Please select a port.");
                    isConnect[idx] = false;
                    setDutButton(selectedRF, Color.Red);
                    return false;
                }

                bool isOpen = SerialPortManager.Instance.openDUT(devicePorts[idx], baudrate, idx);

                if (isOpen)
                {
                    isConnect[idx] = true;
                    setDutButton(selectedRF, Color.Green);
                }
                else
                {
                    isConnect[idx] = false;
                    setDutButton(selectedRF, Color.Red);
                    return false;
                }
            }
            else
            {
                SerialPortManager.Instance.closePort(idx);
                isConnect[idx] = false;
                setDutButton(selectedRF, Color.Red);
                return false;
            }
            return true;
        }
        public static bool connectSC(string selectedRF, int idx)
        {
            if (!isConnect[idx])
            {
                if (devicePorts[idx].Equals("EMPTY"))
                {
                    Util.openPopupOk("Please select a port.");
                    isConnect[idx] = false;
                    //setSCButton(selectedRF, Color.Red);
                    return false;
                }

                bool isOpen = SerialPortManager.Instance.openDUT(devicePorts[idx], 19200, idx);

                if (isOpen)
                {
                    isConnect[idx] = true;
                    //setSCButton(selectedRF, Color.Green);
                }
                else
                {
                    isConnect[idx] = false;
                    //setSCButton(selectedRF, Color.Red);
                    return false;
                }
            }
            else
            {
                SerialPortManager.Instance.closePort(idx);
                isConnect[idx] = false;
                //setSCButton(selectedRF, Color.Red);
                return false;
            }

            return true;
        }
        public static void setDutButton(string selectedRF, Color c)
        {
            var button = selectedRF.Equals("RF1") ? CTPMain.wnd.dutBtn1 : CTPMain.wnd.dutBtn1;
            button.BackColor = c;
        }
        public static bool connectSB(string selectedRF, int idx)
        {
            if (idx > devicePorts.Length - 1)
            {
                Util.openPopupOk("Cannot connect to Index :" + idx);
                return false;
            }

            var button = selectedRF.Equals("RF1") ? CTPMain.wnd.sbBtn1 : CTPMain.wnd.sbBtn1;

            if (!isConnect[idx])
            {
                if (devicePorts[idx].Equals("EMPTY"))
                {
                    Util.openPopupOk("Please select a port.");
                    isConnect[idx] = false;
                    //setSBButton(selectedRF, Color.Red);
                    setButtonStatus(button, Color.Red);
                    return false;
                }

                bool isOpen = SerialPortManager.Instance.openSB(devicePorts[idx], idx);

                if (isOpen)
                {
                    SerialPortManager.Instance.writeSBCommand("lid?\r", selectedRF);
                    Task.Delay(1000).GetAwaiter().GetResult();

                    //string res = Util.dutLog;
                    string res = SerialPortManager.Instance.dutData[idx];

                    if (res.Length <= 0)
                    {
                        Util.openPopupOk("SB port :" + devicePorts[idx] + " not responding.");
                        SerialPortManager.Instance.closePort(idx);
                        return false;
                    }

                    if (res[0] == 'C')      // shield box closed?
                    {
                        //setSBButton(selectedRF, Color.Green);
                        setButtonStatus(button, Color.Green);
                    }
                    else
                    {
                        //setSBButton(selectedRF, Color.Yellow);
                        setButtonStatus(button, Color.Yellow);
                    }
                    isConnect[idx] = true;
                }
                else
                {
                    isConnect[idx] = false;
                    //setSBButton(selectedRF, Color.Red);
                    setButtonStatus(button, Color.Red);
                    SerialPortManager.Instance.closePort(idx);
                    return false;
                }
            }
            else
            {
                SerialPortManager.Instance.closePort(idx);
                isConnect[idx] = false;
                //setSBButton(selectedRF, Color.Red);
                setButtonStatus(button, Color.Red);
                return false;
            }
            return true;
        }
        private void sbConnectButton1_Click(object sender, EventArgs e)
        {
            bool isConnected = connectSB("RF1", (int)ePort.SB1);
            isConnect[(int)ePort.SB1] = isConnected;
            setConnectButtonText(ePort.SB1);
        }
        private void sbConnectButton2_Click(object sender, EventArgs e)
        {
            bool isConnected = connectSB("RF2", (int)ePort.SB2);
            isConnect[(int)ePort.SB2] = isConnected;
            setConnectButtonText(ePort.SB2);
        }

        private void dutConnectButton1_Click(object sender, EventArgs e)
        {
            bool isConnected = connectDUT("RF1", (int)ePort.DUT1, 115200);
            isConnect[(int)ePort.DUT1] = isConnected;
            setConnectButtonText(ePort.DUT1);
        }

        //private void dutConnectButton2_Click(object sender, EventArgs e)
        //{
        //    bool isConnected = connectDUT("RF2", (int)ePort.DUT2);
        //    isConnect[(int)ePort.DUT2] = isConnected;
        //    setConnectButtonText(ePort.DUT2);
        //}

        private void NewSetupWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            //saveSetup();
            save();
        }

        private void sbBoxOn1_CheckedChanged(object sender, EventArgs e)
        {
            CTPMain.sbOn[(int)ePort.SB1] = sbBoxOn1.Checked;

            if (isConnect[(int)ePort.SB1])
            {
                if (!CTPMain.sbOn[(int)ePort.SB1])
                {
                    //setSBButton("RF1", Color.Gray);
                    setButtonStatus(CTPMain.wnd.sbBtn1, Color.Gray);
                }
                else
                {
                    //setSBButton("RF1", Color.Green);
                    setButtonStatus(CTPMain.wnd.sbBtn1, Color.Green);
                }
            }
            else
            {
                //setSBButton("RF1", Color.Red);
                setButtonStatus(CTPMain.wnd.sbBtn1, Color.Red);
            }

            CTPMain.currStatus = eStatus.READY;
        }

        private void sbBoxOn2_CheckedChanged(object sender, EventArgs e)
        {
            //CTPMain.sbOn[(int)ePort.SB2] = sbBoxOn2.Checked;

            if (isConnect[(int)ePort.SB2])
            {
                if (!CTPMain.sbOn[(int)ePort.SB2])
                {
                    //setSBButton("RF2", Color.Gray);
                    //setButtonStatus(CTPMain.wnd.sbBtn2, Color.Gray);
                }
                else
                {
                    //setSBButton("RF2", Color.Green);
                    //setButtonStatus(CTPMain.wnd.sbBtn2, Color.Green);
                }
            }
            else
            {
                //setSBButton("RF2", Color.Red);
                //setButtonStatus(CTPMain.wnd.sbBtn2, Color.Red);
            }
        }

        private void dutComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dutComboBox1.SelectedIndex >= 0)
                devicePorts[(int)ePort.DUT1] = dutComboBox1.SelectedItem as string;
        }

        //private void dutComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (dutComboBox2.SelectedIndex >= 0)
        //        devicePorts[(int)ePort.DUT2] = dutComboBox2.SelectedItem as string;
        //}

        private void sbComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sbComboBox1.SelectedIndex >= 0)
                devicePorts[(int)ePort.SB1] = sbComboBox1.SelectedItem as string;
        }

        //private void sbComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (sbComboBox2.SelectedIndex >= 0)
        //        devicePorts[(int)ePort.SB2] = sbComboBox2.SelectedItem as string;
        //}

        private void mtp300ConnectButton_Click(object sender, EventArgs e)
        {
            //if (isConnect[(int)ePort.MTP300])
            if (isMtp300Connect)
            {
                VisaManager.Instance.closeVisaSession();
                //isConnect[(int)ePort.MTP300] = false;
                isMtp300Connect = false;
                mtp300ConnectButton.Text = "CONNECT";
                mtp300ConnectButton.BackColor = Color.FromArgb(166,175,190);
                //if (!CTPMain.isBLE)
                //    CTPMain.wnd.mtp300IndicatorButton.BackColor = Color.Red;
                //else
                CTPMain.wnd.mtp300Btn.BackColor = Color.Red;
                CTPMain.isVisaOpen = false;
            }
            else // if mtp300 is not connected
            {
                VisaManager.ip = mtp300TextBox.Text;
                VisaManager.Instance.mtp300aIP = "TCPIP0::" + mtp300TextBox.Text + "::inst0::INSTR";
                bool isOpen = false;
                
                //if (Util.IsCheckNetwork())
                    isOpen = VisaManager.Instance.openVisaSession();

                if (isOpen)
                {
                    //if (!CTPMain.isBLE)
                    //    MainWindow.wnd.mtp300IndicatorButton.BackColor = Color.Green;
                    //else
                    CTPMain.wnd.mtp300Btn.BackColor = Color.Green;

                    //isConnect[(int)ePort.MTP300] = true;
                    isMtp300Connect = true;
                    mtp300ConnectButton.Text = "Connected";
                    mtp300ConnectButton.BackColor = Color.Green;
                    CTPMain.isVisaOpen = true;
                }
                else
                {
                    VisaManager.Instance.closeVisaSession();
                    //isConnect[(int)ePort.MTP300] = false;
                    isMtp300Connect = false;
                    mtp300ConnectButton.Text = "Connect";
                    mtp300ConnectButton.BackColor = Color.FromArgb(166,175,190);
                    //if (!CTPMain.isBLE)
                    //    MainWindow.wnd.mtp300IndicatorButton.BackColor = Color.Red;
                    //else
                    CTPMain.wnd.mtp300Btn.BackColor = Color.Red;
                    CTPMain.isVisaOpen = false;
                }
            }
        }

        private void runModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (runModeComboBox.SelectedIndex == 1)
            {
                isIterationMode = true;
                iterComboBox.Visible = true;
                iterComboBox.SelectedIndex = iterCount - 1 < 0 ? 0 : iterCount - 1;
                iterCount = Int32.Parse(iterComboBox.SelectedItem as string);
            }
            else
            {
                isIterationMode = false;
                iterComboBox.Visible = false;
                iterCount = 0;
            }
            Console.WriteLine("Selected Run Mode : " + runModeComboBox.SelectedItem as string + " : isIterationMode : " + isIterationMode);
        }

        private void failStopComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (failStopComboBox.SelectedIndex == 0)
                isFailStop = true;
            else
                isFailStop = false;
            Console.WriteLine("Selected Fail Stop : " + failStopComboBox.SelectedItem as string + " : isFailStop : " + isFailStop);
        }

        private void loadConfigButton_Click(object sender, EventArgs e)
        {
            var dir = CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(selectedModelIdx)] + CTPMain.btleConfigFolderName;
            Console.WriteLine("** load config file paht : " + dir);
            CTPMain.btConfigFile.Load(dir);
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //// CSV 만 필터
            ////openFileDialog.Filter = ".csv files (*.csv)|*.csv";
            ////현재 실행프로그램 경로
            //openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();

            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    //clearGridData();
            //    try
            //    {
            //        Console.WriteLine("opened file name : " + openFileDialog.FileName);
            //        //generateDataTable(openFileDialog.FileName, true);
            //        //selectedFiles[selectedRF.Equals("RF1") ? 0 : 1] = openFileDialog.FileName;
            //    }
            //    catch (Exception ex)
            //    {
            //        Util.openPopupOk("NewSetupWindow : loadConfigButton_Click :" + ex.Message);
            //    }
            //}
        }

        private void saveConfigButton_Click(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            //saveSetup();
            save();
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void changePwdButton_Click(object sender, EventArgs e)
        {
            if (newPwdTextBox.Text.Equals(""))
            {
                Util.openPopupOk("New Password Cannot Be EMPTY.");
                return;
            }
            CTPMain.pwd = newPwdTextBox.Text;
            CTPMain.passwordFile["password"]["password"] = newPwdTextBox.Text;
            CTPMain.passwordFile.Save(CTPMain.rootFolder + CTPMain.passwordFileName);
            Util.openPopupOk("New Password\n\n" + CTPMain.pwd);
        }

        public static void setButtonStatus(Button b, Color c)
        {
            if (b.InvokeRequired)
            {
                //b.BeginInvoke(new Action(() => b.BeginInvoke(new Action(() => b.BackColor = c))));
                b.BeginInvoke(new Action(() => b.BackColor = c));
            }
        }
        //public static void setSBButton(string selectedRF, Color c)
        //{
        //    var button = selectedRF.Equals("RF1") ? CTPMain.wnd.sbBtn1 : CTPMain.wnd.sbBtn2;
        //    button.BackColor = c;
        //}
        
        public class TestMethod
        {
            public string testName { get; set; }
            public int testValue { get; set; }
        }
        public class FlowCtrl
        {
            public string flowName { get; set; }
            public int flowValue { get; set; }
        }
        public class HCIType
        {
            public string methodName { get; set; }
            public int methodValue { get; set; }
        }

        public class BaudRate
        {
            public string rateStr { get; set; }
            public int rate { get; set; }
        }
        // BLE DTM Tab
        private void initBleDtmTab()
        {
            // Test Method
            List<TestMethod> testList = new List<TestMethod>
            {
                new TestMethod { testName = "DTM", testValue = 0 },
                new TestMethod { testName = "Advertising", testValue = 1 },
            };
            testMethodComboBox.ValueMember = "testValue";
            testMethodComboBox.DisplayMember = "testName";
            testMethodComboBox.DataSource = testList;
            testMethodComboBox.SelectedIndex = 0;

            // 1) HCI Type 
            List<HCIType> hciList, hciList2;
            hciList = new List<HCIType>
            {
                new HCIType { methodName = "NONE", methodValue = 0 },
                new HCIType { methodName = "USB", methodValue = 1 },
                new HCIType { methodName = "UART", methodValue = 2 },
                new HCIType { methodName = "2-wire UART", methodValue = 3 },
                new HCIType { methodName = "USB to UART", methodValue = 4 },
                new HCIType { methodName = "USB to 2-wire", methodValue = 5 },
            };

            hciList2 = new List<HCIType>
            {
                new HCIType { methodName = "NONE", methodValue = 0 },
                new HCIType { methodName = "USB", methodValue = 1 },
                new HCIType { methodName = "UART", methodValue = 2 },
                new HCIType { methodName = "2-wire UART", methodValue = 3 },
                new HCIType { methodName = "USB to UART", methodValue = 4 },
                new HCIType { methodName = "USB to 2-wire", methodValue = 5 },
            };

            hciTypeComboBox.ValueMember = "methodValue";
            hciTypeComboBox.DisplayMember = "methodName";
            hciTypeComboBox.SelectedIndexChanged -= new EventHandler(hciTypeComboBox_SelectedIndexChanged);
            hciTypeComboBox.DataSource = hciList;
            hciTypeComboBox.SelectedIndexChanged += new EventHandler(hciTypeComboBox_SelectedIndexChanged);
            hciTypeComboBox.SelectedIndex = Int32.Parse(CTPMain.hciTypeIdx1);
            // 2) HCI Type
            //hciTypeComboBox2.ValueMember = "methodValue";
            //hciTypeComboBox2.DisplayMember = "methodName";
            //hciTypeComboBox2.SelectedIndexChanged -= new EventHandler(hciTypeComboBox2_SelectedIndexChanged);
            //hciTypeComboBox2.DataSource = hciList2;
            //hciTypeComboBox2.SelectedIndexChanged += new EventHandler(hciTypeComboBox2_SelectedIndexChanged);
            //hciTypeComboBox2.SelectedIndex = Int32.Parse(CTPMain.hciTypeIdx2);
            // Baud Rate
            var list = new List<BaudRate>
            {
                new BaudRate { rateStr = "2400", rate = 0 },
                new BaudRate { rateStr = "4800", rate = 1 },
                new BaudRate { rateStr = "9600", rate = 2 },
                new BaudRate { rateStr = "19200", rate = 3 },
                new BaudRate { rateStr = "38400", rate = 4 },
                new BaudRate { rateStr = "57600", rate = 5 },
                new BaudRate { rateStr = "115200", rate = 6 },
            };
            baudRateComboBox.ValueMember = "rate";
            baudRateComboBox.DisplayMember = "rateStr";
            baudRateComboBox.SelectedIndexChanged -= new EventHandler(baudRateComboBox_SelectedIndexChanged);
            baudRateComboBox.DataSource = list;
            baudRateComboBox.SelectedIndexChanged += new EventHandler(baudRateComboBox_SelectedIndexChanged);
            // init baudrate
            baudRateComboBox.SelectedIndex = Int32.Parse(CTPMain.baudRateIdx);
            CTPMain.baudRateValue = baudRateComboBox.GetItemText(baudRateComboBox.SelectedItem);

            // Flow Ctrl
            List<FlowCtrl> flowList = new List<FlowCtrl>
            {
                new FlowCtrl { flowName = "NONE", flowValue = 0 },
                new FlowCtrl { flowName = "FLOW", flowValue = 1 },
            };
            flowCtrlComboBox.ValueMember = "flowValue";
            flowCtrlComboBox.DisplayMember = "flowName";
            flowCtrlComboBox.SelectedIndexChanged -= new EventHandler(flowCtrlComboBox_SelectedIndexChanged);
            flowCtrlComboBox.DataSource = flowList;
            flowCtrlComboBox.SelectedIndexChanged += new EventHandler(flowCtrlComboBox_SelectedIndexChanged);
            flowCtrlComboBox.SelectedIndex = Int32.Parse(CTPMain.flowCtrl);

            // init check box
            opCheckBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.OP];
            op2CheckBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.OP2];
            moCheckBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.MOD];
            mo2CheckBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.MOD2];
            cfodCheckBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.CFD];
            cfod2checkBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.CFD2];
            rsCheckBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.RS];
            rs2checkBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.RS2];
            cfomCheckBox.Checked = bleCaseCheckBoxList[(int)eBLECaseIdx.CFOM];
        }

        private void baudRateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = (int)baudRateComboBox.SelectedValue;
            CTPMain.baudRateIdx = selectedValue.ToString();
            CTPMain.baudRateValue = baudRateComboBox.GetItemText(baudRateComboBox.SelectedItem);
            Console.WriteLine("Selected Baud Rate Idx : " + CTPMain.baudRateIdx + " : Baud Rate Value : " + CTPMain.baudRateValue);
        }

        private void hciTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = (int)hciTypeComboBox.SelectedValue;
            CTPMain.hciTypeIdx1 = selectedValue.ToString();
            Console.WriteLine("Selected 1) HCI Type : " + selectedValue);
        }

        //private void hciTypeComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var selectedValue = (int)hciTypeComboBox2.SelectedValue;
        //    CTPMain.hciTypeIdx2 = selectedValue.ToString();
        //    Console.WriteLine("Selected 2) HCI Type : " + selectedValue);
        //}

        private void flowCtrlComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = (int)flowCtrlComboBox.SelectedValue;
            CTPMain.flowCtrl = selectedValue.ToString();
            Console.WriteLine("Selected Flow Ctrl : " + selectedValue);
        }

        private void testMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedValue = (int)testMethodComboBox.SelectedValue;
            Console.WriteLine("Selected Test Method : " + selectedValue);
        }

        private void iterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            iterCount = Int32.Parse(iterComboBox.SelectedItem as string);
            Console.WriteLine("Selected IterCount  : " + iterCount);
        }

        private void opButton_Click(object sender, EventArgs e)
        {
            new OP().ShowDialog(this);
        }

        private void moButton_Click(object sender, EventArgs e)
        {
            new MOD().ShowDialog(this);
        }

        private void cfodButton_Click(object sender, EventArgs e)
        {
            new CFD().ShowDialog(this);
        }

        private void rsButton_Click(object sender, EventArgs e)
        {
            new RS().ShowDialog(this);
        }

        private void opCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.OP] = opCheckBox.Checked;
        }

        private void moCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.MOD] = moCheckBox.Checked;
        }

        private void cfodCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.CFD] = cfodCheckBox.Checked;
        }

        private void rsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.RS] = rsCheckBox.Checked;
        }
        
        //private void modelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    selectedModelIdx = modelComboBox.SelectedIndex.ToString();
        //}

        private void scannerConnectButton1_Click(object sender, EventArgs e)
        {
            bool isConnected = connectSC("RF1", (int)ePort.SC1);
            isConnect[(int)ePort.SC1] = isConnected;
            setConnectButtonText(ePort.SC1);
        }

        private void scannerConnectButton2_Click(object sender, EventArgs e)
        {
            bool isConnected = connectSC("RF1", (int)ePort.SC2);
            isConnect[(int)ePort.SC2] = isConnected;
            setConnectButtonText(ePort.SC2);
        }

        //private void scannerComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (scannerComboBox1.SelectedIndex >= 0)
        //        devicePorts[(int)ePort.SC1] = scannerComboBox1.SelectedItem as string;
        //}

        //private void scannerComboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        //{
        //    if (scannerComboBox2.SelectedIndex >= 0)
        //        devicePorts[(int)ePort.SC2] = scannerComboBox2.SelectedItem as string;
        //}

        private void op2Button_Click(object sender, EventArgs e)
        {
            new OP2().ShowDialog(this);
        }

        private void op2CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.OP2] = op2CheckBox.Checked;
        }

        private void mo2Button_Click(object sender, EventArgs e)
        {
            new MOD2().ShowDialog(this);
        }

        private void mo2CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.MOD2] = mo2CheckBox.Checked;
        }

        private void cfod2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.CFD2] = cfod2checkBox.Checked;
        }

        private void cfod2Button_Click(object sender, EventArgs e)
        {
            new CFD2().ShowDialog(this);
        }

        private void rs2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.RS2] = rs2checkBox.Checked;
        }

        private void rs2Button_Click(object sender, EventArgs e)
        {
            new RS2().ShowDialog(this);
        }

        private void cfomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bleCaseCheckBoxList[(int)eBLECaseIdx.CFOM] = cfomCheckBox.Checked;
        }

        private void cfomButton_Click(object sender, EventArgs e)
        {
            new CFOM().ShowDialog(this);
        }

        private void userFirmWareCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CTPMain.isUserFirmware = userFirmWareCheckBox.Checked;
        }

        private void dtmDownCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CTPMain.isDTMDown = dtmDownCheckBox.Checked;
        }

        private void bitbangPathButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog o = new FolderBrowserDialog();
            {
                if (o.ShowDialog() == DialogResult.OK)
                {
                    CTPMain.commanderDir = o.SelectedPath;
                    bitbangPathLabel.Text = CTPMain.commanderDir == "" ? "N/A" : CTPMain.commanderDir;
                }
            }
        }

        private void dtmFirmwareChangeButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // CSV 만 필터
            openFileDialog.Filter = "*.*|*.*";
            //현재 실행프로그램 경로
            openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                dtmFilenameLabel.Text = openFileDialog.FileName;
                CTPMain.dtmFileName = Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void userFirmwareChangeButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // CSV 만 필터
            openFileDialog.Filter = "*.*|*.*";
            //현재 실행프로그램 경로
            openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                userfirmwareFileNameLabel.Text = openFileDialog.FileName;
                CTPMain.userFileName = Path.GetFileName(openFileDialog.FileName);
            }
        }

        private void serialNoComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serialNoComboBox1.SelectedIndex >= 0 && serialNoComboBox1.SelectedItem as string != string.Empty)
            {
                var str = serialNoComboBox1.SelectedItem as string;
                var list = str.Split(':');
                CTPMain.serialNo = list[1];
            }
        }

        private void dutScanButton_Click(object sender, EventArgs e)
        {
            if (isConnect[(int)ePort.DUT1])
            {
                Util.openPopupOk("Please disconnect DUT connection First.");
                return;
            }
            serialNoComboBox1.Enabled = true;

            serialNoComboBox1.Items.Clear();

            foreach (var ftdiDevice in FTDIDeviceCtrl.GetDeviceList())
            {
                string str = ftdiDevice.ComPort + ":" + ftdiDevice.SerialNumber;
                Console.WriteLine(str);
                serialNoComboBox1.Items.Add(str);
            }
            isConnect[(int)ePort.SERIALNO] = false;
            setConnectButtonText(ePort.SERIALNO);
        }

        private void serialNoSelectButton_Click(object sender, EventArgs e)
        {
            if (isConnect[(int)ePort.SERIALNO])
            {
                serialNoComboBox1.Items.Clear();
                serialNoComboBox1.Items.Add("");
                serialNoComboBox1.SelectedIndex = -1;
                isConnect[(int)ePort.SERIALNO] = false;
                CTPMain.serialNo = string.Empty;

                foreach (var ftdiDevice in FTDIDeviceCtrl.GetDeviceList())
                {
                    string str = ftdiDevice.ComPort + ":" + ftdiDevice.SerialNumber;
                    Console.WriteLine(str);
                    serialNoComboBox1.Items.Add(str);
                }
                CTPMain.serialNo = string.Empty;
                setConnectButtonText(ePort.SERIALNO);
                if (!isConnect[(int)ePort.SERIALNO])
                    serialNoComboBox1.Enabled = true;
                else
                    serialNoComboBox1.Enabled = false;
                serialNoComboBox1.Text = "";
                return;
            }
            if (serialNoComboBox1.SelectedIndex >= 0)
            {
                var str = serialNoComboBox1.SelectedItem as string;
                var list = str.Split(':');
                CTPMain.serialNo = list[1];

                isConnect[(int)ePort.SERIALNO] = !isConnect[(int)ePort.SERIALNO];
            }
            else
            {
                serialNoComboBox1.Items.Clear();
                serialNoComboBox1.Items.Add("");
                serialNoComboBox1.SelectedIndex = -1;
                isConnect[(int)ePort.SERIALNO] = false;
                CTPMain.serialNo = string.Empty;
            }

            setConnectButtonText(ePort.SERIALNO);

            if (!isConnect[(int)ePort.SERIALNO])
                serialNoComboBox1.Enabled = true;
            else
                serialNoComboBox1.Enabled = false;
        }
    }
}
