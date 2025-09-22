
using DotNetEnv;
using Net.payOS;
using Net.payOS.Types;
using PickleBall.Dto;

namespace PickleBall.Service.Checkout
{
    public class PayOSService : IPayOSService
    {

        private const string returnUrl = "https://localhost:5173/payment/success";
        private const string cancelUrl = "Cancel";
        private readonly PayOS _payOs;

        public PayOSService()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            Env.Load($".env.{env.ToLower()}");

            _payOs = new PayOS(
                Env.GetString("PAYOS_CLIENT_ID"),
                Env.GetString("PAYOS_API_KEY"),
                Env.GetString("PAYOS_CHECKSUM_KEY"));
        }

        public async Task<dynamic> CreatePaymentLink(List<ItemData> items, int amount)
        {  
            PaymentData paymentData = new PaymentData(
                orderCode : int.Parse(DateTimeOffset.Now.ToString("ffffff")),
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

        public async Task<dynamic> ConfirmPayOSWebHook()
        {
            var result = await _payOs.confirmWebhook("https://bookingpickleball.onrender.com/api/WebHook");

            //Console.WriteLine(result);

            return result;
        }

    }
}

//https://localhost:7279/api/WebHook
