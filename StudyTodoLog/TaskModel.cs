using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudyTodoLog
{
    class TaskModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = ""; // コントロール操作時に落ちやすいためnullを避けて設計します
        public string? Memo { get; set; }
        public bool IsCompleted { get; set; }

        public static List<TaskModel> GetAllTasks(string connectionString)
        {
            var tasks = new List<TaskModel>();

            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Memo, IsCompleted FROM Tasks";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var task = new TaskModel
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Memo = reader.IsDBNull(2) ? null : reader.GetString(2),
                    IsCompleted = reader.GetInt32(3) == 1
                };

                tasks.Add(task);
            }
            return tasks;
        }
    }
}
