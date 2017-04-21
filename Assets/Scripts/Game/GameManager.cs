using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD38Runner {
  public class GameManager : MonoBehaviour {
    public static GameManager _instance = null;

    // Use this for initialization
    void Start() {
      if (!_instance) {
        _instance = this;
      } else {
        GameObject.DestroyImmediate(this);
      }
    }

    void Update() {
      //TODO: Write entire game.
    }
  }
}
