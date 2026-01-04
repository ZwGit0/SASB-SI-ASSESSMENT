using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using SASB_SI_ASSESSMENT.Models;
using SASB_SI_ASSESSMENT.Services;
using SASB_SI_ASSESSMENT.ViewModels;

namespace SASB_SI_ASSESSMENT.ViewModels
{
    /// <summary>
    /// ViewModel wrapper for a Device model.
    /// Exposes device state and user actions in a UI-friendly way (MVVM pattern).
    /// </summary>
    public class DeviceViewModel : INotifyPropertyChanged
    {
        // Underlying data model representing the physical device
        private readonly Device _device;

        // Reference to the parent MainViewModel to delegate cross-device operations
        // (e.g., delete, update, power toggle)
        private readonly MainViewModel _parent;

        /// <summary>
        /// Initializes a new DeviceViewModel bound to a Device model.
        /// </summary>
        /// /// <param name="device">Device model instance representing the physical device.</param>
        /// <param name="parent">Parent MainViewModel to delegate operations like update, delete, or toggle power.</param>
        public DeviceViewModel(Device device, MainViewModel parent)
        {
            _device = device;
            _parent = parent;

            UpdateCommand = new RelayCommand(UpdateStatus);
            DeleteCommand = new RelayCommand(Delete);
            TogglePowerCommand = new RelayCommand(TogglePower);
        }

        // Read-only properties mapped directly from the model
        public int Id => _device.Id;
        public string Name => _device.Name;
        public string Type => _device.Type;

        /// <summary>
        /// Current operational status of the device.
        /// Notifies the UI when changed to enable real-time updates.
        /// </summary>
        public string Status
        {
            get => _device.Status;
            set
            {
                if (_device.Status != value)
                {
                    _device.Status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        /// <summary>
        /// Timestamp of the most recent device interaction or state change.
        /// </summary>
        public DateTime LastUpdated
        {
            get => _device.LastUpdated;
            set
            {
                if (_device.LastUpdated != value)
                {
                    _device.LastUpdated = value;
                    OnPropertyChanged(nameof(LastUpdated));
                }
            }
        }

        // Commands bound to UI controls (buttons, menu items)
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand TogglePowerCommand { get; }

        // Command handlers delegate logic to MainViewModel
        // to maintain a single source of truth for application behavior
        private void UpdateStatus()
        {
            _parent.UpdateDeviceStatus(this);
        }

        private void Delete()
        {
            _parent.DeleteDevice(this);
        }

        private void TogglePower()
        {
            _parent.TogglePower(this);
        }

        /// <summary>
        /// Exposes the underlying Device model if lower-level access is required
        /// (e.g., persistence, diagnostics).
        /// </summary>
        public Device Device => _device;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
