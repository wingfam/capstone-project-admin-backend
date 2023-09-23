using DeliverBox_BE.Objects;
using System.Collections.ObjectModel;

namespace DeliverBox_BE.Models
{
    public class BusinessChart
    {
        public BusinessChart(string date, Collection<BusinessChartObject> orderPerDay) {
            this.date = date;
            this.BusinessPerDay = orderPerDay;
        }

        public string? date { get; set; }
        public virtual Collection<BusinessChartObject> BusinessPerDay { get; set; }
    }
}
