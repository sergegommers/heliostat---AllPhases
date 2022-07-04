namespace NFHelio.Tasks
{
  /// <summary>
  /// An interface for tasks to execute as a result of an incoming command
  /// </summary>
  internal interface ITask
  {
    /// <summary>
    /// returns the name of the command
    /// </summary>
    string Command { get; }

    /// <summary>
    /// returns the description of the command
    /// </summary>
    string Description { get; }

    /// <summary>
    /// returns the help of the command
    /// </summary>
    string Help { get; }

    /// <summary>
    /// Executes this instance.
    /// </summary>
    /// <param name="args">The arguments for the command.</param>
    void Execute(string[] args);
  }
}
