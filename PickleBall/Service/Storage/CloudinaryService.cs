
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DotNetEnv;

namespace PickleBall.Service.Storage
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        public CloudinaryService()
        {
            Env.Load();
            var account = new Account
            {
                Cloud = Env.GetString("CLOUD_NAME"),
                ApiKey = Env.GetString("API_KEY"),
                ApiSecret = Env.GetString("API_SECRET")
            };
            
            _cloudinary = new Cloudinary(account);
        }
        public async Task Delete(string imageUrl)
        {
            var publicId = ExtractPublicIdFromUrl(imageUrl);
            if (publicId == null)
            {
                throw new InvalidOperationException($"Lỗi khi trích xuất publicId từ URL: {imageUrl}");
            }
            if (!string.IsNullOrEmpty(publicId))
            {
                var deleteParams = new DeletionParams(publicId);
                await _cloudinary.DestroyAsync(deleteParams);
            }
        }

        public async Task<string> Upload(IFormFile file, string[] allowedExtension, string folder)
        {   
                var fileExtension = Path.GetExtension(file.FileName);

                if (!allowedExtension.Contains(fileExtension))
                {
                    throw new ArgumentException($"Hãy upload các file có đuôi {string.Join(" ,", allowedExtension)}");
                }

                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = folder
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                Console.WriteLine(result);
                return result.SecureUrl.ToString();
        }

        public string ExtractPublicIdFromUrl(string imageUrl)
        {
            var uri = new Uri(imageUrl);
            var path = uri.AbsolutePath; // /dvdv4id16/image/upload/v1749660746/pho_hk86qj.jpg

            // Tách phần sau "upload/"
            var parts = path.Split("/upload/");
            if (parts.Length < 2)
                throw new ArgumentException("File không hợp lệ");

            // Lấy phần sau upload/, loại bỏ version
            var pathAfterUpload = parts[1]; // v1749660746/pho_hk86qj.jpg
            var segments = pathAfterUpload.Split('/').ToList();

            if (segments[0].StartsWith("v") && segments[0].Length > 1)
            {
                segments.RemoveAt(0); // bỏ "v1749660746"
            }

            var fullPath = string.Join("/", segments); // "pho_hk86qj.jpg" hoặc "folder/abc.jpg"
            var publicId = Path.ChangeExtension(fullPath, null); // remove .jpg

            return publicId;
        }
    }
}
