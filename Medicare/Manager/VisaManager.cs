using System;
using Ivi.Visa;
using System.IO;
using System.Text;
using Medicare.Main;
using Medicare.Manager;
using Medicare.Utility;

namespace Medicare.Visa
{
    class VisaManager : Singleton<VisaManager>
    {
        public ReadStatus readStatus = ReadStatus.Unknown;
        public static string ip = string.Empty;
        public string mtp300aIP = "TCPIP0::192.168.100.253::inst0::INSTR";
        private IMessageBasedSession session = null;
        
        private DataManager dataMgr = DataManager.Instance;
        ~VisaManager()
        {
            closeVisaSession();
        }
        public bool isConnected()
        {
            return session != null ? true : false;
        }
        public bool openVisaSession()
        {
            if (session == null)
            {
                try
                {
                    if (!CTPMain.isBLE)
                    {
                        session = (IMessageBasedSession)GlobalResourceManager.Open(mtp300aIP);
                        Console.WriteLine("Connected to mtp300aIP : " + mtp300aIP);
                    }
                    else
                    {
                        string ipaddr = "TCPIP0::" + ip + "::inst0::INSTR";
                        session = (IMessageBasedSession)GlobalResourceManager.Open(ipaddr);
                        Console.WriteLine("Connected to ip : " + ip);
                    }

                    setTimeOut(300000);
                }
                catch (VisaException e)
                {
                    Util.openPopupOk("MTP300C is not connected.\n" + e.Message);
                    session = null;
                    CTPMain.isVisaOpen = false;
                    return false;
                }
                catch (Exception ex)
                {
                    Util.openPopupOk("MTP300C is not connected.\n" + ex.Message);
                    session = null;
                    CTPMain.isVisaOpen = false;
                    return false;
                }
            }
            return true;
        }
        public void closeVisaSession()
        {
            if (session != null)
            {
                session.Dispose();
                session = null;
            }
        }
        public bool writeVisa(string cmd, int rfPortIndex)
        {
            try
            {
                if (session != null)
                {
                    session.FormattedIO.WriteLine(cmd);
                    //Console.WriteLine(cmd);
                    Util.addLog(cmd, rfPortIndex);
                }
            }
            catch (VisaException e)
            {
                Util.openPopupOk("writeVisa VisaException : \n" + " command : " + cmd + "\n" + "Exception Msg : " + e.Message);
                Util.addLog("VisaManager.cs - writeVisa() - exception msg : " + e.Message + " cmd : " + cmd, rfPortIndex);
                return false;
            }
            return true;
        }
        public string readVisa(string cmd, int rfPortIndex)
        {
            string response = "ERROR";

            //try
            //{
            if (session == null)
                return "ERROR";
            response = session.FormattedIO.ReadLine();
            //Console.WriteLine(response);
            Util.addLog("[R]" + response, rfPortIndex);
            //}
            //catch (VisaException e)
            //{
            //    Util.openPopupOk("readVisa VisaException : " + cmd + " : " + e.Message);
            //    Util.addLog("VisaManager.cs - readVisa() - exception msg : " + e.Message + " cmd : " + cmd, rfPortIndex);
            //    return "ERROR";
            //}
            return response;
        }

        public void setTimeOut(int n)
        {
            if (n < 0)
                return;

            if (session != null)
            {
                session.TimeoutMilliseconds = n;
            }
        }

        private string InsertCommonEscapeSequences(string s)
        {
            return s.Replace("\n", "\\n").Replace("\r", "\\r");
        }       
    }
}
