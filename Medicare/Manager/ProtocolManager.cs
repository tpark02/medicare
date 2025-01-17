using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Medicare.Manager;
using Medicare.Utility;

namespace Medicare.Protocol
{
    class ProtocolManager : Singleton<ProtocolManager>
    {
        private IniFile ini = new IniFile();
        private string platformName = Device.MTP300.ToString();
        private List<string> requestProperties = new List<string>() { "Sub_Name_index", "Name", "Cmd", "Par", "Type", "Par_Count", "Par_" };
        private HashSet<int> allowList = new HashSet<int>() { 92, 131, 33, 161, 135, 141, 194, 168, 201, 284, 209, 249, 272, 273, 203, 214, 260, 296, 316, 37, 334, 4, 5, 8, 21, 167, 24, 25, 28, 30, 90, 22, 335, 29, 6, 20, 137, 42, 46, 43, 47, 12, 16, 13, 17, 193, 261, 285 };

        public SortedDictionary<string, List<List<string>>> getCmdList = new SortedDictionary<string, List<List<string>>>();
        public SortedDictionary<string, List<List<string>>> setCmdList = new SortedDictionary<string, List<List<string>>>();

        private int subNameCount = 22;
        public ProtocolManager()
        {
            //var projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var projectPath = Directory.GetCurrentDirectory();
            string filePath = Path.Combine(projectPath, "Resources");

            ini.Load(filePath + "\\MTP300_Protocol.ini");

            string mainName = ini[platformName]["Main_Name"].ToString();

            var subNameList = Enumerable.Range(0, subNameCount).Select(n => ini[platformName]["Sub_Name_" + n.ToString()].ToString()).ToList();

            for (int i = 0; i < ini.Count - 1; i++)
            {
                //if (!(i == 92 || i == 131 || i == 33 || i == 161 || i == 135 || i == 141 || i == 194 || i == 168 ||
                //      i == 201 || i == 284 || i == 209 || i == 249 || i == 272 || i == 203 || i == 214 || i == 260 ||
                //      i == 296 || i == 316 || i == 37 || i == 334))
                //    continue;
                if (!allowList.Contains(i)) continue;
                string request = platformName + "_REQUEST_" + i.ToString();
                string sType = ini[request][requestProperties[4]].ToString();
                string par = ini[request][requestProperties[3]].ToString();     // Par=<!>
                int idx = ini[request][requestProperties[0]].ToInt();           // Sub_Name_index

                if (4 < idx && idx < 10) continue; // WLAN SOURCE, WLAN CONFIG, WLAN READ, BT 제외함

                List<string> properties = new List<string>()
                {
                    ini[request][requestProperties[2]].ToString(),      // Cmd=SYSTem:SERIALnumber?
                    ini[request][requestProperties[1]].ToString(),      // Name=MTP300.GET_IDN
                    ini[request][requestProperties[3]].ToString(),      // Par=<!>
                    ini[request][requestProperties[4]].ToString(),      // Type=GET
                };

                if (par.Equals("<@>"))
                {
                    int cnt = ini[request][requestProperties[5]].ToInt();

                    for (int j = 0; j < cnt; j++)
                    {
                        string p = ini[request][requestProperties[6] + j.ToString()].ToString();
                        properties.Add(p);
                    }
                }
                else if (par.IndexOf("#") >= 0)
                {
                    if (par.IndexOf("/") >= 0)     // 값있음 : 여러개
                    {
                        string substr = par.Substring(3);
                        string[] words = substr.Split('/');

                        foreach (var word in words)
                            properties.Add(word);
                    }
                    else if (par[par.Length - 1] == '>')     // 값이 없음
                    {

                    }
                    else     // 값있음 : 한개
                    {
                        string p = par.Substring(3);
                        properties.Add(p);
                    }
                }

                if (sType.Equals("SET"))
                {
                    if (!setCmdList.ContainsKey(subNameList[idx]))
                        setCmdList.Add(subNameList[idx], new List<List<string>>() { properties });
                    else
                        setCmdList[subNameList[idx]].Add(properties);
                }
                else
                {
                    if (!getCmdList.ContainsKey(subNameList[idx]))
                        getCmdList.Add(subNameList[idx], new List<List<string>>() { properties });
                    else
                        getCmdList[subNameList[idx]].Add(properties);
                }
                //Console.WriteLine(i.ToString());
            }
        }

        public bool checkMinMax(string minValue, string maxValue)
        {
            int minIntVal = 0;
            int maxIntVal = 0;

            bool canMinConvert = Int32.TryParse(minValue, out minIntVal);
            bool canMaxConvert = Int32.TryParse(maxValue, out maxIntVal);

            if (!minValue.Equals("") && !canMinConvert)
            {
                Util.openPopupOk("Please Check Min Value");
                return false;
            }

            if (!maxValue.Equals("") && !canMaxConvert)
            {
                Util.openPopupOk("Pleas Check Max Value");
                return false;
            }

            return true;
        }

        //public bool checkPassFail(string cmd, string cmp, string res, int min, int max, out string meas)
        //{
        //    var info = MTP300A_Commands.queryResultInfo[cmd];
        //    if (info.Equals(""))
        //    {
        //        meas = res;
        //        return false;
        //    }
        //    var indexList = info.Split(':');
        //    var resultList = res.Split(',');
        //    int decisionIndex = Int32.Parse(indexList[1]);
        //    string result = string.Empty;

        //    if (decisionIndex == -1)
        //    {
        //        switch (cmp)
        //        {
        //            case "< X <":

        //                for (int i = 2; i < indexList.Length; i++)
        //                {
        //                    int index = Int32.Parse(indexList[i]);
        //                    var val = double.Parse(resultList[index]);
        //                    result += val.ToString() + ",";

        //                    if (!(min < val && val < max))
        //                    {
        //                        meas = result.Substring(0, result.Length - 1);
        //                        return false;
        //                    }
        //                }
        //                meas = result.Remove(result.Length - 1, 1);
        //                meas = result;
        //                return true;
        //            case "X <":
        //                if (cmd.Contains("READ:WLAN:MEAS:SCARESult:FERRor:VERDict?"))
        //                {
        //                    int index = Int32.Parse(indexList[2]);
        //                    double fv = double.Parse(resultList[index]);
        //                    double hz = Util.convertToPPM(fv);
        //                    meas = hz.ToString();
        //                    return hz < min ? true : false;
        //                }
        //                for (int i = 2; i < indexList.Length; i++)
        //                {
        //                    int index = Int32.Parse(indexList[i]);
        //                    var val = double.Parse(resultList[index]);
        //                    result += val.ToString() + ",";

        //                    if (!(val < min))
        //                    {
        //                        meas = result.Remove(result.Length - 1, 1);
        //                        meas = result;
        //                        return false;
        //                    }
        //                }
        //                meas = result.Remove(result.Length - 1, 1); meas = result;
        //                return true;
        //            default:
        //                if (cmd.Contains("SOURce:WLAN:GEN:ARB:STATus?"))
        //                {
        //                    meas = res;
        //                    return resultList[0].Equals("0\n") ? true : false;
        //                }                                                
        //                meas = res;
        //                return true;
        //        }
        //    }

        //    if (cmd.Contains("READ:WLAN:MEAS:SCARESult:EVM:ACAvg:VERDict?"))
        //    {
        //        meas = resultList[0];
        //        return resultList[decisionIndex].Equals("0\n") ? true : false;
        //    }
        //    else if (cmd.Contains("READ:WLAN:MEAS:SPECMask:MARGin:VERDict?"))
        //    {
        //        var maskLimits = DataManager.Instance.maskLimits;

        //        if (maskLimits.Count == 0)
        //        {
                    
        //            meas = "No Limits Set";
        //            return false;
        //        }

        //        for (int i = 2; i < indexList.Length; i++)
        //        {
        //            int index = Int32.Parse(indexList[i]);
        //            var val = double.Parse(resultList[index]);
        //            result += (val + maskLimits[i - 2]).ToString() + ",";
        //        }
        //        meas = result.Remove(result.Length - 1, 1);
        //        meas = result;
        //        string r = resultList[decisionIndex].Replace(" ", "");
        //        return r.Equals("0\n") ? true : false;
        //    }

        //    for (int i = 2; i < indexList.Length; i++)
        //    {
        //        int index = Int32.Parse(indexList[i]);
        //        var val = double.Parse(resultList[index]);
        //        result += val.ToString() + ",";
        //    }

        //    meas = result.Remove(result.Length - 1, 1);
        //    meas = result;
        //    return resultList[decisionIndex].Equals("0\n") ? true : false;

        //}

        public bool evaluateResult(string str)
        {
            return false;
        }
    }
}
