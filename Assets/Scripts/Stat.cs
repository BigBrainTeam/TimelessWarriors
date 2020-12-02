using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Stat 
{
    private BarScript bar;
    [SerializeField]
    private float maxVal;

    public BarScript Bar
    {
        get { return bar; }
        set
        {
            bar = value;
        }
    }

    public float MaxVal
    {
        get { return maxVal; }
        set 
        {           
            maxVal = value;
            if (bar != null) bar.MaxValue = value;
        }
    }
    [SerializeField]
    private float currentVal;

    public float CurrentVal
    {
        get { return currentVal; }
        set 
        {          
            currentVal = Mathf.Clamp(value,0,MaxVal);
            if(bar!=null) bar.Value = currentVal;
        }
    }

    public void Initialize()
    {
        this.MaxVal = maxVal;
        this.CurrentVal = currentVal;
    }
}
