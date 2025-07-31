using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;
using System.Threading.Tasks;

namespace PickleBall.Repository
{
    public interface IPaymentRepo
    {
        public IQueryable<Payment> Get();
        public Task<Payment?> GetById(Guid id);
        public Task CreateAsync(Payment payment);
        public void Update(Payment payment);
        public void Delete(Payment payment);
    }

    public class PaymentRepo : IPaymentRepo
    {
        private readonly BookingContext _bookingContext;

        public PaymentRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public async Task CreateAsync(Payment payment)
        {
            await _bookingContext.Payments.AddAsync(payment);
        }

        public void Delete(Payment payment)
        {
            _bookingContext.Payments.Remove(payment);
        }

        public IQueryable<Payment> Get()
        {
           return _bookingContext.Payments.AsQueryable();
        }

        public async Task<Payment?> GetById(Guid id)
        {
            return await _bookingContext.Payments.FirstOrDefaultAsync(p => p.ID == id);
        }

        public void Update(Payment payment)
        {
             _bookingContext.Payments.Update(payment);
        }
    };
}
