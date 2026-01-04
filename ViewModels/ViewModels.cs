using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using SASB_SI_ASSESSMENT.Models;
using SASB_SI_ASSESSMENT.Services;

namespace SASB_SI_ASSESSMENT.ViewModels
{
    /// <summary>
    /// MainViewModel coordinates application state, simulated device communication and user interactions. 
    /// Acts as the central controller in the MVVM pattern.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // Service responsible for simulating device communication
        private DeviceService _service;

        // Timer used to simulate periodic device heartbeats
        private DispatcherTimer _timer;

        // Collection bound to the UI to display all devices
        public ObservableCollection<DeviceViewModel> Devices { get; set; }

        // Real-time system log bound to the UI for monitoring and debugging
        public ObservableCollection<string> SystemLogs { get; set; }

        #region Add Device Input Properties

        // Two-way bound input for device name
        private string _newDeviceName = "";
        public string NewDeviceName
        {
            get => _newDeviceName;
            set 
            { 
                _newDeviceName = value; 
                OnPropertyChanged(nameof(NewDeviceName)); 
            }
        }

        // Selected device type from predefined list
        private string _newDeviceType = "Sensor";
        public string NewDeviceType
        {
            get => _newDeviceType;
            set 
            { 
                _newDeviceType = value; 
                OnPropertyChanged(nameof(NewDeviceType)); 
            }
        }

        // Device type options shown in the UI
        public ObservableCollection<string> DeviceTypes { get; set; } = new ObservableCollection<string>
        {
            "Sensor",
            "Actuator", 
            "Gadgets & Appliances", 
            "Security Devices"
        };

        #endregion

        // Command used by the UI to add a new device
        public ICommand AddDeviceCommand { get; set; }

        /// <summary>
        /// Initializes the ViewModel, loads mock devices,
        /// and starts the simulated communication loop.
        /// </summary>
        public MainViewModel()
        {
            _service = new DeviceService();
            SystemLogs = new ObservableCollection<string>();

            Devices = new ObservableCollection<DeviceViewModel>();

            // Load initial mock devices
            foreach (var device in _service.GetMockDevices())
            {
                Devices.Add(new DeviceViewModel(device, this));
                LogAction($"Mock device loaded: {device.Name} ({device.Type}) - Status: {device.Status}");
            }

            AddDeviceCommand = new RelayCommand(AddDevice);

            // Timer simulates periodic device heartbeats (real-world polling or push) 
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += SimulateDeviceHeartbeats;
            _timer.Start();
        }

        /// <summary>
        /// Simulates device communication by periodically updating device status.
        /// Devices in Off, Error, or Offline states do not send heartbeats.
        /// </summary>
        private void SimulateDeviceHeartbeats(object sender, EventArgs e)
        {
            foreach (var deviceVM in Devices)
            {
                // Powered-off devices do not communicate
                if (deviceVM.Status == "Off")
                    continue;

                // Error and Offline states require manual intervention
                if (deviceVM.Status == "Error" || deviceVM.Status == "Offline")
                    continue;

                string oldStatus = deviceVM.Status;
                string newStatus = _service.SimulateDeviceSignal();

                // Apply updates only if state has changed
                if (oldStatus != newStatus)
                {
                    deviceVM.Status = newStatus;
                    deviceVM.LastUpdated = DateTime.Now;

                    LogAction($"Device {deviceVM.Name}: {oldStatus} → {newStatus}");

                    // Notify operator immediately when an error occurs
                    if (newStatus == "Error")
                        ShowErrorPopup(deviceVM);

                    // Log offline state separately for monitoring
                    if (newStatus == "Offline")
                        LogAction($"Device {deviceVM.Name} is offline - operator may reconnect");
                }
            }
        }

        /// <summary>
        /// Simulates remotely powering a device ON or OFF.
        /// </summary>
        public void TogglePower(DeviceViewModel deviceVM)
        {
            if (deviceVM == null) return;

            if (deviceVM.Status == "Off")
            {
                deviceVM.Status = "Online";
                deviceVM.LastUpdated = DateTime.Now;
                LogAction($"Device {deviceVM.Name} powered ON remotely");
            }
            else if (deviceVM.Status == "Error" || deviceVM.Status == "Online" || deviceVM.Status == "Idle")
            {
                deviceVM.Status = "Off";
                deviceVM.LastUpdated = DateTime.Now;
                LogAction($"Device {deviceVM.Name} powered OFF remotely");
            }
            else if (deviceVM.Status == "Offline")
            {
                // Bring it online if offline
                deviceVM.Status = "Online";
                deviceVM.LastUpdated = DateTime.Now;
                LogAction($"Device {deviceVM.Name} remotely reconnected from Offline → Online");
            }
        }

        /// <summary>
        /// Displays a blocking alert when a device enters an error state.
        /// </summary>
        private void ShowErrorPopup(DeviceViewModel deviceVM)
        {
            MessageBox.Show(
                $"Device \"{deviceVM.Name}\" has entered an ERROR state.\n\nManual intervention required.",
                "Device Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        /// <summary>
        /// Adds a new device to the system using user-provided input.
        /// </summary>
        private void AddDevice()
        {
            if (string.IsNullOrWhiteSpace(NewDeviceName)) return;

            var device = new Device
            {
                Id = Devices.Count + 1,
                Name = NewDeviceName,
                Type = NewDeviceType,
                Status = "Online",
                LastUpdated = DateTime.Now
            };

            Devices.Add(new DeviceViewModel(device, this));
            LogAction($"Device {device.Name} added (Status: {device.Status})");

            // Reset input fields
            NewDeviceName = "";
            NewDeviceType = "Sensor";
        }

        /// <summary>
        /// Removes a device from the system.
        /// </summary>
        public void DeleteDevice(DeviceViewModel deviceVM)
        {
            if (deviceVM == null) return;
            Devices.Remove(deviceVM);
            LogAction($"Device {deviceVM.Name} deleted");
        }

        /// <summary>
        /// Allows manual device inspection or recovery.
        /// </summary>
        public void UpdateDeviceStatus(DeviceViewModel deviceVM)
        {
            if (deviceVM == null) return;

            string oldStatus = deviceVM.Status;

            if (oldStatus == "Error")
            {
                // Error devices can be reset manually
                deviceVM.Status = "Online";
                deviceVM.LastUpdated = DateTime.Now;
                LogAction($"Device {deviceVM.Name} manually reset by operator → Online");
            }
            else if (oldStatus == "Offline")
            {
                // Offline devices must power back ON to be updated
                MessageBox.Show(
                    $"Device {deviceVM.Name} is Offline. Please power it ON to reconnect.",
                    "Device Offline",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                LogAction($"Device {deviceVM.Name} is Offline — Power ON to reconnect.");
            }
            else
            {
                // Online or Idle devices can be checked
                deviceVM.LastUpdated = DateTime.Now;
                LogAction($"Device {deviceVM.Name} manually checked; status remains {deviceVM.Status}");
            }
        }

        /// <summary>
        /// Inserts a timestamped message into the system log.
        /// </summary>
        private void LogAction(string message)
        {
            SystemLogs.Insert(0, $"{DateTime.Now}: {message}");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    /// <summary>
    /// Basic ICommand implementation used to bind UI actions (buttons, menu items)
    /// to ViewModel logic without code-behind.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Action<object?> _executeParam;
        private readonly Func<bool>? _canExecute;

        /// <summary>
        /// Creates a command with no parameters.
        /// </summary>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Creates a command that accepts a parameter from the UI.
        /// </summary>
        public RelayCommand(Action<object?> executeParam, Func<bool>? canExecute = null)
        {
            _executeParam = executeParam;
            _canExecute = canExecute;
        }

        /// <summary>
        /// WPF calls this to determine whether the command is currently executable.
        /// </summary>
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

        /// <summary>
        /// Executes the bound action when the command is triggered.
        /// </summary>
        public void Execute(object? parameter)
        {
            if (_execute != null) _execute();
            else if (_executeParam != null) _executeParam(parameter);
        }

        /// <summary>
        /// Raised when command availability changes (e.g., enabling/disabling buttons).
        /// </summary>
        public event EventHandler? CanExecuteChanged;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Generic ICommand implementation for strongly-typed command parameters.
    /// </summary>
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Func<T, bool>? _canExecute;

        public RelayCommand(Action<T> execute, Func<T, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute((T)parameter!);
        public void Execute(object? parameter) => _execute((T)parameter!);
    }
}
