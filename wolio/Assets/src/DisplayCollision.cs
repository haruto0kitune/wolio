using UnityEngine;
using System.Collections;

public class DisplayCollision : MonoBehaviour
{
    BoxCollider2D[] BoxColliders2D;
    CircleCollider2D[] CircleColliders2D;

    // Use this for initialization
    void Start()
    {
        BoxColliders2D = GetComponents<BoxCollider2D>();
        CircleColliders2D = GetComponents<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        DrawCollider();
    }

    void DrawCollider()
    {
        DrawBoxCollider();
        DrawCircleCollider();
    }

    void DrawBoxCollider()
    {
        Vector3[] square = new Vector3[4];

        foreach(var i in BoxColliders2D)
        {
            square[0] = new Vector3(i.bounds.min.x, 0.0f, i.bounds.min.z);
            square[1] = new Vector3(i.bounds.max.x, 0.0f, i.bounds.min.z);
            square[2] = new Vector3(i.bounds.max.x, 0.0f, i.bounds.max.z);
            square[3] = new Vector3(i.bounds.min.x, 0.0f, i.bounds.max.z);

            for (int j = 0; j < 4;j++)
            {
                Debug.DrawLine(square[j], square[(j + 1) % 4], Color.black, 0.0f, false);
            }
        }
    }

    void DrawCircleCollider()
    {

    }
}
