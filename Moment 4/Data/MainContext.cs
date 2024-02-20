using Microsoft.EntityFrameworkCore;
using Moment_4.Models;

namespace Moment_4.Data;

    public class MainContext: DbContext
    {

    public MainContext(DbContextOptions<MainContext> options) : base (options) { }

    public DbSet<Category> Categories { get; set; } 

    public DbSet<Song> Songs { get; set; }
   
    }

