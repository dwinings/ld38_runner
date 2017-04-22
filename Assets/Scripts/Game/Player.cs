using System;
using UnityEngine;

namespace LD38Runner {
  [RequireComponent(typeof(BoxCollider2D))]
  public class Player : MonoBehaviour {
    public float jumpStrength = 3.5f;
    private float velocity = 0f;
    private BoxCollider2D coll;
    private BoxCollider2D[] overlapping = new BoxCollider2D[16];
    private ContactFilter2D filter = new ContactFilter2D();

    public float deathThreshold = -100f;
    private const float NEG_THREE_PI_OVER_4 = -3f * Mathf.PI / 4f;
    private const float NEG_PI_OVER_4 = Mathf.PI / -4f;
    private const float THREE_PI_OVER_4 = 3f * Mathf.PI / 4f;
    private const float PI_OVER_4 = Mathf.PI / 4f;

    bool isGrounded() {
      // TODO: Disable flying...
      int hits = coll.OverlapCollider(filter, overlapping);
      for (int i = 0; i < hits; i++) {
        Vector2 incidence = (overlapping[i].transform.position - transform.position).normalized;
        var angle = Math.Atan2(incidence.y, incidence.x);
        if (angle > NEG_THREE_PI_OVER_4 && angle < NEG_PI_OVER_4 ) {
          if (Mathf.Abs(transform.position.y - overlapping[i].transform.position.y) > 0.0001f) {
            transform.position = new Vector2(transform.position.x, overlapping[i].transform.position.y + 10);
          }
          return true;
        }
      }
      return false;
    }

    // Use this for initialization
    void Start() {
      coll = GetComponent<BoxCollider2D>();
      filter.layerMask = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
    void Update() {

      if (isGrounded()) {
        velocity = 0;
        MaybeJump();
      } else {
        velocity -= GameManager._instance.gravity;
      }
      transform.Translate(0, velocity, 0);

      if (transform.position.y < deathThreshold) {
        GameManager._instance.endMePlease();
        Destroy(this.gameObject);
      }
    }

    void MaybeJump() {
      if (isGrounded() && Input.GetKeyDown(KeyCode.Space)) {
        velocity += jumpStrength;
      }
    }
  }
}
