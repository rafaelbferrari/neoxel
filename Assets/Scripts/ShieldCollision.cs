using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShieldCollision : MonoBehaviour
{
    public BallConfig.BallType m_ballType;
    public Animation m_animation;
    public float force = 30f;
    public GameObject m_hit;

    private Image m_image;

    public Sprite m_breaking;
    public Sprite m_broke;

    private int life = 2;

    void Start()
    {
        m_image = transform.parent.GetComponent<Image>();
        m_image.color = BallManager.Instance.GetColorByType(m_ballType);
    }

    void OnCollisionEnter(Collision col)
    {
        Ball b = col.gameObject.GetComponent<Ball>();
        if(b != null)
        {
            if(b.m_config.m_type == m_ballType && life > 0)
            {
                Instantiate(m_hit, col.contacts[0].point, new Quaternion());
                //col.gameObject.GetComponent<Rigidbody>().velocity *= 3f;
                col.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * force * Time.deltaTime);
                Debug.Log(col.gameObject.GetComponent<Rigidbody>().velocity);
                m_animation.Play();
            }
            else
            {
                if(life > 0)
                {
                    life--;

                    if(life == 1)
                    {
                        m_image.sprite = m_breaking;
                    }
                    else
                    {
                        m_image.sprite = m_broke;
                    }
                }
            }

        }

    }
}
