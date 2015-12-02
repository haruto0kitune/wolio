using UnityEngine;
using System.Collections;

public class Piyo : MonoBehaviour
{
    [SerializeField]
    private int m_hp;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            m_hp--;
        }
    }
}
