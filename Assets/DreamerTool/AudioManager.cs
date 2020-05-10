/*****************************
Created by 师鸿博
*****************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DreamerTool.ScriptableObject;
using DreamerTool.Singleton;
public class AudioManager : MonoSingleton<AudioManager>
{
    private AudioSource _audio;
    private AudioClips clips;
    private void Awake()
    {
        if (!GetComponent<AudioSource>())
            _audio = gameObject.AddComponent<AudioSource>();

        clips= ScriptableObjectUtil.GetScriptableObject<AudioClips>();
    }
 


    public void PlayOneShot(string audio_name)
    {
        var clip = clips.GetClip(audio_name);
        
        _audio.PlayOneShot(clip);
    }
}
