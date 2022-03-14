using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = "OnChangeHealth")]
    [SerializeField] int currentHealth = 100;
    [SerializeField] int maxHealth = 100;

    [SerializeField] bool isDead = false;

    public MenuCamScript _menuCamera;

    public Image _healthBar;

    public GameObject bodyObject;
    public GameObject fpBodyObject;
    public GameObject tpBodyObject;

    // Start is called before the first frame update
    void Start()
    {
        if (_menuCamera == null)
        {
            _menuCamera = GameObject.FindObjectOfType<MenuCamScript>();
        }

        if (_menuCamera != null)
        {
            _menuCamera.Disable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        //if press f2, heal, if press f3, damage, if press f4, die
        if (Keyboard.current.f2Key.wasPressedThisFrame)
        {
            Heal(10);
        }

        if (Keyboard.current.f3Key.wasPressedThisFrame)
        {
            TakeDamage(10);
        }

        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            Die();
        }

        //if dead and press f5, respawn
        if (isDead && Keyboard.current.f5Key.wasPressedThisFrame)
        {
            Respawn();
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (!isLocalPlayer) return;

        _healthBar.rectTransform.sizeDelta = new Vector2(((float)currentHealth) / ((float)maxHealth) * 400.0f, _healthBar.rectTransform.sizeDelta.y);
    }

    void OnChangeHealth(int oldHealth, int newHealth)
    {
        currentHealth = newHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    
    //DAMAGE FUNCTIONS
    [ClientRpc]
    private void RpcTakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [Command]
    private void CmdTakeDamage(int damage)
    {
        RpcTakeDamage(damage);
    }

    public void TakeDamage(int amount){
        if (isServer) {
            RpcTakeDamage(amount);
        }
        else if (hasAuthority) {
            CmdTakeDamage(amount);
        }
        else{
            Debug.LogError("somfing wrong here");
        }
    }



    //HEAL FUNCTIONS
    [ClientRpc]
    private void RpcHeal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    [Command]
    private void CmdHeal(int amount)
    {
        RpcHeal(amount);
    }

    public void Heal(int amount)
    {
        if (isServer)
        {
            RpcHeal(amount);
        }
        else if (hasAuthority)
        {
            CmdHeal(amount);
        }
        else
        {
            Debug.LogError("somfing wrong here");
        }
    }


    //DEATH FUNCTIONS
    [ClientRpc]
    private void RpcDie()
    {
        isDead = true;
        
        if (isLocalPlayer && _menuCamera) {
            _menuCamera.Enable();
        }

        GetComponentInChildren<PlayerMovement>().enabled = false;
        GetComponentInChildren<HitscanShoot>().enabled = false;
        bodyObject.SetActive(false);
        fpBodyObject.SetActive(false);
        tpBodyObject.SetActive(false);
    }

    [Command]
    private void CmdDie()
    {
        RpcDie();
    }

    void Die()
    {
        if (isServer)
        {
            RpcDie();
        }
        else if (hasAuthority)
        {
            CmdDie();
        }
        else
        {
            Debug.LogError("somfing wrong here");
        }
    }



    //RESPAWN FUNCTIONS
    [ClientRpc]
    private void RpcRespawn()
    {
        isDead = false;

        GetComponentInChildren<PlayerMovement>().enabled = true;
        GetComponentInChildren<HitscanShoot>().enabled = true;
        bodyObject.SetActive(true);
        fpBodyObject.SetActive(true);
        tpBodyObject.SetActive(true);

        transform.position = new Vector3(0, 0, 0);

        if (isLocalPlayer && _menuCamera) {
            _menuCamera.Disable();
        }
    }

    [Command]
    private void CmdRespawn()
    {
        RpcRespawn();
    }

    public void Respawn()
    {
        if (isServer)
        {
            RpcRespawn();
        }
        else if (hasAuthority)
        {
            CmdRespawn();
        }
        else
        {
            Debug.LogError("somfing wrong here");
        }
    }
}
