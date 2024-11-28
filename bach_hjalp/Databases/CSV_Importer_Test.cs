using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace bach_hjalp.Databases
{
    public class CSV_Importer_Test
    {
        public async Task ImportCSVToDatabaseAsync(RawDatabase rawDatabase)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ecgChair.Testdata.ECG.csv";

            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                Debug.WriteLine($"File '{resourceName}' was not found in the resources.");
                return;
            }

            try
            {
                using var reader = new StreamReader(stream);
                bool isDataSection = false;

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    if (line.StartsWith("Time (s)"))
                    {
                        isDataSection = true;
                        continue;
                    }

                    if (isDataSection)
                    {
                        var values = line.Split(',');

                        if (values.Length < 2) continue;

                        try
                        {
                            double time = double.Parse(values[0], CultureInfo.InvariantCulture);
                            double ecgValue = double.Parse(values[1], CultureInfo.InvariantCulture);

                            var data = new RawDatabaseModel
                            {
                                Time = time,
                                ECG_data = ecgValue
                            };

                            await rawDatabase.AddDataAsync(data);
                        }
                        catch (FormatException ex)
                        {
                            Debug.WriteLine($"Data parsing failed for line '{line}': {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to read the CSV file: {ex.Message}");
            }
        }
    }
}
