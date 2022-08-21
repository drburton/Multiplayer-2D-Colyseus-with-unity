using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using Colyseus;
using Colyseus.Schema;

using GameDevWare.Serialization;

/*[Serializable]
class MoveData
{
	public float xPos;
	public float yPos;
}   */

public class ColyseusClient : ColyseusManager<ColyseusClient> {

	public String sessionId;
	public String roomName;
	public GameObject Spawn;
    public GameObject player1;
	public GameObject enemy;

	protected ColyseusClient myClient;
	public ColyseusRoom<State> Room;
	protected IndexedDictionary<Player, GameObject> players = new IndexedDictionary<Player, GameObject>();

    
    // Initialization connection to server
    void Start () {
  	    player1 = GameObject.Find("myPlayer");
        ColyseusClient.Instance.InitializeClient();
	 
	  	JoinOrCreateRoom();
	}
    
    public async void JoinOrCreateRoom() {
		Room = await client.JoinOrCreate<State>(roomName);
		Debug.Log("Joined room " + Room.Id);

		RegisterRoomHandlers();
	}

    public void RegisterRoomHandlers() {
		sessionId = Room.SessionId;

		Room.State.players.OnAdd += OnPlayerAdd;
		Room.State.TriggerAll();
		Room.OnLeave += (code) => Debug.Log("ROOM: ON LEAVE");
		Room.OnError += (code, message) => Debug.LogError("ERROR, code =>" + code + ", message => " + message);
		Room.OnStateChange += OnStateChangeHandler;

		PlayerPrefs.SetString("roomId", Room.Id);
		PlayerPrefs.SetString("sessionId", Room.SessionId);
		PlayerPrefs.Save();
    }

	public void OnPlayerMove() {
		Room = GameObject.Find("NetworkClient").GetComponent<ColyseusClient>().Room;
		Room.Send("move", new {
			xPos = player1.transform.position.x, 
			yPos = player1.transform.position.y
		});  
	}

    async void LeaveRoom() {
	await Room.Leave(false);

	// Destroy player entities
	foreach (KeyValuePair<Player, GameObject> player in players)
	  {
		Destroy(player.Value);
	  }
		players.Clear();
	}

void OnStateChangeHandler (State state, bool isFirstState ) {
	} 

void OnPlayerAdd(string key, Player player) {   
//	Debug.Log("OnPlayerAdd routine");
	var tempPos = Spawn.transform.position;

// Set Player's spawnpoint
	if (player.sessionId == Room.SessionId ) {	
		players.Add(player, player1);
		Debug.Log("Player1 added to players list #94");
	} else {

// Set enemy's spawnpoints
		GameObject myEnemy = Instantiate(enemy);
		myEnemy.transform.position = new Vector2(tempPos.x, tempPos.y);

		// Add "enemy" to map of players
		players.Add(player, myEnemy);
		
		Debug.Log("Enemy added to players list #103");	
		player.OnChange += (List<Colyseus.Schema.DataChange> changes) => {
			myEnemy.transform.position = new Vector2(player.xPos, player.yPos);
		};
	}
	}
}
