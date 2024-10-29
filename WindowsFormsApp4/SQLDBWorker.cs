using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Globalization;

namespace WindowsFormsApp4
{
    public class SQLDBWorker : ISourceDataSetter
    {
        private async Task<bool> CheckDistrictIdAsync(int districtId)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString))
            {
                SqlCommand cmd1 = new SqlCommand("SELECT DISTINCT DistrictId FROM Deliveries", connection);
                var DistrictIds = await cmd1.ExecuteReaderAsync();
                var flag = false;
                while (DistrictIds.Read())
                {
                    if ((int)districtId == DistrictIds.GetInt32(0))
                    {
                        flag = true;
                        break;
                    }
                }
                DistrictIds.Close();
                return flag;
            }
        }
        public async Task<int> GetData(int districtId, DateTime startTime, Queue<OrderInfo> orderInfos, Action<Queue<OrderInfo>> createReport)
        {
            int counter = 0;
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);
            connection.StateChange += connection_StateChange;
            connection.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Deliveries \r\n" +
                "WHERE DistrictId = @DstrId \r\n" +
                "and DeliveryTime>=@startTime \r\n" +
                "and DeliveryTime<= (SELECT DATEADD(minute, 30, DeliveryTime) " +
                "FROM (SELECT TOP 1 * FROM Deliveries WHERE DistrictId = @DstrId " +
                "and DeliveryTime>=@startTime Order by DeliveryTime) " +
                "as DeliveryTime1)"
                , connection);
            cmd.Parameters.AddWithValue("@startTime", startTime);
            cmd.Parameters.AddWithValue("@DstrId", districtId);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                orderInfos.Enqueue(new OrderInfo((int)reader[0], Convert.ToDouble(reader[1].ToString(), CultureInfo.InvariantCulture), (int)reader[2], (DateTime)reader[3]));
                counter++;
            }
            reader.Close();
            MessageBox.Show("Записано в очередь: " + counter + " записей");
            createReport.Invoke(orderInfos);
            connection.Close();
            return counter;

        }
        //public async Task CreateReport(Queue<OrderInfo> orderInfos)
        //{
        //    string fileName = string.Format("Result{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss"));
        //    Directory.CreateDirectory(TargetDirectory);
        //    string filePath = Path.Combine(TargetDirectory, fileName);
        //    var counter = 0;
        //    using (StreamWriter writer = new StreamWriter(filePath))
        //    {
        //        while (orderInfos.Count > 0)
        //        {
        //            var orderInfo = orderInfos.Dequeue();
        //            writer.WriteLine($"{orderInfo.OrderId}\t{orderInfo.Weight}\t{orderInfo.CityDistrictId}\t{orderInfo.DeliveryTime:yyyy-MM-dd HH:mm:ss}");
        //            counter++;
        //        }
        //    }
        //    MessageBox.Show("Прочитано из очереди: " + counter + " записей");
        //}
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
        public override string ToString()
        {
            return "База данных SQL";
        }

        public void SetValues(string[] values)
        {

        }
    }
}
