
using System;
using TMPro;
using UnityEngine;

public class Bob : MonoBehaviour
{
    private void OnMouseDown()
    {
        GetComponentInParent<EntityManager>().selectedObject = this.gameObject;
        GetComponentInParent<EntityManager>().showInfo = !GetComponentInParent<EntityManager>().showInfo;

    }

}

public class BobEntity : FSM_Entity
{
    public int gold = 0;
    public int strength = 15;
    public int maxStrength = 15;
    public int eatingtime = 7;

    public BobEntity(CustomTileMap _tileMap)
    {
        ID = "Bob";
        FSM_State b1 = new BobMiningState();
        registerState(ref b1);
        FSM_State b2 = new BobSleepingState();
        registerState(ref b2);
        FSM_State b3 = new BobBankingState();
        registerState(ref b3);
        FSM_State b4 = new BobEatingState();
        registerState(ref b4);

        currentState = b1;
        tileMap = _tileMap;

        tileMap.map[tileMap.coordToLinear(1, 1)].tileType = (int)TILETYPE.HOUSE;
        tileMap.map[tileMap.coordToLinear(1, 1)].totalValue = 500;
        tileMap.map[tileMap.coordToLinear(1, 1)].owner = "Bob & Elsa";

        tileMap.map[tileMap.coordToLinear(5, 1)].tileType = (int)TILETYPE.FARM;
        tileMap.map[tileMap.coordToLinear(5, 1)].totalValue = 2000;
        tileMap.map[tileMap.coordToLinear(5, 1)].owner = "Bob";

        tileMap.map[tileMap.coordToLinear(5, 5)].tileType = (int)TILETYPE.BANK;
        tileMap.map[tileMap.coordToLinear(5, 5)].totalValue = 7000;
        tileMap.map[tileMap.coordToLinear(5, 5)].owner = "Bob";



        houseCoord[0] = 1;
        houseCoord[1] = 1;

        farmCoord[0] = 5;
        farmCoord[1] = 1;

        bankCoord[0] = 5;
        bankCoord[1] = 5;

    }

}

public class BobMiningState : FSM_State 
{
    public BobMiningState()
    {
        ID = "Mining";
    }

    public override void onEnter()
    {
        parent.destination = parent.farmCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I'm gonna mine for gold.\n";
    }

    public override void controlledUpdate()
    {
        parent.dialogue += "I mined a nugget!\n";

        ((BobEntity)parent).gold++;
        ((BobEntity)parent).strength--;
        if (((BobEntity)parent).strength <= 0)
        {
            parent.changeState("Sleeping");
            return;
        }

        if (((BobEntity)parent).gold > 10)
        {
            parent.changeState("Banking");
            return;
        }

    }

    public override void onExit()
    {
        parent.dialogue += "I'm leaving the mine.\n";
    }
}

public class BobSleepingState : FSM_State
{
    public BobSleepingState()
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
        parent.dialogue += "Zzzzzzzzzz\n";
        ((BobEntity)parent).strength++;
        if (((BobEntity)parent).strength >= ((BobEntity)parent).maxStrength)
        {
            if (((BobEntity)parent).gold > 10)
            {
                parent.changeState("Banking");
                return;
            }
            parent.changeState("Mining");
            return;
        }

        

    }

    public override void onExit()
    {
        parent.dialogue += "I feel rested up, alrighty.\n";
    }
}

public class BobBankingState : FSM_State
{
    public BobBankingState()
    {
        ID = "Banking";
    }

    public override void onEnter()
    {
        parent.destination = parent.bankCoord;
        UnityEngine.Debug.Log(parent.destination[0] + " " + parent.destination[1]);
        UnityEngine.Debug.Log(parent.positionCoord[0] + " " + parent.positionCoord[1]);
        /*
        char[][] array = (parent.tileMap.mapToChar(parent.positionCoord, parent.destination));
        string arr ="";
        for (int i = 0; i < array.Length; i++)
        {
            for (int j = 0; j < array[i].Length; j++)
            {
                arr += (array[i][j] + " ");
            }
            arr += ("\n");

        }
        Debug.Log(arr);
        */
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "Mah pockets is full, I'm a'heading to the bank.\n";
    }

    public override void controlledUpdate()
    {
        parent.dialogue += "Deposited all mah gold inna bank.Yeehaw, I'm rich!\n";
        //implement bank
        ((BobEntity)parent).gold = 0;
        
        parent.changeState("Mining");
        return;

    }

    public override void onExit()
    {
        parent.dialogue += "Time to leave the bank.\n";
    }
}

public class BobEatingState : FSM_State
{
    public BobEatingState()
    {
        ID = "Eating";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I'm hungry, let's go eat. Elsa has cooked\n";
    }

    public override void controlledUpdate()
    {
        ((BobEntity)parent).eatingtime--;
        parent.dialogue += "Yumm, this food sure is gewd!\n";
        if (((BobEntity)parent).eatingtime <= 0)
        {
            ((BobEntity)parent).eatingtime = 7;
            parent.sendMessage(new FSM_MESSAGE(parent.ID, "Elsa", (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN));
            parent.changeState("Mining");
            return;
        }
    }

    public override void onExit()
    {
        parent.dialogue += "<burp!> I feel pretty full now.\n";
    }
}


