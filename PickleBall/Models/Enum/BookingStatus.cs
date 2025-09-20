namespace PickleBall.Models.Enum
{
    public enum BookingStatus
    {
        Pending, //Ngay khi user gửi yêu cầu
        Paid, // Sau khi thanh toán hoặc admin xác nhận
        Cancelled, //Tự huỷ hoặc do hết thời gian
    }
}
