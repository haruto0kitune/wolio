using UnityEngine;
using System.Collections;

public class slanting_move_block : MonoBehaviour
{

    private Transform m_transform;

    [SerializeField]
    private float m_speed = 0.01f;
    [SerializeField]
    private float m_volume = 100;
    [SerializeField]
    private string m_direction = "left";

    // Use this for initialization
    void Start()
    {
        m_transform = GetComponent<Transform>();
        StartCoroutine("Move");
    }

    IEnumerator Move()
    {
        while (true)
        {
            switch (m_direction)
            {
                case "left":
                    {
                        // move left
                        for (int i = 0; i < m_volume; i++)
                        {
                            m_transform.Translate(Vector3.up * m_speed);
                            m_transform.Translate(Vector3.left * m_speed);
                            yield return null;
                        }

                        for (int i = 0; i < m_volume; i++)
                        {
                            m_transform.Translate(Vector3.down * m_speed);
                            m_transform.Translate(Vector3.right * m_speed);
                            yield return null;
                        }

                        break;
                    }
                case "right":
                    {
                        // move right
                        for (int i = 0; i < m_volume; i++)
                        {
                            m_transform.Translate(Vector3.up * m_speed);
                            m_transform.Translate(Vector3.right * m_speed);
                            yield return null;
                        }

                        for (int i = 0; i < m_volume; i++)
                        {
                            m_transform.Translate(Vector3.down * m_speed);
                            m_transform.Translate(Vector3.left * m_speed);
                            yield return null;
                        }

                        break;
                    }
            }
        }
    }
}