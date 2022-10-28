namespace NFHelio.Tasks
{
  using System;
  using System.Device.Pwm;
  using System.Threading;

  /// <summary>
  /// Tests the onboard led
  /// </summary>
  internal class TestOnboardLed : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "testled";

    /// <inheritdoc />
    public override string Description => "Tests the onboard led";

    /// <inheritdoc />
    public override string Help => "No further info";

    public TestOnboardLed(IServiceProvider serviceProvider)
      :base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      var pwmPin = PwmChannel.CreateFromPin((int)GPIOPort.ESP32_Onboard_Led, 40000, 0);

      // Start the PWM
      pwmPin.Start();

      TestChannel(pwmPin);

      // Stop the PWM:
      pwmPin.Stop();

      SendString($"Done testing\n");
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
