using PickleBall.Models;

namespace PickleBall.Repository
{
    public interface PaymentRepo
    {
        public IQueryable<Payment> Get();
        public Task<Payment> GetById(Guid id);
        public Task CreateAsync(Court court);
        public void Update(Court court);
        public void Delete(Court court);
    }
}
