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
    public partial class MOD : Form
    {
        // Modulation Characteristic
        public static string df1avglower = string.Empty;
        public static string df1avgupper = string.Empty;
        public static string df2max = string.Empty;
        public static string df2avgdf1avg = string.Empty;

        public static string lower_df2_rate = "99.90";
        public static string lower_df2_df1 = "80.00";

        public static List<string> lstVerdictMOD = new List<string>() { "", "", "" };
        public static List<string> lstDetailReportMOD = new List<string>() { "", "", "" };
        public static string[] df1_avg = new string[] { "", "", "" };
        public static string[] df2_avg = new string[] { "", "", "" };
        public static string[] df2_min = new string[] { "", "", "" };
        public static string[] df2_rate = new string[] { "", "", "" };
        public static string[] df2_df1 = new string[] { "", "", "" };

        //private ERROR error = ERROR.NO_ERROR;
        private VisaManager vgr = null;
        private SerialPortManager sgr = null;
        public MOD()
        {
            InitializeComponent();
            //loadSetup();
            load();
            vgr = VisaManager.Instance;
            sgr = SerialPortManager.Instance;
            initColor();
        }

        private void initColor()
        {
            modCondLabel.BackColor = Color.FromArgb(0, 126, 222);
            modCondLabel.ForeColor = Color.White;

            modSpecLabel.BackColor = Color.FromArgb(0, 126, 222);
            modSpecLabel.ForeColor = Color.White;

            okButton.BackColor = Color.FromArgb(0, 54, 105);
            okButton.ForeColor = Color.White;

            cancelButton.BackColor = Color.FromArgb(0, 54, 105);
            cancelButton.ForeColor = Color.White;
        }
        public void clearResult()
        {
            lstVerdictMOD = new List<string>() { "", "", "" };
            lstDetailReportMOD = new List<string>() { "", "", "" };
            df1_avg = new string[] { "", "", "" };
            df2_avg = new string[] { "", "", "" };
            df2_min = new string[] { "", "", "" };
            df2_rate = new string[] { "", "", "" };
            df2_df1 = new string[] { "", "", "" };
        }
        private void load()
        {
            channelTextBox.Text = CTPMain.btConfigFile["mo"]["channel"].ToString();
            packetNumTextBox.Text = CTPMain.btConfigFile["mo"]["npackets"].ToString();
            df1avgUpperTextBox.Text = CTPMain.btConfigFile["mo"]["df1avgupper"].ToString();
            df1avgLowerTextBox.Text = CTPMain.btConfigFile["mo"]["df1avglower"].ToString();
            df2maxTextBox.Text = CTPMain.btConfigFile["mo"]["df2max"].ToString();
            df2avgdf1avgTextBox.Text = CTPMain.btConfigFile["mo"]["df2avgdf1avg"].ToString();
        }
        public static void modCharacteristicsCommands(out List<string> list)
        {
            List<string> cmdList = new List<string>();
            var channel = CTPMain.channel[(int)eBLECaseIdx.MOD];
            // 1
            cmdList.Add("CNT,0");
            cmdList.Add(String.Format("CMD,CONF:BT:LE:TC:MOD:CHANnel {0}", channel));
            cmdList.Add("CMD,CONF:BT:LE:TC:MOD:DLENgth 37");
            cmdList.Add(string.Format("CMD,CONF:BT:LE:TC:MOD:NPACkets {0}", CTPMain.packetNum[(int)eBLECaseIdx.MOD]));
            cmdList.Add(string.Format("CMD,CONF:BT:LE:TC:MOD:LIM:DF1_AVG_UP {0}", df1avgupper));
            cmdList.Add(string.Format("CMD,CONF:BT:LE:TC:MOD:LIM:DF1_AVG_LOW {0}", df1avglower));
            cmdList.Add(string.Format("CMD,CONF:BT:LE:TC:MOD:LIM:NUM_DF2_L {0}", df2max));
            cmdList.Add(string.Format("CMD,CONF:BT:LE:TC:MOD:LIM:DF2_DF1_LOW {0}", df2avgdf1avg));
            cmdList.Add("CMD,READ:BT:LE:TC:MOD?");
            cmdList.Add("CMD,READ:BT:LE:TCase:MOD:VERDict?");
            list = cmdList;
        }
        public static string getMODReport(float time)
        {
            string report = "";
            try
            {
                report = string.Format("==== Modulation Characteristics ====\n" +
                                       ":::Initial Conditions:::\n" +
                                       "   \tNum. of packets : \t{0}\n" +
                                       ":::Limits:::\n" +
                                       "   \t{1} kHz =< df1_avg =< {2} kHz\n" +
                                       "   \tMax. Num. df2 rate >=  {3} %\n" +
                                       "   \tdf2/df1 >= {4}\n" +
                                       ":::Results:::\n"
                    , CTPMain.packetNum[(int)eBLECaseIdx.MOD]
                    , df1avglower
                    , df1avgupper
                    , lower_df2_rate
                    , lower_df2_df1);

                report += String.Format("ch    f(MHz)  df1_avg  df2_avg   df2_min    df2rate    df2/df1   Verdict\n");

                var channel = CTPMain.channel[(int)eBLECaseIdx.MOD].Split('-');

                for (int i = 0; i < 3; i++)
                {
                    string detailReport = string.Empty;
                    if (channel[i].Length != 2)
                        channel[i] = "0" + channel[i];
                    string freq = ((Int32.Parse(channel[i]) * 2) + 2402).ToString();
                    detailReport += string.Format(
                        "{0}    {1}    {2}      {3}      {4}      {5}      {6}      {7}\n",
                        channel[i], freq,
                        df1_avg[i],
                        df2_avg[i],
                        df2_min[i],
                        df2_rate[i],
                        df2_df1[i],
                        lstVerdictMOD[i]);
                    lstDetailReportMOD[i] = detailReport;
                }

                foreach (var item in lstDetailReportMOD)
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
                CTPMain.wnd.setTextPgrBar("MOD Error", Color.Red);
                report += string.Format("Test Case Error : {0}", e.Message);
                return report;
            }
        }
        private void save()
        {
            CTPMain.channel[(int)eBLECaseIdx.MOD] = channelTextBox.Text;
            CTPMain.packetNum[(int)eBLECaseIdx.MOD] = packetNumTextBox.Text;
            df1avgupper = df1avgUpperTextBox.Text;
            df1avglower = df1avgLowerTextBox.Text;
            df2max = df2maxTextBox.Text;
            df2avgdf1avg = df2avgdf1avgTextBox.Text;

            CTPMain.btConfigFile["mo"]["channel"] = CTPMain.channel[(int)eBLECaseIdx.MOD];
            CTPMain.btConfigFile["mo"]["npackets"] = CTPMain.packetNum[(int)eBLECaseIdx.MOD];
            CTPMain.btConfigFile["mo"]["df1avgupper"] = df1avgupper;
            CTPMain.btConfigFile["mo"]["df1avglower"] = df1avglower;
            CTPMain.btConfigFile["mo"]["df2max"] = df2max;
            CTPMain.btConfigFile["mo"]["df2avgdf1avg"] = df2avgdf1avg;

            CTPMain.btConfigFile.Save(CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] + CTPMain.btleConfigFolderName);
        }
        public async Task<bool> runMOD(List<string> cmdList)
        {
            try
            {
                CTPMain.lstError = new List<string>();
                lstVerdictMOD = new List<string>() { "", "", "" };
                lstDetailReportMOD = new List<string>() { "", "", "" };
                df1_avg = new string[] { "", "", "" };
                df2_avg = new string[] { "", "", "" };
                df2_min = new string[] { "", "", "" };
                df2_rate = new string[] { "", "", "" };
                df2_df1 = new string[] { "", "", "" };
                // measure
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
                        modVerdict(buf[0]);
                    }
                    else if (cmd.Contains("?") && !buf[0].Equals(""))
                    {
                        await Task.Delay(CTPMain.waitTime * 100);
                        modResult(buf[0]);
                    }
                }
                CTPMain.resultVerdictIndex = row;
                CTPMain.testTime[(int)eBLECaseIdx.MOD] = (float)(DateTime.Now - startTime).TotalSeconds;
                bool isPass = addRows();
                //bool isPass = true;
                //for (int r = (int)eTestItem.df1_avg_CH0; r < (int)eTestItem.MODTIME; r++)
                //{
                //    //await Task.Delay(CTPMain.waitTime);
                //    int freIdx = (r - (int)eTestItem.df1_avg_CH0) % 3;
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];

                //    if (r <= (int)eTestItem.df2_max_CH0)
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df1_avg[freIdx];
                //    else if ((int)eTestItem.df2_max_CH0 < r && r <= (int)eTestItem.df2_max_CH39)
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df2_avg[freIdx];
                //    else
                //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df2_df1[freIdx];

                //    if (lstVerdictMOD[freIdx].Equals("PASS"))
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
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictMOD[freIdx];
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = df1avglower;
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = df1avgupper;
                //}
                //CTPMain.wnd.mainBLEGrid.Rows[(int)eTestItem.MODTIME].Cells[(int)eCol.TST].Value = CTPMain.testTime[(int)eBLECaseIdx.MOD];
                CTPMain.wnd.mainBLEGrid.RefreshEdit();
                CTPMain.wnd.pgrBar.Value = CTPMain.prgCount += 10;
                if (!checkError(CTPMain.selectedRFPort))
                    return false;
                string report = getMODReport(CTPMain.testTime[(int)eBLECaseIdx.MOD]);
                //CTPReportWindow.reportList.Add(report);
                //CTPReportWindow.passFailReport += report + "\n";

                return isPass;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("MOD Error", Color.Red);
                Util.openPopupOk("run mod - " + e.Message);
                return false;
            }
        }

        private bool addRows()
        {
            bool isPass = true;
            CTPMain.startIndex += 2;
            int len = CTPMain.startIndex + (int)eTestItem.MODTIME - (int)eTestItem.df1_avg_CH0;

            for (int r = CTPMain.startIndex; r < len; r++)
            {
                int freIdx = (r - CTPMain.startIndex) % 3;
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];
                int row = (r - CTPMain.startIndex) + (int)eTestItem.df1_avg_CH0;

                if (row < (int)eTestItem.df2_max_CH0)
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df1_avg[freIdx];
                else if ((int)eTestItem.df2_max_CH0 <= row && row <= (int)eTestItem.df2_max_CH39)
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df2_rate[freIdx];
                else
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = df2_df1[freIdx];

                if (lstVerdictMOD[freIdx].Equals("PASS"))
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
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictMOD[freIdx];

                if (row < (int)eTestItem.df2_max_CH0)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = df1avglower;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = df1avgupper;
                }
                else if ((int)eTestItem.df2_max_CH0 <= row && row <= (int)eTestItem.df2_max_CH39)
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = df2max;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = "";
                }
                else
                {
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = df2avgdf1avg;
                    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = "";
                }
            }
            CTPMain.wnd.mainBLEGrid.Rows[len].Cells[(int)eCol.TST].Value =
                CTPMain.testTime[(int)eBLECaseIdx.MOD];

            CTPMain.startIndex = len;
            return isPass;
        }

        private void modResult(string resultBuffer)
        {
            // result buffer
            if (!resultBuffer.Equals(""))
            {
                resultBuffer = resultBuffer.TrimEnd('\r', '\n');
                var res = resultBuffer.Split(',');

                for (int i = 0; i < 3; i++)
                {
                    df1_avg[i] = res[(i * 5)];
                    df2_avg[i] = res[(i * 5) + 1];
                    df2_min[i] = res[(i * 5) + 2];
                    df2_rate[i] = res[(i * 5) + 3];
                    df2_df1[i] = res[(i * 5) + 4];
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    lstVerdictMOD[i] = "FAIL";
            }
        }
        private void modVerdict(string verdictBuffer)
        {
            if (!verdictBuffer.Equals(""))
            {
                verdictBuffer = verdictBuffer.TrimEnd('\r', '\n');
                var res = verdictBuffer.Split(',');
                int idx = 0;
                foreach (var re in res)
                    lstVerdictMOD[idx++] = re;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    lstVerdictMOD[i] = "FAIL";
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
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
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
                CTPMain.wnd.setTextPgrBar("MOD Error", Color.Red);
                Util.openPopupOk("Error : " + ex.Message);
            }
        }
    }

}