using System;
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
    public AudioClip gameMusic;
    public LevelChunkList chunkList;

    // This one is off camera to the right (+x axis)
    [NonSerialized]
    public LevelChunk nextChunk;
    [NonSerialized]
    public LevelChunk currentChunk;
    private Player player;

    private float startTime;
    public int chunkIndex;
    public int introChunkAmnt;
    public float gravity;

    private float _levelSpeed = 15f;
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
    public int phaseCounter = 0;
    public float oTime     = 0;
    public float mTime     = 0;
    public float bTime     = 0;
    public float deathTime = 0;

      
    // Use this for initialization
    void Start() {
      musicAudioSource.Play();

      ActivateGameUI();

      if (!_instance) {
        _instance = this;
      } else {
        GameObject.DestroyImmediate(this);
      }

      nextChunk = Instantiate(selectNextChunk().GetComponent<LevelChunk>());
      player    = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
      nextChunk.GetComponent<LevelChunk>().PositionAsGameStart();

      spawnNextChunk(selectNextChunk());

      musicAudioSource.clip = gameMusic;
      musicAudioSource.time = 101.35f;
      musicAudioSource.loop = true;
      musicAudioSource.Play();
    }

    bool isCurrentChunkOffscreen() {
      return currentChunk.transform.position.x > 20;
    }

    LevelChunk selectNextChunk() {
      if (chunkIndex < introChunkAmnt) {
        LevelChunk nextChunk = chunkList.levelChunks[chunkIndex].GetComponent<LevelChunk> ();
        chunkIndex++;
        return nextChunk;       
      } else {
        return chunkList.levelChunks.Sample().GetComponent<LevelChunk>();
      }
    }

    void spawnNextChunk(LevelChunk toBeSpawned) {
      currentChunk = nextChunk;
      nextChunk = Instantiate(toBeSpawned);
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
      deathTime = ElapsedTime;
      ActivateDeathUI();
      _levelSpeed = 0f;
      sfxAudioSource.PlayOneShot(deathSound);
    }

    public void rebirth() {
    }

    public void ActivateGameUI() {
      // HACK
      startTime = Time.time;

      MainUI.SetActive(true);
      DeathUI.SetActive(false);
    }

    public void ActivateDeathUI() {
      MainUI.SetActive(false);
      DeathUI.SetActive(true);
      DeathUI.GetComponent<DeathUI>().Populate();
    }

    public float ElapsedTime {
      get {return Time.time - startTime;}
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
        bTime  += time;
        break;
      }
    }

    public void incPhaseCounter() {
      phaseCounter++;
    }
  }
}
