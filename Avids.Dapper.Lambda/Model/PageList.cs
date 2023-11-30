namespace Avids.Dapper.Lambda.Model
{
    /// <summary>
    /// Pagination Entity Class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageList<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="totalCount">Total Data</param>
        /// <param name="items">Data</param>
        public PageList(int pageIndex, int pageSize, int totalCount, IEnumerable<T> items)
        {
            Total = totalCount;
            PageSize = pageSize;
            PageIndex = pageIndex;
            Items = items;
            TotalPage = Total % PageSize == 0 ? Total / PageSize : Total / PageSize + 1;
        }

        /// <summary>
        /// Total
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// Data
        /// </summary>
        public IEnumerable<T> Items { get; }

        /// <summary>
        /// Page Size
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Page Index
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Total Page
        /// </summary>
        public int TotalPage { get; }

        /// <summary>
        /// Has Previous Page
        /// </summary>
        public bool HasPrev => PageIndex > 1;

        /// <summary>
        /// Hax Next Page
        /// </summary>
        public bool HasNext => PageIndex < TotalPage;
    }
}
