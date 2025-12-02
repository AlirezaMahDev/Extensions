namespace Parto.Extensions.Abstractions;

public static class ObservableCollectionExtensions
{
    extension<T>(IList<T> collection)
    {
        public void AddOrUpdate(Func<T, bool> func, T instance)
        {
            collection.AddOrUpdate(func, () => instance, _ => instance);
        }

        public void AddOrUpdate(Func<T, bool> func, Func<T> addFunc, Action<T> updateAction)
        {
            collection.AddOrUpdate(func,
                addFunc,
                t =>
                {
                    updateAction(t);
                    return t;
                });
        }

        public void AddOrUpdate(Func<T, bool> func, Func<T> addFunc, Func<T, T> updateFunc)
        {
            var item = collection.FirstOrDefault(func);
            if (item is not null)
            {
                collection[collection.IndexOf(item)] = updateFunc(item);
            }
            else
            {
                collection.Add(addFunc());
            }
        }

        public void RemoveAll(Func<T, bool> func)
        {
            foreach (var item in collection.Where(func).ToArray())
            {
                collection.Remove(item);
            }
        }
    }
}