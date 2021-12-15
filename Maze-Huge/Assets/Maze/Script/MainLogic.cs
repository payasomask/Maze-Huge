using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public delegate void CommonAction(); //the CommonAction should works like C# Action
//??InteractiveDiaLogHandler??銵
public delegate void InteractiveDiaLogHandler();
public class PlayerPrefsManager
{
  public static PlayerPrefsManager _PlayerPrefsManager = null;
  string _Audio_T = "on";
  string audioKey = "AudioToggle";
  public string Audio_T
  {
    get { return _Audio_T; }
    set{
      _Audio_T = value;
      savePlayerPrefsString("AudioToggle", value);
      AudioController._AudioController.toggleAudio(convertOnOff2bool(value));
    }
  }
  string _Music_T = "on";
  string musicKey = "MusicToggle";
  public string Music_T
  {
    get { return _Music_T; }
    set{
      _Music_T = value;
      savePlayerPrefsString("MusicToggle", value);
      AudioController._AudioController.toggleMusic(convertOnOff2bool(value));
    }
  }
  string _Vibrate_T = "on";
  string vibrateKey = "VibrateToggle";
  public string Vibrate_T
  {
    get { return _Vibrate_T; }
    set{
      _Vibrate_T = value;
      savePlayerPrefsString("VibrateToggle", value);
    }
  }

  string PlayerPrefs_Ver = "20210120";


  string _Language = "US";
  string LanguageKey = "Language";
  public string Language
  {
    get { return _Language; }
    set
    {
      _Language = value;
      savePlayerPrefsString("Language", value);
    }
  }

  string _AdministationVersion = "off";
  string _AdministationVersionKey = "AdministationVersion";
  public string AdministationVersion
  {
    get { return _AdministationVersion; }
    set
    {
      _AdministationVersion = value;
      savePlayerPrefsString(_AdministationVersionKey, value);
    }
  }
  int _PlayMazeTimes;
  public int PlayMazeTimes
  {
    get { return _PlayMazeTimes; }
    set
    {
      _PlayMazeTimes = value;
      savePlayerPrefsInt("PlayerMazeTimes", value);
    }
  }

  int _LightMazeLevel;
  public int LightMazeLevel
  {
    get { return _LightMazeLevel; }
    set
    {
      _LightMazeLevel = value;
      savePlayerPrefsInt("LightMazeLevel", value);
    }
  }

  int _Item1Num;
  public int Item1Num
  {
    get { return _Item1Num; }
    set
    {
      _Item1Num = value;
      savePlayerPrefsInt("TorchNum", value);
    }
  }

  int _Item2Num;
  public int Item2Num
  {
    get { return _Item2Num; }
    set
    {
      _Item2Num = value;
      savePlayerPrefsInt("OilLmapNum", value);
    }
  }

  MazeRecord _mazerecord = null;
  string RecordKey = "Record";
  public MazeRecord mazerecord
  {
    get {     
      return _mazerecord;
    }
    set
    {
      _mazerecord = value;
      _mazerecord.time = MazeManager._MazeManager.gametimer();
      string record_dic_string = Newtonsoft.Json.JsonConvert.SerializeObject(_mazerecord);
      savePlayerPrefsString(RecordKey, record_dic_string);
    }
  }

  public PlayerPrefsManager()
  {

    _Audio_T = PlayerPrefs.GetString(audioKey, "on");
    _Music_T = PlayerPrefs.GetString(musicKey, "on");
    _Vibrate_T = PlayerPrefs.GetString(vibrateKey, "on");
    _Language = PlayerPrefs.GetString(LanguageKey, "US");
    _AdministationVersion = PlayerPrefs.GetString(_AdministationVersionKey, "");

    _PlayMazeTimes = PlayerPrefs.GetInt("PlayerMazeTimes", 0);
    _LightMazeLevel = PlayerPrefs.GetInt("LightMazeLevel", 1);
    _Item1Num = PlayerPrefs.GetInt("TorchNum", 3);
    _Item2Num = PlayerPrefs.GetInt("OilLmapNum", 3);

    //PlayerPrefs.DeleteKey(RecordKey);

    if (PlayerPrefs.HasKey(RecordKey)){
      string record_string = PlayerPrefs.GetString(RecordKey);
      _mazerecord = Newtonsoft.Json.JsonConvert.DeserializeObject<MazeRecord>(record_string);
      Debug.Log("548 - Record exist ");
      if (_mazerecord.RecordVersion != MainLogic._MainLogic.RecordVersion) {
        Debug.Log("548 - RecordVersion defferent..delete MazeRecord");
        clearRecord();
      }
    }
  }

  public void updateRecord(Cell[,] cells)
  {
    if (_mazerecord == null)
      _mazerecord = new MazeRecord();

    _mazerecord.cells = cells;

    mazerecord = _mazerecord;
  }
  public void updateRecord(Cell cell)
  {
    if (_mazerecord == null)
      _mazerecord = new MazeRecord();

    _mazerecord.cells[cell.X, cell.Y] = cell;

    mazerecord = _mazerecord;
  }

  public void updateRecord(Vector2 goallocation, Vector2 boxlocation)
  {
    if (_mazerecord == null)
      _mazerecord = new MazeRecord();

    _mazerecord.goallocation = new JsonVector2(goallocation.x, goallocation.y);
    _mazerecord.boxlocation = new JsonVector2(boxlocation.x, boxlocation.y);

    mazerecord = _mazerecord;
  }
  public void updateRecord(Vector2 playerlocation)
  {
    if (_mazerecord == null)
      _mazerecord = new MazeRecord();

    _mazerecord.playerlocation = new JsonVector2(playerlocation.x, playerlocation.y); ;

    mazerecord = _mazerecord;
  }

  public void updateItmeRecord(Vector2 itmeposition)
  {
    if (_mazerecord == null)
      _mazerecord = new MazeRecord();
    if (_mazerecord.ItmePosition_list == null)
      _mazerecord.ItmePosition_list = new List<JsonVector2>();

    _mazerecord.ItmePosition_list.Add(new JsonVector2(itmeposition.x, itmeposition.y));

    mazerecord = _mazerecord;
  }
  public void updateRecord(float time)
  {
    if (_mazerecord == null)
      _mazerecord = new MazeRecord();

    _mazerecord.time = time;

    mazerecord = _mazerecord;
  }
  public void updateRecord(Vector3[] linePositions){
    if (_mazerecord == null)
      _mazerecord = new MazeRecord();

    List<JsonVector3> tmp_list = new List<JsonVector3>(linePositions.Length);

    foreach(var v in linePositions){
      tmp_list.Add(new JsonVector3(v.x, v.y, v.z));
    }

    if (_mazerecord.lineRender_lists == null)
      _mazerecord.lineRender_lists = new List<JsonVector3[]>();

    _mazerecord.lineRender_lists.Add(tmp_list.ToArray());

    mazerecord = _mazerecord;
  }
  public void clearRecord(){
    PlayerPrefs.DeleteKey(RecordKey);
    _mazerecord = null;
  }

  public void GetRewrd(ItmeType type,int Num){
    if (type == ItmeType.Item1){
      Item1Num += Num;
      Debug.Log("454 - GetReward : " + type + "嚗arned : " + Num + "嚗urrent :" + Item1Num);
    }
    else if(type == ItmeType.Item2){
      Item2Num += Num;
      Debug.Log("454 - GetReward : " + type + "嚗arned : " + Num + "嚗urrent :" + Item2Num);
    }
  }

  public bool IsPlayTimesODD(){
    return PlayMazeTimes % 2 != 0;
  }

  public void resetplayerprefs(){
    _PlayerPrefsManager = new PlayerPrefsManager();
  }

  //瑼Ｘ?怠?鞈????撣?.
  public bool checkPlayerPrefsDATA(){
    if (PlayerPrefs.HasKey("PlayerPrefs_Ver")){
      string ver = PlayerPrefs.GetString("PlayerPrefs_Ver");
      Debug.Log("Local PlayerPrefs_Ver : " + ver + ", Embedded PlayerPrefs_Ver : " + PlayerPrefs_Ver);
      if (ver != PlayerPrefs_Ver)
        return true;

      return false;
    }
    PlayerPrefs.SetString("PlayerPrefs_Ver", PlayerPrefs_Ver);
    return true;
  }

  public void loginsetting()//?駁?敺?閬身摰??
  {
    //if (PlayerPrefs.HasKey(GetFriendsCardsTimeKey))
    //  _GetFriendsCardsTime = new DateTime().ToString();

  }

  void savePlayerPrefsString(string keyname , string OnOff)
  {
    PlayerPrefs.SetString(keyname, OnOff);
    PlayerPrefs.Save();
  }
  void savePlayerPrefsFloat(string keyname, float value)
  {
    PlayerPrefs.SetFloat(keyname, value);

    PlayerPrefs.Save();
  }
  void savePlayerPrefsInt(string keyname, int value)
  {
    PlayerPrefs.SetInt(keyname, value);

    PlayerPrefs.Save();
  }
  bool convertOnOff2bool(string name)
  {
    if (name.Equals("on"))
      return true;

    return false;
  }
  string convertbool2OnOff(bool OnOff)
  {
    if (OnOff)
      return "on";

    return "off";
  }
  Dictionary<string, object> pauseSaveDic = new Dictionary<string, object>();
  public void OnPause()
  {
    Debug.Log("app paused by user...");
    //AddpauseSaveDic(GetFriendsCardsTimeKey, GetFriendsCardsTime);//?怠??
    //GetFriendsCardsTime = new DateTime().ToString();//?蔭??FB憟賢???????
  }
  public void OnFocus()
  {
    Debug.Log("user back to the app...");
    //GetFriendsCardsTime = (string)LoadpauseSaveDic(GetFriendsCardsTimeKey);
  }

  void AddpauseSaveDic(string name, object value)
  {
    Debug.Log("撠?" + name + " value : " + (string)value+ "摮?怠?Dic");
    if (pauseSaveDic.ContainsKey(name))
      pauseSaveDic[name] = value;
    else
      pauseSaveDic.Add(name, value);
  }

  object LoadpauseSaveDic(string key)
  {
    if (pauseSaveDic.ContainsKey(key)){
      Debug.Log("敺?摮?怠?Dic 霈??Key : " + key + " value : " + (string)pauseSaveDic[key]);
      return pauseSaveDic[key];
    }
    return null;
  }

  //bulltin part
  Dictionary<string, object> bulltin_dic =null;
  void resume_bulltin_dic(){
    if (bulltin_dic ==null){
      if (PlayerPrefs.HasKey("blocked_bulltin_list")){
        string raw_str =PlayerPrefs.GetString("blocked_bulltin_list");
        bulltin_dic =(Dictionary<string, object>)MiniJSON.Json.Deserialize(raw_str);
      }else{
        bulltin_dic =new Dictionary<string, object>();
      }
    }
  }

  public void block_bulltin_today(string id){
    resume_bulltin_dic();

    DateTime block_until =DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).AddTicks(-1).AddDays(1);
    Debug.Log("308 - bulltin "+id+" blocked until "+block_until.ToString("yyyy/MM/dd HH:mm:ss"));
    if (bulltin_dic.ContainsKey(id)==true){
      bulltin_dic[id] =block_until.ToString("yyyy/MM/dd HH:mm:ss");
    }else{
      bulltin_dic.Add(id, block_until.ToString("yyyy/MM/dd HH:mm:ss"));
    }

    PlayerPrefs.SetString("blocked_bulltin_list", MiniJSON.Json.Serialize(bulltin_dic));

  }

  public void remove_obsoleted_bulltin_data(List<string> bulltin_id){
    resume_bulltin_dic();

    List<string> raw_dic_keys =new List<string>(bulltin_dic.Keys);
    for (int i=0;i<raw_dic_keys.Count;){
      if (bulltin_id.Contains(raw_dic_keys[i])==true){
        raw_dic_keys.RemoveAt(i);
      }else{
        ++i;
      }
    }

    for (int i=0;i<raw_dic_keys.Count;++i){
      Debug.Log("342 - bulltin ("+raw_dic_keys[i]+") record deleted");
      bulltin_dic.Remove(raw_dic_keys[i]);
    }

    PlayerPrefs.SetString("blocked_bulltin_list", MiniJSON.Json.Serialize(bulltin_dic));

  }

  public bool is_bulltin_blocked(string id){
    resume_bulltin_dic();
    if (bulltin_dic.ContainsKey(id)){
      System.DateTime ed =System.DateTime.ParseExact((string)bulltin_dic[id], "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
      int curr_ts = (int)(System.DateTime.Now.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
      int ed_ts =(int)(ed.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
      if (curr_ts<=ed_ts){
        Debug.Log("357 - bulltin id "+id+" was blocked");
        return true;
      }
    }else{
      Debug.Log("361 - bulltin id "+id+" not found");
    }
    return false;
  }

  //bulltin part
  Dictionary<string, object> dialog_dic = null;
  void resume_dialog_dic()
  {
    if (dialog_dic == null)
    {
      if (PlayerPrefs.HasKey("blocked_dialog_list"))
      {
        string raw_str = PlayerPrefs.GetString("blocked_dialog_list");
        dialog_dic = (Dictionary<string, object>)MiniJSON.Json.Deserialize(raw_str);
      }
      else
      {
        dialog_dic = new Dictionary<string, object>();
      }
    }
  }

  public void block_dialog_today(string id)
  {
    resume_dialog_dic();

    DateTime dialog_until = DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture).AddTicks(-1).AddDays(1);
    Debug.Log("308 - dialog " + id + " dialog until " + dialog_until.ToString("yyyy/MM/dd HH:mm:ss"));
    if (dialog_dic.ContainsKey(id) == true)
    {
      dialog_dic[id] = dialog_until.ToString("yyyy/MM/dd HH:mm:ss");
    }
    else
    {
      dialog_dic.Add(id, dialog_until.ToString("yyyy/MM/dd HH:mm:ss"));
    }

    PlayerPrefs.SetString("blocked_dialog_list", MiniJSON.Json.Serialize(dialog_dic));

  }

  public void remove_obsoleted_dialog_data(List<string> dialog_id)
  {
    resume_dialog_dic();

    List<string> raw_dic_keys = new List<string>(dialog_dic.Keys);
    for (int i = 0; i < raw_dic_keys.Count;)
    {
      if (dialog_id.Contains(raw_dic_keys[i]) == true)
      {
        raw_dic_keys.RemoveAt(i);
      }
      else
      {
        ++i;
      }
    }

    for (int i = 0; i < raw_dic_keys.Count; ++i)
    {
      Debug.Log("342 - dialog (" + raw_dic_keys[i] + ") record deleted");
      dialog_dic.Remove(raw_dic_keys[i]);
    }

    PlayerPrefs.SetString("blocked_dialog_list", MiniJSON.Json.Serialize(dialog_dic));

  }

  public bool is_dialog_blocked(string id)
  {
    resume_dialog_dic();
    if (dialog_dic.ContainsKey(id))
    {
      System.DateTime ed = System.DateTime.ParseExact((string)dialog_dic[id], "yyyy/MM/dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
      int curr_ts = (int)(System.DateTime.Now.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
      int ed_ts = (int)(ed.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
      if (curr_ts <= ed_ts)
      {
        Debug.Log("357 - dialog id " + id + " was blocked");
        return true;
      }
    }
    else
    {
      Debug.Log("361 - dialog id " + id + " not found");
    }
    return false;
  }

}

public class MainLogic : MonoBehaviour
{
  //紀錄版本號，當Json版本號與此不同，Json存檔將捨棄不用
  [SerializeField]
  public int RecordVersion;

  public static MainLogic _MainLogic = null;
  private GameObject camera_go = null;
  public IScene mS = null;
  class SceneTransition
  {
    public delegate void TransitionDoneHandler();
    // IScene fromScene;
    Type toScene; //IScene Type
    string toScene_name;

    AsyncOperation ao = null;
    float ao_start_time = 0f;

    TransitionDoneHandler pTransitionDoneHandler = null;

    bool force_transition = false;

    enum Status
    {
      IDLE,
      DISPOSE_CURRENT_SCENE,
      WAIT_DISPOSE,
      INIT_NEW_SCENE,
      WAIT_RELEASE_UNUSEDASSET1,
      INIT_NEW_SCENE_PHASE2,
      WAIT_INIT,
      DUPLICATED_LOGIN_SHOWING,
      DONE
    }
    Status mCurrentStatus = Status.IDLE;

    object[] extra_param_for_init = null;

    public SceneTransition(string pto_name, object[] extra_param, TransitionDoneHandler pHandler, bool forceTransition = false)
    {
      force_transition = forceTransition;

      //get gametype from pto_name
      toScene = (Type)_MainLogic.mER.getSettings(pto_name, SceneSettingsType.SCENE_TYPE);
      toScene_name = pto_name;
      pTransitionDoneHandler = pHandler;

      extra_param_for_init = extra_param;
    }

    public void update()
    {
      if (mCurrentStatus == Status.IDLE)
      {
        //start transition...
        mCurrentStatus = Status.DISPOSE_CURRENT_SCENE;

      }
      else
      if (mCurrentStatus == Status.DISPOSE_CURRENT_SCENE)
      {
        if (_MainLogic.mS == null)
        {
          mCurrentStatus = Status.INIT_NEW_SCENE;
          return;
        }
        Debug.Log("66 - disposeScene with force transition =" + force_transition);
        _MainLogic.mS.disposeScene(force_transition);

        mCurrentStatus = Status.WAIT_DISPOSE;

      }
      else
      if (mCurrentStatus == Status.WAIT_DISPOSE)
      {
        if (_MainLogic.mS.isSceneDisposed())
        {

          GameObject.Destroy(((MonoBehaviour)_MainLogic.mS).gameObject);
          _MainLogic.mS = null;

          AssetbundleLoader._AssetbundleLoader.PurgeAudioClipCache();
          AssetbundleLoader._AssetbundleLoader.PurgeAudioClipCache();
          AssetbundleLoader._AssetbundleLoader.PurgeSpriteCache();
          AssetbundleLoader._AssetbundleLoader.PurgeSpriteCache();
          AssetbundleLoader._AssetbundleLoader.PurgeAssetbundles();

          Debug.Log(">>>>>>>>> UNLOAD UNUSED ASSETS - 1 <<<<<<<<<");
          ao_start_time = Time.realtimeSinceStartup;
          ao = Resources.UnloadUnusedAssets();

          System.GC.Collect();
          System.GC.WaitForPendingFinalizers();

          mCurrentStatus = Status.WAIT_RELEASE_UNUSEDASSET1;
        }

      }
      else
      if (mCurrentStatus == Status.WAIT_RELEASE_UNUSEDASSET1)
      {
        if (ao.isDone)
        {
          Debug.Log(">>>>>>>>> UNLOAD UNUSED ASSETS :" + (Time.realtimeSinceStartup - ao_start_time) + " SECONDS TAKEN <<<<<<<<<");
          mCurrentStatus = Status.INIT_NEW_SCENE;
          ao = null;
        }
      }
      else
      if (mCurrentStatus == Status.INIT_NEW_SCENE)
      {
        Debug.Log("82 - init new scene (extra_param.Length=" + extra_param_for_init.Length + ")...");
        GameObject gameObj = new GameObject();
        gameObj.name = toScene_name;

        //EP Type
        //Type ep_type = (Type)_MainLogic.mER.getSettings(toScene_name, SceneSettingsType.PROBABILITY_TYPE);
        //if (ep_type != null)
        //{
        //  mCxt.mEP = (IProbability)System.Activator.CreateInstance(ep_type);
        //  mCxt.mEP.setContext(mCxt);
        //}

        _MainLogic.mS = (IScene)gameObj.AddComponent(toScene);
        _MainLogic.mS.registerSceneDisposeHandler(_MainLogic.SceneDisposeHandler);
        _MainLogic.mS.initLoadingScene(toScene_name, extra_param_for_init);

        mCurrentStatus = Status.INIT_NEW_SCENE_PHASE2;//WAIT_INIT;

      }
      else
      if (mCurrentStatus == Status.INIT_NEW_SCENE_PHASE2)
      {
        _MainLogic.mS.initScene(toScene_name, extra_param_for_init);
        mCurrentStatus = Status.WAIT_INIT;

      }
      else
      if (mCurrentStatus == Status.WAIT_INIT)
      {
        if (_MainLogic.mS.isSceneInitialized())
        {

          if (pTransitionDoneHandler != null)
          {
            pTransitionDoneHandler();
          }

          mCurrentStatus = Status.DONE;
        }

      }

    }
  }
  SceneTransition mCurrentSceneTransition = null;

  private void Awake()
  {
    _MainLogic = this;
    PlayerPrefsManager._PlayerPrefsManager = new PlayerPrefsManager();
    AssetbundleLoader._AssetbundleLoader = new AssetbundleLoader();
    mER = new EmbeddedSceneSettings();
    //AudioManager._AudioManager = new AudioManager();
    //MaskManager._MaskManager = new MaskManager();
  }

  public ISceneSettings mER = null;

  // Use this for initialization
  void Start()
  {

    //
    // SETUP UNITY ENGINE PARAMETERS
    //
    //Application.targetFrameRate = 60;
    QualitySettings.vSyncCount = 1;

    //
    //Create Common Object
    //
    Debug.Log("init MainLogic Environment...");

    //keep screen always on
    Screen.sleepTimeout = SleepTimeout.NeverSleep;

    GameObject dlgmgr = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab("DialogManager");
    dlgmgr.name = "DialogManager";
    dlgmgr.GetComponent<TouchEventHandler>().update_priority(-1);

    UIDialog tmpDLG = dlgmgr.GetComponent<UIDialog>();
    tmpDLG.setupAssetbundleLoader(AssetbundleLoader._AssetbundleLoader);

    //AUDIO
    GameObject audio_obj = new GameObject();
    audio_obj.name = "Audio";
    AudioController mAC = audio_obj.AddComponent<AudioController>();
    mAC.init();

    GameObject dynamicObj = new GameObject();
    dynamicObj.name = "Main Cameras";
    //TouchEventHandler teh =dynamicObj.AddComponent<TouchEventHandler>(); //HANDLE EVENT FOR SLEEPING SCREEN WAKE
    //teh.update_priority(-3);//
    //BoxCollider2D tehbc2d =dynamicObj.AddComponent<BoxCollider2D>();
    //tehbc2d.size =new Vector2(1366f, 768f);

    FontManager._FontManager.init();
    JsonLoader._JsonLoader.Init();

    camera_go = instantiateObject(dynamicObj, "Common_Camera");

    GameObject extendBGCamera = new GameObject();
    extendBGCamera.transform.SetParent(dynamicObj.transform);
    extendBGCamera.transform.localPosition = Vector3.forward * -2000f;
    extendBGCamera.name = "ExtCamera";
    Camera ec = extendBGCamera.AddComponent<Camera>();
    ec.cullingMask = 1 << 31;
    ec.orthographic = true;
    ec.clearFlags = CameraClearFlags.SolidColor;
    ec.backgroundColor = Color.black;
    ec.farClipPlane = 4000f;
    ec.depth = -2;
    ec.allowMSAA = false;

    if (((float)Screen.width / (float)Screen.height) < (768f / 1366))
    {
      ec.orthographicSize = 683f * Screen.height / Screen.width * (768f / 1366);

      GameObject ext_bgv1 = new GameObject();
      ext_bgv1.transform.SetParent(ec.transform, false);
      ext_bgv1.transform.localPosition = new Vector3(0f, -683 - (116F / 2F), 2500f);
      ext_bgv1.name = "ext_bg";
      ext_bgv1.layer = 31;
      SpriteRenderer sr1 = ext_bgv1.AddComponent<SpriteRenderer>();
      sr1.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("embedded2", "ext_bg_h");

      GameObject ext_bgv2 = new GameObject();
      ext_bgv2.transform.SetParent(ec.transform, false);
      ext_bgv2.transform.localPosition = new Vector3(0f, 683 + (116F / 2F), 2500f);
      ext_bgv2.name = "ext_bg";
      ext_bgv2.layer = 31;
      SpriteRenderer sr2 = ext_bgv2.AddComponent<SpriteRenderer>();
      sr2.flipY = true;
      sr2.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("embedded2", "ext_bg_h");


    }
    else
    {
      ec.orthographicSize = 683f;

      GameObject ext_bgv1 = new GameObject();
      ext_bgv1.transform.SetParent(ec.transform, false);
      ext_bgv1.transform.localPosition = new Vector3(-766.5f, 0f, 2500f);
      ext_bgv1.name = "ext_bg";
      ext_bgv1.layer = 31;
      SpriteRenderer sr1 = ext_bgv1.AddComponent<SpriteRenderer>();
      sr1.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("embedded2", "ext_bg_v");

      GameObject ext_bgv2 = new GameObject();
      ext_bgv2.transform.SetParent(ec.transform, false);
      ext_bgv2.transform.localPosition = new Vector3(766.5f, 0f, 2500f);
      ext_bgv2.name = "ext_bg";
      ext_bgv2.layer = 31;
      SpriteRenderer sr2 = ext_bgv2.AddComponent<SpriteRenderer>();
      sr2.flipX = true;
      sr2.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("embedded2", "ext_bg_v");
    }


    MLCamera._MLCamera.init();

    //
    //Create First Scene
    //
    Debug.Log("transit to first scene...");
    mCurrentSceneTransition = new SceneTransition("IntroScene", new object[] { }, delegate () {
      mCurrentSceneTransition = null;
    });
    //mCurrentSceneTransition = new SceneTransition("MazeScene", new object[] { }, delegate ()
    //{
    //  mCurrentSceneTransition = null;
    //});
    //mCurrentSceneTransition = new SceneTransition("LobbyScene", new object[] { }, delegate ()
    //{
    //  mCurrentSceneTransition = null;
    //});

    //#if UNITY_ANDROID && !UNITY_EDITOR
    //          if (android_utility_class ==null)
    //            android_utility_class =(new AndroidJavaClass("com.powergameranger.androidnative.ADSHelper"));

    //          if (android_utility_class !=null)
    //            android_utility_class.CallStatic("Log","test call native");

    //#endif

    AdsHelper._AdsHelper.init();
    //AdsHelper._AdsHelper.RequestInterstitialAds();
  }

  GameObject instantiateObject(GameObject parent, string name)
  {
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }

  void Update()
  {

    //process scene transition
    if (mCurrentSceneTransition != null)
    {
      mCurrentSceneTransition.update();
      return;
    }

    if (Input.GetKeyUp(KeyCode.Escape))
    {//鋆蔭KeyCode.Escape蝯曹?敺?閫貊
      setUIEvent("back_bt", UIEventType.BUTTON, null);
      return;
    }

  }

  virtual public void setUIEvent(string name, UIEventType type, object[] extra_info)
  {
    if (mS != null)
    {
      if (UIDialog._UIDialog.setUIEvent(name, type, extra_info))
      {
        return;
      }

      mS.setUIEvent(name, type, extra_info);

    }
  }

  virtual public bool getSandboxMode()
  {
    return false;
  }

  void SceneDisposeHandler(SceneDisposeReason sdr, object[] extra_info)
  {
    if (mCurrentSceneTransition != null)
    {
      //??甇???scene ??瘨?隞?
      Debug.Log("181 - scene transition is processing...");
      return;
    }

    Debug.Log("SceneDisposed, reason =" + sdr + ", current scene=" + mS.getSceneName());


    //transit to new scene with random mission
    if (mS.getSceneName() == "IntroScene")
    {
      if (sdr == SceneDisposeReason.USER_EXIT)
      {
        mCurrentSceneTransition = new SceneTransition("LobbyScene", new object[] { }, delegate ()
        {
          mCurrentSceneTransition = null;
        }, true);
      }

    }
    else
    //if (mS.getSceneName() == "LoginScene")
    //{
    //  if (sdr == SceneDisposeReason.USER_ACTION)
    //  {
    //    int type = (int)extra_info[0];

    //    if (type == 0)
    //    {
    //      mCurrentSceneTransition = new SceneTransition("LobbyScene", new object[] { extra_info[1], extra_info[2] }, delegate () {
    //        mCurrentSceneTransition = null;
    //      }, true);
    //    }
    //  }
    //}
    //else
    if (mS.getSceneName() == "LobbyScene")
    {
      if (sdr == SceneDisposeReason.USER_ACTION)
      {
        int type = (int)extra_info[0];
        if (type == 0)
        {
          mCurrentSceneTransition = new SceneTransition("MazeScene", new object[] { }, delegate ()
          {
            mCurrentSceneTransition = null;
          }, true);
        }
      }
      else
      if (sdr == SceneDisposeReason.USER_EXIT)
      {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("move task to back !");
        using (AndroidJavaClass javaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject unityActivity = javaClass.GetStatic<AndroidJavaObject>("currentActivity");
            unityActivity.Call<bool>("moveTaskToBack", true);
        }
#endif
      }
    }
    else
    if (mS.getSceneName() == "MazeScene")
    {
      if (sdr == SceneDisposeReason.USER_ACTION)
      {
        //int type = (int)extra_info[0];
        //if (type == 0){

        //}
      }
      else
      if (sdr == SceneDisposeReason.USER_EXIT)
      {
        mCurrentSceneTransition = new SceneTransition("LobbyScene", new object[] { }, delegate ()
        {
          PlayerPrefsManager._PlayerPrefsManager.clearRecord();
          mCurrentSceneTransition = null;
        }, true);
      }
    }
  }


  AndroidJavaClass android_utility_class = null;
  public static void vibrate(int duration_ms)
  {
    MainLogic ml = GameObject.Find("MainLogic").GetComponent<MainLogic>();
    if (ml == null)
      return;

    //#if UNITY_ANDROID && !UNITY_EDITOR
    //    if (ml.mCxt.mPPM.Vibrate_T =="on"){
    //      if (ml.android_utility_class ==null)
    //        ml.android_utility_class =(new AndroidJavaClass("com.phardera.support.Utility"));

    //      if (ml.android_utility_class !=null)
    //        ml.android_utility_class.CallStatic("vibrate", duration_ms); //vibrate with duration 500 ms
    //    }
    //#endif
  }


  //Android 皜祈岫 ????璅∪??ａ?? ???澆OnApplicationFocus(false)?亥??澆OnApplicationPause(true)
  //Android 皜祈岫 敺??芋撘????????澆OnApplicationFocus(true)?亥??澆OnApplicationPause(false)
  //mono 璅⊥?冽葫閰?????璅∪??ａ?? ???澆OnApplicationPause(true)?亥??澆OnApplicationFocus(false)
  //mono 璅⊥?冽葫閰?敺??芋撘????????澆OnApplicationFocus(true)?亥??澆OnApplicationPause(false)
  bool ispause = false;
  bool isfocus = true;
  void OnApplicationPause(bool pause)
  {
    //Debug.Log("OnApplicationPause" + pause);
    ispause = pause;
    if (/*!isfocus &&*/ ispause)
    {
      //?拙振撠??脫??..
      PlayerPrefsManager._PlayerPrefsManager.OnPause();
    }
  }

  void OnApplicationFocus(bool focus)
  {
    //Debug.Log("OnApplicationFocus" + focus);
    isfocus = focus;
    if (isfocus && ispause)
    {
      PlayerPrefsManager._PlayerPrefsManager.OnFocus();
    }
  }

  //???閬???敺?蝵桃??
  void OnApplicationQuit()
  {
    //??賜?沃nity Editor ?蝬 蝔?蝣潛?Application.Quit(); ??鋡怠?恬??丕ndroid撟喳?典??芋撘?仿?????????隞颱???
    PlayerPrefsManager._PlayerPrefsManager.OnPause();
  }

  string ReplaceFirst(string text, string search, string replace)
  {
    int pos = text.IndexOf(search);
    if (pos < 0)
    {
      return text;
    }
    return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
  }

  string stage_id_to_stage_no(string stage_id)
  {
    return int.Parse(stage_id.Substring(stage_id.Length - 2, 2)).ToString();
  }

  public float getCameraWidth()
  {
    if (camera_go == null)
      return 0.0f;
    return camera_go.GetComponent<ConfigCamera>().desiredWidth;
  }

  public float getCameraHight()
  {
    if (camera_go == null)
      return 0.0f;
    return camera_go.GetComponent<ConfigCamera>().desiredHeight;
  }
}
