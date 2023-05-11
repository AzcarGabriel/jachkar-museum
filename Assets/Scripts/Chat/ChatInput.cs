using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ChatInput : MonoBehaviour
{

    InputField m_InputField;
    void Start() {
        //Fetch the Input Field component from the GameObject
        m_InputField = GetComponent<InputField>();
    }

    public void onFocus() {
        InputSystem.DisableDevice(Keyboard.current);
    }

    public void onUnFocus() {
        Debug.Log("Desfocuseado");
    }
}
