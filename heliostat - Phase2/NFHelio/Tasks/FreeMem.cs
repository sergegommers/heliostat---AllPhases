namespace NFHelio.Tasks
{
  using nanoFramework.Hardware.Esp32;
  using System;

  /// <summary>
  /// Shows details about the memory
  /// </summary>
  internal class FreeMem: BaseTask
  {
    /// <inheritdoc />
    public override string Command => "mem";

    /// <inheritdoc />
    public override string Description => "Shows details about the memory";

    /// <inheritdoc />
    public override string Help => "No further info";

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeMem"/> class.
    /// </summary>
    public FreeMem(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      NativeMemory.GetMemoryInfo(
        NativeMemory.MemoryType.All,
        out uint totalSize,
        out uint totalFreeSize,
        out uint largestBlock);

      this.SendString($"Native memory - Total:{totalSize}\n");
      this.SendString($"Native memory - Free:{totalFreeSize}\n");
      this.SendString($"Native memory - Largest:{largestBlock}\n");
    }
  }
}
