using SMSCentral.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace SMSCentral.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Current version info for exe
        System.Diagnostics.FileVersionInfo AppInfo { get; set; }

        //Timer for wait a couple of moments showing shutdown message if another instance is running
        public System.Windows.Threading.DispatcherTimer TimerShutdownApp { get; set; }
        //Timer run interval in seconds
        int TimerShutdownAppInterval = 1;

        public MainWindow()
        {
            InitializeComponent();

            //Setup shutdown timer
            TimerShutdownApp = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            TimerShutdownApp.Tick += new EventHandler(TimerShutdownApp_Tick);
            TimerShutdownApp.Interval = TimeSpan.FromSeconds(TimerShutdownAppInterval);

            //Get program name and version from Assembly file
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            AppInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            var appDescription = AppInfo.ProductName + " [Version: " + AppInfo.ProductVersion + "]";
            Title = appDescription;

            //Check if only one instance of application is running. Kill others with fire :)
            if (Libs.TaskManagerLib.GetRunningProcessCount(AppInfo.ProductName, Libs.TaskManagerLib.ProcessNameMatchType.Contains) > 1)
            {
                //Show another instance is running message, wait some and shutdown
                TimerShutdownApp.Start();
            }

            //Shutdown if no printers installed
            //if(PrinterSettings.InstalledPrinters.Count == 0)
            //{
            //    MessageBox.Show("No printers found! Application will shutdown.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

            //    TimerShutdownApp.Start();
            //}

            #if DEBUG
            AppDomain.CurrentDomain.FirstChanceException += (source, e) =>
            {
                Debug.WriteLine("FirstChanceException event raised in {0}: {1}", AppDomain.CurrentDomain.FriendlyName, e.Exception.Message);
            };
            #endif

            DataContext = new ViewModels.MainViewModel();
        }

        private void TimerShutdownApp_Tick(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        //private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        //{
        //    var index = e.Row.GetIndex() + 1;
        //    e.Row.Header = $"{index}";
        //}

        private void ButtonAddNewReceiver_Click(object sender, RoutedEventArgs e)
        {
            //var equipments = FilterDataGrid.SelectedItems.Cast<Equipment>().OrderBy(o => o.SerialNumber).ToList();
            var items = FilterDataGrid.SelectedItems.Cast<Receiver>().ToList();

            //if (MessageBox.Show("Are you sure you want to print labels for the selected " + items.Count.ToString() + " equipments?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            //{
                // Easy way to pass data to the async method
                int millisecondsTimeout = 100;

                ProgressDialogResult result = ProgressDialog.Execute(this, "Sending " + items.Count.ToString() + " labels to selected printer", () =>
                {
                    //int batchCount = (int)Math.Ceiling((double)equipments.Count / (double)LabelTemplateSelected.LabelCountPerRow);

                    //for (int batch = 0; batch < batchCount; batch++)
                    //{
                    //    int progress = (int)(100 * (double)(batch) / (double)batchCount);

                    //    var labelRange = (batch * LabelTemplateSelected.LabelCountPerRow + 1).ToString() + " - " + ((batch + 1) * LabelTemplateSelected.LabelCountPerRow).ToString();

                    //    ProgressDialog.Current.ReportWithCancellationCheck(progress, "Printing label {0} / {1} ...", labelRange, equipments.Count);

                    //    List<int> equipmentIds = new List<int>();

                    //    var labelContent = LabelTemplateSelected.Content;

                    //    for (int i = 0; i < LabelTemplateSelected.LabelCountPerRow; i++)
                    //    {
                    //        var index = i + (batch * LabelTemplateSelected.LabelCountPerRow);

                    //        if (index < equipments.Count)
                    //        {
                    //            var equipment = equipments[index];

                    //            equipmentIds.Add(equipment.Id);

                    //            var id = equipment.Id.ToString();
                    //            if (id.Length < 3) id = "00" + id;

                    //            labelContent = labelContent
                    //                //.Replace("%Link" + (i + 1).ToString() + "%", "http://tmeqcalib.cw01.contiwan.com/equipments/view/" + equipment.Id.ToString())
                    //                .Replace("%Description" + (i + 1).ToString() + "%", equipment.Description)
                    //                .Replace("%Manufacturer" + (i + 1).ToString() + "%", equipment.Manufacturer)
                    //                .Replace("%Model" + (i + 1).ToString() + "%", equipment.Model)
                    //                .Replace("%SerialNumber" + (i + 1).ToString() + "%", equipment.SerialNumber)
                    //                .Replace("%IdNr" + (i + 1).ToString() + "%", equipment.IdNumber)
                    //                .Replace("%Id" + (i + 1).ToString() + "%", id);
                    //        }
                    //    }

                    //    MultiPack.RawPrinterLib.SendStringToPrinter(PrinterSelected, labelContent);

                    //    Repositories.EquipmentsRepository.UpdateLabelPrintedDate(equipmentIds);

                    //    Thread.Sleep(millisecondsTimeout);
                    //}

                }, new ProgressDialogSettings(true, true, false));

                //if (result.Cancelled)
                //    MessageBox.Show("Printing cancelled");
                //else if (result.OperationFailed)
                //    MessageBox.Show("Printing failed");
                //else
                //    MessageBox.Show("Printing successfully executed");

                //(DataContext as ViewModels.MainViewModel).RefreshReceiversCommand.Execute(null);
            //}
        }

        private void ButtonSendSMS_Click(object sender, RoutedEventArgs e)
        {
            var items = FilterDataGrid.SelectedItems.Cast<Receiver>().ToList();

            if (MessageBox.Show("Are you sure you want to send SMS to the selected " + items.Count.ToString() + " receivers?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
            }

                //var equipments = FilterDataGrid.SelectedItems.Cast<Equipment>().ToList();

                //if (MessageBox.Show("Are you sure you want to clear label printed status for the selected " + equipments.Count.ToString() + " equipments?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                //{
                //    Repositories.EquipmentsRepository.ClearLabelPrintedStatus(equipments.Select(o => o.Id).ToList());

                //    (DataContext as ViewModels.MainViewModel).RefreshEquipmentsCommand.Execute(null);
                //}
            }

        private async void ButtonGetOperator_Click(object sender, RoutedEventArgs e)
        {
            //var items = FilterDataGrid.SelectedItems.Cast<Receiver>().ToList();

            //var dc = (DataContext as ViewModels.MainViewModel);

            //var itemUpdated = await dc.DataContext.GetReceiverOperatorAsync(items[0]);

            //var item = dc.DataContext.Receivers.Items.FirstOrDefault(x => x.PhoneNumber == itemUpdated.PhoneNumber);

            //if (item != null)
            //{
            //    item.Operator = itemUpdated.Operator;
            //}

            //dc.RefreshReceiversCommand.Execute(null);
        }
    }
}
