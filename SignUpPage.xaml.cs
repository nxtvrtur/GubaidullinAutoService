using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace GubaidullinAutoService;

/// <summary>
///     Interaction logic for SignUpPage.xaml
/// </summary>
public partial class SignUpPage : Page
{
    private readonly Service _currentService = new();
    private readonly ClientService _currentClientService = new();

    public SignUpPage(Service SelectedService)
    {
        InitializeComponent();
        if (SelectedService is not null) _currentService = SelectedService;

        DataContext = _currentService;
        var currentClient = Gubaidullin_AutoServiceEntities2.GetContext().Client.ToList();
        ComboClient.ItemsSource = currentClient;
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        StringBuilder errors = new();
        if (ComboClient.SelectedItem is null)
            errors.AppendLine("Укажите ФИО клиента");
        if (StartDate.Text == string.Empty) 
            errors.AppendLine("Укажите дату услуги");

        if (TBStart.Text == string.Empty) 
            errors.AppendLine("Укажите время начала услуги");

        if (errors.Length > 0)
        {
            MessageBox.Show(errors.ToString());
            return;
        }

        _currentClientService.ClientID = ComboClient.SelectedIndex + 1;
        _currentClientService.ServiceID = _currentService.Id;
        _currentClientService.StartTime = Convert.ToDateTime($"{StartDate.Text} {TBStart.Text}");
        if (_currentClientService.ID == 0)
            Gubaidullin_AutoServiceEntities2.GetContext().ClientService.Add(_currentClientService);
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

    private void TBStart_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        string s = TBStart.Text;
        if (s.Length <= 3 || !s.Contains(':'))
            TBEnd.Text = "";
        else
        {
            string[] start = s.Split(new char[] { ':' });
            Console.WriteLine(start);
            int startHour = Convert.ToInt32(start[0].ToString()) * 60;
            int startMin = Convert.ToInt32(start[1].ToString());
            int sum = startHour + startMin + _currentService.Duration;
            int EndHour = sum / 60;
            if (EndHour > 23)
            {
                EndHour -= 24;
            }
            int EndMin = sum % 60;

            if (EndMin < 9)
                s = EndHour.ToString() + ":0" + EndMin.ToString();
            else
            {
                s = EndHour.ToString() + ":" + EndMin.ToString();
            }

            TBEnd.Text = s;
        }
    }

    private void TBStart_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        // проверяем, что вводится только цифра
        if (!char.IsDigit(e.Text, e.Text.Length - 1))
        {
            e.Handled = true;
            return;
        }

        // получаем текущее значение текста в текстбоксе
        string currentValue = ((TextBox)sender).Text;
        if (currentValue.Length == 0)
        {
            int hours1 = Convert.ToInt32(e.Text);
            TBStart.Clear();
            if (hours1 > 2)
            {
                currentValue = "";
                currentValue = "0" + (hours1).ToString();
                TBStart.Text = "";

                TBStart.Text = currentValue;
                e.Handled = true;
            }
        }
        if (currentValue.Length == 1)
        {
            if (currentValue[0] == '2')
            {
                int hours2 = Convert.ToInt32(e.Text);
                Console.WriteLine(hours2);
                if (hours2 > 3)
                {
                    e.Handled = true; // Игнорируем ввод
                    return;
                }
            }

        }

        if (currentValue.Length == 3)
        {
            int minute = Convert.ToInt32(e.Text);
            if (minute > 5)
            {
                e.Handled = true; // Игнорируем ввод
                return;
            }
        }

        // Если введено 2 цифры и следующий символ не ":", добавляем ":"
        if (currentValue.Length == 2 && e.Text != ":")
        {
            currentValue += ":";
        }

        // Если введено 5 символов (формат "hh:mm"), то не даем вводить больше
        if (currentValue.Length > 5)
        {
            e.Handled = true;
            return;
        }

            // Обновляем значение текста в TextBox
            ((TextBox)sender).Text = currentValue;
        ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
    }
}