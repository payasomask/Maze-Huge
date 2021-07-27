using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour {

  public static AudioController _AudioController = null;

  public enum Channel{
    BGM =0,
    // BGM2, //其它物件以為 AudioController 就只有一個 BGM Channel
    ENV =2,
    LONG_SFX,
    SHORT_SFX,
    REEL_SFX,
    EXTRA_SFX,
    VOICE_SFX,
    NUM
  }

  enum ActualBGMChannel{
    BGM1 =0,
    BGM2 =1
  }
  ActualBGMChannel current_bgm_channel =ActualBGMChannel.BGM1;

  public enum Snapshot{
    General =0,
    Winning,
    LowEnv,
    HiEnv,
    NUM
  }

 class AudioSetup{
    public string audio_name;
    public string file_id;
    public string assetbundle_id;
    public float vol;
  }

  AudioMixerSnapshot[] mAMS =null;
  AudioSource[] mAs =null;

  //file_id <--> AudioSetup
  Dictionary<string, AudioSetup> file_id_audio_setup_mapper =new Dictionary<string, AudioSetup>();

  //audioname <--> file_id
  Dictionary<string, string> audio_file_id_mapper =new Dictionary<string, string>();
  Dictionary<string, Dictionary<string, string>> slot_bgm_file_id_mapper =new Dictionary<string, Dictionary<string, string>>();


  //
  //cache data
  //
  //file_id <--> [audioClip, AudioSetup]
  Dictionary<string, object[]> mAudioClipMap =new Dictionary<string, object[]>();

  string curr_active_slot_bgm_type =null;

  bool mInited =false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    if (mInited==false)
      return;

    if (CrossFading)
    {
      starttime+=Time.deltaTime;
      if (starttime <= mfadetime)
      {
        fadeOutBGMAS.volume = (float)CurveUtil.Linear(starttime, fadeoutstartval, fadeoutchangeval, mfadetime);
        fadeInBGMAS.volume = (float)CurveUtil.Linear(starttime, 0.0f, fadeinchangeval, mfadetime);
      }
      else
      {
        fadeOutBGMAS.volume = 0f;//(float)CurveUtil.Linear(mfadetime, fadeoutstartval, fadeoutchangeval, mfadetime);
        fadeOutBGMAS.Stop();
        fadeInBGMAS.volume = fadeinchangeval;//(float)CurveUtil.Linear(mfadetime, 0.0f, fadeinchangeval, mfadetime);
        CrossFading = false;
        Debug.Log("787 - Fading BGM ended.");
      }
    }

    for (int i=0;i<curr_fade_task.Length;++i){
      FadeTask ft =curr_fade_task[i];
      if (ft !=null){
        ft.fadeInCounter+=Time.deltaTime;
        if (ft.fadeInCounter<ft.fadeInDuration){
          mAs[getActualASIdx(ft.fadeChannel)].volume =(float)CurveUtil.Linear(ft.fadeInCounter, ft.startFadeVol, ft.endFadeVol-ft.startFadeVol, ft.fadeInDuration);
        }else{
          mAs[getActualASIdx(ft.fadeChannel)].volume =ft.endFadeVol;
          curr_fade_task[i] =null;
        }
      }
    }

	}

  public void toggleMusic(bool OnOff){
    //BGM ~ ENV
    for (int i=(int)Channel.BGM;i<(int)Channel.ENV+1;++i){
      mAs[i].mute =!OnOff;
    }
  }

  public void toggleAudio(bool OnOff){
    //LONG_SFX ~ NUM
    for (int i=(int)Channel.LONG_SFX;i<(int)Channel.NUM;++i){
      mAs[i].mute =!OnOff;
    }
  }

  public void setActiveSlotBGMType(string type){
    curr_active_slot_bgm_type =type;
  }

  //string getAudioFileID(string audio_name){
  //  string ext_audio_name =audio_name;
  //  if (ext_audio_name.Contains("sfx_")){
  //    ext_audio_name+=".wav";
  //  }else
  //  if (ext_audio_name.Contains("bgm_slot_")){
      
  //  }else
  //  if (ext_audio_name.Contains("bgm_")){
  //    ext_audio_name+=".ogg";
  //  }
  //  if (audio_file_id_mapper.ContainsKey(ext_audio_name)){
  //    return audio_file_id_mapper[ext_audio_name];
  //  }
  //  if (curr_active_slot_bgm_type !=null){
  //    if (slot_bgm_file_id_mapper[curr_active_slot_bgm_type].ContainsKey(ext_audio_name)){
  //      return slot_bgm_file_id_mapper[curr_active_slot_bgm_type][ext_audio_name];
  //    }
  //  }

  //  Debug.LogError("133 - file id for audio_name : "+audio_name+" not found, (curr_active_slot_bgm_type:"+curr_active_slot_bgm_type+")");
  //  return null;
  //}

  void preloadClip(string file_id){
    if (mAudioClipMap.ContainsKey(file_id)==true)
      return;

    //if (file_id_audio_setup_mapper.ContainsKey(file_id)==false){
    //  Debug.LogError("149 - failed to get AudioSetup data (file_id:"+file_id+")");
    //  return;
    //}

    //find audio clip
    AudioClip ac = AssetbundleLoader._AssetbundleLoader.InstantiateAudio(file_id);

    if (ac !=null){
      mAudioClipMap.Add(file_id, new object[] { ac });
    }else{
      Debug.LogError("154 - failed to preload clip (file_id:"+file_id+", file_name:"+file_id_audio_setup_mapper[file_id].audio_name+", assetbundle:"+file_id_audio_setup_mapper[file_id].assetbundle_id+")");
    }

  }

  public void PurgeAudioClipCache(){
    mAudioClipMap.Clear();
  }

  public void init(){
    if (mInited==true)
      return;

    _AudioController = this;

    mAs =new AudioSource[(int)Channel.NUM];

      AudioMixer am = AssetbundleLoader._AssetbundleLoader.InstantiateAudioMixer("MainMixer");
    if(am != null){
      for (int i = 0; i < (int)Channel.NUM; ++i)
      {
        mAs[i] = gameObject.AddComponent<AudioSource>();
        mAs[i].outputAudioMixerGroup = am.FindMatchingGroups(getMixerChannelPath(i))[0];
        mAs[i].playOnAwake = false;
      }

      mAMS = new AudioMixerSnapshot[(int)Snapshot.NUM];
      for (int i = 0; i < (int)Snapshot.NUM; ++i)
      {
        mAMS[i] = am.FindSnapshot(((Snapshot)i).ToString());
      }
    }

    mInited =true;
    

    bool audio_T = PlayerPrefsManager._PlayerPrefsManager.Audio_T == "on";
    toggleAudio(audio_T);
    bool music_T = PlayerPrefsManager._PlayerPrefsManager.Music_T == "on";
    toggleMusic(music_T);
  }

  public void dispose(){

    if (mInited==true){
      GameObject.Destroy(mAs[0].gameObject);
      // GameObject.Destroy(mSFXAs.gameObject);
    }

    mInited =false;
  }

  int getActualASIdx(Channel channel){
    if (channel ==Channel.BGM){
      return (int)current_bgm_channel;
    }
    return (int)channel;
  }

  string getMixerChannelPath(int idx){
    if (idx >=(int)Channel.NUM)
      return null;
    
    if (idx==1){
      return "Master/BGM2";
    }

    return "Master/"+((Channel)idx).ToString();
  }

  AudioSource fadeOutBGMAS = null;
  AudioSource fadeInBGMAS = null;
  bool CrossFading = false;
  float mfadetime = 0.5f;
  float starttime = 0.0f;
  float fadeoutstartval = 0.0f;
  float fadeoutchangeval = 0.0f;
  float fadeinchangeval = 0.0f;
  float fadedesstartval = 1.0f;
  //BGM crossfade 播放
  public void crossFadeBGM(string audio_name,bool loop = false,float fadeouttime = 1f)
  {
    if (mInited==false)
      return;

    if (getCurrentPlayingAudioClip(Channel.BGM)==audio_name){
      Debug.LogWarning("211 - cross fade to the same BGM ("+audio_name+"), ignore process,...");
      return;
    }

    //string file_id =getAudioFileID(audio_name);
    //if (file_id ==null){
    //  Debug.LogWarning("308 - failed to play audio : " + audio_name+" (file id not found)");
    //  return;
    //}
    preloadClip(audio_name);
    if (mAudioClipMap.ContainsKey(audio_name) == false)
    {
      Debug.LogWarning("135 - failed to play audio : " + audio_name);
      return;
    }

    Debug.LogWarning("254 - cross fade bgm from "+getCurrentPlayingAudioClip(Channel.BGM)+" to "+audio_name);    

    float vol =find_audio_vol_fid(audio_name);

    fadeOutBGMAS =mAs[getActualASIdx(Channel.BGM)];
    fadeInBGMAS =mAs[(getActualASIdx(Channel.BGM)+1)%2];
    // Debug.Log("785 - CurrentfadeOutBGMAS :" + fadeOutBGMAS.outputAudioMixerGroup.name);
    // Debug.Log("785 - CurrentfadeInBGMAS :" + fadeInBGMAS.outputAudioMixerGroup.name);
    current_bgm_channel =(ActualBGMChannel)((getActualASIdx(Channel.BGM)+1)%2); //SWITCH ACTUAL BGM CHANNEL

    fadeInBGMAS.clip = (AudioClip)mAudioClipMap[audio_name][0];
    fadeInBGMAS.clip.name = audio_name;
    fadeInBGMAS.loop = loop;
    fadeInBGMAS.time =0.0f;
    fadeInBGMAS.Play();

    CrossFading = true;
    
    mfadetime = fadeouttime;
    starttime = 0f;

    fadeoutstartval = fadeOutBGMAS.volume;
    fadeoutchangeval = 0.0f - fadeoutstartval;
    //fadein的AS要到多大聲之後會由DB控制，先預設為1.0;
    fadedesstartval = vol;
    fadeinchangeval = fadedesstartval - 0.0f;
    // Debug.Log("788 - Start Fading BGM..");
  }

  public float find_audio_vol(string filename){
    //string file_id =getAudioFileID(filename);
    if (filename != null){
      return find_audio_vol_fid(filename);
    }
    Debug.Log("374 - failed to find audio vol (filename="+filename+")");
    return 1f;
  }

  float find_audio_vol_fid(string file_id){
    float vol =1f;
    //if (file_id_audio_setup_mapper.ContainsKey(file_id)){
    //  vol =file_id_audio_setup_mapper[file_id].vol;
    //}else{
    //  Debug.LogError("339 - vol data not found (file_id="+file_id+")");
    //}
    return vol;
  }

  class FadeTask{
    public enum Type{
      FADEOUT,
      FADEIN
    }
    public Type fade_type =Type.FADEIN;
    public Channel fadeChannel =Channel.BGM;

    public float fadeInDuration =0f;
    public float fadeInCounter =0f;

    public float startFadeVol =0f;
    public float endFadeVol =0f;
  }
  FadeTask[] curr_fade_task =new FadeTask[3]; //fade task for BGM/ENV channels

  public void fadeIn(string audio_name, Channel c, float duration =1f){
    if (mInited==false)
      return;

    if (!(c ==Channel.BGM || c ==Channel.ENV)){
      Debug.Log("319 - support ENV, BGM channel only");
      return;
    }

    //string file_id =getAudioFileID(audio_name);
    //if (file_id ==null){
    //  Debug.LogWarning("308 - failed to play audio : " + audio_name+" (file id not found)");
    //  return;
    //}
    preloadClip(audio_name);
    if (mAudioClipMap.ContainsKey(audio_name) ==false){
      Debug.LogWarning("135 - failed to play audio : "+audio_name);
      return;
    }

    stop(c);

    mAs[getActualASIdx(c)].clip =(AudioClip)mAudioClipMap[audio_name][0];
    mAs[getActualASIdx(c)].clip.name =audio_name;
    mAs[getActualASIdx(c)].loop =true;
    mAs[getActualASIdx(c)].time =0f;
    mAs[getActualASIdx(c)].volume =0f; //initial volume set to zero
    mAs[getActualASIdx(c)].Play();

    FadeTask ft =new FadeTask();
    ft.startFadeVol =0f;
    ft.endFadeVol =find_audio_vol_fid(audio_name);

    ft.fadeChannel =c;

    ft.fadeInDuration =duration;
    ft.fadeInCounter =0f;
    ft.fade_type =FadeTask.Type.FADEIN;
    curr_fade_task[(int)c] =ft;

  }

  public void fadeOut(Channel c, float duration =1f){
    if (mInited==false)
      return;
    
    if (!(c ==Channel.BGM || c ==Channel.ENV)){
      Debug.Log("319 - support ENV, BGM channel only");
      return;
    }

    if (isIdle(c)==true){
      return;
    }

    FadeTask ft =new FadeTask();
    ft.startFadeVol =mAs[getActualASIdx(c)].volume;
    ft.endFadeVol =0f;

    ft.fadeChannel =c;

    ft.fadeInDuration =duration;
    ft.fadeInCounter =0f;
    ft.fade_type =FadeTask.Type.FADEOUT;
    curr_fade_task[(int)c] =ft;

  }

  public void fadeInENV(string audio_name){
    fadeIn(audio_name, Channel.ENV);
  }

  public void fadeOutENV(){
    fadeOut(Channel.ENV);
  }

  //假如 queue ==false, 則直接 break 目前的 play
  public void play(string audio_name, bool loop =false, bool queue =false, Channel c =Channel.BGM, float start_position =0f){
    if (mInited==false)
      return;

    if (getCurrentPlayingAudioClip(c)==audio_name)
      return;

    //string file_id =getAudioFileID(audio_name);
    //if (file_id ==null){
    //  Debug.LogWarning("308 - failed to play audio : " + audio_name+" (file id not found)");
    //  return;
    //}
    preloadClip(audio_name);
    if (mAudioClipMap.ContainsKey(audio_name) ==false){
      Debug.LogWarning("135 - failed to play audio : "+audio_name);
      return;
    }

    //stop fading
    if (c ==Channel.BGM){
      CrossFading =false;
      curr_fade_task[(int)c] =null; //clear fade task
    }
    if (c ==Channel.ENV){
      curr_fade_task[(int)c] =null; //clear fade task
    }

    stop(c);

    float vol =find_audio_vol_fid(audio_name);

    //play now
    mAs[getActualASIdx(c)].volume =vol;
    mAs[getActualASIdx(c)].clip =(AudioClip)mAudioClipMap[audio_name][0];
    mAs[getActualASIdx(c)].clip.name =audio_name;
    mAs[getActualASIdx(c)].loop =loop;
    mAs[getActualASIdx(c)].time =start_position;
    mAs[getActualASIdx(c)].Play();

  }

  public void playOverlapEffect(string audio_name, Channel c=Channel.SHORT_SFX, bool pitchscaleble = false){
    if (mInited==false)
      return;

    if (audio_name.Length==0){
      Debug.LogError("527 - calling playOverlapEffect with empty audio_name !");
      return;
    }

    if (c ==Channel.BGM){
      Debug.LogError("306 - cannot play overlapeffect on the BGM channel !");
      return;
    }

    //string file_id =getAudioFileID(audio_name);
    //if (file_id ==null){
    //  Debug.LogWarning("308 - failed to play audio : " + audio_name+" (file id not found)");
    //  return;
    //}
    preloadClip(audio_name);
    if (mAudioClipMap.ContainsKey(audio_name) ==false){
      Debug.LogWarning("169 - failed to play audio : "+audio_name);
      return;
    }

    float vol =find_audio_vol_fid(audio_name);
    mAs[getActualASIdx(c)].volume =1f; //reset basic volume gain
    mAs[getActualASIdx(c)].outputAudioMixerGroup.audioMixer.SetFloat("Pitch Shifter", 1.0f);//reset Pitch Shifter
    mAs[getActualASIdx(c)].outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1.0f);//reset Pitch
    if (pitchscaleble){
    mAs[getActualASIdx(c)].outputAudioMixerGroup.audioMixer.SetFloat("Pitch Shifter", 1.0f / Time.timeScale);
    mAs[getActualASIdx(c)].outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1.0f * Time.timeScale);
    }
    mAs[getActualASIdx(c)].PlayOneShot((AudioClip)mAudioClipMap[audio_name][0], vol);
  }

  public void stop(Channel c, bool stopOneShotEffect =false){
    if (mInited==false)
      return;
    Debug.Log("334 - stop playing at channel "+c);

    //ugly way to stop playoneshot and clip play
    if (stopOneShotEffect){
      mAs[getActualASIdx(c)].enabled =false;
      mAs[getActualASIdx(c)].enabled =true;
    }

    mAs[getActualASIdx(c)].Stop();
  }

  public bool isIdle(Channel c){
    if (mInited==false || mAs==null || mAs[getActualASIdx(c)] ==null)
      return true;

    return !(mAs[getActualASIdx(c)].isPlaying);
    
  }

  public void mute(Channel c,bool mute){
    Debug.Log("384 - channel : " + c + ", mute : " + mute);
    mAs[getActualASIdx(c)].mute = mute;
  }

  public string getCurrentPlayingAudioClip(Channel c){
    if (mInited==false || mAs==null || mAs[getActualASIdx(c)] ==null || mAs[getActualASIdx(c)].clip ==null){
      return null;
    }

    if (mAs[getActualASIdx(c)].isPlaying==false)
      return null;
      
    return mAs[getActualASIdx(c)].clip.name;
  }

  public float getCurrentPlayingPosition(Channel c =Channel.BGM){
    if (c !=Channel.BGM || mAs==null || mAs[getActualASIdx(c)] ==null || mAs[getActualASIdx(c)].clip ==null)
      return 0f;

    return mAs[getActualASIdx(c)].time;
  }

  public void setSnapshot(Snapshot s, float duration){
    if (mInited==false)
      return;
    
    Debug.Log("202 - setSnapshot "+s);
    mAMS[(int)s].TransitionTo(duration);
  }

  public void setVolume(float v, Channel c){
    mAs[getActualASIdx(c)].volume =v;
  }

  public void setPitch(float p, Channel c){
    mAs[getActualASIdx(c)].pitch =p;
  }


}
