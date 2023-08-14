using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public class AudioManager : Singleton<AudioManager>
{
    private IObjectPool<LoopSFXPlayer> _Pool;
    public GameObject loopSFXPrefab;

    public AudioClip[] bgmClip;
    public AudioClip[] sfxClip;
    public AudioClip[] loopSfxClip;

    public float bgmVolume = 1;
    public float sfxVolume = 1;
    public float uiSfxVolume = 1;

    public enum Sfx { Archer_Attack, Healer_Attack, Revive, Worrior_FireStrike, Worrior_Attack, Wizard_Attack }
    public enum LoopSfx { Archer_Buff1, Archer_Buff2, Shield, FireArmor, Wizard_IceAge }
    
    public void Init()
    {
        _Pool = new ObjectPool<LoopSFXPlayer>(CreateLoopSfxPlayer, OnGetLoopSfxPlayer, OnReleaseLoopSfxPlayer, OnDestroyLoopSfxPlayer);
    }
    public void SfxPlay(Sfx type)
    {
        int playNum = 0;
        switch(type){
            case Sfx.Archer_Attack:
                playNum = Random.Range(0, 3);
                break;
            case Sfx.Healer_Attack:
                playNum = Random.Range(3, 6);
                break;
            case Sfx.Revive:
                playNum = 6;
                break;
            case Sfx.Worrior_FireStrike:
                playNum = 7;
                break;
            case Sfx.Worrior_Attack:
                playNum = Random.Range(8, 11);
                break;
            case Sfx.Wizard_Attack:
                playNum = Random.Range(11, 14);
                break;
        }
        AudioSource.PlayClipAtPoint(sfxClip[playNum], GameManager.instance.pool.transform.position, sfxVolume);
    }
    public GameObject LoopSfxPlay(LoopSfx type)
    {
        int playNum = 0;
        switch(type){
            case LoopSfx.Archer_Buff1:
                playNum = 0;
                break;
            case LoopSfx.Archer_Buff2:
                playNum = 1;
                break;
            case LoopSfx.Shield:
                playNum = 2;
                break;
            case LoopSfx.FireArmor:
                playNum = 3;
                break;
            case LoopSfx.Wizard_IceAge:
                playNum = 4;
                break;
        }
        LoopSFXPlayer loopSfxPlayer = _Pool.Get();
        loopSfxPlayer.transform.parent = GameManager.instance.pool.transform;
        AudioSource loopPlayer = loopSfxPlayer.GetComponent<AudioSource>();
        loopPlayer.clip = loopSfxClip[playNum];
        loopPlayer.volume = sfxVolume / 2;
        loopPlayer.loop = true;
        loopPlayer.Play();
        return loopSfxPlayer.gameObject;
    }
    LoopSFXPlayer CreateLoopSfxPlayer()
    {
        LoopSFXPlayer loopPlayer = Instantiate(loopSFXPrefab).GetComponent<LoopSFXPlayer>();
        loopPlayer.SetManagedPool(_Pool);
        return loopPlayer;
    }
    void OnGetLoopSfxPlayer(LoopSFXPlayer player)
    {
        player.gameObject.SetActive(true);
    }
    void OnReleaseLoopSfxPlayer(LoopSFXPlayer player)
    {
        player.gameObject.SetActive(false);
    }
    void OnDestroyLoopSfxPlayer(LoopSFXPlayer player)
    {
        Destroy(player.gameObject);
    }
}
