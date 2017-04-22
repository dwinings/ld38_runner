using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD38Runner {
  public class LevelChunk : MonoBehaviour {

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
      transform.Translate(GameManager._instance.levelSpeed, 0f, 0f);
    }
  }
}
