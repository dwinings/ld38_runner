using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD38Runner {
  public class QuitButton : MonoBehaviour {
    public void OnClick() {
      SceneManager.LoadScene(0);
    }
  }
}
