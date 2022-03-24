namespace BenchmarkEFCoreDapper
{
    public class Team
    {
        public Team()
        {
            Players = new List<Player>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime FoundingDate { get; set; }
        public int SportId { get; set; }
        public virtual ICollection<Player> Players { get; set; }
        public virtual Sport Sport { get; set; }
    }
}
