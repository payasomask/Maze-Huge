using UnityEngine;
using System.Collections.Generic;
 
[RequireComponent(typeof(Camera))]
public class ConfigCamera : MonoBehaviour
{
  public enum ScalePolicy
  {
    SHOW_ALL,
    EXACT_FIT,
    FIXED_WIDTH,
    FIXED_HEIGHT,
    NO_BORDER,
    STRETCH
  }
 
  public float desiredWidth;
  public float desiredHeight;
  public float pixelsToUnits;
  public ScalePolicy scalePolicy;

  private int currentScreenWidth =0;
  private int currentScreenHeight =0;

  Camera cam =null;

  void Start()
  {
    currentScreenHeight =Screen.height;
    currentScreenWidth =Screen.width;

    updateCameraSetup();

  }

  void Update(){
    if (currentScreenWidth !=Screen.width || currentScreenHeight !=Screen.height){
      currentScreenHeight =Screen.height;
      currentScreenWidth =Screen.width;

      updateCameraSetup();
    }

  }

  public void updateCameraSetup(){
    if (cam ==null)
      cam =GetComponent<Camera>();

    if (scalePolicy == ScalePolicy.SHOW_ALL)
      return;
     
    float desiredRatio = desiredWidth / desiredHeight;
    float currentRatio = (float)Screen.width / (float)Screen.height;
    float differenceInSize = desiredRatio / currentRatio;
    float orthographicSize = desiredHeight/2/pixelsToUnits;
     
     
    // Current width is lager than desired ratio.
    if (currentRatio >= desiredRatio){
      if (scalePolicy == ScalePolicy.FIXED_HEIGHT){
        orthographicSize = orthographicSize * differenceInSize;
      }
    }else{
      if (scalePolicy == ScalePolicy.FIXED_WIDTH){
        orthographicSize = orthographicSize * differenceInSize;
      }
    }

    if (scalePolicy == ScalePolicy.EXACT_FIT){
      cam.aspect = desiredRatio;
      cam.orthographicSize = orthographicSize;

      if (currentRatio>=desiredRatio){
        float wide_scale =desiredWidth/(float)Screen.width*(Screen.height/desiredHeight);
        float shift =(Screen.width-wide_scale*Screen.width)*0.5f;
        cam.rect =new Rect(shift/Screen.width, 0f, wide_scale, 1f);
      }else{
        float height_scale =desiredHeight/(float)Screen.height*(Screen.width/desiredWidth);
        float shift =(Screen.height-height_scale*Screen.height)*0.5f;
        cam.rect =new Rect(0f, shift/Screen.height, 1f, height_scale);

      }

    }else if (scalePolicy == ScalePolicy.STRETCH){
      cam.aspect = desiredRatio;
    }else{
      cam.orthographicSize = orthographicSize;
    }
  }
}