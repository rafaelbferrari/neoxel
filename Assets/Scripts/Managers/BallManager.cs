using UnityEngine;
using System.Collections;

public class BallManager : MonoBehaviour
{
    public GameObject m_ball;
    public Transform m_player;

    [SerializeField]
    public BallConfig[] m_ballConfig;

    private ArrayList _throwBalls;

    private static BallManager _instance;
    public static BallManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<BallManager>();
            }

            return _instance;
        }
    }


    // Use this for initialization
    void Start ()
    {

        GetThrowBalls();
        StartCoroutine(Init());
    }

    private void GetThrowBalls()
    {
        _throwBalls = new ArrayList(GameObject.FindObjectsOfType<ThrowBall>());
    }

    IEnumerator Init()
    {
        yield return new WaitForSeconds(Random.Range(GameManager.Instance.m_startRangeDelayToThrow, GameManager.Instance.m_endRangeDelayToThrow));

        ThrowBall t = GetRandomThrowBall();


        GameObject ball = Instantiate(m_ball) as GameObject;
        ball.SetActive(true);
        ball.GetComponent<Ball>().SetupType(GetRandomSetup());
        ball.GetComponent<Ball>().m_destroyMyself = true;
        //t.Init(m_player);
        if (t != null)
        {
            //t.Throw(GameManager.Instance.m_ligthTimeToThrow, ball);
        }

        StartCoroutine(Init());
    }

    private BallConfig GetRandomSetup()
    {
        return m_ballConfig[Random.Range(0, m_ballConfig.Length)];
    }

    private ThrowBall GetRandomThrowBall()
    {
        return _throwBalls[(int)Random.Range(0, _throwBalls.Count - 1)] as ThrowBall;
    }

    public Color GetColorByType(BallConfig.BallType type)
    {
        foreach(BallConfig c in m_ballConfig)
        {
            if(c.m_type == type)
            {
                return c.m_color;
            }
        }

        return Color.black;
    }
}

[System.Serializable]
public class BallConfig
{
    public enum BallType
    {
        TYPE_A,
        TYPE_B
    }

    public BallType m_type;
    public Color m_color;
}
