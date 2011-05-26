using System.IO;

namespace Server
{
    public interface IFileStreamFactory
    {
        Stream Create(string filePath);
    }
}