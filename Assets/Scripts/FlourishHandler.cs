using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Mirror;


public class FlourishHandler : NetworkBehaviour
{
    public GameObject flourishIndicator;
    public Animator reloadAnimator;
    public InputAction initFlourish;
    public InputAction flourishControl;

    public InputAction normalReloadControl;
    Equipment equip;

    public int currentMoveIndex = 0;
    public Flourish currentFlourish;

    public float moveAmountReqiured = 0.5f;

    public float FOV = 50.0f, FlourishFOV = 60.0f;

    float currentMoveAmount = 0.0f;
    bool isFlourishing = false;
    bool isReloading = false;
    float reloadTimer = 0.0f;

    MouseLook mouselook;

    Camera mainCamera;
    public Volume volume;

    public float chromaticAberrationIntensity = 0.0f;

    HitscanShoot hitscanShoot;

    float flourishTimer = 0.0f;
    bool flourishActive = false;
    
    

    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer) return;
        initFlourish.Enable();
        initFlourish.started += OnInitFlourish;
        initFlourish.canceled += OnCancelFlourish;
        normalReloadControl.Enable();
        normalReloadControl.started += BeginReload;
        flourishControl.performed += OnMouseMove;
        mouselook = GetComponentInChildren<MouseLook>();
        mainCamera = Camera.main;
        equip = GetComponent<Equipment>();
        hitscanShoot = GetComponent<HitscanShoot>();
    }

    void BeginReload(InputAction.CallbackContext ctx)
    {
        if(!isLocalPlayer) return;
        if(equip.currentWeapon == null) return;
        
        equip.SetTrigger(new string[]{"r"}, equip.currentWeapon);
        equip.currentWeaponObject.GetComponent<WeaponFX>().PlayReload();
        isReloading = true;
        hitscanShoot.canFire = false;
        
    }

    float GetCurrMovePercentage()
    {
        return (currentMoveAmount / moveAmountReqiured) * 300.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer) return;
        if(equip && isReloading)
        {
            reloadTimer += Time.deltaTime;
            if(reloadTimer >= equip.currentWeapon.reloadTime)
            {
                reloadTimer = 0.0f;
                isReloading = false;
                equip.ReloadAmmo();
                hitscanShoot.canFire = true;
            }
        }
        if(flourishActive)
        {
            flourishTimer -= Time.deltaTime;
            if(flourishTimer <= 0.0f)
            {
                flourishActive = false;
                hitscanShoot.isFlourishing = false;
                equip.currentWeaponObject.GetComponent<WeaponFX>().SetFlourishActive(false);
            }
        }
        if (isFlourishing)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, FlourishFOV, Time.deltaTime * 10.0f);
            
            flourishIndicator.SetActive(true);
            Vector3 newPos = flourishIndicator.transform.localPosition;
            switch (currentFlourish.Moves[currentMoveIndex])
            {
                case Flourish.FlourishDir.UP:
                    newPos = new Vector3(0, GetCurrMovePercentage(), 0);
                    flourishIndicator.transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case Flourish.FlourishDir.DOWN:
                    newPos = new Vector3(0, -GetCurrMovePercentage(), 0);
                    flourishIndicator.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Flourish.FlourishDir.LEFT:
                    newPos = new Vector3(-GetCurrMovePercentage(), 0, 0);
                    flourishIndicator.transform.rotation = Quaternion.Euler(0, 0, 270);
                    break;
                case Flourish.FlourishDir.RIGHT:
                    newPos = new Vector3(GetCurrMovePercentage(), 0, 0);
                    flourishIndicator.transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
            }
            flourishIndicator.transform.localPosition = Vector3.Lerp(flourishIndicator.transform.localPosition, newPos, Time.deltaTime * 20.0f);

            //change colour based on if waiting or not
            if (equip.IsWaiting()){
                if (flourishIndicator.GetComponent<Image>()) flourishIndicator.GetComponent<Image>().color = Color.green;
            }
            else
            {
                if (flourishIndicator.GetComponent<Image>()) flourishIndicator.GetComponent<Image>().color = Color.white;
            }
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, FOV, Time.deltaTime * 10.0f);
            flourishIndicator.SetActive(false);
            chromaticAberrationIntensity = Mathf.Lerp(chromaticAberrationIntensity, 0.0f, Time.deltaTime * 10.0f);
        }
        
    }

    

    void OnMouseMove(InputAction.CallbackContext context)
    {
        if (isFlourishing && equip.IsWaiting())
        {
            
            switch (currentFlourish.Moves[currentMoveIndex])
            {
                case Flourish.FlourishDir.UP:
                    currentMoveAmount += context.ReadValue<Vector2>().y * Time.deltaTime;
                    break;
                case Flourish.FlourishDir.DOWN:
                    currentMoveAmount += -context.ReadValue<Vector2>().y * Time.deltaTime;
                    break;
                case Flourish.FlourishDir.LEFT:
                    currentMoveAmount += -context.ReadValue<Vector2>().x * Time.deltaTime;
                    break;
                case Flourish.FlourishDir.RIGHT:
                    currentMoveAmount += context.ReadValue<Vector2>().x * Time.deltaTime;
                    break;
            }
            if(currentMoveAmount < 0.0f)
            {
                currentMoveAmount = 0.0f;
            }
            if (currentMoveAmount >= moveAmountReqiured)
            {
                currentMoveAmount = 0.0f;
                equip.SetTrigger(new string[] {"r", (currentMoveIndex+1).ToString()}, equip.currentWeapon);
                
                equip.currentWeaponObject.GetComponent<WeaponFX>().PlayFlourish(currentMoveIndex);
                
                currentMoveIndex++;
                if (currentMoveIndex >= currentFlourish.Moves.Count)
                {
                    currentMoveIndex = 0;
                    isFlourishing = false;
                    FlourishComplete();
                }
            }
        }
    }

    void OnInitFlourish(InputAction.CallbackContext context)
    {
        if (currentFlourish != null && !isReloading)
        {
            //Cursor.lockState = CursorLockMode.Confined;
            mouselook.lockMouse = true;
            currentMoveAmount = 0.0f;
            currentMoveIndex = 0;
            isFlourishing = true;
            flourishControl.Enable();
            equip.SetTrigger(new string[] { "r", "0" }, equip.currentWeapon);
            hitscanShoot.canFire = false;
        }
    }

    void OnCancelFlourish(InputAction.CallbackContext context)
    {
        if (isFlourishing)
        {
            //Cursor.lockState = CursorLockMode.Locked;
            StartCoroutine(DisableMouseLock());
            isFlourishing = false;
            flourishControl.Disable();
            equip.SetTrigger(new string[] { "c" }, equip.currentWeapon);
            hitscanShoot.canFire = true;
        }
    }

    void FlourishComplete()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(DisableMouseLock());
        StartCoroutine(FlourishEffectAfterDelay(equip.currentWeapon.flourish.flourishActivateDelay));
        flourishControl.Disable();
        equip.ReloadAmmo();
        
    }

    
    IEnumerator DisableMouseLock()
    {
        yield return new WaitForSeconds(0.5f);
        mouselook.lockMouse = false;
    }

    IEnumerator FlourishEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitscanShoot.canFire = true;
        flourishActive = true;
        flourishTimer = equip.currentWeapon.flourish.effectDuration;
        hitscanShoot.isFlourishing = true;
        equip.currentWeaponObject.GetComponent<WeaponFX>().SetFlourishActive(true);
        
    }
    
}
