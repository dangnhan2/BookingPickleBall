namespace PickleBall.Models.Enum
{
    public enum BookingStatus
    {
        Pending, //Ngay khi user gửi yêu cầu
        Confirmed, // Sau khi thanh toán hoặc admin xác nhận
        Cancelled, //Tự huỷ hoặc do hết thời gian
        CheckedIn, // Sau khi admin xác nhận check-in
        Completed, // Sau thời gian đặt hoặc admin đánh dấu
        NoShow, // Quá giờ nhưng chưa check-in
        Rejected, //Có thể do không hợp lệ, spam
    }
}
