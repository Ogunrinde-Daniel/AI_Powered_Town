
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Elsa : MonoBehaviour
{
    private void OnMouseDown()
    {
        GetComponentInParent<EntityManager>().selectedObject = this.gameObject;
        GetComponentInParent<EntityManager>().showInfo = !GetComponentInParent<EntityManager>().showInfo;

    }
}

public class ElsaEntity : FSM_Entity
{
    public int strength = 15;
    public int maxStrength = 15;

    public bool stew = false;
    public int clean = 0;
    public int cooking = 0;
    public int bored = 0;

    public ElsaEntity(CustomTileMap _tileMap)
    {
        ID = "Elsa";
        FSM_State b1 = new ElsaCleaningState();
        registerState(ref b1);
        FSM_State b2 = new ElsaBoredState();
        registerState(ref b2);
        FSM_State b3 = new ElsaCookingState();
        registerState(ref b3);
        FSM_State b4 = new ElsaSleepingState();
        registerState(ref b4);
        FSM_State b5 = new ElsaTicState();
        registerState(ref b5);

        currentState = b1;
        tileMap = _tileMap;

        tileMap.map[tileMap.coordToLinear(1, 1)].tileType = (int)TILETYPE.HOUSE;
        tileMap.map[tileMap.coordToLinear(1, 1)].totalValue = 500;
        tileMap.map[tileMap.coordToLinear(1, 1)].owner = "Bob & Elsa";

        tileMap.map[tileMap.coordToLinear(7, 7)].tileType = (int)TILETYPE.LIBRARY;
        tileMap.map[tileMap.coordToLinear(7, 7)].totalValue = 1000;
        tileMap.map[tileMap.coordToLinear(7, 7)].owner = "Public";

        houseCoord[0] = 1;
        houseCoord[1] = 1;

        libraryCoord[0] = 7;
        libraryCoord[1] = 7;

    }

}

public class ElsaTicState : FSM_State
{
    
	List<string> ticList = new List<string>();
    public ElsaTicState()
    {
        ID = "Tic";
        ticList.Add("Feck!");
        ticList.Add("Darn!");
        ticList.Add("Urk!");
        ticList.Add("Bah!");
        ticList.Add("Urgh!");
        ticList.Add("BALDERDASH!");
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

public class ElsaCleaningState : FSM_State
{
    public ElsaCleaningState()
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
        for (int i = 0; i < ((ElsaEntity)parent).messageList.Count; i++)
        {
            if (((ElsaEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN)
            {
                //the stew is finished
                ((ElsaEntity)parent).stew = false;
                ((ElsaEntity)parent).messageList.RemoveAt(i);
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

        
        ((ElsaEntity)parent).clean++;
        ((ElsaEntity)parent).bored++;
        ((ElsaEntity)parent).strength--;
        if (((ElsaEntity)parent).strength < 0)
        {
            ((ElsaEntity)parent).strength = 0;
        }

        if (((ElsaEntity)parent).clean > 10)
        {
            if (((ElsaEntity)parent).stew == false)
                parent.changeState("Cooking");
            else if (((ElsaEntity)parent).strength < 0)
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

public class ElsaBoredState : FSM_State
{
    public ElsaBoredState()
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
        for (int i = 0; i < ((ElsaEntity)parent).messageList.Count; i++)
        {
            if (((ElsaEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN)
            {
                //the stew is finished
                ((ElsaEntity)parent).stew = false;
                ((ElsaEntity)parent).messageList.RemoveAt(i);
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

        ((ElsaEntity)parent).bored--;
       

        if (((ElsaEntity)parent).bored < 1)
        {
            if (((ElsaEntity)parent).stew == false)
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

public class ElsaCookingState : FSM_State
{
    public ElsaCookingState()
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
        for (int i = 0; i < ((ElsaEntity)parent).messageList.Count; i++)
        {
            if (((ElsaEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN)
            {
                //the stew is finished
                ((ElsaEntity)parent).stew = false;
                ((ElsaEntity)parent).messageList.RemoveAt(i); 
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

        ((ElsaEntity)parent).cooking++;
        ((ElsaEntity)parent).clean--;
        ((ElsaEntity)parent).strength--;

        if (((ElsaEntity)parent).clean < 0){
            ((ElsaEntity)parent).clean = 0;
        }
        if (((ElsaEntity)parent).strength < 0)
        {
            ((ElsaEntity)parent).strength = 0;
        }
        if (((ElsaEntity)parent).cooking > 7)
        {
            ((ElsaEntity)parent).cooking = 0;
            ((ElsaEntity)parent).stew = true;

            //tell everyone food is ready
            parent.sendMessage(new FSM_MESSAGE(parent.ID, "BROADCAST", (int)MESSAGE_LIST.MESSAGE_SUPPER_READY));


            if (((ElsaEntity)parent).strength < 0)
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

public class ElsaSleepingState : FSM_State
{
    public ElsaSleepingState()
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
        for (int i = 0; i < ((ElsaEntity)parent).messageList.Count; i++)
        {
            if (((ElsaEntity)parent).messageList[i].message == (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN)
            {
                //the stew is finished
                ((ElsaEntity)parent).stew = false;
                ((ElsaEntity)parent).messageList.RemoveAt(i);
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
        ((ElsaEntity)parent).strength++;
        if (((ElsaEntity)parent).strength >= ((ElsaEntity)parent).maxStrength)
        {
            if (((ElsaEntity)parent).stew == false)
            {
                parent.changeState("Cooking");
                return;
            }

            if (((ElsaEntity)parent).clean <= 0)
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

