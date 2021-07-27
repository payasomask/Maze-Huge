using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家使用道具管理
public class PlayerItemManager : MonoBehaviour
{
  public static PlayerItemManager _PlayerItemManager = null;
  private void Awake(){
    _PlayerItemManager = this;
  }

    // Update is called once per frame
    void Update(){

    }

  public void UseTorch(Vector2 position){
    float scale = MazeManager._MazeManager.getCellSize();
    TorchManager._TorchManager.PlaceTorch(position, scale);
    //PlayerPrefsManager._PlayerPrefsManager.Item1Num--;
  }

  public void UseOilLamp(){
    MazeManager._MazeManager.StartLineRender();
    //float playerscale = MazeManager._MazeManager.PlayerMaskScale();
    //float mazescale = MazeManager._MazeManager.getCellSize();
    //MaskManager._MaskManager.SetMaskScale("player", playerscale * oillampmaskscale * mazescale);
    //PlayerPrefsManager._PlayerPrefsManager.Item2Num--;
  }
  public void UseStaff()
  {
    MaskManager._MaskManager.HideBlack("black");

  }
}
