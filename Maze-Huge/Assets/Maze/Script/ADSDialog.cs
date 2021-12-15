using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ADSDialog :  IDialogContext
{

  public ADSDialog(ADReward reward, InteractiveDiaLogHandler[] handlers){
    mreward = reward;
    bt_handlers = handlers;
  }
  bool binited = false;
  GameObject dlgGO = null;
  ADReward  mreward;
  InteractiveDiaLogHandler[] bt_handlers;
  int adreward;
  public bool dismiss()
  {
    GameObject.Destroy(dlgGO);
    return true;
  }

  public DialogEscapeType getEscapeType()
  {
    return DialogEscapeType.NOTHING;
  }

  public DialogType getType()
  {
    return DialogType.NORMAL;
  }

  public GameObject init(int dlgIdx, AssetbundleLoader abl){
      dlgGO = abl.InstantiatePrefab("ADSDialog");
      binited = true;

    string skipspritename = mreward.SkipType == ItmeType.Item2 ? "lamp" : "toch";
    string spritename = mreward.Type == ItmeType.Item2 ? "lamp" : "toch";
    Sprite mskipadicon = abl.InstantiateSprite("common", skipspritename);
    Sprite madicon = abl.InstantiateSprite("common", spritename);

    SpriteRenderer icon_sr = dlgGO.transform.Find("Bg/dialog_No_bt/icon").GetComponent<SpriteRenderer>();
    icon_sr.sprite = mskipadicon;

    icon_sr = dlgGO.transform.Find("Bg/dialog_Yes_bt/icon").GetComponent<SpriteRenderer>();
    icon_sr.sprite = madicon;

    int noadsamount = mreward.SkipNum;
    TextMeshPro text = dlgGO.transform.Find("Bg/dialog_No_bt/amount").GetComponent<TextMeshPro>();
    text.text = "x" + noadsamount;

    int adsamount = mreward.Num;
    text = dlgGO.transform.Find("Bg/dialog_Yes_bt/amount").GetComponent<TextMeshPro>();
    text.text = "x" + adsamount;

    return dlgGO;
  }

  public bool inited()
  {
    return binited;
  }

  public DialogResponse setUIEvent(string name, UIEventType type, object[] extra_info)
  {
    if(type == UIEventType.BUTTON){
      if(name == "dialog_Yes_bt"){
        AudioController._AudioController.playOverlapEffect("yes_no_使用道具_按鍵音效");
        if (bt_handlers[0] != null)
          bt_handlers[0]();
        return DialogResponse.TAKEN_AND_DISMISS;
      }
      else if(name == "dialog_No_bt"){
        AudioController._AudioController.playOverlapEffect("yes_no_使用道具_按鍵音效");
        if (bt_handlers[1] != null)
          bt_handlers[1]();



        return DialogResponse.TAKEN_AND_DISMISS;
      }
    }

    return DialogResponse.PASS;
  }



  public DialogNetworkResponse setNetworkResponseEvent(string name, object payload)
  {
    return DialogNetworkResponse.PASS;
  }
}
