using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD38Runner {
  public class CreditsScroller : MonoBehaviour {
    public float scrollSpeed;

    void Update() {
      var rect = GetComponent<RectTransform>();
      rect.Translate(0, scrollSpeed*Time.deltaTime, 0);

      if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q) || Mathf.Abs(transform.position.y) > rect.rect.height) {
        SceneManager.LoadScene(0); 
      }
    }
  }
}
