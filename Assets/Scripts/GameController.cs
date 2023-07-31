using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class GameController : MonoBehaviour {
    //Initialize Singleton
    public static GameController instance;

    //Initialize Player characters
    [Header("- Players -")]
    [SerializeField] private int maxPlayers = 1;
    public List<Player> players;
    [SerializeField] private GameObject playerInstance;
    [SerializeField] private List<Transform> spawnPoints;

    [Header("- Characters -")]
    public List<GameObject> characters;
    
    //Initialize Input
    [Header("- Players Input -")]
    public List<InputActionMap> inputs;
    public InputActionMap input;

    private void Awake() {
        //Assign Singleton
        if (GameObject.FindGameObjectsWithTag("GameController").Length > 1) { 
            Destroy(this.gameObject); 
        } else { instance = this; DontDestroyOnLoad(this.gameObject); }
    }

    private void Start() => SpawnPlayer();

    public void RespawnPlayer(int playerIndex) {
        if (players.Contains(players[playerIndex])) {
            if (!players[playerIndex].gameObject.activeSelf) {
                players[playerIndex].gameObject.SetActive(true);
                players[playerIndex].transform.position = spawnPoints[playerIndex].position;
            }
        }
    }

    public void SpawnPlayer() {
        if (playerInstance != null && spawnPoints.Count > 0) {
            if (players.Count < maxPlayers) { 
                Player spawnedPlayer = Instantiate(
                    playerInstance, 
                    spawnPoints[players.Count].position, 
                    spawnPoints[players.Count].rotation
                ).GetComponent<Player>(); 
                
                players.Add(spawnedPlayer);
                spawnedPlayer.playerId = players.Count;
                spawnedPlayer.name = "Player " + spawnedPlayer.playerId;
                spawnedPlayer.playerCharacter = 0;
                
            }
        }
    }
}
