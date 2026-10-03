using UnityEngine;

[System.Serializable]
public class DiscordJoinEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordSpectateEvent : UnityEngine.Events.UnityEvent<string> { }

[System.Serializable]
public class DiscordJoinRequestEvent : UnityEngine.Events.UnityEvent<DiscordRpc.DiscordUser> { }

public class DiscordController : MonoBehaviour
{
    public DiscordRpc.RichPresence presence = new DiscordRpc.RichPresence();
    public string applicationId;
    public string optionalSteamId;
    public UnityEngine.Events.UnityEvent onConnect;
    public UnityEngine.Events.UnityEvent onDisconnect;
    public UnityEngine.Events.UnityEvent hasResponded;

    DiscordRpc.EventHandlers handlers;

    public string Installed;
    public string DownloadProgress;

    private static long Timestamp = 0;

    public void ReadyCallback(ref DiscordRpc.DiscordUser connectedUser)
    {
        Debug.Log(string.Format("Discord: connected to {0}#{1}: {2}", connectedUser.username, connectedUser.discriminator, connectedUser.userId));
        onConnect.Invoke();
    }

    public void DisconnectedCallback(int errorCode, string message)
    {
        Debug.Log(string.Format("Discord: disconnect {0}: {1}", errorCode, message));
        onDisconnect.Invoke();
    }

    public void ErrorCallback(int errorCode, string message)
    {
        Debug.LogError(string.Format("Discord: error {0}: {1}", errorCode, message));
    }

    public void SelectVersion()
    {
        presence.details = "Selecting a version";
        presence.state = $"Beat Saber 0.11.2";
        DiscordRpc.UpdatePresence(presence);
    }

    public void DownloadUpdate()
    {
        presence.details = $"{DownloadProgress}"; 
        presence.state = $"Beat Saber 0.11.2";
        DiscordRpc.UpdatePresence(presence);
    }

    public void VersionStart()
    {
        presence.details = $"Main Menu";
        presence.state = "";
        DiscordRpc.UpdatePresence(presence);
    }

    void Start()
    {
        DiscordRpc.UpdatePresence(presence);
    }
    void Update()
    {
        DiscordRpc.RunCallbacks();
    }

    void OnEnable()
    {
        Debug.Log("Discord: init");
        handlers = new DiscordRpc.EventHandlers();
        handlers.readyCallback += ReadyCallback;
        handlers.disconnectedCallback += DisconnectedCallback;
        handlers.errorCallback += ErrorCallback;
        DiscordRpc.Initialize(applicationId, ref handlers, true, optionalSteamId);
    }

    void OnDisable()
    {
        Debug.Log("Discord: shutdown");
        DiscordRpc.Shutdown();
    }

    void OnDestroy()
    {

    }
}