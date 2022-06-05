namespace JATS.Services.Interfaces
{
    public interface IFileService
    {

        public Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file);

        public string ConvertByteArrayToFile(byte[] fileData, string extension);

        public string GetGileIcon(string file);

        public string FormatFileSize(long bytes);

    }
}
