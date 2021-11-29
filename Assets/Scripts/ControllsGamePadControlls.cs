// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/ControllsGamePadControlls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @ControllsGamePadControlls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @ControllsGamePadControlls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""ControllsGamePadControlls"",
    ""maps"": [
        {
            ""name"": ""RunnerGamePad"",
            ""id"": ""04979d69-e216-469f-817d-0a874e345751"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""7a804743-c0f5-41cc-af8d-d108aa08c712"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""8a6737f7-66b7-4bdb-a7f4-822c756a7b53"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // RunnerGamePad
        m_RunnerGamePad = asset.FindActionMap("RunnerGamePad", throwIfNotFound: true);
        m_RunnerGamePad_Jump = m_RunnerGamePad.FindAction("Jump", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // RunnerGamePad
    private readonly InputActionMap m_RunnerGamePad;
    private IRunnerGamePadActions m_RunnerGamePadActionsCallbackInterface;
    private readonly InputAction m_RunnerGamePad_Jump;
    public struct RunnerGamePadActions
    {
        private @ControllsGamePadControlls m_Wrapper;
        public RunnerGamePadActions(@ControllsGamePadControlls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_RunnerGamePad_Jump;
        public InputActionMap Get() { return m_Wrapper.m_RunnerGamePad; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(RunnerGamePadActions set) { return set.Get(); }
        public void SetCallbacks(IRunnerGamePadActions instance)
        {
            if (m_Wrapper.m_RunnerGamePadActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_RunnerGamePadActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_RunnerGamePadActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_RunnerGamePadActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_RunnerGamePadActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public RunnerGamePadActions @RunnerGamePad => new RunnerGamePadActions(this);
    public interface IRunnerGamePadActions
    {
        void OnJump(InputAction.CallbackContext context);
    }
}
