using NutriVisionTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NutriVisionTracker.Dialogues
{
    /// <summary>
    /// Interaction logic for UserDialogue.xaml
    /// </summary>
    public partial class UserDialogue : Window
    {
        public User User { get; set; }

        public UserDialogue()
        {
            InitializeComponent();
            DataContext = this;
            labelTitle.Content = "New User";
            cbGender.ItemsSource = (Activity[])Enum.GetValues(typeof(Gender));
            cbActivity.ItemsSource = (Activity[])Enum.GetValues(typeof(Activity));
            User = new User("", "");
        }

        public UserDialogue(User currentUser)
        {
            InitializeComponent();
            labelTitle.Content = "Edit User";
            DataContext = this;
            cbGender.ItemsSource = (Gender[])Enum.GetValues(typeof(Gender));
            cbActivity.ItemsSource = (Activity[])Enum.GetValues(typeof(Activity));
            User = currentUser;
            tbName.IsEnabled = false;
            tbEmail.IsEnabled = false;
        }

        private void OKClicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbName.Text)) 
            {
                new InfoBox("Empty or incorrect name", InfoBox.Type.Failure).ShowDialog();
                return;
            }
            else if (string.IsNullOrEmpty(tbEmail.Text))
            {
                new InfoBox("Empty or incorrect email", InfoBox.Type.Failure).ShowDialog();
                return;
            }
            else if (string.IsNullOrEmpty(pwdBox1.Password) || string.IsNullOrEmpty(pwdBox2.Password) || pwdBox1.Password != pwdBox2.Password)
            {
                new InfoBox("Empty or incorrect password or passwords do not match.", InfoBox.Type.Failure).ShowDialog();
                return;
            }
            else if (string.IsNullOrEmpty(tbAge.Text) || !(int.TryParse(tbAge.Text, out int a) && a > 0))
            {
                new InfoBox("Empty or incorrect age", InfoBox.Type.Failure).ShowDialog();
                return;
            }
            else if (string.IsNullOrEmpty(tbHeight.Text) || !(int.TryParse(tbHeight.Text, out int h) && h > 0))
            {
                new InfoBox("Empty or incorrect height", InfoBox.Type.Failure).ShowDialog();
                return;
            }
            else if (string.IsNullOrEmpty(tbWeight.Text) || !(int.TryParse(tbWeight.Text, out int w) && w > 0))
            {
                new InfoBox("Empty or incorrect weight", InfoBox.Type.Failure).ShowDialog();
                return;
            }


            DialogResult = true;
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
