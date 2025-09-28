using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Extension;
using PickleBall.Service.SignalR;
using PickleBall.UnitOfWork;

namespace PickleBall.Service.Bookings
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        
        public BookingService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;      
        }

        public async Task<DataReponse<BookingDto>> GetByPartner(Guid id, BookingParams bookingParams)
        {
            var bookings = _unitOfWorks.Booking.GetAllByPartner(id);

            if (bookingParams.Customer != null)
            {
                bookings = bookings.Where(c => c.CustomerName.ToLower().Trim().Contains(bookingParams.Customer.ToLower().Trim()));
            }

            if (bookingParams.BookingStatus.HasValue)
            {
                bookings = bookings.Where(c => c.BookingStatus == bookingParams.BookingStatus);
            }

            var bookingsToDto = bookings.OrderByDescending(b => b.CreatedAt).Select(b => new BookingDto
            {
                ID = b.ID,
                Customer = b.CustomerName,
                Phone = b.PhoneNumber,
                Court = b.Court.Name,
                BookingDate = b.BookingDate,
                BookingStatus = b.BookingStatus,
                TotalAmount = b.TotalAmount,
                CreatedAt = b.CreatedAt,
                BookingTimeSlots = b.BookingTimeSlots.Select(bts => new BookingTimeSlotDto
                {
                    Id = bts.CourtTimeSlotId,
                    StartTime = bts.CourtTimeSlots.TimeSlot.StartTime,
                    EndTime = bts.CourtTimeSlots.TimeSlot.EndTime
                }).ToList()
            }).Paging(bookingParams.Page, bookingParams.PageSize);

            return new DataReponse<BookingDto>
            {
                Page = bookingParams.Page,
                PageSize = bookingParams.PageSize,
                Total = bookings.Count(),
                Data = await bookingsToDto.ToListAsync()
            };
        }
    }
}
