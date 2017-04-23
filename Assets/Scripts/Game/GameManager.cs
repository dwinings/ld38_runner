using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD38Runner {
  public class GameManager : MonoBehaviour {
    public static GameManager _instance = null;
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip deathSound;
    public LevelChunkList chunkList;

    // This one is off camera to the right (+x axis)
    private LevelChunk nextChunk;
    private LevelChunk currentChunk;
    public GameObject player;

    public float gravity;

    private float _levelSpeed = 15f;
    public float levelSpeed {
      get { return _levelSpeed; }
    }

    // Use this for initialization
    void Start() {
      musicAudioSource.Play();

      if (!_instance) {
        _instance = this;
      } else {
        GameObject.DestroyImmediate(this);
      }

      nextChunk = Instantiate(chunkList.levelChunks[0].GetComponent<LevelChunk>());
      player = GameObject.FindGameObjectWithTag("Player");
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
      yield return new WaitForSeconds(2);
      SceneManager.LoadScene(0);
    }

    public void endMePlease() {
      sfxAudioSource.PlayOneShot(deathSound);
      _levelSpeed = 0f;
      StartCoroutine(EventuallyChangeScene());

    }
  }
}
