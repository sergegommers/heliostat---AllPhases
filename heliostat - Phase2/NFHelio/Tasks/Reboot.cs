namespace NFHelio.Tasks
{
  using nanoFramework.Runtime.Native;
  using System.Threading;

  /// <summary>
  /// Reboots the microcontroller
  /// </summary>
  internal class Reboot : ITask
  {
    /// <inheritdoc />
    string ITask.Command => "reboot";

    /// <inheritdoc />
    string ITask.Description => "Reboots the microcontroller";

    /// <inheritdoc />
    string ITask.Help => "No further info";

    /// <inheritdoc />
    public void Execute(string[] args)
    {
      Program.context.BluetoothSpp.SendString("Rebooting now\n");
      Thread.Sleep(100);
      Power.RebootDevice();
    }
  }
}
