using UnityEngine;

public class SimpleWaiter : MonoBehaviour
{
    private InputSystem_Actions controls;
    public bool isWaiting = true;

    private void OnEnable()
    {
        isWaiting = true;
        controls.UI.Submit.performed += OnSubmit;
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.UI.Submit.performed -= OnSubmit;
        controls.Disable();
    }
    private void Awake() => controls = new InputSystem_Actions();

    private void OnSubmit(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        isWaiting = false;
    }
}
