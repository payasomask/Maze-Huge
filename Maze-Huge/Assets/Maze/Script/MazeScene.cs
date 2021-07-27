using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MazeScene : MonoBehaviour,IScene
{
  string secneName;
  SceneDisposeHandler pDisposeHandler = null;
  bool mInited = false;
  bool isadwatched = false;

  TextMeshPro timer;
  //TextMeshPro itemtimer;

  float gametime = 0.0f;
  string oillamptimerID;

  UIButton torchbt = null;
  UIButton oillampbt = null;

  GameObject mRoot = null;
  MazeConfig config = null;
  int currentlevel;

  //�˼�10-1�O�_�w�g����L
  //bool fsx_played = false;

  //UIButton staffbt = null;
  //bool staff_used = false;

  enum State
  {
    NULL = 0,

    CREAT_MAZE,

    CHECK_ADS,
    PLAY_ADS,

    IDLE,
    STOP,

    GAME_OVER,
    COMPLETED,
    WAITPLAYER,
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
    //...do somthing init

    currentlevel = PlayerPrefsManager._PlayerPrefsManager.LightMazeLevel;

    GameObject dynamicObj = gameObject;
    config = JsonLoader._JsonLoader.GetMazeConfig(currentlevel);
    //
    // Intro prefab
    //
    mRoot = GameObject.Find("Maze");
    if (mRoot == null)
    {
      mRoot = instantiateObject(dynamicObj, "Maze");
    }
    timer = mRoot.transform.Find("TopUI/bg/timer").GetComponent<TextMeshPro>();
    updateGameTime(gametime);

    torchbt = mRoot.transform.Find("DownUI/bg/Torchbt").GetComponent<UIButton>();
    oillampbt = mRoot.transform.Find("DownUI/bg/oillampbt").GetComponent<UIButton>();
    //staffbt = cc.transform.Find("DownUI/staffbt").GetComponent<UIButton>();
    mRoot.transform.Find("TopUI/bg/level").GetComponent<TextMeshPro>().text = "Class-" + currentlevel;
    //itemtimer = cc.transform.Find("DownUI/itemtimer").GetComponent<TextMeshPro>();

    string spritename = config.DownUIReward.Type == ItmeType.Item2 ? "lamp" : "toch";
    mRoot.transform.Find("DownUI/bg/adsTorch/icon").GetComponent<SpriteRenderer>().sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", spritename);
    AdsHelper._AdsHelper.RequestRewardAds();
    AdsHelper._AdsHelper.RequestBannerAds(() => {
      currentstate = State.IDLE;
    });
    updateUI();


    MaskManager._MaskManager.Init();
    MazeManager._MazeManager.Init(this,config);


    currentstate = State.CREAT_MAZE;
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
  //Vector2 slider_dir = Vector2.zero;
  public void setUIEvent(string name, UIEventType type, object[] extra_info)
  {
    if(type == UIEventType.TOUCH_LEAVE){
      if(name == "SilderReceiver"){
        Vector2 slider_dir = (Vector2)extra_info[0];

        Debug.Log("TOUCH_LEAVE�A slider_dir :" + slider_dir);

        if (slider_dir == Vector2.zero)
          return;

        if (currentstate != State.IDLE)
          return;

        //�}�l�P�_��V�A������O���k�j�٬O�W�U�j
        float horizontalstr, vertaiclstr;
        horizontalstr = Mathf.Abs(slider_dir.x);
        vertaiclstr = Mathf.Abs(slider_dir.y);
        if (vertaiclstr >= horizontalstr)
        {
          if (slider_dir.y >= 0.0f)
          {
            //���W����
            PlayerControll(Dir.Top);
          }
          else
          {
            //���U����
            PlayerControll(Dir.Bottom);
          }
        }
        else
        {
          if (slider_dir.x >= 0.0f)
          {
            //���k����
            PlayerControll(Dir.Right);

          }
          else
          {
            //��������
            PlayerControll(Dir.Left);

          }
        }
      }
      
    }
    else
    if (type == UIEventType.BUTTON)
    {
      if (name == "stopbt")
      {
        AudioController._AudioController.playOverlapEffect("�Ȱ����s����");
        SpriteRenderer bt = transform.Find("Maze(Clone)/TopUI/bg/stopbt").GetComponent<SpriteRenderer>();
        SpriteRenderer icon = transform.Find("Maze(Clone)/TopUI/bg/stopbt/icon").GetComponent<SpriteRenderer>();
        bt.gameObject.name = "playbt";
        icon.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "play_gray");
        currentstate = State.STOP;
        //switch sprite

      }
      else if(name == "playbt"){
        AudioController._AudioController.playOverlapEffect("�Ȱ����s����");
        SpriteRenderer bt = transform.Find("Maze(Clone)/TopUI/bg/playbt").GetComponent<SpriteRenderer>();
        SpriteRenderer icon = transform.Find("Maze(Clone)/TopUI/bg/playbt/icon").GetComponent<SpriteRenderer>();
        bt.gameObject.name = "stopbt";
        icon.sprite = AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "pause");

        currentstate = State.IDLE;
        //switch sprite

      }
      else if (name == "Torchbt")
      {
        AudioController._AudioController.playOverlapEffect("yes_no_�ϥιD��_���䭵��");
        //...
        UIDialog._UIDialog.show(new UseItemDialog("Use a torch?", AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "toch"), new InteractiveDiaLogHandler[] {
      ()=>{
        //No
        currentstate = State.IDLE;
        return;
      },
      ()=>{
        //YES
        PlayerItemManager._PlayerItemManager.UseTorch(MazeManager._MazeManager.PlayerPosition());
        //PlayerPrefsManager._PlayerPrefsManager.Item1Num--;
        PlayerPrefsManager._PlayerPrefsManager.updateItmeRecord(MazeManager._MazeManager.PlayerPosition());

        updateUI();

        currentstate = State.IDLE;
        return;
      }
      }));
        currentstate = State.WAITPLAYER;

      }
      else if (name == "oillampbt")
      {
        AudioController._AudioController.playOverlapEffect("yes_no_�ϥιD��_���䭵��");
        UIDialog._UIDialog.show(new UseItemDialog("Use a oillamp?", AssetbundleLoader._AssetbundleLoader.InstantiateSprite("common", "lamp"), new InteractiveDiaLogHandler[] {
      ()=>{
        currentstate = State.IDLE;
        return;
      },
      ()=>{
        PlayerItemManager._PlayerItemManager.UseOilLamp();
        //PlayerPrefsManager._PlayerPrefsManager.Item2Num--;
        updateUI();
        currentstate = State.IDLE;
        return;
      }
      }));
        currentstate = State.WAITPLAYER;
      }
      else if(name == "adsTorch"){
        AudioController._AudioController.playOverlapEffect("yes_no_�ϥιD��_���䭵��");
        currentstate = State.PLAY_ADS;
        mRoot.transform.Find("DownUI/bg/adsTorch").gameObject.SetActive(false);
        isadwatched = true;
        AdsHelper._AdsHelper.ShowRewardAd(()=> {
          currentstate = State.IDLE;
          return;
        }, 
        
        () => {
          GetAdReward(config.DownUIReward.Type, config.DownUIReward.Num);
          currentstate = State.IDLE;
          return;
        },
                  () => {
                    //�o�ӥD���I���������y
                    UIDialog._UIDialog.show(new TipDialog("Failed get AD video",
                      () => {
                        currentstate = State.IDLE;
                        return;
                      }));
                  }
        );
      }
      //else if (name == "staffbt")
      //{
      //  UIDialog._UIDialog.show(new InteractiveDialog("Use Staff?", new string[] { "acceptbt", "cancelbt" }, new InteractiveDiaLogHandler[] {
      //()=>{
      //  //staff_used = true;
      //  PlayerItemManager._PlayerItemManager.UseStaff();
      //  currentstate = State.IDLE;
      //  return;
      //},
      //()=>{
      //  currentstate = State.IDLE;
      //  return;
      //}
      //}));
      //  currentstate = State.WAITPLAYER;

      //}

    }
  }

  GameObject instantiateObject(GameObject parent, string name)
  {
    GameObject g = AssetbundleLoader._AssetbundleLoader.InstantiatePrefab(name);
    g.transform.SetParent(parent.transform, true);

    return g;
  }

  void Update(){

    //if (Input.GetKeyUp(KeyCode.R))
    //{
    //  //CREAT_MAZE();
    //  pDisposeHandler(SceneDisposeReason.USER_EXIT, null);
    //  currentstate = State.WAITPLAYER;
    //  return;
    //}

    if (currentstate == State.STOP){
      return;
    }

    UpdateButton();

    if (currentstate == State.IDLE)
    {
      gametime += Time.deltaTime;
      //if (gametime <= 0.0f)
      //{
      //  gametime = 0.0f;
      //  currentstate = State.GAME_OVER;
      //}
      updateGameTime(gametime);
      //updateItemTimer();

      //PlayerControll();
    }
    else
    if (currentstate == State.CREAT_MAZE){

      MazeRecord mr = PlayerPrefsManager._PlayerPrefsManager.mazerecord;
      if (mr != null){
        //Ū������
        gametime = mr.time;
        MazeManager._MazeManager.CreatMaze(mr);
      }
      else{
        //4:3�άO���
        MazeManager._MazeManager.ClearMaze();
        MazeManager._MazeManager.CreatMaze(config.Rows, config.Columns);
      }


      //gametime = 0.0f;

      //���s�˼ƪ�����
      //fsx_played = false;

      GameObject SilderReceiver = mRoot.transform.Find("SilderReceiver").gameObject;
      SilderReceiver.GetComponent<BoxCollider2D>().size = MazeManager._MazeManager.GetMazeSize();
      //SilderReceiver.GetComponent<BoxCollider2D>().offset = new Vector2(0.0f, MazeManager._MazeManager.GetMaze_Pivot());

      //�ʺA�վ�downUI����m
      //Transform DownUI = mRoot.transform.Find("DownUI/bg").transform;
      //float depth = DownUI.localPosition.z;
      //float downuibg_y = DownUI.GetComponent<SpriteRenderer>().sprite.bounds.size.y * 0.5f;
      //DownUI.localPosition = new Vector3(0.0f, GetMazeTopUIBottom() - MazeManager._MazeManager.GetMazeSize().y - downuibg_y, depth);

      

      currentstate = State.IDLE;
    }else if(currentstate == State.CHECK_ADS){
      //�ˬd�@�Ǧ����S���P�_�@�������s�i
      //bool shoads = PlayerPrefsManager._PlayerPrefsManager.IsPlayTimesODD();
      //if (shoads){
      //  AdsHelper._AdsHelper.ShowInterstitialAds(()=> {
      //    //�����s�i��A��^�j�U
      //    GetAdReward(config.CompletedReward.Type, config.CompletedReward.Num);
      //    });
      //  currentstate = State.PLAY_ADS;
      //  return;
      //}

      ////��^�j�U
      //currentstate = State.WAITPLAYER;
      //AdsHelper._AdsHelper.DismissBannerAds();
      //pDisposeHandler(SceneDisposeReason.USER_EXIT, null);
      //return;

      //���μ��s�i�ݬO�n�F��...�N�~��U�@��
      //CREAT_MAZE();
    }
    else if(currentstate == State.GAME_OVER){

      stepPlayMazeTimes();

      //...
      UIDialog._UIDialog.show(new FinishDialog( FinishDialog.Type.GameOver, currentlevel, new InteractiveDiaLogHandler[] {
      ()=>{
        //��^�j�U
        pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
        GetAdReward(config.GameOverReward.SkipType,config.GameOverReward.SkipNum);
        AdsHelper._AdsHelper.DismissBannerAds();
        return;
      },
      ()=>{
        //���s�g�c�A�άO�ϥέ�g�c?
        //CREAT_MAZE();
                AdsHelper._AdsHelper.ShowRewardAd(()=>{
          AdsHelper._AdsHelper.DismissBannerAds();
          pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
          return;
        },()=>{
          AdsHelper._AdsHelper.DismissBannerAds();
          pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
          GetAdReward(config.GameOverReward.Type,config.GameOverReward.Num);
        },
                  ()=>{
             //�٬O�n��skip ad���y
            ItmeType skiptype = config.GameOverReward.SkipType;
            string rewardtype = skiptype.ToString();
            int rewardnum = config.GameOverReward.SkipNum;
              UIDialog._UIDialog.show(new TipDialog("Failed get AD video,\nYou gain " + skiptype + "x" + rewardnum + "instead",
                () => {
                                              pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
                            AdsHelper._AdsHelper.DismissBannerAds();
                    GetAdReward(skiptype,rewardnum);
                    return;
                }));
          }
        );

        currentstate = State.PLAY_ADS;
        return;
      }
      }));

      currentstate = State.WAITPLAYER;
    }
    else if (currentstate == State.COMPLETED){

      stepPlayMazeTimes();
      steplevel();
      //...do somthing
      UIDialog._UIDialog.show(new FinishDialog( FinishDialog.Type.Completed,currentlevel, new InteractiveDiaLogHandler[] {
      ()=>{
        //�������ܴN�O��^�j�U
        pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
        GetAdReward(config.CompletedReward.SkipType,config.CompletedReward.SkipNum);
        AdsHelper._AdsHelper.DismissBannerAds();
        PlayerPrefsManager._PlayerPrefsManager.clearRecord();
        return;
      },
      ()=>{
        //..�󴫰g�c? ��������s? �άOԣ�������D
        //CREAT_MAZE();

        AdsHelper._AdsHelper.ShowRewardAd(()=>{
          AdsHelper._AdsHelper.DismissBannerAds();
          pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
          return;
        },()=>{
          AdsHelper._AdsHelper.DismissBannerAds();
          pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
          GetAdReward(config.CompletedReward.Type,config.CompletedReward.Num);
        },
                  ()=>{
             //�٬O�n��skip ad���y
            ItmeType skiptype = config.CompletedReward.SkipType;
            string rewardtype = skiptype.ToString();
            int rewardnum = config.CompletedReward.SkipNum;
              UIDialog._UIDialog.show(new TipDialog("Failed get AD video,\nYou gain " + skiptype + "x" + rewardnum + "instead",
                () => {
                            pDisposeHandler( SceneDisposeReason.USER_EXIT,null);
                            AdsHelper._AdsHelper.DismissBannerAds();
                    GetAdReward(skiptype,rewardnum);
                    return;
                }));
          }

        );

        //�i�s�i
        currentstate = State.PLAY_ADS;
        return;
      }
      }));
      currentstate = State.WAITPLAYER;
      return;
    }

  }

  void CREAT_MAZE() {
    currentstate = State.CREAT_MAZE;
    //staff_used = false;
    return;
  }

  void updateGameTime(float time){
    if (timer == null)
      return;

    //if(time <= 10.00f && !fsx_played){
    //  AudioController._AudioController.playOverlapEffect("���Ū��10-1");
    //  fsx_played = true;
    //}

    TimeSpan span = TimeSpan.FromSeconds((double)(new decimal(time)));
    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    DateTime date = epoch + span;

    timer.text = "TIME " + date.ToString("HH:mm:ss.ff");
  }

  void PlayerControll(){
    //..�ھڤ��P���x�b�o������J��
    
    if (Input.GetKeyUp(KeyCode.UpArrow))
    {
      MazeManager._MazeManager.movePlayer(Dir.Top);
    }
    if (Input.GetKeyUp(KeyCode.DownArrow))
    {
      MazeManager._MazeManager.movePlayer(Dir.Bottom);
    }
    if (Input.GetKeyUp(KeyCode.LeftArrow))
    {
      MazeManager._MazeManager.movePlayer(Dir.Left);
    }
    if (Input.GetKeyUp(KeyCode.RightArrow))
    {
      MazeManager._MazeManager.movePlayer(Dir.Right);
    }

  }
  void PlayerControll(Dir move_dir)
  {
    MazeManager._MazeManager.movePlayer(move_dir);
  }


  void UpdateButton(){
    //if (mGameType == GameType.LIGHT){
    //  torchbt.setEnabled(false);
    //  oillampbt.setEnabled(false);
    //  mRoot.transform.Find("DownUI/bg/oillampbt/Prohibit").gameObject.SetActive(true);
    //  mRoot.transform.Find("DownUI/bg/Torchbt/Prohibit").gameObject.SetActive(true);
    //  //�ݹL������
    //  mRoot.transform.Find("DownUI/bg/adsTorch").gameObject.SetActive(!isadwatched);
    //  return;
    //}


    bool playermoving = MazeManager._MazeManager.IsPlayerMoving();

    //bool canusestaff = !staff_used && !playermoving;
    //staffbt.setEnabled(canusestaff);
    //torchbt.setEnabled(!playermoving && canusestaff);
    //bool canuseoil = !playermoving && oillampbt.GetComponent<Timer>() == null;
    //oillampbt.setEnabled(canuseoil && canusestaff);

    bool torchnum = PlayerPrefsManager._PlayerPrefsManager.Item1Num > 0;
    bool oilnum = PlayerPrefsManager._PlayerPrefsManager.Item2Num > 0;

    torchbt.setEnabled(!playermoving && torchnum);
    bool canuseoil = !playermoving && !MazeManager._MazeManager.IsTrackLine();
    oillampbt.setEnabled(canuseoil && oilnum);

    mRoot.transform.Find("DownUI/bg/oillampbt/Prohibit").gameObject.SetActive(!oilnum);
    mRoot.transform.Find("DownUI/bg/Torchbt/Prohibit").gameObject.SetActive(!torchnum);
    mRoot.transform.Find("DownUI/bg/adsTorch").gameObject.SetActive(!isadwatched);
  }

  //void updateItemTimer(){
  //  if (itemtimer == null)
  //  return;

  //  Timer oildtimer = oillampbt.GetComponent<Timer>();
  //  if (oildtimer == null){
  //    itemtimer.gameObject.SetActive(false);
  //    return;
  //  }

  //  itemtimer.gameObject.SetActive(true);
  //  itemtimer.text = "Oil Lamp Remain Time : " + oildtimer.getSessionTime(oillamptimerID).ToString("F02", System.Globalization.CultureInfo.InvariantCulture);

  //}

  public void ArrivalCell(string who, Cell c){

    if(c.Type == CellType.Box){
      AudioController._AudioController.playOverlapEffect("�_�c�����X�{���ܭ�");
      UIDialog._UIDialog.show(new ADSDialog( config.boxADReward, new InteractiveDiaLogHandler[] {
      ()=>{
        //..�ݼs�i������ԣ��
        AdsHelper._AdsHelper.ShowRewardAd(
          ()=>
        {
          //�[�����ѩάO���a�j����L�s�i�A�@�ߤ������y
          currentstate = State.IDLE;
          return;
        },
          ()=>{
          //���a�T�꦳�ݧ��s�i�A�������y
          GetAdReward(config.boxADReward.Type,config.boxADReward.Num);
          currentstate = State.IDLE;
          return;
        },
          ()=>{
             //�٬O�n��skip ad���y
            ItmeType skiptype = config.boxADReward.SkipType;
            string rewardtype = skiptype.ToString();
            int rewardnum = config.boxADReward.SkipNum;
              UIDialog._UIDialog.show(new TipDialog("Failed get AD video,\nYou gain " + skiptype + "x" + rewardnum + "instead", 
                () => {
                    GetAdReward(skiptype,rewardnum);
                    currentstate = State.IDLE;
                    return;
                }));
          }
          );

        currentstate = State.PLAY_ADS;
        return;
      },
      ()=>{
        //��ܤ��ݼs�i
        //
        GetAdReward(config.boxADReward.SkipType,config.boxADReward.SkipNum);
        currentstate = State.IDLE;
        return;
      }
      }));
      currentstate = State.WAITPLAYER;
      return;
    }else if(c.Type == CellType.Goal){

      if(currentstate == State.GAME_OVER){
        return;
      }

      currentstate = State.COMPLETED;
      return;
    }

  }

  //��ݭn�ھڬ��N���]�p�վ��y�g�c�������I��m
  //�o�̻ݭn�^��TOPUI���U�tY��m
  public float GetMazeTopUIBottom(){
    
    float topuiHight = mRoot.transform.Find("TopUI/bg").GetComponent<SpriteRenderer>().sprite.bounds.size.y;
    return MainLogic._MainLogic.getCameraHight() * 0.5f - topuiHight;
  }

  void stepPlayMazeTimes(){
    int playertimes = PlayerPrefsManager._PlayerPrefsManager.PlayMazeTimes;
    playertimes++;
    PlayerPrefsManager._PlayerPrefsManager.PlayMazeTimes = playertimes;
    Debug.Log("685 - Current PlayerMazeTimes : " + playertimes);
  }

  void GetAdReward(ItmeType type, int num)
  {
    PlayerPrefsManager._PlayerPrefsManager.GetRewrd(type, num);
    updateUI();
  }

  void updateUI(){
    //��s�D��ƶq
    mRoot.transform.Find("DownUI/bg/Torchbt/amount").GetComponent<TextMeshPro>().text = "X" + PlayerPrefsManager._PlayerPrefsManager.Item1Num;
    mRoot.transform.Find("DownUI/bg/oillampbt/amount").GetComponent<TextMeshPro>().text = "X" + PlayerPrefsManager._PlayerPrefsManager.Item2Num;
  }

  void steplevel(){

      PlayerPrefsManager._PlayerPrefsManager.LightMazeLevel++;

  }

  public float getGametimer(){
    return gametime;
  }
}
