using System.Timers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RFMManagerX : MonoBehaviourPunCallbacks
{
    public static RFMManagerX Instance;
    public GameObject playerPrefab;

    private void Start()
    {
        Instance = this;
        
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in " +
                           "GameObject 'Game Manager'",this);
        }
        else
        {
            if (RFMPlayerX.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }

    #region Public Fields

    [Tooltip("The prefab to use for representing the player")]

    #endregion

    #region Photon Callbacks

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion
    
    #region Private Methods

    void LoadArena()
    {
        // if (!PhotonNetwork.IsMasterClient)
        // {
        //     Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        //     return;
        // }
        // Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        // PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);

        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            StartCountdown();
        }
    }

    private static Timer _countDownTimer;

    private void StartCountdown()
    {
        Debug.LogError("Countdown started!");
        _countDownTimer = new Timer(5000);
        _countDownTimer.Elapsed += StartGame;
        _countDownTimer.AutoReset = false;
        _countDownTimer.Enabled = true;
    }

    private void StartGame(object sender, ElapsedEventArgs elapsedEventArgs)
    {
        Debug.LogError("Game started! at: " + elapsedEventArgs.SignalTime);
        _countDownTimer.Stop();
        _countDownTimer.Dispose();
    }

    #endregion
    
    #region Photon Callbacks

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    #endregion
}