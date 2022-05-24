using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBouncer : MonoBehaviour
{
    private BoxCollider2D flyCollider;
    private Rigidbody2D flyRigid;

    private float WallCooldown;
    private float GroundCooldown;

    [SerializeField] private float velX, velY;
    [SerializeField] private float speed = 1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    // Start is called before the first frame update
    void Start()
    {
        flyCollider = GetComponent<BoxCollider2D>();
        flyRigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (WallCooldown > 0.25f && isWall())
        {
            WallCooldown = 0;
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            velX *= -1;
        }
        if (GroundCooldown > 0.25f && isGroundedOrCeiling())
        {
            GroundCooldown = 0;
            velY = Random.Range(0.2f,0.4f) * -Mathf.Sign(velY);
        }
        flyRigid.velocity = new Vector2(velX, velY) * speed * Time.deltaTime;
        WallCooldown += Time.deltaTime;
        GroundCooldown += Time.deltaTime;
    }

    private bool isGroundedOrCeiling()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(flyCollider.bounds.center, flyCollider.bounds.size, 0, new Vector2(0, transform.localScale.x), 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool isWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(flyCollider.bounds.center, flyCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
