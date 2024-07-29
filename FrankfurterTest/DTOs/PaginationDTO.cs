
namespace FrankfurterTest.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        public int recordsByPage { get; set; } = 5;
        public readonly int maxRecords = 10;

        public int RecordsByPage
        {
            get { return recordsByPage; }
            set { recordsByPage = (value > maxRecords) ? maxRecords : value; }
        }
    }
}
