
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PickleBall.Dto;
using PickleBall.UnitOfWork;

namespace PickleBall.Service.SoftService
{
    public class PayOsWebHookService : IPayOsWebHookService
    {
        private readonly IUnitOfWorks _unitOfWorks;

        public PayOsWebHookService(IUnitOfWorks unitOfWorks)
        {
            _unitOfWorks = unitOfWorks;
        }

        public async Task<Result<string>> HanleWebHook([FromBody] dynamic payload)
        {
            Console.Write(payload);
                string status = payload.status;
                long orderCode = payload.orderCode;

                var payment = await _unitOfWorks.Payment.GetByOrderCode(orderCode);

                if (payment == null)
                {
                    return Result<string>.Fail("Không tìm thấy thanh toán");
                }

                var booking = await _unitOfWorks.Booking.GetById(payment.BookingID);

                if (status == "PAID")
                {
                    payment.PaymentStatus = Models.Enum.PaymentStatus.Paid;
                    booking.BookingStatus = Models.Enum.BookingStatus.Confirmed;
                    booking.PaymentStatus = Models.Enum.PaymentStatus.Paid;

                    _unitOfWorks.Payment.Update(payment);
                    _unitOfWorks.Booking.Update(booking);
                }

                await _unitOfWorks.CompleteAsync();

                return Result<string>.Ok("Thanh toán thành công");
           
        }

            
    }
}

//https://localhost:7279/api/WebHook
