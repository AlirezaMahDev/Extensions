using AlirezaMahDev.Extensions.File.Data.Abstractions;
using AlirezaMahDev.Extensions.File.Data.Collection.Abstractions;

namespace AlirezaMahDev.Extensions.File.Data.Collection;

internal class CollectionObject : CollectionObjectBase, ICollectionObject
{
    public CollectionObject(CollectionObjectArgs args, CollectionPropertiesFactory collectionPropertiesFactory)
        : base(args.Index.ToPathBinary()
            .Aggregate(args.Objects.LocationValue.Location,
                (location, string64) =>
                    location.GetOrAdd(string64))
        )
    {
        Index = args.Index;
        Properties = collectionPropertiesFactory.GetOrCreate(this);
    }

    public virtual int Index { get; }
    public override ICollectionProperties Properties { get; }
}