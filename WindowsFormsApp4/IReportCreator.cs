using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp4
{
    public interface IReportCreator
    {
        void SetValues(string targetDirectory);
        
        void CreateReport(Queue<OrderInfo> orderInfos);
    }
}
