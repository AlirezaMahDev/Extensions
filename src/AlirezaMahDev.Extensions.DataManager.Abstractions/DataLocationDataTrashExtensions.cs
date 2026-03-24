namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationDataTrashExtensions
{
    extension(ref readonly DataLocation<DataTrash> location)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Add<TDataLocation>(IDataAccess access, in TDataLocation dataLocation)
            where TDataLocation : IDataLocationBase<TDataLocation>
        {
            access.Create(new DataTrashItem { Offset = dataLocation.Offset }, out var item);
            var wrap = location.Wrap(access, x => x.Collection());
            wrap.Add(item);
        }
    }
}