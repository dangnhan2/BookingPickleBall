namespace PickleBall.Models.Enum
{
    public enum BookingStatus
    {    
        Free = 0,
        Pending = 1, //Ngay khi user gửi yêu cầu
        Paid = 2, // Sau khi thanh toán hoặc admin xác nhận
        Cancelled = 3, //Tự huỷ hoặc do hết thời gian
    }
}
