
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
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
        public async Task<dynamic> Checkout(BookingRequest booking)
        {
            dynamic result;
            var validator = new BookingRequestValidator();
            var response = await validator.ValidateAsync(booking);

            if (!response.IsValid)
            {
                foreach(var error in response.Errors)
                {
                    result = error.ErrorMessage;
                    break;
                }
            }      

            //var isExistCourt = await _unitOfWorks.Court.GetById(booking.CourtID) ?? throw new KeyNotFoundException("Sân không tồn tại");

            var newBooking = new Booking
            {
                ID = Guid.NewGuid(),
                UserID = booking.UserID,
                CourtID = booking.CourtID,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.TimeSlots.Count * booking.Amount,
                BookingStatus = BookingStatus.Pending,
            };

            foreach (var slot in booking.TimeSlots)
            {
                var timeSlot = new BookingTimeSlots
                {
                    Id = Guid.NewGuid(),
                    BookingId = newBooking.ID,
                    TimeSlotId = slot
                };
                newBooking.BookingTimeSlots.Add(timeSlot);
            }

            foreach (var bt in newBooking.BookingTimeSlots)
            {
                if (bt.TimeSlot == null) continue;
                var slot = bt.TimeSlot;

                var payload = new SlotEventPayload
                {
                    CourtId = booking.CourtID,
                    BookingDate = booking.BookingDate,
                    TimeSlotId = slot.ID,
                    StartTime = slot.StartTime,
                    EndTime = slot.EndTime,
                    Status = SlotStatus.Confirmed
                };

                await _hubContext.Clients.Group(newBooking.ID.ToString())
               .SendAsync("SlotReserved", payload);
            }

            //_unitOfWorks.Court.Update(isExistCourt);
            _unitOfWorks.Booking.Create(newBooking);
            await _unitOfWorks.CompleteAsync();
      
            result = await _payOSService.CreatePaymentLink(
                [new ItemData("Pickleball Court Booking", booking.TimeSlots.Count, newBooking.TotalAmount)],
                newBooking.TotalAmount            
            );

            return result;

        }

        //public static string GeneratePaymentContent()
        //{
        //    var prefix = "PKB"; // có thể thay đổi theo hệ thống bạn
        //    var timestamp = DateTime.Now.ToString("HHmmss"); // giờ phút giây để tăng độ ngẫu nhiên
        //    var random = new Random().Next(1000, 9999); // số ngẫu nhiên 4 chữ số
        //    return $"{prefix}{timestamp}{random}";
        //}

    }
}
