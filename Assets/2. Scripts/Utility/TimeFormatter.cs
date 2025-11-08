using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeFormatter
{
    /// <summary>
    /// 플레이 타임을 00:00:00 포멧으로 변경
    /// </summary>
    /// <param name="playTime"></param>
    /// <returns>포멧팅된 시간을 반환</returns>
    public static string FormatPlayTime(float playTime)
    {
        TimeSpan t = TimeSpan.FromSeconds(playTime);
        int totalHours = (int)t.TotalHours;
        return string.Format("{0:D2}:{1:D2}:{2:D2}", totalHours, t.Minutes, t.Seconds);
    }

    /// <summary>
    /// DateTime을 UnixTime으로 전환
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>UnixTime을 반환</returns>
    public static long GetUnixTimestamp(DateTime dateTime)
    {
        return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
    }

    /// <summary>
    /// UnixTime을 DataTime으로 전환
    /// </summary>
    /// <param name="unixTimestamp"></param>
    /// <returns>DateTime을 반환</returns>
    public static string GetDateTimeToString(long unixTimestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp)
        .ToLocalTime()                                                      // 로컬 시간 적용
        .ToString("yyyy/MM/dd HH:mm:ss");
    }
}
