namespace PickleBall.Service.SoftService
{
    public interface ICloudinaryService
    {
        public Task<string> Upload(IFormFile file,string[] allowedExtension, string folder);

        public Task Delete(string imageUrl);
    }
}
