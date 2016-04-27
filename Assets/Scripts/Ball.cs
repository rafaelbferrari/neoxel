using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    private SphereCollider col;
    public MeshRenderer m_meshRenderer;

    [HideInInspector]
    public BallConfig m_config;

    public bool m_destroyMyself = false;

	// Use this for initialization
	void Start () {
        col = GetComponent<SphereCollider>();
        col.enabled = false;
        Invoke("EnableContainer", .5f);
        if (m_destroyMyself)
            Invoke("Destroy", 5f);
	}

    public void SetupType(BallConfig config)
    {
        m_config = config;
        m_meshRenderer.material.SetColor("_Color", m_config.m_color);
    }

    void EnableContainer()
    {
        col.enabled = true;
    }

    void Destroy()
    {
        Destroy(gameObject);
    }
    
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Enemy")
        {
            //if(GameManager.Instance.IsMyStep(col.gameObject.GetComponent<LevelStep>().levelStep))
            {
                col.gameObject.GetComponent<WalkerSplineBase>().Destroy();
                Explosion.Instance.Explode(transform.position);
                //GameManager.Instance.AddCombo(transform.position);
                ComboManager.Instance.ShowCombo(transform.position, col.gameObject);
                //Destroy(col.gameObject);
                //col.gameObject.SetActive(false);
                Destroy(gameObject);
            }
            /*else
            {
                foreach(Material m in col.gameObject.GetComponent<MeshRenderer>().materials)
                {
                    //m.SetColor("_Color", Color.white);
                    StartCoroutine(ColorLerp(m, Color.white));
                }
                
            }*/
        }
    }
    
    IEnumerator ColorLerp(Material m, Color c)
    {
        float time = 0;

        Color defaultColor = m.GetColor("_Color");

        float delay = .1f;

        while(time < delay)
        {
            time += Time.deltaTime;
            m.SetColor("_Color", Color.Lerp(defaultColor, c, time / delay));

            yield return new WaitForEndOfFrame();
        }
        

        time = 0;

        while (time < delay)
        {
            time += Time.deltaTime;
            m.SetColor("_Color", Color.Lerp(c, defaultColor, time / delay));
            yield return new WaitForEndOfFrame();
        }

    }
}
