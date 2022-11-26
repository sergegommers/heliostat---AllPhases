namespace NFHelio.Tasks
{
  using System;

  /// <summary>
  /// Finds the free RAM by allocating as much bytes as possible
  /// </summary>
  internal class FindFreeRam: BaseTask
  {
    /// <inheritdoc />
    public override string Command => "freeram";

    /// <inheritdoc />
    public override string Description => "Finds how much RAM is available";

    /// <inheritdoc />
    public override string Help => "No further info";

    /// <summary>
    /// Initializes a new instance of the <see cref="FindFreeRam"/> class.
    /// </summary>
    public FindFreeRam(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      int freeram = 0;

      for (int i = 15; i > 4; i--)
      {
        var relsize = (int)Math.Pow(2, i);
        try
        {
          this.SendString($"FreeRam - Allocating {relsize + freeram} bytes\n");
          Alloc(relsize + freeram);
          freeram += relsize;
          this.SendString($"FreeRam - Allocated {freeram} bytes\n");
        }
        catch
        {
        }
      }

      this.SendString($"FreeRam = {freeram} bytes\n");
    }

    private void Alloc(int size)
    {
      byte[] data = new byte[size];
      data[0] = 1;
    }
  }
}
