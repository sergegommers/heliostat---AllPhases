namespace NFHelio.Tasks
{
  using System;
  using NFHelio.Services;

  /// <summary>
  /// A base class for all Tasks.
  /// </summary>
  internal abstract class BaseTask: ITask
  {
    private readonly IServiceProvider serviceProvider;
    private readonly IAppMessageWriter appMessageWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseTask"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public BaseTask(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
      this.appMessageWriter = (IAppMessageWriter)serviceProvider.GetService(typeof(IAppMessageWriter));
    }

    /// <inheritdoc />
    public abstract string Command { get; }

    /// <inheritdoc />
    public abstract string Description { get; }

    /// <inheritdoc />
    public abstract string Help { get; }

    /// <inheritdoc />
    public abstract void Execute(string[] args);

    protected IServiceProvider GetServiceProvider()
    {
      return serviceProvider;
    }

    protected void SendString(string message)
    {
      this.appMessageWriter.SendString(message);
    }
  }
}
