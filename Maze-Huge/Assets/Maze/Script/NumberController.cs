using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public delegate void OnSetValueDone();

public class NumberController : MonoBehaviour
{

  public float sprite_rect_width_scale = 1.0f;
  bool bAdvancedLog = false;

  Sprite[] digitsSprite = null; //0~9

  Sprite commaSprite = null;
  Sprite pointSprite = null;

  Sprite unitAtTheEnd = null;
  Sprite unitAtTheFront = null;

  Sprite CancelLineSprite = null;

  Sprite[] extraSymbolSprite = null;

  public string[] digitsAtlasName = null;
  public string[] digitsSpriteName = null; //spritename

  public string commaAtlasName = null;
  public string commaSpriteName = null;

  public string pointAtlasName = null;
  public string pointSpriteName = null;

  public string unitAtTheEndAtlasName = null;
  public string unitAtTheEndSpriteName = null;

  public string unitAtTheFrontAtlasName = null;
  public string unitAtTheFrontSpriteName = null;

  public string CancelLineAtlasName = null;
  public string CancelLineSpriteName = null;

  public string[] extraSymbolAtlasName = null;
  public string[] extraSymbolSpriteName = null;

  public bool mUseComma = true;
  public bool mUseCancelLine = false;

  public enum AlignType
  {
    LEFT,
    CENTER,
    RIGHT
  }

  public enum AnimationState
  {
    IDLE,
    JUMPING
  }

  public enum AnimationType
  {
    NONE,
    RANDOM_JUMP,
    SEQUENCE_JUMP
  }

  public UInt64 currentValue = 0;
  public UInt64 targetValue = 0;
  UInt64 tempStartValue = 0;

  int mDigits = 1;
  AlignType mAlignType = AlignType.RIGHT;

  bool mInited = false;

  //graphics obj
  GameObject[] mCommaGameObject = null;
  GameObject mCancelLineGameObject = null;
  GameObject mPointGameObject = null;
  GameObject[] mDigitsGameObject = null;
  GameObject[] mAfterPointDigitsGameObject = null;
  int[] mAfterPointDigitsVal = null;
  int[] mCurrentDigitVal = null;
  int[] mTargetDigitVal = null;
  bool[] mDigitsJumpTimes = null;
  float[] mDigitsJumppIdleTimer = null;
  bool[] mHideDigit = null;

  GameObject mUnitAtTheEndGameObject = null;
  GameObject mUnitAtTheFrontGameObject = null;

  AnimationType mAnimType = AnimationType.NONE;
  float mDurationTime = 0.0f;
  float mCurrentDurationTime = 0.0f;
  bool mDigitsDirty = false;

  string mSortingLayerName = "";
  string mLayerName = "";
  int mOrder;
  OnSetValueDone Onsetvaluedone = null;
  CommonAction OnValueChange = null;
  // Use this for initialization
  float initSprites()
  {
    float max_width = 0f;

    //load sprites
    AssetbundleLoader mabl = AssetbundleLoader._AssetbundleLoader;
    if (mabl == null)
    {
      if (bAdvancedLog)
        Debug.LogError("mabl is null");
      return 0f;
    }
    if (digitsSpriteName != null && digitsAtlasName != null)
    {
      digitsSprite = new Sprite[digitsSpriteName.Length];
      for (int i = 0; i < digitsSpriteName.Length; ++i)
      {
        if (bAdvancedLog)
          Debug.Log("84 - intit sprite from atlas:" + digitsAtlasName[i] + ", sprite:" + digitsSpriteName[i]);
        digitsSprite[i] = mabl.InstantiateSprite(digitsAtlasName[i], digitsSpriteName[i]);
        if (digitsSprite[i] != null && digitsSprite[i].rect.width > max_width)
        {
          max_width = digitsSprite[i].rect.width;
        }
      }
    }

    if (commaSpriteName != null && commaAtlasName != null)
    {
      // commaSprite =new Sprite();
      if (bAdvancedLog)
        Debug.Log("84 - intit sprite from atlas:" + commaAtlasName + ", sprite:" + commaSpriteName);
      commaSprite = mabl.InstantiateSprite(commaAtlasName, commaSpriteName);
      if (commaSprite != null && commaSprite.rect.width > max_width)
      {
        max_width = commaSprite.rect.width;
      }
    }

    if (pointSpriteName != null && pointAtlasName != null)
    {
      // pointSprite =new Sprite();
      if (bAdvancedLog)
        Debug.Log("84 - intit sprite from atlas:" + pointAtlasName + ", sprite:" + pointSpriteName);
      pointSprite = mabl.InstantiateSprite(pointAtlasName, pointSpriteName);
      if (pointSprite != null && pointSprite.rect.width > max_width)
      {
        max_width = pointSprite.rect.width;
      }
    }

    //
    if (unitAtTheEndSpriteName != null && unitAtTheEndAtlasName != null)
    {
      unitAtTheEnd = mabl.InstantiateSprite(unitAtTheEndAtlasName, unitAtTheEndSpriteName);
      if (unitAtTheEnd != null && unitAtTheEnd.rect.width > max_width)
      {
        max_width = unitAtTheEnd.rect.width;
      }
    }

    if (unitAtTheFrontSpriteName != null && unitAtTheFrontAtlasName != null)
    {
      unitAtTheFront = mabl.InstantiateSprite(unitAtTheFrontAtlasName, unitAtTheFrontSpriteName);
      if (unitAtTheFront != null && unitAtTheFront.rect.width > max_width)
      {
        max_width = unitAtTheFront.rect.width;
      }
    }

    if (CancelLineSpriteName != null && CancelLineAtlasName != null)
    {
      CancelLineSprite = mabl.InstantiateSprite(CancelLineAtlasName, CancelLineSpriteName);
    }

    if (extraSymbolSpriteName != null && extraSymbolAtlasName != null)
    {
      extraSymbolSprite = new Sprite[extraSymbolSpriteName.Length];
      for (int i = 0; i < extraSymbolSpriteName.Length; ++i)
      {
        if (bAdvancedLog)
          Debug.Log("84 - intit sprite from atlas:" + extraSymbolAtlasName[i] + ", sprite:" + extraSymbolSpriteName[i]);
        extraSymbolSprite[i] = mabl.InstantiateSprite(extraSymbolAtlasName[i], extraSymbolSpriteName[i]);
        if (extraSymbolSprite[i] != null && extraSymbolSprite[i].rect.width > max_width)
        {
          max_width = extraSymbolSprite[i].rect.width;
        }
      }
    }

    return max_width * sprite_rect_width_scale;
  }

  // Update is called once per frame
  void Update()
  {
    if (mInited == false)
      return;

    if (mDigitsDirty == false)
      return;

    if (mAnimType == AnimationType.RANDOM_JUMP)
    {
      UpdateRandomScore();
    }
    else
    if (mAnimType == AnimationType.SEQUENCE_JUMP)
    {
      UpdateSequenceScore();
    }

    layoutDigits();

  }

  float idleLimit = 0.01f;
  void UpdateRandomScore()
  {
    bool founddrity = false;
    mCurrentDurationTime += Time.deltaTime;
    mvalue_change_count += Time.deltaTime;
    for (int i = 0; i < mDigits; ++i)
    {
      if (mDigitsJumpTimes[i] == false)
      {
        if (mHideDigit[i] == true)
        {
          mDigitsGameObject[i].SetActive(false);
        }
        continue;
      }

      //the end of jump (the last jump)
      if (mCurrentDurationTime >= mDurationTime)
      {
        if (mHideDigit[i] == true)
        {
          mDigitsGameObject[i].SetActive(false);
        }
        else
        {
          int rndVal = mTargetDigitVal[i];
          ((SpriteRenderer)mDigitsGameObject[i].GetComponent("SpriteRenderer")).sprite = digitsSprite[rndVal];

          mCurrentDigitVal[i] = rndVal;
        }
        continue;
      }

      founddrity = true;
      mDigitsJumppIdleTimer[i] += Time.deltaTime;
      if (mDigitsJumppIdleTimer[i] >= idleLimit)
      {
        mDigitsJumppIdleTimer[i] = 0.0f;

        int rndVal = UnityEngine.Random.Range(0, 9);
        ((SpriteRenderer)mDigitsGameObject[i].GetComponent("SpriteRenderer")).sprite = digitsSprite[rndVal];

        mCurrentDigitVal[i] = rndVal;
      }

    }

    if (mvalue_change_count >= mvalue_change_limit)
    {
      mvalue_change_count = 0.0f;
      if (OnValueChange != null)
        OnValueChange();
    }

    if (founddrity == false)
    {
      mDigitsDirty = false;
      currentValue = targetValue;
      if (Onsetvaluedone != null)
        Onsetvaluedone();
    }

  }

  void UpdateSequenceScore()
  {
    mCurrentDurationTime += Time.deltaTime;
    mvalue_change_count += Time.deltaTime;
    UInt64 val = targetValue;
    if (mCurrentDurationTime < mDurationTime)
    {

      if (targetValue >= tempStartValue)
      {
        val = (UInt64)(((float)targetValue - (float)tempStartValue) * ((float)mCurrentDurationTime / (float)mDurationTime));
        val = tempStartValue + val;
      }
      else
      {
        val = (UInt64)(((float)tempStartValue - (float)targetValue) * ((float)mCurrentDurationTime / (float)mDurationTime));
        val = tempStartValue - val;
      }
    }
    else
    {
      mDigitsDirty = false;
      if (Onsetvaluedone != null)
        Onsetvaluedone();
    }

    if (currentValue != val)
    {
      if (mvalue_change_count >= mvalue_change_limit)
      {
        mvalue_change_count = 0.0f;
        if (OnValueChange != null)
          OnValueChange();
      }
    }


    currentValue = val;
    string strVal = val.ToString();

    //fill zero
    int digitCt = 0;
    while (digitCt + strVal.Length < mDigits)
    {
      setDigitWithVal(digitCt, 0);
      mDigitsGameObject[digitCt].SetActive(false);

      digitCt++;
    }

    for (int i = 0; i < strVal.Length; ++i)
    {
      if ((digitCt + i) >= mDigitsGameObject.Length)
        continue;

      mDigitsGameObject[digitCt + i].SetActive(true);

      int digitval = 0;
      Int32.TryParse(strVal[i].ToString(), out digitval);

      ((SpriteRenderer)mDigitsGameObject[digitCt + i].GetComponent("SpriteRenderer")).sprite = digitsSprite[digitval];
      mCurrentDigitVal[digitCt + i] = digitval;
    }

  }

  Material co_mat = null;
  public void use_color_overlay()
  {
    co_mat = new Material(Shader.Find("Sprites/ColorOverlay"));
    SpriteRenderer[] srarr = getallnumberSR();
    for (int i = 0; i < srarr.Length; ++i)
    {
      srarr[i].sharedMaterial = co_mat;
    }
  }

  public void set_color_overlay(Color overlay)
  {
    if (co_mat == null)
      return;

    co_mat.SetColor("_OverlayColor", overlay);
  }

  float monospace_width = 0f;
  bool use_monospace = false;
  public void init(int digits, UInt64 default_value, AlignType at, bool useComma, string sortinglayerName, bool monospace = false, string layerName = "Default", int Order = 0, bool useCancelline = false, string Afterpointvalue = "")
  {
    if (mInited)
      return;

    monospace_width = initSprites();
    use_monospace = monospace;

    mDigits = digits;
    mAlignType = at;
    currentValue = default_value + 1;
    targetValue = default_value + 1; //make a difference from default_vale
    mUseComma = useComma;
    mUseCancelLine = useCancelline;
    //mUsePoint = usePoint;
    mSortingLayerName = sortinglayerName;
    mLayerName = layerName;
    mOrder = Order;
    //always 2 digits after point
    InitAfterPointValue(Afterpointvalue);

    mDigitsGameObject = new GameObject[digits];
    mCurrentDigitVal = new int[digits];
    mTargetDigitVal = new int[digits];
    mDigitsJumpTimes = new bool[digits];
    mDigitsJumppIdleTimer = new float[digits];
    mHideDigit = new bool[digits];
    SpriteRenderer num_controller_sr = gameObject.GetComponent<SpriteRenderer>();
    Material m = num_controller_sr ? num_controller_sr.material : null;
    //create gameobjects
    for (int i = 0; i < digits; ++i)
    {
      mDigitsGameObject[i] = new GameObject();
      mDigitsGameObject[i].transform.SetParent(gameObject.transform, false);
      mDigitsGameObject[i].name = i.ToString();
      mDigitsGameObject[i].transform.localRotation = Quaternion.identity;
      mDigitsGameObject[i].SetActive(false);

      SpriteRenderer sr = (SpriteRenderer)mDigitsGameObject[i].AddComponent<SpriteRenderer>();
      sr.sprite = digitsSprite[0];
      sr.sortingLayerName = mSortingLayerName;
      sr.gameObject.layer = LayerMask.NameToLayer(mLayerName);
      sr.sortingOrder = mOrder;
      if (m != null)
        sr.material = m;
      mCurrentDigitVal[i] = 0;
    }

    //max comma digits
    int comma_num = (digits - 1) / 3;
    mCommaGameObject = new GameObject[comma_num];
    for (int i = 0; i < comma_num; ++i)
    {
      mCommaGameObject[i] = new GameObject();
      mCommaGameObject[i].transform.SetParent(gameObject.transform, false);
      mCommaGameObject[i].name = "comma_" + i.ToString();
      mCommaGameObject[i].transform.localRotation = Quaternion.identity;
      mCommaGameObject[i].SetActive(false);

      SpriteRenderer sr = (SpriteRenderer)mCommaGameObject[i].AddComponent<SpriteRenderer>();
      if (m != null)
        sr.material = m;
      sr.sprite = commaSprite;
      sr.sortingLayerName = mSortingLayerName;
      sr.gameObject.layer = LayerMask.NameToLayer(mLayerName);
      sr.sortingOrder = mOrder;
    }

    if (CancelLineSprite != null)
    {
      mCancelLineGameObject = new GameObject("CancelLine");
      mCancelLineGameObject.transform.SetParent(gameObject.transform, false);
      mCancelLineGameObject.transform.localRotation = Quaternion.identity;
      mCancelLineGameObject.SetActive(false);
      SpriteRenderer sr = (SpriteRenderer)mCancelLineGameObject.AddComponent<SpriteRenderer>();
      if (m != null)
        sr.material = m;
      sr.sprite = CancelLineSprite;
      sr.sortingLayerName = mSortingLayerName;
      sr.gameObject.layer = LayerMask.NameToLayer(mLayerName);
      sr.sortingOrder = mOrder;
    }

    if (unitAtTheEnd != null)
    {
      mUnitAtTheEndGameObject = new GameObject();
      mUnitAtTheEndGameObject.transform.SetParent(gameObject.transform, false);
      mUnitAtTheEndGameObject.name = "Unit";
      mUnitAtTheEndGameObject.transform.localRotation = Quaternion.identity;
      mUnitAtTheEndGameObject.SetActive(true);

      SpriteRenderer sr = (SpriteRenderer)mUnitAtTheEndGameObject.AddComponent<SpriteRenderer>();
      if (m != null)
        sr.material = m;
      sr.sprite = unitAtTheEnd;
      sr.sortingLayerName = mSortingLayerName;
      sr.gameObject.layer = LayerMask.NameToLayer(mLayerName);
      sr.sortingOrder = mOrder;
    }

    if (unitAtTheFront != null)
    {
      mUnitAtTheFrontGameObject = new GameObject();
      mUnitAtTheFrontGameObject.transform.SetParent(gameObject.transform, false);
      mUnitAtTheFrontGameObject.name = "Unit";
      mUnitAtTheFrontGameObject.transform.localRotation = Quaternion.identity;
      mUnitAtTheFrontGameObject.SetActive(true);

      SpriteRenderer sr = (SpriteRenderer)mUnitAtTheFrontGameObject.AddComponent<SpriteRenderer>();
      if (m != null)
        sr.material = m;
      sr.sprite = unitAtTheFront;
      sr.sortingLayerName = mSortingLayerName;
      sr.gameObject.layer = LayerMask.NameToLayer(mLayerName);
      sr.sortingOrder = mOrder;
    }

    setValue(default_value, unitAtTheEnd, mOrder);
    mInited = true;
  }

  void InitAfterPointValue(string value)
  {
    //RAY : 目前先限定只能顯示2位(以內)小數點之後的數字，並且一定要先設定好point 的相關參數(spritename/atlasname)
    //之後的所有行為 都依靠 pointSprite 是不是==null 做判斷

    //-----注意 : 取消線、小數點後位數(包含point) 目前無法動態改變其長度、或是重新設定value，之後有需求再修改-----

    if (String.IsNullOrEmpty(value))
    {
      pointSprite = null;
      return;
    }

    if (value.Length > 2 || value.Length == 0)
    {
      Debug.LogWarning("gameobject : " + name + "Afterpointvalue.length is out og range.. : " + value.Length + "please set 0 < Afterpointvalue.length <= 2 ");
      pointSprite = null;
      return;
    }
    mAfterPointDigitsVal = new int[value.Length];
    int check_num = -1;

    for (int i = 0; i < value.Length; i++)
    {
      char s = value[i];
      int.TryParse(s.ToString(), out check_num);
      if (check_num < 0)
      {
        Debug.LogWarning("Please set Afterpointvalue value is Type of INT..");
        pointSprite = null;
        return;
      }
      else
        mAfterPointDigitsVal[i] = check_num;
    }

    mAfterPointDigitsGameObject = new GameObject[value.Length];

    mPointGameObject = new GameObject("Point");
    mPointGameObject.transform.SetParent(gameObject.transform, false);
    mPointGameObject.transform.localRotation = Quaternion.identity;
    mPointGameObject.SetActive(false);
    SpriteRenderer sr = (SpriteRenderer)mPointGameObject.AddComponent<SpriteRenderer>();
    sr.sprite = pointSprite;
    sr.sortingLayerName = mSortingLayerName;
    sr.gameObject.layer = LayerMask.NameToLayer(mLayerName);
    sr.sortingOrder = mOrder;
    for (int i = 0; i < mAfterPointDigitsGameObject.Length; i++)
    {
      mAfterPointDigitsGameObject[i] = new GameObject("Point_" + i);
      mAfterPointDigitsGameObject[i].transform.SetParent(gameObject.transform, false);
      mAfterPointDigitsGameObject[i].transform.localRotation = Quaternion.identity;
      mAfterPointDigitsGameObject[i].SetActive(true);
      SpriteRenderer d_sr = (SpriteRenderer)mAfterPointDigitsGameObject[i].AddComponent<SpriteRenderer>();
      d_sr.sprite = digitsSprite[mAfterPointDigitsVal[i]];
      d_sr.sortingLayerName = mSortingLayerName;
      d_sr.gameObject.layer = LayerMask.NameToLayer(mLayerName);
      d_sr.sortingOrder = mOrder;
    }
  }


  public float cancelline_extra_length = 0.0f;

  void layoutDigits()
  {
    //calculate width
    //current digits width
    float totalWidth = 0.0f;
    int availableDigits = 0;
    for (int i = 0; i < mDigits; ++i)
    {
      if (mDigitsGameObject[i].activeSelf == true)
      {
        totalWidth += use_monospace ? monospace_width * 1 / digitsSprite[mCurrentDigitVal[i]].pixelsPerUnit : digitsSprite[mCurrentDigitVal[i]].rect.width * sprite_rect_width_scale * 1 / digitsSprite[mCurrentDigitVal[i]].pixelsPerUnit;
        availableDigits++;
      }
    }

    if (mUseComma == true)
    {
      // 1,000,000
      int commaNum = (availableDigits - 1) / 3;
      totalWidth += (use_monospace ? monospace_width * 1 / commaSprite.pixelsPerUnit : commaSprite.rect.width * sprite_rect_width_scale * commaNum * 1 / commaSprite.pixelsPerUnit);
    }

    if (unitAtTheEnd != null)
    {
      totalWidth += (use_monospace ? monospace_width * 1 / unitAtTheEnd.pixelsPerUnit : unitAtTheEnd.rect.width * sprite_rect_width_scale * 1 / unitAtTheEnd.pixelsPerUnit);
    }

    if (unitAtTheFront != null)
    {
      totalWidth += (use_monospace ? monospace_width * 1 / unitAtTheFront.pixelsPerUnit : unitAtTheFront.rect.width * sprite_rect_width_scale * 1 / unitAtTheFront.pixelsPerUnit);
    }

    if (pointSprite != null)
    {
      totalWidth += (use_monospace ? monospace_width * 1 / pointSprite.pixelsPerUnit : pointSprite.rect.width * sprite_rect_width_scale * 1 / pointSprite.pixelsPerUnit);

      for (int i = 0; i < mAfterPointDigitsGameObject.Length; ++i)
      {
        if (mAfterPointDigitsGameObject[i].activeSelf == true)
        {
          totalWidth += use_monospace ? monospace_width * 1 / digitsSprite[mCurrentDigitVal[i]].pixelsPerUnit : digitsSprite[mCurrentDigitVal[i]].rect.width * sprite_rect_width_scale * 1 / digitsSprite[mCurrentDigitVal[i]].pixelsPerUnit;
          //availableDigits++;
        }
      }
    }

    //find start point
    float startX = 0.0f;
    if (mAlignType == AlignType.LEFT)
    {
      startX = 0.0f;

    }
    else
    if (mAlignType == AlignType.RIGHT)
    {
      startX = (float)-totalWidth;

    }
    else
    if (mAlignType == AlignType.CENTER)
    {
      startX = (float)-totalWidth * 0.5f;

    }
    else
    {
      //unknown type
      //...

    }

    //reset comma gameobjects
    for (int i = 0; i < mCommaGameObject.Length; ++i)
    {
      mCommaGameObject[i].SetActive(false);
    }

    float currX = startX;
    if (unitAtTheFront != null)
    {
      mUnitAtTheFrontGameObject.transform.localPosition = new Vector3(currX + (use_monospace ? monospace_width * 0.5f * 1 / unitAtTheFront.pixelsPerUnit : unitAtTheFront.rect.width * sprite_rect_width_scale * 0.5f * 1 / unitAtTheFront.pixelsPerUnit), 0.0f, 0.0f);
      currX += use_monospace ? monospace_width * 1 / unitAtTheFront.pixelsPerUnit : unitAtTheFront.rect.width * sprite_rect_width_scale * 1 / unitAtTheFront.pixelsPerUnit;
    }

    int visibleDigit = 0;
    for (int i = 0; i < mDigits; ++i)
    {
      if (mDigitsGameObject[i].activeSelf == false)
        continue;
      //Debug.Log("5464 - digitsSprite[ mCurrentDigitVal[i] ]， " + digitsSprite[mCurrentDigitVal[i]].name + " use_monospace " + use_monospace + "，digitsSprite[ mCurrentDigitVal[i] ].pivot.x : " + digitsSprite[mCurrentDigitVal[i]].pivot.x + "，sprite_rect_width_scale : " + sprite_rect_width_scale);
      float digitWidth = use_monospace ? monospace_width * 1 / digitsSprite[mCurrentDigitVal[i]].pixelsPerUnit : digitsSprite[mCurrentDigitVal[i]].rect.width * sprite_rect_width_scale * 1 / digitsSprite[mCurrentDigitVal[i]].pixelsPerUnit;
      mDigitsGameObject[i].transform.localPosition = new Vector3(currX + (digitWidth * 0.5f), 0.0f, 0.0f);
      currX += digitWidth;

      //6 543 210
      //1,000,000
      //insert comma ?
      if (mUseComma && insertComma(availableDigits, visibleDigit))
      {
        GameObject tmp_comma = FindComma(availableDigits, visibleDigit);

        float commaWidth = use_monospace ? monospace_width * 1 / commaSprite.pixelsPerUnit : commaSprite.rect.width * sprite_rect_width_scale * 1 / commaSprite.pixelsPerUnit;
        tmp_comma.transform.localPosition = new Vector3(currX + (commaWidth * 0.5f), 0.0f, 0.0f);
        currX += commaWidth;
        tmp_comma.SetActive(true);
      }

      visibleDigit++;
    }

    if (pointSprite != null)
    {

      float pointWidth = use_monospace ? monospace_width * 1 / pointSprite.pixelsPerUnit : pointSprite.rect.width * sprite_rect_width_scale * 1 / pointSprite.pixelsPerUnit;
      mPointGameObject.transform.localPosition = new Vector3(currX + (pointWidth * 0.5f), 0.0f, 0.0f);
      currX += pointWidth;
      mPointGameObject.SetActive(true);

      for (int i = 0; i < mAfterPointDigitsGameObject.Length; ++i)
      {
        if (mAfterPointDigitsGameObject[i].activeSelf == false)
          continue;

        float digitWidth = use_monospace ? monospace_width * 1 / digitsSprite[mCurrentDigitVal[i]].pixelsPerUnit : digitsSprite[mCurrentDigitVal[i]].rect.width * sprite_rect_width_scale * 1 / digitsSprite[mCurrentDigitVal[i]].pixelsPerUnit;
        mAfterPointDigitsGameObject[i].transform.localPosition = new Vector3(currX + (digitWidth * 0.5f), 0.0f, 0.0f);
        currX += digitWidth;
      }
    }

    if (unitAtTheEnd != null)
    {
      mUnitAtTheEndGameObject.transform.localPosition = new Vector3(currX + (use_monospace ? monospace_width * 0.5f * 1 / unitAtTheEnd.pixelsPerUnit : unitAtTheEnd.rect.width * sprite_rect_width_scale * 0.5f * 1 / unitAtTheEnd.pixelsPerUnit), 0.0f, 0.0f);
      currX += use_monospace ? monospace_width * 1 / unitAtTheEnd.pixelsPerUnit : unitAtTheEnd.rect.width * sprite_rect_width_scale * 1 / unitAtTheEnd.pixelsPerUnit;
    }

    if (CancelLineSprite != null)
    {
      mCancelLineGameObject.transform.localPosition = new Vector3(currX * 0.5f, 0.0f, -5.0f);
      cancelline_extra_length = monospace_width * 0.5f;
      mCancelLineGameObject.transform.localScale = new Vector3(currX + cancelline_extra_length * 2.0f, 1.0f, 1.0f);
      mCancelLineGameObject.SetActive(true);
      //cancel dont add currX 先試試看這樣吧，有其他需求再改
    }



  }

  //1,000,000
  bool insertComma(int all_digits, int currSeqNo)
  {
    if (currSeqNo == all_digits - 1)
      return false;

    int tc = all_digits - currSeqNo;
    if (((tc - 1) % 3) == 0)
    {
      return true;
    }
    return false;
  }

  GameObject FindComma(int all_digits, int currSeqNo)
  {
    int tc = all_digits - currSeqNo;
    return mCommaGameObject[((tc - 1) / 3) - 1];
  }

  public void setValue(UInt64 val, Sprite newsprite, int Order = 0)
  {
    setValue(val, AnimationType.NONE, 0.0f, newsprite, Order);
  }

  public void setValue(UInt64 val)
  {
    setValue(val, AnimationType.NONE, 0.0f);
  }

  public void setColor(Color c)
  {
    SpriteRenderer[] allsr = getallnumberSR();
    foreach (SpriteRenderer s in allsr)
      s.color = c;
  }

  public void setValue(UInt64 val, AnimationType anim_type, float duration_time, Sprite newsprite = null, int Order = 0)
  {

    if (val == targetValue && anim_type == mAnimType && newsprite == null)
    {
      return;
    }

    if (mDigitsDirty == true && (anim_type == mAnimType && newsprite == null))
    {
      targetValue = val;
      return;
    }

    mOrder = Order;

    SpriteRenderer[] allsr = getallnumberSR();
    foreach (SpriteRenderer s in allsr)
    {
      s.sortingOrder = mOrder;
    }



    //if val > mDigits，force each mDigits value to 9
    int targetDigits = val.ToString().Length;
    if (targetDigits > mDigits)
    {
      val = (ulong)(Mathf.Pow(10.0f, (mDigits + 1)) - 1);
    }

    mAnimType = anim_type;
    mDurationTime = duration_time;
    mCurrentDurationTime = 0.0f;
    tempStartValue = currentValue;

    string strVal = val.ToString();
    targetValue = val;

    if (mAnimType == AnimationType.NONE)
    {
      currentValue = val;
    }

    //fill zero
    int digitCt = 0;
    while (digitCt + strVal.Length < mDigits)
    {
      setDigitWithVal(digitCt, 0);
      mHideDigit[digitCt] = true;

      //目前值己經是 0, 直接隱藏, 個位數除外
      if (mCurrentDigitVal[digitCt] == 0)
      {
        mDigitsGameObject[digitCt].SetActive(false);
      }

      digitCt++;
    }

    bool isJumpDigit = false;
    for (int i = 0; i < strVal.Length; ++i)
    {
      if (i >= mDigits)
      {
        break;
      }

      int digitval = 0;
      Int32.TryParse(strVal[i].ToString(), out digitval);

      setDigitWithVal(i + digitCt, digitval);

      //假如左邊位婁有 jump, 其所有右邊位數也要 jump
      if (mDigitsJumpTimes[i + digitCt] == true)
        isJumpDigit = true;
      if (isJumpDigit == true)
      {
        mDigitsJumpTimes[i + digitCt] = true;
      }

      mHideDigit[digitCt + i] = false;
      mDigitsGameObject[digitCt + i].SetActive(true);
    }

    //0206 RAY : 商城有部分的num需要更換Unit的圖
    if (newsprite != null)
    {
      if (mUnitAtTheEndGameObject != null)
      {
        unitAtTheEnd = newsprite;
        mUnitAtTheEndGameObject.GetComponent<SpriteRenderer>().sprite = newsprite;
      }
      else
        Debug.LogWarning("Wanna ChangeUnitSprite，but mUnitAtTheEndGameObject == null , please set Unit(Spritename/Atlasname) before init nc ");
    }

    layoutDigits();

    if (anim_type == AnimationType.NONE)
    {
      mDigitsDirty = false;
    }
    else
    {
      mDigitsDirty = true;
    }
  }

  public void setValuewithFront(UInt64 val, AnimationType anim_type, float duration_time, Sprite frontsprite)
  {

    if (val == targetValue && anim_type == mAnimType && frontsprite == null)
    {
      return;
    }

    if (mDigitsDirty == true && (anim_type == mAnimType && frontsprite == null))
    {
      targetValue = val;
      return;
    }

    SpriteRenderer[] allsr = getallnumberSR();
    foreach (SpriteRenderer s in allsr)
    {
      s.sortingOrder = mOrder;
    }



    //if val > mDigits，force each mDigits value to 9
    int targetDigits = val.ToString().Length;
    if (targetDigits > mDigits)
    {
      val = (ulong)(Mathf.Pow(10.0f, (mDigits + 1)) - 1);
    }

    mAnimType = anim_type;
    mDurationTime = duration_time;
    mCurrentDurationTime = 0.0f;
    tempStartValue = currentValue;

    string strVal = val.ToString();
    targetValue = val;

    if (mAnimType == AnimationType.NONE)
    {
      currentValue = val;
    }

    //fill zero
    int digitCt = 0;
    while (digitCt + strVal.Length < mDigits)
    {
      setDigitWithVal(digitCt, 0);
      mHideDigit[digitCt] = true;

      //目前值己經是 0, 直接隱藏, 個位數除外
      if (mCurrentDigitVal[digitCt] == 0)
      {
        mDigitsGameObject[digitCt].SetActive(false);
      }

      digitCt++;
    }

    bool isJumpDigit = false;
    for (int i = 0; i < strVal.Length; ++i)
    {
      if (i >= mDigits)
      {
        break;
      }

      int digitval = 0;
      Int32.TryParse(strVal[i].ToString(), out digitval);

      setDigitWithVal(i + digitCt, digitval);

      //假如左邊位婁有 jump, 其所有右邊位數也要 jump
      if (mDigitsJumpTimes[i + digitCt] == true)
        isJumpDigit = true;
      if (isJumpDigit == true)
      {
        mDigitsJumpTimes[i + digitCt] = true;
      }

      mHideDigit[digitCt + i] = false;
      mDigitsGameObject[digitCt + i].SetActive(true);
    }

    //0206 RAY : 商城有部分的num需要更換Unit的圖
    if (frontsprite != null)
    {
      if (mUnitAtTheFrontGameObject != null)
      {
        unitAtTheFront = frontsprite;
        mUnitAtTheFrontGameObject.GetComponent<SpriteRenderer>().sprite = frontsprite;
      }
      else
        Debug.LogWarning("Wanna ChangeUnitSprite，but mUnitAtTheEndGameObject == null , please set Unit(Spritename/Atlasname) before init nc ");
    }

    layoutDigits();

    if (anim_type == AnimationType.NONE)
    {
      mDigitsDirty = false;
    }
    else
    {
      mDigitsDirty = true;
    }
  }


  void setDigitWithVal(int digit, int val)
  {
    if (mAnimType == AnimationType.NONE)
    {
      ((SpriteRenderer)mDigitsGameObject[digit].GetComponent("SpriteRenderer")).sprite = digitsSprite[val];
      mCurrentDigitVal[digit] = val;

    }
    else
    if (mAnimType == AnimationType.RANDOM_JUMP)
    {
      mTargetDigitVal[digit] = val;

      //reset register
      if (mCurrentDigitVal[digit] == mTargetDigitVal[digit])
      {
        mDigitsJumpTimes[digit] = false; //no needs to jump anymore
      }
      else
      {
        mDigitsJumpTimes[digit] = true;
      }

      mDigitsJumppIdleTimer[digit] = UnityEngine.Random.Range(-0.2f, 0.0f); //extend time for starting

    }
    else
    if (mAnimType == AnimationType.SEQUENCE_JUMP)
    {
      mTargetDigitVal[digit] = val;
      mDigitsJumppIdleTimer[digit] = 0.0f;

    }
  }

  public bool isCounting()
  {
    return mDigitsDirty;
  }

  public float estimate_val_width(System.UInt64 val)
  {
    if (mInited == false)
    {
      return 0f;
    }

    float totalWidth = 0.0f;
    int availableDigits = 0;

    string val_str = val.ToString();
    for (int i = 0; i < val_str.Length; ++i)
    {
      totalWidth += use_monospace ? monospace_width : digitsSprite[int.Parse(val_str[i].ToString())].rect.width * sprite_rect_width_scale;
      availableDigits++;
    }

    if (mUseComma == true)
    {
      // 1,000,000
      int commaNum = (availableDigits - 1) / 3;
      totalWidth += (use_monospace ? monospace_width : commaSprite.rect.width * sprite_rect_width_scale * commaNum);
    }

    if (unitAtTheEnd != null)
    {
      totalWidth += (use_monospace ? monospace_width : unitAtTheEnd.rect.width * sprite_rect_width_scale);
    }

    if (unitAtTheFront != null)
    {
      totalWidth += (use_monospace ? monospace_width : unitAtTheFront.rect.width * sprite_rect_width_scale);
    }

    //0105 RAY : 這裡要再在 * go.lossyScale.x 才比較精準
    return totalWidth * gameObject.transform.lossyScale.x;
  }
  public void SetOnComplete(OnSetValueDone Onvaluecomplete)
  {
    Onsetvaluedone = Onvaluecomplete;
  }

  float mvalue_change_limit = 0.05f;
  float mvalue_change_count = 0.1f;
  public void SetOnValueChange(CommonAction onValueChange)
  {
    OnValueChange = onValueChange;
    mvalue_change_count = 0.1f;
  }

  public SpriteRenderer[] getallnumberSR()
  {

    List<SpriteRenderer> tmp = new List<SpriteRenderer>();
    SpriteRenderer s = null;
    if (mCommaGameObject != null)
    {
      foreach (GameObject g in mCommaGameObject)
      {
        s = g.GetComponent<SpriteRenderer>();
        if (s != null)
          tmp.Add(s);
      }
    }

    if (mCancelLineGameObject != null)
    {
      s = mCancelLineGameObject.GetComponent<SpriteRenderer>();
      if (s != null)
        tmp.Add(s);
    }

    if (mPointGameObject != null)
    {
      s = mPointGameObject.GetComponent<SpriteRenderer>();
      if (s != null)
        tmp.Add(s);
    }

    if (mDigitsGameObject != null)
    {
      foreach (GameObject g in mDigitsGameObject)
      {
        s = g.GetComponent<SpriteRenderer>();
        if (s != null)
          tmp.Add(s);
      }
    }

    if (mAfterPointDigitsGameObject != null)
    {
      foreach (GameObject g in mAfterPointDigitsGameObject)
      {
        s = g.GetComponent<SpriteRenderer>();
        if (s != null)
          tmp.Add(s);
      }
    }

    if (mUnitAtTheEndGameObject != null)
    {
      s = mUnitAtTheEndGameObject.GetComponent<SpriteRenderer>();
      if (s != null)
        tmp.Add(s);
    }

    if (mUnitAtTheFrontGameObject != null)
    {
      s = mUnitAtTheFrontGameObject.GetComponent<SpriteRenderer>();
      if (s != null)
        tmp.Add(s);
    }

    return tmp.ToArray();
  }
  public void setDepth(float depth)
  {
    SpriteRenderer[] tmp = getallnumberSR();
    foreach (SpriteRenderer s in tmp)
    {
      s.gameObject.transform.localPosition = new Vector3(s.gameObject.transform.localPosition.x, s.gameObject.transform.localPosition.y, depth);
    }
  }

  //多語系的時候可以在init前動態修改Front 圖資
  public void overrideFront(string Atlas, string Spritename)
  {
    unitAtTheFrontAtlasName = Atlas;
    unitAtTheFrontSpriteName = Spritename;
  }
  public void overrideEnd(string Atlas, string Spritename)
  {
    unitAtTheEndAtlasName = Atlas;
    unitAtTheEndSpriteName = Spritename;
  }
  public void overrideNum(string[] Atlas, string[] Spritename)
  {
    digitsAtlasName = Atlas;
    digitsSpriteName = Spritename;
  }
}

