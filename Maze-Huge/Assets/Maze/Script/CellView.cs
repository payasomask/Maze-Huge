using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellView : MonoBehaviour
{
  [SerializeField]
  SpriteRenderer floor_sr;
  [SerializeField]
  SpriteRenderer wall_sr;
  public void setView(Sprite floor, Sprite wall) {
    floor_sr.sprite = floor;
    wall_sr.sprite = wall;
  }
}
