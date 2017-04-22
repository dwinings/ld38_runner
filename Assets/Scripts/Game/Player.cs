using System;
using UnityEngine;

namespace LD38Runner {
  [RequireComponent(typeof(BoxCollider2D))]
  public class Player : MonoBehaviour {
    public float jumpStrength;
    public float velocity = 0f;
    private BoxCollider2D coll;
    private BoxCollider2D[] overlapping = new BoxCollider2D[16];
    private ContactFilter2D filter = new ContactFilter2D();

    public float deathThreshold = -100f;
    public GameObject spriteHolder;

    private const float NEG_THREE_PI_OVER_4 = -3f * Mathf.PI / 4f;
    private const float NEG_PI_OVER_4 = Mathf.PI / -4f;
    private const float THREE_PI_OVER_4 = 3f * Mathf.PI / 4f;
    private const float PI_OVER_4 = Mathf.PI / 4f;
    private bool isGrounded = false;
    private bool isCeilinged = false;
    private bool isWalled = false;

    private void performCollision() {
      int hits = coll.OverlapCollider(filter, overlapping);
      Vector2 incidence;
      double angle;
      for (int i = 0; i < hits; i++) {
        incidence = (overlapping[i].transform.position - transform.position).normalized;
        angle = Math.Atan2(incidence.y, incidence.x);
        if (!isGrounded && floorBounds(angle)) {
          isGrounded = true;
          onGrounded(overlapping[i].transform);
        } else if (!isCeilinged && ceilBounds(angle)) {
          isCeilinged = true;
          onCeiling(overlapping[i].transform);
        }
      }

      // We check for wall collisions like this last, so that the ceiling/ground snapping will 
      // happen before we check angles that can kill the player.

      for (int i = 0; i < hits; i++) {
        incidence = (overlapping[i].transform.position - transform.position).normalized;
        angle = Math.Atan2(incidence.y, incidence.x);
        if (!isWalled && wallBounds(angle)) {
          isWalled = true;
          onWall(overlapping[i].transform);
          break;
        }
      }
    }

    private bool floorBounds(double angle) {
      return (angle > NEG_THREE_PI_OVER_4 && angle < NEG_PI_OVER_4);
    }

    private bool ceilBounds(double angle) {
      return (angle < THREE_PI_OVER_4 && angle > PI_OVER_4);
    }

    private bool wallBounds(double angle) {
      return (angle < NEG_THREE_PI_OVER_4 - 0.05f || angle > THREE_PI_OVER_4 + 0.05f);
    }

    private void onGrounded(Transform ground) {
      if (Mathf.Abs(transform.position.y - ground.position.y) > 0.0001f) {
        transform.position = new Vector2(transform.position.x, ground.position.y + 1);
      }
      velocity = 0;
      maybeJump();
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
      coll = GetComponent<BoxCollider2D>();
      filter.layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    public void Update() {
      isGrounded = false;
      isCeilinged = false;
      isWalled = false;
      rotateLeft ();

      velocity -= GameManager._instance.gravity;
      velocity = Mathf.Clamp(velocity, -1, 9999);
      performCollision();
      transform.Translate(0, velocity, 0);

      if (transform.position.y < deathThreshold) {
        die();
      }
    }

    private void die() {
      GameManager._instance.endMePlease();
      // Pls no destroy camera ty
      GetComponentInChildren<Camera>().gameObject.transform.SetParent(null, true);
      Destroy(this.gameObject);
    }

    private void maybeJump() {
      if (Input.GetKeyDown(KeyCode.Space)) {
        velocity += jumpStrength;
      }
    }

    void rotateLeft() {
      spriteHolder.transform.Rotate (Vector3.forward * 5);
    }
  }
}
