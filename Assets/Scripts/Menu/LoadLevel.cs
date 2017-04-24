using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD38Runner {
  public class LoadLevel : MonoBehaviour {
    public void LoadLevel1() {
      SceneManager.LoadScene(1);
    }

    public void LoadLevel2() {
      SceneManager.LoadScene(2);
    }
  }
}