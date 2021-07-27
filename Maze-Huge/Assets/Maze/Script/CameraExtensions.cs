// Copyright 2014 Jarrah Technology (http://www.jarrahtechnology.com). All Rights Reserved. 

using UnityEngine;

public static class CameraExtensions
{
  //目前是只有卡戰使用..
  public static void SwitchMainCamera(bool isOn,Camera[] cardgamecameras = null)
  {
    GameObject cam_root = GameObject.Find("Main Cameras");

    Camera main = cam_root.transform.Find("Common_Main Camera(Clone)").GetComponent<Camera>();
    main.enabled = isOn;

    Camera extmain = cam_root.transform.Find("ExtCamera").GetComponent<Camera>();
    Camera[] allC = new Camera[] { main, extmain};

    SpriteRenderer[] ext_bgs = cam_root.GetComponentsInChildren<SpriteRenderer>();
    int Layer = isOn ? 31 : 9; //為解決部分手機renderer並未被清除的問題，將卡戰的ext_bg_vh改成cardgameBG的layer..
    string Sorting = isOn ? "Default" : "UI";

    //移除卡戰的"BG Camera" 直接用ExtCamera代替並且除了剔除cardgameUI defualt(卡片)
    if (isOn){
      extmain.cullingMask = 1 << 31;
    }
    else{
      extmain.cullingMask = -1;
      LayerCullingHide(extmain, "Default");
      LayerCullingHide(extmain, "cardgameUI");
    }

    //CameraSleep cs = cam_root.GetComponent<CameraSleep>();
    //if (cardgamecameras != null && isOn == false){
    //  cs.init(cardgamecameras);
    //}else{
    //  cs.init(allC);
    //}

    for (int i = 0; i < ext_bgs.Length; i++){
      ext_bgs[i].gameObject.layer = Layer;
      ext_bgs[i].sortingLayerName = Sorting;
    }
  }

  public static void LayerCullingShow( Camera cam, int layerMask)
  {
    cam.cullingMask |= layerMask;
  }

  //打開指定layer
  public static void LayerCullingShow( Camera cam, string layer)
  {
    LayerCullingShow(cam, 1 << LayerMask.NameToLayer(layer));
  }

  public static void LayerCullingHide( Camera cam, int layerMask)
  {
    cam.cullingMask &= ~layerMask;
  }

  //關閉指定layer
  public static void LayerCullingHide( Camera cam, string layer)
  {
    LayerCullingHide(cam, 1 << LayerMask.NameToLayer(layer));
  }

  public static void LayerCullingToggle( Camera cam, int layerMask)
  {
    cam.cullingMask ^= layerMask;
  }

  public static void LayerCullingToggle( Camera cam, string layer)
  {
    LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer));
  }

  public static bool LayerCullingIncludes( Camera cam, int layerMask)
  {
    return (cam.cullingMask & layerMask) > 0;
  }

  public static bool LayerCullingIncludes( Camera cam, string layer)
  {
    return LayerCullingIncludes(cam, 1 << LayerMask.NameToLayer(layer));
  }

  public static void LayerCullingToggle( Camera cam, int layerMask, bool isOn)
  {
    bool included = LayerCullingIncludes(cam, layerMask);
    if (isOn && !included)
    {
      LayerCullingShow(cam, layerMask);
    }
    else if (!isOn && included)
    {
      LayerCullingHide(cam, layerMask);
    }
  }

  public static void LayerCullingToggle( Camera cam, string layer, bool isOn)
  {
    LayerCullingToggle(cam, 1 << LayerMask.NameToLayer(layer), isOn);
  }

  public static void CanvasScreenSpaceCamera(GameObject g,Camera specificC = null){
    Canvas[] allc = g.GetComponentsInChildren<Canvas>();
    foreach(Canvas c in allc){
      c.renderMode = RenderMode.ScreenSpaceCamera;
      if(specificC == null)
        c.worldCamera = Camera.main;
      else
        c.worldCamera = specificC;
      c.sortingLayerName = "UI";
    }

    Camera activeC = specificC == null ? Camera.main : specificC;

    //新增對按鈕的gameobject layer 根據攝影機做layer設定
    UIButton[] allbt = g.GetComponentsInChildren<UIButton>();
    foreach (UIButton b in allbt){

      if(activeC != null){
        b.gameObject.layer = activeC.name == "UI Camera" ? LayerMask.NameToLayer("cardgameUI") : LayerMask.NameToLayer("UI");
      }
      //if (specificC == null){
      //  if(Camera.main != null)
      //  b.gameObject.layer = Camera.main.name == "UI Camera" ? LayerMask.NameToLayer("cardgameUI") : LayerMask.NameToLayer("Default");
      //}
      //else{
      //  b.gameObject.layer = specificC.name == "UI Camera" ? LayerMask.NameToLayer("cardgameUI") : LayerMask.NameToLayer("Default");
      //}
    }

  }
}