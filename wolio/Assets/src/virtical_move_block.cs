using UnityEngine;
using System.Collections;

public class virtical_move_block : MonoBehaviour {

    Transform m_transform;
    
    [SerializeField]
    private float m_movement = 0.01f;

    void Start()
    {
        m_transform = GetComponent<Transform>();
        StartCoroutine("Move");
    }
 
    IEnumerator Move()
    {
        while (true)
        {
            // move up
            for (int i = 0; i < 200; i++)
            {
                m_transform.Translate(Vector3.up * m_movement);
                yield return null;
            }

            // move down
            for (int i = 0; i < 200; i++)
            {
                m_transform.Translate(Vector3.down * m_movement);
                yield return null;
            }
        }
    }
}