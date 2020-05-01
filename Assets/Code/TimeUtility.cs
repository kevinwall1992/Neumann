using UnityEngine;
using System.Collections;
using System;

public static class TimeUtility
{
    public static bool IsInPast(this DateTime time)
    {
        return DateTime.Now > time;
    }

    public static bool IsInFuture(this DateTime time)
    {
        return DateTime.Now < time;
    }

    public static float GetCycleMoment(float cycle_length, DateTime start)
    {
        float cycles = (float)(DateTime.Now - start).TotalSeconds / cycle_length;

        return cycles - (int)cycles;
    }

    public static float GetCycleMoment(float cycle_length)
    {
        return GetCycleMoment(cycle_length, LevelLoadDateTime);
    }

    public static float GetLoopedCycleMoment(float cycle_length, DateTime start)
    {
        float moment = GetCycleMoment(cycle_length, start);
        if (moment > 0.5f)
            moment = 0.5f - (moment - 0.5f);

        return moment * 2;
    }

    public static float GetLoopedCycleMoment(float cycle_length)
    {
        return GetLoopedCycleMoment(cycle_length, LevelLoadDateTime);
    }

    public static DateTime LevelLoadDateTime
    { get { return DateTime.Now.AddSeconds(-Time.timeSinceLevelLoad); } }
}
