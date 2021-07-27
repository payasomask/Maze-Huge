using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
  public static FloorManager _FloorManager = null;
  // Start is called before the first frame update
  Sprite floor = null;
  SpriteRenderer floor_sr = null;
  private void Awake(){
    _FloorManager = this;
  }

  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
  public void init(){
    floor = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "brick");
    floor_sr = transform.Find("brick").GetComponent<SpriteRenderer>();
    floor_sr.sprite = floor;
  }

  public void updateSize(float width, float hight){
    if (floor == null)
      return;

    if (floor_sr == null)
      return;

    floor_sr.size = new Vector2(width, hight);
  }
}
