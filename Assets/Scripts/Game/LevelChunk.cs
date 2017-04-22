using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD38Runner {
  public class LevelChunk : MonoBehaviour {
    public float start_height;
    public float end_height;
    public float starting_x;
    public float height;
    public float width;

    public const int PLAYER_DISPLAY_OFFSET = 5;

    public void PositionAsGameStart() {
      transform.position = new Vector3(
        -1*width + PLAYER_DISPLAY_OFFSET + starting_x,
        height - start_height + 2,
        0f
      );
    }

    // Update is called once per frame
    public void Update() {
      transform.Translate(GameManager._instance.levelSpeed, 0f, 0f);
    }
  }
}
