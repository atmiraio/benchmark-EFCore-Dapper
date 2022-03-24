namespace BenchmarkEFCoreDapper
{
    public static class Generator
    {
        public static List<Player> GeneratePlayers(int teamId, int count)
        {
            List<Player> players = new List<Player>();

            var allFirstNames = Names.GetFirstNames();
            var allLastNames = Names.GetLastNames();
            Random rand = new Random();
            DateTime start = new DateTime(1975, 1, 1);
            DateTime end = new DateTime(2001, 1, 1);

            for (int i = 0; i < count; i++)
            {
                Player player = new Player();
                int newFirst = rand.Next(0, allFirstNames.Count - 1);
                player.FirstName = allFirstNames[newFirst];
                int newLast = rand.Next(0, allLastNames.Count - 1);
                player.LastName = allLastNames[newLast];
                player.DateOfBirth = RandomDay(rand, start, end);
                player.TeamId = teamId;
                player.Id = (((teamId - 1) * count) + (i + 1));
                players.Add(player);
            }

            return players;
        }

        public static List<Team> GenerateTeams(int sportId, int count)
        {
            List<Team> teams = new List<Team>();

            var allCityNames = Names.GetCityNames();
            var allTeamNames = Names.GetTeamNames();
            Random rand = new Random();
            DateTime start = new DateTime(1900, 1, 1);
            DateTime end = new DateTime(2016, 1, 1);

            for (int i = 0; i < count; i++)
            {
                Team team = new Team();
                int newCity = rand.Next(0, allCityNames.Count - 1);
                int newTeam = rand.Next(0, allTeamNames.Count - 1);
                team.Name = allCityNames[newCity] + " " + allTeamNames[newTeam];
                team.FoundingDate = RandomDay(rand, start, end);
                team.SportId = sportId;
                team.Id = (((sportId - 1) * count) + (i + 1));
                teams.Add(team);
            }

            return teams;
        }

        public static List<Sport> GenerateSports(int count)
        {
            List<Sport> sports = new List<Sport>();
            var allSportNames = Names.GetSportNames();
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                int newSport = rand.Next(0, allSportNames.Count - 1);
                sports.Add(new Sport()
                {
                    Name = allSportNames[newSport],
                    Id = i + 1
                });
            }

            return sports;
        }

        private static DateTime RandomDay(Random rand, DateTime start, DateTime end)
        {
            int range = (end - start).Days;
            return start.AddDays(rand.Next(range));
        }
    }
}

