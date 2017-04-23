using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD38Runner {
  public class UI : MonoBehaviour {
    public GameObject LeftColorBlock;
    public GameObject TimeText;
    public GameObject RightColorBlock;

    private Text text;
    private RawImage Right;
    private RawImage Left;

    // Use this for initialization
    void Start () {
      Left  = LeftColorBlock.GetComponent<RawImage>();
      text  = TimeText.GetComponent<Text> ();
      Right = RightColorBlock.GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update () {
      Color[] playersColorArray = GameManager._instance.playersColorArray();
      int playersCurrentColor   = GameManager._instance.playersCurrentColor();
      int leftColor  = (playersCurrentColor - 1) % playersColorArray.Length;
      int rightColor = (playersCurrentColor + 1) % playersColorArray.Length;
      if (leftColor < 0) { leftColor = playersColorArray.Length - 1; }
      string elapsed = GameManager._instance.elapsedTime ().ToString ("n2");

      text.text   = elapsed;
      Left.color  = playersColorArray[leftColor];
      Right.color = playersColorArray[rightColor];
    }
  }
}