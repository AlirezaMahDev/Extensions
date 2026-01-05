namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationDataTrashExtensions
{
    extension(DataLocation<DataTrash> location)
    {
        public void Add<TDataLocation>(IDataAccess access, TDataLocation dataLocation)
            where TDataLocation : IDataLocationBase<TDataLocation>
        {
            location
                .Wrap(access, x => x.Collection())
                .Add(access.Create(new DataTrashItem
                {
                    Offset = dataLocation.Offset
                }));
        }

        public async ValueTask AddAsync<TDataLocation>(IDataAccess access,
            TDataLocation dataLocation,
            CancellationToken cancellationToken = default)
            where TDataLocation : IDataLocationBase<TDataLocation>
        {
            await location
                .Wrap(access, x => x.Collection())
                .AddAsync(access.Create(new DataTrashItem
                    {
                        Offset = dataLocation.Offset
                    }),
                    cancellationToken);
        }
    }
}