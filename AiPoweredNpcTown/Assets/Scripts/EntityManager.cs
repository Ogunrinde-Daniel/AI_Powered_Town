using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public float Delay = 2f;
    private float delayCount = 0f;
    int counter = 0;
    public int noOFLoops = 200;
    public bool showInfo;
    public GameObject selectedObject;

    SpawnWorldLand worldLand;
    FSM_EntityManager globalEntityManager;

    List<GameObject> entities = new List<GameObject>();

    public GameObject Bob;
    public GameObject Elsa;
    public GameObject Anna;
    public GameObject Sofia;
    public GameObject Dave;
    public GameObject Ade;
    public TextMeshProUGUI aboutText;
    public Transform dialogueScroll;
    void Start()
    {
        Delay = GameData.delay;
        noOFLoops = GameData.loopTimes;
        worldLand = FindObjectOfType<SpawnWorldLand>();
        globalEntityManager = new FSM_EntityManager();

        FSM_Entity bobEntity = new BobEntity(worldLand.tileMap);
        FSM_Entity elsaEntity = new ElsaEntity(worldLand.tileMap);
        FSM_Entity annaEntity = new AnnaEntity(worldLand.tileMap);
        FSM_Entity sofiaEntity = new SofiaEntity(worldLand.tileMap);
        FSM_Entity daveEntity = new DaveEntity(worldLand.tileMap);
        FSM_Entity adeEntity = new AdeEntity(worldLand.tileMap);


        globalEntityManager.registerEntity(ref bobEntity);
        globalEntityManager.registerEntity(ref elsaEntity);
        globalEntityManager.registerEntity(ref annaEntity);
        globalEntityManager.registerEntity(ref sofiaEntity);
        globalEntityManager.registerEntity(ref daveEntity);
        globalEntityManager.registerEntity(ref adeEntity);

        entities.Add(Bob);
        entities.Add(Elsa);
        entities.Add(Anna);
        entities.Add(Sofia);
        entities.Add(Dave);
        entities.Add(Ade);

        worldLand.reloadMap();


    }
    
    private void FixedUpdate()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            Vector2 posInWorld = worldLand.tileMap.getPosition(globalEntityManager.entityMap.ElementAt(i).Value.positionCoord[0], globalEntityManager.entityMap.ElementAt(i).Value.positionCoord[1]);
            if (posInWorld != Vector2.zero)
                entities[i].transform.position = posInWorld;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


        delayCount -= Time.deltaTime;
        if (delayCount > 0f)
        {
            return;
        }
        delayCount = Delay;
        //display state
        int selectedIndex = entities.IndexOf(selectedObject);
        if (selectedIndex < 0)
            selectedIndex = 0;
        aboutText.text = globalEntityManager.entityMap.ElementAt(selectedIndex).Value.about();
        dialogueScroll.GetComponentInChildren<TextMeshProUGUI>().text = globalEntityManager.entityMap.ElementAt(selectedIndex).Value.dialogue + "\n";

        aboutText.gameObject.SetActive(showInfo);
        dialogueScroll.gameObject.SetActive(showInfo);

        if (counter < noOFLoops)
        {
            globalEntityManager.controlledUpdate();
            counter++;
        }

    }
}

