using BenchmarkDotNet.Running;
using BenchmarkEFCoreDapper;

Main();

void Main() 
{
    Console.WriteLine("Choose an option from the following list:");
    Console.WriteLine("\t1 - BenckmarkDotNet");
    Console.WriteLine("\t2 - Measure Times");
    Console.WriteLine("\t3 - Create Data");
    Console.Write("Your option? ");

    switch (Console.ReadLine())
    {
        case "1":
            RunBenckmark();
            break;
        case "2":
            Console.Write("Num of iterations? ");
            int iterations;
            int.TryParse(Console.ReadLine(), out iterations);
            RunMeasure(iterations);
            break;
        case "3":
            CreateData();
            break;
        default:
            Console.WriteLine("Unrecognized Option");
            Main();
            break;
    }
}

void RunBenckmark() 
{
    BenchmarkRunner.Run<BenchmarkEfvsDapperRunner>();
}

void RunMeasure(int iterations) 
{
    MeasureRunner runner = new();
    Random rnd = new();

    if (iterations <= 0)
        iterations++;

    Console.WriteLine("----GetPlayerById_EF----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetPlayerById(rnd.Next(1, 101));
    }
    Console.WriteLine();
    Console.WriteLine("----GetPlayerById_EF_NoTracking----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetPlayerById_NoTracking(rnd.Next(1, 101));
    }
    Console.WriteLine();
    Console.WriteLine("----GetPlayerById_Dapper----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetPlayerById_Dapper(rnd.Next(1, 101));
    }
    Console.WriteLine();
    Console.WriteLine("----GetPlayersByTeamId_EF----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetPlayersByTeamId(rnd.Next(1, 101));
    }
    Console.WriteLine();
    Console.WriteLine("----GetPlayersByTeamId_EF_NoTracking----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetPlayersByTeamId_NoTracking(rnd.Next(1, 101));
    }
    Console.WriteLine();
    Console.WriteLine("----GetPlayersByTeamId_Dapper----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetPlayersByTeamId_Dapper(rnd.Next(1, 101));
    }
    Console.WriteLine();
    Console.WriteLine("----GetTeamPlayersForSport_EF----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetTeamPlayersForSport(rnd.Next(1, 101));
    }
    Console.WriteLine();
    Console.WriteLine("----GetTeamPlayersForSport_EF_NoTracking----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetTeamPlayersForSport_NoTracking(rnd.Next(1, 101));
    }
    Console.WriteLine();
    Console.WriteLine("----GetTeamPlayersForSport_Dapper----");
    for (int i = 1; i <= iterations; i++)
    {
        runner.GetTeamPlayersForSport_Dapper(rnd.Next(1, 101));
    }
}

void CreateData()
{
    int NumSports = 1000;
    int NumTeams = 100;
    int NumPlayers = 10;

    List<Sport> sports = Generator.GenerateSports(NumSports);
    List<Team> teams = new List<Team>();
    List<Player> players = new List<Player>();

    foreach (var sport in sports)
    {

        var newTeams = Generator.GenerateTeams(sport.Id, NumTeams);
        teams.AddRange(newTeams);
        foreach (var team in newTeams)
        {
            var newPlayers = Generator.GeneratePlayers(team.Id, NumPlayers);
            players.AddRange(newPlayers);
        }
    }

    DataBase.Reset();
    DataBase.Load(sports, teams, players);
}