using CommUTIL;
using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CommFTDI.FDTI_Type;
using static FTD2XX_NET.FTDI;

namespace CommFTDI
{
    public class FTDIDeviceCtrl
    {
        // 로그 작성
        private static FTDI _ftdi = new FTDI();

        private const byte _maskBitBangMode = 0x20;
        private const byte _maskLevelShiftOff = 0x20;   // Level Shift OFF   0x20 (0010 0000)  2번 핀 활성화
        private const byte _maskLevelShiftOn = 0x22;    // Level Shift ON   0x22 (0010 0010)  5번 핀과 1번 핀 활성화
        private const byte _maskVccOn = 0x44;           // VCC ON   0x44 (0100 0100) 7번 핀과 3번 핀 활성화
        private const byte _maskGpioOn = 0x88;          // GPIO ON  0x88 (1000 0000) 8번 핀과 4번 핀 활성화
        private const byte _maskGpioOff = 0x80;         // GPIO OFF  0x44 (0100 0100)
        private const byte _maskVccOff = 0x40;          // VCC ON   0x40 (0100 0000)  7번 핀 활성화

        /// <summary>
        /// 디바이스 목록 조회
        /// </summary>
        /// <returns> List<FTDIDevice> </returns>
        public static List<FTDIDevice> GetDeviceList()
        {
            List<FTDIDevice> devices = new List<FTDIDevice>();
            try
            {
                uint deviceCount = 0;
                FT_STATUS ftStatus = _ftdi.GetNumberOfDevices(ref deviceCount);
                if (ftStatus != FT_STATUS.FT_OK)
                {
                    Logger.Warn($"\tFailed to get number of devices");
                    return devices;
                }

                if (deviceCount == 0)
                {
                    Logger.Warn($"\tNo FTDI devices found.");
                    return devices;
                }

                // 기기 정보 목록 생성
                FTDI.FT_DEVICE_INFO_NODE[] deviceList = new FTDI.FT_DEVICE_INFO_NODE[deviceCount];

                // 기기 정보 목록 조회
                ftStatus = _ftdi.GetDeviceList(deviceList);
                if (ftStatus != FT_STATUS.FT_OK)
                {
                    // MessageBox.Show("Failed to get device list");
                    Logger.Warn($"\tFailed to get device list");
                    return devices;
                }

                foreach (var device in deviceList)
                {
                    if (device.Type == FTDI.FT_DEVICE.FT_DEVICE_232R) // FT232R 디바이스만 리스트에 추가
                    {
                        string comPortName;
                        // 임시로 디바이스를 열어 COM 포트를 확인
                        _ftdi.OpenBySerialNumber(device.SerialNumber);
                        _ftdi.GetCOMPort(out comPortName);  // COM 포트 확인
                        _ftdi.Close();

                        devices.Add(new FTDIDevice
                        {
                            DeviceType = device.Type,
                            ID = device.ID,
                            SerialNumber = device.SerialNumber,
                            Description = device.Description,
                            Handle = device.ftHandle,
                            ComPort = comPortName,
                        });
                        Logger.Info($"\tFind {device.Description} ({device.SerialNumber}) - {comPortName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"\tException : {ex.Message}");
                Logger.Error($"\tFailed to activate GPIO. Please verify configuration and try again.");
            }

            return devices;
        }

        /// <summary>
        /// 디바이스 열기
        /// </summary>
        /// <param name="type">device를 open type - 장치 식별이 가능한 디바이스 인덱스, 시리얼번호, 장치명, 디바이스 위치 중에 선택하여 OPEN</param>
        /// <param name="deviceIdentifier">device 식별자</param>
        /// <returns></returns>
        public static bool DeviceOpen(OPEN_TYPE type, string deviceIdentifier)
        {
            bool result = false;
            uint identifier = 0;
            bool isUintType = (type == OPEN_TYPE.INDEX || type == OPEN_TYPE.LOCATION);

            // UINT로 변환해야 하는 경우 미리 변환 시도
            if (isUintType && !UInt32.TryParse(deviceIdentifier, out identifier))
            {
                Logger.Error($"\tValue cannot be parsed to UInt32");
                return result;
                //throw new ArgumentException("Value cannot be parsed to UInt32", nameof(value));
            }

            FT_STATUS ftOpenStatus = FT_STATUS.FT_DEVICE_NOT_OPENED;
            try
            {
                switch (type)
                {
                    case OPEN_TYPE.INDEX:
                        ftOpenStatus = _ftdi.OpenByIndex(identifier);
                        break;
                    case OPEN_TYPE.SERIALNUMBER:
                        ftOpenStatus = _ftdi.OpenBySerialNumber(deviceIdentifier);
                        break;
                    case OPEN_TYPE.DESCRIPTION:
                        ftOpenStatus = _ftdi.OpenByDescription(deviceIdentifier);
                        break;
                    case OPEN_TYPE.LOCATION:
                        ftOpenStatus = _ftdi.OpenByLocation(identifier);
                        break;
                    default:
                        // 적절한 예외 처리 또는 로그 기록
                        throw new ArgumentException($"\tInvalid open type or value");
                }

                if (ftOpenStatus == FT_STATUS.FT_OK && _ftdi.IsOpen)
                    result = true;
            }
            catch (Exception e)
            {
                Logger.Error($"\tException : {e.Message}");
                Logger.Error($"\tFailed to activate GPIO. Please verify configuration and try again.");
                return result;
            }

            return result;
        }

        /// <summary>
        /// 디바이스 닫기
        /// </summary>
        /// <param name="type">device를 open type</param>
        /// <param name="deviceIdentifier">device 식별자</param>
        /// <returns></returns>
        public static bool DeviceClose()
        {
            bool result = false;
            try
            {
                if (_ftdi.IsOpen)
                    _ftdi.Close();
            }
            catch (Exception e)
            {
                Logger.Error($"\tException : {e.Message}");
                Logger.Error($"\tFailed to close the FTDI device. Please verify configuration and try again.");
                return result;
            }

            return result;
        }

        /// <summary>
        /// Power(VCC) 설정
        /// </summary>
        /// <param name="mode">ON/OFF</param>
        /// <returns></returns>
        public static bool SetPower(BIT_MODE mode)
        {
            bool result = false;
            try
            {
                if (_ftdi.IsOpen)
                {
                    byte mask = 0x0;
                    
                    if (mode == BIT_MODE.ON)
                    {
                        // VCC ON
                        mask |= _maskVccOn;
                        FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 High 로 변경
                        if (ftStatus != FT_STATUS.FT_OK)
                        {
                            Logger.Error($"\tFailed to turn on VCC. Please check the device and try again.");
                            return result;
                        }

                        // 디바이스 응답 시간을 위해 500ms 대기
                        Thread.Sleep(500);

                        result = true;
                    }
                    else if (mode == BIT_MODE.OFF)
                    {
                        // VCC OFF
                        mask |= _maskVccOff;
                        FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 low 로 변경
                        if (ftStatus != FT_STATUS.FT_OK)
                        {
                            Logger.Error($"\tFailed to activate GPIO. Please verify configuration and try again.");
                            return result;
                        }

                        // 디바이스 응답 시간을 위해 500ms 대기
                        Thread.Sleep(500);

                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"\tException : {e.Message}");
            }
            return result;
        }

        /// <summary>
        /// GPIO 설정
        /// </summary>
        /// <param name="mode">ON/OFF</param>
        /// <returns></returns>
        public static bool SetGPIO(BIT_MODE mode)
        {
            bool result = false;
            try
            {
                if (_ftdi.IsOpen)
                {
                    byte mask = 0x0;
                    if (mode == BIT_MODE.ON)
                    {
                        // GPIO ON
                        mask |= _maskVccOn;  // VCC ON 상태여야 한다
                        mask |= _maskGpioOn; // 0xcc 11001100
                        FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 High 로 변경
                        if (ftStatus != FT_STATUS.FT_OK)
                        {
                            Logger.Error($"\tFailed to activate GPIO.Please verify configuration and try again.");
                            return result;
                        }

                        // 디바이스 응답 시간을 위해 500ms 대기
                        Thread.Sleep(500);

                        result = true;
                    }
                    else if (mode == BIT_MODE.OFF)
                    {
                        // GPIO OFF 
                        mask |= _maskGpioOff;
                        FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 low 로 변경
                        if (ftStatus != FT_STATUS.FT_OK)
                        {
                            Logger.Error($"\tFailed to deactivate GPIO.Please verify configuration and try again.");
                            return result;
                        }

                        // 디바이스 응답 시간을 위해 500ms 대기
                        Thread.Sleep(500);

                        result = true;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"\tException : {e.Message}");
            }
            return result;
        }

        /// <summary>
        /// Power(VCC) 와 GPIO 를 On(High) 또는 OFF(low) 로 순차 세팅한다.
        /// </summary>
        /// <param name="type">device를 open type - 장치 식별이 가능한 디바이스 인덱스, 시리얼번호, 장치명, 디바이스 위치 중에 선택하여 OPEN</param>
        /// <param name="deviceIdentifier">device 식별자</param>
        /// <param name="mode">ON, OFF</param>
        /// <returns></returns>
        public static bool SetPowerAndGPIO(OPEN_TYPE type, string deviceIdentifier, BIT_MODE mode)
        {
            bool result = false;
            uint identifier = 0;
            bool isUintType = (type == OPEN_TYPE.INDEX || type == OPEN_TYPE.LOCATION);

            // UINT로 변환해야 하는 경우 미리 변환 시도
            if (isUintType && !UInt32.TryParse(deviceIdentifier, out identifier))
            {
                Logger.Error($"\tValue cannot be parsed to UInt32");
                return result;
                //throw new ArgumentException("Value cannot be parsed to UInt32", nameof(value));
            }

            FT_STATUS ftOpenStatus = FT_STATUS.FT_DEVICE_NOT_OPENED;
            try
            {
                switch (type)
                {
                    case OPEN_TYPE.INDEX:
                        ftOpenStatus = _ftdi.OpenByIndex(identifier);
                        break;
                    case OPEN_TYPE.SERIALNUMBER:
                        ftOpenStatus = _ftdi.OpenBySerialNumber(deviceIdentifier);
                        break;
                    case OPEN_TYPE.DESCRIPTION:
                        ftOpenStatus = _ftdi.OpenByDescription(deviceIdentifier);
                        break;
                    case OPEN_TYPE.LOCATION:
                        ftOpenStatus = _ftdi.OpenByLocation(identifier);
                        break;
                    default:
                        // 적절한 예외 처리 또는 로그 기록
                        throw new ArgumentException($"\tInvalid open type or value");
                }

                if (ftOpenStatus != FT_STATUS.FT_OK)
                {
                    Logger.Error($"\tFailed to device open. {ftOpenStatus.ToString()}.");
                    return result;
                }

                byte mask = 0x0;
                if (mode == BIT_MODE.POWER_ON)
                {
                    // VCC ON
                    mask |= _maskVccOn;
                    //FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 High 로 변경
                    //if (ftStatus != FT_STATUS.FT_OK)
                    //{
                    //    Logger.Error("Failed to turn on VCC. Please check the device and try again.", typeof(FTDIDeviceManager).Name);
                    //    return result;
                    //}

                    //// 디바이스 응답 시간을 위해 500ms 대기
                    //Thread.Sleep(500);

                    // GPIO ON
                    // VCC ON 핀 상태를 유지해야 하므로 mask 변수를 초기화하지 않는다. 
                    mask |= _maskGpioOn; // 0xcc 11001100
                    FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 High 로 변경
                    if (ftStatus != FT_STATUS.FT_OK)
                    {
                        Logger.Error($"\tFailed to activate GPIO. Please verify configuration and try again.");
                        return result;
                    }

                    result = true;
                }
                else if (mode == BIT_MODE.POWER_OFF)
                {
                    // GPIO OFF 
                    mask |= _maskGpioOff;
                    // VCC 는 ON 상태를 유지
                    mask |= _maskVccOn;
                    FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 low 로 변경
                    if (ftStatus != FT_STATUS.FT_OK)
                    {
                        Logger.Error($"\tFailed to turn on VCC. Please check the device and try again.");
                        return result;
                    }

                    // 디바이스 응답 시간을 위해 500ms 대기
                    Thread.Sleep(500);

                    // VCC OFF
                    mask = 0x0;
                    mask |= _maskVccOff;
                    ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 low 로 변경
                    if (ftStatus != FT_STATUS.FT_OK)
                    {
                        Logger.Error($"\tFailed to activate GPIO. Please verify configuration and try again.");
                        return result;
                    }

                    result = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"\tException : {e.Message}");
                result = false;
            }
            finally
            {
                if (_ftdi.IsOpen)
                    _ftdi.Close();
            }
            return result;
        }

        /// <summary>
        /// BitBang 모드로 핀 제어
        /// </summary>
        /// <param name="type">device를 open type - 장치 식별이 가능한 디바이스 인덱스, 시리얼번호, 장치명, 디바이스 위치 중에 선택하여 OPEN</param>
        /// <param name="deviceIdentifier">device 식별자</param>
        /// <param name="mode">ON, OFF</param>
        /// <returns></returns>
        public static bool SetBitBang(OPEN_TYPE type, string deviceIdentifier, BIT_MODE mode)
        {
            bool result = false;
            uint identifier = 0;
            bool isUintType = (type == OPEN_TYPE.INDEX || type == OPEN_TYPE.LOCATION);

            try
            {
                // UINT로 변환해야 하는 경우 미리 변환 시도
                if (isUintType && !UInt32.TryParse(deviceIdentifier, out identifier))
                {
                    Logger.Error($"\tValue cannot be parsed to UInt32");
                    return result;
                    //throw new ArgumentException("Value cannot be parsed to UInt32", nameof(value));
                }

                FT_STATUS ftOpenStatus = FT_STATUS.FT_DEVICE_NOT_OPENED;

                switch (type)
                {
                    case OPEN_TYPE.INDEX:
                        ftOpenStatus = _ftdi.OpenByIndex(identifier);
                        break;
                    case OPEN_TYPE.SERIALNUMBER:
                        ftOpenStatus = _ftdi.OpenBySerialNumber(deviceIdentifier);
                        break;
                    case OPEN_TYPE.DESCRIPTION:
                        ftOpenStatus = _ftdi.OpenByDescription(deviceIdentifier);
                        break;
                    case OPEN_TYPE.LOCATION:
                        ftOpenStatus = _ftdi.OpenByLocation(identifier);
                        break;
                    default:
                        // 적절한 예외 처리 또는 로그 기록
                        throw new ArgumentException("Invalid open type or value");
                }

                if (ftOpenStatus != FT_STATUS.FT_OK)
                {
                    Logger.Error($"\tFailed to device open. {ftOpenStatus.ToString()}.");
                    return result;
                }

                byte mask = 0x0;
                if (mode == BIT_MODE.LEVEL_SHIFT_ON)
                {
                    // VCC ON
                    mask |= _maskVccOn;
                    //FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 High 로 변경
                    //if (ftStatus != FT_STATUS.FT_OK)
                    //{
                    //    Logger.Error("Failed to turn on VCC. Please check the device and try again.", typeof(FTDIDeviceManager).Name);
                    //    return result;
                    //}

                    //// 디바이스 응답 시간을 위해 500ms 대기
                    //Thread.Sleep(500);

                    // LEVEL SHIFT ON
                    // LEVEL SHIFT ON 핀 상태를 유지해야 하므로 mask 변수를 초기화하지 않는다. 
                    mask |= _maskLevelShiftOn; 
                    FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 High 로 변경
                    if (ftStatus != FT_STATUS.FT_OK)
                    {
                        Logger.Error($"\tFailed to turn on Level shift. Please verify configuration and try again.");
                        return result;
                    }

                    result = true;
                }
                else if (mode == BIT_MODE.LEVEL_SHIFT_OFF)
                {
                    // LEVEL SHIFT OFF 
                    mask |= _maskLevelShiftOff; // 0x20
                    // VCC 는 ON 상태를 유지
                    mask |= _maskVccOn;         // 0x44
                    // GPIO ON 
                    mask |= _maskGpioOn;       // 0x88
                    
                    FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 low 로 변경
                    if (ftStatus != FT_STATUS.FT_OK)
                    {
                        Logger.Error($"\tFailed to turn off Level shift. Please check the device and try again.");
                        return result;
                    }

                    result = true;
                }
                else if (mode == BIT_MODE.POWER_ON)
                {
                    // LEVEL SHIFT ON
                    mask |= _maskLevelShiftOn; // 0x22
                    // VCC 는 ON 상태를 유지
                    mask |= _maskVccOn;         // 0x44
                    // GPIO ON 
                    mask |= _maskGpioOn;       // 0x88

                    FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 low 로 변경
                    if (ftStatus != FT_STATUS.FT_OK)
                    {
                        Logger.Error($"\tFailed to turn off Level shift. Please check the device and try again.");
                        return result;
                    }

                    result = true;
                }
                else if (mode == BIT_MODE.POWER_OFF)
                {
                    // LEVEL SHIFT OFF 
                    mask |= _maskLevelShiftOff; // 0x20
                    // VCC 는 OFF
                    mask |= _maskVccOff;         // 0x40
                    // GPIO OFF 
                    mask |= _maskGpioOff;       // 0x80

                    FT_STATUS ftStatus = _ftdi.SetBitMode(mask, _maskBitBangMode); // CUBS Pin 상태를 low 로 변경
                    if (ftStatus != FT_STATUS.FT_OK)
                    {
                        Logger.Error($"\tFailed to turn off Level shift. Please check the device and try again.");
                        return result;
                    }

                    result = true;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"\tException : {e.Message}");
                result = false;
            }
            finally
            {
                if (_ftdi.IsOpen)
                    _ftdi.Close();
            }
            return result;
        }

        private void ReadCBUSPins()
        {
            // 디바이스 열기
            //String deviceDescription = comboBoxDevices.SelectedText;
            //FT_STATUS ftStatus = myFtdiDevice.OpenByDescription(deviceDescription);
            //if (ftStatus != FT_STATUS.FT_OK)
            //{
            //    MessageBox.Show("Failed to open device.");
            //    return;
            //}

            //byte ucMask = 0;

            //// CBUS 핀 상태 가져오기
            //ftStatus = myFtdiDevice.GetPinStates(ref ucMask);
            //if (ftStatus != FT_STATUS.FT_OK)
            //{
            //    MessageBox.Show("Failed to read data from CBUS pins.", "Error");
            //    // 디바이스 핸들 닫기
            //    myFtdiDevice.Close();
            //    return;
            //}

            //// 핀 상태 분석
            //AnalyzePinStates(ucMask);

            //// 디바이스 핸들 닫기
            //myFtdiDevice.Close();
        }

        private void AnalyzePinStates(byte ucMask)
        {
            // 각 CBUS 핀의 상태를 분석

            //if (ucMask & 0x01)
            //{
            //    // bit 0 is high
            //    m_high0.SetCheck(BST_CHECKED);
            //    m_low0.SetCheck(BST_UNCHECKED);

            //    radioHigh0.Checked = (ucMask & 0x01) != 0;
            //    radioLow0.Checked = !radioHigh0.Checked;
            //}
            //else
            //{
            //    // bit 0 is low
            //    m_high0.SetCheck(BST_UNCHECKED);
            //    m_low0.SetCheck(BST_CHECKED);
            //}
            //if (ucMask & 0x02)
            //{
            //    // bit 1 is high
            //    m_high1.SetCheck(BST_CHECKED);
            //    m_low1.SetCheck(BST_UNCHECKED);
            //}
            //else
            //{
            //    // bit 1 is low
            //    m_high1.SetCheck(BST_UNCHECKED);
            //    m_low1.SetCheck(BST_CHECKED);
            //}
            //if (ucMask & 0x04)
            //{
            //    // bit 2 is high
            //    m_high2.SetCheck(BST_CHECKED);
            //    m_low2.SetCheck(BST_UNCHECKED);
            //}
            //else
            //{
            //    // bit 2 is low
            //    m_high2.SetCheck(BST_UNCHECKED);
            //    m_low2.SetCheck(BST_CHECKED);
            //}
            //if (ucMask & 0x08)
            //{
            //    // bit 3 is high
            //    m_high3.SetCheck(BST_CHECKED);
            //    m_low3.SetCheck(BST_UNCHECKED);
            //}
            //else
            //{
            //    // bit 3 is low
            //    m_high3.SetCheck(BST_UNCHECKED);
            //    m_low3.SetCheck(BST_CHECKED);
            //}


        }

    //    private void button1_Click(object sender, EventArgs e)
    //    {
    //        UInt32 ftdiDeviceCount = 0;
    //        FT_STATUS ftStatus = FT_STATUS.FT_OK;

    //        // Create new instance of the FTDI device class
    //        FTDI myFtdiDevice = new FTDI();

    //        // Determine the number of FTDI devices connected to the machine
    //        ftStatus = myFtdiDevice.GetNumberOfDevices(ref ftdiDeviceCount);
    //        // Check status
    //        if (ftStatus == FT_STATUS.FT_OK)
    //        {
    //            Console.WriteLine($"\tNumber of FTDI devices: " + ftdiDeviceCount.ToString());
    //        }
    //        else
    //        {
    //            // Wait for a key press
    //            Console.WriteLine($"\tFailed to get number of devices (error " + ftStatus.ToString() + ")");
    //            Console.ReadKey();
    //            return;
    //        }

    //        // If no devices available, return
    //        if (ftdiDeviceCount == 0)
    //        {
    //            // Wait for a key press
    //            Console.WriteLine($"\tFailed to get number of devices (error " + ftStatus.ToString() + ")");
    //            Console.ReadKey();
    //            return;
    //        }

    //        // Allocate storage for device info list
    //        FTDI.FT_DEVICE_INFO_NODE[] ftdiDeviceList = new FTDI.FT_DEVICE_INFO_NODE[ftdiDeviceCount];

    //        // Populate our device list
    //        ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

    //        if (ftStatus == FT_STATUS.FT_OK)
    //        {
    //            for (UInt32 i = 0; i < ftdiDeviceCount; i++)
    //            {
    //                Console.WriteLine("Device Index: " + i.ToString());
    //                Console.WriteLine("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags));
    //                Console.WriteLine("Type: " + ftdiDeviceList[i].Type.ToString());
    //                Console.WriteLine("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID));
    //                Console.WriteLine("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId));
    //                Console.WriteLine("Serial Number: " + ftdiDeviceList[i].SerialNumber.ToString());
    //                Console.WriteLine("Description: " + ftdiDeviceList[i].Description.ToString());
    //                Console.WriteLine("");
    //            }
    //        }


    //        // Open first device in our list by serial number
    //        ftStatus = myFtdiDevice.OpenBySerialNumber(ftdiDeviceList[0].SerialNumber);
    //        if (ftStatus != FT_STATUS.FT_OK)
    //        {
    //            // Wait for a key press
    //            Console.WriteLine("Failed to open device (error " + ftStatus.ToString() + ")");
    //            Console.ReadKey();
    //            return;
    //        }


    //        // Create our device EEPROM structure based on the type of device we have open
    //        if (ftdiDeviceList[0].Type == FTDI.FT_DEVICE.FT_DEVICE_232R)
    //        {
    //            // We have an FT232R or FT245R so use FT232R EEPROM structure
    //            FTDI.FT232R_EEPROM_STRUCTURE myEEData = new FTDI.FT232R_EEPROM_STRUCTURE();
    //            // Read the device EEPROM
    //            // This can throw an exception if trying to read a device type that does not 
    //            // match the EEPROM structure being used, so should always use a 
    //            // try - catch block when calling
    //            try
    //            {
    //                ftStatus = myFtdiDevice.ReadFT232REEPROM(myEEData);
    //            }
    //            catch (FTDI.FT_EXCEPTION)
    //            {
    //                Console.WriteLine("Exception thrown when calling ReadFT232REEPROM");
    //            }

    //            if (ftStatus != FT_STATUS.FT_OK)
    //            {
    //                // Wait for a key press
    //                Console.WriteLine("Failed to read device EEPROM (error " + ftStatus.ToString() + ")");
    //                Console.ReadKey();
    //                // Close the device
    //                myFtdiDevice.Close();
    //                return;
    //            }

    //            // Write common EEPROM elements to our console
    //            Console.WriteLine("EEPROM Contents for device at index 0:");
    //            Console.WriteLine("Vendor ID: " + String.Format("{0:x}", myEEData.VendorID));
    //            Console.WriteLine("Product ID: " + String.Format("{0:x}", myEEData.ProductID));
    //            Console.WriteLine("Manufacturer: " + myEEData.Manufacturer.ToString());
    //            Console.WriteLine("Manufacturer ID: " + myEEData.ManufacturerID.ToString());
    //            Console.WriteLine("Description: " + myEEData.Description.ToString());
    //            Console.WriteLine("Serial Number: " + myEEData.SerialNumber.ToString());
    //            Console.WriteLine("Max Power: " + myEEData.MaxPower.ToString() + "mA");
    //            Console.WriteLine("Self Powered: " + myEEData.SelfPowered.ToString());
    //            Console.WriteLine("Remote Wakeup Enabled: " + myEEData.RemoteWakeup.ToString());
    //            Console.WriteLine("");

    //            // Change our serial number to write back to device
    //            // By setting to an empty string, we allow the FTD2XX DLL 
    //            // to generate a serial number
    //            myEEData.SerialNumber = String.Empty;

    //            // Write our modified data structure back to the device EEPROM
    //            // This can throw an exception if trying to write a device type that does not 
    //            // match the EEPROM structure being used, so should always use a 
    //            // try - catch block when calling
    //            try
    //            {
    //                ftStatus = myFtdiDevice.WriteFT232REEPROM(myEEData);
    //            }
    //            catch (FTDI.FT_EXCEPTION)
    //            {
    //                Console.WriteLine("Exception thrown when calling WriteFT232REEPROM");
    //            }

    //            if (ftStatus != FT_STATUS.FT_OK)
    //            {
    //                // Wait for a key press
    //                Console.WriteLine("Failed to write device EEPROM (error " + ftStatus.ToString() + ")");
    //                Console.ReadKey();
    //                // Close the device
    //                myFtdiDevice.Close();
    //                return;
    //            }
    //        }
    //        else if (ftdiDeviceList[0].Type == FTDI.FT_DEVICE.FT_DEVICE_2232)
    //        {
    //            // We have an FT2232 so use FT2232 EEPROM structure
    //            FTDI.FT2232_EEPROM_STRUCTURE myEEData = new FTDI.FT2232_EEPROM_STRUCTURE();
    //            // Read the device EEPROM
    //            ftStatus = myFtdiDevice.ReadFT2232EEPROM(myEEData);
    //            // This can throw an exception if trying to read a device type that does not 
    //            // match the EEPROM structure being used, so should always use a 
    //            // try - catch block when calling
    //            try
    //            {
    //                ftStatus = myFtdiDevice.ReadFT2232EEPROM(myEEData);
    //            }
    //            catch (FTDI.FT_EXCEPTION)
    //            {
    //                Console.WriteLine("Exception thrown when calling ReadFT2232EEPROM");
    //            }

    //            if (ftStatus != FT_STATUS.FT_OK)
    //            {
    //                // Wait for a key press
    //                Console.WriteLine("Failed to read device EEPROM (error " + ftStatus.ToString() + ")");
    //                Console.ReadKey();
    //                // Close the device
    //                myFtdiDevice.Close();
    //                return;
    //            }

    //            // Write common EEPROM elements to our console
    //            Console.WriteLine("EEPROM Contents for device at index 0:");
    //            Console.WriteLine("Vendor ID: " + String.Format("{0:x}", myEEData.VendorID));
    //            Console.WriteLine("Product ID: " + String.Format("{0:x}", myEEData.ProductID));
    //            Console.WriteLine("Manufacturer: " + myEEData.Manufacturer.ToString());
    //            Console.WriteLine("Manufacturer ID: " + myEEData.ManufacturerID.ToString());
    //            Console.WriteLine("Description: " + myEEData.Description.ToString());
    //            Console.WriteLine("Serial Number: " + myEEData.SerialNumber.ToString());
    //            Console.WriteLine("Max Power: " + myEEData.MaxPower.ToString() + "mA");
    //            Console.WriteLine("Self Powered: " + myEEData.SelfPowered.ToString());
    //            Console.WriteLine("Remote Wakeup Enabled: " + myEEData.RemoteWakeup.ToString());
    //            Console.WriteLine("");

    //            // Change our serial number to write back to device
    //            // By setting to an empty string, we allow the FTD2XX DLL 
    //            // to generate a serial number
    //            myEEData.SerialNumber = String.Empty;

    //            // Write our modified data structure back to the device EEPROM
    //            // This can throw an exception if trying to write a device type that does not 
    //            // match the EEPROM structure being used, so should always use a 
    //            // try - catch block when calling
    //            try
    //            {
    //                ftStatus = myFtdiDevice.WriteFT2232EEPROM(myEEData);
    //            }
    //            catch (FTDI.FT_EXCEPTION)
    //            {
    //                Console.WriteLine("Exception thrown when calling WriteFT2232EEPROM");
    //            }

    //            if (ftStatus != FT_STATUS.FT_OK)
    //            {
    //                // Wait for a key press
    //                Console.WriteLine("Failed to write device EEPROM (error " + ftStatus.ToString() + ")");
    //                Console.ReadKey();
    //                // Close the device
    //                myFtdiDevice.Close();
    //                return;
    //            }
    //        }
    //        else if (ftdiDeviceList[0].Type == FTDI.FT_DEVICE.FT_DEVICE_BM)
    //        {
    //            // We have an FT232B or FT245B so use FT232B EEPROM structure
    //            FTDI.FT232B_EEPROM_STRUCTURE myEEData = new FTDI.FT232B_EEPROM_STRUCTURE();
    //            // Read the device EEPROM
    //            ftStatus = myFtdiDevice.ReadFT232BEEPROM(myEEData);
    //            // This can throw an exception if trying to read a device type that does not 
    //            // match the EEPROM structure being used, so should always use a 
    //            // try - catch block when calling
    //            try
    //            {
    //                ftStatus = myFtdiDevice.ReadFT232BEEPROM(myEEData);
    //            }
    //            catch (FTDI.FT_EXCEPTION)
    //            {
    //                Console.WriteLine("Exception thrown when calling ReadFT232BEEPROM");
    //            }

    //            if (ftStatus != FT_STATUS.FT_OK)
    //            {
    //                // Wait for a key press
    //                Console.WriteLine("Failed to read device EEPROM (error " + ftStatus.ToString() + ")");
    //                Console.ReadKey();
    //                // Close the device
    //                myFtdiDevice.Close();
    //                return;
    //            }

    //            // Write common EEPROM elements to our console
    //            Console.WriteLine("EEPROM Contents for device at index 0:");
    //            Console.WriteLine("Vendor ID: " + String.Format("{0:x}", myEEData.VendorID));
    //            Console.WriteLine("Product ID: " + String.Format("{0:x}", myEEData.ProductID));
    //            Console.WriteLine("Manufacturer: " + myEEData.Manufacturer.ToString());
    //            Console.WriteLine("Manufacturer ID: " + myEEData.ManufacturerID.ToString());
    //            Console.WriteLine("Description: " + myEEData.Description.ToString());
    //            Console.WriteLine("Serial Number: " + myEEData.SerialNumber.ToString());
    //            Console.WriteLine("Max Power: " + myEEData.MaxPower.ToString() + "mA");
    //            Console.WriteLine("Self Powered: " + myEEData.SelfPowered.ToString());
    //            Console.WriteLine("Remote Wakeup Enabled: " + myEEData.RemoteWakeup.ToString());
    //            Console.WriteLine("");

    //            // Change our serial number to write back to device
    //            // By setting to an empty string, we allow the FTD2XX DLL 
    //            // to generate a serial number
    //            myEEData.SerialNumber = String.Empty;

    //            // Write our modified data structure back to the device EEPROM
    //            // This can throw an exception if trying to write a device type that does not 
    //            // match the EEPROM structure being used, so should always use a 
    //            // try - catch block when calling
    //            try
    //            {
    //                ftStatus = myFtdiDevice.WriteFT232BEEPROM(myEEData);
    //            }
    //            catch (FTDI.FT_EXCEPTION)
    //            {
    //                Console.WriteLine("Exception thrown when calling WriteFT232BEEPROM");
    //            }

    //            if (ftStatus != FT_STATUS.FT_OK)
    //            {
    //                // Wait for a key press
    //                Console.WriteLine("Failed to write device EEPROM (error " + ftStatus.ToString() + ")");
    //                Console.ReadKey();
    //                // Close the device
    //                myFtdiDevice.Close();
    //                return;
    //            }
    //        }


    //        // Use cycle port to force a re-enumeration of the device.  
    //        // In the FTD2XX_NET class library, the cycle port method also 
    //        // closes the open handle so no need to call the Close method separately.
    //        ftStatus = myFtdiDevice.CyclePort();

    //        UInt32 newFtdiDeviceCount = 0;
    //        do
    //        {
    //            // Wait for device to be re-enumerated
    //            // The device will have the same location since it has not been 
    //            // physically unplugged, so we will keep trying to open it until it succeeds
    //            ftStatus = myFtdiDevice.OpenByLocation(ftdiDeviceList[0].LocId);
    //            Thread.Sleep(1000);
    //        } while (ftStatus != FT_STATUS.FT_OK);

    //        // Close the device
    //        myFtdiDevice.Close();

    //        // Re-create our device list
    //        ftStatus = myFtdiDevice.GetNumberOfDevices(ref newFtdiDeviceCount);
    //        if (ftStatus != FT_STATUS.FT_OK)
    //        {
    //            // Wait for a key press
    //            Console.WriteLine("Failed to get number of devices (error " + ftStatus.ToString() + ")");
    //            Console.ReadKey();
    //            return;
    //        }

    //        // Re-populate our device list
    //        ftStatus = myFtdiDevice.GetDeviceList(ftdiDeviceList);

    //        if (ftStatus == FT_STATUS.FT_OK)
    //        {
    //            for (UInt32 i = 0; i < ftdiDeviceCount; i++)
    //            {
    //                Console.WriteLine("Device Index: " + i.ToString());
    //                Console.WriteLine("Flags: " + String.Format("{0:x}", ftdiDeviceList[i].Flags));
    //                Console.WriteLine("Type: " + ftdiDeviceList[i].Type.ToString());
    //                Console.WriteLine("ID: " + String.Format("{0:x}", ftdiDeviceList[i].ID));
    //                Console.WriteLine("Location ID: " + String.Format("{0:x}", ftdiDeviceList[i].LocId));
    //                Console.WriteLine("Serial Number: " + ftdiDeviceList[i].SerialNumber.ToString());
    //                Console.WriteLine("Description: " + ftdiDeviceList[i].Description.ToString());
    //                Console.WriteLine("");
    //            }
    //        }

    //        // Wait for a key press
    //        Console.WriteLine("Press any key to continue.");
    //        Console.ReadKey();
    //        return;
    //    }
    }
}
