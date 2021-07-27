using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Maze {

  public Cell[,] maze_cell_matrix;
  public int max_columns;
  public int max_rows;
  public float grid_size;
  public Vector2 maze_pivot;

  public abstract void BuildMaze();

  public abstract void BuildMaze(Cell[,] cells);

  public Cell[,] getMazeCell(){
    return maze_cell_matrix;
  }
  public Vector2 GetCellPosition(int x, int y){
    return maze_cell_matrix[x, y].position();
  }

  public Cell GetCell(int x, int y){
    return maze_cell_matrix[x, y];
  }

  public bool isRigth_bound(int y, int column_bound)
  {
    return y + 1 == column_bound;
  }

  public Cell getRightCell(int x, int y, CellState targetstate)
  {
    if (isRigth_bound(y, max_columns))
      return null;
    if (maze_cell_matrix[x, y + 1].State == targetstate)
      return maze_cell_matrix[x, y + 1];
    return null;
  }
  public bool isTop_bound(int x, int row_bound)
  {
    return x + 1 == row_bound;
  }
  public Cell getTopCell(int x, int y, CellState targetstate)
  {
    if (isTop_bound(x, max_rows))
      return null;
    if (maze_cell_matrix[x + 1, y].State == targetstate)
      return maze_cell_matrix[x + 1, y];

    return null;
  }

  public bool isLeft_bound(int y)
  {
    return y - 1 < 0;
  }
  public Cell getLeftCell(int x, int y, CellState targetstate)
  {
    if (isLeft_bound(y))
      return null;
    if (maze_cell_matrix[x, y - 1].State == targetstate)
      return maze_cell_matrix[x, y - 1];
    return null;
  }
  public bool isBottom_bound(int x)
  {
    return x - 1 < 0;
  }
  public Cell getBottomCell(int x, int y, CellState targetstate)
  {
    if (isBottom_bound(x))
      return null;
    if (maze_cell_matrix[x - 1, y].State == targetstate)
      return maze_cell_matrix[x - 1, y];

    return null;
  }

  public List<Cell> movePath(int x, int y, Dir movedir){
    List<Cell> Cell_list = new List<Cell>();

    onahead(x, y, movedir,ref Cell_list);
    return Cell_list;
  }

  void onahead(int x, int y, Dir movedir, ref List<Cell> Cell_list){
    if (hasWall(x, y, movedir))
      return;

    Cell currentCell = maze_cell_matrix[x, y];
    Cell Next = GetNextCell(currentCell.X, currentCell.Y, movedir);
    if (Next == null)
      return;

    Cell_list.Add(Next);
    Dir[] canmovedir = DonthasWall(Next.X, Next.Y, movedir);
    if (canmovedir.Length == 0 || canmovedir.Length >= 2){
      return;
    }

    //如果遇到幾個特殊點停下來
    if (Next.Type == CellType.Start || Next.Type == CellType.Goal || Next.Type == CellType.Box)
      return;

    onahead(Next.X,Next.Y, canmovedir[0],ref Cell_list);
  }

  //依照方向，取得該方向的下格CELL
  Cell GetNextCell(int x, int y, Dir movedir){

    if (hasWall(x, y, movedir))
      return null;

    if (movedir == Dir.Top)
      return maze_cell_matrix[x + 1, y];
    else if(movedir == Dir.Bottom)
      return maze_cell_matrix[x - 1, y];
    else if(movedir == Dir.Right)
      return maze_cell_matrix[x, y + 1];
    else
      return maze_cell_matrix[x, y - 1];
  }

  //判斷該座標cell的該方向是不是有牆阻擋
  bool hasWall(int x, int y,Dir movedir){
    if (movedir == Dir.Top)
      return maze_cell_matrix[x, y].TopWall;
    else if(movedir == Dir.Bottom)
      return maze_cell_matrix[x, y].BottomWall;
    else if(movedir == Dir.Left)
      return maze_cell_matrix[x, y].LeftWall;
    else
      return maze_cell_matrix[x, y].RightWall;
  }

  //判斷該cell的上下左右方向沒有牆阻擋
  //來到該格的方向A -> B 那方向就是往右
  Dir[] DonthasWall(int x, int y, Dir commingdir){

    List<Dir> dir_list = new List<Dir>(4);

    for (int i = 0; i < (int)Dir.SZ; i++){
      Dir d = (Dir)i;
      if (d == counterDir(commingdir))
        continue;
      if (hasWall(x, y, d))
        continue;

      dir_list.Add(d);
    }

    return dir_list.ToArray();
  }

  //取得反方向
  Dir counterDir(Dir d){
    if (d == Dir.Bottom)
      return Dir.Top;
    if (d == Dir.Top)
      return Dir.Bottom;
    if (d == Dir.Right)
      return Dir.Left;
    if (d == Dir.Left)
      return Dir.Right;

    return Dir.SZ;
  }


  public abstract void ResetMaze();
}

public class Cell
{
  public int X;
  public int Y;
  public JsonVector2 Jposition;
  public CellState State;
  public CellType Type;
  public bool TopWall;
  public bool BottomWall;
  public bool LeftWall;
  public bool RightWall;

  public Vector2 position(){
    return Jposition.vector2();
  }
}

public enum CellState
{
  NotVisited = 0,
  Visited,
}

public enum Dir
{
  Top = 0,
  Bottom,
  Right,
  Left,
  SZ
}

public enum CellType{
  Road,
  Start,
  Goal,
  Box
}


