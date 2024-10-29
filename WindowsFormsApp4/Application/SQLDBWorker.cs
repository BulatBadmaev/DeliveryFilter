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
using NLog;
using NLog.Config;
using NLog.Targets;

namespace WindowsFormsApp4
{
    public class SQLDBWorker : ISourceDataSetter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
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
        public async Task<int> GetData(int districtId, DateTime startTime, Queue<OrderInfo> orderInfos, Func<Queue<OrderInfo>, int> createReport)
        {
            int counter = 0;
            SqlConnection connection = null; 
            try
            {
                connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);
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
                logger.Debug("SQL: " + cmd.CommandText + " стартовое время: " + districtId + " идентификатор района: " + startTime);
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@DstrId", districtId);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orderInfos.Enqueue(new OrderInfo((int)reader[0], Convert.ToDouble(reader[1].ToString()), (int)reader[2], (DateTime)reader[3]));
                        counter++;
                    }
                }
                logger.Debug("Соответствующих записей: " + counter);
                createReport.Invoke(orderInfos);
                return counter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                logger.Error(ex.Message);
                return 0;
            }
            finally
            {
                if (connection != null && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

        }
        static void connection_StateChange(object sender, StateChangeEventArgs e)
        {
            SqlConnection connection = sender as SqlConnection;
            logger.Trace("Connection State Changed. " + Environment.NewLine +
                "\t\t\tConnection to" + Environment.NewLine +
                "\t\t\tData Source: " + connection.DataSource + Environment.NewLine +
                "\t\t\tDatabase: " + connection.Database + Environment.NewLine +
                "\t\t\tState: " + connection.State);
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
