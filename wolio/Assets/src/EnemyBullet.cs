using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class EnemyBullet : MonoBehaviour
{
    private GameObject Camera;
    private Piyo Piyo;
    private ReactiveProperty<bool> FacingRight;

    void Awake()
    {
        Camera = GameObject.Find("Main Camera");
        Piyo = GameObject.FindGameObjectWithTag("Piyo").GetComponent<Piyo>();
        FacingRight = new ReactiveProperty<bool>(Piyo.FacingRight.Value);
    }

    void Start()
    {
        this.UpdateAsObservable()
            .Where(x => !FacingRight.Value)
            .Subscribe(_ => LeftMove());

        this.UpdateAsObservable()
            .Where(x => FacingRight.Value)
            .Subscribe(_ => RightMove());

        this.UpdateAsObservable()
            .Where(x => (Camera.transform.position.x + 8.3) <= transform.position.x || (Camera.transform.position.x - 8.3) >= transform.position.x)
            .Subscribe(_ => Destroy(gameObject));

        this.OnTriggerEnter2DAsObservable()
            .Where(x => (x.gameObject.layer == LayerMask.NameToLayer("Player")) || (x.gameObject.tag == "Obstacle"))
            .Subscribe(_ => Destroy(gameObject));
    }

    void LeftMove()
    {
        transform.position = new Vector3(transform.position.x - 0.1f, transform.position.y, transform.position.z);
    }

    void RightMove()
    {
        transform.position = new Vector3(transform.position.x + 0.1f, transform.position.y, transform.position.z);
    }
}
