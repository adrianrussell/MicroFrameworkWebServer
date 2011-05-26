using System.IO;

namespace Server
{
    public class FileStreamFactory : IFileStreamFactory
    {
        public Stream Create(string filePath) {
            return new FileStream(filePath, FileMode.Open);
        }
    }
}
