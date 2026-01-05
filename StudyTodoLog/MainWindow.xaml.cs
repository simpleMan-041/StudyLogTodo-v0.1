using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.IO;
using Microsoft.Data.Sqlite;

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
        public void CreateDatabase()
        {
            string dbPath = GetDatabasePath();

            var connectionString = new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users(
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL
                    );";
                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}