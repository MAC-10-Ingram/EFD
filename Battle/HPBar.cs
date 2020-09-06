using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HPBar : ProgressBar
{
    public GameObject followBar;
    float follow;
    public Color followColor;

    private void Start()
    {
        base.Start();


        followBar.GetComponent<MeshRenderer>().material.color = followColor;
        follow = progress;
    }

    void Update()
    {
        base.Update();

        if (follow <= progress)
        {
            follow = progress;
        }
        else
        {
            follow -= Time.deltaTime * 0.25f;
        }

        scale.x = follow;
        pos.x = point + (-0.5f + follow * 0.5f);
        followBar.transform.localScale = scale;
        followBar.transform.localPosition = pos;

    }
}
