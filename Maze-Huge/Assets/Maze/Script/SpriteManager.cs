using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SpriteManager : MonoBehaviour
{
  public static SpriteManager _SpriteManager = null;

  [SerializeField]
  private SpriteAtlas[] Texture_list = null;

  Dictionary<string, Sprite> mSpriteCache = new Dictionary<string, Sprite>();
  private void Awake()
  {
    _SpriteManager = this;
  }
  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {

  }

  public Sprite GetSprite(string texname,string spritename)
  {
    string cachekey = texname + "_" + spritename;
    if (mSpriteCache != null && mSpriteCache.ContainsKey(cachekey)){
      return mSpriteCache[cachekey];
    }

    SpriteAtlas targetTex = null;
    foreach (var v in Texture_list){
      if (v.name == texname)
        targetTex = v;
    }

    if (targetTex == null){
      //Debug.Log("864 - cant find Texture2D.name = " + texname);
      return null;
    }

    Sprite target = targetTex.GetSprite(spritename);
    mSpriteCache.Add(cachekey, target);

    return target;
  }
}
