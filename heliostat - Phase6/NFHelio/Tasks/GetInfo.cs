namespace NFHelio.Tasks
{
  using System;

  internal class GetInfo : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "getinfo";

    /// <inheritdoc />
    public override string Description => "Displays the settings";

    /// <inheritdoc />
    public override string Help => "getinfo";

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInfo"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public GetInfo(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      var info = new Info(this.GetServiceProvider());
      var lines = info.GetInfo();

      foreach (var line in lines)
      {
        this.SendString($"{(string)line}\n");
      }
    }
  }
}
