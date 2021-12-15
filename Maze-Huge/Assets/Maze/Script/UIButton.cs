using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class UIButton : MonoBehaviour, ITouchEventReceiver {

  public enum PressAction//??touch敺?銵蝮賡?
  {
    ChangeSprite,
    Animator
  }

  SpriteRenderer sr =null;
  Sprite normalState =null;
  Sprite hoverState =null;
  Sprite disabledState = null;
  
  public string hoverState_atlasName =null;
  public string hoverState_spriteName =null; //spritename

  public string disabledState_atlasName = null;
  public string disabledState_spriteName = null;

  //?臬?喲?閮 (甇?ui 摨???ui controller 銝? touch event ????
  public bool passTouchEvent =true;
  public bool passClickEvent =false;

  MainLogic tmpMainLogic =null;

  //甇斗???LONG_PRESS ????
  public float longPressDuration =0.7f;

  public PressAction CurrentAction = PressAction.ChangeSprite;//Start?芸??斗
  Animator ani = null;
  string press_trigger_name = "press";//touch?riggername
  string level_trigger_name = "idle";//level?riggername
  bool Idling = true;

  Sprite[] button_label_str =null;//normal, pressed, disabled
  SpriteRenderer string_sr =null;

  // Use this for initialization
  void Start () {
    //?寞?閰脩隞嗆?瘝?Animator ????touch銵
    ani = GetComponent<Animator>();
    if (ani != null)
      CurrentAction = PressAction.Animator;

    setEnabled(btn_enabled);
  }

  bool focusing =true;
	
	// Update is called once per frame
	void Update () {

	}
  public bool btn_enabled = true;
  //1225 Ray : ?∪澈?典???(powerup?volve)撠梁?btn_enabled = false 嚗??舀?Click??瘙?憿舐內?箔?btn_enabled = false??閮?
  bool Not_enabled_can_OnClick = false;
  public void setEnabled(bool enable, bool Emit_OnClick_When_Disabled = false) {
    btn_enabled = enable;
    Not_enabled_can_OnClick = Emit_OnClick_When_Disabled;
    if(sr == null) {
      sr = GetComponent<SpriteRenderer>();
    }

    if(sr == null){//憒??臬??急芋撘?..
      //?湔?券chirl????
      SpriteRenderer[] allch = GetComponentsInChildren<SpriteRenderer>(true);
      foreach(SpriteRenderer s in allch){
        if (s.sprite == null)
          continue;
        if (s.GetComponent<UIButton>() != null)//頝喲??t閫???拐辣
          continue;
        Sprite sprite_from_ml_sprite = find_sprite(s.sprite.name);
        if (sprite_from_ml_sprite == null)
          continue;
        s.sprite = sprite_from_ml_sprite;
      }
      return;
    }

    createStringGo();

    if (enable == true) {
      //1127 ?∪澈?祟?豢????◤暺??店normalState?null嚗setEnabled(true)????霈?sr.sprite == Null
      //?隞亙??????
      if (normalState == null)
        normalState = sr.sprite;

      if(isTouchEnter == false)
      sr.sprite = normalState;
    } else {
      if (normalState == null)
        normalState = sr.sprite;
        
      if(disabledState == null) {
        //Debug.Log("59 - load disabled state sprite with atlas =" + disabledState_atlasName + ", sprite =" + disabledState_spriteName);
        AssetbundleLoader ab = AssetbundleLoader._AssetbundleLoader;
        if(ab == null) {
          Debug.Log("62 - assetbundle not ready");
        } else {
          disabledState = ab.InstantiateSprite(disabledState_atlasName, disabledState_spriteName);
        }
      }

      if(disabledState != null) {
        sr.sprite = disabledState;
      } else {
        //Debug.Log("disabledState is null");
      }

    }
  }

  public bool getStopPassLongPressEvent(Vector3 pt){
    return !passClickEvent;
  }
  
  public float getLongPressDuration(){
    return longPressDuration;
  }

  public bool OnClick(Vector2 pt){

    if(btn_enabled == false && Not_enabled_can_OnClick == false)
      return !passClickEvent;

    //if(btn_enabled == false)
    //  return !passClickEvent;

    if (tmpMainLogic==null){
      tmpMainLogic =GameObject.FindWithTag("MainLogic").GetComponent<MainLogic>();
    }

    if (focusing)
      tmpMainLogic.setUIEvent(gameObject.name, UIEventType.BUTTON, null);

    return !passClickEvent;
  }

  public void OnLongPress(Vector2 pt, out bool request_focusing){
    request_focusing =true;
    if(btn_enabled == false)
      return;

    if (tmpMainLogic==null){
      tmpMainLogic =GameObject.FindWithTag("MainLogic").GetComponent<MainLogic>();
    }

    if (focusing)
      tmpMainLogic.setUIEvent(gameObject.name, UIEventType.BUTTON_LONG_PRESS, null);

  }

  public bool OnTouchMove(Vector2 curr_touch, Vector2 displacement,  out bool request_focusing){
    request_focusing =false;
    return !passTouchEvent;
  }

  bool isTouchEnter =false;
  public bool OnTouchEnter(Vector2 touchPosi, out bool request_focusing){
    request_focusing =false;

    if(btn_enabled == false)
      return !passTouchEvent;

    if (tmpMainLogic==null){
      tmpMainLogic =GameObject.Find("MainLogic").GetComponent<MainLogic>();
    }
    tmpMainLogic.setUIEvent(gameObject.name, UIEventType.TOUCH_ENTER, null);
    isTouchEnter =true;

    if (CurrentAction == PressAction.Animator)
    {
      //ani = GetComponent<Animator>();
      if (ani == null)
      {
        Debug.Log("UIButton?拐辣 : " + name + "嚗ouch銵 : " + CurrentAction + "嚗??航府?拐辣銝??nimator");
        return !passTouchEvent;
      }
      //Debug.Log("UIButton?拐辣 : " + name + "SetTrigger -- " + press_trigger_name);
      ani.SetTrigger(press_trigger_name);
      Idling = false;
      return !passTouchEvent;
    }
    

    if (sr==null){
      sr =GetComponent<SpriteRenderer>();
    }

    if (sr==null){
      return !passTouchEvent;
    }
      
    if(hoverState_spriteName != null)
      normalState =sr.sprite;

    if (hoverState==null){
      //Debug.Log("35 - load hover state sprite with atlas ="+hoverState_atlasName+", sprite ="+hoverState_spriteName);
      AssetbundleLoader ab = AssetbundleLoader._AssetbundleLoader;
      if (ab==null){
        Debug.Log("74 - assetbundle not ready");
      }else{
        hoverState =ab.InstantiateSprite(hoverState_atlasName, hoverState_spriteName);
      }
    }

    if (hoverState !=null){
      //0113 Ray : ?∪澈銝剜??典????閬?overState??嚗?ㄐ?斗sprite?ame?臭??疾quals hoverState_spriteName
      if (!hoverState.name.Equals(hoverState_spriteName)){
        Debug.Log("hoverState_spriteName was changed.. reload Hoversprite...");
        AssetbundleLoader ab = AssetbundleLoader._AssetbundleLoader;
        hoverState = ab.InstantiateSprite(hoverState_atlasName, hoverState_spriteName);
      }
      sr.sprite =hoverState;
    }else{
      //Debug.Log("hoverState is null");
    }

    if (string_sr !=null && button_label_str!=null && button_label_str[1] !=null){
      string_sr.sprite =button_label_str[1];
    }
    
    return !passTouchEvent;
  }

  public void OnTouchLeave(){
    if(btn_enabled == false)
      return;

    // Debug.Log("leave touch");
    if (CurrentAction == PressAction.Animator)
    {
      //0329 Ray: ?潛?詆urrentAction == PressAction.Animator ??UIButton 銝focus??瘜???鋡怠?思?甈﹒nTouchLeave嚗?
      //撠?◤ trigger "idle" ?”?曄撣?銝活press????擐砌?鋡怠??dle) 嚗?????璅?瑞?冽銝撌脩??idle嚗ure 撠梁?叵eturn 銝?甈﹀rigger "idle"
      if (Idling)
        return;

      if (ani == null)
      {
        Debug.Log("UIButton?拐辣 : " + name + "嚗ouch銵 : " + CurrentAction + "嚗??航府?拐辣銝??nimator");
        return ;
      }
      //Debug.Log("UIButton?拐辣 : " + name + "SetTrigger -- " + level_trigger_name);
      ani.SetTrigger(level_trigger_name);
      Idling = true;
      return ;
    }

    if (sr !=null && normalState !=null)
      sr.sprite =normalState;

    if (string_sr !=null && button_label_str!=null && button_label_str[0] !=null){
      string_sr.sprite =button_label_str[0];
    }

    if (tmpMainLogic==null){
      tmpMainLogic =GameObject.FindWithTag("MainLogic").GetComponent<MainLogic>();
    }

    if (isTouchEnter){
      tmpMainLogic.setUIEvent(gameObject.name, UIEventType.TOUCH_LEAVE, null);
      isTouchEnter =false;
    }
  }

  public void OnFocusRequested(GameObject requested_obj){
    if (requested_obj==this.gameObject){
      return;
    }

    if (requested_obj==null){
      focusing =true;
      return;
    }

    //deactivate onclick/onlongpress function
    focusing =false;

    //cancel hover status
    OnTouchLeave();
  }

  void createStringGo(){
    if (button_label_str != null){
      string_sr.sprite = btn_enabled ? button_label_str[0] : button_label_str[2] == null ? button_label_str[0] : button_label_str[2];
      return;
    }

    if (sr.sprite == null)//??瘝??停瘝?敹???
      return ;

    if (hoverState_atlasName == null && disabledState_atlasName == null)
      return;

    GameObject go = new GameObject("string_sr_go");
    string_sr =  go.AddComponent<SpriteRenderer>();
    string_sr.transform.SetParent(gameObject.transform, false);
    string_sr.transform.localPosition = Vector3.forward * -1.0f;
    string_sr.sortingLayerName = sr.sortingLayerName;
    string_sr.gameObject.layer = gameObject.layer;

    button_label_str =new Sprite[3];
    button_label_str[0] =find_sprite(sr.sprite.name);
    button_label_str[1] =find_sprite(hoverState_spriteName);
    button_label_str[2] =find_sprite(disabledState_spriteName);

    //憒??畜tn_enabled false 瑼Ｘ???utton_label_str[2] 嚗?銝霈??舐征???button_label_str[0] 靘＊蝷?
    string_sr.sprite =btn_enabled?button_label_str[0]: button_label_str[2] == null ? button_label_str[0] : button_label_str[2];

  }

  Sprite find_sprite(string sprite_name){
    if (sprite_name == null)
      return null;

    string lang =PlayerPrefsManager._PlayerPrefsManager.Language.ToLower();
    return AssetbundleLoader._AssetbundleLoader.InstantiateSprite("multi_bt_string_" + lang, sprite_name);
  }

}
