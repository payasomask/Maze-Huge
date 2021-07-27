using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeBoxController : MonoBehaviour
{
  private int currentx, currenty;
  private int maskid;
  public void init(int currentx, int currenty, float maze_size)
  {
    this.currentx = currentx;
    this.currenty = currenty;
    float maskscale = 3.0f;
    //gameObject.transform.localScale = new Vector3(maze_size, maze_size, 1.0f);
    GameObject icon_go = transform.Find("Icon").gameObject;
    Sprite icon = icon_go.GetComponent<SpriteRenderer>().sprite;
    if(icon != null){
      float iconscale = maze_size / icon.bounds.size.x;//根據圖資重新計算scale大小
      icon_go.transform.localScale = new Vector3(iconscale, iconscale, 0.0f);
    }
    maskid = MaskManager._MaskManager.AddMask(transform, "box", maskscale * maze_size);
  }
}
