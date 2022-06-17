using SmartWatts.Shared.DBModels;


namespace SmartWatts.Shared.APIParams
{
    public class ActivityParams
    {
        public User User { get; set; }
        public long? Before { get; set; }
        public long? After { get; set; }
        public int? Page { get; set; }
        public int? PerPage { get; set; }
    }
}
