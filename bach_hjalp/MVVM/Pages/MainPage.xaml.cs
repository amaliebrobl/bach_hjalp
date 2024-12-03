using Microcharts;
using SkiaSharp;
using System.Collections.ObjectModel;
using bachHjalp.Databases;
using System.Diagnostics;
using Microcharts.Maui;
using Microsoft.Maui.Controls;

namespace bachHjalp
{
    public partial class MainPage : ContentPage
    {
        private readonly RawDatabase _rawDatabase;
        private readonly CSV_Importer_Test _csvImporter;

        public ObservableCollection<RawDatabaseModel> ECGDataList { get; set; }

        // Main Page comment
        public MainPage()
        {
            InitializeComponent();

            ECGDataList = new ObservableCollection<RawDatabaseModel>();
            BindingContext = this;

            _rawDatabase = new RawDatabase();
            _csvImporter = new CSV_Importer_Test();

            Task.Run(async () => LoadAndDisplayData());
        }

        private async Task LoadAndDisplayData()
        {
            try
            {
                // Clear existing database data
                await _rawDatabase.ClearAllDataAsync();

                // Import data from the CSV file
                await _csvImporter.ImportCSVToDatabaseAsync(_rawDatabase);

                // Load data into the ObservableCollection for the CollectionView
                var data = await _rawDatabase.GetAllDataAsync();

                // Debug: Tjek om data indeholder noget
                Debug.WriteLine($"Data loaded: {data.Count()} items.");

                ECGDataList.Clear();
                foreach (var item in data)
                {
                    ECGDataList.Add(item);
                }

                if (data.Any())
                {
                    // Create and display the chart
                    Debug.WriteLine("Creating chart...");
                    CreateChart(data);
                }
                else
                {
                    Debug.WriteLine("empty to display in the chart");
                }
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
                var entries = data.Select(static item => new ChartEntry((float)item.ECG_data)
                {
                    Label = item.Time.ToString(),
                    ValueLabel = item.ECG_data.ToString(),
                }).ToList();

                var chart = new LineChart { Entries = entries };

                // Chart vises på UI
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    if (chartView == null)
                    {
                        Debug.WriteLine("chartView is null!");
                    }
                    else
                    {
                        Debug.WriteLine($"chartView dimensions: Height={chartView.Height}, Width={chartView.Width}");
                        chartView.Chart = chart;
                    }
                });

                Debug.WriteLine("Chart created and assigned.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error creating chart: {ex.Message}");
            }
        }
    }
}
