using Microsoft.EntityFrameworkCore;

namespace BenchmarkEFCoreDapper
{
    public class DataBase
    {
        public static void Reset()
        {
            using (SportContextEFCore context = new SportContextEFCore(GetOptions()))
            {
                context.Database.ExecuteSqlRaw("DELETE FROM Players");
                context.Database.ExecuteSqlRaw("DELETE FROM Teams");
                context.Database.ExecuteSqlRaw("DELETE FROM Sports");

                Console.WriteLine("The database has been deleted!");
            }
        }

        public static void Load(List<Sport> sports, List<Team> teams, List<Player> players)
        {
            AddSports(sports);
            AddTeams(teams);
            AddPlayers(players);
        }

        private static void AddPlayers(List<Player> players)
        {
            using (SportContextEFCore context = new SportContextEFCore(GetOptions()))
            {
                foreach (var player in players)
                {
                    context.Players.Add(new Player()
                    {
                        Id = player.Id,
                        FirstName = player.FirstName,
                        LastName = player.LastName,
                        DateOfBirth = player.DateOfBirth,
                        TeamId = player.TeamId
                    });
                }

                context.SaveChanges();
                Console.WriteLine($"Added {players.Count} players");
            }
        }

        private static void AddTeams(List<Team> teams)
        {
            using (SportContextEFCore context = new SportContextEFCore(GetOptions()))
            {
                foreach (var team in teams)
                {
                    context.Teams.Add(new Team()
                    {
                        Id = team.Id,
                        Name = team.Name,
                        SportId = team.SportId,
                        FoundingDate = team.FoundingDate
                    });
                }

                context.SaveChanges();
                Console.WriteLine($"Added {teams.Count} teams");
            }
        }

        private static void AddSports(List<Sport> sports)
        {
            using (SportContextEFCore context = new SportContextEFCore(GetOptions()))
            {
                foreach (var sport in sports)
                {
                    context.Sports.Add(new Sport()
                    {
                        Id = sport.Id,
                        Name = sport.Name
                    });
                }

                context.SaveChanges();
                Console.WriteLine($"Added {sports.Count} sports");
            }
        }

        public static DbContextOptions GetOptions()
        {
            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();
            builder.UseSqlServer(Constants.SportsConnectionString);

            return builder.Options;
        }
    }
}
