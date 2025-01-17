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
using System.Windows.Forms.VisualStyles;
using Medicare.Main;
using Medicare.Port;
using Medicare.Setup;
using Medicare.Utility;
using Medicare.Visa;

public partial class OP2 : Form
{
    // outputpower
    public static List<string> lstVerdictOP = new List<string>() { "", "", "" };
    public static List<string> lstDetailReportOP = new List<string>() { "", "", "" };
    public static List<string> maxPower = new List<string>() { "", "", "" };
    public static List<string> avgPower = new List<string>() { "", "", "" };
    public static List<string> diffMaxAvg = new List<string>() { "", "", "" };
    public static List<string> minPower = new List<string>() { "", "", "" };

    public static string pavg_low = "-20.00";
    public static string pavg_up = "10.00";
    public static string power_diff = "3.00";

    private VisaManager vgr = null;
    private SerialPortManager sgr = null;

    public OP2()
    {
        InitializeComponent();
        load();
        vgr = VisaManager.Instance;
        sgr = SerialPortManager.Instance;
        initColor();
    }
    private void initColor()
    {
        op2CondLabel.BackColor = Color.FromArgb(0, 126, 222);
        op2CondLabel.ForeColor = Color.White;

        op2SpecLabel.BackColor = Color.FromArgb(0, 126, 222);
        op2SpecLabel.ForeColor = Color.White;

        okButton.BackColor = Color.FromArgb(0, 54, 105);
        okButton.ForeColor = Color.White;

        cancelButton.BackColor = Color.FromArgb(0, 54, 105);
        cancelButton.ForeColor = Color.White;

    }
    public void clearResult()
    {
        lstVerdictOP = new List<string>() { "", "", "" };
        lstDetailReportOP = new List<string>() { "", "", "" };
        maxPower = new List<string>() { "", "", "" };
        avgPower = new List<string>() { "", "", "" };
        diffMaxAvg = new List<string>() { "", "", "" };
        minPower = new List<string>() { "", "", "" };
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
            CTPMain.wnd.setTextPgrBar("OP2 Error", Color.Red);
            Util.openPopupOk("Error : " + ex.Message);
        }
    }
    private void load()
    {
        channelTextBox.Text = CTPMain.channel[(int)eBLECaseIdx.OP2] = CTPMain.btConfigFile["op2"]["channel"].ToString();
        avgPwrLowerTextBox.Text = CTPMain.btConfigFile["op2"]["Pavg_low"].ToString();
        avgPwrUpperTextBox.Text = CTPMain.btConfigFile["op2"]["Pavg_up"].ToString();
        powerDiffTextBox.Text = CTPMain.btConfigFile["op2"]["Power_diff"].ToString();
        packetNumTextBox.Text = CTPMain.btConfigFile["op2"]["npackets"].ToString();
    }
    public static void opCommands(out List<string> list)
    {
        List<string> cmdList = new List<string>();
        var channel = CTPMain.channel[(int)eBLECaseIdx.OP2];
        // 1
        cmdList.Add("CNT,0");
        cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:OP2M:CHANnel {0}", channel));
        cmdList.Add("CMD,CONFigure:BT:LE:TCase:OP2M:DLENgth 37");
        cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:OP2M:NPACkets {0}", CTPMain.packetNum[(int)eBLECaseIdx.OP2]));
        cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:OP2M:LIMit:PAV_UP {0}", CTPMain.btConfigFile["op2"]["Pavg_up"].ToString()));
        cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:OP2M:LIMit:PAV_LOW {0}", CTPMain.btConfigFile["op2"]["Pavg_low"].ToString()));
        cmdList.Add(string.Format("CMD,CONFigure:BT:LE:TCase:OP2M:LIMit:DP_PAV_UP {0}", CTPMain.btConfigFile["op2"]["Power_diff"].ToString()));
        cmdList.Add(string.Format("CMD,READ:BT:LE:TCase:OP2M?"));
        cmdList.Add(string.Format("CMD,READ:BT:LE:TCase:OP2M:VERDict?"));

        list = cmdList;
    }
    public static string getOPReport(float time)
    {
        string report = string.Empty;
        try
        {
            report = string.Format("==== Output Power ====\n" +
                                   ":::Initial Conditions:::\n" +
                                   "   \tNum. of packets : \t{0}\n" +
                                   ":::Limits:::\n" +
                                   "   \t{1} dBm < Pav < {2} dBm\n" +
                                   "   \tPmax - Pav < {3} dB\n" +
                                   ":::Results:::\n"
                , CTPMain.packetNum[(int)eBLECaseIdx.OP2]
                , pavg_low
                , pavg_up
                , power_diff);

            //report += ("#ch\tf(MHz)\tPmin\tPav\tPmax\tVerdict\n");
            report += ("ch    f(MHz)   Pmin     Pav     Pmax    Verdict\n");

            var channel = CTPMain.channel[(int)eBLECaseIdx.OP2].Split('-');
            for (int i = 0; i < 3; i++)
            {
                string detailReport = string.Empty;
                string freq = ((Int32.Parse(channel[i]) * 2) + 2402).ToString();
                //detailReport += string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\n", channel[i], freq, minPower[i],
                if (channel[i].Length != 2)
                    channel[i] = "0" + channel[i];
                detailReport += string.Format("{0}    {1}    {2}    {3}    {4}    {5}\n", channel[i], freq, minPower[i],
                    avgPower[i], maxPower[i], lstVerdictOP[i]);
                lstDetailReportOP[i] = detailReport;
            }


            foreach (var str in lstDetailReportOP)
                report += str;

            foreach (var str in CTPMain.lstError)
            {
                if (!str.Equals(""))
                    report += string.Format("Test Case Error : {0}\n", str);
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
            CTPMain.wnd.setTextPgrBar("OP2 Error", Color.Red);
            report += string.Format("Test Case Error : {0}", e.Message);
            return report;
        }
    }

    private void save()
    {
        string s = channelTextBox.Text;
        var c = s.Split('-');
        CTPMain.channel[(int)eBLECaseIdx.OP2] = c[0] + "-" + c[1] + "-" + c[2];
        CTPMain.packetNum[(int)eBLECaseIdx.OP2] = packetNumTextBox.Text;
        pavg_low = avgPwrLowerTextBox.Text;
        pavg_up = avgPwrUpperTextBox.Text;
        power_diff = powerDiffTextBox.Text;

        CTPMain.btConfigFile["op2"]["channel"] = CTPMain.channel[(int)eBLECaseIdx.OP2];
        CTPMain.btConfigFile["op2"]["npackets"] = CTPMain.packetNum[(int)eBLECaseIdx.OP2];
        CTPMain.btConfigFile["op2"]["Pavg_up"] = pavg_up;
        CTPMain.btConfigFile["op2"]["Pavg_low"] = pavg_low;
        CTPMain.btConfigFile["op2"]["Power_diff"] = power_diff;
        CTPMain.btConfigFile.Save(CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] + CTPMain.btleConfigFolderName);
    }
    public async Task<bool> runOP(List<string> cmdList)
    {
        try
        {
            // init
            //CTPMain.lstError = new List<string>();
            lstVerdictOP = new List<string>() { "", "", "" };
            lstDetailReportOP = new List<string>() { "", "", "" };
            maxPower = new List<string>() { "", "", "" };
            avgPower = new List<string>() { "", "", "" };
            diffMaxAvg = new List<string>() { "", "", "" };
            minPower = new List<string>() { "", "", "" };

            //error = ERROR.NO_ERROR;

            string resultBuffer = string.Empty;
            string verdictBuffer = string.Empty;
            List<string> buf = new List<string>();

            DateTime startTime = DateTime.Now;

            int row = CTPMain.resultVerdictIndex;

            for (int i = 0; i < cmdList.Count; i++)
            {
                buf.Clear();
                await Task.Delay(CTPMain.waitTime);
                var l = cmdList[i].Split(',');
                var cmd = l[1];

                if (l[0].Equals("CMD"))
                {
                    if (cmd.Contains("?"))
                    {
                        if (vgr.writeVisa(cmd, CTPMain.selectedRFPort))
                        {
                            await Task.Delay(CTPMain.waitTime * 100);

                            string res = vgr.readVisa(cmd, CTPMain.selectedRFPort);
                            Console.WriteLine(res);
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
                    opVerdict(buf[0]);
                }
                else if (cmd.Contains("?") && !buf[0].Equals(""))
                {
                    await Task.Delay(CTPMain.waitTime * 100);
                    opResult(buf[0]);
                }
            }
            CTPMain.resultVerdictIndex = row;
            CTPMain.testTime[(int)eBLECaseIdx.OP2] = (float)(DateTime.Now - startTime).TotalSeconds;
            bool isPass = addRows();
            //bool isPass = true;
            //for (int r = (int)eTestItem.avgCH0_2M; r < (int)eTestItem.OPTIME_2M; r++)
            //{
            //    int freIdx = (r - (int)eTestItem.avgCH0_2M) % 3;
            //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];

            //    if (r <= (int)eTestItem.avgCH39_2M)
            //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = avgPower[freIdx];
            //    else
            //        CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = diffMaxAvg[freIdx];

            //    if (lstVerdictOP[freIdx].Equals("PASS"))
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

            //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictOP[freIdx];
            //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = pavg_low;
            //    CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = pavg_up;
            //}
            //CTPMain.wnd.mainBLEGrid.Rows[(int)eTestItem.OPTIME_2M].Cells[(int)eCol.TST].Value = CTPMain.testTime[(int)eBLECaseIdx.OP2];
            CTPMain.wnd.mainBLEGrid.RefreshEdit();

            CTPMain.wnd.pgrBar.Value = CTPMain.prgCount += 7;
            
            //if (!checkError(CTPMain.selectedRFPort))
            //    return false;

            string report = getOPReport(CTPMain.testTime[(int)eBLECaseIdx.OP2]);
            //CTPReportWindow.reportList.Add(report);
            //CTPReportWindow.passFailReport += report + "\n";

            return isPass;
        }
        catch (Exception e)
        {
            CTPMain.wnd.setTextPgrBar("OP2 Error", Color.Red);
            Util.openPopupOk("run op - " + e.Message);
            return false;
        }
    }
    private bool addRows()
    {
        bool isPass = true;
        CTPMain.startIndex += 2;
        int len = CTPMain.startIndex + (int)eTestItem.OPTIME_2M - (int)eTestItem.avgCH0_2M;

        for (int r = CTPMain.startIndex; r < len; r++)
        {
            int freIdx = (r - CTPMain.startIndex) % 3;
            CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.FREQ].Value = CTPMain.chFreq[freIdx];
            int row = (r - CTPMain.startIndex) + (int)eTestItem.avgCH0_2M;

            if ((int)eTestItem.avgCH0_2M <= row && row <= (int)eTestItem.avgCH39_2M)
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = avgPower[freIdx];
            else
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.MEAS].Value = diffMaxAvg[freIdx];

            if (lstVerdictOP[freIdx].Equals("PASS"))
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

            CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.RES].Value = lstVerdictOP[freIdx];
            
            if ((int)eTestItem.avgCH0_2M <= row && row <= (int)eTestItem.avgCH39_2M)
            {
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = pavg_low;
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = pavg_up;
            }
            else
            {
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.LOWER].Value = "";
                CTPMain.wnd.mainBLEGrid.Rows[r].Cells[(int)eCol.UPPER].Value = power_diff;
            }
        }
        CTPMain.wnd.mainBLEGrid.Rows[len].Cells[(int)eCol.TST].Value =
            CTPMain.testTime[(int)eBLECaseIdx.OP2];

        CTPMain.startIndex = len;
        return isPass;
    }
    private void opVerdict(string verdictBuffer)
    {
        // Verdict
        if (!verdictBuffer.Equals(""))
        {
            verdictBuffer = verdictBuffer.TrimEnd('\r', '\n');
            var res = verdictBuffer.Split(',');
            int idx = 0;

            foreach (var re in res)
                lstVerdictOP[idx++] = re;
        }
        else
        {
            for (int i = 0; i < 3; i++)
                lstVerdictOP[i] = "FAIL";
        }
    }
    private void opResult(string resultBuffer)
    {
        if (!resultBuffer.Equals(""))
        {
            resultBuffer = resultBuffer.TrimEnd('\r', '\n');
            var res = resultBuffer.Split(',');
            for (int i = 0; i < 3; i++)
            {
                avgPower[i] = res[(i * 4)];
                maxPower[i] = res[(i * 4) + 1];
                diffMaxAvg[i] = res[(i * 4) + 2];
                minPower[i] = res[(i * 4) + 3];
            }
        }
        else
        {
            for (int i = 0; i < 3; i++)
                lstVerdictOP[i] = "FAIL";
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
                    //CTPMain.lstError.Add(e);
                }
            }
        }

        return true;
    }

    private void OP_FormClosing(object sender, FormClosingEventArgs e)
    {
        save();
    }

    private void cancelButton_Click_1(object sender, EventArgs e)
    {
        this.Close();
    }

}
