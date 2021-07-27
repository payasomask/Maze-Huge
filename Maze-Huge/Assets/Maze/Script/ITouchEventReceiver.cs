using UnityEngine;
using System.Collections;

public interface ITouchEventReceiver{

  //
  // BASIC FUNC.
  //
  //mouse down 位置擊中 ui controller 的 bounding box 時收到此事件
  //若 return true, 則其底下的所有 ui controller 不再收到 OnTouchEnter/OnTouchMove/OnTouchLeave 事件
  bool OnTouchEnter(Vector2 posi, out bool request_focusing);

  //有收到 OnTouchEnter 的 ui controller 才會收到此事件
  //若 return true, 則其底下的所有 ui controller 不再收到 OnTouchMove 事件
  bool OnTouchMove(Vector2 curr_touch, Vector2 displacement, out bool request_focusing);
  
  //所有收到 OnToucheEnter 的 ui controller 都會收到此事件
  void OnTouchLeave();

  //
  // ADV FUNC.
  //  
  //若 return true 則其底下的所有 ui controller 不再收到 OnClick 事件
  //mouse 點下及放開時都在此 ui controller 的  boundingbox 內則會收到此事件
  bool OnClick(Vector2 pt);

  //若有 ui controller request_focusing 回傳 true, 則廣播此 ui controller 之 gameobject
  //若同時有多個 ui controller request focusing 則以最上層 ui controller 優先
  void OnFocusRequested(GameObject requested_obj);
  
  //回傳達成 long press 事件所需時間
  float getLongPressDuration();

  //底下 ui 的 long press 事件可能先觸發 (觸發時間設定比較短), 所有在此 ui 觸發前先知道能不能 pass
  //若 return true 則其底下的所有 ui controller 不再收到 OnLongPress 事件
  bool getStopPassLongPressEvent(Vector3 pt);
  void OnLongPress(Vector2 pt, out bool request_focusing);

}
