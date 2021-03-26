using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ReviewCheck.Models
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection") { }

        public DbSet<Review> Reviews { get; set; }
    }

    public class Review
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }

}