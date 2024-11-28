using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using bach_hjalp.Databases;

namespace bach_hjalp
{
    public partial class MainPage : ContentPage
    {
        private readonly RawDatabase _rawDatabase;
        private readonly CSV_Importer_Test _csvImporter;

        public ObservableCollection<RawDatabaseModel> ECGDataList { get; set; }

        public MainPage()
        {
            InitializeComponent();

            ECGDataList = new ObservableCollection<RawDatabaseModel>();
            BindingContext = this;

            _rawDatabase = new RawDatabase();
            _csvImporter = new CSV_Importer_Test();

            LoadAndDisplayData();
        }

        private async void LoadAndDisplayData()
        {
            try
            {
                // Clear existing database data
                await _rawDatabase.ClearAllDataAsync();

                // Import data from the CSV file
                await _csvImporter.ImportCSVToDatabaseAsync(_rawDatabase);

                // Load data into the ObservableCollection for the CollectionView
                var data = await _rawDatabase.GetAllDataAsync();
                ECGDataList.Clear();
                foreach (var item in data)
                {
                    ECGDataList.Add(item);
                }

                // Create and display the chart
                CreateChart(data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading and displaying data: {ex.Message}");
            }
        }

        private void CreateChart(IEnumerable<RawDatabaseModel> data)
        {
            try
            {
                var entries = data.Select(item => new ChartEntry((float)item.ECG_data)
                {
                    Label = item.Time.ToString("0.##"),
                    ValueLabel = item.ECG_data.ToString("0.##"),
                    Color = SKColor.Parse("#77d065")
                }).ToList();

                MyChart.Chart = new LineChart
                {
                    Entries = entries,
                    LineMode = LineMode.Straight,
                    LineSize = 4,
                    PointSize = 8,
                    LabelTextSize = 20,
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating chart: {ex.Message}");
            }
        }
    }

}
