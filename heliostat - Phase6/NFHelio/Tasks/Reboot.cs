namespace NFHelio.Tasks
{
  using nanoFramework.Runtime.Native;
  using System;
  using System.Threading;

  /// <summary>
  /// Reboots the microcontroller
  /// </summary>
  internal class Reboot : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "reboot";

    /// <inheritdoc />
    public override string Description => "Reboots the microcontroller";

    /// <inheritdoc />
    public override string Help => "No further info";

    /// <summary>
    /// Initializes a new instance of the <see cref="Reboot"/> class.
    /// </summary>
    public Reboot(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      this.SendString("Rebooting now\n");
      Thread.Sleep(100);
      Power.RebootDevice();
    }
  }
}
