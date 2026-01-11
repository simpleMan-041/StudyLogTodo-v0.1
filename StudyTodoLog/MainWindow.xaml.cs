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

    public partial class MainWindow : Window
    {
        private readonly DatabaseManager _db = new DatabaseManager();
        private readonly string _connectionString;

        private const double WidthRatioCompleted = 0.08;
        private const double WidthRatioTitle = 0.50;
        private const double WidthRatioMemo = 0.48;

        private const double WidthMinCompleted = 80;
        private const double WidthMinTitle = 240;
        private const double WidthMinMemo = 320;
        // !Warning! これ以降Widthはwと省略する

        private DateTime _lastDeleteClickTime = DateTime.MinValue;
        private static readonly TimeSpan DeleteDoubleClickWindow = TimeSpan.FromSeconds(1.5);

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (_, __) => UpdateColumnWidths();
            _db.InitializeDatabase();

            _connectionString = new SqliteConnectionStringBuilder
            {
                DataSource = _db.GetDatabasePath()
            }.ToString();

            LoadTasks();

        }

        private void LoadTasks()
        {
            try
            {
                var tasks = TaskModel.GetAllTasks(_connectionString);
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
            var window = new AddTaskWindow(_connectionString)
            {
                Owner = this
            };

            bool? result = window.ShowDialog();
            if (result == true)
            {
                LoadTasks();
            }
        }

        private void CompletedCheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 操作に対応するチェックボックスのオブジェクトを特定し、状態の更新を行う。
                if (sender is not System.Windows.Controls.CheckBox cb) return;
                if (cb.DataContext is not TaskModel task) return;

                bool isCompleted = cb.IsChecked == true;

                TaskModel.UpdateIsCompleted(_connectionString, task.Id, isCompleted);

                LoadTasks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "完了状態の更新に失敗しました\n\n" + ex.Message,
                    "Update Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                LoadTasks();
            }
        }

        private void DeleteCompletedButton_Click(Object sender, RoutedEventArgs e)
        {
            // 2回連続で押されたか判定
            var now = DateTime.Now;
            bool isSecondPress = (now - _lastDeleteClickTime) <= DeleteDoubleClickWindow;
            _lastDeleteClickTime = now;
            
            // 1回押されたときの削除処理
            if (!isSecondPress)
            {
                try
                {
                    int deletedCount = TaskModel.DeleteCompleted(_connectionString);
                    LoadTasks();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                            "完了タスクの削除に失敗しました\n\n" + ex.Message,
                            "Delete Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                }
                return;
            }
            // 2回押された時、確認後全削除
           var result = MessageBox.Show(
                "全てのタスクを削除します\nこの操作は取り消せません。\n\nよろしいですか？",
                "全タスク削除の確認",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.OK)
            {
                _lastDeleteClickTime = DateTime.MinValue;
                return;
            }

            try
            {
                TaskModel.DeleteAllTasks(_connectionString);
                LoadTasks();
            }

            catch (Exception ex)
            {
                MessageBox.Show(
                    "全タスクの削除に失敗しました\n\n" + ex.Message,
                    "Delete Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _lastDeleteClickTime = DateTime.MinValue;
            }
        } 

        private void TaskItem_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // ダブルクリックによって選択されたタスクを特定し、編集画面を開く
            try
            {
                if (sender is not System.Windows.Controls.ListViewItem item) return;
                if (item.Content is not TaskModel task) return;

                var window = new EditTaskWindow(_connectionString, task)
                {
                    Owner = this
                };

                bool? result = window.ShowDialog();
                if (result == true)
                {
                    LoadTasks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "編集画面を開けませんでした\n\n" + ex.Message,
                    "Edit Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        private void TaskListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateColumnWidths();
        }

        private void UpdateColumnWidths()
        {
            if (ColCompleted == null || ColTitle == null || ColMemo == null) return;

            // ListViewの表示領域からスクロールバー分の横幅を取り除き、表頭が取れる幅を計算
            double available = TaskListView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 28;
            if (available <= 0) return;

            double wCompleted = available * WidthRatioCompleted;
            double wTitle = available * WidthRatioTitle;
            double wMemo = available * WidthRatioMemo;

            // 最小幅を保障して潰れを防止します
            wCompleted = Math.Max(WidthMinCompleted, wCompleted);
            wTitle = Math.Max(WidthMinTitle, wTitle);
            wMemo = Math.Max(WidthMinMemo, wMemo);

            // 最小幅が小さすぎる場合は全体を同じ割合で縮小
            double sum = wCompleted + wTitle + wMemo;
            if (sum > available)
            {
                double scale = available / sum;
                wCompleted *= scale;
                wTitle *= scale;
                wMemo *= scale;
            }

            ColCompleted.Width = wCompleted;
            ColTitle.Width = wTitle;
            ColMemo.Width = wMemo;
        }
    }
}
