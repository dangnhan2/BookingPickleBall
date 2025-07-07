namespace PickleBall.Extension
{
    public static class Extension
    {
        public static IQueryable<T> Paging<T>(this IQueryable<T> values, int page, int pageSize)
        {
            return values.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}
