using Microsoft.EntityFrameworkCore;
using NutriVisionTracker.Controllers;
using NutriVisionTracker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutriVisionTracker.Controllers
{
    public class DatabaseContext : DbContext 
    {
        static readonly string connectionString = "Server=127.0.0.1;Database=nutri_vision_tracker;User ID=root;Password=;";

        public DbSet<User> Users { get; set; }
        public DbSet<Food> Foods { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(u => u.ID);
            modelBuilder.Entity<Food>().HasKey(f => f.ID);

            modelBuilder.Entity<Food>()
                .HasOne(f => f.User)
                .WithMany(u => u.Foods)
                .HasForeignKey(f => f.UserID);
        }
    }

    public class DatabaseController
    {
        private DatabaseContext context;

        public DatabaseController()
        {
            context = new DatabaseContext();
        }

        public User? GetUser(string username, string password) 
        {
            //TODO: user password verification
            User user = context.Users.Include(u => u.Foods).Where(u => u.Name.Equals(username)).FirstOrDefault();
            return user;
        }


        internal void AddNewFood(Food newFood, User user)
        {
            newFood.SetUserAndDate(user.ID, DateTime.Now);
            context.Foods.Add(newFood);
            context.SaveChanges();
        }

        public void SaveChanges() 
        {
            context.SaveChanges();
        }

        internal void RegisterUser(User newUser)
        {
            context.Users.Add(newUser);
            context.SaveChanges();
        }
    }
}