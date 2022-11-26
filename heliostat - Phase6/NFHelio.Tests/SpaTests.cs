namespace NFHelio.Tests
{
  using NFSpa;
  using nanoFramework.TestFramework;
  using System;

  /// <summary>
  /// Tests for the Sun Position Algorithm
  /// </summary>
  [TestClass]
  public class SpaTest
  {
    /// <summary>
    /// Verifies the output of the Sun Position Algorithm
    /// </summary>
    [TestMethod]
    public void VerifySpaCalculation()
    {
      var spa = new Spa_data
      {
        year = 2003,
        month = 10,
        day = 17,
        hour = 12,
        minute = 30,
        second = 30,
        timezone = -7.0f,
        delta_ut1 = 0,
        delta_t = 67,
        longitude = -105.1786f,
        latitude = 39.742476f,
        elevation = 1830.14f,
        pressure = 820,
        temperature = 11,
        slope = 30,
        azm_rotation = -10,
        atmos_refract = 0.5667f,
        function = (int)Calculator.SpaOutputs.SPA_ALL
      };

      var calculator = new Calculator();

      var result = calculator.Spa_calculate(spa);
      Assert.Equal(result, 0);

      Assert.True(Math.Abs(spa.epsilon - 23.44046452) < 0.00001);
      Assert.True(Math.Abs(spa.zenith - 50.11162585) < 0.00001);
      Assert.True(Math.Abs(spa.azimuth - 194.34020414) < 0.00001);
    }
  }
}
