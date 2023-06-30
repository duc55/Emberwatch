using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [HideInInspector]
    public int id;

    [Header("Info")]
    public float moveSpeed;
    public int gold;
    public int curHp;
    public int maxHp;
    public bool dead;

    [Header("Components")]
    public Rigidbody2D body;
    public Player photonPlayer;
    public SpriteRenderer sprRenderer;
    public HeaderInfo headerInfo;

    //local player
    public static PlayerController me;

    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        // add self to players array
        GameManager.instance.players[id - 1] = this;

        //initialize health bar
        headerInfo.Initialize(player.NickName, maxHp);

        if (player.IsLocal) {
            me = this;
            GameManager.instance.cinemachineVirtualCamera.Follow = transform;
        } else {
            body.isKinematic = true;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) {
            return;
        }

        Move();
    }

    void Move()
    {
        //get horizontal and vertical inputs
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        //apply to our velocity
        body.velocity = new Vector2(x, y) * moveSpeed;
    }

    [PunRPC]
    public void TakeDamage(int damage) {
        curHp -= damage;

        //update health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);

        if (curHp < 0) {
            Die();
        } else {
            photonView.RPC("FlashDamage", RpcTarget.All);
        }
    }

    [PunRPC]
    public void FlashDamage()
    {
        StartCoroutine(DamageFlash());

        IEnumerator DamageFlash()
        {
            sprRenderer.color = Color.red;
            yield return new WaitForSeconds(0.05f);
            sprRenderer.color = Color.white;
        }       
    }

    void Die()
    {
        dead = true;
        body.isKinematic = true;

        transform.position = new Vector3(0, 99, 0);

        Vector3 spawnPos = GameManager.instance.spawnPoints[Random.Range(0, GameManager.instance.spawnPoints.Length)].position;
        StartCoroutine(Spawn(spawnPos, GameManager.instance.respawnTime));
    }

    IEnumerator Spawn(Vector3 spawnPos, float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        dead = false;
        transform.position = spawnPos;
        curHp = maxHp;
        body.isKinematic = false;

        //update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void Heal(int amountToHeal)
    {
        curHp = Mathf.Clamp(curHp + amountToHeal, 0, maxHp);

        //update the health bar
        headerInfo.photonView.RPC("UpdateHealthBar", RpcTarget.All, curHp);
    }

    [PunRPC]
    void GiveGold(int goldToGive)
    {
        gold += goldToGive;

        //update the ui
        GameUi.instance.UpdateGoldText(gold);
    }
}
