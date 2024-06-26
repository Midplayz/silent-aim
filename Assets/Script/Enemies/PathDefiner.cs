using UnityEngine;

public class PathDefiner : MonoBehaviour
{
    #region Basic Straight Line Following
    //private void OnDrawGizmos()
    //{
    //    if (waypoints == null || waypoints.Length < 2)
    //        return;

    //    Gizmos.color = Color.green;
    //    for (int i = 0; i < waypoints.Length - 1; i++)
    //    {
    //        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
    //    }
    //}
    #endregion
    
    public Transform[] waypoints;

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2)
            return;

        Gizmos.color = Color.green;

        Vector3 previousPoint = waypoints[0].position;
        int resolution = 20; // if curve not smooth increase this thing

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                float t = (float)j / (resolution - 1);
                Vector3 point = GetCatmullRomPosition(t, i);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }
    }

    private Vector3 GetCatmullRomPosition(float t, int i)
    {
        Vector3 p0 = waypoints[ClampIndex(i - 1)].position;
        Vector3 p1 = waypoints[i].position;
        Vector3 p2 = waypoints[ClampIndex(i + 1)].position;
        Vector3 p3 = waypoints[ClampIndex(i + 2)].position;

        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }

    private int ClampIndex(int index)
    {
        if (index < 0)
            index = 0;
        if (index > waypoints.Length - 1)
            index = waypoints.Length - 1;
        return index;
    }
}
