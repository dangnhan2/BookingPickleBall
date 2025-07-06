using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;

namespace PickleBall.Repository
{
    public interface IUserRepo
    {
        public IQueryable<User> Get();
        public Task<User?> GetById(string id);
        public void Update(User user);
        public void Delete(User user);
    }

    public class UserRepo : IUserRepo
    {   
        private readonly BookingContext _bookingContext;

        public UserRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public void Delete(User user)
        {
            _bookingContext.Users.Remove(user);
        }

        public IQueryable<User> Get()
        {
           return _bookingContext.Users.AsQueryable();
        }

        public async Task<User?> GetById(string id)
        {
            return await _bookingContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public void Update(User user)
        {
           _bookingContext.Users.Update(user);
        }
    }
}
