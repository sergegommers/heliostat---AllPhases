namespace NFHelio.Tests
{
  using nanoFramework.TestFramework;
  using NFCommon;
  using System.Collections;

  /// <summary>
  /// A test class for the <see cref="CalibrationArray"/>
  /// </summary>
  [TestClass]
  public class CalibrationTests
  {
    [TestMethod]
    public void TestIfPointAreSorted()
    {
      var calibrationPoints = new ArrayList();
      var calibrationArray = new CalibrationArray(calibrationPoints);

      calibrationArray.AddCalibrationPoint(3, 3);
      calibrationArray.AddCalibrationPoint(4, 4);
      calibrationArray.AddCalibrationPoint(1, 1);
      calibrationArray.AddCalibrationPoint(-10, -10);

      Assert.Equal(((CalibrationPoint)calibrationPoints[0]).Index, (short)-10);
      Assert.Equal(((CalibrationPoint)calibrationPoints[1]).Index, (short)1);
      Assert.Equal(((CalibrationPoint)calibrationPoints[2]).Index, (short)3);
      Assert.Equal(((CalibrationPoint)calibrationPoints[3]).Index, (short)4);
    }

    [TestMethod]
    public void TestIfPointsCanBeUpdated()
    {
      var calibrationPoints = new ArrayList();
      var calibrationArray = new CalibrationArray(calibrationPoints);

      // add the first point
      calibrationArray.AddCalibrationPoint(10, 1000);
      Assert.Equal(calibrationArray.Count(), 1, "Expected 1 point to be present");
      bool result = calibrationArray.GetCalibrationPoint(10, out short value, true);
      Assert.True(result);
      Assert.Equal(value, (short)1000);

      // update the first point
      calibrationArray.AddCalibrationPoint(10, 2000);
      Assert.Equal(calibrationArray.Count(), 1);
      result = calibrationArray.GetCalibrationPoint(10, out value, true);
      Assert.True(result);
      Assert.Equal(value, (short)2000);

      // add the second point
      calibrationArray.AddCalibrationPoint(20, 4000);
      Assert.Equal(calibrationArray.Count(), 2);
      result = calibrationArray.GetCalibrationPoint(20, out value, true);
      Assert.True(result);
      Assert.Equal(value, (short)4000);
    }

    [TestMethod]
    public void TestWithTwoPoints()
    {
      var calibrationPoints = new ArrayList();
      var calibrationArray = new CalibrationArray(calibrationPoints);

      calibrationArray.AddCalibrationPoint(1, 1);
      calibrationArray.AddCalibrationPoint(10, 10);

      // test in between the 2 points
      bool result = calibrationArray.GetCalibrationPoint(5, out short value);
      Assert.True(result);
      Assert.Equal(value, (short)5);

      // test after the 2 points
      result = calibrationArray.GetCalibrationPoint(12, out value);
      Assert.True(result);
      Assert.Equal(value, (short)12);

      // test before the 2 points
      result = calibrationArray.GetCalibrationPoint(-1, out value);
      Assert.True(result);
      Assert.Equal(value, (short)-1);
    }

    [TestMethod]
    public void TestWithThreePoints()
    {
      var calibrationPoints = new ArrayList();
      var calibrationArray = new CalibrationArray(calibrationPoints);

      calibrationArray.AddCalibrationPoint(1, 1);
      calibrationArray.AddCalibrationPoint(10, 10);
      calibrationArray.AddCalibrationPoint(20, 40);

      // test in between the first 2 points
      bool result = calibrationArray.GetCalibrationPoint(5, out short value);
      Assert.True(result);
      Assert.Equal(value, (short)5);

      // test in between the last 2 points
      result = calibrationArray.GetCalibrationPoint(15, out value);
      Assert.True(result);
      Assert.Equal(value, (short)25);

      // test after the 3 points
      result = calibrationArray.GetCalibrationPoint(30, out value);
      Assert.True(result);
      Assert.Equal(value, (short)70);

      // test before the 3 points
      result = calibrationArray.GetCalibrationPoint(-1, out value);
      Assert.True(result);
      Assert.Equal(value, (short)-1);
    }

    [TestMethod]
    public void TestReverseLookup()
    {
      short[] indexes = new short[] { 1, 10, 20 };
      short[] values = new short[] { 1, 10, 40 };


      // reverse the inputs
      var calibrationArray = new CalibrationArray(values, indexes);

      // test in between the first 2 points
      bool result = calibrationArray.GetCalibrationPoint(5, out short value);
      Assert.True(result);
      Assert.Equal(value, (short)5);

      // test in between the last 2 points
      result = calibrationArray.GetCalibrationPoint(25, out value);
      Assert.True(result);
      Assert.Equal(value, (short)15);

      // test after the 3 points
      result = calibrationArray.GetCalibrationPoint(70, out value);
      Assert.True(result);
      Assert.Equal(value, (short)30);

      // test before the 3 points
      result = calibrationArray.GetCalibrationPoint(-1, out value);
      Assert.True(result);
      Assert.Equal(value, (short)-1);
    }
  }
}
