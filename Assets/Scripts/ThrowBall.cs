using UnityEngine;
using System.Collections;

public class ThrowBall : MonoBehaviour
{
    
    [Range(0f,2f)]
    public float forceMultiply = 1f;
    
    public void BallThrow(GameObject ballToThrow, Transform target)
    {
        GameObject ball = Instantiate(ballToThrow, transform.position, transform.rotation) as GameObject;
        ball.transform.SetParent(transform);
        ball.transform.position = transform.position;
        ball.transform.rotation = transform.rotation;
        ball.GetComponent<Rigidbody>().velocity = BallisticVel(target);
        Debug.Log(ball.GetComponent<Rigidbody>().velocity);
    }
    
    Vector3 BallisticVel(Transform target)
    {
        var dir = target.position - transform.position;  // get target direction
        var h = dir.y;  // get height difference
        dir.y = target.position.y - transform.position.y;  // retain only the horizontal direction
        var dist = dir.magnitude;  // get horizontal distance
        var a = 45f * Mathf.Deg2Rad;  // convert angle to radians
        dir.y = dist * Mathf.Tan(a);  // set dir to the elevation angle
        dist += h / Mathf.Tan(a);  // correct for small height differences
                                   // calculate the velocity magnitude
        var vel = Mathf.Sqrt(dist * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return (vel * dir.normalized)* forceMultiply;
    }
 
}
