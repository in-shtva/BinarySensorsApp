using BinaryTestApp.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace BinaryTestApp.Services
{
    public class BinarySensorsService
    {
        private readonly List<SensorModel> sensorModels;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public BinarySensorsService(List<SensorModel> sensors)
        {
            sensorModels = sensors;
        }

        public void SensorValuesWrite()
        {
            writeConfigs();
            write();
            toCsv();
        }

        private void toCsv()
        {
            var readData = readSensors();
            try
            {
                using (var sw = new StreamWriter(Settings.CsvFileName, false, Encoding.Default))
                {
                    sw.WriteLine("Guid сенсора;Значение до обработки;Округляем до;Значение после обработки (из бинарника);Округляем до (из конфига);Исходное значение (округление, на основе данных из бинарника)");

                    readData.ForEach(data =>
                    {
                        sw.WriteLine($"{data.Guid};{data.StartValue};{data.FloatNumber};{data.UpdatedValueFromBinary};{data.FloatNumberFromConfig};{data.StartValueFromBinary}");
                    });

                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in to_csv. Message: {ex.Message}");
            }
        }

        private void write()
        {
            try
            {
                using (var writer = new BinaryWriter(File.Open(Settings.BinaryFileName, FileMode.Create)))
                {
                    sensorModels.ForEach(sensor =>
                    {
                        writer.Write(sensor.UpdatedValue);
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in write. Message: {ex.Message}");
            }
        }

        /// <summary>
        /// read information from binary file
        /// </summary>
        private List<ResultModel> readSensors()
        {
            var result = new List<ResultModel>();
            try
            {
                using (var reader = new BinaryReader(File.Open(Settings.BinaryFileName, FileMode.Open)))
                {
                    var list = new List<int>();

                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        list.Add(reader.ReadInt32());
                    }

                    list.ForEach(value =>
                    {
                        var sensor = sensorModels.FirstOrDefault(a => a.UpdatedValue == value);
                        if (sensor == null)
                        {
                            _logger.Warn($"Sensor with value {value} wasn't found");
                        } else
                        {
                            _logger.Info($"Sensor was successfully found. Updated value = {value}, start value = {sensor.StartValue}");

                            var gettingResult= int.TryParse(ConfigurationManager.AppSettings[$"{sensor.Guid}"], out int valueFromConfig);
                            result.Add(new ResultModel(sensor, value, gettingResult ? valueFromConfig : -1 ));
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in ReadSensors. Message: {ex.Message}");
            }

            return result;
        }

        /// <summary>
        /// добавляем параметры в configFile
        /// </summary>
        private void writeConfigs()
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;

                sensorModels.ForEach(sensor =>
                {
                    var key = $"{sensor.Guid}";
                    if (settings[key] == null)
                    {
                        settings.Add(key, sensor.FloatNumber.ToString());
                    }
                    else
                    {
                        settings[key].Value = sensor.FloatNumber.ToString();
                    }
                });

                configFile.Save(ConfigurationSaveMode.Modified);

                var e = configFile.AppSettings.SectionInformation.Name;
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
                _logger.Info("Successfully added new values ​​to config");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error in writeConfigs. Message: {ex.Message}");
            }
        }
    }
}
