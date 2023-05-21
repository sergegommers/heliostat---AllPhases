namespace NFHelio.Tasks
{
  using System;

  /// <summary>
  /// Does a self check to see if all configuration is done to move the mirror automatically
  /// </summary>
  internal class Selfcheck : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "selfcheck";

    /// <inheritdoc />
    public override string Description => "Does a check to see if everything is configured";

    /// <inheritdoc />
    public override string Help => "No further info";

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPosition"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public Selfcheck(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      var selfCheck = new SelfCheck(this.GetServiceProvider());
      var issues = selfCheck.Check(
        SelfcheckReason.Basic |
        SelfcheckReason.MotorMovement |
        SelfcheckReason.HelioStat);

      if (!issues)
      {
        this.SendString($"All OK\n");
      }
    }
  }
}
