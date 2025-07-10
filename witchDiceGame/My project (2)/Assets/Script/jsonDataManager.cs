using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class jsonDataManager : MonoBehaviour
{

    private static jsonDataManager instance = null;
    private GameObject libraryMoneyDesc;
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
    private int[] witchPowerMoney = {0, 0, 0, 5000, 1000, 2000, 5000, 1000, 2000, 5000, 1000, 2000, 5000 };
    void Start()
    {
        string fileName = Path.Combine(Application.dataPath, "playerData.json");

        if (File.Exists(fileName)) {
            LoadPlayerFromJson();
        }
        else { 
            playerPlayData = new PlayerPlayData();
            SavePlayerDataToJson(); 
        }
        BattleManager.Instance.takeJsonWitchPower();

        libraryMoneyDesc = GameObject.Find("obj_library_money");
    }
    public void changeMoney(int a)
    {
        libraryMoneyDesc.GetComponent<TextMeshPro>().text = a.ToString();
    }

    public void SavePlayerDataToJson()
    {
        string jsonData = JsonUtility.ToJson(playerPlayData);
        string path = Path.Combine(Application.dataPath, "playerData.json");
        File.WriteAllText(path, jsonData);
    }
    public void LoadPlayerFromJson()
    {
        string fileName = Path.Combine(Application.dataPath, "playerData.json");
        if (File.Exists(fileName)) {
            string jsonFromFile = File.ReadAllText(fileName);
            PlayerPlayData temp = JsonUtility.FromJson<PlayerPlayData>(jsonFromFile);
            playerPlayData = new PlayerPlayData(temp);
        }
    }
    public int getMoney() { return playerPlayData.getMoney(); }
    public int getPowerPrice(int idx) { return witchPowerMoney[idx]; }
    public void addMoney(int addMoney) {

        playerPlayData.addMoney(addMoney);
        Debug.Log("buy third power! extra money :" + getMoney().ToString());
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
    public bool getMonsterSkill(int destinyIdx, int skillIdx)
    {
        if(skillIdx == 0) return this.playerPlayData.monsterSkill0Meet[destinyIdx - 10001];
        if (skillIdx == 1) return this.playerPlayData.monsterSkill1Meet[destinyIdx - 10001];
        return false;
    }
    public void meetMonsterSkill(int destinyIdx, int skillIdx)
    {
        if (getMonsterSkill(destinyIdx, skillIdx)) return;
        else {
            if(skillIdx == 0) this.playerPlayData.monsterSkill0Meet[destinyIdx - 10001] = true;
            if (skillIdx == 1) this.playerPlayData.monsterSkill1Meet[destinyIdx - 10001] = true;
            SavePlayerDataToJson();
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
}

public class PlayerPlayData{

    public int[] curWitchPower = new int[2];
    public int money = 0;
    public bool[] witchPower = new bool[100];
    public bool[] playCharacterAble = new bool[10000];
    public int[] playCharacterToken = new int[10000];
    public bool[] monsterSkill0Meet = new bool[10000];
    public bool[] monsterSkill1Meet = new bool[10000];
    public bool[] stageWatched = new bool[10000];
    public bool downGradeRevive = false;
    public bool tutorialRevive = false;
    public PlayerPlayData()
    {
        this.money = 0;
        curWitchPower[0] = 1;
        curWitchPower[1] = 2;
        this.witchPower[1] = true;
        this.witchPower[2] = true;
        for (int i = 3; i < witchPower.Length; i++) this.witchPower[i] = false;

        for (int i = 0; i < playCharacterAble.GetLength(0); i++){
            playCharacterAble[i] = false;
            playCharacterToken[i] = 0;
        }
        for (int i = 0; i < stageWatched.GetLength(0); i++)
        {
            stageWatched[i] = false;
        }
        for (int i = 0; i < monsterSkill0Meet.GetLength(0); i++){
            monsterSkill0Meet[i] = false;
            monsterSkill1Meet[i] = false;
        }
        downGradeRevive = false;
        tutorialRevive = false;
    }
    public PlayerPlayData(PlayerPlayData playerPlayerData)
    {
        this.money = playerPlayerData.money;
        curWitchPower[0] = playerPlayerData.curWitchPower[0];
        curWitchPower[1] = playerPlayerData.curWitchPower[1];
        for(int i=0;i<witchPower.Length; i++) this.witchPower[i] = playerPlayerData.witchPower[i];
        for (int i = 0; i < playCharacterAble.GetLength(0); i++)
        {
            playCharacterAble[i] = playerPlayerData.playCharacterAble[i];
            playCharacterToken[i] = playerPlayerData.playCharacterToken[i];
        }
        for (int i = 0; i < playCharacterAble.GetLength(0); i++)
        {
            monsterSkill0Meet[i] = playerPlayerData.monsterSkill0Meet[i];
            monsterSkill1Meet[i] = playerPlayerData.monsterSkill1Meet[i];
        }
        for (int i = 0; i < stageWatched.GetLength(0); i++)
        {
            stageWatched[i] = playerPlayerData.stageWatched[i];
        }
        downGradeRevive = playerPlayerData.downGradeRevive;
        tutorialRevive = playerPlayerData.tutorialRevive;
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
    public void addMoney(int a)
    {
        money += a;
        jsonDataManager.Instance.changeMoney(this.money);
    }
    public void minusMoney(int a) {
        money -= a;
        jsonDataManager.Instance.changeMoney(this.money);
    }

    public void addCharacterToken(int idx, int addVal) { //캐릭터 토큰 얻은 경우
        playCharacterToken[idx] += addVal;
    }

}
