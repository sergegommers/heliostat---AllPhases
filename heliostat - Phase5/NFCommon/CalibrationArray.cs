namespace NFCommon
{
  using System.Collections;

  /// <summary>
  /// Represents an calibration point with an index and value
  /// </summary>
  public class CalibrationPoint
  {
    /// <summary>
    /// Gets or sets the index.
    /// </summary>
    public short Index { get; set; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public short Value { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CalibrationPoint"/> class.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="value">The value.</param>
    public CalibrationPoint(short index, short value)
    {
      Index = index;
      Value = value;
    }
  }

  /// <summary>
  /// An class to store an array of calibration points and retrieve the interpolated value for a random index
  /// </summary>
  public class CalibrationArray
  {
    private ArrayList calibrationPoints;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalibrationArray"/> class.
    /// </summary>
    /// <param name="indexes">The indexes.</param>
    /// <param name="values">The values.</param>
    public CalibrationArray(short[] indexes, short[] values)
    {
      calibrationPoints = new ArrayList();
      for (int i = 0; i < indexes.Length; i++)
      {
        calibrationPoints.Add(new CalibrationPoint(indexes[i], values[i]));
      }

      if (calibrationPoints == null)
      {
        throw new System.NullReferenceException("Constructor of CalibrationArray expects a non-null ArrayList");
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CalibrationArray"/> class.
    /// </summary>
    /// <param name="calibrationPoints">The calibration points.</param>
    public CalibrationArray(ArrayList calibrationPoints)
    {
      if (calibrationPoints == null)
      {
        throw new System.NullReferenceException("Constructor of CalibrationArray expects a non-null ArrayList");
      }

      this.calibrationPoints = calibrationPoints;
    }

    /// <summary>
    /// Gets the indexes.
    /// </summary>
    public short[] GetIndexes()
    {
      short[] indexes = new short[calibrationPoints.Count];
      for (int i = 0;i < calibrationPoints.Count;i++)
      {
        indexes[i] = ((CalibrationPoint)calibrationPoints[i]).Index;
      }

      return indexes;
    }

    /// <summary>
    /// Gets the values.
    /// </summary>
    public short[] GetValues()
    {
      short[] values = new short[calibrationPoints.Count];
      for (int i = 0; i < calibrationPoints.Count; i++)
      {
        values[i] = ((CalibrationPoint)calibrationPoints[i]).Value;
      }

      return values;
    }

    /// <summary>
    /// Adds the calibration point.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="value">The value.</param>
    public void AddCalibrationPoint(short index, short value)
    {
      // if we have an exact match, then update the value
      var point = ForIndex(index);
      if (point != null)
      {
        point.Value = value;
        point.Index = index;
        return;
      }

      // find where to insert the new index, so that we can keep our array sorted
      var insertionIndex = GetInsertionIndex(index);
      if (insertionIndex == -1)
      {
        calibrationPoints.Add(new CalibrationPoint(index, value));
      }
      else
      {
        calibrationPoints.Insert(insertionIndex, new CalibrationPoint(index, value));
      }
    }

    /// <summary>
    /// Returns the number of calibration points
    /// </summary>
    public int Count()
    {
      return calibrationPoints.Count;
    }

    /// <summary>
    /// Gets the calibration point.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="value">The calculated output value.</param>
    /// <param name="exact">if set to <c>true</c>, the given index must match a stored index.</param>
    /// <returns>True if a valid value could be calculated</returns>
    public bool GetCalibrationPoint(short index, out short value, bool exact = false)
    {
      value = 0;

      if (exact == true)
      {
        var point = ForIndex(index);
        if (point == null)
        {
          return false;
        }
        else
        {
          value = point.Value;
          return true;
        }
      }

      if (calibrationPoints.Count == 0)
      {
        return false;
      }
      else if (calibrationPoints.Count == 1)
      {
        value = ((CalibrationPoint)calibrationPoints[0]).Value;
        return true;
      }
      else
      {
        int i1;
        int i2 = GetInsertionIndex(index);
        if (i2 == -1)
        {
          i2 = calibrationPoints.Count - 1;
          i1 = calibrationPoints.Count - 2;
        }
        else if (i2 == 0)
        {
          i2 = 1;
          i1 = 0;
        }
        else
        {
          i1 = i2 - 1;
        }

        var p2 = (CalibrationPoint)calibrationPoints[i2];
        var p1 = (CalibrationPoint)calibrationPoints[i1];

        float m = (p2.Value - p1.Value) / (p2.Index - p1.Index);

        float y = m * index - m * p1.Index + p1.Value;

        value = (short)y;

        return true;
      }
    }

    private int GetInsertionIndex(short newIndex)
    {
      for (int i = 0; i < calibrationPoints.Count; i++)
      {
        if (((CalibrationPoint)calibrationPoints[i]).Index > newIndex)
        {
          return i;
        }
      }

      return -1;
    }

    private CalibrationPoint ForIndex(short index)
    {
      foreach (CalibrationPoint point in calibrationPoints)
      {
        if (point.Index == index)
        {
          return point;
        }
      }

      return null;
    }
  }
}
