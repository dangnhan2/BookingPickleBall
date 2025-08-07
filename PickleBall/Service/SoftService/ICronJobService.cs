namespace PickleBall.Service.SoftService
{
    public interface ICronJobService
    {
        public Task CheckExpiredBoookings();
        public Task DeleteExpiredRefreshToken();

    }
}
