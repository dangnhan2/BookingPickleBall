namespace PickleBall.Service.Storage
{
    public interface ICloudinaryService
    {
        public Task<string> Upload(IFormFile file,string[] allowedExtension, string folder);

        public Task Delete(string imageUrl);
    }
}
