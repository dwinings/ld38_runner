﻿using System;
using UnityEngine;

namespace LD38Runner {
  [RequireComponent(typeof(BoxCollider2D))]
  public class Player : MonoBehaviour {
    public float jumpStrength;
    public float velocity;
    public float rotationSpeed;
    private ContactFilter2D filter;

    public GameObject spriteHolder;

    private SpriteRenderer sr;
    public int currentColor = 0;
    public Color[] colorArray;

    public String[] colorMasks;

    private float lastColorSwitchTime;
    private const float TERMINAL_VELOCITY = -40f;
    private bool isGrounded = false;
    private bool isCeilinged = false;
    private bool isWalled = false;

    public AudioClip jumpSound;
    public AudioClip landSound;


    private void performCollision() {
      float deltaX = GameManager._instance.levelSpeed*Time.deltaTime;
      float deltaY = velocity*Time.deltaTime;
      Vector2 trajectory = new Vector2(deltaX, deltaY);

      // We can't die to these colliders.
      var underfoot = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, filter.layerMask);
      var overhead = Physics2D.Raycast(transform.position, Vector2.up, 0.6f, filter.layerMask);

      // This kills us if it's not at foot/head height and is not what we're standing on.
      var hit = Physics2D.Raycast(transform.position - new Vector3(0.5f, 0.5f, 0f), trajectory, trajectory.magnitude, filter.layerMask);

      if (hit.collider != null) {
        if (velocity < 0 && floatEq(hit.point.y, hit.collider.bounds.max.y)) {
          isGrounded = true;
          onGrounded(hit.collider.transform);
        } else if (velocity > 0 && floatEq(hit.point.y, hit.collider.bounds.min.y)) {
          isCeilinged = true;
          onCeiling(hit.collider.transform);
        }
      }

      var adjustedTraj = new Vector2(deltaX, velocity * Time.deltaTime);
      var adjHit = Physics2D.Raycast(transform.position - new Vector3(0.4999f, 0.4999f, 0f), adjustedTraj, adjustedTraj.magnitude, filter.layerMask);
      if (adjHit.collider != null 
        && (underfoot.collider == null || adjHit.collider.gameObject != underfoot.collider.gameObject)
        && (overhead.collider  == null || adjHit.collider.gameObject !=  overhead.collider.gameObject)) 
      {
        isWalled = true;
        onWall(hit.transform);
      }

      // HACK
      if (underfoot.collider && underfoot.collider.gameObject.GetComponent<Spike>()) {
        die();
      }
    }

    private void onGrounded(Transform ground) {
      if (!floatEq(ground.position.y + 1, transform.position.y)) {
        transform.position = new Vector2(transform.position.x, ground.position.y+1);
      }
      if (velocity < -5) {
        //GameManager._instance.sfxAudioSource.PlayOneShot(landSound); 
      }
      velocity = 0;
    }

    private void onCeiling(Transform ceiling) {
      if (Mathf.Abs(transform.position.y + 1f - ceiling.position.y) > 0.0001f) {
        transform.position = new Vector2(transform.position.x, ceiling.position.y - 1);
      }
      velocity = Mathf.Min(velocity, 0);
    }

    private void onWall(Transform wall) {
      die();
    }

    // Use this for initialization
    public void Start() {
      lastColorSwitchTime = (float)Time.time;
      sr = spriteHolder.GetComponent<SpriteRenderer> ();
      updateSpriteAndCollisionLayer();
    }

    // Update is called once per frame
    public void Update() {
      isGrounded  = false;
      isCeilinged = false;
      isWalled    = false;

      velocity -= GameManager._instance.gravity * Time.deltaTime;
      velocity  = Mathf.Clamp(velocity, TERMINAL_VELOCITY, 100);

      performCollision();
      transform.Translate(0, velocity * Time.deltaTime, 0);
      GetComponentInChildren<SpriteRenderer>().transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

      maybeJump();
      maybeChangeColor ();

      if (transform.position.y < GameManager._instance.currentChunk.start_height - GameManager._instance.currentChunk.height - 10f) {
        die();
      }

    }

    private void maybeChangeColor() {
      if (Input.GetKeyDown(KeyCode.A)) {
        antiClockwiseColorChange ();
      } else if (Input.GetKeyDown(KeyCode.D)) {
        clockwiseColorChange ();
      }
    }

    private void clockwiseColorChange() {
      updateTimeAsColor(currentColor % colorArray.Length);
      currentColor++;
      updateSpriteAndCollisionLayer();
      GameManager._instance.incPhaseCounter();
    }

    private void antiClockwiseColorChange() {
      updateTimeAsColor(currentColor % colorArray.Length);
      currentColor--;
      if (currentColor < 0) {
        currentColor = colorArray.Length - 1;
      }
      updateSpriteAndCollisionLayer();
      GameManager._instance.incPhaseCounter();
    }

    private void updateTimeAsColor(int currentColor) {
      float now = (float)Time.time;
      GameManager._instance.increaseColorTimer(currentColor, now - lastColorSwitchTime);
      lastColorSwitchTime = now;
    }

    private void updateSpriteAndCollisionLayer() {
      var party = GetComponentInChildren<ParticleSystem>();
      var main = party.main;
      main.startColor = colorArray[currentColor%colorArray.Length];
    
      // sr.color = colorArray[currentColor % colorArray.Length];
      filter.layerMask = LayerMask.GetMask(colorMasks[currentColor % colorMasks.Length]);
    }

    private void die() {
      updateTimeAsColor(currentColor % colorArray.Length);
      GameManager._instance.endMePlease();
      // Pls no destroy camera ty
      var cam = GetComponentInChildren<Camera>();
      cam.gameObject.transform.SetParent(null, true);
      // pce out thx
      cam.transform.position = new Vector3(999f, 999f, -10f);
      Destroy(this.gameObject);
    }

    private void maybeJump() {
      if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
        GameManager._instance.increaseJumpCounter();
        GameManager._instance.sfxAudioSource.PlayOneShot(jumpSound);
        velocity += jumpStrength;
      }
    }

    private bool floatEq(float a, float b, float tolerance = 0.01f) {
      return Mathf.Abs(a - b) < tolerance;
    }
  }
}
