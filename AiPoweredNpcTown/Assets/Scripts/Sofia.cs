
using TMPro;
using UnityEngine;

public class Sofia : MonoBehaviour
{
    private void OnMouseDown()
    {
        GetComponentInParent<EntityManager>().selectedObject = this.gameObject;
        GetComponentInParent<EntityManager>().showInfo = !GetComponentInParent<EntityManager>().showInfo;

    }

}

public class SofiaEntity : FSM_Entity
{
    public int gold = 0;
    public int strength = 15;
    public int maxStrength = 15;
    public int eatingtime = 7;

    public SofiaEntity(CustomTileMap _tileMap)
    {
        ID = "Sofia";
        FSM_State b1 = new SofiaSellingState();
        registerState(ref b1);
        FSM_State b2 = new SofiaSleepingState();
        registerState(ref b2);
        FSM_State b3 = new SofiaBankingState();
        registerState(ref b3);
        FSM_State b4 = new SofiaEatingState();
        registerState(ref b4);

        currentState = b1;
        tileMap = _tileMap;

        tileMap.map[tileMap.coordToLinear(7, 1)].tileType = (int)TILETYPE.HOUSE;
        tileMap.map[tileMap.coordToLinear(7, 1)].totalValue = 500;
        tileMap.map[tileMap.coordToLinear(7, 1)].owner = "Sofia & Dave";

        tileMap.map[tileMap.coordToLinear(11, 1)].tileType = (int)TILETYPE.MARKET;
        tileMap.map[tileMap.coordToLinear(11, 1)].totalValue = 2000;
        tileMap.map[tileMap.coordToLinear(11, 1)].owner = "Sofia";

        tileMap.map[tileMap.coordToLinear(7, 1)].tileType = (int)TILETYPE.BANK;
        tileMap.map[tileMap.coordToLinear(7, 1)].totalValue = 7000;
        tileMap.map[tileMap.coordToLinear(7, 1)].owner = "Sofia";



        houseCoord[0] = 7;
        houseCoord[1] = 1;

        marketCoord[0] = 11;
        marketCoord[1] = 1;

        bankCoord[0] = 7;
        bankCoord[1] = 5;

    }

}

public class SofiaSellingState : FSM_State 
{
    public SofiaSellingState()
    {
        ID = "Selling";
    }

    public override void onEnter()
    {
        parent.destination = parent.marketCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I'm gonna sell some stuffs.\n";
    }

    public override void controlledUpdate()
    {
        parent.dialogue += "I sold a share!\n";

        ((SofiaEntity)parent).gold++;
        ((SofiaEntity)parent).strength--;
        if (((SofiaEntity)parent).strength <= 0)
        {
            parent.changeState("Sleeping");
            return;
        }

        if (((SofiaEntity)parent).gold > 10)
        {
            parent.changeState("Banking");
            return;
        }

    }

    public override void onExit()
    {
        parent.dialogue += "I'm leaving the market.\n";
    }
}

public class SofiaSleepingState : FSM_State
{
    public SofiaSleepingState()
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
        ((SofiaEntity)parent).strength++;
        if (((SofiaEntity)parent).strength >= ((SofiaEntity)parent).maxStrength)
        {
            if (((SofiaEntity)parent).gold > 10)
            {
                parent.changeState("Banking");
                return;
            }
            parent.changeState("Selling");
            return;
        }

        

    }

    public override void onExit()
    {
        parent.dialogue += "I feel rested up, alrighty.\n";
    }
}

public class SofiaBankingState : FSM_State
{
    public SofiaBankingState()
    {
        ID = "Banking";
    }

    public override void onEnter()
    {
        parent.destination = parent.bankCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "Mah pockets is full, I'm a'heading to the bank.\n";
    }

    public override void controlledUpdate()
    {
        parent.dialogue += "Deposited all mah gold inna bank.Yeehaw, I'm rich!\n";
        //implement bank
        ((SofiaEntity)parent).gold = 0;
        
        parent.changeState("Selling");
        return;

    }

    public override void onExit()
    {
        parent.dialogue += "Time to leave the bank.\n";
    }
}

public class SofiaEatingState : FSM_State
{
    public SofiaEatingState()
    {
        ID = "Eating";
    }

    public override void onEnter()
    {
        parent.destination = parent.houseCoord;
        parent.path = parent.tileMap.getPath(parent.positionCoord, parent.destination);
        parent.dialogue += "I'm hungry, let's go eat. Dave has cooked\n";
    }

    public override void controlledUpdate()
    {
        ((SofiaEntity)parent).eatingtime--;
        parent.dialogue += "Yumm, my husband is a good cook\n";
        if (((SofiaEntity)parent).eatingtime <= 0)
        {
            ((SofiaEntity)parent).eatingtime = 7;
            parent.sendMessage(new FSM_MESSAGE(parent.ID, "Dave", (int)MESSAGE_LIST.MESSAGE_SUPPER_EATEN));
            parent.changeState("Selling");
            return;
        }
    }

    public override void onExit()
    {
        parent.dialogue += "<ahh!> I feel pretty full now.\n";
    }
}


