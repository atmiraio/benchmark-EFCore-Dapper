using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Diagnostics;

namespace BenchmarkEFCoreDapper
{
    public class MeasureRunner
    {
        private readonly IDbContextFactory<SportContextEFCore> _poolingFactory;

        public MeasureRunner()
        {
            var services = new ServiceCollection();
            services.AddPooledDbContextFactory<SportContextEFCore>(options => options
                .UseSqlServer(Constants.SportsConnectionString)
                .EnableThreadSafetyChecks(false));
            var serviceProvider = services.BuildServiceProvider();
            _poolingFactory = serviceProvider.GetRequiredService<IDbContextFactory<SportContextEFCore>>();
        }

        public void GetPlayerById(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            using var context = _poolingFactory.CreateDbContext();

            var result = context.Players.FirstOrDefault(p => p.Id == id);
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        public void GetPlayerById_NoTracking(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            using var context = _poolingFactory.CreateDbContext();

            var result = context.Players.AsNoTracking().FirstOrDefault(p => p.Id == id);
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }


        public void GetPlayerById_Dapper(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SqlConnection conn = new SqlConnection(Constants.SportsConnectionString))
            {
                var result = conn.QuerySingle<Player>("SELECT Id, FirstName, LastName, DateOfBirth, TeamId FROM Players WHERE Id = @ID", new { ID = id });
            }
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        public void GetPlayersByTeamId(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            using var context = _poolingFactory.CreateDbContext();

            var result = context.Teams.Include(x => x.Players).Single(x => x.Id == id);
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        public void GetPlayersByTeamId_NoTracking(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            using var context = _poolingFactory.CreateDbContext();

            var result = context.Teams.Include(x => x.Players).AsNoTracking().Single(x => x.Id == id);
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        public void GetPlayersByTeamId_Dapper(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
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
                param: new { ID = id });
            }
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        public void GetTeamPlayersForSport(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            using var context = _poolingFactory.CreateDbContext();

            var result = context.Teams.Include(x => x.Players).Where(x => x.SportId == id).ToList();
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        public void GetTeamPlayersForSport_NoTracking(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            using var context = _poolingFactory.CreateDbContext();

            var result = context.Teams.Include(x => x.Players).AsNoTracking().Where(x => x.SportId == id).ToList();
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }

        public void GetTeamPlayersForSport_Dapper(int id)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
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
            watch.Stop();
            Console.WriteLine((double)watch.ElapsedTicks / Stopwatch.Frequency * 1000);
        }
    }
}
