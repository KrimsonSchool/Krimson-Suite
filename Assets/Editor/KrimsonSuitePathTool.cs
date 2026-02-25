using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathFollow))]
public class KrimsonSuitePathTool : Editor
{
    private GameObject selected;
    private Vector3[] points;

    private readonly float _ghostSpeed = 25;

    private void OnSceneGUI()
    {
        GUIStyle redText = new GUIStyle
        {
            normal =
            {
                textColor = Color.red
            }
        };

        GUIStyle greenText = new GUIStyle
        {
            normal =
            {
                textColor = Color.green
            }
        };


        selected = Selection.activeGameObject;
        if (selected != null)
        {
            SceneView.RepaintAll();
            if (points != null)
            {
                if (selected.GetComponent<PathFollow>().ghostObj == null && !selected.GetComponent<PathFollow>().ghost && points.Length > 0)
                {
                    selected.GetComponent<PathFollow>().ghostObj = Instantiate(selected, selected.transform);
                    selected.GetComponent<PathFollow>().ghostObj.transform.position = Vector3.zero;
                    if (selected.GetComponent<PathFollow>().ghostObj.GetComponent<Collider>() != null)
                    {
                        DestroyImmediate(selected.GetComponent<PathFollow>().ghostObj.GetComponent<Collider>());
                    }

                    selected.GetComponent<PathFollow>().ghostObj.GetComponent<PathFollow>().ghost = true;
                    selected.GetComponent<PathFollow>().ghostObj.GetComponent<PathFollow>().ghostSpeed = _ghostSpeed;
                    selected.GetComponent<PathFollow>().ghostObj.GetComponent<PathFollow>().runInEditMode = true;
                    selected.GetComponent<PathFollow>().ghostObj.GetComponent<Renderer>().sharedMaterial =
                        Resources.Load("Materials/HOLOGRAM", typeof(Material)) as Material;
                    //selected.GetComponent<PathFollow>().ghostObj.GetComponent<Renderer>().sharedMaterial.color = new Color(0, 0.3f, 1, 0.3f);
                    //selected.GetComponent<PathFollow>().ghostObj.GetComponent<Renderer>().sharedMaterial.SetFloat(" _Surface", 0);
                }
            }



            points = selected.GetComponent<PathFollow>().points;
            if (points != null)
            {
                if (selected.GetComponent<PathFollow>().points.Length > 0)
                {
                    for (int i = 0; i < points.Length; i++)
                    {
                        points[i] = Handles.PositionHandle(points[i], Quaternion.identity);
                        Handles.DrawSolidDisc(points[i], Vector3.up, 0.1f);

                        if (i > 0)
                        {
                            Vector3 average = (points[i - 1] + points[i]) / 2;
                            Handles.Label(average, Vector3.Distance(points[i - 1], points[i]) + "m");
                            Handles.DrawDottedLine(points[i - 1], points[i], 4);
                        }
                        else
                        {
                            Handles.DrawDottedLine(Selection.activeTransform.position, points[i], 4);
                        }
                    }

                    float size = 0.5f;
                    Handles.color = Color.green;
                    Handles.Label(points[^1] + Vector3.up * size, "Add Another", greenText);
                    if (Handles.Button(points[^1], Quaternion.identity, size, size / 2, Handles.SphereHandleCap))
                    {
                        selected.GetComponent<PathFollow>().points = points.Append(new Vector3(points[^1].x + 5, points[^1].y, points[^1].z)).ToArray();
                    }

                    if (points.Length > 1)
                    {
                        Handles.color = Color.red;
                        Handles.Label(points[^2] + Vector3.up * size, "Remove Last", redText);
                        if (Handles.Button(points[^2], Quaternion.identity, size, size / 2, Handles.SphereHandleCap))
                        {
                            selected.GetComponent<PathFollow>().points = points.SkipLast(1).ToArray();
                        }
                    }
                }
            }
        }
    }
}