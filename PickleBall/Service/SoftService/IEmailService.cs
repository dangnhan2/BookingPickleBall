namespace PickleBall.Service.SoftService
{
    public interface IEmailService
    {
        public Task EmailSender(string email, string subject, string body);
    }
}
