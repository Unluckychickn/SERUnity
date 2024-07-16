using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float JumpForce;
    [SerializeField] private Transform ScoreTransform;
    [SerializeField] private ObjectPooler CoinPooler;
    [SerializeField] private ObjectPooler ItemsPooler;
    [SerializeField] private List<string> collectableItems;
    [SerializeField] private float slowDownFactor;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip coinSound;

    private Rigidbody2D PlayerCharakter;
    private BoxCollider2D coll;

    private bool IsJumpPressed = false;
    private bool isGrounded = false;
    public int CheckDoubleJump = 0;
    public int MaxJump = 1;
    private float CoinSpeed = 20f;
    private Animator Anim;

    [SerializeField] private Score Score;
    private Transform CoinTransform;

    private void Start()
    {
        coll = GetComponent<BoxCollider2D>();
        PlayerCharakter = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();
        ItemAssets.Instance.OnSlowTimerTrigger += SlowTime;
    }

    private void OnDestroy()
    {
        ItemAssets.Instance.OnSlowTimerTrigger -= SlowTime;
        Time.timeScale = 1.0f;
    }

    private void Update()
    {
        if (PlayerCharakter.velocity.y == 0)
        {   ItemAssets.Instance.InvokeOnWalking();
            Anim.SetFloat("Move Y", 0);
        }
        if (PlayerCharakter.velocity.y > 0)
        {
            Anim.SetFloat("Move Y", 1f);
        }
        if (PlayerCharakter.velocity.y < 0)
        {
            Anim.SetFloat("Move Y", -1f);
        }
        UpdateIsGrounded();

        if (IsClickUIObject())
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && CheckDoubleJump <= MaxJump)
        {
            IsJumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        if (IsJumpPressed)
        {
            CheckDoubleJump++;
            ItemAssets.Instance.PlaySoundFXClip(jumpSound, transform, 1f);
            PlayerCharakter.velocity = new Vector2(PlayerCharakter.velocity.x, JumpForce);
            IsJumpPressed = false;
        }
    }

    private bool IsClickUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    private void UpdateIsGrounded()
    {
        int groundLayer = LayerMask.NameToLayer("Ground");
        Bounds colliderBounds = coll.bounds;
        float colliderRadius = coll.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);

        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.layer == groundLayer)
                {
                    isGrounded = true;
                    CheckDoubleJump = 0;
                    break;
                }
                else
                {
                    isGrounded = false;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D Collider)
    {
        if (collectableItems.Contains(Collider.transform.tag))
        {
            Collider.transform.parent = null;

            if (Collider.transform.tag == "Coin")
            {
                ItemAssets.Instance.PlaySoundFXClip(coinSound, this.transform, 0.85f);
                Collider.enabled = false;
                CoinTransform = Collider.transform;
                StartCoroutine(MoveCoinToScore(Collider.transform, Collider));
            }
            else
            {
                switch (Collider.transform.tag)
                {
                    case "SlowTime":
                        ItemAssets.Instance.InvokeOnSlowTimeTrigger();
                        break;
                    case "DoubleCoins":
                        ItemAssets.Instance.InvokeOnDoubleCoinsTrigger();
                        break;
                }
                ItemsPooler.ReturnToPool(Collider.transform.tag, Collider.transform.gameObject);
            }
        }
    }

    private void SlowTime(object sender, EventArgs e)
    {
        Item item = new Item(Item.ItemType.SlowTime, 1, true, false);
        Time.timeScale = slowDownFactor;
        StartCoroutine(ResetSlowTimeAfterDelay(item.duration));
    }

    private IEnumerator ResetSlowTimeAfterDelay(float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
    }

    private IEnumerator MoveCoinToScore(Transform coinTransform, Collider2D Collider)
    {
        Vector3 targetPosition = ScoreTransform.position;

        while (Vector3.Distance(coinTransform.position, targetPosition) > 0.5f)
        {
            Vector3 direction = (targetPosition - coinTransform.position).normalized;
            coinTransform.Translate(direction * CoinSpeed * Time.deltaTime);
            yield return null;
        }

        Score.UpdateScore();
        coinTransform.gameObject.SetActive(false);
        Collider.enabled = true;
        CoinPooler.ReturnToPool("Coin", coinTransform.gameObject);
    }
}
