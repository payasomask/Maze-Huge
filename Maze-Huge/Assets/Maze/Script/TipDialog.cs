using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TipDialog :  IDialogContext
{

  public TipDialog(string msg, InteractiveDiaLogHandler handlers){
    message = msg;
    bt_handlers = handlers;
  }
  bool binited = false;
  string message;
  GameObject dlgGO = null;
  InteractiveDiaLogHandler bt_handlers;
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
      dlgGO = abl.InstantiatePrefab("TipDialog");
      binited = true;

      TextMeshPro text = dlgGO.transform.Find("Bg/text").GetComponent<TextMeshPro>();
      text.text = message;


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
        if (bt_handlers != null)
          bt_handlers();
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
