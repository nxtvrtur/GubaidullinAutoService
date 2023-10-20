using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GubaidullinAutoService
{
    public partial class ServicePage : Page
    {
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;
        List<Service> CurrentPageList = new List<Service>();
        List<Service> TableList;
        public ServicePage()
        {
            InitializeComponent();
            var currentServices = Gubaidullin_AutoServiceEntities2.GetContext().Service.ToList();
            ServiceListView.ItemsSource = currentServices;
            ComboType.SelectedIndex = 0;
            UpdateServices();
        }
        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountRecords % 10 > 0)
            {
                CountPage = CountRecords / 10 + 1;
            }
            else
            {
                CountPage = CountRecords / 10;
            }
            var ifUpdate = true;
            int min;
            if (selectedPage.HasValue)
            {
                if (selectedPage >= 0 && selectedPage <= CountPage)
                {
                    CurrentPage = (int)selectedPage;
                    min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                    for (int i = CurrentPage*10; i < min; i++)
                    {
                        CurrentPageList.Add(TableList[i]);
                    }
                }
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        if (CurrentPage > 0)
                        {
                            CurrentPage--;
                            min = CurrentPage * 10 +10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                            for (int i = CurrentPage * 10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            ifUpdate = false;
                        }
                        break;
                    case 2:
                        if (CurrentPage < CountPage - 1) 
                        {
                            CurrentPage++;
                            min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage *10 +10 : CountRecords;
                            for (int i = CurrentPage*10; i < min; i++)
                            {
                                CurrentPageList.Add(TableList[i]);
                            }
                        }
                        else
                        {
                            ifUpdate = false;
                        }
                        break;
                }
            }
            if (ifUpdate)
            {
                PageListBox.Items.Clear();
                for (int i = 1; i <= CountPage; i++)
                {
                    PageListBox.Items.Add(i);
                }
                PageListBox.SelectedIndex = CurrentPage;

                min = CurrentPage * 10 + 10 < CountRecords ? CurrentPage * 10 + 10 : CountRecords;
                TBCount.Text = min.ToString();
                TBAllRecords.Text = " из " + CountRecords.ToString();

                ServiceListView.ItemsSource = CurrentPageList;
                ServiceListView.Items.Refresh();
            }
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
            TableList = currentServices;
            ChangePage(0, 0);
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
            UpdateServices();
        }

        private void DeleteButton_OnClick(object sender, RoutedEventArgs e)
        {
            var currentService = (sender as Button)?.DataContext as Service;

            var currentClientServices = Gubaidullin_AutoServiceEntities2.GetContext().ClientService.ToList();
            currentClientServices = currentClientServices.Where(p => p.ServiceID == currentService.Id).ToList();
            if (currentClientServices.Count != 0)
            {
                MessageBox.Show("Невозможно выполнить удаления, так как существуют записи на эту услугу");
            }
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?",
                        "Внимание!",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == 
                    MessageBoxResult.Yes)
                {
                    try
                    {
                        Gubaidullin_AutoServiceEntities2.GetContext().Service.Remove(currentService!);
                        Gubaidullin_AutoServiceEntities2.GetContext().SaveChanges();
                        ServiceListView.ItemsSource = Gubaidullin_AutoServiceEntities2.GetContext().Service.ToList();
                        UpdateServices();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null); 
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null); 
        }

        private void PageListBox_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }
    }
}