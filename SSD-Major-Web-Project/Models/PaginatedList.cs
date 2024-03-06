namespace SSD_Major_Web_Project.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items
                            , int count
                            , int pageIndex
                            , int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        }

        public bool HasNextPage
        {
            get { return (PageIndex < TotalPages); }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source
                                                              , int pageIndex
                                                              , int pageSize)
        {
            var count = await Task.FromResult(source.Count());
            var items = await Task.FromResult(source.Skip((pageIndex - 1) * pageSize)
                                                    .Take(pageSize).ToList());

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }

}
