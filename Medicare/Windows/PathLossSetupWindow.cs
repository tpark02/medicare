using Microsoft.VisualBasic.FileIO;
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
using Medicare.Main;
using Medicare.Utility;
using Medicare.Visa;

namespace Medicare.PathLoss
{
    public partial class PathLossSetupWindow : Form
    {
        private int[] channel = null;
        private uint[] freqList = null;

        int[] wifiChannel = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60, 62, 64, 100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 132, 134, 136, 138, 140, 142, 144, 149, 151, 153, 155, 157, 159, 161, 165, 172, 174, 176, 178, 180, 182, 184 };
        uint[] wifiFreqList = new uint[] { 2412, 2417, 2422, 2427, 2432, 2437, 2442, 2447, 2452, 2457, 2462, 2467, 2472, 2484, 5170, 5180, 5190, 5200, 5210, 5220, 5230, 5240, 5250, 5260, 5270, 5280, 5290, 5300, 5310, 5320, 500, 510, 520, 530, 540, 550, 560, 570, 580, 590, 600, 610, 620, 630, 640, 660, 670, 680, 690, 700, 710, 720, 745, 755, 765, 775, 785, 795, 805, 825, 860, 870, 880, 890, 900, 910, 920 };

        private int[] bleChannel = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39 };
        private uint[] bleFreqList = new uint[] { 
                        2402000000, 2404000000, 2406000000, 2408000000, 
            2410000000, 2412000000, 2414000000, 2416000000, 2418000000, 
            2420000000, 2422000000, 2424000000, 2426000000, 2428000000, 
            2430000000, 2432000000, 2434000000, 2436000000, 2438000000, 
            2440000000, 2442000000, 2444000000, 2446000000, 2448000000, 
            2450000000, 2452000000, 2454000000, 2456000000, 2458000000, 
            2460000000, 2462000000, 2464000000, 2466000000, 2468000000, 
            2470000000, 2472000000, 2474000000, 2476000000, 2478000000, 
            2480000000 };

        private VisaManager visaMgr = null;
        public PathLossSetupWindow()
        {
            visaMgr = VisaManager.Instance;

            InitializeComponent();

            initGrid();
            if (!readPathLoss())
                return;

            StartPosition = FormStartPosition.CenterScreen;
        }

        private void initGrid()
        {
            if (CTPMain.isBLE)
            {
                channel = bleChannel;
                freqList = bleFreqList;
            }
            else
            {
                channel = wifiChannel;
                //freqList = wifiFreqList;
            }

            int row = 0;

            for (int i = 0; i < channel.Length; i++)
                pathLossGrid.Rows.Insert(i, "", "", "0", "0", "0", "0");

            pathLossGrid.Invalidate();

            for (int i = 0; i < channel.Length; i++)
            {
                pathLossGrid.Rows[i].Cells["CH"].Value = channel[i].ToString();
                pathLossGrid.Rows[i].Cells["Freq"].Value = freqList[i].ToString();
            }
        }
        private void generateDataTable(string fileName, bool firstRowContainsFieldNames = true)
        {
            string delimiters = ",";
            string extension = Path.GetExtension(fileName);

            if (extension.ToLower() == "txt")
                delimiters = "\t";
            else if (extension.ToLower() == "csv")
                delimiters = ",";

            using (TextFieldParser tfp = new TextFieldParser(fileName))
            {
                tfp.SetDelimiters(delimiters);

                // Get The Column Names
                if (!tfp.EndOfData)
                {
                    string[] fields = tfp.ReadFields();
                }

                // Get Remaining Rows from the CSV
                while (!tfp.EndOfData)
                {
                    pathLossGrid.Rows.Add(tfp.ReadFields());
                }
            }
        }
        private void loadButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // CSV 만 필터
            openFileDialog.Filter = ".csv files (*.csv)|*.csv";
            //현재 실행프로그램 경로
            openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // load 전에 모든 row 삭제 
                while (pathLossGrid.Rows.Count > 0)
                {
                    pathLossGrid.Rows.RemoveAt(pathLossGrid.Rows.Count - 1);
                }
                generateDataTable(openFileDialog.FileName, true);
            }
        }
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (Util.openPopupOkCancel("Save as a file ?") == System.Windows.Forms.DialogResult.OK)
            {
                if (pathLossGrid.Rows.Count > 0)
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.Filter = "CSV (*.csv)|*.csv";
                    sfd.FileName = CTPMain.rootFolder + @"\ble_pathloss.csv";
                    bool fileError = false;

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        if (File.Exists(sfd.FileName))
                        {
                            try
                            {
                                File.Delete(sfd.FileName);
                            }
                            catch (IOException ex)
                            {
                                CTPMain.wnd.setTextPgrBar("Path loss window Error", Color.Red);

                                fileError = true;
                                //MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                                Util.openPopupOk("It wasn't possible to write the data to the disk." + ex.Message);
                            }
                        }
                        if (!fileError)
                        {
                            try
                            {
                                int columnCount = 4;
                                //int columnCount = pathLossGrid.Columns.Count;
                                string columnNames = "";
                                string[] outputCsv = new string[pathLossGrid.Rows.Count + 1];
                                for (int i = 0; i < columnCount; i++)
                                {
                                    columnNames += pathLossGrid.Columns[i].HeaderText.ToString() + ",";
                                }
                                outputCsv[0] += columnNames;

                                for (int i = 1; (i - 1) < pathLossGrid.Rows.Count; i++)
                                {
                                    string col = string.Empty;
                                    for (int j = 0; j < columnCount; j++)
                                    {
                                        if (pathLossGrid.Rows[i - 1].Cells[j].Value != null)
                                        {
                                            col += pathLossGrid.Rows[i - 1].Cells[j].Value.ToString() + ",";
                                        }
                                        else
                                            col += ',';
                                    }
                                    outputCsv[i] += col;
                                }

                                File.WriteAllLines(sfd.FileName, outputCsv, Encoding.UTF8);
                                //MessageBox.Show("Data Exported Successfully !!!", "Info");
                                Util.openPopupOk("Data Exported Successfully !!!");
                            }
                            catch (Exception ex)
                            {
                                CTPMain.wnd.setTextPgrBar("Path loss window Error", Color.Red);

                                //MessageBox.Show("Error :" + ex.Message);
                                Util.openPopupOk("Error : " + ex.Message);
                            }
                        }
                    }
                }
                else
                {
                    //MessageBox.Show("No Record To Export !!!", "Info");
                    Util.openPopupOk("No Record To Export !!!");
                }
            }
        }
        private bool readPathLoss()
        {
            string rf1Tx = "CONF:BT:MEAS:P1_PLOSS?";    // 신호 분석기 설정
            string rf2Tx = "CONFigure:BT:MEAS:P2_PLOSS?";

            string rf1Rx = "SOURce:BT:GEN:P1_PLOSS?";     // 신호 발생기 설정
            string rf2Rx = "SOURce:BT:GEN:P2_PLOSS?";

            int n = channel.Length;

            if (!visaMgr.writeVisa(rf1Tx, 0))
                return false;

            string res = visaMgr.readVisa(rf1Tx, 0);

            if (res.Contains('?'))
            {
                Util.openPopupOk("RF1 TX Pathloss 설정을 확인해주세요.");
                return false;
            }
            var list = res.Split('/');
            int idx = 0;
            foreach (var str in list)
            {
                var val = str.Split('@');
                pathLossGrid.Rows[idx++].Cells["RF1_TX"].Value = val[1];
            }

            visaMgr.writeVisa(rf1Rx, 0);
            res = visaMgr.readVisa(rf1Rx, 0);

            if (res.Contains('?'))
            {
                Util.openPopupOk("RF1 RX Pathloss 설정을 확인해주세요.");
                return false;
            }
            list = res.Split('/');
            idx = 0;
            foreach (var str in list)
            {
                var val = str.Split('@');
                pathLossGrid.Rows[idx++].Cells["RF1_RX"].Value = val[1];
            }

            return true;
            visaMgr.writeVisa(rf2Tx, 0);
            res = visaMgr.readVisa(rf2Tx, 1);

            if (res.Contains('?'))
            {
                Util.openPopupOk("RF2 TX Pathloss 설정을 확인해주세요.");
                return false;
            }
            list = res.Split('/');
            idx = 0;
            foreach (var str in list)
            {
                var val = str.Split('@');
                pathLossGrid.Rows[idx++].Cells["RF2_TX"].Value = val[1];
            }

            visaMgr.writeVisa(rf2Rx, 0);
            res = visaMgr.readVisa(rf2Rx, 1);

            if (res.Contains('?'))
            {
                Util.openPopupOk("RF2 RX Pathloss 설정을 확인해주세요.");
                return false;
            }
            list = res.Split('/');
            idx = 0;
            foreach (var str in list)
            {
                var val = str.Split('@');
                pathLossGrid.Rows[idx++].Cells["RF2_RX"].Value = val[1];
            }

        }

        private void editingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl tb = (DataGridViewTextBoxEditingControl)e.Control;
            tb.KeyPress += new KeyPressEventHandler(keyPress);
            e.Control.KeyPress += new KeyPressEventHandler(keyPress);
        }

        private void keyPress(object sender, KeyPressEventArgs e)
        {
            //when i press enter,bellow code never run?
            //if (!('0' <= e.KeyChar && e.KeyChar <= '9') || e.KeyChar == '.') e.Handled = true;
            if (('a' <= e.KeyChar && e.KeyChar <= 'z') || ('A' <= e.KeyChar && e.KeyChar <= 'Z'))
            {
                e.Handled = true;
            }
        }
        private void readButton_Click(object sender, EventArgs e)
        {
            readPathLoss();
        }

        private void writeButton_Click(object sender, EventArgs e)
        {
            string rf1Tx = "CONF:BT:MEAS:P1_PLOSS";     // 신호 분석기 설정
            string rf2Tx = "CONFigure:BT:MEAS:P2_PLOSS";

            string rf1Rx = "SOURce:BT:GEN:P1_PLOSS";     // 신호 발생기 설정
            string rf2Rx = "SOURce:BT:GEN:P2_PLOSS";

            int n = 0;

            if (CTPMain.isBLE)
                n = freqList.Length;
            else
                n = channel.Length;

            string val = string.Empty;

            rf1Tx += ' ';
            for (int i = 0; i < n - 1; i++) // rf1 tx configure
            {
                val = pathLossGrid.Rows[i].Cells["RF1_TX"].Value.ToString();

                if (val.Contains("\n"))
                {
                    val = val.Split('\n')[0];
                }

                if (CTPMain.isBLE)
                    rf1Tx += freqList[i].ToString() + "@" + val.ToString() + "/";
                else
                    rf1Tx += "CH" + channel[i].ToString() + "@" + val.ToString() + "/";
            }

            val = pathLossGrid.Rows[n - 1].Cells["RF1_TX"].Value.ToString();

            if (CTPMain.isBLE)
                rf1Tx += freqList[n - 1].ToString() + "@" + val.ToString();
            else
                rf1Tx += "CH" + channel[n - 1].ToString() + "@" + val.ToString();

            visaMgr.writeVisa(rf1Tx, 0);

            rf1Rx += ' ';
            for (int i = 0; i < n - 1; i++) // rf1 rx configure
            {
                val = pathLossGrid.Rows[i].Cells["RF1_RX"].Value.ToString();

                if (val.Contains("\n"))
                {
                    val = val.Split('\n')[0];
                }

                if (CTPMain.isBLE)
                    rf1Rx += freqList[i].ToString() + "@" + val.ToString() + "/";
                else
                    rf1Rx += "CH" + channel[i].ToString() + "@" + val.ToString() + "/";
            }
            val = pathLossGrid.Rows[n - 1].Cells["RF1_RX"].Value.ToString();

            if (CTPMain.isBLE)
                rf1Rx += freqList[n - 1].ToString() + "@" + val.ToString();
            else
                rf1Rx += "CH" + channel[n - 1].ToString() + "@" + val.ToString();

            visaMgr.writeVisa(rf1Rx, 0);
            return;
            rf2Tx += ' ';
            for (int i = 0; i < n - 1; i++) // rf2 tx configure
            {
                val = pathLossGrid.Rows[i].Cells["RF2_TX"].Value.ToString();

                if (val.Contains("\n"))
                {
                    val = val.Split('\n')[0];
                }

                if (CTPMain.isBLE)
                    rf2Tx += freqList[i].ToString() + "@" + val.ToString() + "/";
                else
                    rf2Tx += "CH" + channel[i].ToString() + "@" + val.ToString() + "/";
            }
            val = pathLossGrid.Rows[n - 1].Cells["RF2_TX"].Value.ToString();

            if (CTPMain.isBLE)
                rf2Tx += freqList[n - 1].ToString() + "@" + val.ToString();
            else
                rf2Tx += "CH" + channel[n - 1].ToString() + "@" + val.ToString();

            visaMgr.writeVisa(rf2Tx, 1);

            rf2Rx += ' ';
            for (int i = 0; i < n - 1; i++) // rf2 rx configure
            {
                val = pathLossGrid.Rows[i].Cells["RF2_RX"].Value.ToString();

                if (val.Contains("\n"))
                {
                    val = val.Split('\n')[0];
                }

                if (CTPMain.isBLE)
                    rf2Rx += freqList[i].ToString() + "@" + val.ToString() + "/";
                else
                    rf2Rx += "CH" + channel[i].ToString() + "@" + val.ToString() + "/";
            }
            val = pathLossGrid.Rows[n - 1].Cells["RF2_RX"].Value.ToString();

            if (CTPMain.isBLE)
                rf2Rx += freqList[n - 1].ToString() + "@" + val.ToString();
            else
                rf2Rx += "CH" + channel[n - 1].ToString() + "@" + val.ToString();

            visaMgr.writeVisa(rf2Rx, 1);

        }
    }
}
