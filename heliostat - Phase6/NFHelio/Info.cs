namespace NFHelio
{
  using NFHelio.Devices;
  using System;
  using System.Collections;

  internal class Info
  {
    public readonly IServiceProvider serviceProvider;

    public Info(IServiceProvider serviceProvider)
    {
      this.serviceProvider = serviceProvider;
    }

    public ArrayList GetInfo()
    {
      var list = new ArrayList();

      var settings = (Settings)this.serviceProvider.GetService(typeof(Settings));
      var realTimeClockFactory = (IRealTimeClockFactory)this.serviceProvider.GetService(typeof(IRealTimeClockFactory));
      var realTimeClock = realTimeClockFactory.Create();

      var dt = realTimeClock.GetTime();

      list.Add($"********************");
      list.Add($"Following the sun: {settings.FollowSun}");
      list.Add($"Projecting the sun: {settings.ProjectSun}");
      list.Add($"********************");
      list.Add($"Latitude: {settings.Latitude}");
      list.Add($"Longitude: {settings.Longitude}");
      list.Add($"DateTime: {dt.ToString("yyyy / MM / dd HH: mm:ss")}");
      list.Add($"********************");
      list.Add($"Azimuth Adc Min: {settings.AzimuthAdcMin}");
      list.Add($"Azimuth Adc Max: {settings.AzimuthAdcMax}");
      list.Add($"Zenith Adc Min: {settings.ZenithAdcMin}");
      list.Add($"Zenith Adc Max: {settings.ZenithAdcMax}");
      list.Add($"********************");
      for (int i = 0; i < settings.Aci.Length; i++)
      {
        list.Add($"Azimuth calibration: {settings.Aci[i]} - {settings.Acv[i]}");
      }
      list.Add($"********************");
      for (int i = 0; i < settings.Zci.Length; i++)
      {
        list.Add($"Zenith calibration: {settings.Zci[i]} - {settings.Zcv[i]}");
      }
      list.Add($"********************");
      list.Add($"Azimuth projecting: {settings.AzimuthProjection}");
      list.Add($"Zenith projecting: {settings.ZenithProjection}");
      list.Add($"********************");

      return list;
    }
  }
}
