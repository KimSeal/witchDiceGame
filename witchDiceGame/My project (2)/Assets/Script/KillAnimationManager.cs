using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAnimationManager : MonoBehaviour
{


    private static KillAnimationManager instance = null;
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

    public static KillAnimationManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }
    [SerializeField]
    public GameObject background;
    //public GameObject attackCharacterObj;
    public GameObject attackCharacterBackObj;
    public GameObject deadCharacterObj;

    // Start is called before the first frame update
    void Start()
    {
        background.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyUp(KeyCode.Space))
        {
            startAnimation(0, CharacterManager.Instance.getCharacter(2), CharacterManager.Instance.getCharacter(2));
        }
        */
    }

    public bool getKillAnimationPlay() {
        return killAnimationPlay;
    }
    public void startAnimation(int type, Character attackCharacter, Character deadCharacter)
    {
        killAnimationPlay = true;
        if(type == 0)StartCoroutine(startAnimationType0(attackCharacter, deadCharacter));
    }
    public bool killAnimationPlay = false;
    private IEnumerator startAnimationType0(Character attackCharacter, Character deadCharacter)
    {
        background.SetActive(true);
        background.GetComponent<Animator>().Play("0");
        attackCharacterBackObj.GetComponent<SpriteRenderer>().sprite = attackCharacter.getBackSprite();
        deadCharacterObj.GetComponent<Animator>().runtimeAnimatorController = deadCharacter.getAnimator(true);
        deadCharacterObj.GetComponent<Animator>().Play("Idle");
            //Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + deadCharacter.getName() + "/animator_" + deadCharacter.getName());
        Vector3 attackInitPoint = new Vector3(-75f, -30f, 0f);
        Vector3 attackEndPoint = new Vector3(100f, 15f, 0f);
        Vector3 deadInitPoint = new Vector3(82f, -57f, 0f);
        Vector3 deadEndPoint = new Vector3(82f, -69f, 0f);
        if(deadCharacter.getShadow() == 2) deadEndPoint = new Vector3(82f, -105f, 0f);
        int stateTemp = 0;
        int changePoint = 30;
        float maxPoint = 50;
        WaitForSeconds delayTemp = new WaitForSeconds(0.01f);
        for (int i = 0; i < maxPoint; i++)
        {
            if (i == changePoint - 10) {
                SoundManager_Sfx.Instance.playSound(19);
            }
            if (i == changePoint) {
                
                attackInitPoint = attackCharacterBackObj.transform.position;
                attackEndPoint = new Vector3(174f, 35f, 0f);
                stateTemp = 1;
            }
            if (stateTemp == 0)
            {
                if(i % 20 == 0) SoundManager_Sfx.Instance.playSound(17);
                attackCharacterBackObj.transform.position = Vector3.Lerp(attackInitPoint, attackEndPoint, i / maxPoint) + new Vector3(Random.Range(-4f + i / 20f, 4f + i / 20f), Random.Range(-2f + i / 20f, 2f + i / 20f), 0f);
            }
            else if (stateTemp == 1)
            {
                attackCharacterBackObj.transform.position = Vector3.Lerp(attackInitPoint, attackEndPoint, (i - changePoint) / (maxPoint - changePoint)) + new Vector3(Random.Range(-2f + i / 20f, 2f + i / 20f), Random.Range(-2f + i / 20f, 2f + i / 20f), 0f);
            }
            deadCharacterObj.transform.position = Vector3.Lerp(deadInitPoint, deadEndPoint, i / maxPoint) + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0f);
            attackCharacterBackObj.transform.localScale = new Vector3(1f + (i / maxPoint), 1f + (i / maxPoint), 0f);
            deadCharacterObj.transform.localScale = new Vector3(1 + (i / maxPoint) * (i / maxPoint), 1 + (i / maxPoint) * (i / maxPoint), 0f);
            yield return delayTemp;
        }
        background.SetActive(false);
        killAnimationPlay = false;
    }
}
