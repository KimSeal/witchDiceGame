using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class optionManager : MonoBehaviour
{
    private static optionManager instance = null;
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

    private int optionIdx = 0;
    //0: language 1: screen Size

    [SerializeField]
    public GameObject optionBoard;
    [SerializeField]
    public GameObject[] languageBtn = new GameObject[3];
    [SerializeField]
    public GameObject[] screenSizeBtn = new GameObject[5];
    [SerializeField]
    public GameObject optionBackBoard;
    [SerializeField]
    public GameObject[] optionBoards = new GameObject[2];

    public static optionManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        optionIdx = 0;
        optionBoard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activeOptionBoard()
    {
        optionBoard.SetActive(true);
        optionBackBoard.SetActive(true);
        optionBackBoard.transform.position = new Vector3(CameraManager.Instance.cameraPointX(), CameraManager.Instance.cameraPointY(), CameraManager.Instance.cameraPointZ());
        for (int i = 0; i < optionBoards.Length; i++)
        {
            if (i == optionIdx) optionBoards[i].SetActive(true);
            else optionBoards[i].SetActive(false);
        }

        if (optionIdx == 0) changeLanguage(jsonDataManager.Instance.getLanguage());
        if (optionIdx == 1) changeScreenSize(jsonDataManager.Instance.getScreenSize());
    }
    public void unactiveOptionBoard()
    {
        optionBoard.SetActive(false);
        optionBackBoard.SetActive(false);
    }

    public void changeOption(int idx)
    {
        optionIdx = idx;
        for (int i = 0; i < optionBoards.Length; i++) { 
            if(idx==i) optionBoards[i].SetActive(true);
            else optionBoards[i].SetActive(false);
        }
        if (idx == 0){changeLanguage(jsonDataManager.Instance.getLanguage());}
        if (idx == 1) changeScreenSize(jsonDataManager.Instance.getScreenSize());
    }

    public void changeLanguage(int idx)
    {
        if (optionIdx == 0)
        {
            Debug.Log(idx);
            jsonDataManager.Instance.setLanguage(idx);
            for (int i = 0; i < languageBtn.Length; i++)
            {
                if (idx == i)
                {
                    languageBtn[i].GetComponent<hoverRotateUI>().setLanguageActive(true);
                }
                else
                {
                    languageBtn[i].GetComponent<hoverRotateUI>().setLanguageActive(false);
                }
            }
        }
    }

    public void changeScreenSize(int idx) {
        if (optionIdx == 1) {
            CameraManager.Instance.changeScreenSize(idx);
            for (int i = 0; i < screenSizeBtn.Length; i++)
            {
                if (idx == i)
                {
                    screenSizeBtn[i].GetComponent<hoverRotateUI>().setLanguageActive(true);
                }
                else
                {
                    screenSizeBtn[i].GetComponent<hoverRotateUI>().setLanguageActive(false);
                }
            }
        }
    }
}
