using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BZipTarget : MonoBehaviour
{
    public LineRenderer lineRenderer;
    
    public void UpdateLineRenderer(Vector3 swingPos)
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, swingPos);
    }
}
