using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer
{
    protected Dictionary<string, Layer> childLayers;
    protected Dictionary<string, BaseSatelliteModel> satellite;
    public bool storeSatellite;


    public Layer(bool hasSatellite)
    {
        satellite = new Dictionary<string, BaseSatelliteModel>();
        childLayers = new Dictionary<string, Layer>();
        storeSatellite = hasSatellite;
    }


    public void addObject(string name, object newObject)
    {
        if (storeSatellite)
        {
            satellite.Add(name, (BaseSatelliteModel)newObject);      
        }
        else
        {
            childLayers.Add(name, (Layer)newObject);
        }
    }

    public object getObject(string name)
    {
        if (storeSatellite)
        {
            return satellite[name];
        }

        return childLayers[name];
    }

    public List<string> getKeys()
    {
        List<string> names = new List<string>();
        if (storeSatellite)
        {
            foreach(string key  in satellite.Keys)
            {
                names.Add(key);
            }
        }
        else
        {
            foreach (string key in childLayers.Keys)
            {
                names.Add(key);
            }
        }

        return names;
    }

    public void setOutline(bool outlineEnable)
    {
        if (storeSatellite)
        {
            foreach(BaseSatelliteModel sat in satellite.Values)
            {
                sat.gameObject.GetComponent<Outline>().enabled = outlineEnable;
            }    
        }
        else
        {
            foreach (Layer layer in childLayers.Values)
            {
                layer.setOutline(outlineEnable);
            }
        }
    }
}
