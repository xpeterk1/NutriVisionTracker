using Microsoft.Win32;
using NutriVisionTracker.Controllers;
using NutriVisionTracker.Dialogues;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NutriVisionTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainController Controller { get; set; }

        public MainWindow()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalExceptionUnhandledHandler);

            Controller = new MainController();

            var loginDialogue = new WelcomeDialogue();
            loginDialogue.RegisterUserRequested += Controller.RegisterUser;
            loginDialogue.LoginUserRequested += Controller.LoginUser;
            var res = loginDialogue.ShowDialog();

            if (!res.HasValue || !res.Value)
            {
                Close();
                return;
            }

            InitializeComponent();

            
            DataContext = this;

            UpdateGraphs(true);
        }

        private void GlobalExceptionUnhandledHandler(object sender, UnhandledExceptionEventArgs e)
        {
            new InfoBox("Error: Global Unhandled Exception Occured. App will now shut down.", InfoBox.Type.Failure).ShowDialog();
        }

        private void btnAddNewFood_Click(object sender, RoutedEventArgs e)
        {
            AddNewFile();
        }

        private async Task AddNewFile()
        {
            btnUpgrade.IsEnabled = false;
            btnAddNewFood.IsEnabled = false;

            var dialog = new System.Windows.Forms.OpenFileDialog();
            dialog.Filter = "PNG Images (*.png)|*.png|JPG Images (*.jpg)|*.jpg";
            var res = dialog.ShowDialog();

            if (res == System.Windows.Forms.DialogResult.OK && dialog.CheckFileExists)
            {
                string imagePath = dialog.FileName;
                bool result = await Controller.ProcessFile(imagePath);

                // parsed image did not contain a food, inform the user. nothing was saved in the DB. 
                if (!result) 
                {
                    new InfoBox("Zero Calorie Exception: No food detected!", InfoBox.Type.Failure).ShowDialog();
                    return;
                }

                UpdateGraphs();
            }

            btnUpgrade.IsEnabled = true;
            btnAddNewFood.IsEnabled = true;
        }


        private void DateChangedHandler(DateTime newDate)
        {
            bool weekChanged = Controller.FilterFoods(newDate);
            UpdateGraphs(weekChanged);
        }

        public void UpdateGraphs(bool weekChanged = false)
        {
            if (Controller.User == null) return;

            // daily totals
            var foodsToday = Controller.User.Foods.Where(f => f.DateTime.Date.Equals(Controller.dateFilter.Date));

            double calsToday = foodsToday.Sum(f => f.Calories);
            double proteinsToday = foodsToday.Sum(f => f.Proteins);
            double fatsToday = foodsToday.Sum(f => f.Fats);
            double carbsToday = foodsToday.Sum(f => f.Carbohydrates);
            double fibersToday = foodsToday.Sum(f => f.Fiber);

            chartCaloriesDay.Series[0].Values[0] = calsToday;
            chartCaloriesDay.Series[1].Values[0] = Math.Max(Math.Round(Controller.User.CaloriesGoal - calsToday), 0);
            dailyCaloriesStatus.Content = ConverToSeverity(calsToday, Controller.User.CaloriesGoal);

            chartProteinsDay.Series[0].Values[0] = proteinsToday;
            chartProteinsDay.Series[1].Values[0] = Math.Max(Math.Round(Controller.User.ProteinsGoal - proteinsToday), 0);
            dailyProteinsStatus.Content = ConverToSeverity(proteinsToday, Controller.User.ProteinsGoal);

            chartFatsDay.Series[0].Values[0] = fatsToday;
            chartFatsDay.Series[1].Values[0] = Math.Max(Math.Round(Controller.User.FatsGoal - fatsToday), 0);
            dailyFatsStatus.Content = ConverToSeverity(fatsToday, Controller.User.FatsGoal);

            chartCarbsDay.Series[0].Values[0] = carbsToday;
            chartCarbsDay.Series[1].Values[0] = Math.Max(Math.Round(Controller.User.CarbsGoal - carbsToday), 0);
            dailyCarbsStatus.Content = ConverToSeverity(carbsToday, Controller.User.CarbsGoal);

            chartFibersDay.Series[0].Values[0] = fibersToday;
            chartFibersDay.Series[1].Values[0] = Math.Max(Math.Round(Controller.User.FiberGoal - fibersToday), 0);
            dailyFibersStatus.Content = ConverToSeverity(fibersToday, Controller.User.FiberGoal);

            // weekly totals
            if (weekChanged) 
            {
                int week = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(Controller.dateFilter, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                var foodsWeekly = Controller.User.Foods.Where(f => CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(f.DateTime.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday).Equals(week));

                double calsWeek = foodsWeekly.Sum(f => f.Calories);
                double proteinsWeek = foodsWeekly.Sum(f => f.Proteins);
                double fatsWeek = foodsWeekly.Sum(f => f.Fats);
                double carbsWeek = foodsWeekly.Sum(f => f.Carbohydrates);
                double fibersWeek = foodsWeekly.Sum(f => f.Fiber);

                chartCaloriesWeek.Series[0].Values[0] = calsWeek;
                chartCaloriesWeek.Series[1].Values[0] = Math.Round(7 * Controller.User.CaloriesGoal - calsWeek);
                weeklyCaloriesStatus.Content = ConverToSeverity(calsWeek, 7 * Controller.User.CaloriesGoal);

                chartProteinsWeek.Series[0].Values[0] = proteinsWeek;
                chartProteinsWeek.Series[1].Values[0] = Math.Round(7 * Controller.User.ProteinsGoal - proteinsWeek);
                weeklyProteinsStatus.Content = ConverToSeverity(proteinsWeek, 7 * Controller.User.ProteinsGoal);

                chartFatsWeek.Series[0].Values[0] = fatsWeek;
                chartFatsWeek.Series[1].Values[0] = Math.Round(7 * Controller.User.FatsGoal - fatsWeek);
                weeklyFatsStatus.Content = ConverToSeverity(fatsWeek, 7 * Controller.User.FatsGoal);

                chartCarbsWeek.Series[0].Values[0] = carbsWeek;
                chartCarbsWeek.Series[1].Values[0] = Math.Round(7 * Controller.User.CarbsGoal - carbsWeek);
                weeklyCarbsStatus.Content = ConverToSeverity(carbsWeek, 7 * Controller.User.CarbsGoal);

                chartFibersWeek.Series[0].Values[0] = fibersWeek;
                chartFibersWeek.Series[1].Values[0] = Math.Round(7 * Controller.User.FiberGoal - fibersWeek);
                weeklyFibersStatus.Content = ConverToSeverity(fibersWeek, 7 * Controller.User.FiberGoal);
            }
        }

        private void btnUpgrade_Click(object sender, RoutedEventArgs e)
        {
            UpgradeFood();
        }

        private async void UpgradeFood()
        {
            btnUpgrade.IsEnabled = false;
            btnAddNewFood.IsEnabled = false;

            await Controller.UpgradeFood();
            Controller.FoodsInRange.Refresh();

            btnUpgrade.IsEnabled = true;
            btnAddNewFood.IsEnabled = true;
        }

        private string ConverToSeverity(double current, double target) 
        {
            double ratio = current / target;

            if (ratio < 0.7) return "Severely Below";
            else if (ratio < 0.9) return "Slightly Below";
            else if (ratio < 1.1) return "Normal";
            else if (ratio < 1.3) return "Slightly Above";

            return "Severely Above";
        }

        private void btnEditUser_Click(object sender, RoutedEventArgs e)
        {
            bool? res = new UserDialogue(Controller.User).ShowDialog();

            if (res.HasValue && res.Value) 
            {
                Controller.SaveDBChanges();
                UpdateGraphs();
            }
        }

        private void CloseWindowHandler(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeWindowHandler(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void TitleBarMouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }
}
