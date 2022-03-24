using BenchmarkDotNet.Attributes;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace BenchmarkEFCoreDapper
{
    [AllStatisticsColumn]
    public class BenchmarkEfvsDapperRunner
    {
        private IDbContextFactory<SportContextEFCore> _poolingFactory;
        private Random rnd = new();

        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddPooledDbContextFactory<SportContextEFCore>(options => options
                .UseSqlServer(Constants.SportsConnectionString)
                .EnableThreadSafetyChecks(false));
            var serviceProvider = services.BuildServiceProvider();
            _poolingFactory = serviceProvider.GetRequiredService<IDbContextFactory<SportContextEFCore>>();
        }

        [Benchmark]
        public void GetPlayerById()
        {
            int id = rnd.Next(1, 101);
            using var context = _poolingFactory.CreateDbContext();

            var result = context.Players.FirstOrDefault(p => p.Id == id);
        }

        [Benchmark]
        public void GetPlayerById_NoTracking()
        {
            int id = rnd.Next(1, 101);
            using var context = _poolingFactory.CreateDbContext();

            var result = context.Players.AsNoTracking().FirstOrDefault(p => p.Id == id);
        }

        [Benchmark]
        public void GetPlayerById_Dapper()
        {
            int id = rnd.Next(1, 101);
            using (SqlConnection conn = new SqlConnection(Constants.SportsConnectionString))
            {
                var result = conn.QuerySingle<Player>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Players WHERE Id = @ID", new { ID = rnd.Next(1, 1000) });
            }
        }

        [Benchmark]
        public void GetPlayersByTeamId()
        {
            int id = rnd.Next(1, 101);
            using var context = _poolingFactory.CreateDbContext();

            var result = context.Teams.Include(x => x.Players).Single(x => x.Id == id);
        }

        [Benchmark]
        public void GetPlayersByTeamId_NoTracking()
        {
            int id = rnd.Next(1, 101);
            using var context = _poolingFactory.CreateDbContext();

            var result = context.Teams.Include(x => x.Players).AsNoTracking().Single(x => x.Id == id);
        }

        [Benchmark]
        public void GetPlayersByTeamId_Dapper()
        {
            Team result = null;
            using (var conn = new SqlConnection(Constants.SportsConnectionString))
            {
                conn.Open();
                var dictionary = new Dictionary<int, Team>();

                string sql = @"SELECT t.Id, t.Name, t.SportId, t.FoundingDate, p.Id, p.FirstName, p.LastName, p.DateOfBirth, p.TeamId 
                                FROM Teams t
                                INNER JOIN Players p on p.TeamId = t.Id
                                WHERE TeamId = @ID";
                var list = conn.Query<Team, Player, Team>(
                sql: sql,
                map: (team, player) =>
                {
                    if (!dictionary.TryGetValue(team.Id, out result))
                    {
                        result = team;
                        result.Players = new List<Player>();
                        dictionary.Add(team.Id, result);
                    }

                    result.Players.Add(player);
                    return result;
                },
                splitOn: "Id",
                param: new { ID = rnd.Next(1, 101) });
            }
        }

        [Benchmark]
        public void GetTeamPlayersForSport()
        {
            int id = rnd.Next(1, 101);
            using var context = _poolingFactory.CreateDbContext();

            var result = context.Teams.Include(x => x.Players).Where(x => x.SportId == id).ToList();
        }

        [Benchmark]
        public void GetTeamPlayersForSport_NoTracking()
        {
            int id = rnd.Next(1, 101);
            using var context = _poolingFactory.CreateDbContext();

            var result = context.Teams.Include(x => x.Players).AsNoTracking().Where(x => x.SportId == id).ToList();
        }

        [Benchmark]
        public void GetTeamPlayersForSport_Dapper()
        {
            int id = rnd.Next(1, 101);
            using (SqlConnection conn = new SqlConnection(Constants.SportsConnectionString))
            {
                var teams = conn.Query<Team>("SELECT Id, Name, SportId, FoundingDate FROM Teams WHERE SportId = @ID", new { ID = id });

                var teamIDs = teams.Select(x => x.Id).ToList();

                var players = conn.Query<Player>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Players WHERE TeamId IN @IDs", new { IDs = teamIDs });

                foreach (var team in teams)
                {
                    team.Players = players.Where(x => x.TeamId == team.Id).ToList();
                }
            }
        }
    }
}
