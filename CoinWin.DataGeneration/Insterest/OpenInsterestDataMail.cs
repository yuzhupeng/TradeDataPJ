using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public class OpenInsterestDataMail : OpenInsterestData
    {
        public decimal pricePrecents
        {
            get
            {
                return Convert.ToDecimal(pricePrecent.Replace("%", ""));
            }

            set { }
        }

        public decimal SumOpenInterestPrecents
        {
            get
            {
                return Convert.ToDecimal(SumOpenInterestPrecent.Replace("%", ""));
            }

            set { }
        }

        public string types { get; set; }

        public string sumopen
        {
            get
            {
                if (SumOpenInterestValueEn > 0)
                {
                    return (Math.Round((this.SumOpenInterestValueEn / 10000),0)) + "W";
                }
                else
                {
                    return "0";
                }

            }

            set { }



        }
    }
}
