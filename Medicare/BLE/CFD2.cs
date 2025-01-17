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
using Medicare.Main;
using Medicare.Port;
using Medicare.Setup;
using Medicare.Utility;
using Medicare.Visa;

namespace Medicare.BLE
{
    public partial class CFD2 : Form
    {
        //public static string lower_max_freq_tolerance = "-150.0";
        //public static string upper_max_freq_tolerance = "150.0";
        //public static string lower_df2_avg = "185.0";
        //public static string lower_df2_min = "92.5";
        //public static List<string> lstVerdictCarrierDrift = new List<string>() { "", "", "" };
        //public static List<string> lstDetailReportCarrierDrift = new List<string>() { "", "", "" };
        public static string[] df_max = new string[] { "", "", "" };
        public static string[] f_drift01_max = new string[] { "", "", "" };
        public static string[] f_drift_max = new string[] { "", "", "" };
        public static string[] drift_rate_max = new string[] { "", "", "" };

        public static string freq_accuracy = "150.0";           // |fTX-f[n]| <= 150.0 kHz
        public static string freq_drift = "50.0";               // |f[0]-f[n]| <= 50.0 kHz
        public static string initial_freq_drift = "23.0";       // |f[1]-f[0]| <= 23.0 kHz
        public static string max_drift_rate = "20.0";               // |f[n]-f[n-5]| <= 20.0 kHz
        public static List<string> lstVerdictCFD = new List<string>() { "", "", "" };
        public static List<string> LstDetailReportCFD = new List<string>() { "", "", "" };
        //public static string[] df0_max_CFOM = new string[] { "", "", "" };
        //public static string[] df0_min_CFOM = new string[] { "", "", "" };
        //public static string[] df0_avg_CFOM = new string[] { "", "", "" };
        //public static string[] df2_min_CFOM = new string[] { "", "", "" };
        //public static string[] df2_avg_CFOM = new string[] { "", "", "" };

        private VisaManager vgr = null;
        private SerialPortManager sgr = null;
        public CFD2()
        {
            InitializeComponent();
            load();
            vgr = VisaManager.Instance;
            sgr = SerialPortManager.Instance;
            initColor();
        }
        private void initColor()
        {
            cfd2CondLabel.BackColor = Color.FromArgb(0, 126, 222);
            cfd2CondLabel.ForeColor = Color.White;

            cfd2SpecLabel.BackColor = Color.FromArgb(0, 126, 222);
            cfd2SpecLabel.ForeColor = Color.White;

            okButton.BackColor = Color.FromArgb(0, 54, 105);
            okButton.ForeColor = Color.White;

            cancelButton.BackColor = Color.FromArgb(0, 54, 105);
            cancelButton.ForeColor = Color.White;
        }
        public void clearResult()
        {
            lstVerdictCFD = new List<string>() { "", "", "" };
            LstDetailReportCFD = new List<string>() { "", "", "" };
            df_max = new string[] { "", "", "" };
            f_drift01_max = new string[] { "", "", "" };
            f_drift_max = new string[] { "", "", "" };
            drift_rate_max = new string[] { "", "", "" };
        }
        private void load()
        {
            channelTextBox.Text = CTPMain.btConfigFile["cfod2"]["channel"].ToString();
            packetNumTextBox.Text = CTPMain.btConfigFile["cfod2"]["npackets"].ToString();

            freqAccuracyTextBox.Text = CTPMain.btConfigFile["cfod2"]["freqacc"].ToString();
            freqDriftTextBox.Text = CTPMain.btConfigFile["cfod2"]["freqdrift"].ToString();
            initialFreqDriftTextBox.Text = CTPMain.btConfigFile["cfod2"]["initfreqdrift"].ToString();
            maxDriftRateTextBox.Text = CTPMain.btConfigFile["cfod2"]["maxdriftrate"].ToString();
        }
        public static void carrierFreqModCommands(out List<string> list)
        {
            List<string> cmdList = new List<string>();
            var channel = CTPMain.channel[(int)eBLECaseIdx.CFD2];
            // 1
            cmdList.Add("CNT,0");
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFD2M:CHANnel {0}", channel));
            cmdList.Add("CMD,CONFigure:BT:LE:TCase:CFD2M:DLENgth 37");
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFD2M:NPACkets {0}", CTPMain.packetNum[(int)eBLECaseIdx.CFD2]));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFD2M:LIMit:DF_MAX_UP {0}", freq_accuracy));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFD2M:LIMit:F_DRIFT01_MAX_UP {0}", initial_freq_drift));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFD2M:LIMit:F_DRIFT_MAX_UP {0}", freq_drift));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:CFD2M:LIMit:DRIFT_RATE_MAX_UP {0}", max_drift_rate));
            cmdList.Add(String.Format("CMD,CONF:BT:LE:TC:CFD:CHANnel {0}", channel));
            cmdList.Add("CMD,READ:BT:LE:TCase:CFD2M?");
            cmdList.Add("CMD,READ:BT:LE:TCase:CFD2M:VERDict?");

            list = cmdList;
        }
        public static string getCFDReport(float time)
        {
            string report = "";

            try
            {
                report = string.Format(
                    "==== Carrier frequency offset and drift, 1M ====\n" +
                    ":::Initial Conditions:::\n" +
                    "       Num. of packets :     {0}\n" +
                    ":::Limits:::\n" +
                    "       |fTX-f[n]|    <= {1} kHz\n" +
                    "       |f[1]-f[0]|   <= {2} kHz\n" +
                    "       |f[0]-f[n]|   <= {3} kHz\n" +
                    "       |f[n]-f[n-5]| <= {4} kHz\n" +
                    ":::Results:::\n",
                    CTPMain.packetNum[(int)eBLECaseIdx.CFD2],
                    freq_accuracy,
                    initial_freq_drift,
                    freq_drift,
                    max_drift_rate);

                report += String.Format("ch    f(MHz)  FTX_FN    F1_F0    F0_Fn    FN-FN5    Verdict\n");

                var channel = CTPMain.channel[(int)eBLECaseIdx.CFD2].Split('-');

                for (int i = 0; i < 3; i++)
                {
                    string detailReport = string.Empty;
                    if (channel[i].Length != 2)
                        channel[i] = "0" + channel[i];
                    string freq = ((Int32.Parse(channel[i]) * 2) + 2402).ToString();
                    detailReport += String.Format("{0}    {1}    {2}          {3}        {4}        {5}          {6}\n",
                        channel[i],
                        freq,
                        df_max[i],
                        f_drift01_max[i],
                        f_drift_max[i],
                        drift_rate_max[i],
                        lstVerdictCFD[i]);
                    LstDetailReportCFD[i] = detailReport;
                }

                // DF_MAX                FTX_FN          
                // F_FRIFT01_MAX         F1_F0
                // F_DRIFT_MAX           F0_Fn
                // DRIFT_RATE_MAS        FN-FN5

                foreach (var item in LstDetailReportCFD)
                    report += item;

                foreach (var item in CTPMain.lstError)
                {
                    if (!item.Equals(""))
                        report += string.Format("Test Case Error : {0}\n", item);
                }

                float totalTime = 0f;

                //foreach (var str in lstTestTime)
                //    totalTime += float.Parse(str) / 1000;
                totalTime += float.Parse(time.ToString());
                report += string.Format("Test Time : {0}sec\n", totalTime.ToString("#.##"));
                report += "\n\n";
                return report;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("CFD2 Error", Color.Red);
                report += string.Format("Test Case Error : {0}", e.Message);
                return report;
            }
        }
        private void save()
        {
            string s = channelTextBox.Text;
            var c = s.Split('-');
            CTPMain.channel[(int)eBLECaseIdx.CFD2] = c[0] + "-" + c[1] + "-" + c[2];
            CTPMain.packetNum[(int)eBLECaseIdx.CFD2] = packetNumTextBox.Text;

            freq_accuracy = freqAccuracyTextBox.Text;
            freq_drift = freqDriftTextBox.Text;
            initial_freq_drift = initialFreqDriftTextBox.Text;
            max_drift_rate = maxDriftRateTextBox.Text;

            CTPMain.btConfigFile["cfod2"]["channel"] = CTPMain.channel[(int)eBLECaseIdx.CFD2];
            CTPMain.btConfigFile["cfod2"]["npackets"] = CTPMain.packetNum[(int)eBLECaseIdx.CFD2];
            CTPMain.btConfigFile["cfod2"]["freqacc"] = freq_accuracy;
            CTPMain.btConfigFile["cfod2"]["freqdrift"] = freq_drift;
            CTPMain.btConfigFile["cfod2"]["initfreqdrift"] = initial_freq_drift;
            CTPMain.btConfigFile["cfod2"]["maxdriftrate"] = max_drift_rate;

            CTPMain.btConfigFile.Save(CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] + CTPMain.btleConfigFolderName);
        }

        private bool addRows()
        {
            bool isPass = true;
            CTPMain.startIndex += 2;
            int len = CTPMain.startIndex + (int)eTestItem.CFDTIME_2M - (int)eTestItem.ftx_fn_CH0_2M;

            for (int r = CTPMain.startIndex; r < len; r++)
            {
                int freIdx = (r - CTPMain.startIndex) % 3;
                int row = (r - CTPMain.startIndex) + (int)eTestItem.ftx_fn_CH0_2M;
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];

                if (row <= (int)eTestItem.ftx_fn_CH39_2M)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df_max[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = "-" + freq_accuracy;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = freq_accuracy;
                }
                else if ((int)eTestItem.ftx_fn_CH39_2M < row && row <= (int)eTestItem.f0_fn_CH39_2M)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = drift_rate_max[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = "-" + freq_drift;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = freq_drift;
                }
                else if ((int)eTestItem.f0_fn_CH39_2M < row && row <= (int)eTestItem.f1_f0_CH39_2M)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = f_drift01_max[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = "-" + initial_freq_drift;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = initial_freq_drift;
                }
                else if ((int)eTestItem.f1_f0_CH39_2M < row && row <= (int)eTestItem.fn_fn5_CH39_2M)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = drift_rate_max[freIdx];
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = "-" + max_drift_rate;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = max_drift_rate;
                }

                if (lstVerdictCFD[freIdx].Equals("PASS"))
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
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictCFD[freIdx];
                //CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = "?";
                //CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = "?";
            }
            CTPMain.wnd.mainBLEGrid.Rows[len].Cells[(int)eCol.TST].Value =
                CTPMain.testTime[(int)eBLECaseIdx.CFD2];
            CTPMain.startIndex = len;
            return isPass;
        }
        public async Task<bool> runCFD(List<string> cmdList)     // carrier freq. offset + mod. char(preamble)
        {
            try
            {
                CTPMain.lstError = new List<string>();
                lstVerdictCFD = new List<string>() { "", "", "" };
                LstDetailReportCFD = new List<string>() { "", "", "" };

                df_max = new string[] { "", "", "" };
                f_drift01_max = new string[] { "", "", "" };
                f_drift_max = new string[] { "", "", "" };
                drift_rate_max = new string[] { "", "", "" };

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
                        cfdVerdict(buf[0]);
                    }
                    else if (cmd.Contains("?") && !buf[0].Equals(""))
                    {
                        await Task.Delay(CTPMain.waitTime * 100);
                        cfdResult(buf[0]);
                    }
                }
                CTPMain.resultVerdictIndex = row;
                CTPMain.testTime[(int)eBLECaseIdx.CFD] = (float)(DateTime.Now - startTime).TotalSeconds;

                bool isPass = addRows();
                //bool isPass = true;
                //for (int r = (int)eTestItem.ftx_fn_CH0_2M; r < (int)eTestItem.CFDTIME_2M; r++)
                //{
                //    //await Task.Delay(CTPMain.waitTime);
                //    int freIdx = (r - (int)eTestItem.avgCH0) % 3;
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];

                //    if (r <= (int)eTestItem.ftx_fn_CH39_2M)
                //    {
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df_max[freIdx];
                //    }
                //    else if ((int)eTestItem.ftx_fn_CH39_2M < r && r <= (int)eTestItem.f0_fn_CH39_2M)
                //    {
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = drift_rate_max[freIdx];
                //    }
                //    else if ((int)eTestItem.f0_fn_CH39_2M < r && r <= (int)eTestItem.f1_f0_CH39_2M)
                //    {
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = f_drift01_max[freIdx];
                //    }
                //    else if ((int)eTestItem.f1_f0_CH39_2M < r && r <= (int)eTestItem.fn_fn5_CH39_2M)
                //    {
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = drift_rate_max[freIdx];
                //    }

                //    if (lstVerdictCFD[freIdx].Equals("PASS"))
                //    {
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.BackColor = Color.Green;
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;
                //    }
                //    else
                //    {
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.BackColor = Color.Red;
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Style.ForeColor = Color.Yellow;
                //        isPass = false;
                //    }
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictCFD[freIdx];
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = "?";
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = "?";
                //}

                //CTPMain.wnd.mainBLEGrid.Rows[(int)eTestItem.CFDTIME_2M].Cells[(int)eCol.TST].Value =
                //    CTPMain.testTime[(int)eBLECaseIdx.CFD2];

                CTPMain.wnd.mainBLEGrid.RefreshEdit();
                CTPMain.wnd.pgrBar.Value = CTPMain.prgCount += 13;
                if (!checkError(CTPMain.selectedRFPort))
                    return false;
                string report = getCFDReport(CTPMain.testTime[(int)eBLECaseIdx.CFD2]);
                //CTPReportWindow.reportList.Add(report);
                //CTPReportWindow.passFailReport += report + "\n";

                return isPass;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("CFD2 Error", Color.Red);
                Util.openPopupOk("run cfod - " + e.Message);
                return false;
            }
        }

        private void cfdVerdict(string verdictBuffer)
        {
            if (!verdictBuffer.Equals(""))
            {
                verdictBuffer = verdictBuffer.TrimEnd('\r', '\n');
                var res = verdictBuffer.Split(',');
                int idx = 0;

                foreach (var re in res)
                    lstVerdictCFD[idx++] = re;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    lstVerdictCFD[i] = "FAIL";
                //error = ERROR.MTP300A_ERROR;
            }
        }

        private void cfdResult(string resultBuffer)
        {
            if (!resultBuffer.Equals(""))
            {
                resultBuffer = resultBuffer.TrimEnd('\r', '\n');
                var res = resultBuffer.Split(',');

                for (int i = 0; i < 3; i++)
                {
                    df_max[i] = res[(i * 4)];
                    f_drift01_max[i] = res[(i * 4) + 1];
                    f_drift_max[i] = res[(i * 4) + 2];
                    drift_rate_max[i] = res[(i * 4) + 3];
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    lstVerdictCFD[i] = "FAIL";
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
                CTPMain.wnd.setTextPgrBar("CFD2 Error", Color.Red);
                Util.openPopupOk("Error : " + ex.Message);
            }
        }
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
