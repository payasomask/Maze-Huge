#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

public class AssetbundleLoader{
  public static AssetbundleLoader _AssetbundleLoader = null;
#if UNITY_EDITOR
  bool bAdvancedLog =true;
#else
  bool bAdvancedLog =false;
#endif
  
  class PrefabMap{
    public string assetBundleName =null;
    public string assetName =null;
  }

  //assetbundle info.
  Dictionary<string, PrefabMap>                                       mPrefabMap =null; // prefab name <--> assetbundle name

  Dictionary<string, AssetBundle>                                     mLoadedAssetBundle =new Dictionary<string, AssetBundle>();

  bool mEditorMode =false;


  // WWW mTmpWWW =null;

#if UNITY_EDITOR
  Dictionary<string, string> mAssetNamePathMapper =new Dictionary<string, string>();
#endif

  public void setEditorMode(){
#if UNITY_EDITOR
    mEditorMode =true;

    var names =AssetDatabase.GetAllAssetBundleNames();
    foreach (var name in names){
      string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(name);
      foreach(string asset in assets){
        if (bAdvancedLog)
          Debug.Log("map prefab with name ="+Path.GetFileName(asset));
        string key =Path.GetFileName(asset)
          .Replace(".prefab", "")
          .Replace(".png", "")
          .Replace(".mp3", "")
          .Replace(".ogg", "")
          .Replace(".wav", "")
          .Replace(".mixer", "")
          .Replace(".asset", "")
          .Replace(".json", "");
        if (mAssetNamePathMapper.ContainsKey(key)==true){
          Debug.LogError("184 - asset name has already exist ("+key+")");
        }else{
          mAssetNamePathMapper.Add(key, asset);
        }
      }

    }

#endif
  }

  public int loadingProgress(){
    return 0;
  }

  public GameObject InstantiatePrefab(string prefabName){


    GameObject prefabeObj = PrefabManager._PrefabManager.GetPrefab(prefabName);

    if (prefabeObj == null)
      return null;

   
    return GameObject.Instantiate(prefabeObj);
  }

//  public TMPro.TMP_FontAsset InstantiateFontAsset(string font_asset_name){
//    if (mEditorMode){
//#if UNITY_EDITOR
//      if (mAssetNamePathMapper.ContainsKey(font_asset_name)==false){
//        Debug.LogWarning("font asset "+font_asset_name+" not found");
//        return null;
//      }
//      return (TMPro.TMP_FontAsset)AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>(mAssetNamePathMapper[font_asset_name]);
//#endif
//    }

//    if (mPrefabMap.ContainsKey(font_asset_name)==false)
//      return null;

//    TMPro.TMP_FontAsset tmp_go =(TMPro.TMP_FontAsset)loadScriptableObject(font_asset_name);
//    if (bAdvancedLog)
//      Debug.Log("280 - load fonat asset "+font_asset_name);

//    return tmp_go;
//  }

//  public TextAsset InstantiateTextAsset(string asset_name){
//    if (mEditorMode){
//#if UNITY_EDITOR
//      if (mAssetNamePathMapper.ContainsKey(asset_name)==false){
//        Debug.LogWarning("textasset "+asset_name+" not found");
//        return null;
//      }
//      return (TextAsset)AssetDatabase.LoadAssetAtPath<TextAsset>(mAssetNamePathMapper[asset_name]);
//#endif
//    }

//    if (mPrefabMap.ContainsKey(asset_name)==false)
//      return null;

//    TextAsset tmp_go =(TextAsset)loadTextAsset(asset_name);
//    if (bAdvancedLog)
//      Debug.Log("280 - load text asset "+asset_name);

//    return tmp_go;

//  }


  Dictionary<string, AudioClip> mAudioClipCache =new Dictionary<string, AudioClip>();
  public void PurgeAudioClipCache(){
    mAudioClipCache.Clear();
  }
  
  public AudioClip InstantiateAudio(string audio_clip_name, bool fromCache =true){

    return AudioManager._AudioManager.GetAudio(audio_clip_name);

  }

  public AudioMixer InstantiateAudioMixer(string audio_mixer_name){
    return AudioManager._AudioManager.GetAudioMixer(audio_mixer_name);
  }

  Dictionary<string, Sprite> mSpriteCache =new Dictionary<string, Sprite>();
  public void PurgeSpriteCache(){
    mSpriteCache.Clear();
  }

  public Sprite InstantiateSprite(string atlasName, string spriteName, bool fromCache =true){
    return SpriteManager._SpriteManager.GetSprite(atlasName,spriteName);
  }

  //TextAsset loadTextAsset(string prefabName){

  //}

  //ScriptableObject loadScriptableObject(string font_asset_name){

  //}

	public void PurgeAssetbundles(){
		foreach(KeyValuePair<string, AssetBundle> ab in mLoadedAssetBundle){
			Debug.LogWarning("144 - unload assetbundle : "+ab.Value.name);
			ab.Value.Unload(false);
		}

		mLoadedAssetBundle.Clear();
	}

  Dictionary<string, List<string>> assets_list =new Dictionary<string, List<string>>(); //assetbundle <--> assets
  Dictionary<string, List<string>> atlas_map =new Dictionary<string, List<string>>(); //assetbundle, sprite <--> atlas
  Dictionary<string, List<string>> sprites_list =new Dictionary<string, List<string>>(); //atlas <--> sprites
	public List<string> get_asset_list(string asset_bundle_name){
		if (assets_list.ContainsKey(asset_bundle_name)){
			return assets_list[asset_bundle_name];
		}

    if (mEditorMode){
#if UNITY_EDITOR
      string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(asset_bundle_name);
      List<string> ret0 =new List<string>();
      foreach(string asset in assets){
        string key =Path.GetFileName(asset)
          .Replace(".prefab", "")
          .Replace(".png", "")
          .Replace(".mp3", "")
          .Replace(".ogg", "")
          .Replace(".wav", "")
          .Replace(".mixer", "")
          .Replace(".asset", "")
          .Replace(".json", "");
        ret0.Add(key);
      }
      assets_list.Add(asset_bundle_name, ret0);
      return ret0;

#endif
    }

		List<string> keys =new List<string>();
		foreach(string e in mPrefabMap.Keys){
			keys.Add(e);
		}

		List<string> ret =new List<string>();
		for (int i=0;i<keys.Count;++i){
			if (mPrefabMap[keys[i]].assetBundleName.Contains(asset_bundle_name)){
				ret.Add(keys[i]);
			}
		}
		assets_list.Add(asset_bundle_name, ret);
		
		return ret;
	}
}
