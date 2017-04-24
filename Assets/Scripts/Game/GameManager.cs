using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD38Runner {
  public class GameManager : MonoBehaviour {
    public static GameManager _instance = null;
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip   deathSound;
    public LevelChunkList chunkList;

    // This one is off camera to the right (+x axis)
    private LevelChunk nextChunk;
    private LevelChunk currentChunk;
    private Player player;

    public float gravity;

    private float _levelSpeed = 8f;
    public  float levelSpeed {
      get { return _levelSpeed; }
    }

    private GameObject mainUI;
    public GameObject MainUI {
      get { 
        if (mainUI == null) {
          mainUI = GameObject.FindGameObjectWithTag ("MainUI");
        }
        return mainUI;
      }
    }

    private GameObject deathUI;
    public GameObject DeathUI {
      get { 
        if (deathUI == null) {
          deathUI = GameObject.FindGameObjectWithTag ("DeathUI");
        }
        return deathUI;
      }
    }

    //Stats Variables;
    public int timesJumped = 0;
    public float oTime     = 0;
    public float mTime     = 0;
    public float bTime     = 0;
    public float deathTime = 0;

      
    // Use this for initialization
    void Start() {
      musicAudioSource.Play();

      mainUIScale ();

      if (!_instance) {
        _instance = this;
      } else {
        GameObject.DestroyImmediate(this);
      }

      nextChunk = Instantiate(chunkList.levelChunks[0].GetComponent<LevelChunk>());
      player    = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
      nextChunk.GetComponent<LevelChunk>().PositionAsGameStart();

      spawnNextChunk(selectNextChunk());
    }

    bool isCurrentChunkOffscreen() {
      return currentChunk.transform.position.x > 20;
    }

    LevelChunk selectNextChunk() {
      return chunkList.levelChunks.Sample().GetComponent<LevelChunk>();
    }

    void spawnNextChunk(LevelChunk toBeSpawned) {
      currentChunk = nextChunk;
      nextChunk = Instantiate(nextChunk);
      nextChunk.PositionAsNewChunk(currentChunk.transform.position.x, currentChunk.end_height);
    }

    void FixedUpdate() {
      if (isCurrentChunkOffscreen()) {
        Destroy(currentChunk.gameObject);
        spawnNextChunk(selectNextChunk());
      }
    }

    IEnumerator EventuallyChangeScene() {
      yield return new WaitForSeconds(5);
      SceneManager.LoadScene(0);
    }

    public void endMePlease() {
      deathTime = Time.time;
      deathUIScale();
      _levelSpeed = 0f;
      sfxAudioSource.PlayOneShot(deathSound);
      StartCoroutine(EventuallyChangeScene());

    }

    public void mainUIScale() {
      MainUI.transform.localScale  = new Vector3( 1f, 1f, 1f);
      DeathUI.transform.localScale = new Vector3( 0f, 1f, 1f);
    }

    public void deathUIScale() {
      MainUI.transform.localScale  = new Vector3( 0f, 1f, 1f);
      DeathUI.transform.localScale = new Vector3( 1f, 1f, 1f);
    }

    public float elapsedTime() {
      return Time.time;
    }

    public int playersCurrentColor() {
      return player.currentColor;
    }

    public Color[] playersColorArray() {
      return player.colorArray;
    }

    public void increaseJumpCounter() {
      timesJumped++;
    }

    public void increaseColorTimer(int colorInt, float time) {
      switch (colorInt) {
      case 0:
        oTime  += time;
        break;
      case 1:
        mTime  += time;
        break;
      case 2:
        mTime  += time;
        break;
      }
    }
  }
}
