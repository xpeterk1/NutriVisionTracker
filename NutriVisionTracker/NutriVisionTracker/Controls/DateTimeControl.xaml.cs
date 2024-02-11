using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NutriVisionTracker.Controls
{
    /// <summary>
    /// Interaction logic for DateTimeControl.xaml
    /// </summary>
    public partial class DateTimeControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<DateTime> DateChanged;

        private DateTime _dateTime;
        public DateTime DateTime { get => _dateTime;
            set 
            {
                _dateTime = value;
                OnPropertyChanged();
                DateChanged?.Invoke(_dateTime);
            }
        }

        public DateTimeControl()
        {
            DataContext = this;
            InitializeComponent();
            DateTime = DateTime.Today;
        }

        private void btnToPast_Click(object sender, RoutedEventArgs e)
        {
            DateTime = DateTime.Subtract(new TimeSpan(1, 0, 0, 0));
        }

        private void btnToFuture_Click(object sender, RoutedEventArgs e)
        {
            DateTime = DateTime.AddDays(1);
        }


        
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
