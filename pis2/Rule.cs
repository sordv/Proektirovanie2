using System;

namespace pis2
{
    public class Rule
    {
        public int Id { get; set; }
        public int Target { get; set; }
        public int[] Citizenship { get; set; }
        public int[] Condition { get; set; }
        public int[] RoadmapItem { get; set; }
    }
}