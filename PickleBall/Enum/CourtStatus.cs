namespace PickleBall.Enum
{
    public enum CourtStatus
    {
        Available, //Sân hoạt động bình thường, có thể đặt
        UnderMaintenance, //Sân đang sửa chữa, không thể đặt
        Inactive, //Sân bị vô hiệu hoá (ngưng hoạt động, chưa mở bán)
        Full //Sân đã kín lịch trong ngày
    }
}
