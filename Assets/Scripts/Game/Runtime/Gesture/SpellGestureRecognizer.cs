using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SpellGestureRecognizer
{
    List<SpellDescriptor> m_Spells = new();

    public void LoadGesture(SpellDescriptor spell)
    {
        if (spell.Gesture.TimedPoints.Any())
            m_Spells.Add(spell);
    }

#if UNITY_EDITOR
    public void SaveGesture(string path, Gesture gesture)
    {
        string jsonData = JsonUtility.ToJson(gesture);
        File.WriteAllText(path, jsonData);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif

    public GestureResultList Recognize(List<Gesture.TimedPoint> timedPositions, float maxDistance)
    {
        float sqrMaxDistance = maxDistance * maxDistance;
        GestureResultList rstList = new GestureResultList();

        foreach (SpellDescriptor spell in m_Spells)
        {
            float score = 0f;
            int maxIteration = Mathf.Min(timedPositions.Count, spell.Gesture.TimedPoints.Count);
            for (int i = 0; i < maxIteration; i++)
            {
                Gesture.TimedPoint targetTimedPosition = spell.Gesture.TimedPoints[i];
                if (Vector3.SqrMagnitude(targetTimedPosition.Point - timedPositions[i].Point) < sqrMaxDistance)
                {
                    score += 1;
                }
            }

            rstList.AddResult(spell.SpellName, score / spell.Gesture.TimedPoints.Count);
        }

        rstList.SortDescending();
        return rstList;
    }
}
