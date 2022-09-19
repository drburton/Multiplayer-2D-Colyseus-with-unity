import { Room, Client, generateId } from "colyseus";
import { Schema, MapSchema, Context } from "@colyseus/schema";

// Create a context for this room's state data.
const type = Context.create();

export class Player extends Schema {
    @type("string") id: string;
    @type("string") ownerId: string;
    @type("number") xPos: number = 0;
    @type("number") yPos: number = 0;
    @type("string") sessionId: string;
    @type("boolean") connected: boolean;
}

class State extends Schema {
    @type({ map: Player }) players = new MapSchema<Player>();
  }

export class Chap1_2D extends Room {

    onCreate (options: any) {
      this.setState(new State());
  
      // make an enemy to be seen by player
      console.log("bot created");
      const enemy = new Player();
      enemy.id = "bot";
      enemy.ownerId = "bot";
      enemy.sessionId = generateId();
      enemy.xPos = 0;
      enemy.yPos = 0;
      enemy.connected = true;
      this.state.players[enemy.sessionId] = enemy;

      this.onMessage("move", (client, message) => {
      this.state.players[client.sessionId].xPos = message.xPos;
      this.state.players[client.sessionId].yPos = message.yPos;
      });
  
    }

    onJoin (client: Client, options: any) {
        console.log(client.sessionId, "joined!");
        console.log(JSON.stringify(client))
        this.state.players[client.sessionId] = new Player();
        this.state.players[client.sessionId].sessionId = client.sessionId;
      }
    
      async onLeave (client: Client, consented: boolean) {
        console.log(client.sessionId, "left!");
        this.state.players[client.sessionId].connected = false;
    
        try {
          if (consented) {
            throw new Error("consented leave!");
          }
    
          console.log("let's wait for reconnection!")
          const newClient = await this.allowReconnection(client, 10);
          console.log("reconnected!", newClient.sessionId)
        } catch (e) {
            console.log("disconnected!", client.sessionId);
            delete this.state.players[client.sessionId];
          }
        }
      
        onDispose() {
          console.log("room", this.roomId, "disposing...");
        }
      
    }
      
  
  