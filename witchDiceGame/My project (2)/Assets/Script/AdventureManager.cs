using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class AdventureManager : MonoBehaviour
{


    private static AdventureManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static AdventureManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }


    private int stageNum = 0; //몇번째 스테이지인지 받는다.
    private int stageIdx = 0; //이번 스테이지에서 몇번째 맵인지(1-1 1-2의 개념) 
    private int[] witchPower = new int[2];

    private int eventWatchNum = 0; //이벤트 선택지 볼때 쓰는 숫자
    private int selectDiceNum = -1; //현재 선택된 주사위
    private int[] adventureEventArr = new int[21]; //앞으로 남은 이벤트들에 대한 정보
    //전투 : 0  주사위 굴리기 이벤트 : 1 


    private GameObject stageInfo; //현재 스테이지의 level과 스테이지 정보를 담는 text
    private GameObject selectInfo;
    private GameObject eventInfo;
    private GameObject selectImage;
    private GameObject[] textObject = new GameObject[2]; // 주사위 굴렸을때 결과를 처리하기 위해 사용한다. 
    private GameObject[] diceObject = new GameObject[4];

    public List<adventureEvent> adventureEventList = new List<adventureEvent>(); //

    public List<AdventureEventReader> adventureEventReaderList = new List<AdventureEventReader>(); // 
    public List<AdventureEventPacketReader> adventureEventPacketReaderList = new List<AdventureEventPacketReader>(); // 

    GameObject mainCamera;

    private adventureEvent curDiceEvent;
    private adventureEvent_Packet curDiceEventPacket;

    bool curCanvasIsAdventure = true;
    // Start is called before the first frame update
    void Start()
    {
        for (int i=0;i<4;i++)
        {
            diceObject[i] = GameObject.Find("adventure_dice_" + i.ToString());
        }
        mainCamera = GameObject.Find("Main Camera");
        selectInfo = GameObject.Find("adventure_selectInfo");
        eventInfo = GameObject.Find("adventure_eventInfo");
        selectImage = GameObject.Find("adventure_selectDice");
        
        stageNum = 1;
        stageIdx = 1;
        stageInfo = GameObject.Find("adventure_stageInfo");
        stageInfo.GetComponent<TextMeshPro>().text = "Stage : " +  stageNum + "\nLevel : "+ stageIdx;
        selectInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "\nLevel : " + stageIdx;

        AdventureEventPacketReader[] tempList =  new AdventureEventPacketReader[6];

        adventureEventReaderList = CSVReader.Read<AdventureEventReader>("Event");
        adventureEventPacketReaderList = CSVReader.Read<AdventureEventPacketReader>("EventPacket");
        
        for (int eventIdx =0; eventIdx < adventureEventReaderList.Count; eventIdx++) //Reader 2개를 병합 시켜 하나의 event를 만들어 list에 추가
        {
            for (int packetIdx = 0; packetIdx < 6; packetIdx++) //각 이벤트 당 6개의 packet을 받는다.
            {
                tempList[packetIdx] = adventureEventPacketReaderList[eventIdx * 6 + packetIdx];
            }
            adventureEventList.Add(new adventureEvent(adventureEventReaderList[eventIdx], tempList)); //packet과 event 내용을 받은 event 리스트 생성
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void makeStageEventArr() //이번 스테이지의 나타나는 이벤트의 종류를 미리 배치한다.
    {
        for (int i=0;i<20;i++)
        {
            adventureEventArr[i] = 1; //지금은 모두 주사위 이벤트가 나오도록 설정. 나중에는 전투, 주사위 이벤트 등 어떤게 나올지 랜덤하게(단 전투가 많이) 나오게 수정해야한다.
        }
    }
    public void startAdventure()
    {
        //지금은 시작 버튼 누르면 바로 시작
        StartCoroutine(phase_Manage_Coroutine());
    }

    private int eventIndexReturn() //어떤 이벤트가 나올지 이후 지정할 필요가 있다. 현재는 0번째 이벤트밖에 나오지 않지만, 나중에는 해당 스테이지에 해당된 랜덤한 이벤트가 나오도록 해야함.
    {
        return 0;
    }
    private IEnumerator phase_Manage_Coroutine()
    {
        makeStageEventArr(); //이번 스테이지의 나타나는 이벤트의 종류를 미리 배치한다.
        stageIdx = 1;
        // 스테이지 끝 혹은 주사위 이벤트가 끝날때까지 유지되도록 (StartCoroutine이랑 하나 계속 돌아가게 하는 것중 뭐가 더 비용 비싼지 확인할것) 살려두는게 쌀것 같긴함.
        while (stageIdx<20)
        {
            stageInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "\nLevel : " + stageIdx;
            if (adventureEventArr[stageIdx] == 1) { //주사위 이벤트 일경우 해당 이벤트 진행. 

                eventWatchNum = 0;
                curDiceEvent = new adventureEvent(adventureEventList[eventIndexReturn()]); //랜덤한 이벤트를 받아온다.

                selectInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getPacket(eventWatchNum).getChooseText(); //선택지 텍스트 변경
                eventInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getSelectText(); // 이벤트 텍스트 내용 변경
                selectImage.transform.rotation = Quaternion.Euler(0, 0, 0);
                selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + (eventWatchNum + 1).ToString());
                selectDiceNum = 0; //고를 수 있는 상태로 변경
                
                yield return new WaitUntil(() => selectDiceNum > 0); // 주사위 쓸 영웅 선택 대기

                eventWatchNum = selectDiceNum - 1;
                curDiceEventPacket = curDiceEvent.getPacket(eventWatchNum);
                selectInfo.GetComponent<TextMeshPro>().text = curDiceEventPacket.getChooseText();//선택지 텍스트 변경
                eventInfo.GetComponent<TextMeshPro>().text = curDiceEventPacket.getResultText();
                if(curDiceEventPacket.getSelectType() == 6) //전투를 진행하는 경우
                {
                    for (int i=0;i<4;i++)
                    {
                        if (curDiceEventPacket.getSelectType() != -99999) CharacterManager.Instance.setCharacter(0, curDiceEventPacket.getVal(i));
                        else CharacterManager.Instance.emptyEnemyCharacter(i);
                    }
                    enterBattleCanvas();
                    yield return new WaitUntil(() => curCanvasIsAdventure); //돌아올때까지 대기
                    eventInfo.GetComponent<TextMeshPro>().text = "You Win! Go to Next Level";
                }
                
                yield return new WaitForSeconds(1f); //잠시 대기
                selectDiceNum = -1; // 선택 못하게 변경
                eventWatchNum = -1;

            }

            stageIdx++;
        }
    }

    public void changeSelectNum(bool upEvent)
    { //현재 아래 방향이 상승
        if (eventWatchNum < 6 && eventWatchNum >= 0)
        {
            if (!upEvent)
            {
                if (eventWatchNum == 5) eventWatchNum = 0;
                else eventWatchNum++;
            }
            else
            {
                if (eventWatchNum == 0) eventWatchNum = 5;
                else eventWatchNum--;
            }
            //회전도 적용(나중에 마녀 능력을 위해서)
            selectInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getPacket(eventWatchNum).getChooseText();//선택지 텍스트 변경
            selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + (eventWatchNum+1).ToString());
        }
        
    }

    public void clickDice(int characterIdx)
    {
        if (selectDiceNum == 0 && CharacterManager.Instance.getCharacterState(characterIdx) == 0)
        {
            CharacterManager.Instance.throwDice(characterIdx);
            selectImage.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
            selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterIdx).ToString());
            
            diceObject[characterIdx].transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
            diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterIdx).ToString());
            selectDiceNum = CharacterManager.Instance.getDiceNum(characterIdx);
        }
    } 

    //가방, 전투 페이즈 입장을 위한 함수들
    public void enterUpgradeCanvas()
    {
        curCanvasIsAdventure = false;
        itemManager.Instance.click_upgradeCanvas_start();
        mainCamera.transform.position = new Vector3(-1000f, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }
    public void exitUpgradeCanvas()
    {
        curCanvasIsAdventure = true;
        mainCamera.transform.position = new Vector3(-500f, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }
    public void enterBattleCanvas()
    {
        curCanvasIsAdventure = false;
        BattleManager.Instance.startBattle_fromAdventure();
        mainCamera.transform.position = new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }
    public void exitBattleCanvas()
    {
        curCanvasIsAdventure = true;
        mainCamera.transform.position = new Vector3(-500f, mainCamera.transform.position.y, mainCamera.transform.position.z);
    }

    public int getWitchPower(int idx)
    {
        return witchPower[idx];
    }

    public void stageClear()
    {
        stageIdx++;
        if (stageIdx == 10)
        {
            stageIdx = 0;
            stageNum++;
        }
    }

}
