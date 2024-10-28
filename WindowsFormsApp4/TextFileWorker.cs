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
    internal class TextFileWorker
    {
        string TargetDirectory { get; set; }
        public void SetValues(string targetDirectory)
        {
            TargetDirectory = targetDirectory;
        }

        public async Task<int> GetData(int districtId, DateTime startTime, Queue<OrderInfo> orderInfos, Func<Queue<OrderInfo>, Task> createReport)
        {
            string directory = @"D:\";
            string fileName = "input.txt";

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(directory);
            // Create the file
            string filePath = Path.Combine(directory, fileName);
            using (StreamReader reader = new StreamReader(filePath))
            {
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
                    weight = double.Parse(values[1], CultureInfo.InvariantCulture);
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
                MessageBox.Show("Прочитано из файла: " + counter + " строк");
                foreach (var item in list.Where(x => x.DeliveryTime <= firstTimeDelivery.AddMinutes(30)))
                {
                    orderInfos.Enqueue(item);
                }
                MessageBox.Show("Прочитано из файла: " + list.Count + " записей");
                if (list.Count > 0)
                {
                    await createReport.Invoke(orderInfos);
                }
                return list.Count;
            }
          
        }
        public async Task CreateReport(Queue<OrderInfo> orderInfos)
        {
            string fileName = string.Format("Result{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
            Directory.CreateDirectory(TargetDirectory);
            string filePath = Path.Combine(TargetDirectory, fileName);
            var counter = 0;
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                while (orderInfos.Count > 0)
                {
                    var orderInfo = orderInfos.Dequeue();
                    writer.WriteLine($"{orderInfo.OrderId}\t{orderInfo.Weight}\t{orderInfo.CityDistrictId}\t{orderInfo.DeliveryTime:yyyy-MM-dd HH:mm:ss}");
                    counter++;
                }
            }
            MessageBox.Show("Прочитано из очереди: " + counter + " записей");
        }
    }
}


