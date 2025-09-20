
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
        public async Task<dynamic> CreatePaymentLink(List<ItemData> items, int amount)
        {
            Env.Load();
            PayOS payOS = new PayOS(
                Env.GetString("PAYOS_CLIENT_ID"), 
                Env.GetString("PAYOS_API_KEY"),
                Env.GetString("PAYOS_CHECKSUM_KEY"));

            PaymentData paymentData = new PaymentData(
                orderCode : int.Parse(DateTimeOffset.Now.ToString("ffffff")),
                amount : amount,
                description : "Thanh toan don hang",
                items : items,
                expiredAt : (int)DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds(),
                returnUrl: returnUrl,
                cancelUrl: cancelUrl
                );

            CreatePaymentResult createPayment = await payOS.createPaymentLink(paymentData);

            return createPayment;
        }
    }
}

//https://localhost:7279/api/WebHook
