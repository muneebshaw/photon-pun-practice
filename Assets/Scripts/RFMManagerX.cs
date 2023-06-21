using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RFMManagerX : MonoBehaviourPunCallbacks
{
    public static RFMManagerX Instance;
    public GameObject playerPrefab;
    public GameObject hunterPrefab;

    [SerializeField] private Transform playersSpawnArea;
    [SerializeField] private Transform huntersSpawnArea;

    [SerializeField] private int countDownTime = 5;
    [SerializeField] private TextMeshProUGUI countDownText;

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

    // private static Timer _countDownTimer;

    private void StartCountdown()
    {
        Debug.LogError("Countdown started!");
        // _countDownTimer = new Timer(5000);
        // _countDownTimer.Elapsed += StartGame;
        // _countDownTimer.AutoReset = false;
        // _countDownTimer.Enabled = true;
        
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        countDownText.gameObject.SetActive(true);
        var remainingTime = countDownTime;

        while (remainingTime > 0)
        {
            countDownText.text = remainingTime.ToString();
            yield return new WaitForSecondsRealtime(1);
            remainingTime--;
        }
        
        countDownText.text = "Game Started!";
        var position = playersSpawnArea.position;
        
        var randomPos = new Vector3(
            position.x + Random.Range(-4, 5),
            position.y,
            position.z + Random.Range(-2, 3));

        
        RFMPlayerX.LocalPlayerInstance.GetComponent<RFMPlayerX>().SetPosition(randomPos, Quaternion.identity);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : MasterClient spawning hunters.");
            PhotonNetwork.Instantiate(hunterPrefab.name, huntersSpawnArea.position, huntersSpawnArea.rotation);
        }
        
        // _countDownTimer.Stop();
        // _countDownTimer.Dispose();
        
        yield return new WaitForSecondsRealtime(1);
        countDownText.gameObject.SetActive(false);
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