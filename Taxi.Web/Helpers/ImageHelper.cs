using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Taxi.Web.Helpers
{
    public class ImageHelper : IImageHelper
    {
        public async Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.jpg";
            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                $"wwwroot\\images\\{folder}",
                file);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"~/images/{folder}/{file}";
        }

        public string UploadImage(byte[] pictureArray, string folder)
        {
            MemoryStream stream = new MemoryStream(pictureArray);
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.jpg";

            try
            {
                stream.Position = 0;
                string path = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\images\\{folder}", file);
                File.WriteAllBytes(path, stream.ToArray());
            }
            catch
            {
                return string.Empty;
            }

            return $"~/images/{folder}/{file}";
        }
    }
}
