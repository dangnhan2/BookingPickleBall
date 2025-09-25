
using DotNetEnv;
using Microsoft.AspNetCore.SignalR;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json.Linq;
using PickleBall.Dto;
using PickleBall.Models.Enum;
using PickleBall.Service.BackgoundJob;
using PickleBall.Service.SignalR;
using PickleBall.UnitOfWork;
using System.Security.Cryptography;
using System.Text;


namespace PickleBall.Service.Checkout
{
    public class PayOSService : IPayOSService
    {
        private readonly IUnitOfWorks _unitOfWorks;
        private const string returnUrl = "https://pickleboom.vercel.app/payment/success";
        private const string cancelUrl = "Cancel";
        private readonly PayOS _payOs;
        private readonly string _checksumKey;
        private readonly IHubContext<BookingHub> _hubContext;

        public PayOSService(IUnitOfWorks unitOfWorks, IHubContext<BookingHub> hubContext)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            Env.Load($".env.{env.ToLower()}");

            _payOs = new PayOS(
                Env.GetString("PAYOS_CLIENT_ID"),
                Env.GetString("PAYOS_API_KEY"),
                Env.GetString("PAYOS_CHECKSUM_KEY"));

            _unitOfWorks = unitOfWorks;
            _checksumKey = Env.GetString("PAYOS_CHECKSUM_KEY");
            _hubContext = hubContext;
        }

        public async Task<dynamic> CreatePaymentLink(List<ItemData> items, int amount, int orderCode)
        {  
            PaymentData paymentData = new PaymentData(
                orderCode : orderCode,
                amount : amount,
                description : "Thanh toan don hang",
                items : items,
                expiredAt : (int)DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds(),
                returnUrl: returnUrl,
                cancelUrl: cancelUrl
                );

            CreatePaymentResult createPayment = await _payOs.createPaymentLink(paymentData);

            return createPayment;
        }

        public async Task<string> ConfirmPayOSWebHook()
        {
            var result = await _payOs.confirmWebhook("https://bookingpickleball.onrender.com/api/WebHook");
            return result;
        }

        public async Task<Result<string>> CallBack(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            var rawJson = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(rawJson))
                return Result<string>.Fail("Empty body", StatusCodes.Status400BadRequest);

            Console.WriteLine("Webhook raw: " + rawJson);

            // Parse JSON
            var root = JObject.Parse(rawJson);
            var signatureProvided = root["signature"]?.ToString();
            var data = root["data"] as JObject;

            if (string.IsNullOrEmpty(signatureProvided) || data == null)
                return Result<string>.Fail("Invalid payload", StatusCodes.Status400BadRequest);

            // Build transactionStr = key=value&key2=value2...
            var sorted = data.Properties().OrderBy(p => p.Name, StringComparer.Ordinal).ToList();
            var sb = new StringBuilder();
            for (int i = 0; i < sorted.Count; i++)
            {
                var prop = sorted[i];
                sb.Append(prop.Name).Append('=').Append(prop.Value.ToString());
                if (i < sorted.Count - 1) sb.Append('&');
            }
            var transactionStr = sb.ToString();

            // Compute HMAC SHA256
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(transactionStr));
            var signatureComputed = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            if (!string.Equals(signatureProvided, signatureComputed, StringComparison.OrdinalIgnoreCase))
            {
                return Result<string>.Fail("Invalid signature", StatusCodes.Status401Unauthorized);
            }

            // ✅ Nếu hợp lệ → xử lý booking
            var orderCode = data["orderCode"]?.ToObject<long>();
            if (orderCode == null)
                return Result<string>.Fail("Missing orderCode", StatusCodes.Status400BadRequest);

            var booking = await _unitOfWorks.Booking.GetByOrderCode(orderCode.Value.ToString());
            if (booking == null)
                return Result<string>.Fail("Không tìm thấy booking", StatusCodes.Status404NotFound);

            if (booking.BookingStatus == BookingStatus.Pending)
            {
                booking.BookingStatus = BookingStatus.Paid;
                await _unitOfWorks.CompleteAsync();

                // Bắn SignalR để client update
                foreach (var bt in booking.BookingTimeSlots)
                {
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
                    await _hubContext.Clients.Group(booking.CourtID.ToString())
                        .SendAsync("SlotConfirmed", payload);
                }
            }

            return Result<string>.Ok("Webhook processed successfully", StatusCodes.Status200OK);
        }


    }
}

