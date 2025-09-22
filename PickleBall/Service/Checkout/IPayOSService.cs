using Net.payOS.Types;
using PickleBall.Dto;

namespace PickleBall.Service.Checkout
{
    public interface IPayOSService
    {
        public Task<dynamic> CreatePaymentLink(List<ItemData> items,int amount);
        public Task<dynamic> ConfirmPayOSWebHook();
    }
}
