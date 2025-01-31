using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice
{
    //
    //현재 주사위가 가진 회전 정도, 보이는 면을 의미.
    public int dir=0, curIdx=0;
    //각 면이 가지고 있는 실제 눈의 값
    private int[] diceNum = new int[6] { 1, 2, 3, 4, 5, 6 };
    //테스트 용 이미지 배열

    //주사위 회전을 위한 배열
    private int[,] diceRollNum = new int[6, 4] { { 4, 3, 2, 1 }, { 0, 2, 5, 4 }, { 0, 3, 5, 1 }, { 0, 4, 5, 2 }, { 0, 1, 5, 3 }, { 2, 3, 4, 1 } };
    private int[,] diceRollDir = new int[6, 4] { { 0, 0, 0, 0 }, { 1, 1, 1, 3 }, { 2, 1, 0, 3 }, { 3, 1, 3, 3 }, { 0, 1, 2, 3 }, { 2, 2, 2, 2 } };
    private int[] diceRollDirAdd = new int[4] { 2, 3, 0, 1 };
    //
    // Start is called before the first frame update

    public Dice() {
    }
    public Dice(Dice dice)
    {
        this.dir = dice.dir;
        this.curIdx = dice.curIdx;
        for (int i=0;i<6;i++) {
            this.diceNum[i] = dice.getDiceNum(i);
        }
    }

    public void setCurDice(int afterIdx, int afterDir) { 
        curIdx = afterIdx; dir = afterDir;
        
    }
    public void turnDice(int turnDir)
    {
        //굴린 후 나온 눈과 회전 정도
        setCurDice(diceRollNum[curIdx,(4 + turnDir - dir) % 4], (diceRollDirAdd[turnDir] + diceRollDir[curIdx, (4 + turnDir - dir) % 4]) % 4);
    }

    public int getDiceNum(int idx)
    {
        return this.diceNum[idx];
    }

    public int throwDice()
    {
        setCurDice(Random.Range(0, 6), Random.Range(0, 4));
        return this.diceNum[curIdx];
    }
    public int getNum()
    {
        return this.diceNum[curIdx];
    }
    public void setNum(int idx, int val)
    {
        this.diceNum[idx] = val;
    }
    public void setNum_useItem(int idx, int itemIdx) //아이템 사용시. 지금은 index를 받아오는데 나중에는 item으로 바꿔도 괜찮을것 같다.
    {
        if(itemIdx == 1)
        {
            diceNum[idx] = itemIdx;
        }
    }
    public int getDir()
    {
        return dir;
    }
}
