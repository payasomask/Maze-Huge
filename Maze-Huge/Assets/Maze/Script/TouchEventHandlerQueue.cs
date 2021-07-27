using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEventHandlerQueue : MonoBehaviour {
	public List<TouchEventHandler> queue =new List<TouchEventHandler>();
	bool dirty =false;

	GameObject focusing =null;
	bool focusing_broadcased =false;

	void reset_focusing(){
		focusing =null;
		focusing_broadcased =false;
	}

	void set_focusing(GameObject obj){
		if (focusing==null){
			if (obj !=null){
				focusing =obj;
				focusing_broadcased =false;
			}
		}
	}

	void broadcast_focusing(){
		if (focusing_broadcased==true)
			return;
		
		//broadcast focusing
		for (int i=0;i<queue.Count;++i){
			queue[i].broadcast_focusing(focusing);
		}
		focusing_broadcased =true;
	}

	public TouchEventHandler getSpecifyPrioityHandler(int prioity){
		for(int i = 0; i < queue.Count; i++){
			if (queue[i].priority == prioity)
				return queue[i];
		}
		return null;
  }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
    if (Input.GetMouseButton(0)){
			dirty =true;
			
      if (Input.GetMouseButtonDown(0)){

        bool click_event_emitted =false;
				for (int i=0;i<queue.Count;++i){
        	click_event_emitted =queue[i].clearTouch(Input.mousePosition, click_event_emitted);
				}
				reset_focusing();

				bool touch_event_taken =false;
				for (int i=0;i<queue.Count;++i){
					GameObject tmpFocusing =null;
        	touch_event_taken =queue[i].checkTouch(Input.mousePosition, touch_event_taken, out tmpFocusing);
					set_focusing(tmpFocusing);
				}
      }

			for (int i=0;i<queue.Count;++i){
				GameObject tmpFocusing =null;
      	if (queue[i].deliverTouch(Input.mousePosition, out tmpFocusing)){
					set_focusing(tmpFocusing);
					break;
				}
				set_focusing(tmpFocusing);
			}

			for (int i=0;i<queue.Count;++i){
				GameObject tmpFocusing =null;
      	if (queue[i].checkLongPress(Input.mousePosition, out tmpFocusing)){
					set_focusing(tmpFocusing);
					break;
				}
				set_focusing(tmpFocusing);
			}
    }else{

			if (dirty){
				bool click_event_emitted =false;
				for (int i=0;i<queue.Count;++i){
					click_event_emitted =queue[i].clearTouch(Input.mousePosition, click_event_emitted);
				}
				reset_focusing();

				dirty =false;
			}
		}

		broadcast_focusing();
	}
}
