using PickleBall.Models.Enum;

namespace PickleBall.Dto.QueryParams
{
    public class BookingParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Customer { get; set; }
        public BookingStatus? BookingStatus { get; set; }
    }
}
