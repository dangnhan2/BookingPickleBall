using PickleBall.Dto;

namespace PickleBall.Service.SoftService
{
    public interface IPayOsWebHookService
    {
        public Task<Result<string>> HanleWebHook(dynamic payload);
    }
}
