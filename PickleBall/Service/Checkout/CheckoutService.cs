
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using PickleBall.Dto;
using PickleBall.Dto.Request;
using PickleBall.Models;
using PickleBall.Models.Enum;
using PickleBall.Service.BackgoundJob;
using PickleBall.Service.Checkout;
using PickleBall.Service.SignalR;
using PickleBall.UnitOfWork;
using PickleBall.Validation;

namespace PickleBall.Service.SoftService
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IPayOSService _payOSService;
        private readonly IHubContext<BookingHub> _hubContext;

        public CheckoutService(IUnitOfWorks unitOfWorks, IPayOSService payOSService, IHubContext<BookingHub> hubContext)
        {
            _unitOfWorks = unitOfWorks;
            _payOSService = payOSService;
            _hubContext = hubContext;
        }
        public async Task<Result<dynamic>> Checkout(BookingRequest booking)
        {
            dynamic result;
            var requestTimeSlots = booking.BookingTimeSlot;

            var conflictingsSlots = await _unitOfWorks.BookingTimeSlot.Get()
                .Where(bt => requestTimeSlots.Contains(bt.CourtTimeSlotId)
                &&
                (bt.Booking.BookingStatus == BookingStatus.Pending || bt.Booking.BookingStatus == BookingStatus.Paid)
                &&
                bt.Booking.ExpriedAt > DateTime.UtcNow).Select(bt => bt.CourtTimeSlotId).ToArrayAsync();

            if (conflictingsSlots.Any())
                return Result<dynamic>.Fail("Một hoặc nhiều slot đã được đặt. Vui lòng chọn slot khác.", StatusCodes.Status400BadRequest);

            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

            var newBooking = new Booking
            {
                ID = Guid.NewGuid(),
                CourtID = booking.CourtID,
                PartnerId = booking.PartnerId,
                BookingDate = booking.BookingDate,
                TransactionId = orderCode.ToString(),
                TotalAmount = booking.BookingTimeSlot.Count * booking.Amount,
                CustomerName = booking.CustomerName,
                PhoneNumber = booking.PhoneNumber,
                Email = booking.Email,
                BookingStatus = BookingStatus.Pending,
            };

            foreach (var slot in booking.BookingTimeSlot)
            {
                var timeSlot = new BookingTimeSlots
                {
                    Id = Guid.NewGuid(),
                    BookingId = newBooking.ID,
                    CourtTimeSlotId = slot
                };
                newBooking.BookingTimeSlots.Add(timeSlot);
            }

            foreach (var bt in newBooking.BookingTimeSlots)
            {
                var slot = bt.CourtTimeSlots;

                if (slot == null) continue;

                var payload = new SlotEventPayload
                {
                    CourtId = slot.CourtID,
                    BookingDate = booking.BookingDate,
                    TimeSlotId = slot.TimeSlotID,
                    StartTime = slot.TimeSlot.StartTime,
                    EndTime = slot.TimeSlot.EndTime,
                    Status = BookingStatus.Pending
                };

                await _hubContext.Clients.Group(booking.CourtID.ToString()).SendAsync("SlotReserved", payload);
            }



            _unitOfWorks.Booking.Create(newBooking);
            await _unitOfWorks.CompleteAsync();
      
            result = await _payOSService.CreatePaymentLink(
                [new ItemData("Pickleball Court Booking", booking.BookingTimeSlot.Count, newBooking.TotalAmount)],
                newBooking.TotalAmount, orderCode , booking.PartnerId         
            );

            return Result<dynamic>.Ok(result, StatusCodes.Status200OK);

        }

    }
}
