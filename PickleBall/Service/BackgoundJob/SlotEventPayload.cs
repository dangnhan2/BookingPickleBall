using PickleBall.Models.Enum;

namespace PickleBall.Service.BackgoundJob
{
    public class SlotEventPayload
    {
        public Guid CourtId { get; set; }          // Sân nào
        public DateOnly BookingDate { get; set; }  // Ngày cụ thể
        public Guid TimeSlotId { get; set; }       // ID khung giờ
        public TimeOnly StartTime { get; set; }    // Giờ bắt đầu
        public TimeOnly EndTime { get; set; }      // Giờ kết thúc
        public BookingStatus Status { get; set; }
    }
}
