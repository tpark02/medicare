using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Medicare.Main;
using Medicare.Port;
using Medicare.Utility;

namespace Medicare.Dut
{
    class DutManager : Singleton<DutManager>
    {
        public SerialPortManager portMgr = null;
        private IniFile ini = new IniFile();
        private string platformName = "Nordic_nRF7002";
        public SortedDictionary<string, List<List<string>>> cmdlist = new SortedDictionary<string, List<List<string>>>();
        public SortedDictionary<int, List<string>> minmaxlist = new SortedDictionary<int, List<string>>();

        public DutManager()
        {
            var projectPath = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(projectPath, "Resources");
            ini.Load(filePath + "\\Nordic_nRF7002.ini");

            string mainName = ini[platformName]["Main_Name"].ToString();

            for (int i = 0; i < ini.Count - 1; i++)
            {
                string request = platformName + "_REQUEST_" + i.ToString();

                string type = ini[request]["type"].ToString();
                string cnt = ini[request]["cnt"].ToString();
                string name = ini[request]["name"].ToString();

                List<string> properties = new List<string>()
                {
                    ini[request]["cmd"].ToString(),
                    name,
                    cnt,
                    ini[request]["default_value"].ToString(),
                };

                if (cnt == "2")  // on, off
                {
                    properties.Add("0");
                    properties.Add("1");                    
                }
                else if (cnt == "NA" || cnt == "val")
                {

                }
                else if (cnt == "max")
                {
                    properties.Add(ini[request]["Param_0"].ToString());
                    properties.Add(ini[request]["Param_1"].ToString());
                }
                else
                {
                    int nCnt = Int32.Parse(cnt);

                    for (int j = 0; j < nCnt; j++)
                    {
                        string param = "Param_" + j.ToString();
                        properties.Add(ini[request][param].ToString());
                    }
                }

                if (!cmdlist.ContainsKey(type))
                    cmdlist.Add(type, new List<List<string>>() { properties });
                else
                    cmdlist[type].Add(properties);
            }
        }

        public bool checkRange(int rowIndex)
        {
            var l = DutManager.Instance.minmaxlist;
            if (l.ContainsKey(rowIndex) && !(Int32.Parse(l[rowIndex][2]) <= Int32.Parse(l[rowIndex][0]) && Int32.Parse(l[rowIndex][0]) <= Int32.Parse(l[rowIndex][3])))
            {
                Util.openPopupOk(string.Format("row : {0} Par1 is out of range !!!", rowIndex));
                return false;
            }

            return true;
        }
        public bool checkPacketCount(out float packetCount, string currentType, int minVal)
        {            
            try
            {
                //string str = Util.dutResult;
                string str = string.Empty;

                if (str.Equals(""))
                {
                    packetCount = -1;
                    return false;
                }
                else if (str.Contains("stats_get failed"))
                {
                    packetCount = -2;
                    return false;
                }

                int len = 0, index = 0, endIndex = 0;
                string packetCnt = string.Empty;

                if (currentType.Equals("B"))
                {
                    len = ("32mdsss_crc32_pass_cnt=").Length;
                    index = str.IndexOf("32mdsss_crc32_pass_cnt=");

                }
                else
                {
                    len = ("32mofdm_crc32_pass_cnt=").Length;
                    index = str.IndexOf("32mofdm_crc32_pass_cnt=");
                }

                str = str.Substring(index + len);
                int idx = 0;

                while (true)
                {
                    if (!Char.IsDigit(str[idx])) break;
                    packetCnt += str[idx++];
                }

                float limit = (float)minVal;
                float rCnt = float.Parse(packetCnt);
                float val = (rCnt / Util.transmittedPacketCount);
                float percentOfPackets = ((1 - val) * 100);
                packetCount = rCnt;
                Console.WriteLine("checkPaketCount = packetCnt : " + packetCnt.ToString() + ", transmittedPacketCount : " + Util.transmittedPacketCount.ToString() + ", Limit : " + limit.ToString());
                return percentOfPackets < limit ? true : false;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("DUT Manager Error", Color.Red);
                Util.openPopupOk("DutManager.cs - checkPacketCount - " + e.Message);
                packetCount = -1;
                return false;
            }
        }
    }
}
