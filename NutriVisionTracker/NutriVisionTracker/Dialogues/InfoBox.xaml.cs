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
    /// Interaction logic for InfoBox.xaml
    /// </summary>
    public partial class InfoBox : Window
    {
        public enum Type { Success,  Failure}

        public InfoBox(string text, Type type)
        {
            InitializeComponent();

            label.Content = text;

            if (type == Type.Success) 
            {
                Uri imageUri = new Uri("/NutriVisionTracker;component/Resources/success.png", UriKind.Relative);
                img.Source = new BitmapImage(imageUri);
            } else if (type == Type.Failure) 
            {
                Uri imageUri = new Uri("/NutriVisionTracker;component/Resources/failure.png", UriKind.Relative);
                img.Source = new BitmapImage(imageUri);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
