using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        Label label1;
        Label label2;
        Label label3;
        Label label4;
        TextBox textBox1;
        TextBox textBox2;
        DateTimePicker date;
        DateTimePicker time;
        NumericUpDown districtId;
        Button fileDirectory;
        Button logsDirectory;
        Button apply;
        FolderBrowserDialog folderBrowserDialog1;
        FolderBrowserDialog folderBrowserDialog2;
        public Form1()
        {
            Text = "DeliveryApp pre-alpha release";

            folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog2 = new FolderBrowserDialog();

            label1 = new Label();
            label1.Text = "Введите параметры сортировки";
            label1.Top = 20;
            label1.Left = 10;
            label1.Width = ClientSize.Width - 20;
            Controls.Add(label1);

            label2 = new Label()
            {
                Text = "Район:", 
                Top = label1.Bottom,
                Left = 10,
                Width = 50
            };
            Controls.Add(label2);

            districtId = new NumericUpDown();
            districtId.Left = label2.Right;
            districtId.Top = label1.Bottom;
            districtId.Width = ClientSize.Width - label2.Width - 20;
            districtId.Height = 20;
            Controls.Add(districtId);

            label3 = new Label();
            label3.Text = "Дата:";
            label3.Top = label2.Bottom;
            label3.Left = 10;
            label3.Width = 50;
            Controls.Add(label3);

            date = new DateTimePicker();
            date.Left = label3.Right;
            date.Top = label2.Bottom;
            date.Width = ClientSize.Width - label2.Width - 20;
            date.Height = 20;
            Controls.Add(date);

            label4 = new Label();
            label4.Text = "Время:";
            label4.Top = label3.Bottom;
            label4.Left = 10;
            label4.Width = 50;
            Controls.Add(label4);

            time = new DateTimePicker();
            time.Left = label4.Right;
            time.Top = label3.Bottom;
            time.Width = ClientSize.Width - label2.Width - 20;
            time.Height = 20;
            time.Format = DateTimePickerFormat.Time;
            time.ShowUpDown = true;
            Controls.Add(time);

            textBox1 = new TextBox();
            textBox1.Left = 10;
            textBox1.Top = label4.Bottom;
            textBox1.Width = ClientSize.Width - 20;
            textBox1.Height = 20;
            Controls.Add(textBox1);

            fileDirectory = new Button();
            fileDirectory.Text = "Указать путь к файлу с результатом выборки";
            fileDirectory.Click += FileDirectory_Click;
            fileDirectory.Left = 10;
            fileDirectory.Top = textBox1.Bottom;
            fileDirectory.Width = ClientSize.Width - 20;
            fileDirectory.Height = 20;
            Controls.Add(fileDirectory);

            textBox2 = new TextBox();
            textBox2.Left = 10;
            textBox2.Top = fileDirectory.Bottom;
            textBox2.Width = ClientSize.Width - 20;
            textBox2.Height = 20;
            Controls.Add(textBox2);

            logsDirectory = new Button();
            logsDirectory.Text = "Указать путь к файлу с логами";
            logsDirectory.Click += LogsDirectory_Click;
            logsDirectory.Left = 10;
            logsDirectory.Top = textBox2.Bottom;
            logsDirectory.Width = ClientSize.Width - 20;
            logsDirectory.Height = 20;
            Controls.Add(logsDirectory);

            apply = new Button();
            apply.Text = "Применить";
            apply.Click += Process;
            apply.Left = 10;
            apply.Top = ClientSize.Height - 50;
            apply.Width = ClientSize.Width - 20;
            apply.Height = 40;
            Controls.Add(apply);
        }
        private async void Process(object sender, EventArgs empty)
        {
            //await Method();
            if (folderBrowserDialog1.SelectedPath == "" || folderBrowserDialog2.SelectedPath == "")
            {
                MessageBox.Show("Укажите путь к папкам");
                return;
            }
            if (date.Value.Date + time.Value.TimeOfDay > DateTime.Now)
            {
                MessageBox.Show("Дата должна быть меньше текущей");
                return;
            }

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                connection.StateChange += connection_StateChange;
                connection.Open();
                SqlCommand cmd1 = new SqlCommand("SELECT DISTINCT DistrictId FROM Deliveries", connection);
                var DistrictIds = cmd1.ExecuteReader();
                var flag = false;
                while (DistrictIds.Read())
                {
                    if ((int)districtId.Value == DistrictIds.GetInt32(0))
                    {
                        flag = true;
                        break;
                    }
                }
                DistrictIds.Close();
                if (!flag)
                {
                    MessageBox.Show("Район не найден");
                    return;
                }
                var startTime = date.Value.Date + time.Value.TimeOfDay;
                SqlCommand cmd = new SqlCommand("SELECT * FROM Deliveries \r\n" +
                    "WHERE DistrictId = @DstrId \r\n" +
                    "and DeliveryTime>=@startTime \r\n" +
                    "and DeliveryTime<= (SELECT DATEADD(minute, 30, DeliveryTime) " +
                    "FROM (SELECT TOP 1 * FROM Deliveries WHERE DistrictId = @DstrId " +
                    "and DeliveryTime>=@startTime Order by DeliveryTime) " +
                    "as DeliveryTime1)"
                    , connection);
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@DstrId", (int)districtId.Value);
                GetResult(cmd);
                connection.Close();
            }
        }

        private void GetResult(SqlCommand cmd)
        {
            string directory = folderBrowserDialog1.SelectedPath;
            string fileName = string.Format("Result{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(directory);
            string filePath = Path.Combine(directory, fileName);
            var counter = 0;
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    while (reader.Read())
                    {
                        writer.WriteLine(reader[0].ToString() + "\t"
                            + reader[1].ToString() + "\t"
                            + reader[2].ToString() + "\t"
                            + ((DateTime)reader[3]).ToString("yyyy-MM-dd HH:mm:ss"));
                        counter++;
                    }
                }
            }
            MessageBox.Show("Всего записей: " + counter);
        }
        static void connection_StateChange(object sender, StateChangeEventArgs e)
        {
            SqlConnection connection = sender as SqlConnection;
            MessageBox.Show("Connection to" + Environment.NewLine +
            "Data Source: " + connection.DataSource + Environment.NewLine +
            "Database: " + connection.Database + Environment.NewLine +
            "State: " + connection.State);
            //TODO добавить логирование (вывод информации о соединении и его состоянии)
            //Console.WriteLine
            //(
            //"Connection to" + Environment.NewLine +
            //"Data Source: " + connection.DataSource + Environment.NewLine +
            //"Database: " + connection.Database + Environment.NewLine +
            //"State: " + connection.State
            //);
        }
        private void FileDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
            textBox1.Text = folderBrowserDialog1.SelectedPath;
        }
        private void LogsDirectory_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
                folderBrowserDialog2.RootFolder = Environment.SpecialFolder.Desktop;
            textBox2.Text = folderBrowserDialog2.SelectedPath;
        }


        async Task Method()
        {
            string path = "D:/input.txt";

            // асинхронное чтение
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    MessageBox.Show(line);
                }
            }
        }
    }
}
