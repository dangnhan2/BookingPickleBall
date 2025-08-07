using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using PickleBall.Dto.Request;
using PickleBall.Models;
using PickleBall.Models.Enum;
using PickleBall.UnitOfWork;

namespace PickleBall.Service.SoftService
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUnitOfWorks _unitOfWorks;

        public CheckoutService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }
        public async Task<object> Checkout(BookingRequest booking)
        {
            object result;
            Env.Load();

            var clientId = Env.GetString("PAYOS_CLIENT_ID");
            var apiKey = Env.GetString("PAYOS_API_KEY");
            var checkSumKey = Env.GetString("PAYOS_CHECKSUM_KEY");

            var payOs = new PayOS(clientId, apiKey, checkSumKey);

            var domain = Env.GetString("BASE_URL");

            var paymentLinkRequest = new PaymentData(
                orderCode: int.Parse(DateTimeOffset.Now.ToString("fffff")),
                amount : booking.Quantity * booking.Amount,
                description : GeneratePaymentContent(),
                items : [new(booking.Name, booking.Quantity, booking.Quantity * booking.Amount)],
                expiredAt: DateTimeOffset.UtcNow.AddMinutes(2).ToUnixTimeSeconds(),
                returnUrl: domain,
                cancelUrl : domain
            );

            var isExistCourt = await _unitOfWorks.Court.GetById(booking.CourtID) ?? throw new KeyNotFoundException("Sân không tồn tại");

            var newBooking = new Booking
            {
                ID = Guid.NewGuid(),
                UserID = booking.UserID,
                CourtID = booking.CourtID,
                BookingDate = booking.BookingDate,
                TotalAmount = booking.Quantity * booking.Amount,
                PaymentStatus = PaymentStatus.Unpaid,
                BookingStatus = BookingStatus.Pending,
                QRCodeUrl = "",
                BookingTimeSlots = new List<BookingTimeSlots>()
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

            var newPayment = new Payment
            {
                BookingID = newBooking.ID,
                MethodPayment = "QRCode",
                TransactionID = paymentLinkRequest.orderCode.ToString(),
                OrderCode = paymentLinkRequest.orderCode,
                PaidAmount = newBooking.TotalAmount,
                PaymentStatus = PaymentStatus.Unpaid
            };

            _unitOfWorks.Court.Update(isExistCourt);
            await _unitOfWorks.Booking.CreateAsync(newBooking);
            await _unitOfWorks.Payment.CreateAsync(newPayment);
            await _unitOfWorks.CompleteAsync();

            result = await payOs.createPaymentLink(paymentLinkRequest);

            return result;

        }

        public static string GeneratePaymentContent()
        {
            var prefix = "PKB"; // có thể thay đổi theo hệ thống bạn
            var timestamp = DateTime.Now.ToString("HHmmss"); // giờ phút giây để tăng độ ngẫu nhiên
            var random = new Random().Next(1000, 9999); // số ngẫu nhiên 4 chữ số
            return $"{prefix}{timestamp}{random}";
        }

    }
}
