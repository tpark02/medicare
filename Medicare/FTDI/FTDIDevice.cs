using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FTD2XX_NET.FTDI;

namespace CommFTDI
{
    public class FTDIDevice
    {
        /// <summary>
        /// Indicates the device type. Can be one of the following: FT_DEVICE_232R, FT_DEVICE_2232C,
        //  FT_DEVICE_BM, FT_DEVICE_AM, FT_DEVICE_100AX or FT_DEVICE_UNKNOWN
        /// </summary>
        public FT_DEVICE DeviceType { get; set; }
        /// <summary>
        /// The Vendor ID and Product ID of the device
        /// </summary>
        public uint ID { get; set; }
        /// <summary>
        /// The device serial number~
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// The device description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// The device handle. This value is not used externally and is provided for information
        //  only. If the device is not open, this value is 0.
        /// </summary>
        public IntPtr Handle { get; set; }
        /// <summary>
        /// The device comp port
        /// </summary>
        public string ComPort { get; set; }
        /// <summary>
        /// The device firmware file path
        /// </summary>
        public string FirmwareFile { get; set; }
        /// <summary>
        /// The device power on current value
        /// </summary>
        public string PowerOnCurrent { get; set; }
        /// <summary>
        /// The device power off current value
        /// </summary>
        public string PowerOffCurrent { get; set; }


        public override string ToString()
        {
            return $"{ComPort}::{SerialNumber}";
        }
    }
}
