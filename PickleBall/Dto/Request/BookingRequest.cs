namespace PickleBall.Dto.Request
{
    public class BookingRequest
    {   
        public Guid CourtID { get; set; }
        public DateOnly BookingDate { get; set; }
        public Guid PartnerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int Amount { get; set; }
        public List<Guid> BookingTimeSlot { get; set; } = new List<Guid>();

    }
}
