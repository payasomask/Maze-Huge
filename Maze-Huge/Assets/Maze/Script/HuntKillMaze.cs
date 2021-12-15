using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HuntKillMaze : Maze
{
  public HuntKillMaze(int max_row, int max_column, float grid_size, Vector3 maze_pivot)
  {
    this.max_columns = max_column;
    this.max_rows = max_row;
    this.grid_size = grid_size;
    this.maze_pivot = maze_pivot;
    //Debug.Log("Maze Point : " + maze_pivot);
  }

  class HuntData {
    public Cell LinkCell = null;
    public Cell PickCell = null;
  }

  //可選的Cell
  private List<Cell> PickPool_List = new List<Cell>();

  public override void BuildMaze()
  {

    //GizmosDebug._gizmosdebug.clear_debug_list();

    resetmaze();

    //可能要從邊邊開始...
    //int coordinateX = Random(0,max_rows);
    //int coordinateY = Random(0, max_columns);

    Cell currentCell = maze_cell_matrix[2, 0];
    currentCell.State = CellState.Visited;
    //Debug.Log("目前Cell座標 : [" + currentCell.X + "，" + currentCell.Y + "]");
    bool mazenotdone = true;
    addPick_Pool(currentCell.X, currentCell.Y);

    CellSpriteFactory csf = new CellSpriteFactory();

    while (mazenotdone){


      while (PickPool_List.Count > 0){

        //從pickpool裡面隨機挑一個cell
        int pickindex = UtilityHelper.Random(0, PickPool_List.Count);
        Cell pickCell = PickPool_List[pickindex];
        //Debug.Log("pickCell座標 : [" + pickCell.X + "，" + pickCell.Y + "]");

        //從已經是迷宮的部分，並且相鄰pickCell的上下左右挑一個Cell連通
        Cell LinkCell = currentCell;

        //Debug.Log("LinkCell座標 : [" + LinkCell.X + "，" + LinkCell.Y + "]");

        PickPool_List.Clear();

        //並且要選擇從哪一邊打通牆壁
        BreakWall(pickCell, LinkCell);

        currentCell = pickCell;
        currentCell.State = CellState.Visited;

        addPick_Pool(currentCell.X, currentCell.Y);
      }

      //掃描
      HuntData hd = FindNotVisitedCell();

      //有huntdata，表是迷宮還沒生成結束
      if(hd != null){
        //打通兩個Cell牆
        BreakWall(hd.PickCell, hd.LinkCell);

        currentCell = hd.PickCell;
        currentCell.State = CellState.Visited;
        PickPool_List.Clear();
        addPick_Pool(currentCell.X, currentCell.Y);
        csf.ProcessCell(hd.PickCell);
        csf.ProcessCell(hd.LinkCell);
        continue;
      }

      mazenotdone = false;
    }
    DrawWall();
    PlayerPrefsManager._PlayerPrefsManager.updateRecord(maze_cell_matrix);
  }


  Cell selectionLinkCell(Cell sourceCell)
  {
    List<Cell> Visited_Cell = new List<Cell>(4);

    Cell tmp = null;
    tmp = getTopCell(sourceCell.X, sourceCell.Y, CellState.Visited);
    if (tmp != null)
    {
      Visited_Cell.Add(tmp);
      //Debug.Log("加入Link上方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "]");
    }
    tmp = getBottomCell(sourceCell.X, sourceCell.Y, CellState.Visited);
    if (tmp != null)
    {
      Visited_Cell.Add(tmp);
      //Debug.Log("加入Link下方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "]");
    }
    tmp = getRightCell(sourceCell.X, sourceCell.Y, CellState.Visited);
    if (tmp != null)
    {
      Visited_Cell.Add(tmp);
      //Debug.Log("加入Link右方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "]");
    }
    tmp = getLeftCell(sourceCell.X, sourceCell.Y, CellState.Visited);
    if (tmp != null)
    {
      Visited_Cell.Add(tmp);
      //Debug.Log("加入Link左方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "]");
    }

    if (Visited_Cell.Count == 0)
      return null;

    return Visited_Cell[UtilityHelper.Random(0, Visited_Cell.Count)];
  }

  //LinkCell為原本就是迷宮的Cell
  void BreakWall(Cell pickCell, Cell LinkCell)
  {
    //方向以原本是迷宮的Cell為準


    if (pickCell.X == LinkCell.X)
    {
      //表示pick在左或是右方

      if (pickCell.Y > LinkCell.Y)
      {
        //pickCell在LinkCell右邊，打掉link右邊的牆
        LinkCell.RightWall = false;
        //而對pickCell來說就是打通左邊的牆
        pickCell.LeftWall = false;
        //Debug.Log("LinkCell打通右邊牆");
      }
      else
      {
        LinkCell.LeftWall = false;
        pickCell.RightWall = false;
        //Debug.Log("LinkCell打通左邊牆");
      }
    }
    else
    {
      //表示pick在上或是下方

      if (pickCell.X > LinkCell.X)
      {
        //pickCell在LinkCell上邊，打掉link上邊的牆
        LinkCell.TopWall = false;
        //而對pickCell來說就是打通下邊的牆
        pickCell.BottomWall = false;
        //Debug.Log("LinkCell打通上邊牆");
      }
      else
      {
        LinkCell.BottomWall = false;
        pickCell.TopWall = false;
        //Debug.Log("LinkCell打通下邊牆");
      }
    }


  }

  //回傳null的話表示迷宮生成結束
  HuntData FindNotVisitedCell(){
    //逐row下而上由左而右找尋有相連迷宮部分Cell的未被訪問Cell
    for (int i = 0; i < max_rows; i++){
      for (int j = 0; j < max_columns; j++) {
        Cell tmp = maze_cell_matrix[i, j];
        if (tmp.State == CellState.Visited)
          continue;

        //如果是未被訪問的話要開始收集已經是迷宮部分的cell
        Cell LinkCell = selectionLinkCell(tmp);

        //如果完全沒有相鄰迷宮部分，就往下一個找
        if (LinkCell == null)
          continue;

        //如果有相鄰迷宮部分，隨機取一個相鄰的當作LinkCell並且回傳
        return new HuntData() { LinkCell = LinkCell, PickCell = tmp };
      }
    }

    return null;
  }

  enum LinkDir
  {
    Top,
    Bottom,
    Left,
    Right
  }




  void addPick_Pool(int coordinateX, int coordinateY)
  {

    Cell tmp = null;
    tmp = getTopCell(coordinateX, coordinateY, CellState.NotVisited);
    if (tmp != null)
    {
      if (PickPool_List.IndexOf(tmp) < 0)
      {
        PickPool_List.Add(tmp);
        //Debug.Log("加入Pick上方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "]");
      }
      else
      {
        //Debug.Log("上方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "] 已經在pick名單內忽略該cell");
      }
    }
    tmp = getBottomCell(coordinateX, coordinateY, CellState.NotVisited);
    if (tmp != null)
    {
      if (PickPool_List.IndexOf(tmp) < 0)
      {
        PickPool_List.Add(tmp);
        //Debug.Log("加入Pick下方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "]");
      }
      else
      {
        //Debug.Log("下方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "] 已經在pick名單內忽略該cell");
      }
    }
    tmp = getRightCell(coordinateX, coordinateY, CellState.NotVisited);
    if (tmp != null)
    {
      if (PickPool_List.IndexOf(tmp) < 0)
      {
        PickPool_List.Add(tmp);
        //Debug.Log("加入Pick右方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "]");
      }
      else
      {
        //Debug.Log("右方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "] 已經在pick名單內忽略該cell");
      }
    }
    tmp = getLeftCell(coordinateX, coordinateY, CellState.NotVisited);
    if (tmp != null)
    {
      if (PickPool_List.IndexOf(tmp) < 0)
      {
        PickPool_List.Add(tmp);
        //Debug.Log("加入Pick左方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "]");
      }
      else
      {
        //Debug.Log("左方Cell，Cell 座標 : [" + tmp.X + "，" + tmp.Y + "] 已經在pick名單內忽略該cell");
      }
    }
  }

  void resetmaze()
  {
    if (maze_cell_matrix == null)
      maze_cell_matrix = new Cell[max_rows, max_columns];
    else
      if (maze_cell_matrix.Length != max_columns * max_rows)
    {
      //表示迷宮長寬改變了
      maze_cell_matrix = new Cell[max_rows, max_columns];
    }

    for (int i = 0; i < max_rows; i++)
    {
      for (int j = 0; j < max_columns; j++)
      {
        if (maze_cell_matrix[i, j] == null)
        {
          maze_cell_matrix[i, j] = new Cell() { X = i, Y = j, Jposition = new JsonVector2(j * grid_size + grid_size * 0.5f- maze_pivot.x, i * grid_size + grid_size * 0.5f - maze_pivot.y), 
            BottomWall = true, LeftWall = true, RightWall = true, TopWall = true,
            State = CellState.NotVisited};
          //Debug.Log("Cell座標 : [" + maze_cell_matrix[i, j].X + "，" + maze_cell_matrix[i, j].Y + "]的 Cell position : " + maze_cell_matrix[i,j].position);
        }
        else
        {
          Cell tmp = maze_cell_matrix[i, j];
          tmp.State = CellState.NotVisited;
          tmp.TopWall = true;
          tmp.BottomWall = true;
          tmp.LeftWall = true;
          tmp.RightWall = true;
        }
      }
    }
  }

  void DrawWall()
  {
    foreach (var v in maze_cell_matrix)
    {
      //if (v.BottomWall == true)
      //  WallBuilder._WallBuilder.BuildBottomWall(v.position(), grid_size);
      //if (v.LeftWall == true)
      //  WallBuilder._WallBuilder.BuildLeftWall(v.position(), grid_size);
      //if (v.TopWall == true)
      //  WallBuilder._WallBuilder.BuildTopWall(v.position(), grid_size);
      //if (v.RightWall == true)
      //  WallBuilder._WallBuilder.BuildRightWall(v.position(), grid_size);
      WallBuilder._WallBuilder.BuildCell(v,grid_size);
    }
  }

  public override void ResetMaze()
  {
    WallBuilder._WallBuilder.ClearWall();
  }

  public override void BuildMaze(Cell[,] cells){
    maze_cell_matrix = cells;
    this.max_columns = cells.GetLength(1);
    this.max_rows = cells.GetLength(0);
    this.grid_size = MazeManager._MazeManager.getCellSize();
    DrawWall();
  }

  //public Vector2 GetCellPosition(int x,int y){
  //  return maze_cell_matrix[x, y].position;
  //}

  //public override Cell[,] getMazeCell()
  //{
  //  return maze_cell_matrix;
  //}
}
