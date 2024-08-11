using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MicrophoneInput : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private TextMeshProUGUI volumeText; // Assign this in the Inspector
    [SerializeField] private TextMeshProUGUI volumeMessageText; // Assign this in the Inspector
    private string microphone;
    private int sampleSize = 1024; // Size of the sample array
    public float sensitivity = 2.0f; // Sensitivity multiplier

    void Start()
    {
        // Get the available microphone devices
        if (Microphone.devices.Length > 0)
        {
            // Select the first available microphone
            microphone = Microphone.devices[0];

            // Create an AudioSource component if it doesn't exist
            audioSource = gameObject.AddComponent<AudioSource>();

            // Start recording from the microphone
            audioSource.clip = Microphone.Start(microphone, true, 10, 44100);
            audioSource.loop = true;

            // Wait until the microphone starts recording
            while (!(Microphone.GetPosition(microphone) > 0)) { }

            // Play the audio source with the microphone input
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No microphone detected.");
        }
    }

    void Update()
    {
        if (audioSource != null && Microphone.IsRecording(microphone))
        {
            // Get the current microphone input data
            float[] data = new float[sampleSize];
            audioSource.GetOutputData(data, 0);

            // Calculate the average volume from the audio data
            float sum = 0f;
            for (int i = 0; i < data.Length; i++)
            {
                sum += Mathf.Abs(data[i]);
            }
            float averageVolume = sum / data.Length;

            // Adjust sensitivity
            averageVolume *= sensitivity;

            // Convert the volume to a bar string
            int barCount = Mathf.Clamp((int)(averageVolume * 100), 0, 20); // Scale and clamp the value
            volumeText.text = new string('|', barCount);

            // Set volume message
            if (averageVolume < 0.05f)
            {
                volumeMessageText.text = "Whispering";
            }
            else if (averageVolume < 0.5f)
            {
                volumeMessageText.text = "Talking";
            }
            else
            {
                volumeMessageText.text = "Shouting";
            }
        }
    }

    void OnDisable()
    {
        // Stop recording and clean up when the object is disabled
        if (microphone != null)
        {
            Microphone.End(microphone);
        }
    }
}
