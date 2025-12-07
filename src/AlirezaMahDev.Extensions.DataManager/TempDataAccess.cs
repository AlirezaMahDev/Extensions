using AlirezaMahDev.Extensions.DataManager.Abstractions;

namespace AlirezaMahDev.Extensions.DataManager;

class TempDataAccess() : DataAccess(System.IO.Path.GetTempFileName()), ITempDataAccess
{
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            File.Delete(Path);
        }
    }
}