using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.InputSystem;


public class FlourishHandler : MonoBehaviour
{
    public GameObject flourishIndicator;
    public Animator reloadAnimator;
    public InputAction initFlourish;
    public InputAction flourishControl;
    Equipment equip;

    public int currentMoveIndex = 0;
    public Flourish currentFlourish;

    public float moveAmountReqiured = 0.5f;

    float currentMoveAmount = 0.0f;
    bool isFlourishing = false;

    MouseLook mouselook;

    Camera mainCamera;
    public Volume volume;

    public float chromaticAberrationIntensity = 0.0f;

    HitscanShoot hitscanShoot;
    
    

    // Start is called before the first frame update
    void Start()
    {
        initFlourish.Enable();
        initFlourish.started += OnInitFlourish;
        initFlourish.canceled += OnCancelFlourish;
        flourishControl.performed += OnMouseMove;
        mouselook = GetComponentInChildren<MouseLook>();
        mainCamera = Camera.main;
        equip = GetComponent<Equipment>();
        hitscanShoot = GetComponent<HitscanShoot>();
    }

    float GetCurrMovePercentage()
    {
        return (currentMoveAmount / moveAmountReqiured) * 200.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFlourishing)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 90.0f, Time.deltaTime * 10.0f);
            chromaticAberrationIntensity = Mathf.Lerp(chromaticAberrationIntensity, 1.0f, Time.deltaTime * 10.0f);
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
        }
        else
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 80.0f, Time.deltaTime * 10.0f);
            flourishIndicator.SetActive(false);
            chromaticAberrationIntensity = Mathf.Lerp(chromaticAberrationIntensity, 0.0f, Time.deltaTime * 10.0f);
        }
        
    }

    

    void OnMouseMove(InputAction.CallbackContext context)
    {
        if (isFlourishing)
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
        if (currentFlourish != null)
        {
            //Cursor.lockState = CursorLockMode.Confined;
            mouselook.lockMouse = true;
            currentMoveAmount = 0.0f;
            currentMoveIndex = 0;
            isFlourishing = true;
            flourishControl.Enable();
            equip.SetTrigger(new string[] { "r", "0" }, equip.currentWeapon);
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
        }
    }

    void FlourishComplete()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(DisableMouseLock());
        flourishControl.Disable();
        equip.ReloadAmmo();
    }

    
    IEnumerator DisableMouseLock()
    {
        yield return new WaitForSeconds(0.4f);
        mouselook.lockMouse = false;
    }
    
}
