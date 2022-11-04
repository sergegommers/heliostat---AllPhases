namespace NFHelio.Tasks
{
  using System;

  /// <summary>
  /// Reads the stored geographical position in the <see cref="Settings"/>
  /// </summary>
  internal class GetPosition : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "getpos";

    /// <inheritdoc />
    public override string Description => "Gets the stored position as latitude longitude";

    /// <inheritdoc />
    public override string Help => "getpos";

    /// <summary>
    /// Initializes a new instance of the <see cref="GetPosition"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public GetPosition(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      var settings = (Settings)this.GetServiceProvider().GetService(typeof(Settings));
      this.SendString($"Latitude longitude set to {settings.Latitude}, {settings.Longitude}\n");
    }
  }
}
