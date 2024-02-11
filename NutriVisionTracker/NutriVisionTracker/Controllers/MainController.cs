using NutriVisionTracker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace NutriVisionTracker.Controllers
{
    public class MainController : INotifyPropertyChanged
    {
        public DateTime dateFilter = DateTime.Today;
        private ICollectionView foodsInRange;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICollectionView FoodsInRange
        {
            get { return foodsInRange; }
            set
            {
                foodsInRange = value;
                OnPropertyChanged(nameof(FoodsInRange));
            }
        }

        public User? User { get; set; }
        
        DatabaseController DatabaseController { get; set; }
        GPTController GPTController { get; set; }


        public MainController()
        {
            DatabaseController = new DatabaseController();
            GPTController = new GPTController();
        }

        public bool LoginUser(string username, string password) 
        {
            User = DatabaseController.GetUser(username, password);

            if (User == null) return false;

            foodsInRange = CollectionViewSource.GetDefaultView(User.Foods);
            foodsInRange.Filter = FilterLogic;
            foodsInRange.SortDescriptions.Add(new SortDescription("DateTime", ListSortDirection.Ascending));

            return true;
        }

        public bool RegisterUser(User newUser) 
        {
            User? existingUser = DatabaseController.GetUser(newUser.Name, "all_passwords_work");

            // user with this name already exists
            if (existingUser != null) return false;

            DatabaseController.RegisterUser(newUser);
            return true;
        }

        public async Task<bool> ProcessFile(string imagePath)
        {
            if (User == null) return false;

            Food newFood = await GPTController.EstimateFoodFromPicture(imagePath);
            if (!newFood.IsValid())
                return false;
            
            DatabaseController.AddNewFood(newFood, User);
            return true;
        }

        public async Task UpgradeFood() 
        {
            var foods = User.Foods.Where(x => x.DateTime.Date.Equals(dateFilter)).ToList();
            await GPTController.UpgradeFood(User, foods);
            DatabaseController.SaveChanges();
        }

        internal bool FilterFoods(DateTime newDate)
        {
            int oldWeek = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(dateFilter, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            int newWeek = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(newDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            bool weekChanged = oldWeek != newWeek;

            dateFilter = newDate;
            FoodsInRange.Refresh();

            return weekChanged;
        }

        private bool FilterLogic(object item)
        {
            Food f = (Food)item;
            return dateFilter.Date.Equals(f.DateTime.Date);
        } 

        internal void SaveDBChanges()
        {
            DatabaseController.SaveChanges();
        }
    }
}
