using Microsoft.SqlServer.Server;
using NLog.Config;
using NLog.Targets;
using NLog;
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



namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        #region инициализация компонентов
        GroupBox groupBox1;
        GroupBox groupBox2;
        GroupBox groupBox3;
        Label label2;
        Label label3;
        Label label4;
        Label label5;
        Label label6;
        Label label7;
        Label label8;
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
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public Form1()
        {
            var offset1 = 30;
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new FileDialogPermission(PermissionState.Unrestricted));
            Text = "DeliveryApp pre-alpha release";
            this.Size = new Size(400, 600);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            setPathToResult = new FolderBrowserDialog();
            //setPathToResult.SelectedPath = "C:\\Users\\Булат\\Desktop";
            setPathToLogs = new FolderBrowserDialog();
            //setPathToLogs.SelectedPath = "C:\\Users\\Булат\\Desktop";
            setPathToData = new OpenFileDialog();
            //setPathToData.FileName = "D:\\input.txt";
         
            groupBox1 = new GroupBox();
            groupBox1.Text = "Введите параметры сортировки";
            groupBox1.Top = 10;
            groupBox1.Left = 10;
            groupBox1.Width = ClientSize.Width - 20;
            groupBox1.Height = 100;
            Controls.Add(groupBox1);

            label2 = new Label()
            {
                Text = "Район:",
                TextAlign = ContentAlignment.MiddleRight,
                Top = groupBox1.Top + 10,
                Left = offset1,
                Width = 50
            };
            districtId = new NumericUpDown()
            {
                Left = label2.Right,
                Top = groupBox1.Top + 10,
                Width = groupBox1.Width - label2.Width - offset1 - 10,
                Height = 20
            };
            label3 = new Label()
            {
                Text = "Дата:",
                TextAlign = ContentAlignment.MiddleRight,
                Top = label2.Bottom,
                Left = offset1,
                Width = 50
            };
            date = new DateTimePicker()
            {
                Top = label2.Bottom,
                Left = label3.Right,
                Width = groupBox1.Width - label2.Width - offset1 - 10,
                Height = 20
            };
            label4 = new Label()
            {
                Text = "Время:",
                TextAlign = ContentAlignment.MiddleRight,
                Top = label3.Bottom,
                Left = offset1,
                Width = 50,
            };
            time = new DateTimePicker()
            {
                Left = label4.Right,
                Top = label3.Bottom,
                Width = groupBox1.Width - label2.Width - offset1 - 10,
                Height = 20,
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true,
            };
            groupBox1.Controls.AddRange(new Control[] { label2, districtId, date, time, label3, label4 });

            groupBox2 = new GroupBox();
            groupBox2.Text = "Укажите соответствующие директории";
            groupBox2.Top = groupBox1.Bottom + 10;
            groupBox2.Left = 10;
            groupBox2.Width = ClientSize.Width - 20;
            groupBox2.Height = 130;


            label7 = new Label()
            {
                Text = "Результат выборки:",
                TextAlign = ContentAlignment.MiddleRight,
                Top =  20,
                Left = 10,
                Width = 120,
            };
            groupBox2.Controls.Add(label7);
            textBox1 = new TextBox();
            textBox1.ReadOnly = true;
            textBox1.Left = label7.Right;
            textBox1.Top = 20;
            textBox1.Width = groupBox2.Width - label7.Width -20;
            textBox1.Height = 20;
            groupBox2.Controls.Add(textBox1);
            Controls.Add(groupBox2);

            pickPathToResult = new Button();
            pickPathToResult.Text = "Задать путь сохранения результата выборки";
            pickPathToResult.Click += FileDirectory_Click;
            pickPathToResult.Left = 10;
            pickPathToResult.Top = label7.Bottom+1;
            pickPathToResult.Width = groupBox2.Width - 20;
            pickPathToResult.Height = 20;
            groupBox2.Controls.Add(pickPathToResult);
            label8 = new Label()
            {
                Text = "Логи:",
                TextAlign = ContentAlignment.MiddleRight,
                Top = pickPathToResult.Bottom + 10,
                Left = 10,
                Width = 120,
            };
            groupBox2.Controls.Add(label8);
            textBox2 = new TextBox();
            textBox2.ReadOnly = true;
            textBox2.Left = label8.Right;
            textBox2.Top = pickPathToResult.Bottom +10;
            textBox2.Width = groupBox2.Width - label8.Width - 20;
            textBox2.Height = 20;
            groupBox2.Controls.Add(textBox2);

            pickPathToLogs = new Button();
            pickPathToLogs.Text = "Задать путь сохранения логов";
            pickPathToLogs.Click += LogsDirectory_Click;
            pickPathToLogs.Left = 10;
            pickPathToLogs.Top = label8.Bottom+1;
            pickPathToLogs.Width = groupBox2.Width - 20;
            pickPathToLogs.Height = 20;
            groupBox2.Controls.Add(pickPathToLogs);

            groupBox3 = new GroupBox();
            groupBox3.Text = "Настройте режим работы";
            groupBox3.Top = groupBox2.Bottom + 10;
            groupBox3.Left = 10;
            groupBox3.Width = ClientSize.Width - 20;
            groupBox3.Height = 170;
            Controls.Add(groupBox3);

            label5 = new Label();
            label5.Text = "Формат входных данных:";
            label5.Top = 20;
            label5.Left = 10;
            label5.Width = groupBox3.Width - 20;
            groupBox3.Controls.Add(label5);


            SourceDataSetterSelect = new ComboBox();
            SourceDataSetterSelect.Top = label5.Bottom;
            SourceDataSetterSelect.Left = 10;
            SourceDataSetterSelect.Width = groupBox3.Width - 20;
            SourceDataSetterSelect.Height = 20;
            groupBox3.Controls.Add(SourceDataSetterSelect);

            label6 = new Label();
            label6.Text = "Формат результатов:";
            label6.Top = SourceDataSetterSelect.Bottom;
            label6.Left = 10;
            label6.Width = groupBox3.Width - 20;
            groupBox3.Controls.Add(label6);

            ReportCreatorSelect = new ComboBox();
            ReportCreatorSelect.Left = 10;
            ReportCreatorSelect.Top = label6.Bottom;
            ReportCreatorSelect.Width = groupBox3.Width - 20;
            ReportCreatorSelect.Height = 20;
            groupBox3.Controls.Add(ReportCreatorSelect);

            pickPathToData = new Button();
            pickPathToData.Text = "Указать путь к исходным данным(.txt)";
            pickPathToData.Click += DataDirectory_Click;
            pickPathToData.Left = 10;
            pickPathToData.Top = ReportCreatorSelect.Bottom+10;
            pickPathToData.Width = groupBox3.Width - 20;
            pickPathToData.Height = 20;
            groupBox3.Controls.Add(pickPathToData);

            apply = new Button();
            apply.Text = "Применить";
            apply.Click += Process;
            apply.Left = 10;
            apply.Top = ClientSize.Height - 50;
            apply.Width = ClientSize.Width - 20;
            apply.Height = 40;
            Controls.Add(apply);
            
        }
        #endregion
        private async void Process(object sender, EventArgs empty)
        {
            CheckInsertedParameters();
            sourceDataSetters[SourceDataSetterSelect.SelectedIndex].SetValues(new string[] { setPathToData.FileName, setPathToLogs.SelectedPath });
            reportCreators[ReportCreatorSelect.SelectedIndex].SetValues(setPathToResult.SelectedPath);
            var queue = new Queue<OrderInfo>();
            sourceDataSetters[SourceDataSetterSelect.SelectedIndex].GetData((int)districtId.Value, date.Value.Date + time.Value.TimeOfDay, queue, reportCreators[ReportCreatorSelect.SelectedIndex].CreateReport);
            logger.Info("Отчет сформирован");
            
        }

        public void CheckInsertedParameters()
        {

            if (ReportCreatorSelect.SelectedIndex == -1 || SourceDataSetterSelect.SelectedIndex == -1)
            {
                MessageBox.Show("Не задана логика создания отчета и (или) получения данных. Обратитесь к разработчикам");
                logger.Fatal("Не задана логика создания отчета и (или) получения данных");
                this.Close();
            }
            if (setPathToResult.SelectedPath == "")
            {
                MessageBox.Show("Укажите директорию сохранения результата выборки");
                return;
            }
            if (setPathToLogs.SelectedPath == "")
            {
                MessageBox.Show("Укажите директорию сохранения логов");
                return;
            }
            if (sourceDataSetters[SourceDataSetterSelect.SelectedIndex] is TextFileWorker && setPathToData.FileName == "")
            {
                MessageBox.Show("Укажите путь к исходным данным(.txt)");
                return;
            }
            if (date.Value.Date + time.Value.TimeOfDay > DateTime.Now)
            {
                MessageBox.Show("Дата должна быть меньше текущей");
                return;
            }
        }

        private void FileDirectory_Click(object sender, EventArgs e)
        {
            if (setPathToResult.ShowDialog() == DialogResult.OK)
                setPathToResult.RootFolder = Environment.SpecialFolder.Desktop;
            textBox1.Text = setPathToResult.SelectedPath;
            logger.Info("Указан путь к папке с результатами: " + setPathToResult.SelectedPath);
        }
        private void LogsDirectory_Click(object sender, EventArgs e)
        {
            if (setPathToLogs.ShowDialog() == DialogResult.OK)
            {
                setPathToLogs.RootFolder = Environment.SpecialFolder.Desktop;
                textBox2.Text = setPathToLogs.SelectedPath;
                string fileName = string.Format("log{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                Directory.CreateDirectory(setPathToLogs.SelectedPath);
                string filePath = Path.Combine(setPathToLogs.SelectedPath, fileName);
                SetDynamicLogFilePath(filePath);
                logger.Info("Указан путь к папке с логами: " + setPathToLogs.SelectedPath);
            }
        }
        private void DataDirectory_Click(object sender, EventArgs e)
        {
            setPathToData.Filter = "Текстовый файл. Файл формата  .txt|*.txt";
            if (setPathToData.ShowDialog() == DialogResult.OK)
            { 
                logger.Info("Указан путь к исходным данным: " + setPathToData.FileName);
            }
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
        private void SetDynamicLogFilePath(string newPath)
        {
            var config = LogManager.Configuration as LoggingConfiguration;
            if (config == null) return;

            var fileTarget = config.FindTargetByName<FileTarget>("file");
            if (fileTarget != null)
            {
                fileTarget.FileName = newPath;
                LogManager.Configuration = config;
            }
        }
    }
}
