namespace AlirezaMahDev.Extensions.Brain.Abstractions;

public static class NerveSleepExtensions
{
    extension<TData, TLink>(INerve<TData, TLink> nerve)
        where TData : unmanaged, ICellData<TData>
        where TLink : unmanaged, ICellLink<TLink>
    {
        public void Sleep()
        {
            throw new NotImplementedException();
        }

        public ValueTask SleepAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}