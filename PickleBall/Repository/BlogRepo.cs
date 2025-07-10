using Microsoft.EntityFrameworkCore;
using PickleBall.Data;
using PickleBall.Models;
using System.Reflection.Metadata;

namespace PickleBall.Repository
{
    public interface IBlogRepo
    {
        public IQueryable<Blog> Get();
        public Task<Blog?> GetById(Guid id);
        public Task CreateAsync(Blog blog);
        public void Update(Blog blog);
        public void Delete(Blog blog);
    }

    public class BlogRepo : IBlogRepo
    {
        private readonly BookingContext _bookingContext;

        public BlogRepo(BookingContext bookingContext)
        {
            _bookingContext = bookingContext;
        }

        public async Task CreateAsync(Blog blog)
        {
            await _bookingContext.Blogs.AddAsync(blog);
        }

        public void Delete(Blog blog)
        {
            _bookingContext.Blogs.Remove(blog);
        }

        public IQueryable<Blog> Get()
        {
            return _bookingContext.Blogs.AsQueryable();
        }

        public async Task<Blog?> GetById(Guid id)
        {
            return await _bookingContext.Blogs.FirstOrDefaultAsync(c => c.ID == id);
        }

        public void Update (Blog blog)
        {
            _bookingContext.Blogs.Update(blog);
        }
    }
}
