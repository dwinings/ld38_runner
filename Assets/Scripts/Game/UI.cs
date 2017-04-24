using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LD38Runner {
  public class UI : MonoBehaviour {
    public GameObject LeftColorBlock;
    public GameObject TimeText;
    public GameObject RightColorBlock;

    public GameObject LengthData;
    public GameObject JumpData;

    public GameObject OrangeBar;
    public GameObject MagentaBar;
    public GameObject BlueBar;

    private Text     Elapsed;
    private RawImage Right;
    private RawImage Left;

    private Text Length;
    private Text Jumped;

    // Use this for initialization
    void Start () {
      Left     = LeftColorBlock.GetComponent<RawImage>();
      Elapsed  = TimeText.GetComponent<Text>();
      Right    = RightColorBlock.GetComponent<RawImage>();

      Length = LengthData.GetComponent<Text>();
      Jumped = JumpData.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update () {
      Color[] playersColorArray = GameManager._instance.playersColorArray();
      int playersCurrentColor   = GameManager._instance.playersCurrentColor();
      int leftColor  = (playersCurrentColor - 1) % playersColorArray.Length;
      int rightColor = (playersCurrentColor + 1) % playersColorArray.Length;
      if (leftColor < 0) { leftColor = playersColorArray.Length - 1; }

      string elapsed      = GameManager._instance.elapsedTime().ToString("n2");
      string amountJumped = GameManager._instance.timesJumped.ToString("n");
      string deathTime    = GameManager._instance.deathTime.ToString("n");

      float O = GameManager._instance.oTime;
      float M = GameManager._instance.mTime;
      float B = GameManager._instance.bTime;

      Elapsed.text = elapsed;
      Length.text  = deathTime;

      Jumped.text  = amountJumped;
        
      Left.color   = playersColorArray[leftColor];
      Right.color  = playersColorArray[rightColor];


      OrangeBar.transform.localScale  = new Vector2(O * 0.2f, 1f);
      MagentaBar.transform.localScale = new Vector2(M * 0.2f, 1f);
      BlueBar.transform.localScale    = new Vector2(B * 0.2f, 1f);
    }
  }
}