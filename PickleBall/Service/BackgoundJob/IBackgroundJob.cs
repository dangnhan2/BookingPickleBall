namespace PickleBall.Service.BackgoundJob
{
    public interface IBackgroundJob
    {
        public Task CheckAndReleaseExpiredBookings();
        public Task DeleteExpiredRefreshToken();

    }
}
