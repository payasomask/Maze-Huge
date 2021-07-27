using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour,IScene
{

  string secneName;
  SceneDisposeHandler pDisposeHandler = null;
  bool mInited = false;

  enum State
  {
    NULL=0,

    ALPHA_IN,
    LOGOSHOW,
    ALPHA_OUT,

    DONE,
  }

  State currentstate = State.NULL;

  public void disposeScene(bool forceDispose)
  {
    pDisposeHandler = null;
  }

  public int getSceneInitializeProgress()
  {
    return 0;
  }

  public string getSceneName()
  {
    return secneName;
  }

  public void initLoadingScene(string name, object[] extra_param = null){
    secneName = name;
  }

  public void initScene(string name, object[] extra_param = null){
    //...

    GameObject dynamicObj = gameObject;

    //
    // Intro prefab
    //
    GameObject cc = GameObject.Find("Intro");
    if (cc == null){
      cc = instantiateObject(dynamicObj, "Intro");
    }

    ALPHA_IN(2.0f);

    mInited = true;
    return;
  }

  public bool isSceneDisposed()
  {
    return (pDisposeHandler == null);
  }

  public bool isSceneInitialized()
  {
    return mInited;
  }

  public void registerSceneDisposeHandler(SceneDisposeHandler pHandler)
  {
    pDisposeHandler = pHandler;
  }

  public void setUIEvent(string name, UIEventType type, object[] extra_info){
    
  }

  // Start is called before the first frame update
  void Start()
    {
        
    }


  float mct = 0.0f;
    // Update is called once per frame
    void Update()
    {
    if (currentstate == State.NULL)
      return;

    if(currentstate == State.DONE){
      currentstate = State.NULL;
      //...go to lobby
      pDisposeHandler(SceneDisposeReason.USER_EXIT, null);
      return;
    }

    if(currentstate == State.ALPHA_IN){
      mct -= Time.deltaTime;
      if(mct <= 0.0f){
        LOGOSHOW(2.0f);
        return;
      }
    }
    else if (currentstate == State.LOGOSHOW){
      mct -= Time.deltaTime;
      if (mct <= 0.0f){
        ALPHA_OUT(2.0f);
        return;
      }
    }else if(currentstate == State.ALPHA_OUT){
      mct -= Time.deltaTime;
      if (mct <= 0.0f){
        Done();
        return;
      }
    }
  }

  void ALPHA_IN(float duration){
    currentstate = State.ALPHA_IN;
    mct = duration;
  }

  void LOGOSHOW(float duration){
    currentstate = State.LOGOSHOW;
    mct = duration;
  }
  void ALPHA_OUT(float duration)
  {
    currentstate = State.ALPHA_OUT;
    mct = duration;
  }

  void Done(){
    currentstate = State.DONE;
  }

  GameObject instantiateObject(GameObject parent, string name){
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }
}
