namespace PickleBall.Service.Email
{
    public interface IEmailService
    {
        public Task EmailSender(string email, string subject, string body);
    }
}
