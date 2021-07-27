using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazePlayerController : MonoBehaviour
{
  [SerializeField]
  private float basic_speed = 10.0f;
  private float maze_size;
  //玩家火光範圍
  float maskscale = 3.0f;

  private int currentx, currenty;
  private List<Cell> movepath = null;

  private List<Cell> trackpath = new List<Cell>();
  private LineRenderer LineRenderer = null;
  bool Tracking = false;
  private int maskid;
  enum MoveState
  {
    Arrival,
    Moving,
  }

  MoveState mcurrentState = MoveState.Arrival;
    // Update is called once per frame
    void Update()
    {

    move();

    if(Tracking)
      updateTrackLine();

  }

  public void init(int currentx, int currenty, float maze_size) {
    this.maze_size = maze_size;
    this.currentx = currentx;
    this.currenty = currenty;

    //感覺3倍的maze_size比較舒服

    //gameObject.transform.localScale = new Vector3(maze_size, maze_size, 1.0f);
    GameObject icon_go = transform.Find("Icon").gameObject;
    Sprite icon = icon_go.GetComponent<SpriteRenderer>().sprite;
    if (icon != null){
      float iconscale = maze_size / icon.bounds.size.x;//根據圖資重新計算scale大小
      icon_go.transform.localScale = new Vector3(iconscale, iconscale, 0.0f);
    }

    LineRenderer = gameObject.GetComponent<LineRenderer>();
    LineRenderer.startWidth = LineRenderManager._LineRenderManager.lineWidth();
    LineRenderer.endWidth = LineRenderManager._LineRenderManager.lineWidth();

    Cell StartCell = MazeManager._MazeManager.GetMaze().GetCell(currentx, currenty);
    //StartCell.PlayerVisitedState = CellState.Visited;
    //trackpath.Add(StartCell);

    maskid = MaskManager._MaskManager.AddMask(transform, "player", maskscale * maze_size, true);
  }

  public Vector2 position(){
    return  MazeManager._MazeManager.GetMaze().GetCellPosition(currentx, currenty);
  }


  public bool IsMoving(){
    return mcurrentState == MoveState.Moving;
  }

  public void moveDir(Dir dir){
    if (mcurrentState == MoveState.Moving)
      return;

    AudioController._AudioController.playOverlapEffect("腳色移動時_玩家滑動螢幕時撥放");

    //movepath是不包含現在站著的Cell
    movepath = MazeManager._MazeManager.GetMaze().movePath(currentx, currenty,dir);

    if (movepath.Count == 0)
      return;

    //我必須先知道將要移動的點是不是已經在trackpath中了
    int CellIntrackpathindex = trackpath.IndexOf(movepath[0]);
    if (CellIntrackpathindex >= 0){
      //如果是表示正在收線
      //那就移除腳下的點
      RemoveTrackPath(MazeManager._MazeManager.GetMaze().GetCell(currentx, currenty));
    }

    //Debug.Log("MOVESTART : " + mcurrentType);
    //Debug.Log("移動格數 : " + movepath.Count);

    mcurrentState = MoveState.Moving;
  }

  public float maskScale(){
    return maskscale;
  }

  void move(){
    if (mcurrentState == MoveState.Arrival)
      return;

    if(movepath.Count == 0){
      mcurrentState = MoveState.Arrival;
      return;
    }

    Vector2 currentposi = gameObject.transform.position;
    Cell TargetCell = movepath[0];
    
    Vector2 dir = (TargetCell.position() - currentposi).normalized;
    float dis = basic_speed * Time.deltaTime * maze_size;

    if ((TargetCell.position() - currentposi).magnitude <= dis){
      gameObject.transform.position = TargetCell.position();
      mcurrentState = MoveState.Arrival;
      //Debug.Log("到達位置[" + TargetCell.position.x + "，" + TargetCell.position.x + "]， CellState : " + TargetCell.PlayerVisitedState);
      currentx = movepath[0].X;
      currenty = movepath[0].Y;

      MazeManager._MazeManager.ArrivalCell("player", TargetCell);

      //到達目標時
      int CellIntrackpathindex = trackpath.IndexOf(TargetCell);
      if(CellIntrackpathindex < 0){
        AddTrackPath(TargetCell);
      }
      //RemoveTrackPath(TargetCell);


        movepath.RemoveAt(0);
      if (movepath.Count > 0)
        mcurrentState = MoveState.Moving;
      else{//移動到最後一個的時候紀錄位置
        PlayerPrefsManager._PlayerPrefsManager.updateRecord(new Vector2(currentx, currenty));
        return;
      }
      //如果還有目標
      //我必須先知道即將要移動到的點是不是已經在trackpath中了
      CellIntrackpathindex = trackpath.IndexOf(movepath[0]);
      if (CellIntrackpathindex >= 0)
      {
        //如果是表示正在收線
        //那就移除腳下的點
        RemoveTrackPath(MazeManager._MazeManager.GetMaze().GetCell(currentx, currenty));
      }

    }

    gameObject.transform.position += (Vector3)(dir * dis);

    //CanvasMaskManager._MazeMaskManager.updateMaskPosion(gameObject.transform.position);
  }

  void AddTrackPath(Cell targetCell){
    if (Tracking == false)
      return;
    //開始的點不會被移除
    //if (targetCell.Type == CellType.Start)
    //  return;
    //Debug.Log("位置[" + targetCell.position.x + "，" + targetCell.position.y + "]， 被加入");
    trackpath.Add(targetCell);
  }

  void RemoveTrackPath(Cell targetCell){
    if (Tracking == false)
      return;
    //開始的點不會被移除
    //if (targetCell.Type == CellType.Start)
    //  return;
    //Debug.Log("位置[" + targetCell.position.x + "，" + targetCell.position.y + "]， 被移除");
    //int index = trackpath.LastIndexOf(targetCell);
    //trackpath.RemoveAt(index);
    trackpath.Remove(targetCell);
  }

  void updateTrackLine(){
    if (LineRenderer == null)
      return;

    if (trackpath.Count <= 0)
      return;

    LineRenderer.positionCount = trackpath.Count +1;
    float linedepth = 1.0f;

    Vector3[] trackpositions = new Vector3[trackpath.Count+1];
      int index = 0;
      foreach (var v in trackpath){
        trackpositions[index] = new Vector3(v.position().x, v.position().y, linedepth);
        index++;
      }

    trackpositions[trackpath.Count] = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, linedepth);
    LineRenderer.SetPositions(trackpositions);

    if (trackpath.Count - 1 > 5)
    {
      LineRenderManager._LineRenderManager.BuildLine(trackpositions);
      PlayerPrefsManager._PlayerPrefsManager.updateRecord(trackpositions);
      EndTrackLine();
      return;
    }
  }

  public void StartTrackLine(){
    trackpath.Add(MazeManager._MazeManager.GetMaze().GetCell(currentx, currenty));
    Tracking = true;
  }

  public bool IsTrackLine()
  {
    return Tracking == true;
  }

  void EndTrackLine()
  {
    trackpath = new List<Cell>();
    LineRenderer.positionCount = 0;/*(new Vector3[] { Vector2.zero, Vector2.zero });//洗掉本身的*/
    Tracking = false;
  }
}
