namespace NFHelio.Tasks
{
  using System;

  internal class MoveMirror : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "move";

    /// <inheritdoc />
    public override string Description => "Moves the mirror";

    /// <inheritdoc />
    public override string Help => "move <plane> <angle> where plane is a or z,\nangle is the desired angle";

    public MoveMirror(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      if (args.Length != 2)
      {
        this.SendString("To move, provide a or z and angle\n");
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

      short angleDesired = short.Parse(args[1]);

      var selfCheck = new SelfCheck(this.GetServiceProvider());
      var issues = selfCheck.Check(SelfcheckReason.MotorMovement);
      if (issues)
      {
        return;
      }

      var motorController = new MotorController(this.GetServiceProvider());
      motorController.MoveMotorToAngle(plane, angleDesired);
    }
  }
}
