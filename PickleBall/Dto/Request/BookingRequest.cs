namespace PickleBall.Dto.Request
{
    public class BookingRequest
    {   
        public string UserID { get; set; }
        public Guid CourtID { get; set; }
        public DateOnly BookingDate { get; set; }
        public string CustomerName { get; set; }
        //public int Quantity { get; set; }
        public int Amount { get; set; }
        public List<Guid> TimeSlots { get; set; } = new List<Guid>();

    }
}
