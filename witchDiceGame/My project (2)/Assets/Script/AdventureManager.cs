using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class AdventureManager : MonoBehaviour
{

    public Canvas canvas_adventure;
    public Canvas canvas_battle;
    public CanvasGroup canvasGroupAdventure;
    public CanvasGroup canvasGroupBattle;

    public GameObject[] cameraArray = new GameObject[3];
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

        canvasGroupAdventure.alpha = 1.0f;
        canvas_adventure.enabled = true;
        canvasGroupBattle.alpha = 0.0f;
        canvas_battle.enabled = false;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            adventureFadeClick();
            
        }
        if (Input.GetKeyUp(KeyCode.S))
        {

            battleFadeClick();
        }
    }
    public int getWitchPower(int idx)
    {
        return witchPower[idx];
    }
    public void CanvasAdventureActive()
    {
        canvasGroupAdventure.alpha = 1.0f;
        canvasGroupAdventure.interactable = true;
        canvas_adventure.enabled = true;
        canvas_battle.enabled=false;
        
    }
    public void CanvasBattleActive()
    {
        canvasGroupBattle.alpha = 1.0f;
        canvasGroupBattle.interactable = true;
        canvas_battle.enabled = true;
        canvas_adventure.enabled = false;
        
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

    public void adventureFadeClick()
    {
        cameraArray[1].GetComponent<CinemachineVirtualCamera>().Priority = 10;
        cameraArray[0].GetComponent<CinemachineVirtualCamera>().Priority = 11;
        StartCoroutine(BattleDoFadeOut());
    }
    IEnumerator BattleDoFadeOut()
    {
        CanvasGroup canvasGroup = canvasGroupBattle;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 2 * Time.deltaTime;
            yield return null;
        }
        canvasGroup.interactable = false;
        yield return null;
        AdventureManager.Instance.CanvasAdventureActive();
    }

    public void battleFadeClick()
    {
        BattleManager.Instance.curPhase = -1;
        cameraArray[1].GetComponent<CinemachineVirtualCamera>().Priority = 11;
        cameraArray[0].GetComponent<CinemachineVirtualCamera>().Priority = 10;
        StartCoroutine(AdventureDoFadeOut());
    }
    IEnumerator AdventureDoFadeOut()
    {
        CanvasGroup canvasGroup = canvasGroupAdventure;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 2 * Time.deltaTime;
            yield return null;
        }
        canvasGroup.interactable = false;
        yield return null;
        AdventureManager.Instance.CanvasBattleActive();
    }

}
