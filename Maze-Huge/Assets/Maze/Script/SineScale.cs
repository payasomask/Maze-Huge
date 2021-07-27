using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineScale : MonoBehaviour
{
  public float rate = 1.0f;
  float time = 0.0f;
  float power = 1f;
  float start;
  private void Start(){
    start = transform.localScale.x;
    power = start * 0.1f;
  }
  // Update is called once per frame
  void Update(){
    time += Time.deltaTime * rate;
    float sin =  Mathf.Sin(time * rate) * power;
    float sinscale = start + sin;
    transform.localScale = new Vector3(sinscale, sinscale, 1.0f);

      //Debug.Log("sin : " + sin);
    }

  public void setScale(float scale){
    start = scale;
    power = start * 0.1f;
  }
}
