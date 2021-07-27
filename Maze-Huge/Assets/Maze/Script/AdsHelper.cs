using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsHelper : MonoBehaviour
{
  public static AdsHelper _AdsHelper = null;
  public bool inited = false;
  //float Adscale;
  int RectangleBannerSize = 200;
  //ca-app-pub-4959011404007459/2245648839
  //ca-app-pub-4959011404007459/7145200848
  //ca-app-pub-4959011404007459/8513288688
  //ca-app-pub-4959011404007459/2522595401

  CommonAction OnAdClosed = null;
  CommonAction OnRewardAdEarned = null;

  private void Awake()
  {
    _AdsHelper = this;
  }

  public void Update(){
    //if (Input.GetKeyUp(KeyCode.S)){
    //  if (interstitial == null)
    //    return;
    //  interstitial.Destroy();
    //}

    //if (Input.GetKeyUp(KeyCode.B))
    //{
    //  if (bannerView == null)
    //    return;
    //  bannerView.Destroy();
    //}
  }

  public void init()
  {
#if !UNITY_ANDROID
    return;
#endif
    Debug.Log("MobileAds init...");
    MobileAds.Initialize(initStatus =>
    {
      Debug.Log("MobileAds init completed");

      Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
      foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
      {
        string className = keyValuePair.Key;
        AdapterStatus status = keyValuePair.Value;
        switch (status.InitializationState)
        {
          case AdapterState.NotReady:
            // The adapter initialization did not complete.
            Debug.Log("Adapter: " + className + " not ready.");
            break;
          case AdapterState.Ready:
            // The adapter was successfully initialized.
            Debug.Log("Adapter: " + className + " is initialized.");
            break;
        }
      }

      //Adscale = MobileAds.Utils.GetDeviceScale();
      //Debug.Log("645 - GetDeviceScale" + Adscale);

      //EDITOR裡會是0
      //ANDROID就不一定
      //if (Adscale == 0)
      //{
      //  Adscale = 2.0f;
      //}

      inited = true;
      //RequestRectangleBannerAds(null);

    });


    //#if UNITY_ANDROID && !UNITY_EDITOR
    //    AndroidJavaClass upjc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //    AndroidJavaObject jo = upjc.GetStatic<AndroidJavaObject>("currentActivity");

    //    AndroidJavaClass jc =new AndroidJavaClass("com.powergameranger.androidnative.ADSHelper");
    //    jc.CallStatic("Init", jo);       
    //#endif
  }
  private BannerView RectanglebannerView;
  private BannerView bannerView;

  public void RequestRectangleBannerAds(CommonAction onclosed)
  {
#if !UNITY_ANDROID
    return;
#endif
    if (RectanglebannerView != null)
      return;

#if UNITY_ANDROID
    string adUnitId = "ca-app-pub-4959011404007459/2245648839";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-4959011404007459/2245648839";
#else
            string adUnitId = "unexpected_platform";
#endif

    //AdSize size = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth((int)(768 / Adscale));
    //Debug.Log("size.Width" + size.Width);
    //Debug.Log("size.Height" + size.Height);
    //int width = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth).Width;

    //不同裝置有不同的dpi，不同的dpi在使用定義好的adszie(ex : Banner)時，會因為dpi不同，取得到不同大小的Adsize
    //會造成不同size大小的的廣告覆蓋，進而不可控制..
    //但如果使用new AdSize的方式就可以在不同dpi的裝置取得一模一樣大小廣告size...但文件上沒說能接受的size是多少
    //使用不支援的size，將會導致loadfaild..
    //必須靠經驗法則..?
    AdSize size = new AdSize(RectangleBannerSize, RectangleBannerSize);
#if !UNITY_EDITOR && UNITY_ANDROID
    this.RectanglebannerView = new BannerView(adUnitId, size, AdPosition.Bottom);
#elif UNITY_EDITOR && UNITY_ANDROID
    this.RectanglebannerView = new BannerView(adUnitId, AdSize.MediumRectangle, AdPosition.Bottom);
#endif
    // Create a 320x50 banner at the top of the screen.

    //自訂義位置在android上是以左上角為0.0右邊是+X下面是+Y
    //this.RectanglebannerView = new BannerView(adUnitId, AdSize.MediumRectangle, 384, 880);

    // Called when an ad request has successfully loaded.
    this.RectanglebannerView.OnAdLoaded += this.AdLoaded;
    // Called when an ad request failed to load.
    this.RectanglebannerView.OnAdFailedToLoad += this.AdFailedToLoad;
    // Called when an ad is clicked.
    this.RectanglebannerView.OnAdOpening += this.AdClicked;
    // Called when the user returned from the app after an ad click.
    this.RectanglebannerView.OnAdClosed += this.AdClosed;


    // Create an empty ad request.
    AdRequest request = new AdRequest.Builder().Build();

    // Load the banner with the request.
    this.RectanglebannerView.LoadAd(request);

    OnAdClosed = onclosed;

    //#if UNITY_ANDROID && !UNITY_EDITOR
    //    AndroidJavaClass upjc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //    AndroidJavaObject jo = upjc.GetStatic<AndroidJavaObject>("currentActivity");

    //    AndroidJavaClass jc =new AndroidJavaClass("com.powergameranger.androidnative.ADSHelper");
    //    jc.CallStatic("BANNER", jo,gameObject.name);
    //#endif
  }
  public void DismissRectangleBannerAds(){
    if (RectanglebannerView == null)
      return;

    RectanglebannerView.Destroy();
    RectanglebannerView = null;
  }

  public void RequestBannerAds(CommonAction onclosed)
  {
#if !UNITY_ANDROID
    return;
#endif
    if (bannerView != null)
      return;

#if UNITY_ANDROID
    string adUnitId = "ca-app-pub-4959011404007459/5909222360";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-4959011404007459/5909222360";
#else
            string adUnitId = "unexpected_platform";
#endif

    //這種size的只有高度(32、50)會有影響，但是寬度也不能亂給..目前只測出320、AdSize.FullWidth
    AdSize size = new AdSize(AdSize.FullWidth, 32);
#if !UNITY_EDITOR && UNITY_ANDROID
    this.bannerView = new BannerView(adUnitId, size, AdPosition.Bottom);
#elif UNITY_EDITOR && UNITY_ANDROID
    this.bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
#endif
    // Create a 320x50 banner at the top of the screen.
    //this.bannerView = new BannerView(adUnitId, size, AdPosition.Bottom);
    //float scale = MobileAds.Utils.GetDeviceScale();
    //Debug.Log("645 - GetDeviceScale" + scale);
    // Called when an ad request has successfully loaded.
    this.bannerView.OnAdLoaded += this.AdLoaded;
    // Called when an ad request failed to load.
    this.bannerView.OnAdFailedToLoad += this.AdFailedToLoad;
    // Called when an ad is clicked.
    this.bannerView.OnAdOpening += this.AdClicked;
    // Called when the user returned from the app after an ad click.
    this.bannerView.OnAdClosed += this.AdClosed;


    // Create an empty ad request.
    AdRequest request = new AdRequest.Builder().Build();

    // Load the banner with the request.
    this.bannerView.LoadAd(request);

    OnAdClosed = onclosed;

    //#if UNITY_ANDROID && !UNITY_EDITOR
    //    AndroidJavaClass upjc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    //    AndroidJavaObject jo = upjc.GetStatic<AndroidJavaObject>("currentActivity");

    //    AndroidJavaClass jc =new AndroidJavaClass("com.powergameranger.androidnative.ADSHelper");
    //    jc.CallStatic("BANNER", jo,gameObject.name);
    //#endif
  }
  public void DismissBannerAds(){
    if (bannerView == null)
      return;

    bannerView.Destroy();
    bannerView = null;
  }


  private InterstitialAd interstitial;

  public void RequestInterstitialAds()
  {
#if !UNITY_ANDROID
    return;
#endif
    if (interstitial != null)
      return;

#if UNITY_ANDROID
    string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
        string adUnitId = "unexpected_platform";
#endif

    //在 iOS 上，對像是InterstitialAd一次性使用對象。
    //這意味著一旦展示了一個插頁式廣告，該InterstitialAd對象就不能用於加載另一個廣告。要請求另一個插頁式廣告，您需要創建一個新 InterstitialAd對象。
    //if (this.interstitial != null){
    //  this.interstitial = null;
    //}

    // Initialize an InterstitialAd.
    this.interstitial = new InterstitialAd(adUnitId);

    //interstitial 播放期間要暫停音樂音效，或任何遊戲中持續運行且造成玩家影響的功能

    // Called when an ad request has successfully loaded.
    this.interstitial.OnAdLoaded += AdLoaded;
    // Called when an ad request failed to load.
    this.interstitial.OnAdFailedToLoad += AdFailedToLoad;
    // Called when an ad is shown.
    this.interstitial.OnAdOpening += AdClicked;
    // Called when the ad is closed.
    this.interstitial.OnAdClosed += AdClosed;

    // Create an empty ad request.
    AdRequest request = new AdRequest.Builder().Build();
    // Load the interstitial with the request.
    this.interstitial.LoadAd(request);
  }
  public void ShowInterstitialAds(CommonAction OnClosed)
  {

    if (this.interstitial == null)
    {
      Debug.Log("interstitialAds is null Requset it first");
      return;
    }

    if (this.interstitial.IsLoaded())
    {
      OnAdClosed = OnClosed;
      this.interstitial.Show();
      AudioController._AudioController.toggleMusic(false);
    }
  }

  private RewardedAd rewardedAd;
  public void RequestRewardAds()
  {

    if(rewardedAd != null){
      rewardedAd.Destroy();
      rewardedAd = null;
    }


#if UNITY_ANDROID
    string adUnitId = "ca-app-pub-4959011404007459/8513288688";
#elif UNITY_IPHONE
        string adUnitId = "ca-app-pub-4959011404007459/8513288688";
#else
        string adUnitId = "unexpected_platform";
#endif

    //在 iOS 上，對像是InterstitialAd一次性使用對象。
    //這意味著一旦展示了一個插頁式廣告，該InterstitialAd對象就不能用於加載另一個廣告。要請求另一個插頁式廣告，您需要創建一個新 InterstitialAd對象。
    //if (this.interstitial != null){
    //  this.interstitial = null;
    //}

    // Initialize an InterstitialAd.
    this.rewardedAd = new RewardedAd(adUnitId);

    //interstitial 播放期間要暫停音樂音效，或任何遊戲中持續運行且造成玩家影響的功能

    // Called when an ad request has successfully loaded.
    this.rewardedAd.OnAdLoaded += AdLoaded;
    // Called when an ad request failed to load.
    this.rewardedAd.OnAdFailedToLoad += AdFailedToLoad;
    this.rewardedAd.OnAdFailedToShow += AdFailedToShow;

    // Called when an ad is shown.
    this.rewardedAd.OnAdOpening += AdClicked;
    // Called when the ad is closed.
    this.rewardedAd.OnAdClosed += AdClosed;
    // Called when the user should be rewarded for interacting with the ad.
    this.rewardedAd.OnUserEarnedReward += AdEarnedReward;

    // Create an empty ad request.
    AdRequest request = new AdRequest.Builder().Build();
    // Load the rewarded ad with the request.
    this.rewardedAd.LoadAd(request);

  }
  public  void ShowRewardAd(CommonAction OnClosed, CommonAction OnEarned,CommonAction OnFailedLoad)
  {
    OnAdClosed = OnClosed;
    //Debug.Log("519 - ShowRewardAd");

    if (this.rewardedAd == null)
      return;
    //Debug.Log("519 - ShowRewardAd != null");

    if (this.rewardedAd.IsLoaded())
    {
      //Debug.Log("519 - ShowRewardAd IsLoaded");
      this.rewardedAd.Show();
      OnRewardAdEarned = OnEarned;
      AudioController._AudioController.toggleMusic(false);
    }
    else
    {
      Debug.Log("519 - ShowRewardAd Is Not Loaded");
      if (OnFailedLoad != null)
        OnFailedLoad();

      AudioController._AudioController.toggleMusic(true);
    }
  }


  void AdLoaded(object sender, EventArgs args)
  {
    if(this.RectanglebannerView != null){
      Debug.Log("645 GetHeightInPixels : " + this.RectanglebannerView.GetHeightInPixels());
      Debug.Log("645 GetWidthInPixels : " + this.RectanglebannerView.GetWidthInPixels());
    }
    //if(sender.GetType() == (Typeof)GoogleMobileAds.Api.BannerView)
    Debug.Log("AdLoaded Get call back from native");
    //if(sender.GetType() == typeof(GoogleMobileAds.Api.BannerView)){
    //  GameObject go = GameObject.Find("MEDIUM_RECTANGLE(Clone)");
    //  if(go != null){
    //    go.transform.Find("Image").transform.localScale = new Vector3(1 / Adscale, 1 / Adscale, 1.0f);
    //    Debug.Log("645 rescale RECTANGLE : " + 1 / Adscale);
    //    return;
    //  }
    //  go = GameObject.Find("BANNER(Clone)");
    //  if(go != null){
    //    go.transform.Find("Image").transform.localScale = new Vector3(1 / Adscale, 1 / Adscale, 1.0f);
    //    Debug.Log("645 rescale BANNER : " + 1 / Adscale);
    //    return;
    //  }

    //}
  }
  void AdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
  {
    if (OnAdClosed != null){
      OnAdClosed();
      OnAdClosed = null;
    }
    AudioController._AudioController.toggleMusic(true);
    Debug.Log("519 AdFailedToLoad sender : "+ sender .GetType()+ "，Error " + args.LoadAdError);
  }
  // Called when an ad is clicked.
  void AdClicked(object sender, EventArgs args)
  {
    Debug.Log("AdClicked Get call back from native");
  }
  void AdClosed(object sender, EventArgs args)
  {
    //確保destroy()廣告?
    //獎勵式廣告則為玩家強制關閉廣告
    //獎勵式廣告，建議在這邊要求加載下一個獎勵式廣告
    //恢復遊戲運行
    if (sender.GetType() == typeof(GoogleMobileAds.Api.RewardedAd)){
      RequestRewardAds();
      if (OnAdClosed != null){
        OnAdClosed();
        OnAdClosed = null;
      }
      AudioController._AudioController.toggleMusic(true);
    }
    else if(sender.GetType() == typeof(GoogleMobileAds.Api.InterstitialAd)){

      if (OnAdClosed != null){
        OnAdClosed();
        OnAdClosed = null;
      }
      AudioController._AudioController.toggleMusic(true);
    }

    Debug.Log("AdClosed Get call back from native");
  }

  void AdEarnedReward(object sender, Reward args){
    string type = args.Type;
    double amount = args.Amount;
    Debug.Log(
        "HandleRewardedAdRewarded event received for "
                    + amount.ToString() + " " + type);
    if (OnRewardAdEarned != null)
    {
      OnRewardAdEarned();
      OnRewardAdEarned = null;
    }
  }
  void AdFailedToShow(object sender, AdErrorEventArgs args)
  {
    //獎勵廣告無法順利顯示的情況
    //可能就先跳過不讓玩家看廣告與給獎
    if (sender.GetType() == typeof(GoogleMobileAds.Api.RewardedAd))
    {
      //RequestRewardAds();不建議在無法顯示廣告的情況在加載廣告
      if (OnAdClosed != null){
        OnAdClosed();
        OnAdClosed = null;
        Debug.Log("519 AdFailedToShow : " + args);
      }
      AudioController._AudioController.toggleMusic(true);
    }
    else if (sender.GetType() == typeof(GoogleMobileAds.Api.InterstitialAd)){
      AudioController._AudioController.toggleMusic(true);
    }
    Debug.Log("AdFailedToShow : " + args);

  }
}
