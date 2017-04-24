using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD38Runner {
  public class DeathUI : MonoBehaviour {
    public Text durationText;
    public Text jumpCountText;
    public Text phaseText;

    public RectTransform OrangeBar;
    public RectTransform MagentaBar;
    public RectTransform BlueBar;

    public void Populate() {
      string jumpStr = GameManager._instance.timesJumped.ToString("n0");
      string phaseStr = GameManager._instance.phaseCounter.ToString("n0");
      string durationStr = GameManager._instance.deathTime.ToString("n2");

      float oTime = GameManager._instance.oTime;
      float mTime = GameManager._instance.mTime;
      float bTime = GameManager._instance.bTime;

      durationText.text = durationStr;
      jumpCountText.text = jumpStr;
      phaseText.text = phaseStr;

      float maxWidth = OrangeBar.transform.parent.GetComponent<RectTransform>().rect.width;
      float maxVal = Mathf.Max(oTime, mTime, bTime);

      OrangeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, oTime*maxWidth/maxVal);
      MagentaBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, mTime*maxWidth/maxVal);
      BlueBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bTime*maxWidth/maxVal);
    }

    // Update is called once per frame
    void Update() {
      if (Input.GetKeyDown(KeyCode.Space)) {
        GetComponentInChildren<RetryButton>().OnClick();
      } else if (Input.GetKeyDown(KeyCode.Q)) {
        GetComponentInChildren<QuitButton>().OnClick();
      }
    }
  }
}
