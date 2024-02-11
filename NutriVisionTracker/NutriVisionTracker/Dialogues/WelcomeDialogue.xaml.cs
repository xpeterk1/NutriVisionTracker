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
    /// Interaction logic for WelcomeDialogue.xaml
    /// </summary>
    public partial class WelcomeDialogue : Window
    {
        public event Func<User, bool> RegisterUserRequested;
        public event Func<string, string, bool> LoginUserRequested;

        public WelcomeDialogue()
        {
            InitializeComponent();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            bool? success = LoginUserRequested?.Invoke(nameBox.Text, pwdBox.Password);

            if (!success.HasValue || !success.Value)
            {
                new InfoBox("User with this name does not exist!", InfoBox.Type.Failure).ShowDialog();
                return;
            }


            DialogResult = true;
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            var dial = new UserDialogue();
            var res = dial.ShowDialog();
            bool? success = RegisterUserRequested?.Invoke(dial.User);

            if (success.HasValue && success.Value) 
            {
                new InfoBox("Registration successful.", InfoBox.Type.Success).ShowDialog();
            } else 
            {
                new InfoBox("Error with registration.", InfoBox.Type.Failure).ShowDialog();
            }
        }

        private void ExitClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
