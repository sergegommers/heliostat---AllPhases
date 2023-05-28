namespace NFHelio
{
  using NFCommon;
  using NFCommon.Services;
  using System;
  using System.Device.Pwm;
  using System.Diagnostics;

  /// <summary>
  /// An enum for the planes we can move in
  /// </summary>
  public enum MotorPlane
  {
    Azimuth,
    Zenith
  }

  public static class MotorPlaneNames
  {
    public static string Name(MotorPlane plane)
    {
      switch (plane)
      {
        case MotorPlane.Azimuth:
          return "Azimuth";
        case MotorPlane.Zenith:
          return "Zenith";
        default:
          return "Unknown plane";
      }
    }
  }

  /// <summary>
  /// A class for controlling motors
  /// </summary>
  public class MotorController
  {
    public readonly IServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MotorController"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public MotorController(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public void MoveMotorToPosition(MotorPlane plane, short position)
    {
      int adcChannel;
      PwmChannel pwmPin1;
      PwmChannel pwmPin2;

      var appMessageWriter = (IAppMessageWriter)serviceProvider.GetService(typeof(IAppMessageWriter));

      var selfCheck = new SelfCheck(this.serviceProvider);
      var issues = selfCheck.Check(SelfcheckReason.MotorMovement);
      if (issues)
      {
        return;
      }

      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

      switch (plane)
      {
        case MotorPlane.Azimuth:
          adcChannel = Context.AzimuthAdcChannel;
          pwmPin1 = PwmChannel.CreateFromPin((int)GPIOPort.PWM_Azimuth_East_to_West, Context.PwmFrequency, 0);
          pwmPin2 = PwmChannel.CreateFromPin((int)GPIOPort.PWM_Azimuth_West_to_East, Context.PwmFrequency, 0);
          break;

        case MotorPlane.Zenith:
          adcChannel = Context.ZenithAdcChannel;
          pwmPin1 = PwmChannel.CreateFromPin((int)GPIOPort.PWM_Zenith_Up, Context.PwmFrequency, 0);
          pwmPin2 = PwmChannel.CreateFromPin((int)GPIOPort.PWM_Zenith_Down, Context.PwmFrequency, 0);
          break;

        default:
          appMessageWriter.SendString("Unknown plane\n");
          return;
      }

      // limit the new adc value between the set ranges
      switch (plane)
      {
        case MotorPlane.Azimuth:
          {
            short min = (short)Math.Min(settings.AzimuthAdcMin, settings.AzimuthAdcMax);
            short max = (short)Math.Max(settings.AzimuthAdcMin, settings.AzimuthAdcMax);

            position = (short)Math.Min(position, max);
            position = (short)Math.Max(position, min);
          }
          break;
        case MotorPlane.Zenith:
          {
            short min = (short)Math.Min(settings.ZenithAdcMin, settings.ZenithAdcMax);
            short max = (short)Math.Max(settings.ZenithAdcMin, settings.ZenithAdcMax);

            position = (short)Math.Min(position, max);
            position = (short)Math.Max(position, min);
          }
          break;
      }

      // get the current value of the motor position
      short value = (short)AdcReader.GetValue(adcChannel, Context.AdcSampleSize);

      // determine the direction of movement
      PwmChannel pwmPin;
      if (value > position)
      {
        pwmPin = pwmPin1;
      }
      else
      {
        pwmPin = pwmPin2;
      }

      int originalDiff = Math.Abs(position - value);
      int lastCheckedDiff = originalDiff;
      float currentDutyCycle = 0f;

      try
      {
        // initialize the PWM output, but note nothing will happen until the dutycycle is set different from 0
        pwmPin1.Start();
        pwmPin2.Start();

        var polarityCheckTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);
        var standstillCheckTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);
        while (true)
        {
          // read the current adc value
          value = (short)AdcReader.GetValue(adcChannel, Context.AdcSampleSize);

          Debug.WriteLine($"ValueDesired: {position} value: {value}");

          // know when to stop...
          if (Math.Abs(position - value) < 10)
          {
            appMessageWriter.SendString($"Endpoint reached: valueDesired {position} value {value}\n");
            break;
          }

          int currentDiff = Math.Abs(position - value);

          // after some seconds, check if we are farther away from where we are supposed to be
          if (DateTime.UtcNow > polarityCheckTime)
          {
            if (currentDiff > originalDiff)
            {
              pwmPin.DutyCycle = 0f;

              appMessageWriter.SendString($"Motor {MotorPlaneNames.Name(plane)} is moving in wrong direction, reverse the polarity\n");

              Debug.WriteLine($"Original diff: {originalDiff}, current diff {currentDiff}");

              break;
            }

            polarityCheckTime = DateTime.MaxValue;
          }

          // check if the mirror is still moving, if not, stop the motor
          if (DateTime.UtcNow > standstillCheckTime)
          {
            if (Math.Abs(currentDiff - lastCheckedDiff) < 10)
            {
              pwmPin.DutyCycle = 0f;

              appMessageWriter.SendString($"Mirror {MotorPlaneNames.Name(plane)} is not moving, check if the motor is stuck\n");

              Debug.WriteLine($"Current diff: {originalDiff}, last checked diff {lastCheckedDiff}");

              break;
            }
            else
            {
              // keep on checking
              standstillCheckTime = DateTime.UtcNow + TimeSpan.FromSeconds(3);
              lastCheckedDiff = currentDiff;
            }
          }

          // check if we overshot our desired value, and if yes reverse direction
          if (value > position && pwmPin != pwmPin1)
          {
            pwmPin.DutyCycle = 0f;
            pwmPin = pwmPin1;
            Debug.WriteLine($"Overshoot detected, reversing polarity");
          }

          if (value < position && pwmPin != pwmPin2)
          {
            pwmPin.DutyCycle = 0f;
            pwmPin = pwmPin2;
            Debug.WriteLine($"Overshoot detected, reversing polarity");
          }

          // adapt the speed of motor depending how far we are from the desired position
          float dutyCycle;
          if (Math.Abs(position - value) > 100)
          {
            dutyCycle = 1f;
          }
          else
          {
            dutyCycle = Math.Abs(position - value) / 100f;

            // set a minimum speed at which the motor still moves
            dutyCycle = Math.Max(dutyCycle, 0.1f);
          }

          if (currentDutyCycle != dutyCycle)
          {
            currentDutyCycle = dutyCycle;
          }

          pwmPin.DutyCycle = dutyCycle;
        }
      }
      catch (Exception ex)
      {
        appMessageWriter.SendString($"MoveMirror stopped with exception {ex.Message}\n");
      }
      finally
      {
        // full stop
        pwmPin.DutyCycle = 0f;
        pwmPin1.Stop();
        pwmPin2.Stop();
      }
    }

    /// <summary>
    /// Moves the motor.
    /// </summary>
    /// <param name="plane">The plane.</param>
    /// <param name="angleDesired">The angle desired.</param>
    public void MoveMotorToAngle(MotorPlane plane, short angleDesired)
    {
      CalibrationArray array;

      var appMessageWriter = (IAppMessageWriter)serviceProvider.GetService(typeof(IAppMessageWriter));

      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));

      switch (plane)
      {
        case MotorPlane.Azimuth:
          array = new CalibrationArray(settings.Aci, settings.Acv);
          break;

        case MotorPlane.Zenith:
          array = new CalibrationArray(settings.Zci, settings.Zcv);

          break;
        default:
          appMessageWriter.SendString("Unknown plane\n");
          return;
      }

      // get the desired value using the calibrated angle/value settings
      array.GetCalibrationPoint(angleDesired, out short valueDesired);

      MoveMotorToPosition(plane, valueDesired);

      return;
    }

    /// <summary>
    /// Moves the motor.
    /// </summary>
    /// <param name="plane">The plane.</param>
    /// <param name="speed">The speed.</param>
    public void MoveMotorIndefinitely(MotorPlane plane, double speed)
    {
      PwmChannel pwmPin1;
      PwmChannel pwmPin2;

      if (speed < -1.0 || speed > 1.0)
      {
        return;
      }

      var appMessageWriter = (IAppMessageWriter)serviceProvider.GetService(typeof(IAppMessageWriter));

      switch (plane)
      {
        case MotorPlane.Azimuth:
          pwmPin1 = PwmChannel.CreateFromPin((int)GPIOPort.PWM_Azimuth_East_to_West, Context.PwmFrequency, 0);
          pwmPin2 = PwmChannel.CreateFromPin((int)GPIOPort.PWM_Azimuth_West_to_East, Context.PwmFrequency, 0);
          break;
        case MotorPlane.Zenith:
          pwmPin1 = PwmChannel.CreateFromPin((int)GPIOPort.PWM_Zenith_Up, Context.PwmFrequency, 0);
          pwmPin2 = PwmChannel.CreateFromPin((int)GPIOPort.PWM_Zenith_Down, Context.PwmFrequency, 0);
          break;
        default:
          appMessageWriter.SendString("Unknown plane\n");
          return;
      }

      PwmChannel pwmPin;
      if (speed > 0)
      {
        pwmPin = pwmPin1;
      }
      else
      {
        pwmPin = pwmPin2;
      }

      float dutyCycle = (float)Math.Abs(speed);

      if (dutyCycle != 0.0)
      {
        // initialize the PWM output, but note nothing will happen until the dutycycle is set different from 0
        pwmPin1.Start();
        pwmPin2.Start();

        pwmPin.DutyCycle = dutyCycle;
      }
      else
      {
        // full stop
        pwmPin1.DutyCycle = 0f;
        pwmPin2.DutyCycle = 0f;
        pwmPin1.Stop();
        pwmPin2.Stop();
      }
    }
  }
}
