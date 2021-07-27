using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UseItemDialog :  IDialogContext
{

  public UseItemDialog(string msg, Sprite itemicon, InteractiveDiaLogHandler[] handlers){
    message = msg;
    micon = itemicon;
    bt_handlers = handlers;
  }
  bool binited = false;
  string message;
  Sprite micon = null;
  GameObject dlgGO = null;
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
      dlgGO = abl.InstantiatePrefab("UseItemDialog");
      binited = true;

      TextMeshPro text = dlgGO.transform.Find("Bg/text").GetComponent<TextMeshPro>();
      text.text = message;

      SpriteRenderer icon_sr = dlgGO.transform.Find("Bg/icon_bg/icon").GetComponent<SpriteRenderer>();
      icon_sr.sprite = micon;

    if (micon == null)
      dlgGO.transform.Find("Bg/icon_bg").gameObject.SetActive(false);


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
