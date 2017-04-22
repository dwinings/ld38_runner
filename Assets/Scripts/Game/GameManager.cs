using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD38Runner {
  public class GameManager : MonoBehaviour {
    public static GameManager _instance = null;
    public AudioSource musicAudioSource;
    public AudioSource sfxAudioSource;
    public AudioClip deathSound;
    public LevelChunkList chunkList;
    public GameObject player;

    public float gravity = 0.05f;

    private float _levelSpeed = 0.2f;
    public float levelSpeed {
      get { return _levelSpeed; }
    }

    // Use this for initialization
    void Start() {
      if (!_instance) {
        _instance = this;
      } else {
        GameObject.DestroyImmediate(this);
      }

      var chunk = Instantiate(chunkList.levelChunks[0]);
      player = GameObject.FindGameObjectWithTag("Player");
      chunk.GetComponent<LevelChunk>().PositionAsGameStart();
    }

    void Update() {
      //TODO: Write entire game.
    }

    public void endMePlease() {
      sfxAudioSource.PlayOneShot(deathSound);
      _levelSpeed = 0f;
    }
  }
}
