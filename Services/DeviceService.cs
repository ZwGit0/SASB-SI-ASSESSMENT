using SASB_SI_ASSESSMENT.Models;

namespace SASB_SI_ASSESSMENT.Services
{
    /// <summary>
    /// Service class responsible for simulating device operations.
    /// Provides mock data for testing and simulates device signals/status changes.
    /// In a real-world application, this would interface with actual hardware or APIs (e.g., MQTT, REST, Serial).
    /// </summary>
    public class DeviceService
    {
        private Random _random = new Random();

        // Simulates fetching initial seed data 
        public List<Device> GetMockDevices()
        {
            return new List<Device>
        {
            new Device { Id = 1, Name = "Temp Sensor 01", Type = "Sensor", Status = "Online", LastUpdated = DateTime.Now},
            new Device { Id = 2, Name = "Robot Arm A", Type = "Actuator", Status = "Offline", LastUpdated = DateTime.Now }
        };
        }

        // Simulates receiving data/status updates 
        public string SimulateDeviceSignal()
        {
            int roll = _random.Next(1, 101); // 1–100

            if (roll <= 50) return "Online";   // 50 numbers -> 50% occuring
            if (roll <= 75) return "Offline";  // 25 numbers -> 25% occuring
            if (roll <= 90) return "Idle";     // 15 numbers -> 15% occuring
            return "Error";                    // 10 numbers -> 10% occuring
        }
    }
}
