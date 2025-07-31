namespace PickleBall.Models.Enum
{
    public enum CourtStatus
    {
        Available = 0, //Sân hoạt động bình thường, có thể đặt
        UnderMaintenance = 1, //Sân đang sửa chữa, không thể đặt
        Inactive = 2, //Sân bị vô hiệu hoá (ngưng hoạt động, chưa mở bán)
        Full = 3 //Sân đã kín lịch trong ngày
    }
}
