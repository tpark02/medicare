using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Medicare.Main;
using Medicare.Port;
using Medicare.Setup;
using Medicare.Utility;
using Medicare.Visa;

namespace Medicare.BLE
{
    public partial class RS2 : Form
    {
        public static List<string> pkts_sent = new List<string>() { "", "", "" };
        public static List<string> pkts_rcvd = new List<string>() { "", "", "" };
        public static List<string> per = new List<string>() { "", "", "" };
        public static List<string> lstVerdictRS = new List<string>() { "", "", "" };
        public static List<string> lstDetailReportRS = new List<string>() { "", "", "" };
        public static List<string> lstErrorRS = new List<string>() { "", "", "" };

        public static string rs_per = string.Empty;
        public static string rs_level = string.Empty;
        public static string rs_dirty = string.Empty;

        //public static List<byte> dutBuffer = new List<byte>();
        //public static string dutBinary = string.Empty;

        private VisaManager vgr = null;
        private SerialPortManager sgr = null;

        public RS2()
        {
            InitializeComponent();

            dirtyComboBox.Items.Clear();
            dirtyComboBox.Items.Add("0: OFF");
            dirtyComboBox.Items.Add("1: ON");

            vgr = VisaManager.Instance;
            sgr = SerialPortManager.Instance;

            load();
            initColor();
        }

        private void initColor()
        {
            rs2CondLabel.BackColor = Color.FromArgb(0, 126, 222);
            rs2CondLabel.ForeColor = Color.White;

            rs2SpecLabel.BackColor = Color.FromArgb(0, 126, 222);
            rs2SpecLabel.ForeColor = Color.White;

            okButton.BackColor = Color.FromArgb(0, 54, 105);
            okButton.ForeColor = Color.White;

            cancelButton.BackColor = Color.FromArgb(0, 54, 105);
            cancelButton.ForeColor = Color.White;
        }
        public void clearResult()
        {
            pkts_sent = new List<string>() { "", "", "" };
            pkts_rcvd = new List<string>() { "", "", "" };
            per = new List<string>() { "", "", "" };
            lstVerdictRS = new List<string>() { "", "", "" };
            lstDetailReportRS = new List<string>() { "", "", "" };
        }
        public void load()
        {
            channelTextBox.Text = CTPMain.btConfigFile["rs2"]["channel"].ToString();
            packetNumTextBox.Text = CTPMain.btConfigFile["rs2"]["npackets"].ToString();
            dirtyComboBox.SelectedIndex = Int32.Parse(CTPMain.btConfigFile["rs2"]["dirty"].ToString());
            rsLevelTextBox.Text = CTPMain.btConfigFile["rs2"]["rxlevel"].ToString();
            perTextBox.Text = CTPMain.btConfigFile["rs2"]["per"].ToString();
        }

        public static void rsCommands(out List<string> list)
        {
            List<string> cmdList = new List<string>();
            var channel = CTPMain.channel[(int)eBLECaseIdx.RS2];
            //string rfport = CTPMain.selectedRFPort == 0 ? "RF1" : "RF2";
            // 1
            cmdList.Add("CNT,0");
            //cmdList.Add(string.Format("CMD,ROUT:GEN:PORT {0}", rfport)); // SG로 변경
            //cmdList.Add("WAIT,500");

            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:SENS2M:CHANnel {0}", channel)); //채널 0
            cmdList.Add("CMD,CONFigure:BT:LE:TCase:SENS2M:DLENgth 37");
            cmdList.Add("CMD,CONFigure:BT:LE:TCase:SENS2M:PATTern 0");
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:SENS2M:NPACkets {0}", CTPMain.packetNum[(int)eBLECaseIdx.RS2]));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:SENS2M:TXPOWer {0}", rs_level));
            cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:SENS2M:DIRTY {0}", rs_dirty));

            //cmdList.Add("DUT," + Util.hex2binary("0x00") + Util.hex2binary("0x00")); // Reset
            //cmdList.Add("DUT," + Util.hex2binary("0x40") +
            //            Util.hex2binary("0x94")); // 0x40 0x94 channel: 0, length:37, PRBS9

            //cmdList.Add(string.Format("CMD,ROUT:MEAS:PORT {0}", rfport)); // RX 포트 변경 시, TC port가 반대로 되어있어 변경해줘야함
            //cmdList.Add("WAIT,500");

            cmdList.Add("CMD,READ:BT:LE:TCase:SENS2M?");
            //cmdList.Add("DUT," + Util.hex2binary("0xC0") +
            //            Util.hex2binary("0x00")); // LE_Test_End,Control= 0xC0,Parameter= 0x00            
            cmdList.Add("CMD,READ:BT:LE:TCase:SENS2M:VERDict?"); // pass, fail 표시
            //cmdList.Add("DUT," + Util.hex2binary("0x00") + Util.hex2binary("0x00")); // Reset

            list = cmdList;
        }

        public static string getRSReport(float time)
        {
            string report = string.Empty;

            try
            {
                report = string.Format("==== Receiver sensitivity, 1 M ====\n" +
                                       ":::Initial Conditions:::\n" +
                                       "       Packets to transmit:  {0}\n" +
                                       "       RX (DUT) power:       {1} dBm\n" +
                                       "       Dirty TX mode:        {2} (0:OFF/1:ON)\n" +
                                       ":::Limits:::\n" +
                                       "       PER  <  {3} %\n" +
                                       ":::Results:::\n",
                    CTPMain.packetNum[(int)eBLECaseIdx.RS2],
                    rs_level,
                    rs_dirty,
                    rs_per);
                // (pkts_sent,pkts_rcvd,per) * channel 갯수
                report += ("ch    f(MHz)    pkts_sent    pkts_rcvd   per        Verdict\n");

                var channel = CTPMain.channel[(int)eBLECaseIdx.RS2].Split('-');

                for (int i = 0; i < 3; i++)
                {
                    string detailReport = string.Empty;
                    if (channel[i].Length != 2)
                        channel[i] = "0" + channel[i];
                    string freq = ((Int32.Parse(channel[i]) * 2) + 2402).ToString();
                    detailReport += String.Format(
                        "{0}    {1}      {2}           {3}          {4}       {5}\n",
                        channel[i],
                        freq,
                        pkts_sent[i],
                        pkts_rcvd[i],
                        per[i],
                        lstVerdictRS[i]);
                    lstDetailReportRS[i] = detailReport;
                }

                foreach (var item in lstDetailReportRS)
                    report += item;

                foreach (var item in lstErrorRS)
                {
                    if (!item.Equals(""))
                        report += string.Format("Test Case Error : {0}\n", item);
                }

                float totalTime = 0f;
                totalTime += float.Parse(time.ToString());
                report += string.Format("Test Time : {0}sec\n", totalTime.ToString("#.##"));
                report += "\n\n";
                return report;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("RS2 Error", Color.Red);
                report += string.Format("Test Case Error : {0}", e.Message);
                return report;
            }
        }

        private void save()
        {
            CTPMain.channel[(int)eBLECaseIdx.RS2] = channelTextBox.Text;
            CTPMain.packetNum[(int)eBLECaseIdx.RS2] = packetNumTextBox.Text;
            rs_dirty = dirtyComboBox.SelectedIndex.ToString();
            rs_level = rsLevelTextBox.Text;
            rs_per = perTextBox.Text;

            CTPMain.btConfigFile["rs2"]["channel"] = channelTextBox.Text;
            CTPMain.btConfigFile["rs2"]["npackets"] = packetNumTextBox.Text;
            CTPMain.btConfigFile["rs2"]["dirty"] = dirtyComboBox.SelectedIndex.ToString();
            CTPMain.btConfigFile["rs2"]["rxlevel"] = rsLevelTextBox.Text;
            CTPMain.btConfigFile["rs2"]["per"] = perTextBox.Text;
            CTPMain.btConfigFile.Save(CTPMain.rootFolder +
                                        CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] +
                                        CTPMain.btleConfigFolderName);
        }

        public async Task<bool> runRS(List<string> cmdList)
        {
            try
            {
                CTPMain.lstError = new List<string>();
                lstVerdictRS = new List<string>() { "", "", "" };
                lstDetailReportRS = new List<string>() { "", "", "" };
                pkts_sent = new List<string>() { "", "", "" };
                pkts_rcvd = new List<string>() { "", "", "" };
                per = new List<string>() { "", "", "" };
                DateTime startTime = DateTime.Now;

                List<string> buf = new List<string>();
                int row = CTPMain.resultVerdictIndex;
                double packetRcvPer = -1;

                foreach (var s in cmdList)
                {
                    buf.Clear();
                    await Task.Delay(CTPMain.waitTime);
                    var l = s.Split(',');
                    var cmd = l[1];

                    if (l[0].Equals("CMD"))
                    {
                        if (cmd.Contains(":SENS2M?"))
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
                        else if (cmd.Contains(":VERDict?"))
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
                    else if (l[0].Equals("DUT") && !cmd.Equals(Util.hex2binary("0xC0") + Util.hex2binary("0x00")))
                    {
                        var port = CTPMain.selectedRFPort == 0 ? "RF1" : "RF2";
                        var idx = CTPMain.selectedRFPort == 0 ? (int)ePort.DUT1 : (int)ePort.DUT2;

                        sgr.dutData[idx] = string.Empty;
                        //if (port == "RF1")
                        //    sgr.dutData[(int)ePort.DUT1] = string.Empty;
                        //else
                        //    sgr.dutData[(int)ePort.DUT2] = string.Empty;

                        sgr.dutBuffer.Clear();

                        var isOk = sgr.sendBitData(cmd, port);
                        if (!isOk) return false;

                        while (sgr.dutData[idx].Length < 16)
                            await Task.Delay(CTPMain.waitTime);

                        //if (port.Equals("RF1"))
                        //{
                        //    while (sgr.dutData[(int)ePort.DUT1].Length < 16)
                        //        await Task.Delay(CTPMain.waitTime);
                        //}
                        //else
                        //{
                        //    while (sgr.dutData[(int)ePort.DUT2].Length < 16)
                        //        await Task.Delay(CTPMain.waitTime);
                        //}
                    }
                    else if (l[0].Equals("DUT") && cmd.Equals(Util.hex2binary("0xC0") + Util.hex2binary("0x00")))
                    {
                        //var port = CTPMain.selectedRFPort == 0 ? "RF1" : "RF2";
                        //var idx = CTPMain.selectedRFPort == 0 ? (int)ePort.DUT1 : (int)ePort.DUT2;

                        //sgr.dutData[idx] = string.Empty;
                        ////if (port == "RF1")
                        ////    sgr.dutData[(int)ePort.DUT1] = string.Empty;
                        ////else
                        ////    sgr.dutData[(int)ePort.DUT2] = string.Empty;

                        //sgr.dutBuffer.Clear();

                        ////await Task.Delay(100);
                        //var isOk = sgr.sendBitData(cmd, port);
                        //if (!isOk) return false;

                        //while (sgr.dutData[idx].Length < 16)
                        //    await Task.Delay(CTPMain.waitTime);
                        //if (port.Equals("RF1"))
                        //{
                        //    while (sgr.dutData[(int)ePort.DUT1].Length < 16)
                        //        await Task.Delay(CTPMain.waitTime);
                        //}
                        //else
                        //{
                        //    while (sgr.dutData[(int)ePort.DUT2].Length < 16)
                        //        await Task.Delay(CTPMain.waitTime);
                        //}

                        //char[] arr = sgr.dutData[idx].ToCharArray();
                        //arr[0] = '0';
                        //sgr.dutData[idx] = new string(arr);
                        //int pktCnt = Convert.ToInt32(sgr.dutData[idx], 2);

                        //if (port.Equals("RF1"))
                        //{
                        //    char[] arr = sgr.dutData[(int)ePort.DUT1].ToCharArray();
                        //    arr[0] = '0';
                        //    sgr.dutData[(int)ePort.DUT1] = new string(arr);
                        //    pktCnt = Convert.ToInt32(sgr.dutData[(int)ePort.DUT1], 2);
                        //}
                        //else
                        //{
                        //    char[] arr = sgr.dutData[(int)ePort.DUT2].ToCharArray();
                        //    arr[0] = '0';
                        //    sgr.dutData[(int)ePort.DUT2] = new string(arr);
                        //    pktCnt = Convert.ToInt32(sgr.dutData[(int)ePort.DUT2], 2);
                        //}


                        //int totalCnt = Int32.Parse(CTPMain.packetNum[(int)eBLECaseIdx.RS2]);

                        //packetRcvPer = (double)pktCnt / totalCnt;
                        //packetRcvPer *= 100.0;

                        //buf.Add(CTPMain.packetNum[(int)eBLECaseIdx.RS2]);
                        //buf.Add(pktCnt.ToString());
                        //buf.Add(rs_per);
                    }
                    else if (l[0].Equals("CNT"))
                    {
                        CTPMain.dtmIdx = int.Parse(cmd);
                    }

                    //if (cmd.Contains(Util.hex2binary("0xC0") + Util.hex2binary("0x00")) && !buf[0].Equals(""))
                    //{
                    //    rsResult(buf);
                    //}

                    //if (cmd.Contains("VERDict?"))
                    //{
                    //    rsVerdict(packetRcvPer);
                    //}

                    if (cmd.Contains("VERDict?") && !buf[0].Equals(""))
                    {
                        await Task.Delay(CTPMain.waitTime * 100);
                        rsVerdict(buf[0]);
                    }
                    else if (cmd.Contains("?") && !buf[0].Equals(""))
                    {
                        await Task.Delay(CTPMain.waitTime * 100);
                        rsResult(buf[0]);
                    }
                }

                CTPMain.resultVerdictIndex = row;
                CTPMain.testTime[(int)eBLECaseIdx.RS2] = (float)(DateTime.Now - startTime).TotalSeconds;

                bool isPass = addRows();
                //bool isPass = true;
                //for (int r = (int)eTestItem.RV_PER_CH0_2M; r <= (int)eTestItem.RV_PER_CH39_2M; r++)
                //{
                //    //await Task.Delay(CTPMain.waitTime);
                //    int freIdx = (r - (int)eTestItem.avgCH0) % 3;
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];
                //    ;
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = pkts_rcvd[freIdx];

                //    if (lstVerdictRS[freIdx].Equals("PASS"))
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

                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictRS[freIdx];
                //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = per[freIdx];
                //}

                //CTPMain.wnd.mainBLEGrid.Rows[(int)eTestItem.RSTIME_2M].Cells[(int)eCol.TST].Value =
                //    CTPMain.testTime[(int)eBLECaseIdx.RS2];

                CTPMain.wnd.mainBLEGrid.RefreshEdit();
                CTPMain.wnd.pgrBar.Value = CTPMain.prgCount += 4;
                if (!checkError(CTPMain.selectedRFPort))
                    return false;
                string report = getRSReport(CTPMain.testTime[(int)eBLECaseIdx.RS2]);
                //CTPReportWindow.reportList.Add(report);
                //CTPReportWindow.passFailReport += report + "\n";

                return isPass;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("RS2 Error", Color.Red);
                Util.openPopupOk("run rs " + e.Message);
                return false;
            }
        }

        private bool addRows()
        {
            bool isPass = true;
            CTPMain.startIndex += 2;
            int len = CTPMain.startIndex + (int)eTestItem.RSTIME_2M - (int)eTestItem.RV_PER_CH0_2M;

            for (int r = CTPMain.startIndex; r < len; r++)
            {
                int freIdx = (r - CTPMain.startIndex) % 3;
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];
                int row = (r - CTPMain.startIndex) + (int)eTestItem.RV_PER_CH0_2M;

                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = pkts_rcvd[freIdx];

                if (lstVerdictRS[freIdx].Equals("PASS"))
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

                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictRS[freIdx];
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = per[freIdx];
            }
            CTPMain.wnd.mainBLEGrid.Rows[len].Cells[(int)eCol.TST].Value =
                CTPMain.testTime[(int)eBLECaseIdx.RS2];
            CTPMain.startIndex = len;
            return isPass;
        }
        private void rsVerdict(string verdictBuffer)
        {
            if (!verdictBuffer.Equals(""))
            {
                verdictBuffer = verdictBuffer.TrimEnd('\r', '\n');
                var res = verdictBuffer.Split(',');
                int idx = 0;

                foreach (var re in res)
                    lstVerdictRS[idx++] = re;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    lstVerdictRS[i] = "FAIL";
            }
        }

        private void rsResult(string resultBuffer)
        {
            if (!resultBuffer.Equals(""))
            {
                resultBuffer = resultBuffer.TrimEnd('\r', '\n');
                var res = resultBuffer.Split(',');
                for (int i = 0; i < 3; i++)
                {
                    pkts_sent[i] = res[(i * 3)];
                    pkts_rcvd[i] = res[(i * 3) + 1];
                    per[i] = res[(i * 3) + 2];
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    lstVerdictRS[i] = "FAIL";
                //error = ERROR.MTP300A_ERROR;
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
                CTPMain.wnd.setTextPgrBar("RS2 Error", Color.Red);
                Util.openPopupOk("Error : " + ex.Message);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dirtyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}