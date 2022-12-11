using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections.Generic;

using System.Threading;
using System.Threading.Tasks;

using Colyseus;
using Colyseus.Schema;

using GameDevWare.Serialization;

public class ColyseusClient : ColyseusManager<ColyseusClient> {

	public String ownerId;
	public String sessionId;
	public String roomName;
    public GameObject player1;
	public GameObject enemy;

	public HealthBar playerHealth;

//	public HealthBar enemyHealth;
	public myScore playerScore;

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
//	var tempPos = Spawn.transform.position;

// Set Playerinfo
	if (player.sessionId == Room.SessionId ) {	
		players.Add(player, player1);
		player1.GetComponentInChildren<TMPro.TMP_Text>().text = ownerId;
//		Debug.Log("Player1 added to players list #87 "+ ownerId);
		player.ownerId = ownerId;
		playerScore.SetScore((int) player.score);
		playerHealth.SetHealth((int) player.health);
		Debug.Log("player health & score set at 94:"+player.health+" "+player.score);

	
	} else {

// Spawn enemy
		GameObject myEnemy = Instantiate(enemy);
		myEnemy.transform.position = new Vector2(player.xPos, player.yPos);
		Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, myEnemy.transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 250 * Time.deltaTime);

		// Add "enemy" to map of players
		players.Add(player, myEnemy);
		myEnemy.GetComponentInChildren<TMPro.TMP_Text>().text = player.ownerId;
		Debug.Log("Enemy added to players list #101 "+player.ownerId);	

		player.OnChange += (List<Colyseus.Schema.DataChange> changes) => {
			myEnemy.transform.position = new Vector2(player.xPos, player.yPos);
			var findTMPro = myEnemy.GetComponentInChildren<TMPro.TMP_Text>(); 
        	Quaternion savedRotation = findTMPro.GetComponent<RectTransform>().rotation;
        	Vector3 savedPosition = findTMPro.GetComponent<RectTransform>().position;

			Quaternion toRotation = Quaternion.LookRotation(Vector3.forward, myEnemy.transform.position);
    	    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 250 * Time.deltaTime);
			
	        findTMPro.GetComponent<RectTransform>().rotation = savedRotation;
        	findTMPro.GetComponent<RectTransform>().position = savedPosition;		
			myEnemy.GetComponentInChildren<HealthBar>().SetHealth((int) player.health);
		};
	}
	}
}
