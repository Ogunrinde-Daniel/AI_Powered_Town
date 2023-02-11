using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;


public enum MESSAGE_LIST { MESSAGE_SUPPER_READY = 1, MESSAGE_SUPPER_EATEN };

public class FSMBASE
{

}

public class FSM_EntityManager
{
    public Dictionary<string, FSM_Entity> entityMap = new Dictionary<string, FSM_Entity>();
    public List<FSM_MESSAGE> messageList = new List<FSM_MESSAGE>();

    public void FSM_SendMessage(FSM_MESSAGE message)
    {
        messageList.Add(message);
    }
    public void registerEntity(ref FSM_Entity entity)
    {
        entity.parent = this;
        entityMap[entity.ID] = entity; //adds a new entity
    }
    public void controlledUpdate()
    {
        //Message Handling
        //loop backwards so new messages can be added as old messages are being deleted
        for (int i = messageList.Count - 1; i >= 0; i--)
        {
            if (messageList[i].receiverID == "BROADCAST")
            {
                UnityEngine.Debug.Log("Global Message:: " + ((MESSAGE_LIST)messageList[i].message).ToString());

                //send message to every entity except the original sender
                for (int j = 0; j < entityMap.Count; j++)
                {
                    if (entityMap.ElementAt(j).Key != messageList[i].senderID)
                        entityMap.ElementAt(j).Value.recieveMessage(messageList[i]);
                }

            }
            else
            {
                var reciever = entityMap.ContainsKey(messageList[i].receiverID) ? entityMap[messageList[i].receiverID] : null;
                if (reciever != null)
                {
                    reciever.recieveMessage(messageList[i]);
                }

            }
            messageList.RemoveAt(i);
        }

        //Entity Update
        for (int i = 0; i < entityMap.Count; i++)
            entityMap.ElementAt(i).Value.controlledUpdate();
    }


}


public class FSM_Entity 
{
    public int[] positionCoord = { 0, 0 };
    public int[] destination = {0,0 };
    public Stack<int[]> path;
    public CustomTileMap tileMap;
    public int[] houseCoord = {0,0 };
    public int[] marketCoord = { 0, 0 };
    public int[] libraryCoord = {0,0};
    public int[] farmCoord = { 0, 0 };
    public int[] bankCoord = { 0, 0 };
    public int[] schoolCoord = { 0, 0 };
    public int[] hangoutCoord = { 0, 0 };



    public FSM_EntityManager parent;

    public FSM_State currentState;
    public FSM_State previousState;
    public string ID = "";
    public string name = "";
    public string netWorth = "";

    public string dialogue = "Dialogue\n";

    public List<FSM_MESSAGE> messageList = new List<FSM_MESSAGE>();
    public Dictionary<string, FSM_State> stateMap = new Dictionary<string, FSM_State>();
    public void changeState(string stateID)
    {
        previousState = currentState;
        previousState.onExit();

        currentState = stateMap.ContainsKey(stateID) ? stateMap[stateID] : null;
        currentState.onEnter();

    }
    public void registerState(ref FSM_State state)
    {
        state.parent = this;
        stateMap[state.ID] = state; //adds a new state
    }
    public void changeToBlipState(string stateID)
    {
        previousState = currentState;
        currentState = stateMap.ContainsKey(stateID) ? stateMap[stateID] : null;

        currentState.onEnter();
    }
    public void returnToPreviousState()
    {
        FSM_State temp = currentState;
        currentState.onExit();
        currentState = previousState;
        previousState = temp;
    }
    public void recieveMessage(FSM_MESSAGE message)
    {
        messageList.Add(message);
    }
    public void sendMessage(FSM_MESSAGE message)
    {
        parent.FSM_SendMessage(message);
    }
    public void controlledUpdate()
    {
        if (dialogue.Length > 1000000) dialogue = "";   //prevent dialogue from overflowing
        if(!isMoving())currentState.controlledUpdate();
        move();
    }
    public bool isMoving()
    {
        return (destination[0] != positionCoord[0] || destination[1] != positionCoord[1]);
    }
    public void move()
    {
        if (!isMoving())
            return;
        if (path != null && path.Count > 0)
        {
            int[] nodes = path.Pop();
            positionCoord[0] = nodes[0];
            positionCoord[1] = nodes[1];
        }
    }    
    public string about()
    {
        string about = "";
        about += "Name: " + name + "\n";
        about += "ID: " + ID + "\n";    
        about += "Networth: " + netWorth + "\n";
        about += "Current State: " + currentState.ID + "\n";
        about += "States::" + "\n";

        foreach (var s in stateMap.Keys)
        {
            about += "\t" + s + "\n";
        }

        return about;

    }

    /*
    public void move()
    {
        if (destination[0] == positionCoord[0] && destination[1] == positionCoord[1])
            return;

        if (nearDestination())
            return;

        //check path
        bool rightFree = tileMap.isTileRoad(positionCoord[0]+ 1, positionCoord[1]);
        bool downFree =  tileMap.isTileRoad(positionCoord[0], positionCoord[1] + 1);
        bool leftFree =  tileMap.isTileRoad(positionCoord[0] - 1, positionCoord[1]);
        bool upFree =    tileMap.isTileRoad(positionCoord[0], positionCoord[1] - 1);

        //move right
        if (positionCoord[0] < destination[0] && rightFree)
            positionCoord[0]++;
        //move down
        else if (positionCoord[1] < destination[1] && downFree)
            positionCoord[1]++;
        //move left
        else if (positionCoord[0] > destination[0] && leftFree)
            positionCoord[0]--;
        //move up
        else if (positionCoord[1] > destination[1] && upFree)
            positionCoord[1]--;
    }
    
    public bool nearDestination()
    {
        //check if close by
        bool rightExists = tileMap.isTileInBounds(positionCoord[0] + 1, positionCoord[1]);
        bool downExists = tileMap.isTileInBounds(positionCoord[0], positionCoord[1] + 1);
        bool leftExists = tileMap.isTileInBounds(positionCoord[0] - 1, positionCoord[1]);
        bool upExists = tileMap.isTileInBounds(positionCoord[0], positionCoord[1] - 1);
        //snap right
        if (positionCoord[1] == destination[1] && rightExists
            && positionCoord[0] - destination[0] <= -1)
            positionCoord[0] = destination[0];

        //snap left
        if (positionCoord[1] == destination[1] && leftExists
            && positionCoord[0] - destination[0] <= 1)
            positionCoord[0] = destination[0];

        //snap top
        if (positionCoord[0] == destination[0] && upExists
            && positionCoord[1] - destination[1] <= 1)
            positionCoord[1] = destination[1];

        //snap down
        if (positionCoord[0] == destination[0] && downExists
            && positionCoord[1] - destination[1] <= -1)
            positionCoord[1] = destination[1];

        return (positionCoord[0] == destination[0] && positionCoord[1] == destination[1]);
    }
    */


}

public class FSM_MESSAGE
{
    public string senderID;
    public string receiverID;
    public int message;
    public int count = 0;
    public FSM_MESSAGE(string senderID, string receiverID, int message)
    {
        this.senderID = senderID;
        this.receiverID = receiverID;
        this.message = message;
    }   
}


public class FSM_State
{
    public FSM_Entity parent;
    public string ID = "";

    public virtual void controlledUpdate() { }
    public virtual void onEnter() { }
    public virtual void onExit() { }

}


