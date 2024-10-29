using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp4
{
    internal class TextFileWorker : IReportCreator, ISourceDataSetter
    {
        string targetDirectory;
        string sourceFileDirectory;
        string sourceFileName;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public async Task<int> GetData(int districtId, DateTime startTime, Queue<OrderInfo> orderInfos, Func<Queue<OrderInfo>, int> createReport)
        {
            logger.Debug("Запущен метод GetData. Входные данные "+"идентификатор района: " + districtId + " старотое время: " + startTime);
            StreamReader reader = null;
            try
            {
                Directory.CreateDirectory(sourceFileDirectory);
                string filePath = Path.Combine(sourceFileDirectory, sourceFileName);
                reader = new StreamReader(filePath);
                var list = new List<OrderInfo>();
                string line;
                int id;
                double weight;
                int cityDistrictId;
                DateTime deliveryTime;
                DateTime  firstTimeDelivery = DateTime.MaxValue;
                int counter = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    counter++;
                    string[] values = line.Split('\t');
                    id = int.Parse(values[0]);
                    weight = double.Parse(values[1]);
                    cityDistrictId = int.Parse(values[2]);
                    deliveryTime = DateTime.ParseExact(values[3], "yyyy-MM-dd HH:mm:ss", null);
                    if (cityDistrictId == districtId && deliveryTime >= startTime)
                    {
                        if (deliveryTime < firstTimeDelivery)
                            firstTimeDelivery = deliveryTime;
                        if (deliveryTime <= deliveryTime.AddMinutes(30))
                            list.Add(new OrderInfo(id, weight, cityDistrictId, deliveryTime));
                    }

                }
                foreach (var item in list.Where(x => x.DeliveryTime <= firstTimeDelivery.AddMinutes(30)))
                {
                    orderInfos.Enqueue(item);
                }
                logger.Debug("Прочитано из файла: " + list.Count + " записей");
                if (list.Count > 0)
                {
                    createReport.Invoke(orderInfos);
                }
                return list.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Error(ex);
                return 0;
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
          
        }
        public int CreateReport(Queue<OrderInfo> orderInfos)
        {
            string fileName = string.Format("Result{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(targetDirectory);
            string filePath = Path.Combine(targetDirectory, fileName);
            var counter = 0;
            StreamWriter writer = null;
            try
            {
                writer = new StreamWriter(filePath);
                while (orderInfos.Count > 0)
                {
                    var orderInfo = orderInfos.Dequeue();
                    writer.WriteLine($"{orderInfo.OrderId}\t{orderInfo.Weight}\t{orderInfo.CityDistrictId}\t{orderInfo.DeliveryTime:yyyy-MM-dd HH:mm:ss}");
                    counter++;
                }
                MessageBox.Show("Отчет сформирован");
            }
            catch (Exception ex)
            { logger.Error(ex); }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
            logger.Debug("В файл: " + fileName +" записано " + counter + " записей");
            return counter;
        }
        public override string ToString()
        {
            return "Файл в формате .txt";
        }

        void IReportCreator.SetValues(string targetDirectory)
        {
            this.targetDirectory = targetDirectory;
        }

        void ISourceDataSetter.SetValues(string[] values)
        {
            sourceFileName = values[0];
            sourceFileDirectory = values[1];
        }
    }
}


