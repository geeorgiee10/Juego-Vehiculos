using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarSounds : MonoBehaviour
{

    public Rigidbody carRigidbody;

    //Motor
    public AudioSource engineAudioSource;
    //Arranque del motor
    public AudioSource startEngineAudioSource;
    //Pito
    public AudioSource putoPitoAudioSource;

    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    public float maxSpeed = 50f;

    public float pitchSmoothSpeed = 3.0f;
    public CarController carController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (carRigidbody == null)
            carRigidbody = GetComponent<Rigidbody>();

        engineAudioSource.loop = true;
        startEngineAudioSource.loop = false;
        putoPitoAudioSource.loop = false; 
        engineAudioSource.Stop();
        startEngineAudioSource.Stop();
        putoPitoAudioSource.Stop();  
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnArrancar(InputValue playerValue)
    {
        if(playerValue.isPressed)
        {
            //if (!carController.arrancado)
            //    return;

            carController.arrancado = !carController.arrancado;
            
            if(carController.arrancado)
            {
                StartCoroutine(SonidoArranqueYMotor());
            }
            else
            {
                engineAudioSource.Stop();
                startEngineAudioSource.Stop();
            }
            
        }
    }

    private IEnumerator SonidoArranqueYMotor()
    {
        startEngineAudioSource.Play();
        yield return new WaitWhile(() => startEngineAudioSource.isPlaying);
        engineAudioSource.Play();
    }

    public void OnPutoPito(InputValue playerValue)
    {
        if (playerValue.isPressed)
        {
            putoPitoAudioSource.Play();
        }
    }
}
