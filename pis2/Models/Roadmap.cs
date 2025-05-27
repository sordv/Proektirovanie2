using System.Text;

namespace pis2.Models
{
    public class Roadmap
    {
        public List<RoadmapItem> Items { get; set; } = new List<RoadmapItem>();

        public void AddItem(RoadmapItem item)
        {
            Items.Add(item);
        }

        public string Render()
        {
            var roadmapBuilder = new StringBuilder();
            roadmapBuilder.AppendLine("Ваша дорожная карта");
            roadmapBuilder.AppendLine();

            foreach (var item in Items)
            {
                if (item.Period.HasValue)
                {
                    roadmapBuilder.AppendLine($"Выполнить до {DateTime.Now.AddDays(item.Period.Value):dd.MM.yyyy}");
                }
                roadmapBuilder.AppendLine(item.Value);
                roadmapBuilder.AppendLine();
            }

            return roadmapBuilder.ToString();
        }
    }
}