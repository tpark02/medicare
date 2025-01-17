using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using Medicare.Main;
using Medicare.Setup;
using Medicare.Utility;

namespace Medicare.Port
{
    class SerialPortManager : Singleton<SerialPortManager>
    {
        public string[] dutData = new string[] { "", "", "", "", "", "", "" };
        public List<byte> dutBuffer = new List<byte>();

        private SafeSerialPort[] ports = new SafeSerialPort[(int)ePort.MAX];
        private string terminationSequence = "\r\n"; // Anything that can't be part of a message

        public bool isSBConnected(string selectedRF)
        {
            var port = selectedRF.Equals("RF1") ? ports[(int)ePort.SB1] : ports[(int)ePort.SB2];
            if (port != null) return true;
            return false;
        }
        public bool isConnected(string portName)
        {
            foreach (var p in ports)
            {
                if (p != null && p.PortName.Equals(portName))
                    return true;
            }
            return false;
        }
        public void closePort(int idx)
        {

            if (ports[idx] != null)
            {
                ports[idx].Close();
                ports[idx].Dispose();
                ports[idx] = null;
            }

        }
        public void closePorts()
        {
            foreach (var port in ports)
            {
                if (port != null)
                {
                    port.Close();
                    port.Dispose();
                }
            }
        }

        public bool isPortOpen(int idx)
        {
            if (ports[idx] != null)
                return ports[idx].IsOpen;
            return false;
        }
        public bool openSB(string portName, int idx)
        {
            try
            {
                SafeSerialPort port = new SafeSerialPort(portName, 9600, 0, 8, StopBits.One);
                port.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
                //Open을 했으면 close는 필수

                if (!port.Open())
                {
                    return false;
                }

                ports[idx] = port;
                return true;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("Open SB Error", Color.Red);
                Util.openPopupOk("SerialPortManager.cs : openSB() - " + e.Message);
                Console.WriteLine("SerialPortManager.cs : openSB() - " + e.Message);
            }
            return false;
        }
        public bool openDUT(string portName, int baudRate, int idx)
        {
            try
            {
                for (int i = 0; i < NewSetupWindow.devicePorts.Length; i++)
                {
                    if (idx == i)
                        continue;

                    if (NewSetupWindow.devicePorts[i] == portName)
                    {
                        Util.openPopupOk(portName + " is in use. Please select a different port.");
                        return false;
                    }
                }

                SafeSerialPort port = new SafeSerialPort(portName, baudRate, 0, 8, StopBits.One);
                port.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);

                //Open을 했으면 close는 필수
                if (!port.Open())
                {
                    port.Close();
                    return false;
                }

                ports[idx] = port;
                return true;
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("Open DUT Error", Color.Red);
                Util.openPopupOk("SerialPortManager.cs : openDUT() - " + e.Message);
                Console.WriteLine("SerialPortManager.cs : openDUT() - " + e.Message);
            }
            return false;
        }
        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs se)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;

                byte[] buffer = null;
                SafeSerialPort p = null;
                int idx = -1;
                int cnt = 0;

                if (ports[(int)ePort.DUT1] != null && ports[(int)ePort.DUT1].IsOpen && sp.PortName.Equals(ports[(int)ePort.DUT1].PortName))
                    idx = (int)ePort.DUT1;
                else if (ports[(int)ePort.DUT2] != null && ports[(int)ePort.DUT2].IsOpen && sp.PortName.Equals(ports[(int)ePort.DUT2].PortName))
                    idx = (int)ePort.DUT2;
                else if (ports[(int)ePort.SC1] != null && ports[(int)ePort.SC1].IsOpen &&
                         sp.PortName.Equals(ports[(int)ePort.SC1].PortName))
                    idx = (int)ePort.SC1;
                else if (ports[(int)ePort.SC2] != null && ports[(int)ePort.SC2].IsOpen &&
                         sp.PortName.Equals(ports[(int)ePort.SC2].PortName))
                    idx = (int)ePort.SC2;
                else if (ports[(int)ePort.SB1] != null && ports[(int)ePort.SB1].IsOpen &&
                         sp.PortName.Equals(ports[(int)ePort.SB1].PortName))
                    idx = (int)ePort.SB1;
                else if (ports[(int)ePort.SB2] != null && ports[(int)ePort.SB2].IsOpen &&
                         sp.PortName.Equals(ports[(int)ePort.SB2].PortName))
                    idx = (int)ePort.SB2;
                else if (ports[(int)ePort.TEST] != null && ports[(int)ePort.TEST].IsOpen &&
                         sp.PortName.Equals(ports[(int)ePort.TEST].PortName))
                {
                    idx = (int)ePort.TEST;
                    string indata = sp.ReadExisting();
                    Util.addLog("Serial Read : " + indata, sp.PortName);
                    return;
                }

                if (idx != -1)
                {
                    cnt = ports[idx].BytesToRead;
                    buffer = new byte[cnt];
                    p = ports[idx];
                    p.Read(buffer, 0, cnt);

                    if ((ports[(int)ePort.DUT1] != null && sp.PortName.Equals(ports[(int)ePort.DUT1].PortName))
                        || (ports[(int)ePort.DUT2] != null && sp.PortName.Equals(ports[(int)ePort.DUT2].PortName))) // DUT
                    {
                        string buf = Encoding.UTF8.GetString(buffer);

                        if (CTPMain.isGetBDAddr && buf.Length >= 31)
                        {
                            int st = buf.IndexOf("'");
                            int ed = buf.LastIndexOf("'") - 1;
                            
                            int len = ed - st;
                            string addr = buf.Substring(st + 1, 17);
                            int semicolonCnt = 0;

                            for (int i = 0; i < addr.Length; i++)
                            {
                                if (addr[i] == ':') semicolonCnt++;
                            }

                            if (semicolonCnt < 5)
                            {
                                Console.WriteLine("semicolon cnt : " + semicolonCnt);
                                return;
                            }
                            Console.WriteLine("bd addr : " + addr);
                            CTPMain.bdAddr = addr;
                            Console.WriteLine(buf);
                            p.DiscardInBuffer();
                            p.DiscardOutBuffer();
                        }
                        else if (CTPMain.isGetVersion)
                        {
                            int st = buf.IndexOf("@");
                            int ed = buf.LastIndexOf("@");
                            if (st >= 0 && ed >= 0 && st != ed)
                            {
                                int len = ed - 1 - st;
                                string ver = buf.Substring(st + 1, len);
                                CTPMain.swVersion += ver;
                                Console.WriteLine(ver);
                                CTPMain.isGetVersion = false;
                            }
                        }
                        //cnt = ports[idx].BytesToRead;
                        //buffer = new byte[cnt];
                        //p = ports[idx];

                        //p.Read(buffer, 0, cnt);

                        //foreach (var b in buffer)
                        //{
                        //    //ITM_RS.dutBuffer.Add(b);
                        //    dutBuffer.Add(b);
                        //}

                        //if (dutBuffer.Count >= 2)
                        //{
                        //    var byteArray = dutBuffer.ToArray();
                        //    string hex = BitConverter.ToString(byteArray);

                        //    if (hex.Contains('-'))
                        //    {
                        //        hex = hex.Remove(2, 1);
                        //        string bin = Util.hex2binary(hex.Substring(0, 2)) + Util.hex2binary(hex.Substring(2));
                        //        Util.addLog("DUT Read : " + hex + " : " + bin, sp.PortName);
                        //        //ITM_RS.dutBinary = bin;
                        //        dutData[idx] = bin;
                        //        dutBuffer.Clear();
                        //    }
                        //}
                    }
                    else if ((ports[(int)ePort.SC1] != null && sp.PortName.Equals(ports[(int)ePort.SC1].PortName))
                            || (ports[(int)ePort.SC1] != null && sp.PortName.Equals(ports[(int)ePort.SC2].PortName)))  // scanner
                    {
                        foreach (var b in buffer)
                        {
                            dutBuffer.Add(b);
                        }

                        var byteArray = dutBuffer.ToArray();
                        string str = BitConverter.ToString(byteArray);

                        if (dutData[idx].Length + str.Length >= 79)
                        {
                            dutData[idx] += str;
                            Util.addLog("Scanner Read : " + dutData[idx], sp.PortName);
                            dutBuffer.Clear();
                        }
                        else
                        {
                            dutData[idx] += str;
                        }
                    }
                    else if ((ports[(int)ePort.SB1] != null && sp.PortName.Equals(ports[(int)ePort.SB1].PortName))
                             || (ports[(int)ePort.SB2] != null && sp.PortName.Equals(ports[(int)ePort.SB2].PortName)))   // Shield Box
                    {
                        //string str = BitConverter.ToString(buffer);
                        string str = Encoding.UTF8.GetString(buffer);
                        dutData[idx] = str;
                        Util.addLog("Box Read : " + str, sp.PortName);
                        dutBuffer.Clear();
                    }
                    else
                    {
                        string indata = sp.ReadExisting();
                        Util.addLog("Serial Read : " + indata, sp.PortName);
                    }
                }
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("Serial Data Recv Error", Color.Red);
                Util.openPopupOk("SerialPortManager.cs : serialPort_DataReceived() - " + e.Message);
                Console.WriteLine("SerialPortManager.cs : serialPort_DataReceived() - " + e.Message);
            }
        }
        public bool sendBitData(string str, string selectedRF)
        {
            var port = selectedRF.Equals("RF1") ? ports[(int)ePort.DUT1] : ports[(int)ePort.DUT2];
            int portIdx = selectedRF.Equals("RF1") ? (int)ePort.DUT1 : (int)ePort.DUT2;

            try
            {
                if (port != null)
                {
                    int numOfBytes = str.Length / 8;
                    byte[] buffer = new byte[numOfBytes];

                    for (int i = 0; i < numOfBytes; ++i)
                        buffer[i] = Convert.ToByte(str.Substring(8 * i, 8), 2);

                    port.Write(buffer, 0, buffer.Length);
                    Util.addLog("send Bit Data : " + str.ToString(), ports[portIdx].PortName);
                }
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("Send Bit Data Error", Color.Red);
                Util.openPopupOk("SerialPortManager.cs : sendBitData() - " + e.Message);
                Console.WriteLine("SerialPortManager.cs : sendBitData() - " + e.Message);
                return false;
            }

            return true;
        }

        public void writeTestCommand(string cmd)
        {
            var port = ports[(int)ePort.TEST];
            int portIdx = (int)ePort.TEST;

            try
            {
                if (port != null)
                {
                    byte[] buffer = Util.StringToByte(cmd);
                    port.Write(buffer, 0, buffer.Length);
                    //Util.dutLog = string.Empty;
                    dutData[portIdx] = string.Empty;
                    Util.addLog("writeCmd : " + cmd, ports[portIdx].PortName);
                }
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("Write Test Command Error", Color.Red);
                Util.openPopupOk("SerialPortManager.cs : writeCommand() - " + e.Message);
                Console.WriteLine("SerialPortManager.cs : writeCommand() - " + e.Message);
            }
        }
        public void writeCommand(string cmd, string selectedRF)
        {
            var port = selectedRF.Equals("RF1") ? ports[(int)ePort.DUT1] : ports[(int)ePort.DUT2];
            int portIdx = selectedRF.Equals("RF1") ? (int)ePort.DUT1 : (int)ePort.DUT2;

            try
            {
                if (port != null)
                {
                    byte[] buffer = Util.StringToByte(cmd);
                    port.Write(buffer, 0, buffer.Length);
                    //Util.dutLog = string.Empty;
                    dutData[portIdx] = string.Empty;
                    Util.addLog("writeCmd : " + cmd, ports[portIdx].PortName);
                }
            }
            catch (Exception e)
            {
                CTPMain.wnd.setTextPgrBar("Write Command Error", Color.Red);
                Util.openPopupOk("SerialPortManager.cs : writeCommand() - " + e.Message);
                Console.WriteLine("SerialPortManager.cs : writeCommand() - " + e.Message);
            }
        }
        public bool writeSBCommand(string cmd, string selectedRF)
        {
            var port = selectedRF.Equals("RF1") ? ports[(int)ePort.SB1] : ports[(int)ePort.SB2];
            int portIdx = selectedRF.Equals("RF1") ? (int)ePort.SB1 : (int)ePort.SB2;

            if (port != null)
            {
                try
                {
                    byte[] buffer = Util.StringToByte(cmd);
                    port.Write(buffer, 0, buffer.Length);
                    Util.dutLog = string.Empty;
                    dutData[portIdx] = string.Empty;
                    Util.addLog("write SB cmd : " + cmd, ports[portIdx].PortName);
                }
                catch (Exception e)
                {
                    CTPMain.wnd.setTextPgrBar("Write SB Command Error", Color.Red);
                    Util.openPopupOk("SerialPortManager.cs : writeSBCommand() - " + e.Message);
                    Console.WriteLine("SerialPortManager.cs : writeSBCommand() - " + e.Message);
                    return false;
                }
            }
            return true;
        }
    }
}
