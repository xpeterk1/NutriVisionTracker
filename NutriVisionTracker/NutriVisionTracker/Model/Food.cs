using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace NutriVisionTracker.Model
{
    public class Food
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        [Required]
        public string Name { get;  private set; }

        [Required]
        public int Calories { get; private set; }

        [Required]
        public int Fats { get; private set; }

        [Required]
        public int Proteins { get; private set; }

        [Required]
        public int Carbohydrates { get; private set; }

        [Required]
        public int Fiber { get; private set; }

        [Required]
        public int UserID { get; private set; }

        [Required]
        public DateTime DateTime { get; private set; }

        [Required]
        public string Suggestion { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; private set; }

        public Food(string name, int calories, int fats, int proteins, int carbohydrates, int fiber)
        {
            Name = name;
            Calories = calories;
            Fats = fats;
            Proteins = proteins;
            Carbohydrates = carbohydrates;
            Fiber = fiber;
        }

        public void SetUserAndDate(int userID, DateTime dt) 
        {
            UserID = userID;
            DateTime = dt;
        }

        public bool IsValid() 
        {
            return !string.IsNullOrEmpty(Name) && Calories != -1 && Proteins != -1 && Fats != -1 && Fiber != -1 && Carbohydrates != -1;
        }
    }
}
