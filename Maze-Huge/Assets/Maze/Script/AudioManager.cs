using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
  public static AudioManager _AudioManager = null;

  [SerializeField]
  private AudioClip[] audio_list = null;

  [SerializeField]
  private AudioMixer[] AudioMixer_list = null;

  private void Awake()
  {
    _AudioManager = this;
  }

  // Start is called before the first frame update
  void Start()
  {
    
  }

  // Update is called once per frame
  void Update()
  {

  }

  public AudioClip GetAudio(string name)
  {

    foreach (var v in audio_list)
    {
      if (v.name == name)
        return v;
    }

    return null;
  }

  public AudioMixer GetAudioMixer(string name)
  {
    if (AudioMixer_list == null)
      return null;

    foreach (var v in AudioMixer_list)
    {
      if (v.name == name)
        return v;
    }

    return null;
  }
}
