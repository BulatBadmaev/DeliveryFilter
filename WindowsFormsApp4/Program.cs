using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var MyForm = new Form1();
            var textFileWorker = new TextFileWorker();
            var sqlDBWorker = new SQLDBWorker();
            MyForm.AddReportCreator(textFileWorker);
            MyForm.AddSourceDataSetter(textFileWorker);
            MyForm.AddSourceDataSetter(sqlDBWorker);
            Application.Run(MyForm);
        }
    }
}
