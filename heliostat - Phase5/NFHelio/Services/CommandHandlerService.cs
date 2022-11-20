namespace NFHelio.Services
{
  using System.Collections;
  using NFHelio.Tasks;
  using System.Diagnostics;
  using System;
  using NFCommon.Services;

  /// <summary>
  /// Here we check the incoming commands and start the matching tasks.
  /// </summary>
  public class CommandHandlerService : ICommandHandlerService
  {
    private readonly IServiceProvider provider;
    public readonly IAppMessageWriter appMessageWriter;

    /// <summary>Initializes a new instance of the <see cref="CommandHandlerService" /> class.</summary>
    /// <param name="provider"></param>
    /// <param name="appMessageWriter"></param>
    public CommandHandlerService(
      IServiceProvider provider,
      IAppMessageWriter appMessageWriter)
    {
      this.provider = provider;
      this.appMessageWriter = appMessageWriter;
    }

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public void HandleMessage(string message)
    {
      if (string.IsNullOrEmpty(message))
      {
        return;
      }

      message = message.Trim();

      Debug.WriteLine($"Received=>{message}");

      string[] args = message.Split(' ');
      if (args.Length != 0)
      {
        var command = args[0].ToLower();
        message = message.Substring(args[0].Length).Trim();
        if (string.IsNullOrEmpty(message))
        {
          args = new string[0];
        }
        else
        {
          args = message.Split(' ');
        }

        var tasks = new ArrayList
        {
          new SetTime(this.provider),
          new GetTime(this.provider),
          new SetPosition(this.provider),
          new GetPosition(this.provider),
          new TestAdc(this.provider),
          new Calibrate(this.provider),
          new CalcSpa(this.provider),
          new TestOnboardLed(this.provider),
          new FindFreeRam(this.provider),
          new FreeMem(this.provider),
          new Reboot(this.provider),
        };

        switch (command)
        {
          case "help":
            if (args.Length == 0)
            {
              this.appMessageWriter.SendString($"help <command> for more info.\n");
              this.appMessageWriter.SendString($"\n");
            }
            foreach (ITask task in tasks)
            {
              if (args.Length > 0)
              {
                if (args[0].ToLower() == task.Command.ToLower())
                {
                  this.appMessageWriter.SendString($"{task.Help}\n");
                  break;
                }
              }
              else
              {
                this.appMessageWriter.SendString($"{task.Command}: {task.Description}\n");
              }
            }
            break;
          default:
            foreach (ITask task in tasks)
            {
              if (command == task.Command)
              {
                task.Execute(args);

                return;
              }
            }

            this.appMessageWriter.SendString($"Unknown command\n");

            break;
        }
      }
    }
  }
}
