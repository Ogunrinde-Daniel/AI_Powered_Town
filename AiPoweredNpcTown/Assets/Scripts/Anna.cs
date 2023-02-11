
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Anna : MonoBehaviour
{
    private void OnMouseDown()
    {
        GetComponentInParent<EntityManager>().selectedObject = this.gameObject;
        GetComponentInParent<EntityManager>().showInfo = !GetComponentInParent<EntityManager>().showInfo;

    }

}

public class AnnaEntity : FSM_Entity
{
    public int strength = 15;
    public int maxStrength = 15;

    public bool stew = false;
    public int eatingtime = 7;
    public int bored = 0;

    public AnnaEntity(CustomTileMap _tileMap)
    {
        ID = "Anna";
        FSM_State b1 = new AnnaSchoolingState();
        registerState(ref b1);
        FSM_State b2 = new AnnaRestingState();
        registerState(ref b2);
        FSM_State b3 = new AnnaEatingState();
        registerState(ref b3);
        FSM_State b4 = new AnnaHangoutState();
        registerState(ref b4);
        FSM_State b5 = new AnnaTicState();
        registerState(ref b5);

        currentState = b1;
        tileMap = _tileMap;

        tileMap.map[tileMap.coordToLinear(1, 1)].tileType = (int)TILETYPE.HOUSE;
        tileMap.map[tileMap.coordToLinear(1, 1)].totalValue = 500;
        tileMap.map[tileMap.coordToLinear(1, 1)].owner = "Bob, Elsa & Anna";

        tileMap.map[tileMap.coordToLinear(7, 7)].tileType = (int)TILETYPE.LIBRARY;
        tileMap.map[tileMap.coordToLinear(7, 7)].totalValue = 1000;
        tileMap.map[tileMap.coordToLinear(7, 7)].owner = "Public";

        tileMap.map[tileMap.coordToLinear(1, 5)].tileType = (int)TILETYPE.SCHOOL;
        tileMap.map[tileMap.coordToLinear(1, 5)].totalValue = 1000;
        tileMap.map[tileMap.coordToLinear(1, 5)].owner = "Anna";

        tileMap.map[tileMap.coordToLinear(1, 7)].tileType = (int)TILETYPE.HANGOUT_SPOT;
        tileMap.map[tileMap.coordToLinear(1, 7)].totalValue = 1000;
        tileMap.map[tileMap.coordToLinear(1, 7)].owner = "Anna & Friends";

        houseCoord[0] = 1;
        houseCoord[1] = 1;

        libraryCoord[0] = 7;
        libraryCoord[1] = 7;

        schoolCoord[0] = 1;
        schoolCoord[1] = 5;

        hangoutCoord[0] = 1;
        hangoutCoord[1] = 7;


    }

}

public class AnnaTicState : FSM_State
{

    List<string> ticList = new List<string>();
    public AnnaTicState()
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

public class AnnaSchoolingState : FSM_State
{
    public AnnaSchoolingState()
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
        for (int i = 0; i < ((AnnaEntity)parent).messageList.Count; i++)
        {
            if (((AnnaEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_READY)
            {
                //the stew is finished
                ((AnnaEntity)parent).messageList.RemoveAt(i);
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


        ((AnnaEntity)parent).bored++;
        ((AnnaEntity)parent).strength--;
        
        if (((AnnaEntity)parent).strength <= 0)
        {
            ((AnnaEntity)parent).strength = 0;
            parent.changeState("Resting");
                return;

        }
        else if (((AnnaEntity)parent).bored >= 9)
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

public class AnnaRestingState : FSM_State
{
    public AnnaRestingState()
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
        for (int i = 0; i < ((AnnaEntity)parent).messageList.Count; i++)
        {
            if (((AnnaEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_READY)
            {
                //the stew is finished
                ((AnnaEntity)parent).messageList.RemoveAt(i);
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

        ((AnnaEntity)parent).strength++;

        if (((AnnaEntity)parent).strength == ((AnnaEntity)parent).maxStrength)
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

public class AnnaHangoutState : FSM_State
{
    public AnnaHangoutState()
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
        for (int i = 0; i < ((AnnaEntity)parent).messageList.Count; i++)
        {
            if (((AnnaEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_READY)
            {
                //the stew is finished
                ((AnnaEntity)parent).messageList.RemoveAt(i);
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

        parent.dialogue += "I'm hanging out with my friends.\n";

        ((AnnaEntity)parent).bored--;

        if (((AnnaEntity)parent).bored <= 0)
        {
            parent.changeState("Schooling");
            return;
        }

    }

    public override void onExit()
    {
        parent.dialogue += "It was good hanging out.\n";
    }
}

public class AnnaEatingState : FSM_State
{
    public AnnaEatingState()
    {
        ID = "Eating";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        UnityEngine.Debug.Log(parent.destination[0] + " " + parent.destination[1]);
        UnityEngine.Debug.Log(parent.positionCoord[0] + " " + parent.positionCoord[1]);
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "Mom has cooked stew\n";
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
        ((AnnaEntity)parent).eatingtime--;
        parent.dialogue += "Eating\n";
        if (((AnnaEntity)parent).eatingtime <= 0)
        {
            ((AnnaEntity)parent).eatingtime = 6;

            parent.sendMessage(new FSM_MESSAGE(parent.ID, "Elsa", (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN));
            if (((AnnaEntity)parent).bored >= 9)
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

