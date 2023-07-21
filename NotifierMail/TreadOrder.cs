using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifierMail
{
    public class TreadOrder
    {
        public string Type { get; set; }
        public DateTime OrderDateTimeUTC { get; set; }
        public double TakeProfits { get; set; }
    }
}
