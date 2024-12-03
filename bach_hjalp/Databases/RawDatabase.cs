using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Diagnostics;

namespace bachHjalp.Databases
{
    public class RawDatabase
    {
        private readonly SQLiteAsyncConnection _connection;

        // RawDatabase
        public RawDatabase()
        {
            try
            {
                var dataDirectory = FileSystem.AppDataDirectory;
                var databasePath = Path.Combine(dataDirectory, "ECG.db");

                Directory.CreateDirectory(dataDirectory); // Ensure directory exists

                var dbOptions = new SQLiteConnectionString(databasePath, storeDateTimeAsTicks: true);
                _connection = new SQLiteAsyncConnection(dbOptions);

                Debug.WriteLine($"Database path: {databasePath}");

                Initialize().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database initialization failed: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
        }

        private async Task Initialize()
        {
            try
            {
                await _connection.CreateTableAsync<RawDatabaseModel>();
                Debug.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Database table creation failed: {ex.Message}");
            }
        }

        public async Task<int> AddDataAsync(RawDatabaseModel rawData)
        {
            try
            {
                Debug.WriteLine("Adding data to database...");
                var result = await _connection.InsertAsync(rawData);
                Debug.WriteLine($"Data added: {result}");
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inserting data: {ex.Message}");
                Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return 0;
            }
        }

        public async Task<List<RawDatabaseModel>> GetAllDataAsync()
        {
            try
            {
                return await _connection.Table<RawDatabaseModel>().ToListAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error fetching data: {ex.Message}");
                return new List<RawDatabaseModel>();
            }
        }

        public async Task ClearAllDataAsync()
        {
            try
            {
                await _connection.DeleteAllAsync<RawDatabaseModel>();
                Debug.WriteLine("All data cleared from the database.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error clearing data: {ex.Message}");
            }
        }
    }
}
