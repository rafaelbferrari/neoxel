﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CatmullRomSpline : MonoBehaviour
{
    //Has to be at least 4 so-called control points
    public List<Transform> controlPointsList = new List<Transform>();

    public bool isLooping = true;

    //Display without having to press Play
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        //Draw a sphere at each control point
        for (int i = 0; i < controlPointsList.Count; i++)
        {
            Gizmos.DrawSphere(controlPointsList[i].position, 0.3f);
        }

        //Draw the Catmull-Rom lines between the points
        for (int i = 0; i < controlPointsList.Count; i++)
        {
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            //...if we are not making a looping line
            if ((i == 0 || i == controlPointsList.Count - 2 || i == controlPointsList.Count - 1) && !isLooping)
            {
                continue;
            }

            DisplayCatmullRomSpline(i);
        }
    }

    /// <summary>
    /// /////////////////////////////
    /// </summary>
    private List<Vector3> _splinePoints;
    public Transform m_object;
    private int _count = 0;
    public float m_speed = 3f;

   /* void LateUpdate()
    {
        if(m_object != null)
        {
            if(m_object.position == _splinePoints[_count])
            {
                _count++;
                if(_count >= _splinePoints.Count)
                {
                    _count = 0;
                }
            }

            m_object.position = Vector3.MoveTowards(m_object.position, _splinePoints[_count], Time.deltaTime * m_speed); 
            
        }
    }*/

    void Awake()
    {
        SaveSplinePoints();
    }

    private void SaveSplinePoints()
    {
        _splinePoints = new List<Vector3>();


        //Draw the Catmull-Rom lines between the points
        for (int i = 0; i < controlPointsList.Count; i++)
        {
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            //...if we are not making a looping line
            if ((i == 0 || i == controlPointsList.Count - 2 || i == controlPointsList.Count - 1) && !isLooping)
            {
                continue;
            }

            SavePoint(i);
        }
    }

    private void SavePoint(int pos)
    {
        //Clamp to allow looping
        Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].position;
        Vector3 p1 = controlPointsList[pos].position;
        Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].position;
        Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].position;


        //Just assign a tmp value to this
        Vector3 lastPos = Vector3.zero;

        //t is always between 0 and 1 and determines the resolution of the spline
        //0 is always at p1
        for (float t = 0; t < 1; t += 0.1f)
        {
            //Find the coordinates between the control points with a Catmull-Rom spline
            Vector3 newPos = ReturnCatmullRom(t, p0, p1, p2, p3);

            //Cant display anything the first iteration
            if (t == 0)
            {
                lastPos = newPos;
                _splinePoints.Add(lastPos);
                continue;
            }
            
            lastPos = newPos;
            _splinePoints.Add(lastPos);
        }
        
    }

    public List<Vector3> GetSplinePositions()
    {
        return _splinePoints;
    }
    /// <summary>
    /// ///////////////
    /// </summary>
    /// 


    //Returns a position between 4 Vector3 with Catmull-Rom Spline algorithm
    //http://www.iquilezles.org/www/articles/minispline/minispline.htm
    Vector3 ReturnCatmullRom(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 0.5f * (2f * p1);
        Vector3 b = 0.5f * (p2 - p0);
        Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

        Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

        return pos;
    }

    //Display a spline between 2 points derived with the Catmull-Rom spline algorithm
    void DisplayCatmullRomSpline(int pos)
    {
        //Clamp to allow looping
        Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].position;
        Vector3 p1 = controlPointsList[pos].position;
        Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].position;
        Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].position;


        //Just assign a tmp value to this
        Vector3 lastPos = Vector3.zero;

        //t is always between 0 and 1 and determines the resolution of the spline
        //0 is always at p1
        for (float t = 0; t < 1; t += 0.1f)
        {
            //Find the coordinates between the control points with a Catmull-Rom spline
            Vector3 newPos = ReturnCatmullRom(t, p0, p1, p2, p3);

            //Cant display anything the first iteration
            if (t == 0)
            {
                lastPos = newPos;
                continue;
            }
            
            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }

        //Also draw the last line since it is always less than 1, so we will always miss it
        Gizmos.DrawLine(lastPos, p2);
    }


    //Clamp the list positions to allow looping
    //start over again when reaching the end or beginning
    int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = controlPointsList.Count - 1;
        }

        if (pos > controlPointsList.Count)
        {
            pos = 1;
        }
        else if (pos > controlPointsList.Count - 1)
        {
            pos = 0;
        }

        return pos;
    }
}