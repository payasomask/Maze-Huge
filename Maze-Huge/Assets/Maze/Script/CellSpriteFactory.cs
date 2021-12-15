using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSpriteFactory
{
  void SetFloorSpirte(Cell c) {
    c.floorSpriteName = "maze_floor";
  }
  void SetWallSpirte(Cell c) {
    //規則上下左右是否有牆，有牆=1
    string wallTag = WallBool2int(c.TopWall) + WallBool2int(c.BottomWall) + WallBool2int(c.LeftWall) + WallBool2int(c.RightWall);
    c.wallSpriteName = "maze_wall_" + wallTag;
  }

  public void ProcessCell(Cell c) {
    SetFloorSpirte(c);
    SetWallSpirte(c);
  }

  string WallBool2int(bool wall) {
    return Convert.ToInt32(wall).ToString();
  }
}
