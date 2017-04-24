using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace LD38Runner {
  class GameHUD : MonoBehaviour{
    public Text     timerText;
    public RawImage rightColorDisplay;
    public RawImage leftColorDisplay;

    public void Update() {
      Color[] playersColorArray = GameManager._instance.playersColorArray();
      int playersCurrentColor = GameManager._instance.playersCurrentColor();
      int leftColor = (playersCurrentColor - 1)%playersColorArray.Length;
      int rightColor = (playersCurrentColor + 1)%playersColorArray.Length;
      if (leftColor < 0) {
        leftColor = playersColorArray.Length - 1;
      }

      leftColorDisplay.color   = playersColorArray[leftColor];
      rightColorDisplay.color  = playersColorArray[rightColor];
      timerText.text = GameManager._instance.ElapsedTime.ToString("n2");
    }
  }
}
