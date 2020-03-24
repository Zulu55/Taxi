using System.IO;

namespace Taxi.Common.Helpers
{
    public interface IFilesHelper
    {
        byte[] ReadFully(Stream input);
    }
}
