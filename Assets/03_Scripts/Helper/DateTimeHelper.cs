using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Static class contain datetime math and whatnot
/// </summary>
public static class DateTimeHelper
{
    public static DateTime ConvertStringToDateTime(string _value)
    {
        string[] newDate = _value.Split('-');

        int year = int.Parse(newDate[0]);
        int month = int.Parse(newDate[1]);
        int day = int.Parse(newDate[2]);
        return new DateTime(year, month, day);
    }

    /// <summary>
    /// Returns age in years
    /// </summary>
    /// <returns>Age in int</returns>
    public static int GetYears(int year, int month, int day)
    {
        DateTime zeroTime = new DateTime(1, 1, 1);
        DateTime now = DateTime.Now;
        DateTime compareTime = new DateTime(year, month, day);

        TimeSpan span = now - compareTime;

        return (zeroTime + span).Year - 1;
    }


    /// <summary>
    /// Returns the differnce in days of a date vs now
    /// </summary>

    public static int GetDays(int year, int month, int day)
    {
        DateTime now = DateTime.Now;
        DateTime compareTime = new DateTime(year, month, day);

        TimeSpan span = now.Subtract(compareTime);

        return (int)(span.Days);
    }
}
