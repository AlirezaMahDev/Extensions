namespace AlirezaMahDev.Extensions.DataManager.Abstractions;

public static class DataLocationDataTrashExtensions
{
    extension(DataLocation<DataTrash> location)
    {
        public void Add<TDataLocation>(TDataLocation dataLocation)
            where TDataLocation : IDataLocationBase<TDataLocation>
        {
            dataLocation.Access.GetTrash()
                .Wrap(x => x.Collection())
                .Add(x => x.RefValue = x.RefValue with
                {
                    Offset = dataLocation.Offset,
                    Length = dataLocation.Length
                });
        }

        public async ValueTask AddAsync<TDataLocation>(TDataLocation dataLocation,
            CancellationToken cancellationToken = default)
            where TDataLocation : IDataLocationBase<TDataLocation>
        {
            await (await dataLocation.Access.GetTrashAsync(cancellationToken))
                .Wrap(x => x.Collection())
                .AddAsync(x => x.RefValue = x.RefValue with
                    {
                        Offset = dataLocation.Offset,
                        Length = dataLocation.Length
                    },
                    cancellationToken: cancellationToken);
        }
    }
}