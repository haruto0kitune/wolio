using UnityEngine;
using System.Collections;

public class side_move_block : MonoBehaviour
{
    Rigidbody2D rigidbody2d;

    [SerializeField]
    private float speed = 0.01f;
    [SerializeField]
    private float volume = 100;

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (true)
        {
            // move left
            for (int i = 0; i < volume; i++)
            {
                transform.Translate(Vector3.left * speed);
                yield return null;
            }

            // move right
            for (int i = 0; i < volume; i++)
            {
                transform.Translate(Vector3.right * speed);
                yield return null;
            }
        }
    }
}
