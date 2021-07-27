//#if UNITY_EDITOR
//using UnityEditor;
//#endif

//using UnityEngine.Audio;
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using System;
//using System.Reflection;

//public class CustomAssetBundleManifest
//{
//  Dictionary<string, List<string>> mdic = null;
//  public CustomAssetBundleManifest(object raw_dic)
//  {
//    Dictionary<string, object> dic = (Dictionary<string, object>)raw_dic;
//    mdic = new Dictionary<string, List<string>>();
//    foreach (KeyValuePair<string, object> item in dic)
//    {
//      List<object> list = (List<object>)item.Value;
//      List<string> str_list = new List<string>();
//      for (int i = 0; i < list.Count; ++i)
//        str_list.Add((string)list[i]);

//      mdic.Add(item.Key, str_list);
//    }
//  }

//  public string[] GetAllDependencies(string asset_name)
//  {
//    if (mdic == null)
//      return null;

//    if (mdic.ContainsKey(asset_name) == false)
//    {
//      return new string[] { };
//    }
//    return mdic[asset_name].ToArray();
//  }
//}
//public class AssetBundleMeta
//{

//  public class Payload
//  {
//    public string filename = null;
//    public string related_assetbundle_name = null;

//    public string[] asset_name = null;
//  }
//  public Int64 magic;
//  public Int64 bootloader_version;
//  public Int64 script_version;
//  public bool development_version;
//  public string patch_version = null;
//  public string script_paylod = null;
//  public Payload[] property_payload = null;
//  public Payload[] general_payload = null;
//  public Payload[] audio_payload = null;

//  static public AssetBundleMeta parse(Dictionary<string, object> raw)
//  {
//    AssetBundleMeta tmpABM = new AssetBundleMeta();
//    if (raw == null)
//      return null;

//    try
//    {
//      tmpABM.magic = (Int64)raw["magic"];
//      tmpABM.bootloader_version = (Int64)raw["bootloader_version"];
//      tmpABM.script_version = (Int64)raw["script_version"];
//      tmpABM.development_version = (bool)raw["development_version"];
//      tmpABM.patch_version = (string)raw["patch_version"];

//      Dictionary<string, object> payload_dic = (Dictionary<string, object>)raw["payload"];

//      //script
//      tmpABM.script_paylod = (string)payload_dic["script"];

//      //property
//      List<Payload> tmpPayload = new List<Payload>();
//      Dictionary<string, object> property_dic = (Dictionary<string, object>)payload_dic["property"];
//      foreach (KeyValuePair<string, object> entry in property_dic)
//      {
//        Payload p = new Payload();
//        p.filename = entry.Key;
//        p.asset_name = new string[] { (string)entry.Value };
//        p.related_assetbundle_name = entry.Key.Replace("_property", "");

//        tmpPayload.Add(p);
//      }
//      tmpABM.property_payload = tmpPayload.ToArray();

//      //general
//      tmpPayload.Clear();
//      Dictionary<string, object> general_dic = (Dictionary<string, object>)payload_dic["general"];
//      foreach (KeyValuePair<string, object> entry in general_dic)
//      {
//        Payload p = new Payload();
//        p.filename = entry.Key;
//        p.related_assetbundle_name = entry.Key;

//        List<object> prefabList = (List<object>)entry.Value;
//        p.asset_name = new string[prefabList.Count];
//        for (int i = 0; i < prefabList.Count; ++i)
//        {
//          p.asset_name[i] = (string)prefabList[i];
//        }

//        tmpPayload.Add(p);
//      }
//      tmpABM.general_payload = tmpPayload.ToArray();

//      //audio
//      tmpPayload.Clear();
//      Dictionary<string, object> audio_dic = (Dictionary<string, object>)payload_dic["audio"];
//      foreach (KeyValuePair<string, object> entry in audio_dic)
//      {
//        Payload p = new Payload();
//        p.filename = entry.Key;
//        p.related_assetbundle_name = entry.Key;

//        List<object> prefabList = (List<object>)entry.Value;
//        p.asset_name = new string[prefabList.Count];
//        for (int i = 0; i < prefabList.Count; ++i)
//        {
//          p.asset_name[i] = (string)prefabList[i];
//        }

//        tmpPayload.Add(p);
//      }
//      tmpABM.audio_payload = tmpPayload.ToArray();
//    }
//    catch (Exception e)
//    {
//      Debug.LogError(e.ToString());
//      return null;
//    }

//    return tmpABM;
//  }
//}

//public class AssetbundleLoader
//{

//#if UNITY_EDITOR
//  bool bAdvancedLog = true;
//#else
//  bool bAdvancedLog =false;
//#endif

//  class PrefabMap
//  {
//    public string assetBundleName = null;
//    public string assetName = null;
//  }

//  enum LoadingStatus
//  {
//    NULL,

//    //EDITORMODE
//    EDITORMODE,

//    //CUSTOM ASSETS
//    META,
//    // META_FROM_WWW,
//    SCRIPT_PAYLOAD,
//    PROPERTY_PAYLOAD,

//    //GENERAL ASSETS
//    MANIFEST,
//    INDEX_ASSETS,
//    // INDEX_FILM_ASSETS,

//    //DONE
//    DONE,
//    ERROR
//  }
//  LoadingStatus mStatus = LoadingStatus.NULL;

//  //assetbundle info.
//  AssetBundleMeta mMeta = null;
//  Assembly mScript = null;
//  Dictionary<string, Dictionary<string, Dictionary<int, string[]>>> mProperty = null; //<prefab name, <gameobject name, <components id, [components property]>>> 
//  CustomAssetBundleManifest mABM = null;
//  Dictionary<string, PrefabMap> mPrefabMap = null; // prefab name <--> assetbundle name

//  Dictionary<string, AssetBundle> mLoadedAssetBundle = new Dictionary<string, AssetBundle>();

//  bool mEditorMode = false;

//  MiniMessagePack.MiniMessagePacker msgpack = new MiniMessagePack.MiniMessagePacker();

//  // WWW mTmpWWW =null;

//#if UNITY_EDITOR
//  Dictionary<string, string> mAssetNamePathMapper = new Dictionary<string, string>();
//#endif

//  public bool isError()
//  {
//    return (mStatus == LoadingStatus.ERROR);
//  }

//  public void setEditorMode()
//  {
//#if UNITY_EDITOR
//    mEditorMode = true;

//    var names = AssetDatabase.GetAllAssetBundleNames();
//    foreach (var name in names)
//    {
//      string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(name);
//      foreach (string asset in assets)
//      {
//        if (bAdvancedLog)
//          Debug.Log("map prefab with name =" + Path.GetFileName(asset));
//        string key = Path.GetFileName(asset)
//          .Replace(".prefab", "")
//          .Replace(".png", "")
//          .Replace(".mp3", "")
//          .Replace(".ogg", "")
//          .Replace(".wav", "")
//          .Replace(".mixer", "")
//          .Replace(".asset", "")
//          .Replace(".json", "");
//        if (mAssetNamePathMapper.ContainsKey(key) == true)
//        {
//          Debug.LogError("184 - asset name has already exist (" + key + ")");
//        }
//        else
//        {
//          mAssetNamePathMapper.Add(key, asset);
//        }
//      }

//    }

//    mStatus = LoadingStatus.DONE;
//#endif
//  }

//  FileSync tmp_fs = null;
//  public void verifyAssetBundle(FileSync fs)
//  {
//    if (mStatus != LoadingStatus.NULL)
//      return;

//    tmp_fs = fs;
//    mStatus = LoadingStatus.META;
//  }

//  float last_time_1 = 0f;
//  // float last_time_2 =0f;

//  //true : load done
//  public bool update()
//  {
//    if (mStatus == LoadingStatus.META)
//    {

//      mMeta = AssetBundleMeta.parse(loadMetaFile());
//      if (mMeta == null)
//      {
//        if (bAdvancedLog)
//          Debug.LogError("52 - loadMetaFile return null");

//        mStatus = LoadingStatus.ERROR;
//        return true;
//      }

//      Debug.Log("239 - current scripting backend is " + GameObject.Find("Bootloader").GetComponent<Bootloader>().currScriptingBackend);
//      if (GameObject.Find("Bootloader").GetComponent<Bootloader>().currScriptingBackend == Bootloader.ScriptingBackend.IL2CPP)
//      {
//        mStatus = LoadingStatus.PROPERTY_PAYLOAD;
//      }
//      else
//      {
//        mStatus = LoadingStatus.SCRIPT_PAYLOAD;
//      }
//    }
//    else
//    // if (mStatus ==LoadingStatus.META_FROM_WWW){
//    //   if (!string.IsNullOrEmpty(mTmpWWW.error)){
//    //     if (bAdvancedLog)
//    //       Debug.LogError("171 - "+mTmpWWW.error.ToString());
//    //     mStatus =LoadingStatus.ERROR;
//    //     return true;
//    //   }
//    //   if (mTmpWWW.isDone){
//    //     mMeta =AssetBundleMeta.parse( msgpack.Unpack(mTmpWWW.bytes) as Dictionary<string, object> );
//    //     if (mMeta==null){
//    //       if (bAdvancedLog)
//    //         Debug.LogError("178 - loadMetaFile return null");
//    //       mStatus =LoadingStatus.ERROR;
//    //       return true;
//    //     }

//    //     mStatus =LoadingStatus.SCRIPT_PAYLOAD;
//    //   }else{
//    //     Debug.Log("246 - meta download progress ("+mTmpWWW.progress+")");
//    //   }

//    // }else

//    //CUSTOM ASSETS
//    if (mStatus == LoadingStatus.SCRIPT_PAYLOAD)
//    {
//      //find script asset name
//      if (mMeta.script_paylod != null)
//      {
//        Debug.Log("255 - [MONO Scripting Backend] load scripts...");
//        last_time_1 = Time.realtimeSinceStartup;
//        mScript = loadScripts(mMeta.script_paylod);
//        if (mScript == null)
//        {
//          if (bAdvancedLog)
//            Debug.LogError("64 - loadScripts return null");
//          mStatus = LoadingStatus.ERROR;
//          return true;
//        }
//      }

//      Debug.Log("255 - load scripts done. (" + (Time.realtimeSinceStartup - last_time_1) + " sec(s) taken.");
//      mStatus = LoadingStatus.PROPERTY_PAYLOAD;
//    }
//    else
//    if (mStatus == LoadingStatus.PROPERTY_PAYLOAD)
//    {
//      //recursive load property payload
//      Debug.Log("255 - generate property list...");
//      last_time_1 = Time.realtimeSinceStartup;
//      for (int i = 0; i < mMeta.property_payload.Length; ++i)
//      {
//        Dictionary<string, Dictionary<string, Dictionary<int, string[]>>> tmp_dic = loadProperty(mMeta.property_payload[i]);
//        foreach (KeyValuePair<string, Dictionary<string, Dictionary<int, string[]>>> entry in tmp_dic)
//        {
//          if (mProperty == null)
//            mProperty = new Dictionary<string, Dictionary<string, Dictionary<int, string[]>>>();

//          mProperty.Add(entry.Key.Replace(".prefab", ""), entry.Value);
//        }
//      }

//      Debug.Log("284 - generate property list done. (" + (Time.realtimeSinceStartup - last_time_1) + " sec(s) taken.");
//      mStatus = LoadingStatus.MANIFEST;

//    }
//    else

//    //GENERAL ASSETS
//    if (mStatus == LoadingStatus.MANIFEST)
//    {
//      Debug.Log("255 - load manifest...");
//      last_time_1 = Time.realtimeSinceStartup;
//      string strManifestPath = getFilePath("StreamingAssets");

//      if (bAdvancedLog)
//        Debug.Log("96 - load AssetBundleManifest file from " + strManifestPath);
//      // AssetBundle tmpAB =AssetBundle.LoadFromFile(strManifestPath);
//      // mABM =(AssetBundleManifest)tmpAB.LoadAsset("assetbundlemanifest");
//      // tmpAB.Unload(false);
//      try
//      {
//        mABM = new CustomAssetBundleManifest((new MiniMessagePack.MiniMessagePacker()).Unpack(System.IO.File.ReadAllBytes(strManifestPath)));
//      }
//      catch (Exception e)
//      {
//        Debug.Log("378 - " + e.ToString());
//        mStatus = LoadingStatus.ERROR;
//        return true;
//      }

//      Debug.Log("255 - load manifest done. (" + (Time.realtimeSinceStartup - last_time_1) + " sec(s) taken.");

//      mStatus = LoadingStatus.INDEX_ASSETS;
//    }
//    else
//    if (mStatus == LoadingStatus.INDEX_ASSETS)
//    {

//      Debug.Log("255 - generate prefabmap...");
//      last_time_1 = Time.realtimeSinceStartup;

//      if (mPrefabMap == null)
//        mPrefabMap = new Dictionary<string, PrefabMap>();

//      for (int i = 0; i < mMeta.general_payload.Length; ++i)
//      {
//        AssetBundleMeta.Payload p = mMeta.general_payload[i];

//        for (int j = 0; j < p.asset_name.Length; ++j)
//        {
//          PrefabMap pm = new PrefabMap();
//          pm.assetBundleName = p.filename;
//          pm.assetName = p.asset_name[j];

//          string prefabName = Path.GetFileName(pm.assetName)
//            .Replace(".prefab", "")
//            .Replace(".png", "")
//            .Replace(".wav", "")
//            .Replace(".mixer", "")
//            .Replace(".mp3", "")
//            .Replace(".ogg", "")
//            .Replace(".asset", "")
//            .Replace(".json", "");

//          if (bAdvancedLog)
//            Debug.Log("183 - " + prefabName + ", path=" + pm.assetBundleName + "[" + pm.assetName + "]");
//          mPrefabMap.Add(prefabName, pm);
//        }
//      }

//      for (int i = 0; i < mMeta.audio_payload.Length; ++i)
//      {
//        AssetBundleMeta.Payload p = mMeta.audio_payload[i];

//        for (int j = 0; j < p.asset_name.Length; ++j)
//        {
//          PrefabMap pm = new PrefabMap();
//          pm.assetBundleName = p.filename;
//          pm.assetName = p.asset_name[j];

//          string prefabName = Path.GetFileName(pm.assetName);

//          if (bAdvancedLog)
//            Debug.Log("375 - " + prefabName + ", path=" + pm.assetBundleName + "[" + pm.assetName + "]");
//          mPrefabMap.Add(prefabName, pm);
//        }
//      }

//      Debug.Log("255 - generate prefabmap done. (" + (Time.realtimeSinceStartup - last_time_1) + " sec(s) taken).");
//      mStatus = LoadingStatus.DONE;

//    }
//    else
//    if (mStatus == LoadingStatus.DONE)
//    {
//      //PROCESS DONE
//      return true;
//    }

//    return false;
//  }

//  public int loadingProgress()
//  {
//    return 0;
//  }

//  public GameObject InstantiatePrefab(string prefabName)
//  {
//    if (mEditorMode)
//    {
//#if UNITY_EDITOR
//      if (mAssetNamePathMapper.ContainsKey(prefabName) == false)
//      {
//        Debug.LogWarning("prefab " + prefabName + " not found");
//        return null;
//      }
//      return GameObject.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath<GameObject>(mAssetNamePathMapper[prefabName]));
//#endif
//    }

//    if (mPrefabMap.ContainsKey(prefabName) == false)
//      return null;

//    GameObject tmp_go = loadAsset(prefabName);
//    if (bAdvancedLog)
//      Debug.Log("280 - load prefab " + prefabName);

//    return tmp_go;
//  }

//  public TMPro.TMP_FontAsset InstantiateFontAsset(string font_asset_name)
//  {
//    if (mEditorMode)
//    {
//#if UNITY_EDITOR
//      if (mAssetNamePathMapper.ContainsKey(font_asset_name) == false)
//      {
//        Debug.LogWarning("font asset " + font_asset_name + " not found");
//        return null;
//      }
//      return (TMPro.TMP_FontAsset)AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(mAssetNamePathMapper[font_asset_name]);
//#endif
//    }

//    if (mPrefabMap.ContainsKey(font_asset_name) == false)
//      return null;

//    TMPro.TMP_FontAsset tmp_go = (TMPro.TMP_FontAsset)loadScriptableObject(font_asset_name);
//    if (bAdvancedLog)
//      Debug.Log("280 - load fonat asset " + font_asset_name);

//    return tmp_go;
//  }

//  public TextAsset InstantiateTextAsset(string asset_name)
//  {
//    if (mEditorMode)
//    {
//#if UNITY_EDITOR
//      if (mAssetNamePathMapper.ContainsKey(asset_name) == false)
//      {
//        Debug.LogWarning("textasset " + asset_name + " not found");
//        return null;
//      }
//      return (TextAsset)AssetDatabase.LoadAssetAtPath<TextAsset>(mAssetNamePathMapper[asset_name]);
//#endif
//    }

//    if (mPrefabMap.ContainsKey(asset_name) == false)
//      return null;

//    TextAsset tmp_go = (TextAsset)loadTextAsset(asset_name);
//    if (bAdvancedLog)
//      Debug.Log("280 - load text asset " + asset_name);

//    return tmp_go;

//  }

//  public GameObject InstantiateScript(string gameobject_name, string script_name)
//  {
//    if (mEditorMode)
//    {
//#if UNITY_EDITOR
//      GameObject g = new GameObject();
//      g.name = gameobject_name;
//      g.AddComponent(Type.GetType(script_name));

//      return g;
//#endif
//    }

//    GameObject new_go = null;
//    if (GameObject.Find("Bootloader").GetComponent<Bootloader>().currScriptingBackend == Bootloader.ScriptingBackend.IL2CPP)
//    {
//      new_go = new GameObject();
//      new_go.name = gameobject_name;
//      new_go.AddComponent(Type.GetType(script_name));

//    }
//    else
//    {
//      if (mScript.GetType(script_name) == null)
//        return null;

//      new_go = new GameObject();
//      new_go.name = gameobject_name;
//      new_go.AddComponent(mScript.GetType(script_name));
//    }
//    return new_go;
//  }

//  Dictionary<string, AudioClip> mAudioClipCache = new Dictionary<string, AudioClip>();
//  public void PurgeAudioClipCache()
//  {
//    mAudioClipCache.Clear();
//  }

//  public AudioClip InstantiateAudio(string audio_clip_name, bool fromCache = true)
//  {

//    if (fromCache && mAudioClipCache.ContainsKey(audio_clip_name))
//    {
//      return mAudioClipCache[audio_clip_name];
//    }

//    if (mEditorMode)
//    {
//#if UNITY_EDITOR

//      if (mMeta == null)
//      {
//        string meta_path = Path.Combine(Path.Combine(Application.temporaryCachePath, "history"), Path.GetFileName("meta"));
//        if (File.Exists(meta_path) == false)
//        {
//          //download meta file
//          string url1 = GameObject.Find("Bootloader").GetComponent<Bootloader>().url;
//          if (GameObject.Find("Bootloader").GetComponent<Bootloader>().tmpReleaseType == Bootloader.ReleaseType.DEVELOPMENT)
//          {
//            url1 += "_" + GameObject.Find("Bootloader").GetComponent<Bootloader>().customUrl;
//          }

//          WWW w2 = new WWW(url1 + "/meta");
//          while (w2.isDone == false)
//          {
//            //wait process...
//          }

//          byte[] raw_meta = null;
//          if (w2.error != null || w2.bytes.Length == 0)
//          {
//            //...
//          }
//          else
//          {
//            raw_meta = w2.bytes;
//          }
//          w2.Dispose();

//          if (raw_meta != null)
//          {
//            System.IO.File.WriteAllBytes(meta_path, raw_meta);
//          }
//          else
//          {
//            return null;
//          }
//        }

//        //load meta file
//        Dictionary<string, object> meta_dic = (Dictionary<string, object>)msgpack.Unpack(File.ReadAllBytes(meta_path));
//        mMeta = AssetBundleMeta.parse(meta_dic);
//        if (mMeta == null)
//        {
//          return null;
//        }
//      }

//      //find and load assetbundle from history folder ?
//      string assetbundle_name = null;
//      for (int i = 0; i < mMeta.audio_payload.Length; ++i)
//      {
//        AssetBundleMeta.Payload p = mMeta.audio_payload[i];
//        for (int j = 0; j < p.asset_name.Length; ++j)
//        {
//          if (p.asset_name[j] == audio_clip_name)
//          {
//            assetbundle_name = p.related_assetbundle_name;
//            break;
//          }
//        }
//      }

//      string assetbundle_local_path = Path.Combine(Application.persistentDataPath, "audio_assetbundle");
//      if (assetbundle_name != null)
//      {
//        AssetBundle tmp_ab2 = null;
//        if (mLoadedAssetBundle.ContainsKey(assetbundle_name) == false)
//        {

//          tmp_ab2 = AssetBundle.LoadFromFile(Path.Combine(assetbundle_local_path, assetbundle_name));
//          if (tmp_ab2 != null)
//          {
//            mLoadedAssetBundle.Add(assetbundle_name, tmp_ab2);
//          }
//        }
//        else
//        {
//          tmp_ab2 = mLoadedAssetBundle[assetbundle_name];

//        }

//        if (tmp_ab2 == null)
//        {
//          Debug.LogError("519 - failed to load assetbundle file : " + Path.Combine(assetbundle_local_path, assetbundle_name));
//          return null;
//        }
//        AudioClip tmp_ac2 = tmp_ab2.LoadAsset<AudioClip>(audio_clip_name);
//        mAudioClipCache[audio_clip_name] = tmp_ac2;
//        return tmp_ac2;
//      }
//      else
//      {
//        Debug.LogError("522 - assetbundle not found, file_id=" + audio_clip_name);
//      }

//      return null;

//#endif
//    }

//    if (mPrefabMap.ContainsKey(audio_clip_name) == false)
//    {
//      Debug.LogWarning("379 - audio clip : " + audio_clip_name + " not found");
//      return null;
//    }

//    AssetBundle tmp_ab = null;
//    if (mLoadedAssetBundle.ContainsKey(mPrefabMap[audio_clip_name].assetBundleName) == false)
//    {
//      string ab_path = getFilePath(mPrefabMap[audio_clip_name].assetBundleName);
//      tmp_ab = AssetBundle.LoadFromFile(ab_path);
//      if (tmp_ab != null)
//        mLoadedAssetBundle.Add(mPrefabMap[audio_clip_name].assetBundleName, tmp_ab);
//    }
//    else
//    {
//      tmp_ab = mLoadedAssetBundle[mPrefabMap[audio_clip_name].assetBundleName];
//    }

//    AudioClip tmp_ac = tmp_ab.LoadAsset<AudioClip>(mPrefabMap[audio_clip_name].assetName);
//    mAudioClipCache[audio_clip_name] = tmp_ac;

//    return tmp_ac;

//  }

//  public AudioMixer InstantiateAudioMixer(string audio_mixer_name)
//  {
//    if (mEditorMode)
//    {
//#if UNITY_EDITOR
//      if (mAssetNamePathMapper.ContainsKey(audio_mixer_name) == false)
//      {
//        if (bAdvancedLog)
//          Debug.LogWarning("369 - audio mixer : " + audio_mixer_name + " not found");
//        return null;
//      }

//      return AssetDatabase.LoadAssetAtPath<AudioMixer>(mAssetNamePathMapper[audio_mixer_name]);
//#endif
//    }

//    if (mPrefabMap.ContainsKey(audio_mixer_name) == false)
//    {
//      Debug.LogWarning("379 - audio mixer : " + audio_mixer_name + " not found");
//      return null;
//    }

//    AssetBundle tmp_ab = null;
//    if (mLoadedAssetBundle.ContainsKey(mPrefabMap[audio_mixer_name].assetBundleName) == false)
//    {
//      string ab_path = getFilePath(mPrefabMap[audio_mixer_name].assetBundleName);
//      tmp_ab = AssetBundle.LoadFromFile(ab_path);
//      if (tmp_ab != null)
//        mLoadedAssetBundle.Add(mPrefabMap[audio_mixer_name].assetBundleName, tmp_ab);
//    }
//    else
//    {
//      tmp_ab = mLoadedAssetBundle[mPrefabMap[audio_mixer_name].assetBundleName];
//    }

//    return tmp_ab.LoadAsset<AudioMixer>(mPrefabMap[audio_mixer_name].assetName);

//  }

//  Dictionary<string, Sprite> mSpriteCache = new Dictionary<string, Sprite>();
//  public void PurgeSpriteCache()
//  {
//    mSpriteCache.Clear();
//  }

//  public Sprite InstantiateSprite(string atlasName, string spriteName, bool fromCache = true)
//  {
//    if (atlasName == null || spriteName == null)
//      return null;

//    if (fromCache && mSpriteCache.ContainsKey(atlasName + "_" + spriteName))
//    {
//      return mSpriteCache[atlasName + "_" + spriteName];
//    }

//    if (mEditorMode)
//    {
//#if UNITY_EDITOR

//      if (mAssetNamePathMapper.ContainsKey(atlasName) == false)
//      {
//        if (atlasName.Length > 0)
//          Debug.LogWarning("309 - atlas:" + atlasName + " not found");
//        return null;
//      }

//      UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(mAssetNamePathMapper[atlasName]);
//      foreach (UnityEngine.Object obj in sprites)
//      {
//        if (obj.name == spriteName)
//        {
//          mSpriteCache[atlasName + "_" + spriteName] = (Sprite)obj;
//          return (Sprite)obj;
//        }
//      }

//      Debug.LogError("sprite not found (atlas:" + atlasName + ", sprite:" + spriteName + ")");

//      return null;
//#endif
//    }

//    if (mPrefabMap == null)
//    {
//      return null;
//    }

//    if (mPrefabMap.ContainsKey(atlasName) == false || atlasName.Length == 0)
//    {
//      if (bAdvancedLog && atlasName.Length > 0)
//        Debug.LogWarning("325 - atlas:" + atlasName + " not found");
//      return null;
//    }

//    loadDependencies(mPrefabMap[atlasName].assetBundleName);

//    AssetBundle tmp_ab = null;
//    if (mLoadedAssetBundle.ContainsKey(mPrefabMap[atlasName].assetBundleName) == false)
//    {
//      string ab_path = getFilePath(mPrefabMap[atlasName].assetBundleName);
//      tmp_ab = AssetBundle.LoadFromFile(ab_path);
//      if (tmp_ab != null)
//        mLoadedAssetBundle.Add(mPrefabMap[atlasName].assetBundleName, tmp_ab);
//    }
//    else
//    {
//      tmp_ab = mLoadedAssetBundle[mPrefabMap[atlasName].assetBundleName];
//    }

//    UnityEngine.Object[] tmpSubObj = tmp_ab.LoadAssetWithSubAssets<UnityEngine.Object>(mPrefabMap[atlasName].assetName);
//    foreach (UnityEngine.Object obj in tmpSubObj)
//    {
//      if (obj.name == spriteName)
//      {
//        mSpriteCache[atlasName + "_" + spriteName] = (Sprite)obj;
//        return (Sprite)obj;
//      }
//    }

//    return null;
//  }

//  GameObject loadAsset(string prefabName)
//  {
//    loadDependencies(mPrefabMap[prefabName].assetBundleName);

//    AssetBundle tmp_ab = null;
//    if (mLoadedAssetBundle.ContainsKey(mPrefabMap[prefabName].assetBundleName) == false)
//    {
//      string ab_path = getFilePath(mPrefabMap[prefabName].assetBundleName);
//      tmp_ab = AssetBundle.LoadFromFile(ab_path);
//      if (tmp_ab != null)
//        mLoadedAssetBundle.Add(mPrefabMap[prefabName].assetBundleName, tmp_ab);
//    }
//    else
//    {
//      tmp_ab = mLoadedAssetBundle[mPrefabMap[prefabName].assetBundleName];
//    }

//    GameObject cloneGO = GameObject.Instantiate((GameObject)tmp_ab.LoadAsset(mPrefabMap[prefabName].assetName));
//    restoreProperty(cloneGO, prefabName);

//    return cloneGO;
//  }

//  TextAsset loadTextAsset(string prefabName)
//  {
//    loadDependencies(mPrefabMap[prefabName].assetBundleName);

//    AssetBundle tmp_ab = null;
//    if (mLoadedAssetBundle.ContainsKey(mPrefabMap[prefabName].assetBundleName) == false)
//    {
//      string ab_path = getFilePath(mPrefabMap[prefabName].assetBundleName);
//      tmp_ab = AssetBundle.LoadFromFile(ab_path);
//      if (tmp_ab != null)
//        mLoadedAssetBundle.Add(mPrefabMap[prefabName].assetBundleName, tmp_ab);
//    }
//    else
//    {
//      tmp_ab = mLoadedAssetBundle[mPrefabMap[prefabName].assetBundleName];
//    }

//    return (TextAsset)tmp_ab.LoadAsset(mPrefabMap[prefabName].assetName);
//  }

//  ScriptableObject loadScriptableObject(string font_asset_name)
//  {
//    loadDependencies(mPrefabMap[font_asset_name].assetBundleName);

//    AssetBundle tmp_ab = null;
//    if (mLoadedAssetBundle.ContainsKey(mPrefabMap[font_asset_name].assetBundleName) == false)
//    {
//      string ab_path = getFilePath(mPrefabMap[font_asset_name].assetBundleName);
//      tmp_ab = AssetBundle.LoadFromFile(ab_path);
//      if (tmp_ab != null)
//        mLoadedAssetBundle.Add(mPrefabMap[font_asset_name].assetBundleName, tmp_ab);
//    }
//    else
//    {
//      tmp_ab = mLoadedAssetBundle[mPrefabMap[font_asset_name].assetBundleName];
//    }

//    return (ScriptableObject)tmp_ab.LoadAsset(mPrefabMap[font_asset_name].assetName);
//  }

//  void loadDependencies(string assetbundlename)
//  {
//    string[] deps = mABM.GetAllDependencies(assetbundlename);

//    for (int i = 0; i < deps.Length; ++i)
//    {
//      if (mLoadedAssetBundle.ContainsKey(deps[i]) == false)
//      {
//        loadDependencies(deps[i]);

//        if (bAdvancedLog)
//          Debug.Log("335 - load prefab dependencies " + deps[i]);

//        string path = getFilePath(deps[i]);
//        AssetBundle tmp_ab = AssetBundle.LoadFromFile(path);
//        if (tmp_ab != null)
//        {
//          mLoadedAssetBundle.Add(deps[i], tmp_ab);
//        }
//      }
//    }

//  }

//  void restoreProperty(GameObject gameobject, string prefabName)
//  {
//    if (mProperty.ContainsKey(prefabName) == false)
//    {
//      if (bAdvancedLog)
//        Debug.LogWarning("260 - property not found (" + prefabName + ")");
//      return;
//    }

//    Dictionary<string, Dictionary<int, string[]>> comp_dic = mProperty[prefabName];
//    foreach (KeyValuePair<string, Dictionary<int, string[]>> entry in comp_dic)
//    {

//      if (GameObject.Find("Bootloader").GetComponent<Bootloader>().currScriptingBackend == Bootloader.ScriptingBackend.IL2CPP)
//      {
//        if (Type.GetType(entry.Key) == null)
//        {
//          if (bAdvancedLog)
//            Debug.LogWarning("265 - missing assembly : " + entry.Key);
//          continue; //missing script assembly
//        }
//      }
//      else
//      {
//        if (mScript.GetType(entry.Key) == null)
//        {
//          if (bAdvancedLog)
//            Debug.LogWarning("265 - missing assembly : " + entry.Key);
//          continue; //missing script assembly
//        }
//      }

//      Dictionary<int, string[]> comp_id_dic = entry.Value;
//      foreach (KeyValuePair<int, string[]> comp_id_entry in comp_id_dic)
//      {
//        GameObject tmp_parent_go = findGameObject(gameobject, comp_id_entry.Key);
//        if (tmp_parent_go != null)
//        {
//          //add components
//          for (int i = 0; i < comp_id_entry.Value.Length; ++i)
//          {

//            object t = null;
//            if (GameObject.Find("Bootloader").GetComponent<Bootloader>().currScriptingBackend == Bootloader.ScriptingBackend.IL2CPP)
//            {
//              Component tmp_c = tmp_parent_go.AddComponent(Type.GetType(entry.Key));
//              t = System.Convert.ChangeType(tmp_c, Type.GetType(entry.Key));
//            }
//            else
//            {
//              Component tmp_c = tmp_parent_go.AddComponent(mScript.GetType(entry.Key));
//              t = System.Convert.ChangeType(tmp_c, mScript.GetType(entry.Key));
//            }

//            MethodInfo fjo = t.GetType().GetMethod("FromJsonOverwrite", new Type[] { typeof(string) });
//            if (fjo != null)
//            {
//              //回覆 GameObject 裡面變數的設定 (如果 JsonUtility 無法如預期恢復時使用自訂的函數來恢復)
//              //use custom Json deserializer
//              fjo.Invoke(t, new object[] { comp_id_entry.Value[i] });
//            }
//            else
//            {
//              JsonUtility.FromJsonOverwrite((string)comp_id_entry.Value[i], t);

//              //
//              //handle specific classes
//              //
//              FieldInfo[] pi = t.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
//              for (int j = 0; j < pi.Length; ++j)
//              {
//                string field_type = pi[j].FieldType.ToString();
//                if (field_type == "PatchableSprite")
//                {
//                  PatchableSprite ps = (PatchableSprite)pi[j].GetValue(t);
//                  ps.load_sprite(this);

//                }
//                else
//                if (field_type == "PatchableSprite[]")
//                {
//                  PatchableSprite[] psarr = (PatchableSprite[])pi[j].GetValue(t);
//                  for (int k = 0; k < psarr.Length; ++k)
//                  {
//                    psarr[k].load_sprite(this);
//                  }
//                }

//              }

//            }

//          }

//        }
//        else
//        {
//          if (bAdvancedLog)
//            Debug.LogWarning("278 - missing game object with identity : " + comp_id_entry.Key);
//        }
//      }
//    }


//  }

//  GameObject findGameObject(GameObject parent_go, int identity)
//  {
//    foreach (var component in parent_go.GetComponentsInChildren<MonoBehaviour>(true))
//    {
//      if (component.GetType().Name.Contains("ComponentIdentity"))
//      {
//        ComponentIdentity ci = (ComponentIdentity)component;
//        if (ci.ParentIdentity == identity)
//        {
//          return ci.gameObject;
//        }
//      }
//    }

//    return null;
//  }

//  //<prefab name, <gameobject name, <components id, [components property]>>> 
//  Dictionary<string, Dictionary<string, Dictionary<int, string[]>>> loadProperty(AssetBundleMeta.Payload payload)
//  {
//    string strPayloadFilePath = getFilePath(payload.filename);
//    if (strPayloadFilePath == null)
//      return null;

//    if (bAdvancedLog)
//      Debug.Log("158 - load property info from " + strPayloadFilePath);

//    AssetBundle tmpAB = AssetBundle.LoadFromFile(strPayloadFilePath);
//    TextAsset ta = (TextAsset)tmpAB.LoadAsset(payload.asset_name[0]);

//    // Debug.Log(ta.text);

//    //<assetbundle name, 
//    //  <asset name(prefab), 
//    //    <component name, 
//    //      <identity, [property]> 
//    //    > 
//    //  > 
//    //>
//    Dictionary<string, object> raw = (Dictionary<string, object>)msgpack.Unpack(ta.bytes);
//    Dictionary<string, Dictionary<string, Dictionary<int, string[]>>> out_dic = new Dictionary<string, Dictionary<string, Dictionary<int, string[]>>>();
//    foreach (KeyValuePair<string, object> prefab_entry in raw)
//    {
//      string prefab_name = prefab_entry.Key;
//      Dictionary<string, Dictionary<int, string[]>> out_comp_dic = new Dictionary<string, Dictionary<int, string[]>>();
//      Dictionary<string, object> comp_dic = (Dictionary<string, object>)prefab_entry.Value;
//      foreach (KeyValuePair<string, object> comp_entry in comp_dic)
//      {
//        string comp_name = comp_entry.Key;
//        Dictionary<int, string[]> out_comp_id_dic = new Dictionary<int, string[]>();
//        Dictionary<string, object> comp_id_dic = (Dictionary<string, object>)comp_entry.Value;
//        foreach (KeyValuePair<string, object> comp_id_entry in comp_id_dic)
//        {
//          int comp_id = 0;
//          int.TryParse(comp_id_entry.Key, out comp_id);
//          List<object> comp_property = (List<object>)comp_id_entry.Value;

//          string[] property_arr = new string[comp_property.Count];
//          for (int i = 0; i < property_arr.Length; ++i)
//          {
//            property_arr[i] = (string)comp_property[i];
//          }

//          out_comp_id_dic.Add(comp_id, property_arr);
//        }
//        out_comp_dic.Add(comp_name, out_comp_id_dic);
//      }
//      out_dic.Add(prefab_name, out_comp_dic);
//    }

//    tmpAB.Unload(false);
//    return out_dic;
//  }

//  Assembly loadScripts(string asset_name)
//  {
//    string strScriptFilePath = getFilePath("script");
//    if (strScriptFilePath == null)
//      return null;

//    if (bAdvancedLog)
//      Debug.Log("113 - load script file from " + strScriptFilePath);

//    AssetBundle tmpAB = AssetBundle.LoadFromFile(strScriptFilePath);
//    TextAsset ta = (TextAsset)tmpAB.LoadAsset(asset_name);

//    Assembly tmpA = Assembly.Load(ta.bytes);

//    tmpAB.Unload(false);

//    return tmpA;
//  }

//  Dictionary<string, object> loadMetaFile()
//  {
//    // string strMetaFilePath =getFilePath("meta");
//    string strMetaFilePath = tmp_fs.getHistoryPath("meta");
//    if (strMetaFilePath == null)
//    {
//      Debug.LogError("988 - meta file path is null");
//      return null;
//    }

//    if (bAdvancedLog)
//      Debug.Log("993 - load meta file from " + strMetaFilePath);

//    //read meta file from path strMetaFilePath
//    return (Dictionary<string, object>)msgpack.Unpack(File.ReadAllBytes(strMetaFilePath));
//  }

//  string getFilePath(string filename)
//  {
//    if (tmp_fs.getSyncSource().ContainsKey(filename) == false)
//    {
//      Debug.LogError("1003 - sync type not found for file :" + filename);
//      return null;
//    }
//    if (tmp_fs.getSyncSource()[filename] == FileSync.PatchSource.HISTORY)
//    {
//      return tmp_fs.getHistoryPath(filename);

//    }
//    else
//    if (tmp_fs.getSyncSource()[filename] == FileSync.PatchSource.CUSTOM_OBB ||
//        tmp_fs.getSyncSource()[filename] == FileSync.PatchSource.PLAYSTORE_OBB)
//    {
//      return tmp_fs.getOBBMountRoot(filename);

//    }

//    return null;
//  }

//  public string getPatchVersion()
//  {
//    if (mEditorMode)
//    {
//      return "EDITOR";
//    }

//    string ret = mMeta.patch_version;
//    return ret;
//  }

//  public void PurgeAssetbundles()
//  {
//    foreach (KeyValuePair<string, AssetBundle> ab in mLoadedAssetBundle)
//    {
//      Debug.LogWarning("144 - unload assetbundle : " + ab.Value.name);
//      ab.Value.Unload(false);
//    }

//    mLoadedAssetBundle.Clear();
//  }

//  Dictionary<string, List<string>> assets_list = new Dictionary<string, List<string>>(); //assetbundle <--> assets
//  Dictionary<string, List<string>> atlas_map = new Dictionary<string, List<string>>(); //assetbundle, sprite <--> atlas
//  Dictionary<string, List<string>> sprites_list = new Dictionary<string, List<string>>(); //atlas <--> sprites
//  public List<string> get_asset_list(string asset_bundle_name)
//  {
//    if (assets_list.ContainsKey(asset_bundle_name))
//    {
//      return assets_list[asset_bundle_name];
//    }

//    if (mEditorMode)
//    {
//#if UNITY_EDITOR
//      string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(asset_bundle_name);
//      List<string> ret0 = new List<string>();
//      foreach (string asset in assets)
//      {
//        string key = Path.GetFileName(asset)
//          .Replace(".prefab", "")
//          .Replace(".png", "")
//          .Replace(".mp3", "")
//          .Replace(".ogg", "")
//          .Replace(".wav", "")
//          .Replace(".mixer", "")
//          .Replace(".asset", "")
//          .Replace(".json", "");
//        ret0.Add(key);
//      }
//      assets_list.Add(asset_bundle_name, ret0);
//      return ret0;

//#endif
//    }

//    List<string> keys = new List<string>();
//    foreach (string e in mPrefabMap.Keys)
//    {
//      keys.Add(e);
//    }

//    List<string> ret = new List<string>();
//    for (int i = 0; i < keys.Count; ++i)
//    {
//      if (mPrefabMap[keys[i]].assetBundleName.Contains(asset_bundle_name))
//      {
//        ret.Add(keys[i]);
//      }
//    }
//    assets_list.Add(asset_bundle_name, ret);

//    return ret;
//  }

//  public List<string> get_sprite_list(string atlas_asset_name)
//  {
//    if (sprites_list.ContainsKey(atlas_asset_name))
//    {
//      return sprites_list[atlas_asset_name];
//    }

//    if (mEditorMode)
//    {
//#if UNITY_EDITOR
//      if (mAssetNamePathMapper.ContainsKey(atlas_asset_name) == false)
//      {
//        return null; //atlas not found
//      }

//      UnityEngine.Object[] sprites = AssetDatabase.LoadAllAssetRepresentationsAtPath(mAssetNamePathMapper[atlas_asset_name]);
//      List<string> ret0 = new List<string>();
//      foreach (UnityEngine.Object obj0 in sprites)
//      {
//        ret0.Add(obj0.name);
//      }
//      sprites_list.Add(atlas_asset_name, ret0);
//      return ret0;
//#endif
//    }

//    string assetBundleName = mPrefabMap[atlas_asset_name].assetBundleName;
//    string assetName = mPrefabMap[atlas_asset_name].assetName;

//    AssetBundle tmp_ab = null;
//    if (mLoadedAssetBundle.ContainsKey(assetBundleName) == false)
//    {
//      string ab_path = getFilePath(assetBundleName);
//      tmp_ab = AssetBundle.LoadFromFile(ab_path);
//      if (tmp_ab != null)
//      {
//        mLoadedAssetBundle.Add(assetBundleName, tmp_ab);
//      }
//    }
//    else
//    {
//      tmp_ab = mLoadedAssetBundle[assetBundleName];
//    }

//    List<string> ret = new List<string>();
//    UnityEngine.Object[] tmpSubObj = tmp_ab.LoadAssetWithSubAssets<UnityEngine.Object>(assetName);
//    foreach (UnityEngine.Object entity in tmpSubObj)
//    {
//      ret.Add(entity.name);
//    }
//    sprites_list.Add(atlas_asset_name, ret);

//    return ret;
//  }

//  public List<string> get_atlas_list(string assetbundle_name, string sprite_name)
//  {
//    if (atlas_map.ContainsKey(assetbundle_name + "_" + sprite_name))
//    {
//      return atlas_map[assetbundle_name + "_" + sprite_name];
//    }

//    List<string> asset_list = get_asset_list(assetbundle_name);
//    List<string> ret = new List<string>();

//    for (int i = 0; i < asset_list.Count; ++i)
//    {
//      if (get_sprite_list(asset_list[i]).Contains(sprite_name))
//      {
//        ret.Add(asset_list[i]);
//      }
//    }

//    atlas_map.Add(assetbundle_name + "_" + sprite_name, ret);
//    return ret;
//  }
//}
