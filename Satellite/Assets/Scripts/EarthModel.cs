using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthModel : MonoBehaviour
{

    public DateTime ReferenceTime
    {
        get
        {
            return referenceTime;
        }
        set
        {
            DateTime simTime = value;
            TimeSpan tf = simTime - referenceTime;
            double dif = tf.TotalSeconds;

            Vector3 current = transform.localEulerAngles;
            current.x = (float)(current.x + SatelliteManager.EARTH_ROTATION_SPEED_PER_SECOND * dif) % 360;
            transform.localEulerAngles = current;

            referenceTime = simTime;
        }
    }


    protected DateTime referenceTime = new DateTime(1970,1,1,0,0,0);
   
}
