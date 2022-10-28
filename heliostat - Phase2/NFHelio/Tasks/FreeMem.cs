namespace NFHelio.Tasks
{
  using nanoFramework.Hardware.Esp32;

  /// <summary>
  /// Shows details about the memory
  /// </summary>
  internal class FreeMem : ITask
  {
    /// <inheritdoc />
    string ITask.Command => "mem";

    /// <inheritdoc />
    string ITask.Description => "Shows details about the memory";

    /// <inheritdoc />
    string ITask.Help => "No further info";

    /// <inheritdoc />
    public void Execute(string[] args)
    {
      NativeMemory.GetMemoryInfo(
        NativeMemory.MemoryType.All,
        out uint totalSize,
        out uint totalFreeSize,
        out uint largestBlock);

      Program.context.BluetoothSpp.SendString($"Native memory - Total:{totalSize}\n");
      Program.context.BluetoothSpp.SendString($"Native memory - Free:{totalFreeSize}\n");
      Program.context.BluetoothSpp.SendString($"Native memory - Largest:{largestBlock}\n");
    }
  }
}
