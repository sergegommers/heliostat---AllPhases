namespace NFHelio.Tasks
{
  using System;

  internal class MoveMotor : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "movemotor";

    /// <inheritdoc />
    public override string Description => "Moves the motor";

    /// <inheritdoc />
    public override string Help => "movemotor <plane> <speed> where plane is a or z,\nspeed is the speed between -1 and +1";

    public MoveMotor(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      if (args.Length != 2)
      {
        this.SendString("To move, provide a or z and speed\n");
        return;
      }

      MotorPlane plane;
      switch (args[0].ToLower())
      {
        case "a":
          plane = MotorPlane.Azimuth;
          break;
        case "z":
          plane = MotorPlane.Zenith;
          break;
        default:
          this.SendString("Unknown plane\n");
          return;
      }

      double speed = double.Parse(args[1]);

      var motorController = new MotorController(this.GetServiceProvider());
      motorController.MoveMotorIndefinitely(plane, speed);
    }
  }
}
