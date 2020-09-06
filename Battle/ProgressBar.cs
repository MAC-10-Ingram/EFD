using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{

    public GameObject back;
    public GameObject front;
    public float progress;
    public Color color;
    protected Vector3 scale;
    protected Vector3 pos;
    protected float point;

    // Start is called before the first frame update
    protected void Start()
    {
        scale = Vector3.one;
        pos = front.transform.localPosition;
        point =  pos.x;

        front.GetComponent<MeshRenderer>().material.color = color;
    }

    // Update is called once per frame
    protected void Update()
    {
        scale.x = progress;
        pos.x = point + ( -0.5f + progress * 0.5f);
        front.transform.localScale = scale;
        front.transform.localPosition = pos;
    }

    public void setProgress(float prog) {
        if (prog >= 0.0f && prog <= 1.0f) progress = prog;
    }

    public void active() {
        this.gameObject.SetActive(true);
    }

    public void deActive() {
        this.gameObject.SetActive(false);
    }
}



