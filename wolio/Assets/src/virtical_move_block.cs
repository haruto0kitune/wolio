using UnityEngine;
using System.Collections;

public class virtical_move_block : MonoBehaviour
{
    [SerializeField]
    private float movement = 0.01f;

    void Start()
    {
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (true)
        {
            // move up
            for (int i = 0; i < 200; i++)
            {
                base.transform.Translate(Vector3.up * movement);
                yield return null;
            }

            // move down
            for (int i = 0; i < 200; i++)
            {
                base.transform.Translate(Vector3.down * movement);
                yield return null;
            }
        }
    }
}