using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//為全屏的遮罩，比較沒有彈性只能遮罩一個圖形
public class CanvasMaskManager : MonoBehaviour
{
  public static CanvasMaskManager _MazeMaskManager = null;
  [SerializeField]
  private GameObject MaskObj = null;
  private GameObject maskgo = null;
  private RectTransform maskTr = null;
  private RectTransform blackTr = null;
  bool inited = false;

  private void Awake(){
    _MazeMaskManager = this;
  }

  public void init(float whidth,float hight,float maskscale){
    maskgo = Instantiate(MaskObj, Vector3.zero,Quaternion.identity);
    maskTr = maskgo.transform.Find("mask").GetComponent<RectTransform>();
    maskTr.sizeDelta = new Vector2(maskscale, maskscale);
    blackTr = maskgo.transform.Find("mask/black").GetComponent<RectTransform>();
    blackTr.sizeDelta = new Vector2(whidth, hight);

    inited = true;
  }

  public void updateMaskPosion(Vector2 position){
    if (inited == false)
      return;

    maskTr.localPosition = position;
    blackTr.localPosition = new Vector3(-maskTr.position.x , -maskTr.position.y, maskTr.position.z);
  }

}
