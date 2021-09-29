using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration 
{
    public class MarketData
    {
        public long id { get; set; }

        public string sdie { get; set; }

        public  string symbol { get; set; }


        public decimal sellcount { get; set; }
        public decimal buycount { get; set; }
        public decimal sellprice { get; set; }
        public decimal buyprice { get; set; }





        public decimal endbuycount { get; set; }

        public decimal endsellcount { get; set; }


        public decimal totalcount { get; set; }

        public decimal vol { get; set; }
        public string vols { get; set; }

        public DateTime starttime { get; set; }

        public DateTime endtime { get; set; }
    }
}
