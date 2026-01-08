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

namespace StudyTodoLog
{
    public partial class AddTaskWindow : Window
    {
        private readonly string _connectionString;

        public AddTaskWindow(string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
            TitleTextBox.Focus();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // タイトルは常に必須。メモは任意である。
                string title = TitleTextBox.Text.Trim();
                string? memo = string.IsNullOrWhiteSpace(MemoTextBox.Text) ? null : MemoTextBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(title))
                {
                    MessageBox.Show("タイトルを入力してください", "入力エラー",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //　DBへタスクを保存させる。
                TaskModel.Insert(_connectionString, title, memo);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存に失敗しました\n\n" + ex.Message, "Save Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
