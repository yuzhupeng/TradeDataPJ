using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    public static class MongoPaginationService
    {
        /// <summary>
        /// 支持排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static IQueryable<T> BaseOrderPager<T>(this IOrderedQueryable<T> entitys, ref BasePaginationModel pagination)
        {
            if (pagination != null)
            {
                var result = entitys.GetBasePagination(pagination);
                return result;
            }
            return null;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="source"></param>
        ///// <param name="pagination"></param>
        ///// <returns></returns>
        //private static IQueryable<T> GetBasePagination<T>(this IOrderedQueryable<T> source, BasePaginationModel pagination)
        //{
        //    pagination.Total = source.Count();
        //    return source.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize);
        //}

        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="json"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static IQueryable<T> BasePager<T>(this IOrderedQueryable<T> entitys, ref BasePaginationModel pagination)
        {
            if (pagination != null)
            {
                var result = entitys.GetBasePagination(pagination);
                return result;
            }
            return null;
        }

        /// <summary>
        /// 获取分页后的数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源IQueryable</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        private static IQueryable<T> GetBasePagination<T>(this IOrderedQueryable<T> source, BasePaginationModel pagination)
        {
            pagination.Total = source.Count();
            return source.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize);
        }
    }

    public static class PaginationService
    {
        /// <summary>
        /// 分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entitys"></param>
        /// <param name="json"></param>
        /// <param name="pagination"></param>
        /// <returns></returns>
        public static IEnumerable<T> BasePager<T>(this IEnumerable<T> entitys, ref BasePaginationModel pagination)
        {
            if (pagination != null)
                entitys = entitys.GetBasePagination(pagination);
            return entitys;
        }

        /// <summary>
        /// 获取分页后的数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="source">数据源IQueryable</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        private static IEnumerable<T> GetBasePagination<T>(this IEnumerable<T> source, BasePaginationModel pagination)
        {
            pagination.Total = source.Count();
            return source.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize);
        }
    }



}
