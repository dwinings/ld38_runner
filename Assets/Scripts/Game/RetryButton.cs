using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD38Runner {
  public class RetryButton : MonoBehaviour {
    public void OnClick() {
      SceneManager.LoadScene(1);
    }
  }
}
