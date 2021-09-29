using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{

   public class TimeCore
    {

        public static List<DateTime> GetAllDateMin(DateTime dt)
        {
            List<DateTime> listMin = new List<DateTime>();
            List<DateTime> listHour = new List<DateTime>();

            listHour = GetHourList(dt);

            foreach (var item in listHour.OrderBy(p=>p.Hour))
            {
                var starttime = new DateTime(item.Year, item.Month, item.Day, item.Hour, 00, 00);

                var overhour= starttime.AddHours(1);

               // var starttime = new DateTime(item.Year, item.Month, item.Day, item.Hour, 00,00);

                for (; starttime.Ticks < overhour.Ticks; starttime = starttime.AddMinutes(1))
                {
                    listMin.Add(starttime);
                }



            }
            return listMin;
        
        }


        /// <summary>
        /// 根据日期获取整日小时数据
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<DateTime> GetHourList(DateTime date)
        {

            List<DateTime> datelist = new List<DateTime>();

            DateTime start = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);

            DateTime end = new DateTime(date.Year, date.Month, date.Day, 23, 00, 00);

            for (; start.Ticks < end.Ticks; start = start.AddHours(1))
            {
                datelist.Add(start);
            }
            datelist.Add(end);

            return datelist;
        }


         /// <summary>
         /// 根据开始和结束获取所以分钟
         /// </summary>
         /// <param name="start"></param>
         /// <param name="end"></param>
         /// <returns></returns>
        public static List<DateTime> GetAllMinByDate(DateTime start,DateTime end)
        {
            List<DateTime> listMin = new List<DateTime>();
            List<DateTime> listHour = new List<DateTime>();

            //listHour = GetStartAndEndTime(start,end);

            //foreach (var item in listHour.OrderBy(p => p.Hour))
            //{

                var starttime = start;
                //var overhour = starttime.AddHours(1);
                // var starttime = new DateTime(item.Year, item.Month, item.Day, item.Hour, 00,00);
                for (; starttime.Ticks <= end.Ticks; starttime = starttime.AddMinutes(1))
                {
                    listMin.Add(starttime);
                }
            //}
            return listMin;

        }


        /// <summary>
        /// 获取开始到结束之间的小时
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public static List<DateTime> GetStartAndEndTime(DateTime start, DateTime end)
        {
            List<DateTime> datelist = new List<DateTime>();

            start = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);

            if (start.Day == end.Day && start.Hour == end.Hour&& start.Minute == end.Minute)
            {
                return null;
            }


            for (; start <= end; start = start.AddHours(1))
            {
                datelist.Add(start);
            }

            return datelist;



        }


        /// <summary>
        /// 获取开始到结束之间的小时
        /// </summary>
        /// <param name="start">开始时间</param>
        /// <param name="end">结束时间</param>
        /// <returns></returns>
        public static List<DateTime> GetStartAndEndTimeContent(DateTime start, DateTime end)
        {
            List<DateTime> datelist = new List<DateTime>();

            start = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);

            if (start.Day == end.Day && start.Hour == end.Hour)
            {
                datelist.Add(start);
                return datelist;
            }


            for (; start <= end; start = start.AddHours(1))
            {
                datelist.Add(start);
            }

            return datelist;



        }


        ///根据开始和结束获取之间的日期
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<DateTime> GetDate(DateTime start, DateTime end)
        {
            var selectedDates = new List<DateTime>();
            try
            {

                for (; start <= end; start = start.AddDays(1))
                {
                    selectedDates.Add(start);
                }


                var dates = selectedDates.Where(p => p.Year == end.Year && p.Month == end.Month && p.Day == end.Day).FirstOrDefault();

                if (dates == new DateTime(01, 01, 01))
                {
                    selectedDates.Add(end);
                }

                return selectedDates;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
