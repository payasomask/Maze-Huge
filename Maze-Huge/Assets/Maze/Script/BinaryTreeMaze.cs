using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinaryTreeMaze
{
  //生成迷宮時要打掉的牆
  public enum WallBreak
  {
    TopWall = 1,//上牆
    RightWall,//右牆
    Non,
  }

  public BinaryTreeMaze(int max_row, int max_column, int grid_size){
    this.max_columns = max_column;
    this.max_rows = max_row;
    this.grid_size = grid_size;
    maze_pivot = UtilityHelper.MazePoint(max_columns * grid_size, max_rows * grid_size);
    Debug.Log("Maze Point : " + maze_pivot);
  }

  public class MazeGrid
  {
    public int id;
    public Vector2 position;
    public Sprite wall;
    public WallBreak wall_break;
  }

  private int max_columns = 4;
  private int max_rows = 4;
  private int grid_size = 1;
  private Vector2 maze_pivot;
  List<MazeGrid> grid_list = new List<MazeGrid>();

  //只會產生出1、2，對應WellSpawnDebug
  int RandomWall(){
    return UtilityHelper.Random(1,3);
  }

  public void BuildMaze(){

    //GizmosDebug._gizmosdebug.clear_debug_list();
    grid_list.Clear();

    for (int i = 0; i < max_columns; i++){
      for(int j = 0; j < max_rows; j++){
        WallBreak break_Wall = maze_bound(i, j, max_columns, max_rows);
        //grid_size * 0.5f 先讓迷宮最左下角是在0,0的位置// - maze_pivot 讓整個迷宮在畫面正中央
        Vector2 gridPosition = new Vector2(i* grid_size + grid_size*0.5f - maze_pivot.x , j* grid_size+ grid_size * 0.5f - maze_pivot.y); 
        //Debug.Log("gird position : " + gridPosition + "，WellSpawnDebug : " + break_Wall);
        //GizmosDebug._gizmosdebug.AddDebugGrid(gridPosition, break_Wall == WallBreak.TopWall? Color.green : Color.red,grid_size);
        grid_list.Add(new MazeGrid() { id = (i * max_columns + j), position = gridPosition, wall_break = break_Wall });

        if(break_Wall == WallBreak.RightWall){
          WallBuilder._WallBuilder.BuildTopWall(gridPosition,grid_size);
        }
        else if(break_Wall == WallBreak.TopWall){
          WallBuilder._WallBuilder.BuildRightWall(gridPosition, grid_size);
        }
        else{
          WallBuilder._WallBuilder.BuildTopWall(gridPosition, grid_size);
          WallBuilder._WallBuilder.BuildRightWall(gridPosition, grid_size);
        }

        if(j == 0)
          WallBuilder._WallBuilder.BuildBottomWall(gridPosition, grid_size);
        if(i == 0)
          WallBuilder._WallBuilder.BuildLeftWall(gridPosition, grid_size);

        
      }
    }
  }
  //邊界判斷(xy為該迷宮格子座標，rowcolumns為迷宮邊界)
  //
  WallBreak maze_bound(int x, int y,int row,int columns){
    if (isRigth_bound(x, row))//已達右邊邊界，只能打掉上牆
      if (isTop_bound(y, columns))
        return WallBreak.Non;
      else
      return WallBreak.TopWall;

    if(isTop_bound(y,columns))//已達上面邊界，只能打掉右牆
      return WallBreak.RightWall;

    return (WallBreak)RandomWall();//隨機上或是右牆
  }

  bool isRigth_bound(int x , int row_bound){
    return x + 1 == row_bound;
  }
  bool isTop_bound(int y, int column_bound){
    return y + 1 == column_bound;
  }
}
