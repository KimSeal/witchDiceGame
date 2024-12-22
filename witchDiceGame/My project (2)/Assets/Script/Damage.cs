using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    //실제 데미지를 연산하는 변수
    private int damageVal=0;
    //상태이상 변화에 대한 변수
    private int stateChange=0;
    //공격하는 대상과 맞는 대상 정보
    private int attackCharacter = 0;
    private int defendCharacter = 0;
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    int getdamageVal() { return this.damageVal; }
    void setdamageVal(int input) { this.damageVal = input; }
    int getStateChange() { return this.stateChange; }
    void setStateChange(int input){ this.stateChange = input; }

    int getattackCharacter() { return this.attackCharacter; }
    void setattackCharacter(int input) { this.attackCharacter = input; }
    int getdefendCharacter() { return this.defendCharacter; }
    void setdefendCharacter(int input) { this.defendCharacter = input; }
}
