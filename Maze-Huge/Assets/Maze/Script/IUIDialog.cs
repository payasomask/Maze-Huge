using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DialogType {
  NORMAL, //CANCELABLE
  PANIC,
  MANUAL,
  DELAY_DESTROY
}


public enum DialogResponse{
  TAKEN =0,               //己處理,UI EVENT 不會再傳給下一個 dialogue, scene
  PASS =1,                //UI EVENT 傳給下一個 dialogue, scene
  TAKEN_AND_DISMISS =2,   //關閉 DIALOGUE 並且 UI EVENT 不傳給下一個 dialogue, scene
  PASS_AND_DISMISS =3,    //關閉 DIALOGUE 並且 UI EVENT 傳給下一個 dialogue, scene
  // TAKEN_AND_DISMISS_PREVIOUS = 4,   //關閉 DIALOGUE 前 有另一個 DIALOGUE 被建立 則關閉前者

  ERROR =5
}

public enum DialogNetworkResponse{
  TAKEN =0,
  PASS =1
}

public enum DialogEscapeType{
  NOTHING,            //甚麼都不做
  DISMISS,              //關閉
  PASS,
  TAKEN
}

public interface IDialogContext {
  DialogType getType();
  DialogEscapeType getEscapeType();

  GameObject init(int dlgIdx, AssetbundleLoader abl);  //產生dialog之gameobject, 並回覆給 manager
  //void dismiss();                                      //刪除dialog之gameobject
  bool dismiss();                                      //刪除dialog之gameobject，並執行指定的delegate (回傳 false 代表 dialog 不想被 dismiss)

  bool inited();

  DialogResponse setUIEvent(string name, UIEventType type, object[] extra_info);
  DialogNetworkResponse setNetworkResponseEvent(string name, object payload);
}

public interface IUIDialog  {
  int show(IDialogContext dialogCxt);
  void dismiss(int dlgidx);
  void dismiss_all();

  bool isDialogShowing();
  bool isDialogShowing(int specific_dlg);
  int  getCurrentDialogIdx();
  int getQueuedDialog();

  IDialogContext getDlgCxt(int idx);

  bool setUIEvent(string name, UIEventType type, object[] extra_info);
  bool setNetworkResponseEvent(string name, object payload);

}
