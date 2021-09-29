using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
   public class paramhelper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewid"></param>
        /// <param name="onetimetoken"></param>
        /// <returns></returns>
        public static List<KeyValuePair<String, String>> getwjxParma()
        {
            List<KeyValuePair<String, String>> paramList = new List<KeyValuePair<String, String>>();
            paramList.Add(new KeyValuePair<string, string>("__EVENTTARGET", ""));
            paramList.Add(new KeyValuePair<string, string>("__EVENTARGUMENT", ""));
            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATE", "/wEPDwUKLTE3ODE3MTU3OQ8WAh4TVmFsaWRhdGVSZXF1ZXN0TW9kZQIBFgICAg9kFgRmDw8WAh4EVGV4dAUe5Lic6I6e55+z6b6Z5Lqs55O35pyJ6ZmQ5YWs5Y+4ZGQCDA9kFgICAQ8WAh4EaHJlZgUTaHR0cHM6Ly93d3cud2p4LmNuL2RkrE9Li8ZIUt6vygAfgKiEq2vP4oo="));


            paramList.Add(new KeyValuePair<string, string>("__VIEWSTATEGENERATOR", "C16E7785"));
            paramList.Add(new KeyValuePair<string, string>("__EVENTVALIDATION", "/wEdAAXgO2nDle0VsM+PXLdLpTQgR1LBKX1P1xh290RQyTesRRHS0B3lkDg8wcTlzQR027xcCvP9YJ59uTpXZUTciHnZ3Kmo+ouB2mmE3Mg9Mibsbko47Z09Lx7JFXvHe7kgoJk0KZVf"));


            paramList.Add(new KeyValuePair<string, string>("UserName", "kyocera.sl"));
            paramList.Add(new KeyValuePair<string, string>("hfUserName", ""));
            paramList.Add(new KeyValuePair<string, string>("Password", "ivG4GW5rlgnup7iHy/4TEq47bJ8NccKRyZZ0xiwZ+Lq0Gx9ajOdfgdpJdHcnGZ8suqHLAIZOqFYCIjIOw1vsUg=="));
            paramList.Add(new KeyValuePair<string, string>("LoginButton", "登 录"));

            return paramList;

        }

    }
}
