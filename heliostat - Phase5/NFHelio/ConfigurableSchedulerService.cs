namespace NFHelio
{
  using nanoFramework.Hosting;
  using System;
  using System.Threading;

  public abstract class ConfigurableSchedulerService : IHostedService, IDisposable
  {
    private Timer _executeTimer;

    /// <summary>
    /// Schedules the immediate execution of <see cref="ExecuteAsync"/> on the provided interval.
    /// </summary>
    protected ConfigurableSchedulerService()
    {
      Interval = TimeSpan.FromMilliseconds(-1);
      Time = TimeSpan.FromMilliseconds(-1);
    }

    /// <summary>
    /// Gets or sets the due time of the timer. 
    /// </summary>
    private TimeSpan Time { get; set; }

    /// <summary>
    /// Gets or sets the interval of the timer.
    /// </summary>
    private TimeSpan Interval { get; set; }

    /// <summary>
    /// This method is called each time the timer elapses. 
    /// </summary>
    protected abstract void ExecuteAsync();

    /// <summary>
    /// Starts the timer, the ExecuteAsync method will be called every interval.
    /// </summary>
    /// <param name="interval">The interval.</param>
    public virtual void RunEvery(TimeSpan interval)
    {
      Interval = interval;
      Time = TimeSpan.Zero;

      // Store the timer we're executing
      _executeTimer = new Timer(state =>
      {
        ExecuteAsync();
      }, null, Time, Interval);
    }

    /// <inheritdoc />
    public virtual void Start()
    {
    }

    /// <inheritdoc />
    public virtual void Stop()
    {
      if (_executeTimer == null)
      {
        return;
      }

      try
      {
        _executeTimer.Change(Timeout.Infinite, 0);
      }
      finally
      {
        _executeTimer = null;
      }
    }

    /// <inheritdoc />
    public virtual void Dispose()
    {
      _executeTimer?.Dispose();
    }
  }
}
