namespace PersistenceNet.Extensions
{
    public static class CollectionExtension
    {
        public static ICollection<T>? ToCollection<T>(this IEnumerable<T>? collections)
        {
            ICollection<T> returnList = [];
            if (collections is not null && collections.Any())
                collections.ToList().ForEach(orig => returnList.Add(orig));

            return returnList;
        }
    }
}