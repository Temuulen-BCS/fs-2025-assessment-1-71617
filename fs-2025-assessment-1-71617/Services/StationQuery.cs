namespace fs_2025_assessment_1_71617.Services
{
    public class StationQueryOptions
    {
        public string? Status { get; set; }
        public string? Q { get; set; }
        public int? MinBikes { get; set; }
        public string? Sort { get; set; }
        public string? Dir { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;

        public string GetCacheKey()
        {
            return $"{Status}|{Q}|{MinBikes}|{Sort}|{Dir}|{Page}|{PageSize}";
        }
    }
}

