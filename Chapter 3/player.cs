using Colyseus.Schema;

public partial class Player : Schema {
	[Type(0, "string")]
	public string id = default(string);

	[Type(1, "string")]
	public string ownerId = default(string);

	[Type(2, "number")]
	public float xPos = default(float);

	[Type(3, "number")]
	public float yPos = default(float);

	[Type(4, "string")]
	public string sessionId = default(string);

	[Type(5, "boolean")]
	public bool connected = default(bool);	

	[Type(6, "number")]
	public float health = default(float);

	[Type(7, "number")]
	public float score = default(float);
}

