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
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
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
        TextBox textBox3;
        DateTimePicker date;
        DateTimePicker time;
        NumericUpDown districtId;
        Button pickPathToResult;
        Button pickPathToLogs;
        Button pickPathToData;
        Button apply;
        FolderBrowserDialog setPathToResult;
        FolderBrowserDialog setPathToLogs;
        OpenFileDialog setPathToData;
        public Form1()
        {
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new FileDialogPermission(PermissionState.Unrestricted));
            Text = "DeliveryApp pre-alpha release";

            setPathToResult = new FolderBrowserDialog();
            setPathToLogs = new FolderBrowserDialog();
            setPathToData = new OpenFileDialog();

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

            pickPathToResult = new Button();
            pickPathToResult.Text = "Указать путь к файлу с результатом выборки";
            pickPathToResult.Click += FileDirectory_Click;
            pickPathToResult.Left = 10;
            pickPathToResult.Top = textBox1.Bottom;
            pickPathToResult.Width = ClientSize.Width - 20;
            pickPathToResult.Height = 20;
            Controls.Add(pickPathToResult);

            textBox2 = new TextBox();
            textBox2.Left = 10;
            textBox2.Top = pickPathToResult.Bottom;
            textBox2.Width = ClientSize.Width - 20;
            textBox2.Height = 20;
            Controls.Add(textBox2);

            pickPathToLogs = new Button();
            pickPathToLogs.Text = "Указать путь к файлу с логами";
            pickPathToLogs.Click += LogsDirectory_Click;
            pickPathToLogs.Left = 10;
            pickPathToLogs.Top = textBox2.Bottom;
            pickPathToLogs.Width = ClientSize.Width - 20;
            pickPathToLogs.Height = 20;
            Controls.Add(pickPathToLogs);

            textBox3 = new TextBox();
            textBox3.Left = 10;
            textBox3.Top = pickPathToLogs.Bottom;
            textBox3.Width = ClientSize.Width - 20;
            textBox3.Height = 20;
            Controls.Add(textBox3);



            pickPathToData = new Button();
            pickPathToData.Text = "Указать путь к файлу с данными";
            pickPathToData.Click += DataDirectory_Click;
            pickPathToData.Left = 10;
            pickPathToData.Top = textBox3.Bottom;
            pickPathToData.Width = ClientSize.Width - 20;
            pickPathToData.Height = 20;
            Controls.Add(pickPathToData);

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
            if (setPathToResult.SelectedPath == "" || setPathToLogs.SelectedPath == "")
            {
                MessageBox.Show("Укажите путь к папкам");
                return;
            }
            if (date.Value.Date + time.Value.TimeOfDay > DateTime.Now)
            {
                MessageBox.Show("Дата должна быть меньше текущей");
                return;
            }
            var sqlDBWorker = new SQLDBWorker();
            sqlDBWorker.SetValues(setPathToResult.SelectedPath);
            var textFileWorker = new TextFileWorker();
            
            textFileWorker.SetValues(setPathToResult.SelectedPath);
            var queue = new Queue<OrderInfo>();
            textFileWorker.GetData((int)districtId.Value, date.Value.Date + time.Value.TimeOfDay, queue, textFileWorker.CreateReport);
            //sqlDBWorker.GetData((int)districtId.Value, date.Value.Date + time.Value.TimeOfDay, queue, textFileWorker.CreateReport);

        }


        private void FileDirectory_Click(object sender, EventArgs e)
        {
            if (setPathToResult.ShowDialog() == DialogResult.OK)
                setPathToResult.RootFolder = Environment.SpecialFolder.Desktop;
            textBox1.Text = setPathToResult.SelectedPath;
        }
        private void LogsDirectory_Click(object sender, EventArgs e)
        {
            if (setPathToLogs.ShowDialog() == DialogResult.OK)
                setPathToLogs.RootFolder = Environment.SpecialFolder.Desktop;
            textBox2.Text = setPathToLogs.SelectedPath;
        }
        private void DataDirectory_Click(object sender, EventArgs e)
        {
            setPathToData.Filter = "Текстовый файл. Файл формата  .txt|*.txt";
            if (setPathToData.ShowDialog() == DialogResult.OK)
                textBox3.Text = setPathToData.FileName;
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Form1";
            this.ResumeLayout(false);

        }
    }
}
