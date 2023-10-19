using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GubaidullinAutoService
{
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();
            var currentServices = Gubaidullin_AutoServiceEntities2.GetContext().Service.ToList();
            ServiceListView.ItemsSource = currentServices;
            ComboType.SelectedIndex = 0;
            UpdateServices();
        }

        private void UpdateServices()
        {
            var currentServices = Gubaidullin_AutoServiceEntities2.GetContext().Service.ToList();
            currentServices = ComboType.SelectedIndex switch
            {
                0 => currentServices.Where(p => p.Discount >= 0 && p.Discount <= 100).ToList(),
                1 => currentServices.Where(p => p.Discount >= 0 && p.Discount < 5).ToList(),
                2 => currentServices.Where(p => p.Discount >= 5 && p.Discount < 15).ToList(),
                3 => currentServices.Where(p => p.Discount >= 15 && p.Discount < 30).ToList(),
                4 => currentServices.Where(p => p.Discount >= 30 && p.Discount < 70).ToList(),
                5 => currentServices.Where(p => p.Discount >= 70 && p.Discount <= 100).ToList()
            };
            currentServices = currentServices.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower()))
                .ToList();
            if (RButtonDown.IsChecked != null && RButtonDown.IsChecked.Value)
                currentServices = currentServices.OrderByDescending(p => p.Cost).ToList();

            if (RButtonUp.IsChecked != null && RButtonUp.IsChecked.Value)
                currentServices = currentServices.OrderBy(p => p.Cost).ToList();

            ServiceListView.ItemsSource = currentServices;
        }

        private void TBoxSearch_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateServices();

        private void ComboType_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateServices();

        private void RButtonUp_OnChecked(object sender, RoutedEventArgs e) => UpdateServices();

        private void RButtonDown_OnChecked(object sender, RoutedEventArgs e) => UpdateServices();

        private void AddButton_OnClick(object sender, RoutedEventArgs e) => 
            Manager.MainFrame.Navigate(new AddEditPage(null));

        private void EditButton_OnClick(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button)?.DataContext as Service));
        }

        private void ServicePage_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Gubaidullin_AutoServiceEntities2.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                ServiceListView.ItemsSource = Gubaidullin_AutoServiceEntities2.GetContext().Service.ToList();
            }
        }
    }
}