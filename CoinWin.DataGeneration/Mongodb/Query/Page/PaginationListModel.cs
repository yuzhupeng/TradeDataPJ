using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoinWin.DataGeneration
{
    /// <summary>
    /// 基础的分页类库
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PaginationListModel<T>
    {
        public PaginationListModel()
        {
            Data = new List<T>();
        }
        public List<T> Data { get; set; }
        public BasePaginationModel Pagination { get; set; }
    }

    public class PaginationModel
    {
        #region 构造函数

        public PaginationModel()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        #endregion
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; }

    }

    /// <summary>
    /// 基本分页实体类
    /// </summary>
    public class BasePaginationModel
    {
        #region 构造函数

        public BasePaginationModel(int page=1,int size=10)
        {
             
            PageNumber = page;
            PageSize = size;
        }

        #endregion

        #region 成员

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                int pages = Total / PageSize;
                int pageCount = Total % PageSize == 0 ? pages : pages + 1;
                return pageCount;
            }
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// 每页行数
        /// </summary>
        public int PageSize { get; set; }


        /// <summary>
        /// 总记录数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int Pages { get => PageCount; }

        /// <summary>
        /// 是否首页
        /// </summary>
        public bool IsFirstPage { get => PageNumber == 1; }

        /// <summary>
        /// 是否尾页
        /// </summary>
        public bool IsLastPage { get => PageNumber == Pages; }

        #endregion
    }
}
