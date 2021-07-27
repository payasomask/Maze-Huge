using UnityEngine;
using System.Collections;

public class UIDir : MonoBehaviour, ITouchEventReceiver {

  MainLogic tmpMainLogic =null;

  Vector2 velocity =Vector2.zero;
  bool add_velocity =false;

  //是否傳遞訊息 (此 ui 底下的 ui controller 不對 touch event 有反應)
  public bool passTouchEvent =false;
  public bool passClickEvent =true;

  bool focusing =true;

  // Use this for initialization
  void Start () {
    tmpMainLogic =GameObject.Find("MainLogic").GetComponent<MainLogic>();
  
  }
  
  // Update is called once per frame
  void Update () {
    if (add_velocity){
      if (velocity.magnitude>0.1f){
        velocity*=0.9f;
        Vector2 tmpDisplacement =velocity*Time.deltaTime;

        tmpMainLogic.setUIEvent(gameObject.name, UIEventType.BALL_ROLLER, new object[]{tmpDisplacement});

      }
    }

  }

  public float getLongPressDuration(){
    return 0f;
  }

  public bool OnClick(Vector2 pt){
    return !passClickEvent;
  }

  public bool getStopPassLongPressEvent(Vector3 pt){
    return !passClickEvent;
  }
  
  public void OnLongPress(Vector2 pt, out bool request_focusing){
    request_focusing =false;
  }

  Vector3 prev_collide_pt =Vector3.zero;
  Vector3 start_collide_pt = Vector3.zero;

  float acc_displacement =0f;
  public bool OnTouchMove(Vector2 curr_touch, Vector2 displacement, out bool request_focusing){
    request_focusing =false;
    if (prev_collide_pt==Vector3.zero){
      prev_collide_pt =new Vector3(curr_touch.x, curr_touch.y, 0.0f);
      return !passTouchEvent;
    }

    if (focusing)
      tmpMainLogic.setUIEvent(gameObject.name, UIEventType.BALL_ROLLER, new object[]{displacement});

    prev_collide_pt =new Vector3(curr_touch.x, curr_touch.y, 0.0f);

    velocity =displacement/Time.deltaTime;
    acc_displacement+=displacement.magnitude;
    if (acc_displacement>=50f){
      acc_displacement =0f;
      request_focusing =true;
    }

    return !passTouchEvent;
  }

  public bool OnTouchEnter(Vector2 posi, out bool request_focusing){
    request_focusing =false;
    add_velocity =false;
    velocity =Vector2.zero;
    start_collide_pt = new Vector3(posi.x, posi.y, 0.0f);
    

    if (tmpMainLogic == null){
      tmpMainLogic = GameObject.Find("MainLogic").GetComponent<MainLogic>();
    }
    tmpMainLogic.setUIEvent(gameObject.name, UIEventType.TOUCH_ENTER, null);

    return !passTouchEvent;
  }

  public void OnTouchLeave(){

    Vector2 Leave_dir = (prev_collide_pt - start_collide_pt).normalized;
    float leave_length = (prev_collide_pt - start_collide_pt).magnitude;
    //短到一個數值就當作沒滑動
    if (leave_length < 3.0f){
      Leave_dir = Vector2.zero;
      return;
    }

    if (tmpMainLogic == null)
    {
      tmpMainLogic = GameObject.Find("MainLogic").GetComponent<MainLogic>();
    }
    tmpMainLogic.setUIEvent(gameObject.name, UIEventType.TOUCH_LEAVE, new object[] { Leave_dir });

    prev_collide_pt =Vector3.zero;
    add_velocity =true;

  }

  public void OnFocusRequested(GameObject requested_obj){
    if (requested_obj==this.gameObject){
      return;
    }

    if (requested_obj==null){
      focusing =true;
      return;
    }

    //deactivate ontouchmove event
    focusing =false;
  }

}
