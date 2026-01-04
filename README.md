# SASB_SI_ASSESSMENT [IOT]

## a) Setup / Build Instructions

1. Ensure **Visual Studio 2022** (or later) with **.NET 6 / WPF** support is installed.
2. Clone the repository or download the project ZIP.
3. Open `SASB_SI_ASSESSMENT.sln` in Visual Studio.
4. Build the solution (`Ctrl+Shift+B`) to restore dependencies.
5. Run the application (`F5`) to launch the main window.

---

## b) Summary of Completed Items

- Simulated device communication with periodic heartbeats.
- Device statuses: **Online**, **Idle**, **Offline**, **Error**, **Off**.
- Real-time **System Logs** for monitoring status changes.
- Add, delete, toggle power, and manually update devices.
- Error pop-ups for devices in **Error** state.
- Offline device warnings and recovery guidance.
- Implementation follows **MVVM pattern** with clean separation of logic, UI, and data.

---

## c) Tools / Libraries Used

- **C# / .NET 6 WPF**
- **MVVM** design pattern for maintainable code structure
- **DispatcherTimer** to simulate device heartbeats
- **ObservableCollection** for real-time UI updates
- Standard **WPF MessageBox** for alerts
- **INotifyPropertyChanged** for data-binding notifications

---

## d) How to Use / Run the Application

- **Add Device:** Enter a device name, select a type, click **Add Device**.
- **Toggle Power:** Use the Power On/Off button to switch device states.
- **Update Device:** Click **Update** to manually check or reset a device.
  - Error devices can be reset to **Online**.
  - Offline devices require powering on first.
- **Delete Device:** Click **Delete** to remove a device from the system.
- **View System Logs:** Monitor real-time status changes and alerts in the **System Logs** panel.
- **Error Handling:** Pop-ups notify operators of devices in **Error** states for immediate action.

---

## ✅ Optional Notes for Future Extensions

- Integrate a **database** to persist devices and logs.
- Use **cloud services** (Azure IoT, AWS IoT) for large-scale deployment.
- Implement **user authentication** and **role-based access control**.
