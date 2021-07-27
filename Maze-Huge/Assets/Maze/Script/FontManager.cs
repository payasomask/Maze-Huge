using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontManager : MonoBehaviour
{
  public static FontManager _FontManager = null;

  TMP_FontAsset current_font = null;

  [SerializeField]
  private TMP_FontAsset[] Font_list = null;
  private void Awake()
  {
    _FontManager = this;
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

    //string Lang = PlayerPrefsManager._PlayerPrefsManager.Language;
    string Lang = "LiberationSans SDF";

    foreach (var v in Font_list){
      if (v.name == Lang)
        current_font = v;
    }


  }

  //public Font getFont(string name)
  //{

  //  foreach (var v in Font_list)
  //  {
  //    if (v.name == name)
  //      return v;
  //  }

  //  return null;
  //}

  //public TMP_FontAsset getFont(TextMeshProUGUI TMP, bool InstanceMaterial = true)
  //{
  //  //TMP_FontAsset tmp = mCxt.mABL.InstantiateFontAsset("font_" + mCxt.mPPM.Language.ToLower());
  //  TMP.font = currentfont;
  //  if (InstanceMaterial)
  //    TMP.fontSharedMaterial = currentfont.material;

  //  //default setup is Mask Soft (The Mask Off setup let the compiler strips its function after code compilation)
  //  //restore back to Mask Off
  //  //TMP.maskType = MaskingTypes.MaskOff;
  //  return currentfont;
  //}

  public TMP_FontAsset getFont(TextMeshProUGUI TMP, bool InstanceMaterial = true)
  {
    //TMP_FontAsset tmp = mCxt.mABL.InstantiateFontAsset("font_" + mCxt.mPPM.Language.ToLower());
    TMP.font = current_font;
    if (InstanceMaterial)
      TMP.fontSharedMaterial = current_font.material;

    //default setup is Mask Soft (The Mask Off setup let the compiler strips its function after code compilation)
    //restore back to Mask Off
    //TMP.maskType = MaskingTypes.MaskOff;
    return current_font;
  }
}
