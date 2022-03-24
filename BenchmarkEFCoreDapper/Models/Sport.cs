namespace BenchmarkEFCoreDapper
{
    public class Sport
    {
        public Sport()
        {
            Teams = new List<Team>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Team> Teams { get; set; }
    }
}
