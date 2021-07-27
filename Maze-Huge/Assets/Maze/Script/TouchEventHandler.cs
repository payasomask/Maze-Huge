using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchEventHandler : MonoBehaviour {
  public int priority =0; //a lower number being the highest priority

  class EventReceiver{
    public ITouchEventReceiver currCtrl =null;
    public GameObject attached_obj =null;
    public string attached_obj_name ="";

    public Vector2 prev_touch =Vector2.zero;

    public float touchBeginTime =0.0f;
    public bool longPressEventDelivered =false;

    public bool handle_click_event =false;
    public bool handle_touch_event =false;

    public EventReceiver(GameObject obj, ITouchEventReceiver ctrl, Vector2 prevTouch){
      attached_obj =obj;
      attached_obj_name =obj.name;
      prev_touch =prevTouch;
      currCtrl =ctrl;
      touchBeginTime =Time.realtimeSinceStartup;
    }

  }
  List<EventReceiver> er =new List<EventReceiver>();

  void sort_queue(){
    if (GameObject.Find("DialogManager") ==null || GameObject.Find("DialogManager").GetComponent<TouchEventHandler>() ==null)
      return;
      
    GameObject.Find("DialogManager").GetComponent<TouchEventHandlerQueue>().queue.Sort((a, b)=>{
      int priority_of_a =a.priority;
      int priority_of_b =b.priority;
      return priority_of_a.CompareTo(priority_of_b);
    });
  }

	// Use this for initialization
	void Start () {
    GameObject.Find("DialogManager").GetComponent<TouchEventHandlerQueue>().queue.Add(this);
    sort_queue();
	}

  void OnDisable(){
    if (isApplicationQuitting)
      return;
      
    GameObject.Find("DialogManager").GetComponent<TouchEventHandlerQueue>().queue.Remove(this);
    sort_queue();
  }

  public void update_priority(int val =0){
    priority =val;
    sort_queue();
  }

  bool isApplicationQuitting =false;
  void OnApplicationQuit () {
    isApplicationQuitting = true;
  }

  public bool checkLongPress(Vector3 position, out GameObject focusing){
    focusing =null;
    
    Vector3 pos =Camera.main.ScreenToWorldPoint(position);
    Vector2 touchPos =new Vector2(pos.x, pos.y);

    //check long-press status
    for (int i=0;i<er.Count;++i){
      if (er[i].longPressEventDelivered==false){
        if (Time.realtimeSinceStartup - er[i].touchBeginTime >er[i].currCtrl.getLongPressDuration()){
          try{
            //deliver long-press event
            bool request_focusing =false;
            er[i].currCtrl.OnLongPress(touchPos, out request_focusing);            
            if (request_focusing && focusing==null){
              focusing =er[i].attached_obj;
            }

            er[i].longPressEventDelivered =true;

          }catch(System.Exception e){
            if (er!=null && er.Count>i && er[i] !=null ){
              Debug.LogError("UI Action caused unhandled exception (UI="+er[i].attached_obj_name+") : "+e.ToString());
            }else{
              Debug.LogError("UI Action caused unhandled exception :"+e.ToString());
            }
          }

        }
      }

      if(er[i].currCtrl.getStopPassLongPressEvent(touchPos)==true){
        return true;
      }

    }

    return false;
  }

  public bool checkTouch(Vector3 position, bool touch_event_taken, out GameObject focusing){
    focusing =null;

    Vector3 pos =Camera.main.ScreenToWorldPoint(position);
    Vector2 touchPos =new Vector2(pos.x, pos.y);
    Collider2D[] hitObj =Physics2D.OverlapPointAll(touchPos);
    if (hitObj==null || hitObj.Length ==0)
      return touch_event_taken;

    for (int i=0;i<hitObj.Length;++i){
      TouchEventHandler[] teh_arr =hitObj[i].gameObject.GetComponentsInParent<TouchEventHandler>();
      if (teh_arr.Length==0 || teh_arr[0].gameObject !=gameObject){
        continue;
      }

      ITouchEventReceiver ctrl =(ITouchEventReceiver)hitObj[i].gameObject.GetComponent<ITouchEventReceiver>();
      if (ctrl !=null){
        EventReceiver tmp_er =new EventReceiver(hitObj[i].gameObject, ctrl, touchPos);
        tmp_er.handle_click_event =true;
        tmp_er.handle_touch_event =!touch_event_taken;
        er.Add(tmp_er);

        if (touch_event_taken==false){
          bool request_focusing =false;
          if (ctrl.OnTouchEnter(touchPos, out request_focusing) ==true){
            //Stopping event from passing through the controller
            touch_event_taken =true;
          }

          if (request_focusing && focusing==null){
            focusing =tmp_er.attached_obj;
          }
        }
      }
    }

    return touch_event_taken;
  }

  public bool deliverTouch(Vector3 position, out GameObject focusing){
    focusing =null;

    if (er.Count==0)
      return false;

    Vector3 pos =Camera.main.ScreenToWorldPoint(position);
    Vector2 touchPos =new Vector2(pos.x, pos.y);

    for (int i=0;i<er.Count;++i){
      if (er[i].handle_touch_event==false)
        continue;

      bool request_focusing =false;
      if (er[i].currCtrl.OnTouchMove(touchPos, touchPos-er[i].prev_touch, out request_focusing)==true){
        er[i].prev_touch =touchPos;

        if (request_focusing && focusing ==null){
          focusing =er[i].attached_obj;
        }

        //Stopping event from passing through the controller
        return true;
      }

      if (request_focusing && focusing ==null){
        focusing =er[i].attached_obj;
      }

      er[i].prev_touch =touchPos;
    }

    return false;

  }

  public bool clearTouch(Vector3 position, bool click_event_emitted){
    if (er.Count==0)
      return click_event_emitted;

    Vector3 pos =Camera.main.ScreenToWorldPoint(position);
    Vector2 touchPos =new Vector2(pos.x, pos.y);

    for (int i=0;i<er.Count;++i){
      try{
        if (er[i].handle_touch_event){
          er[i].currCtrl.OnTouchLeave();
        }
      }catch(System.Exception e){
        if (er!=null && er.Count>i && er[i] !=null ){
          Debug.LogError("UI Action caused unhandled exception (UI="+er[i].attached_obj_name+") : "+e.ToString());
        }else{
          Debug.LogError("UI Action caused unhandled exception :"+e.ToString());
        }
      }
    }

    if (click_event_emitted==false){
      for (int i=0;i<er.Count;++i){
        try{
          if(er[i].handle_click_event==false)
            continue;

          //若 touch 結束時仍停留在 collider 內則產生 click event
          Collider2D[] hitObj =Physics2D.OverlapPointAll(touchPos);
          if (isHitCollider(hitObj, er[i].attached_obj.name)){
            if (er[i].currCtrl.OnClick(touchPos)==true){
              //Stopping event from passing through the controller
              click_event_emitted =true;
              break;
            }
          }
        }catch(System.Exception e){
          if (er!=null && er.Count>i && er[i] !=null ){
            Debug.LogError("UI Action caused unhandled exception (UI="+er[i].attached_obj_name+") : "+e.ToString());
          }else{
            Debug.LogError("UI Action caused unhandled exception :"+e.ToString());
          }
        }

      }
    }

    er.Clear();
    return click_event_emitted;
  }

  public void broadcast_focusing(GameObject obj){
    if (obj==null){
      ITouchEventReceiver[] iter =gameObject.transform.GetComponentsInChildren<ITouchEventReceiver>();
      for (int i=0;i<iter.Length;++i){
        iter[i].OnFocusRequested(obj);
      }
    }else{
      for (int i=0;i<er.Count;++i){
        er[i].currCtrl.OnFocusRequested(obj);
      }
    }
  }

  bool isHitCollider(Collider2D[] hitObj, string obj_name){
    for (int i=0;i<hitObj.Length;++i){
      if (hitObj[i].name ==obj_name){
        return true;
      }
    }
    return false;
  }

}
