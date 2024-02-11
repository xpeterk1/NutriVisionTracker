using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace NutriVisionTracker.Model
{
    public enum Gender { Male = 0, Female = 1 }

    public enum Activity { Sedentary, LightlyActive, ModeratelyActive, VeryActive, ExtremelyActive }

    public class User : INotifyPropertyChanged
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        [NotMapped]
        private string _name;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Required]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        public int Height { get; set; }

        [Required]
        public int Weight { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public Activity Activity { get; set; }

        [NotMapped]
        public virtual ObservableCollection<Food> Foods { get; set; } = new ObservableCollection<Food>();

        public double BMR { 
            get 
            {
                return 10 * Weight + 6.25 * Height - 5 * Age + (Gender.Equals(Gender.Male) ? 5 : -161);
            } 
        }

        public double CaloriesGoal 
        {
            get 
            {
                double coef = 0;
                switch (Activity) 
                {
                    case Activity.Sedentary:
                        coef = 1.2; break;
                    case Activity.LightlyActive:
                        coef = 1.375; break;
                    case Activity.ModeratelyActive:
                        coef = 1.55; break;
                    case Activity.VeryActive:
                        coef = 1.725; break;
                    case Activity.ExtremelyActive:
                        coef = 1.9; break;
                }

                return coef * BMR;
            }
        }

        public double ProteinsGoal 
        {
            get
            {
                double coef = 0;
                switch (Activity)
                {
                    case Activity.Sedentary:
                        coef = 17.5; break;
                    case Activity.LightlyActive:
                        coef = 17.5; break;
                    case Activity.ModeratelyActive:
                        coef = 22.5; break;
                    case Activity.VeryActive:
                        coef = 22.5; break;
                    case Activity.ExtremelyActive:
                        coef = 27.5; break;
                }

                return CaloriesGoal / 4 * coef / 100;
            }
        }

        public double FatsGoal
        {
            get
            {
                return CaloriesGoal / 4 * 27.5 / 100;
            }
        }

        public double CarbsGoal
        {
            get
            {
                return CaloriesGoal / 4 * 55 / 100;
            }
        }

        public double FiberGoal
        {
            get
            {
                return Gender.Equals(Gender.Male) ? 38 : 25;
            }
        }

        public User(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
