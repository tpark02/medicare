using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;
using Medicare.BLE;
using Medicare.Main;
using Medicare.OK;
using Medicare.OKCancel;
using Medicare.Setup;
using Medicare.Visa;

namespace Medicare.Utility
{
    public enum eReportCol
    {
        NUM,
        RF_Port,
        Start_TIME,
        Test_TIME,
        OverAllTime,
        QRCode,
        BD_ADDRESS,
        FINAL_RESULT,
        BLE_OP_AVG_POWER_CH0,
        BLE_OP_MAX_POWER_CH0,
        BLE_OP_AVG_POWER_CH19,
        BLE_OP_MAX_POWER_CH19,
        BLE_OP_AVG_POWER_CH39,
        BLE_OP_MAX_POWER_CH39,
        BLE_MOD_DF1_AVG_CH0,
        BLE_MOD_DF2_RATE_CH0,
        BLE_MOD_DF2_DF1_CH0,
        BLE_MOD_DF1_AVG_CH19,
        BLE_MOD_DF2_RATE_CH19,
        BLE_MOD_DF2_DF1_CH19,
        BLE_MOD_DF1_AVG_CH39,
        BLE_MOD_DF2_RATE_CH39,
        BLE_MOD_DF2_DF1_CH39,
        BLE_CFOD_FTX_FN_CH0,
        BLE_CFOD_F0_FN_CH0,
        BLE_CFOD_F1_F0_CH0,
        BLE_CFOD_FN_FN5_CH0,
        BLE_CFOD_FTX_FN_CH19,
        BLE_CFOD_F0_FN_CH19,
        BLE_CFOD_F1_F0_CH19,
        BLE_CFOD_FN_FN5_CH19,
        BLE_CFOD_FTX_FN_CH39,
        BLE_CFOD_F0_FN_CH39,
        BLE_CFOD_F1_F0_CH39,
        BLE_CFOD_FN_FN5_CH39,
        BLE_SENS_PER_CH0,
        BLE_SENS_PER_CH19,
        BLE_SENS_PER_CH3,
        MAX
    }
    public enum ResStatus
    {
        NONE = 0,
        FAIL = 1,
        PASS = 2,
    }
    public enum eStatus
    {
        READY,
        PAUSED,
        STOP,
        RUNNING,
        FINISH,
        ERROR,
    }
    class Util
    {                
        public const int rowCount = 50;
        public static double cv = 0;
        public static int transmittedPacketCount = 0;
        //private static string _dutResult { get; set; }      
        //public static string dutResult = string.Empty;
        //public static string dutLog = string.Empty;

        //public static DateTime EndDate = DateTime.Today.AddDays(1).AddSeconds(-1); // 실전 24시간용
        public static DateTime endDate = DateTime.Now.AddMinutes(15);

        public static string projectPath = Directory.GetCurrentDirectory();

        public static List<string> logList
        {
            get { return _logList; }
        }
        private static List<string> _logList = new List<string>();
        public static string dutLog = string.Empty;
        Util()
        {
        }

        public static DialogResult openPopupOk(string msg)
        {
            addLog(msg, "-");
            PopupOK p = new PopupOK();
            p.setPopupMsg("\n\n" + msg);
            return p.ShowDialog();
        }
        public static DialogResult openPopupOkCancel(string msg)
        {
            addLog(msg, "-");
            PopupOkCancel p = new PopupOkCancel();
            p.setPopupMsg(msg);
            return p.ShowDialog();
        }
        // 바이트 배열을 String으로 변환 
        public static string ByteToString(byte[] strByte)
        {
            string str = Encoding.UTF8.GetString(strByte);
            return str;
        }
        // String을 바이트 배열로 변환 
        public static byte[] StringToByte(string str)
        {
            byte[] StrByte = Encoding.UTF8.GetBytes(str);
            return StrByte;
        }
        public static double convertToPPM(double fv)
        {
            return (fv * 1000000) / cv;
        }
        public static string hex2binary(string hexvalue)
        {
            string s = Convert.ToString(Convert.ToInt32(hexvalue, 16), 2).PadLeft(8, '0'); ;
            return s; 
        }
        public static void addLog(string str, string portName)
        {
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("dd MMMM yyyy HH: mm:ss");
            _logList.Add("[" + formattedDateTime + "][" + portName + "]" + str + "\n");
            Console.WriteLine("[" + formattedDateTime + "][" + portName + "]" + str);
        }
        public static void addLog(string str, int rfPortIndex)
        {
            string rfport = rfPortIndex == 0 ? "RF1" : "RF2";

            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("dd MMMM yyyy HH: mm:ss");
            _logList.Add("[" + formattedDateTime + "][" + rfport + "]" + str + "\n");
            Console.WriteLine("[" + formattedDateTime + "][" + rfport + "]" + str);
        }
        public static void writeLog(string rfport)
        {
            string logFilePath = Directory.GetCurrentDirectory() + CTPMain.logFolder;
            string fileNameDateTime = DateTime.Now.ToString("[yyyy-dd-MM HH_mm_ss]");
            string fileName = fileNameDateTime + rfport + "_LOG.txt";
            string path = Path.Combine(logFilePath, fileName);
            string text = String.Join("", _logList.Select(p => p));
            File.WriteAllText(path, text);
        }

        public static void writeLog24()
        {
            if (DateTime.Now > endDate)
            {
                string logFilePath = Path.Combine(projectPath, "Logs");
                string fileNameDateTime = DateTime.Now.ToString("[yyyy-dd-MM HH_mm_ss]");
                string fileName = fileNameDateTime + "_LOG.txt";
                string path = Path.Combine(logFilePath, fileName);
                string text = String.Join("", _logList.Select(p => p));
                File.WriteAllText(path, text);
                //EndDate = DateTime.Today.AddDays(1).AddSeconds(-1); // // 실전 24시간용
                endDate = DateTime.Now.AddMinutes(15);
                _logList.Clear();
            }
        }
        public static string[] colNames = new string[]
        {
            "NUM", "RF Port", "Start TIME", "Test TIME", "OverallTime", "SW Ver", "BD_ADDRESS", "FINAL_RESULT",
            "PASS_FAIL",
            // OP
            "OP_CHANNEL",
            "BLE_OP_AVG_POWER_CH0", "BLE_OP_MAX_POWER_CH0", "BLE_OP_AVG_POWER_CH19",
            "BLE_OP_MAX_POWER_CH19", "BLE_OP_AVG_POWER_CH39", "BLE_OP_MAX_POWER_CH39",
            // OP 2M
            "OP2_CHANNEL",
            "BLE_OP_2M_AVG_POWER_CH0", "BLE_OP_2M_MAX_POWER_CH0", "BLE_OP_2M_AVG_POWER_CH19",
            "BLE_OP_2M_MAX_POWER_CH19", "BLE_OP_2M_AVG_POWER_CH39", "BLE_OP_2M_MAX_POWER_CH39",
            // MOD
            "MOD_CHANNEL",
            "BLE_MOD_DF1_AVG_CH0", "BLE_MOD_DF2_RATE_CH0", "BLE_MOD_DF2_DF1_CH0",
            "BLE_MOD_DF1_AVG_CH19", "BLE_MOD_DF2_RATE_CH19", "BLE_MOD_DF2_DF1_CH19",
            "BLE_MOD_DF1_AVG_CH39", "BLE_MOD_DF2_RATE_CH39", "BLE_MOD_DF2_DF1_CH39",
            // MOD 2M
            "MOD2_CHANNEL",
            "BLE_MOD_2M_DF1_AVG_CH0", "BLE_MOD_2M_DF2_RATE_CH0", "BLE_MOD_2M_DF2_DF1_CH0",
            "BLE_MOD_2M_DF1_AVG_CH19", "BLE_MOD_2M_DF2_RATE_CH19", "BLE_MOD_2M_DF2_DF1_CH19",
            "BLE_MOD_2M_DF1_AVG_CH39", "BLE_MOD_2M_DF2_RATE_CH39", "BLE_MOD_2M_DF2_DF1_CH39",
            // CFOD
            "CFOD_CHANNEL",
            "BLE_CFOD_FTX_FN_CH0", "BLE_CFOD_F0_FN_CH0", "BLE_CFOD_F1_F0_CH0", "BLE_CFOD_FN_FN5_CH0",
            "BLE_CFOD_FTX_FN_CH19", "BLE_CFOD_F0_FN_CH19", "BLE_CFOD_F1_F0_CH19", "BLE_CFOD_FN_FN5_CH19",
            "BLE_CFOD_FTX_FN_CH39", "BLE_CFOD_F0_FN_CH39", "BLE_CFOD_F1_F0_CH39", "BLE_CFOD_FN_FN5_CH39",
            // CFOD 2M
            "CFOD2_CHANNEL",
            "BLE_CFOD_2M_FTX_FN_CH0", "BLE_CFOD_2M_F0_FN_CH0", "BLE_CFOD_2M_F1_F0_CH0", "BLE_CFOD_2M_FN_FN5_CH0",
            "BLE_CFOD_2M_FTX_FN_CH19", "BLE_CFOD_2M_F0_FN_CH19", "BLE_CFOD_2M_F1_F0_CH19", "BLE_CFOD_2M_FN_FN5_CH19",
            "BLE_CFOD_2M_FTX_FN_CH39", "BLE_CFOD_2M_F0_FN_CH39", "BLE_CFOD_2M_F1_F0_CH39", "BLE_CFOD_2M_FN_FN5_CH39",
            // RS
            "RS_CHANNEL",
            "BLE_SENS_PER_CH0", "BLE_SENS_PER_CH19", "BLE_SENS_PER_CH39",
            // RS2
            "RS2_CHANNEL",
            "BLE_SENS_2M_PER_CH0", "BLE_SENS_2M_PER_CH19", "BLE_SENS_2M_PER_CH39",
            // CFOM
            "CFOM_CHANNEL",
            "CFOM_DF0_MAX_CH0", "CFOM_DF0_MAX_CH19", "CFOM_DF0_MAX_CH39", "CFOM_DF0_MIN_CH0", "CFOM_DF0_MIN_CH19", "CFOM_DF0_MIN_CH39", "CFOM_DF0_AVG_CH0", "CFOM_DF0_AVG_CH19", "CFOM_DF0_AVG_CH39", "CFOM_DF2_MIN_CH0", "CFOM_DF2_MIN_CH19", "CFOM_DF2_MIN_CH39", "CFOM_DF2_AVG_CH0", "CFOM_DF2_AVG_CH19", "CFOM_DF2_AVG_CH39",
            // DOWNLOAD
            "DOWNLOAD"
        };

        public static string GetSystemInfo(string path, string arg)
        {
            var command = arg;
            var cmdsi = new ProcessStartInfo(path);
            cmdsi.Arguments = command;
            cmdsi.RedirectStandardOutput = true;
            cmdsi.UseShellExecute = false;
            var cmd = Process.Start(cmdsi);
            var output = cmd.StandardOutput.ReadToEnd();

            cmd.WaitForExit();
            Console.WriteLine(output);
            return output;
        }
        public static bool downloadDTM(string commander_serial_number, int idx, string filename)
        {
            var prevWorking = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = CTPMain.commanderDir;

                //string currpath = CTPMain.commanderDir + @"\Flash_commander_24_0206\Flash_commander";
                //string exepath = currpath + @"\commander.exe";
                //string filepath = currpath + @"\" + filename;
                string tail = " --device EFR32BG22C222F352GM32 --serialno ";

                string[] arglist = new string[3]
                {
                    "security erasedevice ",
                    "flash ",
                    "verify "
                };
                string[] argcmd = new string[3]
                {
                    arglist[0] + tail + commander_serial_number,
                    arglist[1] + filename + tail + commander_serial_number,
                    arglist[2] + filename + tail + commander_serial_number,
                };

                //ProcessStartInfo info = new ProcessStartInfo("commander.exe");
                Process p = new Process();
                p.StartInfo.FileName = "commander.exe";
                p.StartInfo.Arguments = argcmd[idx];
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.WaitForExit();
                string result = p.StandardOutput.ReadToEnd();
                Console.WriteLine("commander.exe download : " + result);

                if (result.Contains("failed"))
                {
                    openPopupOk(filename + "\n\tDonwload Incomplete.");
                    return false;
                } 
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("downloadDTM Error", Color.Red);
                openPopupOk("Bitbang 실행 에러 : " + e.Message + "\n" + "Setup >> Bitbang 경로 설정을 해주세요.");
                return false;
            }
            finally
            {
                Environment.CurrentDirectory = prevWorking;
            }

            return true;
        }
        public static void savePassFailReport(bool isFail)
        {
            var port = "RF1";
            var fileName = DateTime.Now.ToString("yyyyddMM") + "_" + DateTime.Now.ToString("HH_mm_ss") + "_" +
                           port + ".csv";

            try
            {
                string dir = string.Empty;
                if (!isFail)
                {
                    dir = CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] +
                          @"\Result\Report\Pass\";
                }
                else
                {
                    dir = CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] +
                          @"\Result\Report\Fail\";
                }

                string res = string.Empty;

                try
                {
                    res = string.Join(",", colNames);
                    //res = res.Remove(res.Length - 1) + Environment.NewLine;
                    res += Environment.NewLine;

                    res += "1" + ",";
                    res = createResult(res);
                    //insertQuery(res);

                    File.WriteAllText(dir + fileName, res + Environment.NewLine, Encoding.UTF8);
                    Util.addLog("Pass/Fail Result File : " + fileName + " is created.", CTPMain.selectedRFPort);
                }
                catch (Exception e)
                {
                    CTPMain.wnd.setTextPgrBar("save pass fail report Error", Color.Red);
                    Util.openPopupOk("Pass/Fail Result File : " + fileName + " is failed to be created.\nError - " +
                                     e.Message);
                    Util.addLog("Pass/Fail Result File : " + fileName + " is failed to be created.\nError - " + e.Message,
                        CTPMain.selectedRFPort);
                }

                Util.addLog("A new Pass/Fail result file : " + dir + fileName, CTPMain.selectedRFPort);
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("save pass fail report Error", Color.Red);
                Util.openPopupOk("Pass/Fail Result File : " + fileName + ".\nError - " +
                                 e.Message);
                Util.addLog("Pass/Fail Result File : " + fileName + ".\nError - " + e.Message,
                    CTPMain.selectedRFPort);
            }
        }

        public static void closeFile()
        {
            var fileName = CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] + @"\Result\Report\" + DateTime.Now.ToString("yyyyddMM") + ".csv";
            StreamReader rs = null;

            try
            {
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    rs = new StreamReader(stream);
                    rs.Dispose();
                }
            }
            catch (Exception e)
            {
                if (rs != null)
                    rs.Dispose();

            }
        }

        public static void saveReport()
        {
            var fileName = CTPMain.rootFolder + CTPMain.modelFolderName[Int32.Parse(NewSetupWindow.selectedModelIdx)] + @"\Result\Report\" + DateTime.Now.ToString("yyyyddMM") + ".csv";
            string res = string.Empty;
            StreamReader rs = null;
            StreamWriter sw = null;

            try
            {
                if (File.Exists(fileName))
                {
                    try
                    {
                        using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                        {
                            rs = new StreamReader(stream);
                            {
                                string lastLine = "";

                                while (rs.EndOfStream == false)
                                    lastLine = rs.ReadLine();

                                var list = lastLine.Split(',');
                                int num = Int32.Parse(list[0]);

                                num += 1;
                                res += num.ToString() + ",";
                                res = createResult(res);

                                sw = new StreamWriter(stream, Encoding.UTF8);
                                {
                                    sw.WriteLine(res);
                                    sw.Close();
                                    sw.Dispose();
                                }
                                rs.Dispose();
                            }
                            //insertQuery(res);
                        }
                    }
                    catch (Exception e)
                    {
                        CTPMain.wnd.setTextPgrBar("save report Error", Color.Red);
                        // 이미 열려있는 파일을 open 하는경우 예외 발생
                        if (rs != null)
                            rs.Dispose();
                        if (sw != null)
                            sw.Dispose();
                        Util.openPopupOk("Result File : " + fileName + " is opened by another program. Please close the file.");
                        Util.addLog("Result File : " + fileName + ".  Please close the file.", CTPMain.selectedRFPort);
                    }
                }
                else  // file does not exist - create a file
                {
                    try
                    {
                        res = string.Join(",", colNames);
                        //res = res.Remove(res.Length - 1) + Environment.NewLine;
                        res += Environment.NewLine;

                        res += "1" + ",";
                        res = createResult(res);
                        //insertQuery(res);

                        File.WriteAllText(fileName, res + Environment.NewLine, Encoding.UTF8);
                        Util.addLog("Result File : " + fileName + " is created.", CTPMain.selectedRFPort);
                    }
                    catch (Exception e)
                    {
                        CTPMain.wnd.setTextPgrBar("save report Error", Color.Red);
                        Util.openPopupOk("Result File : " + fileName + " is failed to be created.\nError - " +
                                         e.Message);
                        Util.addLog("Result File : " + fileName + " is failed to be created.\nError - " + e.Message,
                            CTPMain.selectedRFPort);
                    }
                }
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("save report Error", Color.Red);
                Util.openPopupOk("Result File : " + fileName + ".\nError - " +
                                 e.Message);
                Util.addLog("Result File : " + fileName + ".\nError - " + e.Message,
                    CTPMain.selectedRFPort);
            }
        }

        //private static void insertQuery(string res)
        //{
        //    // insert data into DB
        //    var dblist = res.Split(',');
        //    string query = string.Empty;
        //    query = string.Format("insert into {0}", DBManager.Instance.tableName);
        //    query += " (EQUIPMENT_ID, TESTER_VER, MODEL, WORK_TIME, MCU_ID, CHANNEL, SPEC_FILE, TACT_TIME, TOTAL_JUDGMENT, NG_ITEM, BD_ADDRESS, BLE_OP_AVG_POWER_CH0, BLE_OP_MAX_POWER_CH0, BLE_OP_AVG_POWER_CH19, BLE_OP_MAX_POWER_CH19, BLE_OP_AVG_POWER_CH39, BLE_OP_MAX_POWER_CH39, BLE_MOD_DF1_AVG_CH0, BLE_MOD_DF2_RATE_CH0, BLE_MOD_DF2_DF1_CH0, BLE_MOD_DF1_AVG_CH19, BLE_MOD_DF2_RATE_CH19, BLE_MOD_DF2_DF1_CH19, BLE_MOD_DF1_AVG_CH39, BLE_MOD_DF2_RATE_CH39, BLE_MOD_DF2_DF1_CH39,BLE_CFOD_FTX_FN_CH0, BLE_CFOD_F0_FN_CH0, BLE_CFOD_F1_F0_CH0, BLE_CFOD_FN_FN5_CH0, BLE_CFOD_FTX_FN_CH19, BLE_CFOD_F0_FN_CH19, BLE_CFOD_F1_F0_CH19, BLE_CFOD_FN_FN5_CH19,  BLE_CFOD_FTX_FN_CH39, BLE_CFOD_F0_FN_CH39, BLE_CFOD_F1_F0_CH39, BLE_CFOD_FN_FN5_CH39, BLE_SENS_PER_CH0, BLE_SENS_PER_CH19, BLE_SENS_PER_CH39)";
        //    query += " values (";
        //    query += "'test',";     // EQUIPMENT_ID     0
        //    query += "'test',";     // TESTER_VER       1
        //    query += "'test',";     // MODEL            2
        //    query += string.Format("'{0}',", dblist[(int)eReportCol.OverAllTime]);     // WORK_TIME        3
        //    query += "'test',";     // MCU_ID           4
        //    query += "'test',";     // CHANNEL          5
        //    query += "'test',";     // SPEC_FILE        6
        //    query += string.Format("'{0}',", dblist[(int)eReportCol.Test_TIME]);     // TACT_TIME        7
        //    query += string.Format("'{0}',", dblist[(int)eReportCol.FINAL_RESULT]);     // TOTAL_JUDGMENT   8
        //    query += "'test',";     // NG_ITEM          9
        //    query += "'test',";     //  BD_ADDRESS      10

        //    for (int i = (int)eReportCol.BLE_OP_AVG_POWER_CH0; i < (int)eReportCol.MAX - 1; i++)
        //        query += string.Format("'{0}',", dblist[i]);

        //    query += string.Format("'{0}')", dblist[(int)eReportCol.MAX - 1]);
        //    DBManager.Instance.ExecuteNonQuery(query);
        //    Console.WriteLine(" ** query insert : " + query);
        //}

        private static string createResult(string res)
        {
            res += CTPMain.selectedRFPort.ToString() + ",";
            res += CTPMain.wnd.stTime.ToString("HH:mm:ss") + ",";
            res += CTPMain.wnd.tactTime.ToString() + ",";
            res += CTPMain.wnd.overallTime.ToString() + ",";
            res += CTPMain.swVersion + ",";
            res += CTPMain.bdAddr + ",";
            res += CTPMain.wnd.finalResult + ",";
            res += CTPMain.passfailstring + ",";
            // op
            res += CTPMain.channel[(int)eBLECaseIdx.OP].ToString() + ',';
            res += OP.avgPower[0] + ",";
            res += OP.diffMaxAvg[0] + ",";
            res += OP.avgPower[1] + ",";
            res += OP.diffMaxAvg[1] + ",";
            res += OP.avgPower[2] + ",";
            res += OP.diffMaxAvg[2] + ",";
            // op 2M
            res += CTPMain.channel[(int)eBLECaseIdx.OP2].ToString() + ',';
            res += OP2.avgPower[0] + ",";
            res += OP2.diffMaxAvg[0] + ",";
            res += OP2.avgPower[1] + ",";
            res += OP2.diffMaxAvg[1] + ",";
            res += OP2.avgPower[2] + ",";
            res += OP2.diffMaxAvg[2] + ",";
            // mod
            res += CTPMain.channel[(int)eBLECaseIdx.MOD].ToString() + ',';
            res += MOD.df1_avg[0] + ",";
            res += MOD.df2_avg[0] + ",";
            res += MOD.df2_df1[0] + ",";
            res += MOD.df1_avg[1] + ",";
            res += MOD.df2_avg[1] + ",";
            res += MOD.df2_df1[1] + ",";
            res += MOD.df1_avg[2] + ",";
            res += MOD.df2_avg[2] + ",";
            res += MOD.df2_df1[2] + ",";
            // mod2
            res += CTPMain.channel[(int)eBLECaseIdx.MOD2].ToString() + ',';
            res += MOD2.df1_avg[0] + ",";
            res += MOD2.df2_avg[0] + ",";
            res += MOD2.df2_df1[0] + ",";
            res += MOD2.df1_avg[1] + ",";
            res += MOD2.df2_avg[1] + ",";
            res += MOD2.df2_df1[1] + ",";
            res += MOD2.df1_avg[2] + ",";
            res += MOD2.df2_avg[2] + ",";
            res += MOD2.df2_df1[2] + ",";
            // cfod
            res += CTPMain.channel[(int)eBLECaseIdx.CFD].ToString() + ',';
            res += CFD.df_max[0] + ",";
            res += CFD.drift_rate_max[0] + ",";
            res += CFD.f_drift01_max[0] + ",";
            res += CFD.drift_rate_max[0] + ",";

            res += CFD.df_max[1] + ",";
            res += CFD.drift_rate_max[1] + ",";
            res += CFD.f_drift01_max[1] + ",";
            res += CFD.drift_rate_max[1] + ",";

            res += CFD.df_max[2] + ",";
            res += CFD.drift_rate_max[2] + ",";
            res += CFD.f_drift01_max[2] + ",";
            res += CFD.drift_rate_max[2] + ",";
            // cfod2
            res += CTPMain.channel[(int)eBLECaseIdx.CFD2].ToString() + ',';
            res += CFD2.df_max[0] + ",";
            res += CFD2.drift_rate_max[0] + ",";
            res += CFD2.f_drift01_max[0] + ",";
            res += CFD2.drift_rate_max[0] + ",";

            res += CFD2.df_max[1] + ",";
            res += CFD2.drift_rate_max[1] + ",";
            res += CFD2.f_drift01_max[1] + ",";
            res += CFD2.drift_rate_max[1] + ",";

            res += CFD2.df_max[2] + ",";
            res += CFD2.drift_rate_max[2] + ",";
            res += CFD2.f_drift01_max[2] + ",";
            res += CFD2.drift_rate_max[2] + ",";
            // rs
            res += CTPMain.channel[(int)eBLECaseIdx.RS].ToString() + ',';
            res += RS.per[0] + ",";
            res += RS.per[1] + ",";
            res += RS.per[2] + ",";

            // rs2
            res += CTPMain.channel[(int)eBLECaseIdx.RS2].ToString() + ',';

            res += RS2.per[0] + ",";
            res += RS2.per[1] + ",";
            res += RS2.per[2] + ",";

            // CFOM
            res += CTPMain.channel[(int)eBLECaseIdx.CFOM].ToString() + ',';

            res += CFOM.df0_max[0] + ",";
            res += CFOM.df0_max[1] + ",";
            res += CFOM.df0_max[2] + ",";
                                   
            res += CFOM.df0_min[0] + ",";
            res += CFOM.df0_min[1] + ",";
            res += CFOM.df0_min[2] + ",";
                                   
            res += CFOM.df0_avg[0] + ",";
            res += CFOM.df0_avg[1] + ",";
            res += CFOM.df0_avg[2] + ",";
                                   
            res += CFOM.df2_min[0] + ",";
            res += CFOM.df2_min[1] + ",";
            res += CFOM.df2_min[2] + ",";
                               
            res += CFOM.df2_avg[0] + ",";
            res += CFOM.df2_avg[1] + ",";
            res += CFOM.df2_avg[2] + ",";

            string downloadMsg = string.Empty;
            if (CTPMain.isUserFirmware)
            {
                downloadMsg = CTPMain.isDownload ? "OK" : "FAIL";
            }
            else
            {
                downloadMsg = "N/A";
            }

            res += downloadMsg;
            return res;
        }
        public static string MakeUnique(string dir)
        {
            var fileName = DateTime.Now.ToString("yyyyddMM") + ".txt";
            string path = dir + fileName;

            for (int i = 1; ; ++i)
            {
                if (!File.Exists(path))
                    return path;

                path = Path.Combine(dir, fileName + "_" + i + ".txt");
            }
        }
        public static void openFolder(string path)
        {
            Process.Start(Directory.GetCurrentDirectory() + path);
        }

        public static bool checkUserFirmWareDownload()
        {
            string path = CTPMain.commanderDir + "/dwsuccess";
            string[] textValue = System.IO.File.ReadAllLines(path);
            int okcnt = 0;
            if (textValue.Length > 0)
            {
                for (int i =0; i < textValue.Length; i++)
                {
                    Console.WriteLine(textValue[i]);
                    if (textValue[i].Contains("OK!"))
                        okcnt++;
                }
            }

            return okcnt >= 2;
        }
        public static bool IsCheckNetwork()
        {
            bool networkState = NetworkInterface.GetIsNetworkAvailable();
            bool pingResult = false;

            try
            {
                //네트워크가 연결이 되어있다면
                if (networkState)
                {
                    string addr = VisaManager.ip; // 상태 체크 ip 입력
                    Ping pingSender = new Ping();

                    //Ping 체크 (IP, TimeOut 지정)
                    PingReply reply = pingSender.Send(addr, 300);

                    //상태가 Success이면 true반환
                    pingResult = reply.Status == IPStatus.Success;
                }
            }
            catch (Exception e)
            {
                Util.openPopupOk("MTP300 connection is lost. Please check MTP300 connection.");
                VisaManager.Instance.closeVisaSession();
                return false;
            }

            return networkState & pingResult;
        }
    }
}
