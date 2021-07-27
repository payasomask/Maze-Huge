using UnityEngine;
using System.Collections;

public enum SceneDisposeReason{
  MISSION_COMPLETED,
  DAILYGAME_COMPLETED,
  USER_ACTION,
  USER_EXIT,
  INSUFFICIENT_FUNDS,
  NETWORK_DISCONNECTED,
  UNKNOWN
}

public enum UIEventType{
  TOUCH_ENTER,
  TOUCH_LEAVE,

  BUTTON,
  BUTTON_LONG_PRESS,
  
  HORIZONTAL_SLIDER,
  HORIZONTAL_SLIDER_TOUCH,

  VERTICAL_SLIDER,
  VERTICAL_SLIDER_TOUCH,

  DRAG_START,     //開始 drag
  DRAG_END,       //結束 drag (ps. DROPZONE_DROP 被呼叫的話就不會再呼叫 DRAG_END)
  DROPZONE_ENTER, //進入 dropzone
  DROPZONE_EXIT,  //離開 dropzone
  DROPZONE_DROP,  //payload 丟入 dropzone

  PAGEBUTTON,

  BALL_ROLLER,
  
  UNDEFINED
}

public delegate void SceneDisposeHandler(SceneDisposeReason sdr, object[] extra_info);

public interface IScene
{
  string getSceneName();

  void initLoadingScene(string name, object[] extra_param = null);
  void initScene(string name, object[] extra_param = null);
  void disposeScene(bool forceDispose);

  bool isSceneInitialized();
  int getSceneInitializeProgress();
  bool isSceneDisposed();

  void registerSceneDisposeHandler(SceneDisposeHandler pHandler);


  void setUIEvent(string name, UIEventType type, object[] extra_info);
}
