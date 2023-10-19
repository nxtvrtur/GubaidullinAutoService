using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GubaidullinAutoService
{
    /// <summary>
    ///     Interaction logic for AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private readonly Service _currentService = new Service();

        public AddEditPage(Service selectedService)
        {
            InitializeComponent();
            if (selectedService != null) _currentService = selectedService;
            DataContext = _currentService;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentService.Title)) errors.AppendLine("Укажите название услуги");
            if (_currentService.Cost == 0) errors.AppendLine("Укажите стоимость услуги");

            if (_currentService.Discount <= 0)
                errors.AppendLine("Укажите скидку");
            if (_currentService.Discount > 100)
                errors.AppendLine("Невозможно указать такую скидку");

            if (string.IsNullOrWhiteSpace(_currentService.Discount.ToString())) 
                _currentService.Discount = 0;
            if (string.IsNullOrWhiteSpace(_currentService.DurationInSeconds))
                errors.AppendLine("Укажите длительность услуги");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentService.Id == 0) Gubaidullin_AutoServiceEntities2.GetContext().Service.Add(_currentService);

            try
            {
                Gubaidullin_AutoServiceEntities2.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}