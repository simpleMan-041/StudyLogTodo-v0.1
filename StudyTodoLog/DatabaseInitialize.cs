using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Windows;

namespace StudyTodoLog
{
    public class DatabaseManager
    {
        public string GetDatabasePath()
        {
            string roamingPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string appFolderName = "StudyLogTodoApp";
            string targetDirectory = System.IO.Path.Combine(roamingPath, appFolderName);

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            return System.IO.Path.Combine(targetDirectory, "app_data.db");
        }
        public void InitializeDatabase()
        {
            try
            {
                string dbPath = GetDatabasePath();

                var connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();

                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Tasks(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Memo TEXT,
                    IsCompleted INTEGER NOT NULL DEFAULT 0
                    );";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "データベースの初期化に失敗しました \n\n" + ex.Message,
                    "DB Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

}