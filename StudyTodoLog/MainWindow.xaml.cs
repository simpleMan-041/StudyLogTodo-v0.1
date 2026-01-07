using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
namespace StudyTodoLog
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DatabaseManager _db = new DatabaseManager();

        public MainWindow()
        {
            InitializeComponent();

            _db.InitializeDatabase();
            LoadTasks();

        }

        private void LoadTasks()
        {
            try
            {
                string cs = new SqliteConnectionStringBuilder
                {
                    DataSource = _db.GetDatabasePath()
                }.ToString();

                var tasks = TaskModel.GetAllTasks(cs);

                TaskListView.ItemsSource = tasks;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "タスクの読み込みに失敗しました\n\n" + ex.Message,
                    "Load Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string cs = new SqliteConnectionStringBuilder
            {
                DataSource = _db.GetDatabasePath()
            }.ToString();

            var window = new AddTaskWindow(cs)
            {
                Owner = this
            };

            bool? result = window.ShowDialog();
            if (result == true)
            {
                LoadTasks();
            }
        }
    }
}
