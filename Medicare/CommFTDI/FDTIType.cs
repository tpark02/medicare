using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommFTDI
{
    public class FDTI_Type
    {
        /// <summary>
        /// device를 open type - 장치 식별이 가능한 디바이스 인덱스, 시리얼번호, 장치명, 디바이스 위치 중에 선택하여 OPEN</param>
        /// </summary>
        public enum OPEN_TYPE
        {
            INDEX,          // Opens the FTDI device with the specified index.
            SERIALNUMBER,   // Opens the FTDI device with the specified serial number.
            DESCRIPTION,    // Opens the FTDI device with the specified description.
            LOCATION,       // Opens the FTDI device at the specified physical location.
        }

        public enum BIT_MODE
        {
            LEVEL_SHIFT_ON,
            LEVEL_SHIFT_OFF,
            POWER_ON,
            POWER_OFF,
            ON,
            OFF
        }
    }
}
    