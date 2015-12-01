using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Player))]
public class PlayerControl : MonoBehaviour
{
    private Player m_Character;
    private float m_Direction;
    private bool m_Jump;
    private bool m_Attack;
    private bool m_Dash;
    private bool m_LeftShift;
    private bool m_RightShift;
    private bool m_left;
    private bool m_right;
   
    private void Awake()
    {
        m_Character = GetComponent<Player>();
    }

    private void Start()
    {
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                m_left = CrossPlatformInputManager.GetButton("Left");
                m_right = CrossPlatformInputManager.GetButton("Right");
                m_Direction = CrossPlatformInputManager.GetAxisRaw("Horizontal");

                if ((CrossPlatformInputManager.GetButton("Left") || CrossPlatformInputManager.GetButton("Right")) && CrossPlatformInputManager.GetButton("Left Shift"))
                {
                    m_Dash = true;
                }
                else
                {
                    m_Dash = false;
                }

                if (!m_Jump)
                {
                    m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
                }

                m_Attack = CrossPlatformInputManager.GetButton("Attack");

                m_LeftShift = CrossPlatformInputManager.GetButton("Left Shift");
            });

        this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                if (m_Attack)
                {
                    m_Character.FireBall();
                }

                if ((m_Dash || m_Character.m_isDashing) && m_Character.m_Grounded)
                {
                    m_Character.Dash(m_Direction, m_LeftShift);
                }
                else
                {
                    m_Character.TurnAround(m_Direction);
                    m_Character.Run(m_Direction);
                    m_Character.Jump(m_Jump);
                    m_Jump = false;
                }
            });
    }
}