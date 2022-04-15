using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryTestApp
{
    public static class Settings
    {
        public static string BinaryFileName
        {
            get
            {
                return ConfigurationManager.AppSettings[nameof(BinaryFileName)];
            }
        }
        public static string CsvFileName
        {
            get
            {
                return ConfigurationManager.AppSettings[nameof(CsvFileName)];
            }
        }
    }
}
