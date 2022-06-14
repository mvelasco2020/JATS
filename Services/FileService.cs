using JATS.Services.Interfaces;

namespace JATS.Services
{
    public class FileService : IFileService
    {

        private readonly string[] sufficex = { "Bytes", "Kb", "MB", "GB", "TB", "PB" };
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);
                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] bytefile = memoryStream.ToArray();
                memoryStream.Close();
                memoryStream.Dispose();
                return bytefile;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal filesize = bytes;

            while (Math.Round(filesize / 1024) >= 1)
            {
                filesize /= bytes;
                counter++;
            }

            return string.Format("{0:n1}{1}", filesize, sufficex[counter]);
        }

        public string GetGileIcon(string file)
        {
            string fileImage = "/img/contenttype/default.png";

            if (!string.IsNullOrEmpty(file))
            {
                fileImage = Path.GetExtension(file).Replace(".", "");
                return $"/img/contenttype/{fileImage}.png";
            }

            return fileImage;
        }
    }
}
