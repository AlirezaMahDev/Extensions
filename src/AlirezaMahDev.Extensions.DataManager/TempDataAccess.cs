using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

class TempDataAccess()
    : DataAccess(System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.CreateVersion7().ToString())),
        ITempDataAccess
{
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            Directory.Delete(Path);
        }
    }
}