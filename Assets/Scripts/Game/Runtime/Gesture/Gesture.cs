using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Gesture
{
    [Serializable]
    public struct TimedPoint
    {
        public Vector3 Point;
        public float Time;

        public TimedPoint(Vector3 point, float time)
        {
            Point = point;
            Time = time;
        }
    }

    public List<TimedPoint> TimedPoints;
    public float ErrorThreashold;

    public Gesture(float errorThreashold = 0.2f, int count = 0)
    {
        TimedPoints = new List<TimedPoint>(count);
        ErrorThreashold = errorThreashold;
    }
}
