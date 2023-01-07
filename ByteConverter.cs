using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisEn
{
    public class ByteConverter
    {
        static public double ConvertByteToMegaByte(double byteToConvert)
        {
            return byteToConvert / 1048576.0;
        }

        static public String ConvertByToMegaByteToString(double byteToConvert)
        {
            double MB = byteToConvert / 1048576.0;
            return MB.ToString("0.###") + " MB";
        }
    }
}
