using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDialog : MonoBehaviour{

  public static UIDialog _UIDialog = null;
  public class NormalMsgDlg :IDialogContext{
    public bool binited = false;
    public string message;
    public GameObject dlgGO = null;
    // public GameObject closeBtn = null;
    public UnityEngine.UI.Text tm = null;

    public bool close_btn_act = false;

    public NormalMsgDlg(string msg, bool show_close_btn) {
      close_btn_act=show_close_btn;
      message=msg;
    }

    public DialogType getType() { return DialogType.NORMAL; }
    public GameObject init(int dlgIdx, AssetbundleLoader abl) {
      dlgGO=abl.InstantiatePrefab("DialogMsgDlg");

      // closeBtn=dlgGO.transform.Find("CLOSEBTN").gameObject;
      // closeBtn.name="EMBEDDEDDLG_CLOSEBTN_"+dlgIdx;
      // closeBtn.SetActive(close_btn_act);

      CameraExtensions.CanvasScreenSpaceCamera(dlgGO);

      tm = dlgGO.transform.Find("Canvas/Text").GetComponent<UnityEngine.UI.Text>();
      tm.text=message;

      binited=true;

      return dlgGO;
    }


    public void updateMessage(string msg) {
      message=msg;

      if(tm==null)
        return;
      tm.text= msg;
    }

    // public void setCloseBtnActivity(bool act) {
    //   close_btn_act=act;
    //   if(closeBtn==null)
    //     return;
    //   closeBtn.SetActive(close_btn_act);
    // }

    public bool inited() { return binited; }

    public bool dismiss() {
      GameObject.Destroy(dlgGO);
      return true;
    }

    public DialogResponse setUIEvent(string name, UIEventType type, object[] extra_info){
      if (name.Contains("EMBEDDEDDLG_CLOSEBTN_")){
        return DialogResponse.TAKEN_AND_DISMISS;
      }

      return DialogResponse.PASS;
    }

    public DialogNetworkResponse setNetworkResponseEvent(string name, object payload){
      return DialogNetworkResponse.PASS;
    }

    public DialogEscapeType getEscapeType(){
      return DialogEscapeType.NOTHING;
    }
  }

  int startZDepth =-20;
  class DialogInfo{
    public int dlgIdx;
    public GameObject parent_obj;
    public IDialogContext dlgCxt;
  }
  List<DialogInfo> mMyDialogList =new List<DialogInfo>();
  int mDlgIdx =0;

  AssetbundleLoader mABL =null;

  private void Awake()
  {
    _UIDialog = this;
  }

  // Use this for initialization
  void Start () {
  }
  
  // Update is called once per frame
  void Update () {
    checkAndShowDialog();
    checkAndDismissDialog();
  }

  int findStartZDepth(){
    if (mMyDialogList.Count==0){
      return startZDepth;
    }

    int s =startZDepth;
    for (int i=0;i<mMyDialogList.Count;++i){
      if (mMyDialogList[i].parent_obj.transform.localPosition.z <s){
        s =(int)mMyDialogList[i].parent_obj.transform.localPosition.z;
      }
    }

    return (s-zstride);
  }
  int zstride =350;

  void checkAndShowDialog(){
    // 0213 RAY setting那邊有必須要超過一個的dailog顯示
    //if (mMyDialogList.Count>0){
    //  return;
    //}

    if (mQueuedDialog.Count<=0){
      return;
    }

    QueuedDialog qd =mQueuedDialog[0];
    mQueuedDialog.RemoveAt(0);

    DialogInfo tmpdlg =new DialogInfo();
    tmpdlg.dlgIdx=qd.dlgIdx;
    tmpdlg.dlgCxt=qd.dlgCxt;
    tmpdlg.parent_obj=new GameObject();
    tmpdlg.parent_obj.name="Dialog_"+tmpdlg.dlgIdx;
    tmpdlg.parent_obj.transform.SetParent(gameObject.transform, false);
    tmpdlg.parent_obj.transform.localPosition=new Vector3(0f, 0f, findStartZDepth());

    GameObject obj =tmpdlg.dlgCxt.init(tmpdlg.dlgIdx, mABL);
    obj.transform.SetParent(tmpdlg.parent_obj.transform, false);

    mMyDialogList.Insert(0, tmpdlg); //event 處理dialogue array 順序是由小到大, (上層到下層)

  }

  void checkAndDismissDialog(){
    if (mQueueRemoveDialog.Count<=0)
      return;
    
    int dialog_id =mQueueRemoveDialog[0];
    mQueueRemoveDialog.RemoveAt(0);

    for (int i=0;i<mMyDialogList.Count;++i){
      if (mMyDialogList[i].dlgIdx==dialog_id){
        if(mMyDialogList[i].dlgCxt.getType()==DialogType.DELAY_DESTROY){
          if (mMyDialogList[i].dlgCxt.dismiss()){
            GameObject.Destroy(mMyDialogList[i].parent_obj, 1f);
          }else{
            Debug.Log("153 - dialog "+dialog_id+" refused to dismiss");
            return;
          }
        }else{
          if (mMyDialogList[i].dlgCxt.dismiss()){
            GameObject.Destroy(mMyDialogList[i].parent_obj);
          }else{
            Debug.Log("160 - dialog "+dialog_id+" refused to dismiss");
            return;
          }
        }
        mMyDialogList.RemoveAt(i);

        // checkAndShowDialog();

        // anydailogdismissed = true;

        Debug.Log("137 - remove dialog "+dialog_id);
        break;
      }
    }
  }

  public void setupAssetbundleLoader(AssetbundleLoader ab){
    mABL =ab;
  }
  

  //在一次的按鈕事件中，如果有任何dialog被dismiss 則 anydailogdismissed = true
  public void dismiss(int dialog_id){
    if (mQueueRemoveDialog.Contains(dialog_id))
      return;
      
    mQueueRemoveDialog.Add(dialog_id);

  }

  public void dismiss_all(){
    mQueuedDialog.Clear();
    for (int i=0;i<mMyDialogList.Count;++i){
      if (mQueueRemoveDialog.Contains(mMyDialogList[i].dlgIdx)==false){
        mQueueRemoveDialog.Add(mMyDialogList[i].dlgIdx);
      }
    }
  }

  public bool isDialogShowing(int specific_dlg) {
    if(specific_dlg==-1)
      return false;

    // if(getCurrentDialogIdx()==specific_dlg) {
    //   return true;
    // }
    for (int i=0;i<mMyDialogList.Count;++i){
      if (mMyDialogList[i].dlgIdx ==specific_dlg){
        return true;
      }
    }
    return false;
  }

  public bool isDialogShowing(){
    return (mMyDialogList.Count>0);
  }

  public int  getCurrentDialogIdx(){
    if (mMyDialogList.Count>0)
      return mMyDialogList[0].dlgIdx;
    return -1;
  }

  public int getQueuedDialog(){
    return mQueuedDialog.Count;
  }

  public IDialogContext getDlgCxt(int idx) {
    for (int i = 0;i<mMyDialogList.Count;++i) {
      if(mMyDialogList[i].dlgIdx==idx) {
        return mMyDialogList[i].dlgCxt;
      }
    }

    Debug.LogWarning("163 - dialog id "+idx+" not found");
    return null;
  }

  class QueuedDialog {
    public int dlgIdx;
    public IDialogContext dlgCxt;
  }
  List<QueuedDialog> mQueuedDialog = new List<QueuedDialog>();
  List<int> mQueueRemoveDialog =new List<int>();

  public int show(IDialogContext dialogCxt) {
    QueuedDialog qd = new QueuedDialog();
    qd.dlgIdx=mDlgIdx;
    qd.dlgCxt=dialogCxt;
    mDlgIdx++;
    mQueuedDialog.Add(qd);

    return qd.dlgIdx;
  }

  //ui 事件優先給 dialog 處理, 
  // 回傳 true 代表 uidialog 把訊息處理完畢, 不要把 ui 事件傳給 scene
  // 回傳 false 代表不管 uidialog 沒有要處理這個 ui event, 把 ui event pass 給 scene
  public bool setUIEvent(string name, UIEventType type, object[] extra_info){

    if (name == "back_bt" && type == UIEventType.BUTTON){
      if (mMyDialogList == null || mMyDialogList.Count == 0)
        return false;
      Debug.Log("999 -  UIDialog taken device back bt event");
      if (mMyDialogList[0].dlgCxt.getEscapeType() == DialogEscapeType.PASS)
        return false;
      else if (mMyDialogList[0].dlgCxt.getEscapeType() == DialogEscapeType.DISMISS){
        dismiss(mMyDialogList[0].dlgIdx);
        return true;
      }
      else if (mMyDialogList[0].dlgCxt.getEscapeType() == DialogEscapeType.NOTHING)
        return true;
      //return true; //有些context 需要對這個行為做事情
    }

    for (int i=0;i<mMyDialogList.Count;++i){
      int curr_dlg_idx =mMyDialogList[i].dlgIdx;
      DialogResponse dr =mMyDialogList[i].dlgCxt.setUIEvent(name, type, extra_info);
      if (curr_dlg_idx !=mMyDialogList[i].dlgIdx){
        Debug.LogError("241 - mMyDialogList list modified during iteration !");
        break;
      }
      if (dr ==DialogResponse.TAKEN){
        return true;
      }else
      if (dr ==DialogResponse.PASS){
        // ++i;

      }else
      if (dr ==DialogResponse.PASS_AND_DISMISS){
        // int tmpct =mMyDialogList.Count;
        dismiss(mMyDialogList[i].dlgIdx);
        // if(mMyDialogList.Count >=tmpct){
        //   ++i;
        // }
      }else
      if (dr ==DialogResponse.TAKEN_AND_DISMISS){
        //如果當interationdialog的callback又有一個dialog被建立..，後者會先被建立，之後前者會回傳TAKEN_AND_DISMISS，導致後者被刪除..
        dismiss(mMyDialogList[i].dlgIdx);
        return true;
      }/* else
      if (dr == DialogResponse.TAKEN_AND_DISMISS_PREVIOUS){
        dismiss(getCurrentDialogIdx() - 1);//-1為了關閉目前顯示的上一個dialog，而不是關閉新被建立出來的那個
        return true;
      }*/

    }

    return false;
  }

  public bool setNetworkResponseEvent(string name, object payload){
    for (int i=0;i<mMyDialogList.Count;/*++i*/){
      DialogNetworkResponse dr =mMyDialogList[i].dlgCxt.setNetworkResponseEvent(name, payload);
      if (dr ==DialogNetworkResponse.TAKEN){
        return true;
      }else
      if (dr ==DialogNetworkResponse.PASS){
        ++i;
      }

    }

    return false;
  }

}
