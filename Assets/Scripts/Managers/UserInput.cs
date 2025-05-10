using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput Instance;

    [field: SerializeField] public Vector2 MoveInput { get; private set; }
    [field: SerializeField] public bool JumpInput { get; private set; }
    [field: SerializeField] public bool DashInput { get; private set; }
    [field: SerializeField] public bool RunInput { get; private set; }
    [field: SerializeField] public bool Use1Input { get; private set; }
    [field: SerializeField] public bool Use2Input { get; private set; }
    [field: SerializeField] public bool MenuToggleInput { get; private set; }
    [field: SerializeField] public bool ActiveItem1Input { get; private set; }
    [field: SerializeField] public bool MapInput { get; private set; }


    public PlayerInput _playerInput { get; private set; }
    public InputAction _moveAction { get; private set; }
    public InputAction _jumpAction { get; private set; }
    public InputAction _dashAction { get; private set; }
    public InputAction _runAction { get; private set; }
    public InputAction _use1Action { get; private set; }
    public InputAction _use2Action { get; private set; }
    public InputAction _menuToggleAction { get; private set; }
    public InputAction _activeItem1Action { get; private set; }
    public InputAction _mapAction { get; private set; }

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


        _playerInput = GetComponent<PlayerInput>();


        _moveAction = _playerInput.actions["Move"];
        _jumpAction = _playerInput.actions["Jump"];
        _dashAction = _playerInput.actions["Dash"];
        _runAction = _playerInput.actions["Run"];
        _use1Action = _playerInput.actions["Use1"];
        _use2Action = _playerInput.actions["Use2"];
        _menuToggleAction = _playerInput.actions["MenuToggle"];
        _activeItem1Action = _playerInput.actions["ActiveItem1"];
        _mapAction = _playerInput.actions["Map"];
    
    }
    void Update()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        JumpInput = _jumpAction.WasPressedThisFrame();
        DashInput = _dashAction.WasPressedThisFrame();
        RunInput = _runAction.WasPressedThisFrame();
        Use1Input = _use1Action.WasPressedThisFrame();
        Use2Input = _use2Action.WasPressedThisFrame();
        MenuToggleInput = _menuToggleAction.WasPressedThisFrame();
        ActiveItem1Input = _activeItem1Action.WasPressedThisFrame();
        MapInput = _mapAction.WasPressedThisFrame();
    }
}
