using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinaryTestApp.Models
{
    public class ResultModel : SensorModel
    {
        public ResultModel(SensorModel sensor, int updatedValueFromBinary, int floatNumberFromConfig)
        {
            UpdatedValueFromBinary = updatedValueFromBinary;
            FloatNumberFromConfig = floatNumberFromConfig;
            FloatNumber = sensor.FloatNumber;
            StartValue = sensor.StartValue;
            UpdatedValue = sensor.UpdatedValue;
            Guid = sensor.Guid;

            StartValueFromBinary = (float)(UpdatedValueFromBinary / Math.Pow(10, FloatNumberFromConfig));
        }
        public int UpdatedValueFromBinary { get; set; }
        public float StartValueFromBinary { get; set; }
        public int FloatNumberFromConfig { get; set; }
    }
}
