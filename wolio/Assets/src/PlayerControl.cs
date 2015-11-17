using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    private Player m_Character;
    private bool m_Jump;
    private bool m_Attack;
    private bool m_Forwardstep;
    public bool m_Backstep = true;
    private bool m_LeftShift;
    private bool m_RightShift;
    private bool m_left;
    private bool m_right;
    private bool m_oldleft;
    private bool m_oldright;

    private void Awake()
    {
        m_Character = GetComponent<Player>();
    }

    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log(CrossPlatformInputManager.GetAxisRaw("Horizontal"));

                m_oldleft = m_left;
                m_oldright = m_right;

                m_left = CrossPlatformInputManager.GetButton("Left");
                m_right = CrossPlatformInputManager.GetButton("Right");


                if ((m_left || m_right) && m_LeftShift)
                {
                    Debug.Log("left and right");
                    StartCoroutine(m_Character.Backstep(m_left, m_right, m_LeftShift));
                }

                if (!m_Jump)
                {
                    m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }

                if (!m_Attack)
                {
                    m_Attack = CrossPlatformInputManager.GetButtonDown("Attack");
                }

                m_LeftShift = CrossPlatformInputManager.GetButton("Left Shift");
            });

        this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                // Read the inputs.
                float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
                bool shift = CrossPlatformInputManager.GetButtonDown("Left Shift");
                bool left = CrossPlatformInputManager.GetButton("Left");
                bool right = CrossPlatformInputManager.GetButton("Right");

                // Pass all parameters to the character control script.
                if ((left || right) && shift)
                {

                }
                else
                {
                    m_Character.Move(h, m_Jump);
                    m_Jump = false;
                }
            });
    }
}