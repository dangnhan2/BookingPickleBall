namespace PickleBall.Service.Storage
{
    public interface ICronJobService
    {
        public Task CheckExpiredBoookings();
        public Task DeleteExpiredRefreshToken();

    }
}
