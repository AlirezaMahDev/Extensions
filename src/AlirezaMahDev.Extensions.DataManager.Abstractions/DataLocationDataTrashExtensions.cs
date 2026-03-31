namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationDataTrashExtensions
{
    extension(in DataLocation<DataTrash> location)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public void Add(IDataAccess access, in DataOffset offset)
        {
            access.Create(new() { Offset = offset }, out DataLocation<DataTrashItem> item);
            var wrap = location.Wrap(access, x => x.Collection());
            wrap.Add(in item);
        }
    }
}