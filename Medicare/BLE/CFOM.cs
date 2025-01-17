using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Medicare.Main;
using Medicare.Port;
using Medicare.Setup;
using Medicare.Utility;
using Medicare.Visa;

namespace Medicare.BLE
{
    public partial class CFOM : Form
    {
        public static string maxFreqTolLower = "-150.0";
        public static string maxFreqTolUpper = "150.0";
        public static string deltaf2avg = "185.0";
        public static string deltaf2min = "92.5";

        public static List<string> lstVerdictCFOM = new List<string>() { "", "", "" };
        public static List<string> LstDetailReportCFOM = new List<string>() { "", "", "" };

        public static string[] df0_max = new string[] { "", "", "" };
        public static string[] df0_min = new string[] { "", "", "" };
        public static string[] df0_avg = new string[] { "", "", "" };
        public static string[] df2_min = new string[] { "", "", "" };
        public static string[] df2_avg = new string[] { "", "", "" };

        private VisaManager vgr = null;
        private SerialPortManager sgr = null;

        public CFOM()
        {
            InitializeComponent();
            load();
            vgr = VisaManager.Instance;
            sgr = SerialPortManager.Instance;

            initColor();
        }

        private void initColor()
        {
            cfomCondLabel.BackColor = Color.FromArgb(0, 126, 222);
            cfomCondLabel.ForeColor = Color.White;

            cfomSpecLabel.BackColor = Color.FromArgb(0, 126, 222);
            cfomSpecLabel.ForeColor = Color.White;

            okButton.BackColor = Color.FromArgb(0, 54, 105);
            okButton.ForeColor = Color.White;

            cancelButton.BackColor = Color.FromArgb(0, 54, 105);
            cancelButton.ForeColor = Color.White;
        }
        public void clearResult()
        {
            lstVerdictCFOM = new List<string>() { "", "", "" };
            LstDetailReportCFOM = new List<string>() { "", "", "" };
            df0_max = new string[] { "", "", "" };
            df0_min = new string[] { "", "", "" };
            df0_avg = new string[] { "", "", "" };
            df2_min = new string[] { "", "", "" };
            df2_avg = new string[] { "", "", "" };
        }
        private void load()
        {
            channelTextBox.Text = CTPMain.btConfigFile["cfom"]["channel"].ToString();
            packetNumTextBox.Text = CTPMain.btConfigFile["cfom"]["npackets"].ToString();

            maxFreqTol_LowerTextBox.Text = CTPMain.btConfigFile["cfom"]["maxfreqtolLow"].ToString();
            maxFreqTol_UpperTextBox.Text = CTPMain.btConfigFile["cfom"]["maxfreqtolUp"].ToString();
            deltaf2avgTextBox.Text = CTPMain.btConfigFile["cfom"]["deltaf2avg"].ToString();
            deltaf2minTextBox.Text = CTPMain.btConfigFile["cfom"]["deltaf2min"].ToString();
        }

        public static void cfomCommands(out List<string> list)
        {
            List<string> cmdList = new List<string>();
            var channel = CTPMain.channel[(int)eBLECaseIdx.CFOM];
            // 1
            cmdList.Add("CNT,0");
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFOM:CHANnel {0}", channel));
            cmdList.Add("CMD,CONF:BT:LE:TC:CFOM:DLENgth 37");
            cmdList.Add(string.Format("CMD,CONF:BT:LE:TC:CFOM:NPACkets {0}", CTPMain.packetNum[(int)eBLECaseIdx.CFOM]));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFOM:LIMit:DF0_MAX_UP {0}", maxFreqTolUpper));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFOM:LIMit:DF0_MIN_LOW {0}", maxFreqTolLower));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LETCase:CFOM:LIMit:DF2_AVG_LOW {0}", deltaf2avg));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFOM:LIMit:DF2_MIN_LOW {0}", deltaf2min));
            cmdList.Add("CMD,READ:BT:LE:TCase:CFOM?");
            cmdList.Add("CMD,READ:BT:LE:TCase:CFOM:VERDict?");

            list = cmdList;
        }

        private void save()
        {
            string s = channelTextBox.Text;
            var c = s.Split('-');
            CTPMain.channel[(int)eBLECaseIdx.CFOM] = c[0] + "-" + c[1] + "-" + c[2];
            CTPMain.packetNum[(int)eBLECaseIdx.CFOM] = packetNumTextBox.Text;

            maxFreqTolLower = maxFreqTol_LowerTextBox.Text;
            maxFreqTolUpper = maxFreqTol_UpperTextBox.Text;
            deltaf2avg = deltaf2avgTextBox.Text;
            deltaf2min = deltaf2minTextBox.Text;

            CTPMain.btConfigFile["cfom"]["channel"] = CTPMain.channel[(int)eBLECaseIdx.CFD];
            CTPMain.btConfigFile["cfom"]["npackets"] = CTPMain.packetNum[(int)eBLECaseIdx.CFD];
            CTPMain.btConfigFile["cfom"]["maxFreqTolLower"] = maxFreqTolLower;
            CTPMain.btConfigFile["cfom"]["maxFreqTolUpper"] = maxFreqTolUpper;
            CTPMain.btConfigFile["cfom"]["deltaf2avg"] = deltaf2avg;
            CTPMain.btConfigFile["cfom"]["deltaf2min"] = deltaf2min;

            CTPMain.btConfigFile.Save(CTPMain.rootFolder +
                                      CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] +
                                      CTPMain.btleConfigFolderName);
        }

        private bool addRows()
        {
            bool isPass = true;
            CTPMain.startIndex += 2;
            int len = CTPMain.startIndex + (int)eTestItem.CFOM_TIME - (int)eTestItem.DF0_MAX_CH0;

            for (int r = CTPMain.startIndex; r < len; r++)
            {
                //await Task.Delay(CTPMain.waitTime);
                int freIdx = (r - CTPMain.startIndex) % 3;
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];
                int row = (r - CTPMain.startIndex) + (int)eTestItem.DF0_MAX_CH0;

                if (row <= (int)eTestItem.DF0_MAX_CH39)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df0_max[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = maxFreqTolLower;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = maxFreqTolUpper;
                }
                else if ((int)eTestItem.DF0_MAX_CH39 < row && row <= (int)eTestItem.DF0_MIN_CH39)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df0_min[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = maxFreqTolLower;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = maxFreqTolUpper;
                }
                else if ((int)eTestItem.DF0_MIN_CH39 < row && row <= (int)eTestItem.DF0_AVG_CH39)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df0_avg[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = maxFreqTolLower;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = maxFreqTolUpper;
                }
                else if ((int)eTestItem.DF0_AVG_CH39 < row && row <= (int)eTestItem.DF2_MIN_CH39)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df2_min[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = deltaf2min;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = "-";
                }
                else if ((int)eTestItem.DF2_MIN_CH39 < row && row <= (int)eTestItem.DF2_AVG_CH39)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df2_avg[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = deltaf2avg;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = "-";
                }

                if (lstVerdictCFOM[freIdx].Equals("PASS"))
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.BackColor = Color.Green;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;
                }
                else
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.BackColor = Color.Red;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;
                    isPass = false;
                }
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictCFOM[freIdx];
                //CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = "?";
                //CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = "?";
            }

            CTPMain.wnd.mainBLEGrid.Rows[len].Cells[(int)eCol.TST].Value =
                CTPMain.testTime[(int)eBLECaseIdx.CFOM];
            CTPMain.startIndex = len;
            return isPass;
        }

        public async Task<bool> runCFOM(List<string> cmdList) // carrier freq. offset + mod. char(preamble)
        {
            try
            {
                lstVerdictCFOM = new List<string>() { "", "", "" };
                LstDetailReportCFOM = new List<string>() { "", "", "" };
                df0_max = new string[] { "", "", "" };
                df0_min = new string[] { "", "", "" };
                df0_avg = new string[] { "", "", "" };
                df2_min = new string[] { "", "", "" };
                df2_avg = new string[] { "", "", "" };

                // measure
                //error = ERROR.NO_ERROR;
                // start time
                DateTime startTime = DateTime.Now;

                string resultBuffer = string.Empty;
                string verdictBuffer = string.Empty;
                List<string> buf = new List<string>();
                int row = CTPMain.resultVerdictIndex;

                foreach (var s in cmdList)
                {
                    buf.Clear();
                    await Task.Delay(CTPMain.waitTime);
                    var l = s.Split(',');
                    var cmd = l[1];

                    if (l[0].Equals("CMD"))
                    {
                        if (cmd.Contains("?"))
                        {
                            if (vgr.writeVisa(cmd, CTPMain.selectedRFPort))
                            {
                                await Task.Delay(CTPMain.waitTime * 100);

                                string res = vgr.readVisa(cmd, CTPMain.selectedRFPort);
                                if (res.Equals("ERROR"))
                                    return false;
                                buf.Add(res);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (!vgr.writeVisa(cmd, CTPMain.selectedRFPort))
                                return false;
                        }
                    }
                    else if (l[0].Equals("DUT"))
                    {
                        var port = CTPMain.selectedRFPort == 0 ? "RF1" : "RF2";
                        var isOk = sgr.sendBitData(cmd, port);
                        if (!isOk) return false;
                    }
                    else if (l[0].Equals("CNT"))
                    {
                        CTPMain.dtmIdx = int.Parse(cmd);
                    }

                    if (cmd.Contains("VERDict?") && !buf[0].Equals(""))
                    {
                        await Task.Delay(CTPMain.waitTime * 100);
                        cfomVerdict(buf[0]);
                    }
                    else if (cmd.Contains("?") && !buf[0].Equals(""))
                    {
                        await Task.Delay(CTPMain.waitTime * 100);
                        cfomResult(buf[0]);
                    }
                }
                CTPMain.resultVerdictIndex = row;
                CTPMain.testTime[(int)eBLECaseIdx.CFOM] = (float)(DateTime.Now - startTime).TotalSeconds;

                bool isPass = addRows();
               
                CTPMain.wnd.mainBLEGrid.RefreshEdit();
                CTPMain.wnd.pgrBar.Value = CTPMain.prgCount += 16;
                if (!checkError(CTPMain.selectedRFPort))
                    return false;
                //string report = getCFDReport(CTPMain.testTime[(int)eBLECaseIdx.CFD]);
                //CTPReportWindow.reportList.Add(report);
                //CTPReportWindow.passFailReport += report + "\n";
                return isPass;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("CFOM Error", Color.Red);
                Util.openPopupOk("run cfom - " + e.Message);
                return false;
            }
        }
        private void cfomVerdict(string verdictBuffer)
        {
            if (!verdictBuffer.Equals(""))
            {
                verdictBuffer = verdictBuffer.TrimEnd('\r', '\n');
                var res = verdictBuffer.Split(',');
                int idx = 0;

                foreach (var re in res)
                    lstVerdictCFOM[idx++] = re;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    lstVerdictCFOM[i] = "FAIL";
                //error = ERROR.MTP300A_ERROR;
            }
        }
        private void cfomResult(string resultBuffer)
        {
            if (!resultBuffer.Equals(""))
            {
                resultBuffer = resultBuffer.TrimEnd('\r', '\n');
                var res = resultBuffer.Split(',');

                for (int i = 0; i < 3; i++)
                {
                    df0_max[i] = res[(i * 5)];
                    df0_min[i] = res[(i * 5) + 1];
                    df0_avg[i] = res[(i * 5) + 2];
                    df2_min[i] = res[(i * 5) + 3];
                    df2_avg[i] = res[(i * 5) + 4];
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    lstVerdictCFOM[i] = "FAIL";
                // error = ERROR.MTP300A_ERROR;
            }
        }
        private bool checkError(int selectedRFPort)
        {
            // checkError
            if (!vgr.writeVisa("SYST:ERR:COUN?", selectedRFPort))
                return false;

            string response = vgr.readVisa("SYST:ERR:COUN?", selectedRFPort);

            if (!response.Equals(""))
            {
                int count = Int32.Parse(response);
                if (count != 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (!vgr.writeVisa("SYST:ERR?", selectedRFPort))
                            return false;
                        string e = vgr.readVisa("SYST:ERR?", selectedRFPort);
                        CTPMain.lstError.Add(e);
                    }
                }
            }

            return true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                save();
                Close();
            }
            catch (Exception ex)
            {
                CTPMain.wnd.setTextPgrBar("CFOM Error", Color.Red);
                Util.openPopupOk("Error : " + ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
