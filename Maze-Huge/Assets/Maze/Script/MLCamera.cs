using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

//負責處理遊戲中生成的Cinemachine
public class MLCamera : MonoBehaviour
{
  public static MLCamera _MLCamera = null;
  [System.Serializable]
  public class Camera_Setting{
    public string Name;//必須與unity layer 設定裡一致，沒有layer要新增
    public LayerMask LayerMask;
    public Camera c;
    public CinemachineVirtualCamera virtualcamera;
  }

  public List<Camera_Setting> Camera_list = new List<Camera_Setting>();

  private void Awake()
  {
    _MLCamera = this;
  }
  // Start is called before the first frame update
  void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  public void init(){

    setCamerasetting();

    setVirtualCamer();
  }

  Camera_Setting getCameraFromList(string name){
    if(Camera_list.Count == 0){
      Debug.Log("MLCamera Camera_list.count == 0 , set it first");
      return null;
    }

    for(int i = 0; i < Camera_list.Count; i++){
      Camera_Setting cs = Camera_list[i];
      if (cs.Name == name)
        return cs;
    }

    return null;
  }
  Camera_Setting getCameraFromList(int index)
  {
    if (Camera_list.Count == 0)
    {
      Debug.Log("MLCamera Camera_list.count == 0 , set it first");
      return null;
    }

    return Camera_list[index];
  }

  void setCamerasetting(){
    if (Camera_list.Count == 0){
      Debug.Log("MLCamera Camera_list.count == 0 , set it first");
      return;
    }

    Camera main_c = Camera.main;
    main_c.cullingMask = getCameraFromList(0).LayerMask;
    main_c.gameObject.AddComponent<CinemachineBrain>();
    main_c.clearFlags = CameraClearFlags.Depth;

    //複製除了main之外camera
    for (int i = 1; i < Camera_list.Count; i++){
      Camera othercamera = Instantiate(main_c.gameObject, Vector3.zero, Quaternion.identity, transform).GetComponent<Camera>();
      Camera_Setting cs = getCameraFromList(i);

      othercamera.gameObject.name = cs.Name + "_Camera";
      othercamera.cullingMask = getCameraFromList(i).LayerMask;

      //
      othercamera.GetComponent<AudioListener>().enabled = false;
      othercamera.depth = main_c.depth - i;
      othercamera.clearFlags = CameraClearFlags.Depth;
      cs.c = othercamera;
      //othercamera.gameObject.AddComponent<CinemachineBrain>();
    }
  }

  void setVirtualCamer(){

    Camera main_c = Camera.main;

    //複製所有camera
    for (int i = 0; i < Camera_list.Count; i++)
    {
      Camera_Setting cs = getCameraFromList(i);

      GameObject virtual_camera_go = new GameObject(cs.Name + "_virtualcam");
      virtual_camera_go.transform.parent = transform;
      virtual_camera_go.transform.localPosition = main_c.transform.localPosition;
      CinemachineVirtualCamera vc = virtual_camera_go.AddComponent<CinemachineVirtualCamera>();
      vc.m_Lens.Orthographic = true;
      vc.m_Lens.OrthographicSize = main_c.orthographicSize;
      vc.m_Lens.FarClipPlane = main_c.farClipPlane;
      vc.m_Lens.NearClipPlane = main_c.nearClipPlane;
      vc.gameObject.layer = LayerMask.NameToLayer(cs.Name);
      cs.virtualcamera = vc;
      cs.virtualcamera.AddCinemachineComponent<CinemachineFramingTransposer>();
    }
  }

  //請參照此物件的camera_list來傳入想要被哪個camera照到
  public void setGameObjectLayer(GameObject go, string layername,bool applychild = true){
    go.layer = LayerMask.NameToLayer(layername);

    if (applychild == false){
      return;
    }

    int children = go.transform.childCount;
    for(int i = 0; i < children; i++){
      go.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(layername);
    }
  }
  //請參照此物件的camera_list來傳入想要被哪個camera追蹤
  public void setVirtualCamerFollow(Transform target, string name)
  {
    Camera_Setting cs = getCameraFromList(name);
    cs.virtualcamera.Follow = target;
  }
  public void setVirtualCamerLockAt(Transform target, string name)
  {
    Camera_Setting cs = getCameraFromList(name);
    cs.virtualcamera.LookAt = target;
  }
}
