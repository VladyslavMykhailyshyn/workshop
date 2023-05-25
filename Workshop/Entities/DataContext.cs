namespace Workshop.Entities;

public class DataContext 
{
    protected readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder options)
    // {
    //     // connect to sql server database
    //     options.UseSqlServer(Configuration.GetConnectionString("WebApiDatabase"));
    // }

    public List<User> Users { get; set; } = new ();
}