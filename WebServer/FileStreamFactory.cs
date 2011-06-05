using System.IO;

namespace Server
{
    public class FileStreamFactory : IStreamFactory
    {
        public Stream Create(string filePath) {
            return new FileStream(filePath, FileMode.Open);
        }
    }
}
