using System.IO;

namespace Server
{
    public interface IStreamFactory
    {
        Stream Create(string filePath);
    }
}