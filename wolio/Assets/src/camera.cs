using UnityEngine;
using UnityEngine.UI;
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
                if (0 > player.transform.position.x && 0 > player.transform.position.y)
                {
                    transform.position = new Vector3(0, 0, -10);
                }
                else if (0 > player.transform.position.x && 0 <= player.transform.position.y)
                {
                    transform.position = new Vector3(0, player.transform.position.y, -10);
                }
                else if ((0 < player.transform.position.x && player.transform.position.x <= 12) && 0 > player.transform.position.y)
                {
                    transform.position = new Vector3(player.transform.position.x, 0, -10);
                }
                else if ((0 < player.transform.position.x && player.transform.position.x <= 12) && (0 < player.transform.position.y && player.transform.position.y <= 11))
                {
                    transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
                }
                else if (12 <= player.transform.position.x && player.transform.position.y > 0)
                {
                    transform.position = new Vector3(12, player.transform.position.y, -10);
                }
                else if (player.transform.position.x >= 12 && player.transform.position.y <= 0)
                {
                    transform.position = new Vector3(12, 0, -10);
                }
            });
    }
}
