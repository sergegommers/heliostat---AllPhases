namespace NFHelio.Tasks
{
  using NFHelio.Devices;
  using System;

  /// <summary>
  /// Sets the time
  /// </summary>
  internal class SetTime : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "settime";

    /// <inheritdoc />
    public override string Description => "Sets the time of the real time clock";

    /// <inheritdoc />
    public override string Help => "settime <yyyy> <mm> <dd> <hh> <mm> <ss>\nwith the time in UTC";

    /// <summary>
    /// Initializes a new instance of the <see cref="SetTime" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public SetTime(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      if (args.Length != 6)
      {
        this.SendString("To set the time provide year month day hour minute second\n");
        return;
      }

      var realTimeClockFactory = (IRealTimeClockFactory)this.GetServiceProvider().GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();
      
      realTimeClock.SetTime(
        int.Parse(args[0]),
        int.Parse(args[1]),
        int.Parse(args[2]),
        int.Parse(args[3]),
        int.Parse(args[4]),
        int.Parse(args[5]));

      var dt = realTimeClock.GetTime();
      this.SendString($"Time set to: {dt.ToString("yyyy/MM/dd HH:mm:ss")}\n");
    }
  }
}
