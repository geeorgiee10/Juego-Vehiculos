using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher2 : MonoBehaviour
{

    [Header("Cameras")] public List<Camera> cameras;

    private int indiceActual = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ActivarSoloEsta(indiceActual);
    }

    private void CambiarCamara()
    {
        indiceActual++;
        if (indiceActual >= cameras.Count)
            indiceActual = 0;

        ActivarSoloEsta(indiceActual);
    }

    public void OnCambioCamara(InputValue value)
    {
        if (value.isPressed)
            CambiarCamara();
    }

    private void ActivarSoloEsta(int index)
    {
        for(int i = 0; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(i == index);
            if(i == index)
                cameras[i].gameObject.tag = "MainCamera";
            else
                cameras[i].gameObject.tag = "Untagged";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
