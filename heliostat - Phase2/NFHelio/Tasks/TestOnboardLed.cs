namespace NFHelio.Tasks
{
  using System.Device.Pwm;
  using System.Threading;

  /// <summary>
  /// Tests the onboard led
  /// </summary>
  internal class TestOnboardLed : ITask
  {
    /// <inheritdoc />
    string ITask.Command => "testled";

    /// <inheritdoc />
    string ITask.Description => "Tests the onboard led";

    /// <inheritdoc />
    string ITask.Help => "No further info";

    /// <inheritdoc />
    public void Execute(string[] args)
    {
      var pwmPin = PwmChannel.CreateFromPin((int)GPIOPort.ESP32_Onboard_Led, 40000, 0);

      // Start the PWM
      pwmPin.Start();

      TestChannel(pwmPin);

      // Stop the PWM:
      pwmPin.Stop();

      Program.context.BluetoothSpp.SendString($"Done testing\n");
    }

    private void TestChannel(PwmChannel channel)
    {
      bool dutyCycleGoingUp = true;
      var dutyCycle = .00f;

      while (true)
      {
        if (dutyCycleGoingUp)
        {
          // slowly increase light intensity
          dutyCycle += 0.05f;

          // change direction if reaching maximum duty cycle (100%)
          if (dutyCycle > .95)
          {
            dutyCycleGoingUp = !dutyCycleGoingUp;
          }
        }
        else
        {
          // slowly decrease light intensity
          dutyCycle -= 0.05f;

          // change direction if reaching minimum duty cycle (0%)
          if (dutyCycle < 0.10)
          {
            break;
          }
        }

        // update duty cycle
        channel.DutyCycle = dutyCycle;

        Thread.Sleep(50);
      }

      channel.DutyCycle = .00f;
    }
  }
}
