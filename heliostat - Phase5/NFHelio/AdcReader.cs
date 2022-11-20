namespace NFHelio
{
  using System.Device.Adc;

  internal static class AdcReader
  {
    // reads the adc channel
    public static double GetValue(int channel, int sampleSize)
    {
      var adc = new AdcController();

      var adcChannel = adc.OpenChannel(channel);

      double value = 0;

      for (int i = 0; i < sampleSize; i++)
      {
        value += adcChannel.ReadValue();
      }

      value /= sampleSize;

      return value;
    }
  }
}
