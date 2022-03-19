using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

public class PlayerHealth : NetworkBehaviour
{
    [Serializable]
    public struct Damage {     
        public int m_damageAmount;
        public string m_damageName;
        public float m_time;
        public float m_range;
        public Vector3 m_force;
        public Vector3 m_origin;
        public Vector3 m_direction;
        public Vector3 m_hitPoint;
        public Vector3 m_hitNormal;
        public NetworkIdentity m_originNetId;

        public Damage(int _damageAmount = 0, string _damageName = "", float _time = 0, float _range = 0, Vector3 _force = new Vector3(), Vector3 _origin = new Vector3(), Vector3 _direction = new Vector3(), Vector3 _hitPoint = new Vector3(), Vector3 _hitNormal = new Vector3(), NetworkIdentity _originNetId = null) {
            m_damageAmount = _damageAmount;
            m_damageName = _damageName;
            m_time = _time;
            m_range = _range;
            m_force = _force;
            m_origin = _origin;
            m_direction = _direction;
            m_hitPoint = _hitPoint;
            m_hitNormal = _hitNormal;
            m_originNetId = _originNetId;
        }
    }

    [SyncVar] [SerializeField] int currentHealth = 100;
    [SyncVar] [SerializeField] int maxHealth = 100;
    
    [SyncVar] [SerializeField] bool isDead = false;

    public MenuCamScript _menuCamera;

    public Image _healthBar;
    public GameObject bodyObject;
    public GameObject fpBodyObject;
    public GameObject tpBodyObject;

    public List<Damage> damageLog = new List<Damage>();

    // Start is called before the first frame update
    void Start()
    {
        if (_menuCamera == null)
        {
            _menuCamera = GameObject.FindObjectOfType<MenuCamScript>();
        }

        if (isLocalPlayer && _menuCamera != null)
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
            TakeDamage(new Damage(10, "pure willpower", _originNetId: GetComponent<NetworkIdentity>()));
        }

        if (Keyboard.current.f4Key.wasPressedThisFrame)
        {
            TakeDamage(new Damage(int.MaxValue, "pure willpower", _originNetId: GetComponent<NetworkIdentity>()));
        }

        //if dead and press f5, respawn
        if (isDead && Keyboard.current.f5Key.wasPressedThisFrame)
        {
            CmdRespawn();
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
    }


    
    //DAMAGE FUNCTIONS
    [ClientRpc]
    private void RpcTakeDamage(Damage _damage)
    {
        TakeDamage(_damage);
    }

    [Command(requiresAuthority = false)]
    public void CmdTakeDamage(Damage _damage)
    {
        RpcTakeDamage(_damage);
    }

    private void TakeDamage(Damage _damage){
        if (isDead) return;

        damageLog.Add(_damage);

        currentHealth -= _damage.m_damageAmount;

        if (isLocalPlayer && currentHealth <= 0)
        {
            CmdDie();
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
            Debug.LogWarning("Something isnt quite right!");
        }
    }


    //DEATH FUNCTIONS
    [ClientRpc]
    private void RpcDie()
    {
        Die();
    }

    [Command]
    private void CmdDie()
    {
        RpcDie();

        if (isServerOnly) Die();
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        transform.position = new Vector3(0, 0, 0);
        
        if (isLocalPlayer) {
            if (_menuCamera) _menuCamera.Enable();
            //unclock mouse
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            PlayerStats ps = GetComponent<PlayerStats>();
            Damage lastDamage = new Damage();
            string lastDamageSourceName = "someone";
            string lastDamageName = "something";
            if (damageLog.Count > 0)
            {
                lastDamage = damageLog[damageLog.Count - 1];
                lastDamageName = lastDamage.m_damageName;
                if (lastDamage.m_originNetId != null && lastDamage.m_originNetId.GetComponent<PlayerStats>())
                {
                    lastDamageSourceName = lastDamage.m_originNetId.GetComponent<PlayerStats>().username;
                }
            }

            //death event
            FindObjectOfType<EventLogger>().CmdLogEvent(ps.username + " died to " + lastDamageSourceName + " with " + lastDamageName + "!");
        }

        GetComponentInChildren<PlayerMovement>().enabled = false;
        GetComponentInChildren<HitscanShoot>().enabled = false;
        bodyObject.SetActive(false);
        fpBodyObject.SetActive(false);
        tpBodyObject.SetActive(false);
    }



    //RESPAWN FUNCTIONS
    [ClientRpc]
    private void RpcRespawn()
    {
        Respawn();
    }

    [Command]
    private void CmdRespawn()
    {
        RpcRespawn();

        if (isServerOnly) Respawn();
    }

    private void Respawn()
    {
        isDead = false;
        currentHealth = maxHealth;

        GetComponentInChildren<PlayerMovement>().enabled = true;
        GetComponentInChildren<HitscanShoot>().enabled = true;
        bodyObject.SetActive(true);
        fpBodyObject.SetActive(true);
        tpBodyObject.SetActive(true);

        if (isLocalPlayer && _menuCamera) {
            _menuCamera.Disable();
            //lock mouse
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
