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
        Label label5;
        Label label6;
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
        ComboBox ReportCreatorSelect;
        ComboBox SourceDataSetterSelect;
        Dictionary<int, IReportCreator> reportCreators = new Dictionary<int, IReportCreator>();
        Dictionary<int, ISourceDataSetter> sourceDataSetters = new Dictionary<int, ISourceDataSetter>();
        public Form1()
        {
            
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new FileDialogPermission(PermissionState.Unrestricted));
            Text = "DeliveryApp pre-alpha release";
            this.Size = new Size(300, 500);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            setPathToResult = new FolderBrowserDialog();
            setPathToResult.SelectedPath = "C:\\Users\\Булат\\Desktop";
            setPathToLogs = new FolderBrowserDialog();
            setPathToLogs.SelectedPath = "C:\\Users\\Булат\\Desktop";
            setPathToData = new OpenFileDialog();
            setPathToData.FileName = "D:\\input.txt";
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
            pickPathToResult.Text = "Задать путь сохранения результата выборки";
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
            pickPathToLogs.Text = "Задать путь сохранения логов";
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
            pickPathToData.Text = "Указать путь к исходным данным(.txt)";
            pickPathToData.Click += DataDirectory_Click;
            pickPathToData.Left = 10;
            pickPathToData.Top = textBox3.Bottom;
            pickPathToData.Width = ClientSize.Width - 20;
            pickPathToData.Height = 20;
            Controls.Add(pickPathToData);

            label5 = new Label();
            label5.Text = "Формат входных данных:";
            label5.Top = pickPathToData.Bottom;
            label5.Left = 10;
            label5.Width = ClientSize.Width - 20;
            Controls.Add(label5);
            

            SourceDataSetterSelect = new ComboBox();
            SourceDataSetterSelect.Top = label5.Bottom;
            SourceDataSetterSelect.Left = 10;
            SourceDataSetterSelect.Width = ClientSize.Width - 20;
            SourceDataSetterSelect.Height = 20;
            Controls.Add(SourceDataSetterSelect);

            label6 = new Label();
            label6.Text = "Формат результатов:";
            label6.Top = SourceDataSetterSelect.Bottom;
            label6.Left = 10;
            label6.Width = ClientSize.Width - 20;
            Controls.Add(label6);

            ReportCreatorSelect = new ComboBox();
            ReportCreatorSelect.Left = 10;
            ReportCreatorSelect.Top = label6.Bottom;
            ReportCreatorSelect.Width = ClientSize.Width - 20;
            ReportCreatorSelect.Height = 20;
            Controls.Add(ReportCreatorSelect);

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
            //switch (ReportCreatorSelect.SelectedIndex)
            //{
            //    case 0:
            //        reportCreators[0].SetValues(setPathToLogs.SelectedPath);
             
            //        break;
            //    case 1:
            //        AddReportCreator(new ReportCreator());
            //        break;
            //    default:
            //        break;
            //

            sourceDataSetters[SourceDataSetterSelect.SelectedIndex].SetValues(new string[] { setPathToData.FileName, setPathToLogs.SelectedPath});
            reportCreators[ReportCreatorSelect.SelectedIndex].SetValues(setPathToResult.SelectedPath);
            //var textFileWorker = new TextFileWorker();

            //textFileWorker.SetValues(setPathToResult.SelectedPath);
            var queue = new Queue<OrderInfo>();
            sourceDataSetters[SourceDataSetterSelect.SelectedIndex].GetData((int)districtId.Value, date.Value.Date + time.Value.TimeOfDay, queue, reportCreators[ReportCreatorSelect.SelectedIndex].CreateReport);
            //sqlDBWorker.GetData((int)districtId.Value, date.Value.Date + time.Value.TimeOfDay, queue, textFileWorker.CreateReport);

        }

        public void CheckCreatorsAndSetters()
        {
            if (ReportCreatorSelect.SelectedIndex == -1 || SourceDataSetterSelect.SelectedIndex == -1)
            {
                MessageBox.Show("Не задана логика создания отчета и получения данных. Обратитесь к разработчикам");
                this.Close();
            }
                
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
        public void AddReportCreator(IReportCreator reportCreator)
        {
            var index = ReportCreatorSelect.Items.Add(reportCreator);
            reportCreators.Add(index, reportCreator); 
            if (ReportCreatorSelect.SelectedIndex == -1)
            {
                ReportCreatorSelect.SelectedIndex = 0;
            }
        }
        public void AddSourceDataSetter(ISourceDataSetter sourceDataSetter)
        {
            var index = SourceDataSetterSelect.Items.Add(sourceDataSetter);
            sourceDataSetters.Add(index, sourceDataSetter); 
            if (SourceDataSetterSelect.SelectedIndex == -1)
            {
                SourceDataSetterSelect.SelectedIndex = 0;
            }
        }
    }
}
