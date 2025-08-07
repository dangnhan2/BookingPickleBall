using Microsoft.EntityFrameworkCore;
using PickleBall.Dto;
using PickleBall.Dto.QueryParams;
using PickleBall.Extension;
using PickleBall.UnitOfWork;

namespace PickleBall.Service
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        public BookingService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task<DataReponse<BookingDto>> Get(BookingParams bookingParams)
        {
            var bookings = _unitOfWorks.Booking.Get();

            if (bookingParams.Customer != null)
            {
                bookings = bookings.Where(c => c.User.FullName.ToLower().Trim().Contains(bookingParams.Customer.ToLower().Trim()));
            }

            if (bookingParams.BookingStatus.HasValue)
            {
                bookings = bookings.Where(c => c.BookingStatus == bookingParams.BookingStatus);
            }

            var bookingsToDto = bookings.OrderByDescending(b => b.CreatedAt).Select(b => new BookingDto
            {
                ID = b.ID,
                Customer = b.User.FullName,
                Phone = b.User.PhoneNumber,
                Court = b.Court.Name,
                BookingDate = b.BookingDate,
                BookingStatus = b.BookingStatus,
                PaymentStatus = b.PaymentStatus,
                PaymentMethod = b.Payments.MethodPayment,
                TotalAmount = b.TotalAmount,
                CreatedAt = b.CreatedAt,
                TimeSlots = b.BookingTimeSlots.Select(s => new TimeSlotDto
                {
                    ID = s.Id,
                    StartTime = s.TimeSlot.StartTime,
                    EndTime = s.TimeSlot.EndTime
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
