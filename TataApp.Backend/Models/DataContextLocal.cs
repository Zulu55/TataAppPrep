using TataApp.Domain;

namespace TataApp.Backend.Models
{
    public class DataContextLocal : DataContext
    {
        public System.Data.Entity.DbSet<TataApp.Domain.Project> Projects { get; set; }

        public System.Data.Entity.DbSet<TataApp.Domain.Activity> Activities { get; set; }

        public System.Data.Entity.DbSet<TataApp.Domain.Time> Times { get; set; }
    }
}