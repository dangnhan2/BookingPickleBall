using PickleBall.Enum;

namespace PickleBall.QueryParams
{
    public class CourtParams
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string? Name { get; set; }
        public CourtStatus? Status { get; set; } 

    }
}
