using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput Instance;

    public PlayerInput playerInput { get; private set; }

    [field: SerializeField] public Vector2 moveInput { get; private set; }
    [field: SerializeField] public bool jumpInput { get; private set; }
    [field: SerializeField] public bool dashInput { get; private set; }
    [field: SerializeField] public bool runInput { get; private set; }
    [field: SerializeField] public bool use1Input { get; private set; }
    [field: SerializeField] public bool use2Input { get; private set; }
    [field: SerializeField] public bool menuToggleInput { get; private set; }
    [field: SerializeField] public bool activeItem1Input { get; private set; }
    [field: SerializeField] public bool mapInput { get; private set; }

    public InputAction moveAction { get; private set; }
    public InputAction jumpAction { get; private set; }
    public InputAction dashAction { get; private set; }
    public InputAction runAction { get; private set; }
    public InputAction use1Action { get; private set; }
    public InputAction use2Action { get; private set; }
    public InputAction menuToggleAction { get; private set; }
    public InputAction activeItem1Action { get; private set; }
    public InputAction mapAction { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        playerInput = GetComponent<PlayerInput>();


        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        dashAction = playerInput.actions["Dash"];
        runAction = playerInput.actions["Run"];
        use1Action = playerInput.actions["Use1"];
        use2Action = playerInput.actions["Use2"];
        menuToggleAction = playerInput.actions["MenuToggle"];
        activeItem1Action = playerInput.actions["ActiveItem1"];
        mapAction = playerInput.actions["Map"];
    
    }
    void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        jumpInput = jumpAction.WasPressedThisFrame();
        dashInput = dashAction.WasPressedThisFrame();
        runInput = runAction.WasPressedThisFrame();
        use1Input = use1Action.WasPressedThisFrame();
        use2Input = use2Action.WasPressedThisFrame();
        menuToggleInput = menuToggleAction.WasPressedThisFrame();
        activeItem1Input = activeItem1Action.WasPressedThisFrame();
        mapInput = mapAction.WasPressedThisFrame();
    }
}
