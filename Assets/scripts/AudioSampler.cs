using UnityEngine;
using System.Collections.Generic;


[RequireComponent(typeof(AudioSource))]
public class AudioSampler : MonoBehaviour
{
    [SerializeField]
    AudioClip[] clips;

    [SerializeField]
    float[] spectrum = new float[256];
    [SerializeField]
    float[] outputData = new float[256];
    float[] upscaledOutputData = new float[256];
    public bool play;
    [SerializeField]float[] sampleData;
    AudioSource source;

    public int startingPitch = 4;
    public int timeToDecrease = 5;

    [SerializeField]
    AudioSource speakerSource;

    Queue<float> targetQueue = new Queue<float>();
    private void Start()
    {
        source = GetComponent<AudioSource>();

        //sampleData = new float[(int)(source.clip.length * 48)];
        ////clips[0].SetData(sampleData, 0);
        ////AudioClip clip = new AudioClip();
        //for (int i = 0; i < sampleData.Length; ++i)
        //{
        //    sampleData[i] = 0.5f;
        //}
        //clips[0].SetData(sampleData, 0);
        //source.clip.GetData(sampleData, 0);
        //source.pitch = startingPitch;
    }

    AudioClip myClip;
    private void Awake()
    {
        //Debug.LogWarning(clips[0].loadType.ToString());
        sampleData = new float[(int)(clips[0].samples)];
        //clips[0].SetData(sampleData, 0);
        //AudioClip clip = new AudioClip();
        for (int i = 0; i < sampleData.Length; ++i)
        {
            sampleData[i] = Random.Range(-1, 1);
        }
        //clips[0].SetData(sampleData, 0);
        //Debug.LogWarning("Ready");

        myClip = AudioClip.Create("MySinusoid", samplerate * 5, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
        //Debug.LogWarning("My clip samples: " + myClip.samples);
        //myClip.St
        //clips[0] = myClip;
    }


    public int position = 0;
    public int samplerate = 256;
    public float frequency = 440;
    public int dataCount = 0;
    void OnAudioRead(float[] data)
    {
        int count = 0;
        //data = new float[100000];
        while (count < data.Length)
        {
            if (targetQueue.Count > 0) {
                data[count] = targetQueue.Dequeue();
                //The unit should now move towards this point
            }
            //data[count] = upscaledOutputData[position];
            position++;
            count++;
        }
        //source.getout
        dataCount += count;
        
        //Debug.LogWarning("Audio Reading: Data length: "+ data.Length);
    }

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
        //Debug.LogWarning("Audio position set");
    }


    void Update()
    {
        if (play)
        {
            //source.pitch = startingPitch;
            source.clip = clips[0];
            //Debug.LogWarning("Samples: "+ source.clip.samples+ " Length: "+source.clip.length);
            source.Play();
            dataCount = 0;
        }
        if (source.pitch > 0)
        {
            //source.pitch -= Time.deltaTime * startingPitch / timeToDecrease;
        }
        //source.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        source.GetOutputData(outputData, 0);
        int repeatCount = samplerate / outputData.Length;
        for (int i = 0; i < samplerate && source.isPlaying; i++)
        {
            if (i / repeatCount < outputData.Length)
                targetQueue.Enqueue(outputData[i / repeatCount]);
        }
        //if (Mathf.Abs(outputData[0]) > 10)
        //{
        //    Debug.Log("Audio");
        //}

        if (play)
        {
            speakerSource.clip = myClip;
            speakerSource.Play();
            play = false;
        }
        speakerSource.GetOutputData(spectrum, 0);
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            //Debug.LogError("Audio");
            Debug.DrawLine(new Vector3(i - 1, spectrum[i] + 10, 0), new Vector3(i, spectrum[i + 1] + 10, 0), Color.red);
            Debug.DrawLine(new Vector3(i - 1, Mathf.Log(spectrum[i - 1]) + 10, 2), new Vector3(i, Mathf.Log(spectrum[i]) + 10, 2), Color.cyan);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), spectrum[i - 1] - 10, 1), new Vector3(Mathf.Log(i), spectrum[i] - 10, 1), Color.green);
            Debug.DrawLine(new Vector3(Mathf.Log(i - 1), Mathf.Log(spectrum[i - 1]), 3), new Vector3(Mathf.Log(i), Mathf.Log(spectrum[i]), 3), Color.blue);
        }
    }
}