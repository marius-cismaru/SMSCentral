using SMSCentral.Models;
using SMSCentral.Services;
using SMSCentral.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using SMSCentral.Views;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Management;
using GsmComm.GsmCommunication;

namespace SMSCentral.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ICollectionView _viewCollection;
        private string _searchCriteriaReceivers;
        private DataService _dataService;
        private Receiver _receiverSelected;
        private Receiver _receiverUpdated;
        private string _smsMessage;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            _receiverUpdated = new Receiver();
            _dataService = new DataService();

            ReloadReceivers();
            ReloadOperators();
            ReloadModems();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Receiver> Receivers { get; set; }
        public ObservableCollection<Operator> Operators { get; set; }
        public ObservableCollection<Modem> Modems { get; set; }

        public ICommand ResetFiltersReceiversCommand => new DelegateCommand(ResetFiltersReceivers);
        public ICommand ImportReceiversCommand => new DelegateCommand(ImportReceivers);
        public ICommand DeleteReceiversCommand => new DelegateCommand(DeleteReceivers);
        public ICommand RefreshOperatorsReceiversCommand => new DelegateCommand(RefreshOperatorsReceivers);
        public ICommand SaveReceiverCommand => new DelegateCommand(SaveReceiver);
        public ICommand SendSMSReceiversCommand => new DelegateCommand(SendSMSReceivers);
        public ICommand RefreshModemsCommand => new DelegateCommand(RefreshModems);

        public string SearchCriteriaReceivers
        {
            get => _searchCriteriaReceivers;
            set
            {
                _searchCriteriaReceivers = value;

                _viewCollection.Filter = e =>
                {
                    var item = (Receiver)e;
                    return item != null &&
                           ((item.Name?.Contains(_searchCriteriaReceivers, StringComparison.OrdinalIgnoreCase) ?? false)
                            || (item.PhoneNumber?.StartsWith(_searchCriteriaReceivers, StringComparison.OrdinalIgnoreCase) ?? false)
                            || (item.Group?.Contains(_searchCriteriaReceivers, StringComparison.OrdinalIgnoreCase) ?? false));
                };

                _viewCollection.Refresh();

                Receivers = new ObservableCollection<Receiver>(_viewCollection.OfType<Receiver>());

                OnPropertyChanged(nameof(SearchCriteriaReceivers));
                OnPropertyChanged(nameof(Receivers));
            }
        }

        public Receiver ReceiverSelected
        {
            get => _receiverSelected;
            set
            {
                _receiverSelected = value;

                if (value != null)
                {
                    _receiverUpdated.PhoneNumber = value.PhoneNumber;
                    _receiverUpdated.Name = value.Name;
                    _receiverUpdated.Group = value.Group;
                    _receiverUpdated.Operator = value.Operator;
                }
                else
                {
                    _receiverUpdated = new Receiver();
                }

                OnPropertyChanged(nameof(ReceiverSelected));
                OnPropertyChanged(nameof(ReceiverUpdated));
            }
        }

        public string SMSMessage
        {
            get => _smsMessage;
            set
            {
                _smsMessage = value;

                OnPropertyChanged(nameof(SMSMessage));
            }
        }

        public Receiver ReceiverUpdated
        {
            get => _receiverUpdated;
        }

        private void ReloadReceivers()
        {
            _searchCriteriaReceivers = "";

            Receivers = new ObservableCollection<Receiver>(_dataService.Receivers);
            _viewCollection = CollectionViewSource.GetDefaultView(Receivers);
            ReceiverSelected = null;

            OnPropertyChanged(nameof(ReceiverSelected));
            OnPropertyChanged(nameof(SearchCriteriaReceivers));
            OnPropertyChanged(nameof(Receivers));
        }

        private void ReloadOperators()
        {
            Operators = new ObservableCollection<Operator>(_dataService.Operators);
            Operators.Insert(0, new Operator { Id = 0, Name = "-- UNKNOWN --" });

            OnPropertyChanged(nameof(Operators));
        }

        private void ReloadModems()
        {
            _dataService.Modems.ForEach(o =>
            {
                o.COMPort = string.Empty;
                o.IsConnected = null;
                o.GsmComm = null;
            });

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_POTSModem ");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    if ((string)queryObj["Status"] == "OK")
                    {
                        foreach(var modem in _dataService.Modems)
                        {
                            if (queryObj["Description"].ToString().StartsWith(modem.Name))
                            {
                                modem.COMPort = queryObj["AttachedTo"].ToString();

                                try
                                {
                                    var gsmComm = new GsmCommMain(modem.COMPort, 9600, 30);
                                    gsmComm.Open();

                                    modem.IsConnected = true;
                                    modem.GsmComm = gsmComm;

                                    Thread.Sleep(100);
                                    gsmComm.Close();
                                }
                                catch (Exception ex)
                                {
                                    Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(ReloadModems), ex.Message);
                                }

                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Libs.EventLogFileLib.Write(Libs.EventLogFileLib.Levels.ERROR, this.GetType().FullName + " -> " + nameof(ReloadModems), ex.Message);
            }

            Modems = new ObservableCollection<Modem>(_dataService.Modems);

            OnPropertyChanged(nameof(Modems));
        }

        private void ImportReceivers(object parameter)
        {
            MessageBox.Show("Please select a text file with lines in this format: <phone number>;<name>;<group>", "Import receivers", MessageBoxButton.OK, MessageBoxImage.Information);

            OpenFileDialog dialog = new OpenFileDialog();
            var isOk = dialog.ShowDialog();

            if (isOk.HasValue && isOk.Value == true && File.Exists(dialog.FileName))
            {
                var lines = new List<string>();

                try
                {
                    lines = File.ReadAllLines(dialog.FileName).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error open file", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (lines.Count > 0)
                {
                    int counter = 0;

                    ProgressDialogResult result = ProgressDialog.Execute(Application.Current.MainWindow, "Importing " + lines.Count.ToString() + " receivers", () =>
                    {
                        for (int i = 0; i < lines.Count; i++)
                        {
                            string line = lines[i];

                            var chunks = line.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                            if (chunks.Length >= 1)
                            {
                                var phoneNumber = chunks[0];
                                if (int.TryParse(phoneNumber, out _))
                                {
                                    if (!_dataService.Receivers.Any(o => o.PhoneNumber == phoneNumber))
                                    {
                                        _dataService.Receivers.Add(new Receiver
                                        {
                                            PhoneNumber = phoneNumber,
                                            Name = chunks.Length >= 2 ? chunks[1] : string.Empty,
                                            Group = chunks.Length >= 3 ? chunks[2] : string.Empty
                                        });

                                        counter++;
                                    }
                                }
                            }

                            Thread.Sleep(10);

                            int progress = (int)(100 * (double)(i + 1) / (double)lines.Count);

                            ProgressDialog.Current.ReportWithCancellationCheck(progress, "Imported receiver {0} / {1} ...", (i + 1), lines.Count);
                        }
                    }, new ProgressDialogSettings(true, true, false));

                    var importedMessage = "Imported " + counter + " receivers";

                    if (result.Cancelled)
                        MessageBox.Show(importedMessage, "Importing cancelled", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (result.OperationFailed)
                        MessageBox.Show(importedMessage, "Importing failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show(importedMessage, "Imported successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    _dataService.SaveReceivers();

                    ReloadReceivers();
                }
            }
        }

        private void DeleteReceivers(object parameter)
        {
            if (parameter != null)
            {
                System.Collections.IList items = (System.Collections.IList)parameter;

                var selection = items?.Cast<Receiver>().ToList();

                if (MessageBox.Show($"Are you sure you want to delete selected {selection.Count} receiver(s)?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Hand) == MessageBoxResult.Yes)
                {
                    _dataService.DeleteReceivers(selection);

                    _dataService.SaveReceivers();

                    ReloadReceivers();
                }
            }
        }

        private void ResetFiltersReceivers(object parameter)
        {
            ReloadReceivers();
        }

        private void RefreshOperatorsReceivers(object parameter)
        {
            if (parameter != null)
            {
                System.Collections.IList items = (System.Collections.IList)parameter;

                var selection = items?.Cast<Receiver>().ToList();

                ProgressDialogResult result = ProgressDialog.Execute(Application.Current.MainWindow, "Refreshing operators for " + selection.Count.ToString() + " receivers", () =>
                {
                    for (int i = 0; i < selection.Count; i++)
                    {
                        Receiver receiver = selection[i];

                        _dataService.RefreshOperatorForReceiver(receiver);

                        Thread.Sleep(200);

                        int progress = (int)(100 * (double)(i + 1) / (double)selection.Count);

                        ProgressDialog.Current.ReportWithCancellationCheck(progress, "Got operator for receiver {0} / {1} ...", (i + 1), selection.Count);
                    }
                }, new ProgressDialogSettings(true, true, false));

                //if (result.Cancelled)
                //    MessageBox.Show("Cancelled");
                //else if (result.OperationFailed)
                //    MessageBox.Show("Failed");
                //else
                //    MessageBox.Show("Successfully executed");

                _dataService.SaveReceivers();

                ReloadReceivers();
            }
        }

        private void SendSMSReceivers(object parameter)
        {
            if (parameter != null)
            {
                System.Collections.IList items = (System.Collections.IList)parameter;

                var receivers = items?.Cast<Receiver>().ToList();

                var counter = 0;

                ProgressDialogResult result = ProgressDialog.Execute(Application.Current.MainWindow, "Sending SMS to " + receivers.Count.ToString() + " receivers", () =>
                {
                    for (int i = 0; i < receivers.Count; i++)
                    {
                        Receiver receiver = receivers[i];

                        if (_dataService.SendSMS(receiver, SMSMessage.Replace(Environment.NewLine, " ")))
                            counter++;

                        Thread.Sleep(100);

                        int progress = (int)(100 * (double)(i + 1) / (double)receivers.Count);

                        ProgressDialog.Current.ReportWithCancellationCheck(progress, "Sent SMS to receiver {0} / {1} ...", (i + 1), receivers.Count);
                    }
                }, new ProgressDialogSettings(true, true, false));

                var importedMessage = "Sent SMS to " + counter + " receivers.";

                if (result.Cancelled)
                    MessageBox.Show(importedMessage, "Sending SMS cancelled", MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (result.OperationFailed)
                    MessageBox.Show(importedMessage, "Sending SMS failed", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show(importedMessage, "Sending SMS successful", MessageBoxButton.OK, MessageBoxImage.Information);

                _dataService.SaveReceivers();

                ReloadReceivers();
            }
        }

        private void SaveReceiver(object parameter)
        {
            if (ReceiverUpdated != null && !string.IsNullOrWhiteSpace(ReceiverUpdated.PhoneNumber))
            {
                var itemExisting = _dataService.Receivers
                    .FirstOrDefault(o => o.PhoneNumber == ReceiverUpdated.PhoneNumber);

                if (itemExisting != null)
                {
                    itemExisting.Name = ReceiverUpdated.Name;
                    itemExisting.Group = ReceiverUpdated.Group;
                    itemExisting.Operator = ReceiverUpdated.Operator == null || ReceiverUpdated.Operator.Id == 0 ? null : ReceiverUpdated.Operator;
                    itemExisting.OperatorId = ReceiverUpdated.Operator == null || ReceiverUpdated.Operator.Id == 0 ? null : (int?)ReceiverUpdated.Operator.Id;
                }
                else
                {
                    if (int.TryParse(ReceiverUpdated.PhoneNumber, out _))
                    {
                        _dataService.Receivers.Add(
                            new Receiver
                            {
                                Name = ReceiverUpdated.Name,
                                PhoneNumber = ReceiverUpdated.PhoneNumber,
                                Group = ReceiverUpdated.Group,
                                OperatorId = ReceiverUpdated.Operator == null || ReceiverUpdated.Operator.Id == 0 ? null : (int?)ReceiverUpdated.Operator.Id,
                                Operator = ReceiverUpdated.Operator == null || ReceiverUpdated.Operator.Id == 0 ? null : ReceiverUpdated.Operator
                            });
                    }
                }

                _dataService.SaveReceivers();

                ReloadReceivers();
            }
        }

        private void RefreshModems(object parameter)
        {
            ReloadModems();
        }
    }
}