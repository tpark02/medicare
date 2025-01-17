using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Medicare.Dut;
using Medicare.Protocol;
using Medicare.Utility;

namespace Medicare.Manager
{
    enum Category
    {
        SELECT = 0,
        TYPE,
        COMMAND,
        DISPLAY_NAME,
        ACTION,
        PAR1,
        CASENG,
        DELAY,
        TIME_OUT,
        RETRY,
        COMPARE,
        MIN,
        MAX,
        CAT,
    }

    enum Device
    {
        MTP300,
        NordicnRF7002,
    }

    class DataManager : Singleton<DataManager>
    {
        public static int[] passCount = new int[2] { 0, 0 };
        public static int[] failCount = new int[2] { 0, 0 };
        public static int[] totalCount = new int[2] { 0, 0 };
        //public static string projectPath = Directory.GetCurrentDirectory();

        public List<Dictionary<string, string>> rf1ExeList = new List<Dictionary<string, string>>();
        public List<Dictionary<string, string>> rf2ExeList = new List<Dictionary<string, string>>();
        public ProtocolManager protocl = null;
        //public Task<List<Tuple<string, int>>>[] taskList = new Task<List<Tuple<string, int>>>[2];
        public List<double> maskLimits = new List<double>();
        //public List<string> logList
        //{
        //    get { return _logList; }
        //}
        public bool[] isStopRF = new bool[2] { false, false };
        //private List<string> _logList = new List<string>();
        private List<StringBuilder> cmdList = new List<StringBuilder>();
        private string currentRxType = "";
        public DataManager()
        {
            if (protocl == null)
                protocl = ProtocolManager.Instance;   
        }
        public void resetCommandList(string selectedRF)
        {
            cmdList.Clear();
            if (selectedRF.Equals("RF1"))
                rf1ExeList.Clear();
            else
                rf2ExeList.Clear();
        }

        public void dgvToString(DataGridView dgv, char delimiter, string selectedRF)
        {
            StringBuilder sb = null;
            bool isEmpty = false;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                isEmpty = false;

                if (row.Cells[(int)Category.COMMAND].Value == null || row.Cells[(int)Category.COMMAND].Value.Equals(""))
                {
                    isEmpty = true;
                    continue;
                }

                sb = new StringBuilder();

                foreach (var e in Enum.GetValues(typeof(Category)))
                {
                    if (row.Cells[(int)e].Value == null && (int) Category.CAT != (int) e)
                        continue;
                    else if (row.Cells[(int)e].Value == null && (int)Category.CAT == (int) e)
                        sb.Append("");
                    else
                        sb.Append(row.Cells[(int)e].Value);
                    sb.Append(delimiter);
                }

                string type = row.Cells[(int)Category.TYPE].Value.ToString();

                if (type.Equals(Device.NordicnRF7002.ToString())) // NordicnRF7002의 par1의 값이 min <= x <= max 여부 체크
                {
                    bool isRange = DutManager.Instance.checkRange(row.Index);

                    if (!isRange)
                        return;
                }

                if (!isEmpty)
                {
                    sb.Remove(sb.Length - 1, 1); // Removes the last delimiter
                    cmdList.Add(sb);
                }
            }
            parseDGV(selectedRF);
        }

        public void parseDGV(string selectedRF)
        {
            if (cmdList.Count <= 0)
            {
                Util.openPopupOk("No commands !!!");
                return;
            }

            foreach (var str in cmdList)
            {
                string[] words = str.ToString().Split(';');
                string exec = string.Empty;
                string param = words[(int)Category.PAR1]; //MTP300의 par1은 A|B 형태를 split해서 B를 인자로 exec에 추가

                string device = words[(int)Category.TYPE];
                string cmd = words[(int)Category.COMMAND];
                string name = words[(int)Category.DISPLAY_NAME];
                string action = words[(int)Category.ACTION];
                string caseng = words[(int)Category.CASENG];
                string delay = words[(int)Category.DELAY];
                string timeout = words[(int)Category.TIME_OUT];
                string retry = words[(int)Category.RETRY];

                string minValue = words[(int)Category.MIN];
                string maxValue = words[(int)Category.MAX];

                string compare = words[(int)Category.COMPARE];
                string isSelected = words[(int)Category.SELECT];
                string cat = words[(int)Category.CAT];

                switch (device)
                {
                    case nameof(Device.MTP300):
                        if (cmd.IndexOf("<@>") >= 0)
                        {
                            string[] par1 = param.Split('|');
                            exec = cmd.Substring(0, cmd.IndexOf("<@>")) + par1[1];
                        }
                        else if (cmd.IndexOf("<#>") >= 0)
                        {
                            exec = cmd.Substring(0, cmd.IndexOf("<#>")) + param;
                        }
                        else
                        {
                            exec = cmd;
                        }
                        break;
                    case nameof(Device.NordicnRF7002):
                        param = words[(int)Category.PAR1].ToString();

                        if (param.Equals("NA"))
                            exec = "wifi_radio_test " + cmd;
                        else
                            exec = "wifi_radio_test " + cmd + " " + param;

                        exec += "\n";
                        break;
                }

                if (device.Equals(nameof(Device.MTP300)) && cmd.Last().Equals('?'))
                {
                    bool isValueInteger = ProtocolManager.Instance.checkMinMax(minValue, maxValue);

                    if (!isValueInteger)
                        return;
                }

                var dic = new Dictionary<string, string>();
                {
                    dic.Add(Category.SELECT.ToString(), isSelected);
                    dic.Add(Category.TYPE.ToString(), device);
                    dic.Add(Category.COMMAND.ToString(), exec);
                    dic.Add(Category.DISPLAY_NAME.ToString(), name);
                    dic.Add(Category.ACTION.ToString(), action);
                    dic.Add(Category.CASENG.ToString(), caseng);
                    dic.Add(Category.DELAY.ToString(), delay);
                    dic.Add(Category.TIME_OUT.ToString(), timeout);
                    dic.Add(Category.RETRY.ToString(), retry);
                    dic.Add(Category.MIN.ToString(), minValue);
                    dic.Add(Category.MAX.ToString(), maxValue);
                    dic.Add(Category.COMPARE.ToString(), compare);
                    dic.Add(Category.PAR1.ToString(), param);
                    dic.Add(Category.CAT.ToString(), cat);
                }
                if (selectedRF.Equals("RF1"))
                    rf1ExeList.Add(dic);
                else
                    rf2ExeList.Add(dic);
            }
        }
        public void resetStop(string selectedRF)
        {
            var taskIndex = selectedRF.Equals("RF1") ? 0 : 1;
            isStopRF[taskIndex] = false;
        }
        public void stopExecute(string selectedRF)
        {
            var taskIndex = selectedRF.Equals("RF1") ? 0 : 1;
            isStopRF[taskIndex] = true;
        }

        //private static async Task<Tuple<string, int>> crystalCalibration(string selectedRF, string cmd, int taskIndex)
        //{
        //    string result;
        //    int minDist = Int32.MaxValue;
        //    int xoVal = 0;

        //    for (int i = 0; i < 128; i++)
        //    {
        //        SerialPortManager.Instance.writeCommand("wifi_radio_test tx 0\n", selectedRF);
        //        await Task.Delay(100);
        //        SerialPortManager.Instance.writeCommand("wifi_radio_test set_xo_val " + i.ToString() + "\n", selectedRF);
        //        await Task.Delay(100);
        //        SerialPortManager.Instance.writeCommand("wifi_radio_test tx 1\n", selectedRF);
        //        await Task.Delay(100);
        //        VisaManager.Instance.writeVisa(cmd, taskIndex);
        //        await Task.Delay(100);
        //        result = VisaManager.Instance.readVisa(cmd, taskIndex);
        //        await Task.Delay(100);

        //        int dist = Math.Abs(Int32.Parse(result));
        //        if (minDist > dist)
        //        {
        //            xoVal = i;
        //            minDist = dist;
        //        }
        //    }

        //    SerialPortManager.Instance.writeCommand("wifi_radio_test tx 0\n", selectedRF);
        //    await Task.Delay(100);
        //    SerialPortManager.Instance.writeCommand("wifi_radio_test set_xo_val " + xoVal.ToString() + "\n", selectedRF);
        //    await Task.Delay(100);

        //    return new Tuple<string, int>(xoVal.ToString() + ":" + minDist.ToString(), 0);
        //}

        public bool checkInfoQuery(string cmd, string res, int min)
        {
            bool isInfoQuery = false;
            if (cmd.Contains("CONFigure:MEAS:RFSetting:FREQuency?"))
            {
                Util.cv = double.Parse(res);
                isInfoQuery = true;
            }
            else if (cmd.Contains("SOURce:WLAN:GEN:ARB:NPACkets?"))
            {
                int index = res.IndexOf("\n");
                Util.transmittedPacketCount = Int32.Parse(res.Substring(0, index));
                isInfoQuery = true;
            }
            return isInfoQuery;
        }
        //public void addLog(string str, string portName)
        //{
        //    DateTime currentDateTime = DateTime.Now;
        //    string formattedDateTime = currentDateTime.ToString("dd MMMM yyyy HH: mm:ss");
        //    _logList.Add("[" + formattedDateTime + "][" + portName + "]" + str + "\n");
        //    Console.WriteLine("[" + formattedDateTime + "][" + portName + "]" + str);
        //}
        //public void addLog(string str, int rfPortIndex)
        //{
        //    string rfport = rfPortIndex == 0 ? "RF1" : "RF2";

        //    DateTime currentDateTime = DateTime.Now;
        //    string formattedDateTime = currentDateTime.ToString("dd MMMM yyyy HH: mm:ss");
        //    _logList.Add("[" + formattedDateTime + "][" + rfport + "]" + str + "\n");
        //    Console.WriteLine("[" + formattedDateTime + "][" + rfport + "]" + str);
        //}
        //public void writeLog(string rfport)
        //{
        //    string logFilePath = Path.Combine(projectPath, "Logs");
        //    string fileNameDateTime = DateTime.Now.ToString("[yyyy-dd-MM HH_mm_ss]");
        //    string fileName = fileNameDateTime + rfport + "_LOG.txt";
        //    string path = Path.Combine(logFilePath, fileName);
        //    string text = String.Join("", _logList.Select(p => p));
        //    File.WriteAllText(path, text);
        //}
        
        public void setWifiType(string cmd)
        {
            if (cmd.Contains("CONFigure:WLAN:MEAS:STANdard"))
            {
                maskLimits.Clear();

                if (cmd.Contains("1"))
                {
                    Console.WriteLine("802.11a");
                    maskLimits.Add(-40);
                    maskLimits.Add(-40);
                    maskLimits.Add(-28);
                    maskLimits.Add(-20);
                    maskLimits.Add(-53);
                }
                if (cmd.Contains("2"))
                {
                    Console.WriteLine("802.11g");
                    maskLimits.Add(-40);
                    maskLimits.Add(-40);
                    maskLimits.Add(-28);
                    maskLimits.Add(-20);
                    maskLimits.Add(-53);
                }
                if (cmd.Contains("3"))
                {
                    Console.WriteLine("802.11n");
                    maskLimits.Add(-45);
                    maskLimits.Add(-45);
                    maskLimits.Add(-28);
                    maskLimits.Add(-20);
                    maskLimits.Add(-53);
                }
                if (cmd.Contains("4"))
                {
                    Console.WriteLine("802.11ac");
                    maskLimits.Add(-40);
                    maskLimits.Add(-40);
                    maskLimits.Add(-28);
                    maskLimits.Add(-20);
                    maskLimits.Add(-53);
                }
                if (cmd.Contains("5"))
                {
                    Console.WriteLine("802.11p");
                }
                if (cmd.Contains("6"))
                {
                    Console.WriteLine("802.11b");
                    maskLimits.Add(-50);
                    maskLimits.Add(-30);
                    maskLimits.Add(-30);
                    maskLimits.Add(-50);
                }
                if (cmd.Contains("7"))
                {
                    Console.WriteLine("802.11ax");
                    maskLimits.Add(-40);
                    maskLimits.Add(-40);
                    maskLimits.Add(-28);
                    maskLimits.Add(-20);
                    maskLimits.Add(-53);
                }
            }

        }
        private void setRxType(string param)
        {
            currentRxType = param[0].ToString();
        }
        //public void setScareResultLimit(string cmd, out string res)
        //{
        //    if (cmd.Contains("CONFigure:WLAN:MEAS:LIMit:SCARESult:11AC:EVM"))
        //    {
        //        res = cmd + " -5,-10,-13,-16,-19,-22,-25,-27,-30,-32";
        //    }
        //    else if (cmd.Contains("CONFigure:WLAN:MEAS:LIMit:SCARESult:11N:EVM")
        //             || cmd.Contains("CONFigure:WLAN:MEAS:LIMit:SCARESult:11A:EVM")
        //             || cmd.Contains("CONFigure:WLAN:MEAS:LIMit:SCARESult:11G:EVM"))
        //    {
        //        res = cmd + " -5,-8,-10,-13,-16,-19,-22,-25";

        //    }
        //    else if (cmd.Contains("CONFigure:WLAN:MEAS:LIMit:SCARESult:STD11AX:EVM"))
        //    {
        //        res = cmd + " -5.00,-10.00,-13.00,-16.00,-19.00,-22.00,-25.00,-27.00,-30.00,-32.00,-35.00,-35.00";
        //    }
        //    else
        //    {
        //        res = cmd;
        //    }
        //}
    }
}

