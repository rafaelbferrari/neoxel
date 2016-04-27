using UnityEngine;
using System.Collections;

public class ShiledManager : MonoBehaviour {

    public float m_rotationAmount = 5f;
    public float m_time = .3f;
    private float degree;
    private float angle;
    
    void Start()
    {
        OVRTouchpad.Create();
        OVRTouchpad.TouchHandler += HandleTouchHandler;
    }

    void HandleTouchHandler(object sender, System.EventArgs e)
    {
        OVRTouchpad.TouchArgs touchArgs = (OVRTouchpad.TouchArgs)e;
        OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;

        switch (touchEvent)
        {
            case OVRTouchpad.TouchEvent.SingleTap:
                //Do something for Single Tap
                break;

            case OVRTouchpad.TouchEvent.Left:
                StartCoroutine(Rotate(-m_rotationAmount));
                break;

            case OVRTouchpad.TouchEvent.Right:
                StartCoroutine(Rotate(m_rotationAmount));
                break;
        }
    }
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            StartCoroutine(Rotate(m_rotationAmount));
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            StartCoroutine(Rotate(-m_rotationAmount));
    }

    IEnumerator Rotate(float angle)
    {
        float time = 0;
        
        Vector3 targetEulerAngle = new Vector3(270, transform.localEulerAngles.y + angle, 0);
        while(time < m_time)
        {
            time += Time.deltaTime;
            transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, targetEulerAngle, time / m_time);
            yield return new WaitForEndOfFrame();
        }
    }
    
}
