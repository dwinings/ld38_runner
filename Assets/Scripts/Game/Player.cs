using System;
using UnityEngine;

namespace LD38Runner {
  [RequireComponent(typeof(BoxCollider2D))]
  public class Player : MonoBehaviour {
    public float jumpStrength;
    public float velocity;
    private ContactFilter2D filter;

    public float deathThreshold = -100f;
    public GameObject spriteHolder;

    private SpriteRenderer sr;
    public int currentColor = 0;
    public Color[] colorArray;

    public String[] colorMasks;


    private const float TERMINAL_VELOCITY = -40f;
    private bool isGrounded = false;
    private bool isCeilinged = false;
    private bool isWalled = false;

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
        } else if (velocity > 0 &&  floatEq(hit.point.y, hit.collider.bounds.min.y)) {
          isCeilinged = true;
          onCeiling(hit.collider.transform);
        } else if (hit.collider != underfoot && hit.collider != overhead) {
          isWalled = true;
          onWall(hit.transform);
        }
      }
    }

    private void onGrounded(Transform ground) {
      if (!floatEq(ground.position.y + 1, transform.position.y)) {
        transform.position = new Vector2(transform.position.x, ground.position.y+1);
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

      maybeJump();
      maybeChangeColor ();

      if (transform.position.y < deathThreshold) {
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
      currentColor++;
      updateSpriteAndCollisionLayer();
    }

    private void antiClockwiseColorChange() {
      currentColor--;
      if (currentColor < 0) {
        currentColor = colorArray.Length - 1;
      }
      updateSpriteAndCollisionLayer();
    }

    private void updateSpriteAndCollisionLayer() {
      sr.color = colorArray[currentColor % colorArray.Length];
      filter.layerMask = LayerMask.GetMask(colorMasks[currentColor % colorMasks.Length]);
    }

    private void die() {
      GameManager._instance.endMePlease();
      // Pls no destroy camera ty
      GetComponentInChildren<Camera>().gameObject.transform.SetParent(null, true);
      Destroy(this.gameObject);
    }

    private void maybeJump() {
      if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
        velocity += jumpStrength;
      }
    }

    private bool floatEq(float a, float b, float tolerance = 0.01f) {
      return Mathf.Abs(a - b) < tolerance;
    }
  }
}
