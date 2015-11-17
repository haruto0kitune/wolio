using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public class camera : MonoBehaviour
{
    // Player sprite
    [SerializeField]
    private GameObject player;

    // Use this for initialization
    void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                this.transform.position = new Vector3(player.transform.position.x, 0, -10);

                if (transform.position.x < 0) transform.position = new Vector3(0, 0, -10);
                else if (transform.position.x >= 12) transform.position = new Vector3(12, 0, -10);
            });
    }
}
