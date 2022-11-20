namespace NFHelio.Tasks
{
  using System;
  using System.Device.Adc;
  using System.Diagnostics;
  using System.Threading;

  /// <summary>
  /// Tests the ADC
  /// </summary>
  internal class TestAdc : BaseTask
  {
    /// <inheritdoc />
    public override string Command => "testadc";

    /// <inheritdoc />
    public override string Description => "Tests the adc controller";

    /// <inheritdoc />
    public override string Help => "testadc <plane> <samples> where plane is a or z\nand samples is the number of measurements to take";

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAdc"/> class.
    /// </summary>
    public TestAdc(IServiceProvider serviceProvider)
      : base(serviceProvider)
    {
    }

    /// <inheritdoc />
    public override void Execute(string[] args)
    {
      var adc = new AdcController();

      if (args.Length < 1)
      {
        this.SendString("To test, provide a or z and (optionally) #samples\n");
        return;
      }

      int adcChannelNumber;
      switch (args[0].ToLower())
      {
        case "a":
          adcChannelNumber = Context.AzimuthAdcChannel;
          break;
        case "z":
          adcChannelNumber = Context.ZenithAdcChannel;
          break;
        default:
          this.SendString("Unknown plane\n");
          return;
      }

      int setSize = 1;
      if (args.Length > 1)
      {
        setSize = int.Parse(args[1]);
      }

      Debug.WriteLine($"adc min = {adc.MinValue}, max = {adc.MaxValue}, setsize = {setSize}");

      var endTime = DateTime.UtcNow + TimeSpan.FromSeconds(10);

      var adcChannel = adc.OpenChannel(adcChannelNumber);

      double value = 0;
      double percent = 0;
      while (DateTime.UtcNow < endTime)
      {
        for (int i = 0; i < setSize; i++)
        {
          value += adcChannel.ReadValue();
          percent += adcChannel.ReadRatio();
        }

        value /= setSize;
        percent /= setSize;

        this.SendString($"value = {(int)value}, ratio = {percent}\n");

        value = 0;
        percent = 0;

        Thread.Sleep(1000);
      }

      this.SendString($"Done testing adc\n");
    }
  }
}
