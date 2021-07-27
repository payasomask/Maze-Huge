using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchManager : MonoBehaviour
{
  public static TorchManager _TorchManager = null;

  int torchid = 0;
  class Torch{
    public GameObject gameobj = null;
    public int id;
  }

  Dictionary<int, Torch> torch_dic = new Dictionary<int, Torch>();

  private void Awake(){
    _TorchManager = this;
  }


  //基礎照亮範圍
  float basic_Light_Radius = 3.0f;

  public void PlaceTorch(Vector2 position, float scale){

    Torch tmpT = new Torch();

    GameObject tmp = instantiateObject(gameObject, "Torch");
    GameObject icon_go = tmp.transform.Find("Icon").gameObject;
    Sprite icon = icon_go.GetComponent<SpriteRenderer>().sprite;

    if (icon != null)
    {
      float iconscale = scale / icon.bounds.size.x;//根據圖資重新計算scale大小
      icon_go.transform.localScale = new Vector3(iconscale, iconscale, 0.0f);
    }

    tmp.transform.localPosition = position;

    GameObject mask_go = tmp.transform.Find("mask").gameObject;
    Sprite mask = mask_go.GetComponent<SpriteMask>().sprite;

    if (mask != null){
      float iconscale = scale / mask.bounds.size.x;//根據圖資重新計算scale大小
      tmp.transform.Find("mask").localScale = new Vector3(basic_Light_Radius * iconscale, basic_Light_Radius * iconscale, 1.0f);
    }

    SineScale ss = tmp.transform.Find("mask").gameObject.AddComponent<SineScale>();
    tmpT.gameobj = tmp;
    tmpT.id = torchid;
    torchid++;
    torch_dic.Add(tmpT.id, tmpT);

  }

  public void ClearAllTorch(){
    foreach(var v in torch_dic){
      Destroy(v.Value.gameobj);
    }
    torch_dic = new Dictionary<int, Torch>();
  }

  GameObject instantiateObject(GameObject parent, string name)
  {
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }
}
