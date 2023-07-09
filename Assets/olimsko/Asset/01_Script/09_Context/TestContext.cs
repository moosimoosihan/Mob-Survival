using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using olimsko;
using UnityEngine.InputSystem;

public class TestContext : ContextModel
{
    // Start is called before the first frame update
    // void Start()
    // {
    //     OSManager.GetService<InputManager>().GetAction("Move").Enable();
    //     OSManager.GetService<InputManager>().GetAction("Move").performed += OnMove;

    //     OSManager.GetService<InputManager>().GetAction("GatheringCharacter").Enable();
    //     OSManager.GetService<InputManager>().GetAction("GatheringCharacter").performed += OnGather;
    // }

    // private void OnMove(InputAction.CallbackContext obj)
    // {
    //     obj.ReadValue<Vector2>();
    // }

    // private void OnGather(InputAction.CallbackContext obj)
    // {
    //     if (obj.)
    //     {
    //         Debug.Log("ssss");
    //     }
    // }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            OSManager.GetService<GlobalManager>().PlayerInventory.Gold += 1000;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            OSManager.GetService<GlobalManager>().PlayerInventory.Gold -= 1000;
        }
    }
}
