using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishDialog :  IDialogContext
{
  public enum Type{
    Completed,
    GameOver
  }
  public FinishDialog(Type type,int level, InteractiveDiaLogHandler[] handlers){
    bt_handlers = handlers;
    currentmazelevel = level;
    mtype = type;
  }
  Type mtype;
  bool binited = false;
  GameObject dlgGO = null;
  int currentmazelevel;
  InteractiveDiaLogHandler[] bt_handlers;
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
      dlgGO = abl.InstantiatePrefab("FinishDialog");
      binited = true;

    MazeConfig tmp = JsonLoader._JsonLoader.GetMazeConfig(currentmazelevel);

    ADReward reward = mtype == Type.GameOver ? tmp.GameOverReward : tmp.CompletedReward;

    string message = mtype == Type.GameOver ? "GameOver\nGet item by watch a  AD?" : "Completed\nGet item by watch a  AD?";

    string skipspritename = reward.SkipType == ItmeType.Item2 ? "lamp" : "toch";
    string spritename = reward.Type == ItmeType.Item2 ? "lamp" : "toch";


    Sprite mskipadicon = abl.InstantiateSprite("common", skipspritename);

    Sprite madicon = abl.InstantiateSprite("common", spritename);

    TextMeshPro text = dlgGO.transform.Find("Bg/text").GetComponent<TextMeshPro>();
      text.text = message;

      SpriteRenderer icon_sr = dlgGO.transform.Find("Bg/dialog_No_bt/icon").GetComponent<SpriteRenderer>();
      icon_sr.sprite = mskipadicon;
      dlgGO.transform.Find("Bg/dialog_No_bt/amount").GetComponent<TextMeshPro>().text = "x" + reward.SkipNum;

    icon_sr = dlgGO.transform.Find("Bg/dialog_Yes_bt/icon").GetComponent<SpriteRenderer>();
    icon_sr.sprite = madicon;
    dlgGO.transform.Find("Bg/dialog_Yes_bt/amount").GetComponent<TextMeshPro>().text = "x" + reward.Num;


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
        if (bt_handlers[1] != null)
          bt_handlers[1]();
        return DialogResponse.TAKEN_AND_DISMISS;
      }
      else if(name == "dialog_No_bt"){
        AudioController._AudioController.playOverlapEffect("yes_no_使用道具_按鍵音效");
        if (bt_handlers[0] != null)
          bt_handlers[0]();
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
