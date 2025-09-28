using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository.Users
{
    public interface IUserRepo
    {
        public IQueryable<Partner> Get();
        public Task<Partner?> GetById(Guid id);
        public void Update(Partner user);
        public void Delete(Partner user);
    }

    public class UserRepo : IUserRepo
    {   
        private readonly BookingContext _bookingContext;

        public UserRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public void Delete(Partner user)
        {
            _bookingContext.Users.Remove(user);
        }

        public IQueryable<Partner> Get()
        {
           return _bookingContext.Users.AsQueryable();
        }

        public async Task<Partner?> GetById(Guid id)
        {
            return await _bookingContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public void Update(Partner user)
        {
           _bookingContext.Users.Update(user);
        }
    }
}
