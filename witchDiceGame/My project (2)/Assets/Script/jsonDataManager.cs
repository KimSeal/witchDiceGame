using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class jsonDataManager : MonoBehaviour
{

    private static jsonDataManager instance = null;
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

    public static jsonDataManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }
    // Start is called before the first frame update
    private PlayerPlayData playerPlayData;
    private int[] witchPowerMoney = {0, 0, 0, 500, 100, 200, 500, 100, 200, 500, 100, 200, 500 };
    private string jsonFileName = "playerData3.json";
    void Start()
    {
        string fileName = Path.Combine(Application.persistentDataPath, jsonFileName);
        string preFileName = Path.Combine(Application.dataPath, jsonFileName);

        if (!File.Exists(fileName))
        {
            playerPlayData = new PlayerPlayData();
            SoundManager_Main.Instance.setBackgroundVolume(1.0f);
            if (File.Exists(preFileName))
            {
                LoadPlayerFromJson_pre();

            }
            SavePlayerDataToJson();
        }
        else
        {
            LoadPlayerFromJson();
        }

        optionManager.Instance.changeOption(1);
        optionManager.Instance.changeOption(2);
        optionManager.Instance.changeOption(0);

        if (getChapterRead(1, 2) != 2) {
            setCharacterSelect(1,0);
        }

        //TalkManager.Instance.setDescIdx(39);

    }
    public void changeMoney(int a)
    {
        //libraryMoneyDesc.GetComponent<TextMeshPro>().text = "$ "+ a.ToString();
    }

    public void SavePlayerDataToJson_pre()
    {
        string jsonData = JsonUtility.ToJson(playerPlayData);
        string path = Path.Combine(Application.dataPath, jsonFileName);
        File.WriteAllText(path, jsonData);
    }
    public void SavePlayerDataToJson()
    {
        string jsonData = JsonUtility.ToJson(playerPlayData);
        string path = Path.Combine(Application.persistentDataPath, jsonFileName);
        File.WriteAllText(path, jsonData);
    }
    public void LoadPlayerFromJson_pre()
    {
        string fileName = Path.Combine(Application.dataPath, jsonFileName);
        if (File.Exists(fileName))
        {
            string jsonFromFile = File.ReadAllText(fileName);
            PlayerPlayData temp = JsonUtility.FromJson<PlayerPlayData>(jsonFromFile);
            playerPlayData = new PlayerPlayData(temp);
        }
        SoundManager_Main.Instance.setBackgroundVolume(getBackgroundVol());
        SoundManager_Sfx.Instance.setSFXVolume(getSFXVol());
    }
    public void LoadPlayerFromJson()
    {
        string fileName = Path.Combine(Application.persistentDataPath, jsonFileName);
        if (File.Exists(fileName)) {
            string jsonFromFile = File.ReadAllText(fileName);
            PlayerPlayData temp = JsonUtility.FromJson<PlayerPlayData>(jsonFromFile);
            playerPlayData = new PlayerPlayData(temp);
        }
        SoundManager_Main.Instance.setBackgroundVolume(getBackgroundVol());
        SoundManager_Sfx.Instance.setSFXVolume(getSFXVol());
    }

    public float getBackgroundVol() { return this.playerPlayData.getBackgroundVol(); }
    public void setBackgroundVol(float input) { 
        this.playerPlayData.setBackgroundVol(input);
        SoundManager_Main.Instance.setBackgroundVolume(input);
        SavePlayerDataToJson(); 
    }

    public float getSFXVol() { return this.playerPlayData.getSFXVol(); }
    public void setSFXVol(float input) {
        SoundManager_Sfx.Instance.setSFXVolume(input);
        this.playerPlayData.setSFXVol(input); 
        SavePlayerDataToJson(); 
    }


    public int getScreenSize()
    {
        return playerPlayData.getScreenSize();
    }
    public void setScreenSize(int idx) { 
        playerPlayData.setScreenSize(idx);
        SavePlayerDataToJson();
    } 

    public int getMoney() { return playerPlayData.getMoney(); }
    public int getPowerPrice(int idx) { return witchPowerMoney[idx]; }
    public void addMoney(int addMoney) {

        playerPlayData.addMoney(addMoney);
        Debug.Log("buy third power! extra money :" + getMoney().ToString());
        SavePlayerDataToJson();
    }
    public void setMoney(int money) { 
        playerPlayData.setMoney(money);
        SavePlayerDataToJson();
    }
    public int getPlayerCharacterToken(int destinyIdx)
    {
        return playerPlayData.playCharacterToken[destinyIdx];
    }
    public bool getPlayerCharacterAble(int destinyIdx)
    {
        return playerPlayData.playCharacterAble[destinyIdx];
    }
    public void setPlayerCharacterAble(int destinyIdx)
    {
        Debug.Log("seve character Destiny" + destinyIdx.ToString());
        playerPlayData.playCharacterAble[destinyIdx] = true;
        SavePlayerDataToJson();
    }
    //마녀 능력 구매 관련
    public int checkWitchPower(int powerIdx)
    {
        if (this.playerPlayData.witchPower[powerIdx]) return 0; //이미 가지고 있으면 변경

        if (this.playerPlayData.getMoney() >= witchPowerMoney[powerIdx]) {//비소유지만 구매 가능한 경우
            return 1;
        }
        return 2; // 비소유면서 구매도 불가능한 경우
    }
    public bool buyWitchPower(int powerIdx) {
        if (!this.playerPlayData.witchPower[powerIdx] && this.playerPlayData.getMoney() >= witchPowerMoney[powerIdx]) //아직 구매안했고 돈이 있을 경우 구매
        {
            this.playerPlayData.witchPower[powerIdx] = true;
            this.playerPlayData.minusMoney(witchPowerMoney[powerIdx]);
            SavePlayerDataToJson();
            return true;
        }
        return false; //이미 있거나 돈 부족한 경우 구매 X
    }
    public void buyTest()
    {
        if (this.buyWitchPower(3)) {
            Debug.Log("buy third power! extra money :" + getMoney().ToString());
        }
        else
        {
            Debug.Log("no you cant buy  extra money :" + getMoney().ToString());
        }
    }

    public int getCurWitchPower(int idx)
    {
        return playerPlayData.curWitchPower[idx];
    }
    public void changeWitchPower(int idx1, int idx2)
    {
        playerPlayData.curWitchPower[0] = idx1;
        playerPlayData.curWitchPower[1] = idx2;
        SavePlayerDataToJson();
    }

    public void addCharacterToken(int idx, int addVal) {
        playerPlayData.addCharacterToken(idx, addVal);
        jsonDataManager.Instance.SavePlayerDataToJson();
    }

    public bool getStageWatched(int i)
    {
        return playerPlayData.getStageWatched(i);
    }
    public void setStageWatched(int i)
    {
        playerPlayData.setStageWatched(i);
        jsonDataManager.Instance.SavePlayerDataToJson();
    }
    public void downGradeRevive()
    {
        if (!playerPlayData.downGradeRevive) {
            TalkManager.Instance.startTalk(14);
            playerPlayData.downGradeRevive = true;
            SavePlayerDataToJson();
        }
    }
    public void tutorialRevive()
    {
        if (!playerPlayData.tutorialRevive)
        {
            TalkManager.Instance.startTalk(15);
            playerPlayData.tutorialRevive = true;
            SavePlayerDataToJson();
        }
    }
    // 0 - 0 : 아무것도 안함. 1 :처음 들어감 2: 중간 보스 클리어 3: 최종 보스 클리어 4: 올빼미 선배 조우 5: 올빼미 선배 쓰러트림 6: 기억 모두 봄.
    public int getChapterDid(int idx)
    {
        return playerPlayData.chapterDid[idx];
    }
    
    public bool setChapterDid(int chapterIdx, int num)
    {
        if (playerPlayData.chapterDid[chapterIdx] < num)
        {
            playerPlayData.chapterDid[chapterIdx] = num;
            SavePlayerDataToJson();
            return true;
        }
        return false;
    }
    public bool getTutorialDid()
    {
        return playerPlayData.getTutorialDid();
    }
    public void tutorialDid()
    {
        if (!playerPlayData.tutorialDid)
        {
            playerPlayData.tutorialDid = true;
            SavePlayerDataToJson();
        }
    }
    public bool getTowerMeet()
    {
        return playerPlayData.towerMeet;
    }
    public void towerMeet()
    {
        if (!playerPlayData.towerMeet)
        {
            playerPlayData.towerMeet = true;
            SavePlayerDataToJson();
        }
    }
    public bool getTowerEntry()
    {
        return playerPlayData.towerEntry;
    }
    public void towerEntry()
    {
        if (!playerPlayData.towerEntry) { 
            playerPlayData.towerEntry = true;
            SavePlayerDataToJson();
        }
    }
    
    public bool getLibraryMeet()
    {
        return playerPlayData.libraryMeet;
    }
    public bool getHomeMeet()
    {
        return playerPlayData.homeMeet;
    }
    public void HomeMeet()
    {
        if (!playerPlayData.homeMeet)
        {
            playerPlayData.homeMeet = true;
            SavePlayerDataToJson();
        }
    }
    public void libraryMeet()
    {
        if (!playerPlayData.libraryMeet)
        {
            playerPlayData.libraryMeet = true;
            SavePlayerDataToJson();
        }
    }
    public bool getOwlBattleWin()
    {
        return playerPlayData.owlBattleWin;
    }
    public void owlBattleWin()
    {
        if (!playerPlayData.owlBattleWin) { 
            playerPlayData.owlBattleWin = true;
            SavePlayerDataToJson();
        }
    }
    public bool getFirstGetCharacterPart()
    {
        return playerPlayData.firstGetCharacterPart;
    }
    public void firstGetCharacterPart()
    {
        if (!playerPlayData.getFirstGetCharacterPart())
        {
            playerPlayData.firstGetCharacterPart = true;
            SavePlayerDataToJson();
        }
    }

    public int getCurChapter()
    {
        if (playerPlayData.chapter1Read[2] == 2) return 1;
        if (playerPlayData.chapter2Read[2] == 2) return 2;
        if (playerPlayData.chapter3Read[2] == 2) return 3;
        if (playerPlayData.chapter4Read[2] == 2) return 4;
        if (playerPlayData.chapter5Read[2] == 2) return 5;
        if (playerPlayData.chapter6Read[2] == 2) return 6;
        return 0;
    }
    public int getChapterRead(int chapterIdx, int idx)
    {
        if (chapterIdx == 0) return playerPlayData.chapter1Read[idx];
        if (chapterIdx == 1) return playerPlayData.chapter2Read[idx];
        if (chapterIdx == 2) return playerPlayData.chapter3Read[idx];
        if (chapterIdx == 3) return playerPlayData.chapter4Read[idx];
        if (chapterIdx == 4) return playerPlayData.chapter5Read[idx];
        if (chapterIdx == 5) return playerPlayData.chapter6Read[idx];
        else return 0;
    }
    public void setChapterRead(int chapterIdx, int idx) {
        if (chapterIdx == 0) playerPlayData.chapter1Read[idx]++;
        if (chapterIdx == 1) playerPlayData.chapter2Read[idx]++;
        if (chapterIdx == 2) playerPlayData.chapter3Read[idx]++;
        if (chapterIdx == 3) playerPlayData.chapter4Read[idx]++;
        if (chapterIdx == 4) playerPlayData.chapter5Read[idx]++;
        if (chapterIdx == 5) playerPlayData.chapter6Read[idx]++;
        SavePlayerDataToJson();

    }
    public int getLanguage()
    {
        return this.playerPlayData.language;
    }
    public void setLanguage(int lan)
    {
        this.playerPlayData.setLanguage(lan);
        SavePlayerDataToJson();
    }
    public int getCharacterSelect(int idx)
    {
        return this.playerPlayData.getCharacterSelect(idx);
    }
    public void setCharacterSelect(int idx, int val) { 
        this.playerPlayData.setCharacterSelect(idx, val);
        SavePlayerDataToJson();
    }
    public bool getEventMeet(int idx) { if (idx < 8) return false;
        return playerPlayData.eventMeet[idx]; }
    public void setEventMeet(int idx) { playerPlayData.eventMeet[idx] = true; SavePlayerDataToJson(); }

    public void recordArchiveMeet()
    {
        playerPlayData.recordArchiveMeet = true;
        SavePlayerDataToJson();
    }
    public bool getRecordArchiveMeet()
    {
        return playerPlayData.recordArchiveMeet;
    }
    public bool getFoodStreetMeet()
    {
        return playerPlayData.foodStreetMeet;
    }
    public void foodStreetMeet()
    {
        playerPlayData.foodStreetMeet = true;
        SavePlayerDataToJson();
    }
    public int getFoodStreetStat(int idx) { return playerPlayData.getFoodStreetStat(idx); }
    public void setFoodStreetStat(int idx, int val) { playerPlayData.setFoodStreetStat(idx, val); SavePlayerDataToJson(); }

    public class PlayerPlayData
    {
        public int language;
        public int screenSize = 1;
        public float backgroundVolume = 1.0f;
        public float SFXVolume = 1.0f;
        public int[] curWitchPower = new int[2];
        public int money = 0;
        public bool[] witchPower = new bool[100];
        public bool[] playCharacterAble = new bool[100];
        public int[] playCharacterToken = new int[100];
        public bool[] stageWatched = new bool[100];
        public bool downGradeRevive = false;
        public bool tutorialRevive = false;
        public bool tutorialDid = false;
        public bool homeMeet = false;
        public bool towerMeet = false;
        public bool towerEntry = false;
        public bool libraryMeet = false;
        public bool owlBattleWin = false;
        public bool firstGetCharacterPart = false;
        public int[] chapterDid = new int[6];
        public int[] chapter1Read = new int[3]; // 1챕터 각 스토리 대응. int값이 0이면 미해금. 1이면 스토리 막 개방 2면 스토리 종료
        public int[] chapter2Read = new int[3];
        public int[] chapter3Read = new int[3];
        public int[] chapter4Read = new int[3];
        public int[] chapter5Read = new int[3];
        public int[] chapter6Read = new int[3];

        public int[] characterSelect = new int[2];
        public bool[] eventMeet = new bool[300];
        public bool recordArchiveMeet = false;

        public bool foodStreetMeet = false;
        public int[] foodStreetStat = new int[4]; //음식 거리에서 분배된 스탯값.
        public int getFoodStreetStat(int idx)
        {
            return foodStreetStat[idx];
        }
        public void setFoodStreetStat(int idx, int val)
        {
            this.foodStreetStat[idx] = val;
        }

        public void setCharacterSelect(int idx, int val)
        {
            characterSelect[idx] = val;
        }
        public int getCharacterSelect(int idx) { return characterSelect[idx]; }

        public void setBackgroundVol(float input) {  backgroundVolume = input;}
        public float getBackgroundVol(){ return backgroundVolume; }
        public void setSFXVol(float input) { SFXVolume = input; }
        public float getSFXVol() { return SFXVolume; }

        public int getChapterDid(int idx)
        {
            return chapterDid[idx];
        }
        public int getScreenSize()
        {
            return screenSize;
        }
        public void setScreenSize(int idx)
        {
            screenSize = idx;
        }
        public void setLanguage(int language)
        {
            this.language = language;
        }
        public void setChapterDid(int idx, int num)
        {
            if(chapterDid[idx] < num) chapterDid[idx] = num;
        }
        public PlayerPlayData()
        {
            this.recordArchiveMeet = false;
            this.screenSize = 1;
            this.language = 1;
            this.money = 0;
            this.SFXVolume = 1.0f;
            this.backgroundVolume = 1.0f;
            curWitchPower[0] = 1;
            curWitchPower[1] = 2;
            this.witchPower[1] = true;
            this.witchPower[2] = true;
            for (int i = 3; i < witchPower.Length; i++) this.witchPower[i] = false;

            playCharacterAble[0] = true;
            playCharacterToken[0] = 1;
            for (int i = 1; i < playCharacterAble.GetLength(0); i++)
            {
                playCharacterAble[i] = false;
                playCharacterToken[i] = 0;
            }
            for (int i = 0; i < stageWatched.GetLength(0); i++)
            {
                stageWatched[i] = false;
            }
            for(int i=0;i<eventMeet.Length;i++) eventMeet[i] = false;
            downGradeRevive = false;
            tutorialRevive = false;
            tutorialDid = false;
            homeMeet = false;
            towerMeet = false;
            towerEntry = false;
            libraryMeet = false;
            owlBattleWin = false;
            firstGetCharacterPart = false;
            for (int i = 0; i < chapterDid.Length; i++) chapterDid[i] = 0;
            for (int i = 0; i < 3; i++)
            {
                chapter1Read[i] = 0;
                chapter2Read[i] = 0;
                chapter3Read[i] = 0;
                chapter4Read[i] = 0;
                chapter5Read[i] = 0;
                chapter6Read[i] = 0;
            }
            characterSelect[0] = 0; characterSelect[1] = 0;
            foodStreetMeet = false;
            for (int i=0;i<foodStreetStat.Length;i++) foodStreetStat[i] = 0;
        }
        public PlayerPlayData(PlayerPlayData playerPlayerData)
        {
            this.recordArchiveMeet = playerPlayerData.recordArchiveMeet;
            this.screenSize = playerPlayerData.screenSize;
            this.money = playerPlayerData.money;

            this.SFXVolume = playerPlayerData.SFXVolume;
            this.backgroundVolume = playerPlayerData.backgroundVolume;

            curWitchPower[0] = playerPlayerData.curWitchPower[0];
            curWitchPower[1] = playerPlayerData.curWitchPower[1];
            for (int i = 0; i < witchPower.Length; i++) this.witchPower[i] = playerPlayerData.witchPower[i];
            playCharacterAble[0] = true;
            playCharacterToken[0] = 1;
            for (int i = 1; i < playCharacterAble.GetLength(0); i++)
            {
                playCharacterAble[i] = playerPlayerData.playCharacterAble[i];
                playCharacterToken[i] = playerPlayerData.playCharacterToken[i];
            }
            for (int i = 0; i < stageWatched.GetLength(0); i++)
            {
                stageWatched[i] = playerPlayerData.stageWatched[i];
            }
            for (int i = 0; i < eventMeet.Length; i++) eventMeet[i] = playerPlayerData.eventMeet[i];
            downGradeRevive = playerPlayerData.downGradeRevive;
            tutorialRevive = playerPlayerData.tutorialRevive;
            tutorialDid = playerPlayerData.tutorialDid;
            homeMeet = playerPlayerData.homeMeet;
            towerMeet = playerPlayerData.towerMeet;
            towerEntry = playerPlayerData.towerEntry;
            libraryMeet = playerPlayerData.libraryMeet;
            owlBattleWin = playerPlayerData.owlBattleWin;
            firstGetCharacterPart = playerPlayerData.firstGetCharacterPart;
            this.language = playerPlayerData.language;
            for (int i = 0; i < chapterDid.Length; i++) chapterDid[i] = playerPlayerData.chapterDid[i];
            for (int i = 0; i < 3; i++)
            {
                chapter1Read[i] = playerPlayerData.chapter1Read[i];
                chapter2Read[i] = playerPlayerData.chapter2Read[i];
                chapter3Read[i] = playerPlayerData.chapter3Read[i];
                chapter4Read[i] = playerPlayerData.chapter4Read[i];
                chapter5Read[i] = playerPlayerData.chapter5Read[i];
                chapter6Read[i] = playerPlayerData.chapter6Read[i];
            }
            characterSelect[0] = playerPlayerData.characterSelect[0]; 
            characterSelect[1] = playerPlayerData.characterSelect[1];
            foodStreetMeet = playerPlayerData.foodStreetMeet;
            for (int i = 0; i < foodStreetStat.Length; i++) foodStreetStat[i] = playerPlayerData.foodStreetStat[i];
        }
        public bool getFirstGetCharacterPart()
        {
            return firstGetCharacterPart;
        }
        public bool getTutorialDid()
        {
            return tutorialDid;
        }
        public void setTutorialDid()
        {
            tutorialDid = true;
        }

        public bool getStageWatched(int i)
        {
            return stageWatched[i];
        }
        public bool setStageWatched(int i)
        {
            return stageWatched[i] = true;
        }
        public int getMoney()
        {
            return money;
        }
        public void setMoney(int a) {
            money = a;
            jsonDataManager.Instance.changeMoney(this.money);
        }
        public void addMoney(int a)
        {
            money += a;
            jsonDataManager.Instance.changeMoney(this.money);
        }
        public void minusMoney(int a)
        {
            money -= a;
            jsonDataManager.Instance.changeMoney(this.money);
        }

        public void addCharacterToken(int idx, int addVal)
        { //캐릭터 토큰 얻은 경우
            playCharacterToken[idx] += addVal;
        }
    }

}
