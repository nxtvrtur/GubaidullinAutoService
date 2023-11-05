using System;
using System.Linq;
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
        private readonly bool IsServiceExist = false;
        public AddEditPage(Service selectedService)
        {
            InitializeComponent();
            if (selectedService != null)
            {
                IsServiceExist = true;
                _currentService = selectedService;
            }
            DataContext = _currentService;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            var errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentService.Title)) errors.AppendLine("Укажите название услуги");
            if (_currentService.Cost == 0) errors.AppendLine("Укажите стоимость услуги");

            if (_currentService.Discount < 0)
                errors.AppendLine("Укажите скидку");
            if (_currentService.Discount > 100)
                errors.AppendLine("Невозможно указать такую скидку");

            if (string.IsNullOrWhiteSpace(_currentService.Discount.ToString())) 
                _currentService.Discount = 0;
            if (_currentService.Duration < 0 || _currentService.Duration > 240)
                errors.AppendLine("Невозможно указать такую длительность");
            if (string.IsNullOrWhiteSpace(_currentService.Duration.ToString())) 
                _currentService.Duration = 0;

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            var allServices = Gubaidullin_AutoServiceEntities2.GetContext().Service.ToList();
            allServices = allServices.Where(p => p.Title == _currentService.Title).ToList();
            if (allServices.Count == 0 || IsServiceExist == true)
            {
                if (_currentService.Id == 0)
                {
                    Gubaidullin_AutoServiceEntities2.GetContext().Service.Add(_currentService);
                }

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
            else
            {
                Gubaidullin_AutoServiceEntities2.GetContext().SaveChanges();
                MessageBox.Show("Уже существует такая услуга");
            }
        }
    }
}