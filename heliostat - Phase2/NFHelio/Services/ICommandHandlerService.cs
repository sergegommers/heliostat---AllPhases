namespace NFHelio.Services
{
  /// <summary>
  /// An interface for a command handler
  /// </summary>
  public interface ICommandHandlerService
  {
    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    void HandleMessage(string message);
  }
}