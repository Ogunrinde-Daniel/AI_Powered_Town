
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dave : MonoBehaviour
{
    private void OnMouseDown()
    {
        GetComponentInParent<EntityManager>().selectedObject = this.gameObject;
        GetComponentInParent<EntityManager>().showInfo = !GetComponentInParent<EntityManager>().showInfo;

    }
}

public class DaveEntity : FSM_Entity
{
    public int strength = 15;
    public int maxStrength = 15;

    public bool stew = false;
    public int clean = 0;
    public int cooking = 0;
    public int bored = 0;

    public DaveEntity(CustomTileMap _tileMap)
    {
        ID = "Dave";
        FSM_State b1 = new DaveCleaningState();
        registerState(ref b1);
        FSM_State b2 = new DaveBoredState();
        registerState(ref b2);
        FSM_State b3 = new DaveCookingState();
        registerState(ref b3);
        FSM_State b4 = new DaveSleepingState();
        registerState(ref b4);
        FSM_State b5 = new DaveTicState();
        registerState(ref b5);

        currentState = b1;
        tileMap = _tileMap;

        tileMap.map[tileMap.coordToLinear(7, 1)].tileType = (int)TILETYPE.HOUSE;
        tileMap.map[tileMap.coordToLinear(7, 1)].totalValue = 500;
        tileMap.map[tileMap.coordToLinear(7, 1)].owner = "Sofia & Dave";

        tileMap.map[tileMap.coordToLinear(11, 5)].tileType = (int)TILETYPE.LIBRARY;
        tileMap.map[tileMap.coordToLinear(11, 5)].totalValue = 1000;
        tileMap.map[tileMap.coordToLinear(11, 5)].owner = "Public";

        houseCoord[0] = 7;
        houseCoord[1] = 1;

        libraryCoord[0] = 11;
        libraryCoord[1] = 5;

    }

}

public class DaveTicState : FSM_State
{
    
	List<string> ticList = new List<string>();
    public DaveTicState()
    {
        ID = "Tic";
        ticList.Add("Ouch!");
        ticList.Add("Fjku!");
        ticList.Add("iich!");
        ticList.Add("Duh!");
        ticList.Add("Urgh!");
        ticList.Add("BRUH!");
    }

    public override void onEnter()
    {
        parent.dialogue += "I feel a tic coming on.\n";
    }

    public override void controlledUpdate()
    {
        int n = Random.Range(0,ticList.Count);
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

public class DaveCleaningState : FSM_State
{
    public DaveCleaningState()
    {
        ID = "Cleaning";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "This place is disgusting! I should clean it up.\n";
    }

    public override void controlledUpdate()
    {
        //handle messages
        for (int i = 0; i < ((DaveEntity)parent).messageList.Count; i++)
        {
            if (((DaveEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN)
            {
                //the stew is finished
                ((DaveEntity)parent).stew = false;
                ((DaveEntity)parent).messageList.RemoveAt(i);
            }
        }


        int n = Random.Range(0, 15);
        if (n == 0)
        {
            //change tpo a blip state temporarily
            parent.changeToBlipState("Tic");
            return;
        }
        parent.dialogue += "I'm sweeping the floor.\n";

        
        ((DaveEntity)parent).clean++;
        ((DaveEntity)parent).bored++;
        ((DaveEntity)parent).strength--;
        if (((DaveEntity)parent).strength < 0)
        {
            ((DaveEntity)parent).strength = 0;
        }

        if (((DaveEntity)parent).clean > 10)
        {
            if (((DaveEntity)parent).stew == false)
                parent.changeState("Cooking");
            else if (((DaveEntity)parent).strength < 0)
                parent.changeState("Sleeping");
            else
                parent.changeState("Bored");
            return;

        }


    }

    public override void onExit()
    {
        parent.dialogue += "There! That looks better.\n";
    }
}

public class DaveBoredState : FSM_State
{
    public DaveBoredState()
    {
        ID = "Bored";
    }

    public override void onEnter()
    {
        parent.destination = parent.libraryCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I am soooooo bored.\n";
    }

    public override void controlledUpdate()
    {
        //handle messages
        for (int i = 0; i < ((DaveEntity)parent).messageList.Count; i++)
        {
            if (((DaveEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN)
            {
                //the stew is finished
                ((DaveEntity)parent).stew = false;
                ((DaveEntity)parent).messageList.RemoveAt(i);
            }
        }

        int n = Random.Range(0, 15);
        if (n == 0)
        {
            //change tpo a blip state temporarily
            parent.changeToBlipState("Tic");
            return;
        }
        parent.dialogue += "I'm reading a book.\n";

        ((DaveEntity)parent).bored--;
       

        if (((DaveEntity)parent).bored < 1)
        {
            if (((DaveEntity)parent).stew == false)
            {
                parent.changeState("Cooking");
                return;
            }
            parent.changeState("Cleaning");
            return;
        }

        

    }

    public override void onExit()
    {
        parent.dialogue += "I feel refreshed after reading for a while.\n";
    }
}

public class DaveCookingState : FSM_State
{
    public DaveCookingState()
    {
        ID = "Cooking";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I should cook a stew.\n";
    }

    public override void controlledUpdate()
    {
        //handle messages
        for (int i = 0; i < ((DaveEntity)parent).messageList.Count; i++)
        {
            if (((DaveEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN)
            {
                //the stew is finished
                ((DaveEntity)parent).stew = false;
                ((DaveEntity)parent).messageList.RemoveAt(i); 
            }
        }


        int n = Random.Range(0, 15);
        if (n == 0)
        {
            //change tpo a blip state temporarily
            parent.changeToBlipState("Tic");
            return;
        }

        parent.dialogue += "I'm cooking something delicious.\n";

        ((DaveEntity)parent).cooking++;
        ((DaveEntity)parent).clean--;
        ((DaveEntity)parent).strength--;

        if (((DaveEntity)parent).clean < 0){
            ((DaveEntity)parent).clean = 0;
        }
        if (((DaveEntity)parent).strength < 0)
        {
            ((DaveEntity)parent).strength = 0;
        }
        if (((DaveEntity)parent).cooking > 7)
        {
            ((DaveEntity)parent).cooking = 0;
            ((DaveEntity)parent).stew = true;

            //tell everyone food is ready
            parent.sendMessage(new FSM_MESSAGE(parent.ID, "Sofia", (int)MESSAGE_LIST.MESSAGE_SUPPER_READY));
            parent.sendMessage(new FSM_MESSAGE(parent.ID, "Ade", (int)MESSAGE_LIST.MESSAGE_SUPPER_READY));



            if (((DaveEntity)parent).strength < 0)
            {
                parent.changeState("Sleeping");
                return;
            }


            parent.changeState("Bored");
            return;

        }

    }

    public override void onExit()
    {
        parent.dialogue += "A yummy yummy stew is ready.\n";
    }
}

public class DaveSleepingState : FSM_State
{
    public DaveSleepingState()
    {
        ID = "Sleeping";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I'm pretty tired. I should go rest.\n";
    }

    public override void controlledUpdate()
    {
        //handle messages
        for (int i = 0; i < ((DaveEntity)parent).messageList.Count; i++)
        {
            if (((DaveEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN)
            {
                //the stew is finished
                ((DaveEntity)parent).stew = false;
                ((DaveEntity)parent).messageList.RemoveAt(i);
            }
        }
        int n = Random.Range(0, 15);
        if (n == 0)
        {
            //change tpo a blip state temporarily
            parent.changeToBlipState("Tic");
            return;
        }
        parent.dialogue += "Zzzzzzzzzz\n";
        ((DaveEntity)parent).strength++;
        if (((DaveEntity)parent).strength >= ((DaveEntity)parent).maxStrength)
        {
            if (((DaveEntity)parent).stew == false)
            {
                parent.changeState("Cooking");
                return;
            }

            if (((DaveEntity)parent).clean <= 0)
            {
                parent.changeState("Cleaning");
                return;
            }
            parent.changeState("Bored");
            return;
        }



    }

    public override void onExit()
    {
        parent.dialogue += "I feel rested up, alrighty.\n";
    }
}

