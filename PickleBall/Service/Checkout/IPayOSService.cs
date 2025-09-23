using Net.payOS.Types;
using PickleBall.Dto;

namespace PickleBall.Service.Checkout
{
    public interface IPayOSService
    {
        public Task<dynamic> CreatePaymentLink(List<ItemData> items,int amount, int orderCode);
        public Task<string> ConfirmPayOSWebHook();
        public Task<Result<string>> CallBack(HttpRequest request);
    }
}
