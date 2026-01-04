using System;
using System.ComponentModel; // For INotifyPropertyChanged

namespace SASB_SI_ASSESSMENT.Models
{
    /// <summary>
    /// Represents a physical device in the system (sensor, actuator, or other types)
    /// with properties for ID, name, type, status, and last updated timestamp.
    /// Implements INotifyPropertyChanged to notify the UI of property changes.
    /// </summary>
    public class Device : INotifyPropertyChanged
    {
        // Backing field for Status property
        private string _status = string.Empty;

        // Backing field for Status property
        private DateTime _lastUpdated;

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the device (Online, Offline, Idle, Error, Off).
        /// Updates to this property will notify the UI automatically.
        /// </summary>
        public string Status
        {
            get => _status;

            set
            {
                if (_status != value)
                {
                    _status = value;
                    //Notifies the UI to refresh when Status changes
                    OnPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// Timestamp of the last status update.
        /// Automatically notifies the UI when changed.
        /// </summary>
        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set
            {
                if (_lastUpdated != value)
                {
                    _lastUpdated = value;
                    OnPropertyChanged(nameof(LastUpdated));
                }
            }
        }

        /// <summary>
        /// Event triggered when a property value changes.
        /// Required by INotifyPropertyChanged to update UI bindings.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Helper method to raise PropertyChanged events.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
