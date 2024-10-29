using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4
{
    public interface ISourceDataSetter
    {
        void SetValues(string[] values);
        Task<int> GetData(int districtId, DateTime startTime, Queue<OrderInfo> orderInfos, Action<Queue<OrderInfo>> createReport);
    }
}
