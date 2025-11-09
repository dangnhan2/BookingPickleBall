using Net.payOS.Types;
using PickleBall.Dto;

namespace PickleBall.Service.Checkout
{
    public interface IPayOSService
    {
        public Task<dynamic> CreatePaymentLink(List<ItemData> items,int amount, int orderCode, Guid partnerId);
        public Task<string> ConfirmPayOSWebHook(Guid partnerId);
        public Task<ApiResponse<string>> CallBack(HttpRequest request);
    }
}
