using UnityEngine;
using System.Collections;

public class slanting_move_block : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.01f;
    [SerializeField]
    private float volume = 100;
    [SerializeField]
    private string direction = "left";

    // Use this for initialization
    void Start()
    {
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (true)
        {
            switch (direction)
            {
                case "left":
                    {
                        // move left
                        for (int i = 0; i < volume; i++)
                        {
                            transform.Translate(Vector3.up * speed);
                            transform.Translate(Vector3.left * speed);
                            yield return null;
                        }

                        for (int i = 0; i < volume; i++)
                        {
                            transform.Translate(Vector3.down * speed);
                            transform.Translate(Vector3.right * speed);
                            yield return null;
                        }

                        break;
                    }
                case "right":
                    {
                        // move right
                        for (int i = 0; i < volume; i++)
                        {
                            transform.Translate(Vector3.up * speed);
                            transform.Translate(Vector3.right * speed);
                            yield return null;
                        }

                        for (int i = 0; i < volume; i++)
                        {
                            transform.Translate(Vector3.down * speed);
                            transform.Translate(Vector3.left * speed);
                            yield return null;
                        }

                        break;
                    }
            }
        }
    }
}