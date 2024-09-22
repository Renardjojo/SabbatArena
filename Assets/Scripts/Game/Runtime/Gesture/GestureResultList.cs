using System;
using System.Collections;

public class GestureResultList
{
    #region NBestResult Inner Class

    public class GestureResult : IComparable
    {
        public static GestureResult Empty = new GestureResult(String.Empty, -1f);

        private string m_Name;
        private float m_Score;

        // constructor
        public GestureResult(string name, float score)
        {
            m_Name = name;
            m_Score = score;
        }

        public string Name { get { return m_Name; } }
        public float Score { get { return m_Score; } }
        public bool IsEmpty { get { return m_Score == -1d; } }

        // sorts in descending order of Score
        public int CompareTo(object obj)
        {
            if (obj is GestureResult)
            {
                GestureResult r = (GestureResult) obj;
                if (m_Score < r.m_Score)
                    return 1;
                else if (m_Score > r.m_Score)
                    return -1;
                return 0;
            }
            else throw new ArgumentException("object is not an NBestResult");
        }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Score);
        }
    }

    #endregion

    #region Fields

    public static GestureResultList Empty = new GestureResultList();
    private ArrayList m_GestureResults;

    #endregion

    #region Constructor & Methods

    public GestureResultList()
    {
        m_GestureResults = new ArrayList();
    }

    public bool IsEmpty
    {
        get
        {
            return m_GestureResults.Count == 0;
        }
    }

    public void AddResult(string name, float score)
    {
        GestureResult r = new GestureResult(name, score);
        m_GestureResults.Add(r);
    }

    public void SortDescending()
    {
        m_GestureResults.Sort();
    }

    #endregion

    #region Top Result

    /// <summary>
    /// Gets the gesture name of the top result of the NBestList.
    /// </summary>
    public string Name
    {
        get
        {
            if (m_GestureResults.Count > 0)
            {
                GestureResult r = (GestureResult) m_GestureResults[0];
                return r.Name;
            }
            return String.Empty;
        }
    }

    /// <summary>
    /// Gets the [0..1] matching score of the top result of the NBestList.
    /// </summary>
    public float Score
    {
        get
        {
            if (m_GestureResults.Count > 0)
            {
                GestureResult r = (GestureResult) m_GestureResults[0];
                return r.Score;
            }
            return -1.0f;
        }
    }

    #endregion

    #region All Results

    public GestureResult this[int index]
    {
        get
        {
            if (0 <= index && index < m_GestureResults.Count)
            {
                return (GestureResult) m_GestureResults[index];
            }
            return null;
        }
    }

    public string[] Names
    {
        get
        {
            string[] s = new string[m_GestureResults.Count];
            if (m_GestureResults.Count > 0)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = ((GestureResult) m_GestureResults[i]).Name;
                }
            }
            return s;
        }
    }

    public double[] Scores
    {
        get
        {
            double[] s = new double[m_GestureResults.Count];
            if (m_GestureResults.Count > 0)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = ((GestureResult) m_GestureResults[i]).Score;
                }
            }
            return s;
        }
    }

    #endregion

    public override string ToString()
    {
        return m_GestureResults[0].ToString();
    }
}
