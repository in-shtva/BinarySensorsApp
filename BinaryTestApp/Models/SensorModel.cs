

using System;

namespace BinaryTestApp.Models
{
    public class SensorModel
    {
        public SensorModel() { }
        public SensorModel(int floatNumber, float value)
        {
            FloatNumber = floatNumber;
            StartValue = value;
            UpdatedValue = (int)(Math.Round(value, floatNumber) * Math.Pow(10, floatNumber));
            Guid = Guid.NewGuid();
        }

        public int FloatNumber { get; set; }
        public float StartValue { get; set; }
        public int UpdatedValue { get; set; }

        public Guid Guid { get; set; }
    }
}
