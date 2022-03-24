namespace BenchmarkEFCoreDapper
{
    public static class Constants
    {
        // Localhost database
        public static readonly string SportsConnectionString = @"Server=.\SQLEXPRESS;Database=EFCore6Dapper;Trusted_Connection=True;";

        // Docker Data Base
        //public static readonly string SportsConnectionString = @"Data Source=host.docker.internal,1433;Initial Catalog=master;User ID=sa;Password=Hpotter.1987";
    }
}