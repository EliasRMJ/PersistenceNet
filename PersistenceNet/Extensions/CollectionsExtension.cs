using PersistenceNet.Interfaces;
using PersistenceNet.Views;

namespace PersistenceNet.Extensions
{
    public static class CollectionsExtension
    {
        public static ICollection<ViewBase> ConvertElementToView<T>(this ICollection<T> collections)
        {
            ICollection<ViewBase> returnList = [];
            if (collections.IsNotNull())
                collections.ToList().ForEach(c => returnList.Add(((IElement)c!).GetView()));

            return returnList;
        }

        public static IEnumerable<ViewBase> ConvertElementToView<T>(this IEnumerable<T> collections)
        {
            List<ViewBase> returnList = [];
            collections?.ToList().ForEach(c => returnList.Add(((IElement)c!).GetView()));

            return returnList.AsEnumerable();
        }

        public static IEnumerable<T> ConvertViewToElement<T>(this IEnumerable<ViewBase> collections)
        {
            List<IElement> returnList = [];
            collections?.ToList().ForEach(obj => returnList.Add(obj.ConvertTo(null) as IElement));

            return returnList.Cast<T>();
        }

        public static ICollection<T> ConvertViewToElementInCollection<T>(this IEnumerable<ViewBase> collections)
        {
            ICollection<IElement> returnList = [];
            collections?.ToList().ForEach(obj => returnList.Add(obj.ConvertTo(null) as IElement));

            return returnList.Cast<T>().ToList();
        }

        public static IEnumerable<T> ConvertViewToElement<T>(this List<ViewBase> collections)
        {
            List<IElement> returnList = [];
            if (collections.IsNotNull())
                collections.ToList().ForEach(obj => returnList.Add(obj.ConvertTo(null) as IElement));

            return returnList.Cast<T>();
        }

        public static ICollection<T> ConvertViewToElementInCollection<T>(this List<ViewBase> collections)
        {
            ICollection<IElement> returnList = [];
            if (collections.IsNotNull())
                collections.ToList().ForEach(obj => returnList.Add(obj.ConvertTo(null) as IElement));

            return returnList.Cast<T>().ToList();
        }

        public static bool IsNotNull<T>(this ICollection<T> collections)
        {
            return (collections is not null && collections.Count > 0);
        }

        public static bool IsNotNull<T>(this List<T> collections)
        {
            return (collections is not null && collections.Count > 0);
        }

        public static bool IsNullOrZero<T>(this ICollection<T> collections)
        {
            return (collections is null || collections.Count == 0);
        }

        public static bool IsNullOrZero<T>(this List<T> collections)
        {
            return (collections is null || collections.Count == 0);
        }

        public static void Clean<T>(this ICollection<T> collections)
        {
            if (collections.IsNotNull())
                collections.Clear();
        }

        public static void Clean<T>(this IEnumerable<T> collections)
        {
            collections?.ToList().Clear();
        }

        public static void Clean<T>(this List<T> collections)
        {
            if (collections.IsNotNull())
                collections.Clear();
        }
    }
}