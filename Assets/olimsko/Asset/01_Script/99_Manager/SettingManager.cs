using UnityEngine;
using olimsko;
using Cysharp.Threading.Tasks;

[InitializeAtRuntime]
public class SettingManager : IOSMEntity, IManagedState<SettingState>
{
    private VideoSetting m_VideoSetting;
    private InputSetting m_InputSetting;
    private AudioSetting m_AudioSetting;
    private GamePlaySetting m_GamePlaySetting;

    private StateManager StateManager => OSManager.GetService<StateManager>();

    public VideoSetting VideoSetting { get => m_VideoSetting; set => m_VideoSetting = value; }
    public InputSetting InputSetting { get => m_InputSetting; set => m_InputSetting = value; }
    public AudioSetting AudioSetting { get => m_AudioSetting; set => m_AudioSetting = value; }
    public GamePlaySetting GamePlaySetting { get => m_GamePlaySetting; set => m_GamePlaySetting = value; }

    public UniTask InitializeAsync()
    {
        return UniTask.CompletedTask;
    }

    public UniTask LoadDataStateAsync(SettingState state)
    {
        m_VideoSetting = state.GetState<VideoSetting>();
        m_InputSetting = state.GetState<InputSetting>();
        m_AudioSetting = state.GetState<AudioSetting>();
        m_GamePlaySetting = state.GetState<GamePlaySetting>();

        return UniTask.CompletedTask;
    }

    public void SaveDataState(SettingState state)
    {
        state.SetState<VideoSetting>(m_VideoSetting);
        state.SetState<InputSetting>(m_InputSetting);
        state.SetState<AudioSetting>(m_AudioSetting);
        state.SetState<GamePlaySetting>(m_GamePlaySetting);
    }

    public async UniTask SaveSetting()
    {
        await StateManager.SaveSettingsAsync();
    }

    public void RevertSetting()
    {
        m_VideoSetting = new VideoSetting();
        m_InputSetting = new InputSetting();
        m_AudioSetting = new AudioSetting();
        m_GamePlaySetting = new GamePlaySetting();
    }

    public void Reset()
    {

    }

    public void DestroyThis()
    {

    }

    public UniTask InitializeStateAsync()
    {
        return UniTask.CompletedTask;
    }
}