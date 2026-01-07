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
        

        private void LoadTasks()
        {
            string cs = new SqliteConnectionStringBuilder
            {
                DataSource = _db.GetDatabasePath()
            }.ToString();

            var tasks = TaskModel.GetAllTasks(cs);
            MessageBox.Show($"タスク件数{tasks.Count}");
        }

        private void TestInsert()
        {
            var db = new DatabaseManager();
            string cs = new SqliteConnectionStringBuilder
            {
                DataSource = db.GetDatabasePath()
            }.ToString();

            int newId = TaskModel.Insert(cs, "テストタスク", "DBに追加できるか確認します！");
            var tasks = TaskModel.GetAllTasks(cs);

            MessageBox.Show($"追加したId:{newId}\n現在の件数:{tasks.Count}"); 
        }


        public MainWindow()
        {
            InitializeComponent();

            _db.InitializeDatabase();
            LoadTasks();
            TestInsert();

        }

    }
}
