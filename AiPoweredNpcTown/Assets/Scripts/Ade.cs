
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ade : MonoBehaviour
{
    private void OnMouseDown()
    {
        GetComponentInParent<EntityManager>().selectedObject = this.gameObject;
        GetComponentInParent<EntityManager>().showInfo = !GetComponentInParent<EntityManager>().showInfo;

    }

}

public class AdeEntity : FSM_Entity
{
    public int strength = 15;
    public int maxStrength = 15;

    public bool stew = false;
    public int eatingtime = 7;
    public int bored = 0;

    public AdeEntity(CustomTileMap _tileMap)
    {
        ID = "Ade";
        FSM_State b1 = new AdeSchoolingState();
        registerState(ref b1);
        FSM_State b2 = new AdeRestingState();
        registerState(ref b2);
        FSM_State b3 = new AdeEatingState();
        registerState(ref b3);
        FSM_State b4 = new AdeHangoutState();
        registerState(ref b4);
        FSM_State b5 = new AdeTicState();
        registerState(ref b5);

        currentState = b1;
        tileMap = _tileMap;

        tileMap.map[tileMap.coordToLinear(7, 1)].tileType = (int)TILETYPE.HOUSE;
        tileMap.map[tileMap.coordToLinear(7, 1)].totalValue = 500;
        tileMap.map[tileMap.coordToLinear(7, 1)].owner = "Sofia, Dave & Ade";

        tileMap.map[tileMap.coordToLinear(11, 5)].tileType = (int)TILETYPE.LIBRARY;
        tileMap.map[tileMap.coordToLinear(11, 5)].totalValue = 1000;
        tileMap.map[tileMap.coordToLinear(11, 5)].owner = "Public";

        tileMap.map[tileMap.coordToLinear(5, 7)].tileType = (int)TILETYPE.SCHOOL;
        tileMap.map[tileMap.coordToLinear(5, 7)].totalValue = 1000;
        tileMap.map[tileMap.coordToLinear(5, 7)].owner = "Ade";

        tileMap.map[tileMap.coordToLinear(7, 5)].tileType = (int)TILETYPE.HANGOUT_SPOT;
        tileMap.map[tileMap.coordToLinear(7, 5)].totalValue = 1000;
        tileMap.map[tileMap.coordToLinear(7, 5)].owner = "Ade & Friends";

        houseCoord[0] = 7;
        houseCoord[1] = 1;

        libraryCoord[0] = 11;
        libraryCoord[1] = 5;

        schoolCoord[0] = 5;
        schoolCoord[1] = 7;

        hangoutCoord[0] = 7;
        hangoutCoord[1] = 5;


    }

}

public class AdeTicState : FSM_State
{

    List<string> ticList = new List<string>();
    public AdeTicState()
    {
        ID = "Tic";
        ticList.Add("Gosh!");
        ticList.Add("jezz!");
        ticList.Add("darn!");
        ticList.Add("achew!");

    }

    public override void onEnter()
    {
        parent.dialogue += "I feel a tic coming on.\n";
    }

    public override void controlledUpdate()
    {
        int n = Random.Range(0, ticList.Count);
        parent.dialogue += ticList[n] + "\n";
        n = Random.Range(0, 2);
        if (n == 1)
            parent.returnToPreviousState();
    }

    public override void onExit()
    {
        parent.dialogue += "OK, I feel better now.\n";
    }
}

public class AdeSchoolingState : FSM_State
{
    public AdeSchoolingState()
    {
        ID = "Schooling";
    }

    public override void onEnter()
    {
        parent.destination = parent.schoolCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "Time to learn.\n";
    }

    public override void controlledUpdate()
    {
        //handle messages
        for (int i = 0; i < ((AdeEntity)parent).messageList.Count; i++)
        {
            if (((AdeEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_READY)
            {
                //the stew is finished
                ((AdeEntity)parent).messageList.RemoveAt(i);
                parent.changeState("Eating");
                return;
            }
        }

        int n = Random.Range(0, 15);
        if (n == 0)
        {
            //change tpo a blip state temporarily
            parent.changeToBlipState("Tic");
            return;
        }
        parent.dialogue += "I'm in school learning.\n";


        ((AdeEntity)parent).bored++;
        ((AdeEntity)parent).strength--;
        
        if (((AdeEntity)parent).strength <= 0)
        {
            ((AdeEntity)parent).strength = 0;
            parent.changeState("Resting");
                return;

        }
        else if (((AdeEntity)parent).bored >= 9)
        {
            parent.changeState("HangOut");
            return;

        }


    }

    public override void onExit()
    {
        parent.dialogue += "Time to leave school\n";
    }
}

public class AdeRestingState : FSM_State
{
    public AdeRestingState()
    {
        ID = "Resting";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I am soooooo tired.\n";
    }

    public override void controlledUpdate()
    {
        //handle messages
        for (int i = 0; i < ((AdeEntity)parent).messageList.Count; i++)
        {
            if (((AdeEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_READY)
            {
                //the stew is finished
                ((AdeEntity)parent).messageList.RemoveAt(i);
                parent.changeState("Eating");
                return;
            }
        }

        int n = Random.Range(0, 15);
        if (n == 0)
        {
            //change tpo a blip state temporarily
            parent.changeToBlipState("Tic");
            return;
        }
        parent.dialogue += "I'm resting.\n";

        ((AdeEntity)parent).strength++;

        if (((AdeEntity)parent).strength == ((AdeEntity)parent).maxStrength)
        {
            parent.changeState("HangOut");
            return;
        }

    }

    public override void onExit()
    {
        parent.dialogue += "I feel refreshed after resting for a while.\n";
    }
}

public class AdeHangoutState : FSM_State
{
    public AdeHangoutState()
    {
        ID = "HangOut";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I should go flex with my friends.\n";
    }

    public override void controlledUpdate()
    {
        //handle messages
        for (int i = 0; i < ((AdeEntity)parent).messageList.Count; i++)
        {
            if (((AdeEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_READY)
            {
                //the stew is finished
                ((AdeEntity)parent).messageList.RemoveAt(i);
                parent.changeState("Eating");
                return;
            }
        }

        int n = Random.Range(0, 15);
        if (n == 0)
        {
            //change tpo a blip state temporarily
            parent.changeToBlipState("Tic");
            return;
        }

        parent.dialogue += "I'm playing ball with my friends.\n";

        ((AdeEntity)parent).bored--;

        if (((AdeEntity)parent).bored <= 0)
        {
            parent.changeState("Schooling");
            return;
        }

    }

    public override void onExit()
    {
        parent.dialogue += "It was good playing ball out.\n";
    }
}

public class AdeEatingState : FSM_State
{
    public AdeEatingState()
    {
        ID = "Eating";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "Dad has cooked stew\n";
    }

    public override void controlledUpdate()
    {

        int n = Random.Range(0, 15);
        if (n == 0)
        {
            //change tpo a blip state temporarily
            parent.changeToBlipState("Tic");
            return;
        }
        ((AdeEntity)parent).eatingtime--;
        parent.dialogue += "Eating\n";
        if (((AdeEntity)parent).eatingtime <= 0)
        {
            ((AdeEntity)parent).eatingtime = 6;

            parent.sendMessage(new FSM_MESSAGE(parent.ID, "Elsa", (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN));
            if (((AdeEntity)parent).bored >= 9)
            {
                parent.changeState("HangOut");
                return;

            }
            else
            {
                parent.changeState("Schooling");
                return;
            }
        }

    }

    public override void onExit()
    {
        parent.dialogue += "I feel rested up, alrighty.\n";
    }
}

