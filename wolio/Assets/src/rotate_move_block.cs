using UnityEngine;
using System.Collections;

public class rotate_move_block : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    [SerializeField]
    private float radius = 1f;
    [SerializeField]
    private string direction = "left";

    float angle = 0f;
    float sin = 1 * Mathf.Sin(30 * Mathf.Deg2Rad);
    float cos = 1 * Mathf.Cos(180 * Mathf.Deg2Rad);

    void Start()
    {
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (true)
        {
            sin = radius * Mathf.Sin(angle * Mathf.Deg2Rad) / 100;
            cos = radius * Mathf.Cos(angle * Mathf.Deg2Rad) / 100;
            transform.Translate(sin, cos, 0);
            if (angle == 360 || angle == -360) angle = 0;
            if (direction == "left") angle--;
            if (direction == "right") angle++;

            yield return null;
        }
    }
}