using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyScene : MonoBehaviour,IScene
{
  string secneName;
  SceneDisposeHandler pDisposeHandler = null;
  private bool mInited = false;
  GameObject Root;
  enum State
  {
    NULL = 0,


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

  public void initLoadingScene(string name, object[] extra_param = null)
  {
    secneName = name;
  }

  public void initScene(string name, object[] extra_param = null)
  {
    //...

    GameObject dynamicObj = gameObject;

    //
    // Intro prefab
    //
    Root = GameObject.Find("Lobby");
    if (Root == null){
      Root = instantiateObject(dynamicObj, "Lobby");
    }

    mInited = true;

    AudioController._AudioController.play("bgm",true);
    updateUI();

    AdsHelper._AdsHelper.RequestRectangleBannerAds(null);
    //AdsHelper._AdsHelper.RequestInterstitialAds();
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

  public void setUIEvent(string name, UIEventType type, object[] extra_info)
  {
    if(type == UIEventType.BUTTON){
      if(name == "PlayDark_bt")
      {
        AudioController._AudioController.playOverlapEffect("大廳點擊Class Icon音效");
        //Debug.Log("Play_bt");
        pDisposeHandler(SceneDisposeReason.USER_ACTION, new object[] { 0});
        AdsHelper._AdsHelper.DismissRectangleBannerAds();
      }
      else if (name == "Play_bt")
      {
        AudioController._AudioController.playOverlapEffect("大廳點擊Class Icon音效");
        //Debug.Log("Play_bt");
        pDisposeHandler(SceneDisposeReason.USER_ACTION, new object[] { 0});
        AdsHelper._AdsHelper.DismissRectangleBannerAds();
      }
    }
  }

  GameObject instantiateObject(GameObject parent, string name)
  {
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }

  void updateUI(){
    //Root.transform.Find("PlayDark_bt/level").GetComponent<TextMeshPro>().text = PlayerPrefsManager._PlayerPrefsManager.DarkMazeLevel.ToString();
    Root.transform.Find("PlayDark_bt/torch/amount").GetComponent<TextMeshPro>().text = "X" +  PlayerPrefsManager._PlayerPrefsManager.Item1Num.ToString();
    Root.transform.Find("PlayDark_bt/oillamp/amount").GetComponent<TextMeshPro>().text = "X" + PlayerPrefsManager._PlayerPrefsManager.Item2Num.ToString();

    Root.transform.Find("Play_bt/level").GetComponent<TextMeshPro>().text = PlayerPrefsManager._PlayerPrefsManager.LightMazeLevel.ToString();
    Root.transform.Find("Play_bt/torch/amount").gameObject.SetActive(false);
    Root.transform.Find("Play_bt/oillamp/amount").gameObject.SetActive(false);
  }
}
