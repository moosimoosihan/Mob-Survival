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

    public AudioSource sfxPlayer;
    public AudioSource bgmPlayer;

    public enum Sfx {
        Archer_Attack,
        Healer_Attack,
        Revive, Worrior_FireStrike,
        Worrior_Attack,
        Wizard_Attack,
        Ent_Hit,
        Ent_Red_Die,
        Ent_Yellow_Die,
        Goblin_Hit,
        Goblin_Armed_Die,
        Goblin_Die,
        Moth_Hit,
        Moth_1_Die,
        Moth_2_Die,
        Slime_Hit,
        Slime_Blue_Fire_Die,
        Slime_Fire_Die,
        Slime_Poison_Die,
        Slime_Water_Die,
        Snake_Hit,
        Snake_Die,
        Cobra_Die,
        Toad_Hit,
        Toad_Die,
        }
    public enum LoopSfx { Archer_Buff1, Archer_Buff2, Shield, FireArmor, Wizard_IceAge }
    
    public void Init()
    {
        _Pool = new ObjectPool<LoopSFXPlayer>(CreateLoopSfxPlayer, OnGetLoopSfxPlayer, OnReleaseLoopSfxPlayer, OnDestroyLoopSfxPlayer);
        bgmPlayer = GetComponentsInChildren<AudioSource>()[0];
        sfxPlayer = GetComponentsInChildren<AudioSource>()[1];
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
            case Sfx.Ent_Hit:
                playNum = 14;
                break;
            case Sfx.Ent_Red_Die:
                playNum = 15;
                break;
            case Sfx.Ent_Yellow_Die:
                playNum = 16;
                break;
            case Sfx.Goblin_Hit:
                playNum = 17;
                break;
            case Sfx.Goblin_Armed_Die:
                playNum = 18;
                break;
            case Sfx.Goblin_Die:
                playNum = 19;
                break;
            case Sfx.Moth_Hit:
                playNum = 20;
                break;
            case Sfx.Moth_1_Die:    
                playNum = 21;
                break;
            case Sfx.Moth_2_Die:
                playNum = 22;
                break;
            case Sfx.Slime_Hit:
                playNum = 23;
                break;
            case Sfx.Slime_Blue_Fire_Die:
                playNum = 24;
                break;
            case Sfx.Slime_Fire_Die:
                playNum = 25;
                break;
            case Sfx.Slime_Poison_Die:
                playNum = 26;
                break;
            case Sfx.Slime_Water_Die:
                playNum = 27;
                break;
            case Sfx.Snake_Hit:
                playNum = 28;
                break;
            case Sfx.Snake_Die:
                playNum = 29;
                break;
            case Sfx.Cobra_Die:
                playNum = 30;
                break;
            case Sfx.Toad_Hit:
                playNum = 31;
                break;
            case Sfx.Toad_Die:
                playNum = 32;
                break;
        }
        sfxPlayer.PlayOneShot(sfxClip[playNum], sfxVolume / 2);
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
        loopPlayer.volume = sfxVolume / 10; // 너무 크다
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
