using System.Linq;
using UnityEditor;
using UnityEngine;

public class PathFollow : MonoBehaviour
{
    public float speed;
    public Vector3[] points;

    private int index;

    [HideInInspector] public bool ghost;
    [HideInInspector] public GameObject ghostObj;
    [HideInInspector] public float ghostSpeed;

    private Vector3[] tempPoints;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!ghost)
        {
            points = points.Append(transform.position).ToArray();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (points.Length > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, points[index], Time.deltaTime * speed);
            if (transform.position == points[index])
            {
                index++;
                if (index >= points.Length)
                {
                    index = 0;
                }
            }

            if (ghost)
            {
                speed = transform.parent.GetComponent<PathFollow>().speed;
                points = transform.parent.GetComponent<PathFollow>().points;
                points = points.Append(transform.parent.position).ToArray();
                if (Selection.activeGameObject == null || Selection.activeGameObject != transform.parent.gameObject)
                {
                    DestroyImmediate(gameObject);
                }
            }
        }
    }
}