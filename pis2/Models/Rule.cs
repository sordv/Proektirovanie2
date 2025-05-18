namespace pis2.Models
{
    public class Rule
    {
        public int Id { get; set; }
        public int[] Citizenship { get; set; }
        public int[] Flag { get; set; }
        public int[] BannedCitizenships { get; set; }
        public int[] BannedFlags { get; set; }
        public int RoadmapItem { get; set; }
        public int? Period { get; set; }
    }
}
