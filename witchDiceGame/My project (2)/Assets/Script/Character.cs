using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_battle{
    private int originIdx;
    private int atk;
    private int mag;
    private int spd;
    private int armor;
    private int diceState;
    private int characterState;
    private int specialVal = 0;
    private int skillUseCount0 = 0;
    private int skillUseCount1 = 0;
    public Character_battle()
    {
        originIdx = -999;
        atk = 0;
        mag = 0;
        spd = 0;
        armor = 0;
        diceState = 0;
        specialVal = 0;

        skillUseCount0 = 0;
        skillUseCount1 = 0;
    }

    public int getSkillUseCount(int opt)
    {
        if (opt == 0) return skillUseCount0;
        else if(opt == 1)return skillUseCount1;
        return 0;
    }
    public void setSkillUseCount(int opt, int val)
    {
        if (opt == 0) skillUseCount0 = val;
        else if (opt == 1) skillUseCount1 = val;
    }
    public int getDiceState()
    {
        return diceState;
    }
    public void setDiceState(int diceState)
    {
        this.diceState = diceState;
    }

    public int getOriginIdx()
    {
        return originIdx;
    }
    public int getAtk()
    {
        return atk;
    }
    public int getSpeed()
    {
        return this.spd;
    }
    public int getMag() { return mag; }
    public void setOriginIdx(int originIdx)
    {
        this.originIdx = originIdx;
    }
    public int getArmor()
    {
        return armor;
    }
    public void setArmor(int a) {
        this.armor = a;
    }
    public int getSpecialVal()
    {
        return specialVal;
    }
    public void setSpecialVal(int a)
    {
        specialVal = a;
    }

    public void upgrade(int idx, int val)
    {
        if (idx == 2) {  //공격력업
            this.atk += val;
            if (this.atk > 99) this.atk = 99;
            if (this.atk < -99) this.atk = -99;
        }
        if (idx == 4)
        {  //마법감응력업
            this.mag += val;
            if (this.mag > 99) this.mag = 99;
            if (this.mag < -99) this.mag = -99;
        }
        if (idx == 5)
        {  //스피드업
            this.spd += val;
            if (this.spd > 99) this.spd = 99;
            if (this.spd < -99) this.spd = -99;
        }
        if (idx == 6) {
            this.armor += val;
            if (this.armor > AdventureManager.Instance.getArmorMaxVal()) this.armor = AdventureManager.Instance.getArmorMaxVal();
            if (this.armor < 0) this.armor = 0;
        }
    }
   
}
public abstract class Character
{
    // 0 : 활성화 1: 미배정 2: 비활성화 3 : 사용불가
    protected int curState = 3;
    protected int level = 0, exp = 0, phyAtk = 0, magAtk = 0, speed = 0,
        hp = 0, maxHp = 0;

    protected Item[] item = new Item[2];
    //버프, 디버프, 상태이상, 패시브, 지닌 주사위
    protected int[] skillIdx = new int[2] { 0, 1 };
    protected Destiny destiny; //할당된 운명에 대한 클래스.
    public Dice dice;
    protected Character_battle character_battle;
    protected bool reviveUnit = false;
    protected int shadow = 0;
    protected int money = 0;
    public Character(int curState, Destiny destiny)
    {
        this.destiny = destiny;
        this.curState = curState;
        dice = new Dice(); //일단 디폴트로 둠 추후 캐릭터마다 다르게 만들어줄 필요가 있다.
        if (curState == 0 || curState == 2)
        {
            this.level = 1;
            this.exp = 0;
            this.phyAtk = destiny.phyAtk;
            this.magAtk = destiny.magAtk;
            this.speed = destiny.speed;
            this.maxHp = destiny.maxHp;
            this.hp = maxHp;

            skillIdx[0] = 0;//destiny.getSkillIdx(0); 
            skillIdx[1] = 1;//destiny.getSkillIdx(1);

            item[0] = new Item(itemManager.Instance.getItem(2, 0)); //빈 아이템을 넣어준다.
            item[1] = new Item(itemManager.Instance.getItem(2, 0));
        }
        this.character_battle = new Character_battle();
        reviveUnit = false;
        this.shadow = destiny.getShadow();
        this.money = destiny.getMoney();
    }

    public Character(Character character) {
        this.curState = character.curState;
        this.level = character.level;
        this.exp = character.exp;
        this.phyAtk = character.phyAtk;
        this.magAtk = character.magAtk;
        this.speed = character.speed;
        this.hp = character.hp;
        this.maxHp = character.maxHp;
        this.item[0] = new Item(character.getItem(0));
        this.item[1] = new Item(character.getItem(1));
        this.skillIdx[0] = character.skillIdx[0];
        this.skillIdx[1] = character.skillIdx[1];
        this.destiny = new Destiny(character.getDestiny());
        this.dice = new Dice(character.getDiceTrue());
        this.character_battle = new Character_battle();
        this.reviveUnit = character.reviveUnit;
        this.shadow = character.shadow;
        this.money = character.getMoney();
    }

    public void characterDeepCopy(Character character)
    {
        this.curState = character.curState;
        this.level = character.level;
        this.exp = character.exp;
        this.phyAtk = character.phyAtk;
        this.magAtk = character.magAtk;
        this.speed = character.speed;
        this.hp = character.hp;
        this.maxHp = character.maxHp;
        this.item[0] = new Item(character.getItem(0));
        this.item[1] = new Item(character.getItem(1));
        this.skillIdx[0] = character.skillIdx[0];
        this.skillIdx[1] = character.skillIdx[1];
        this.destiny = new Destiny(character.getDestiny());
        this.dice = new Dice(character.getDiceTrue());
        this.character_battle = new Character_battle();
        if (destiny.getDestinyIdx() > 10001) this.character_battle.setArmor(character.character_battle.getArmor());
        this.reviveUnit = character.reviveUnit;
        this.shadow = character.shadow;
        this.money = character.getMoney();
    }

    public RuntimeAnimatorController getAnimator(bool deadOk)
    {
        string temp = this.getDestiny().getName();

        if (!deadOk && this.getCurState() != 0)
        {
            return Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");
        }

        if (
            (jsonDataManager.Instance.getChapterRead(0, 2) == 0 && (this.getDestiny().getDestinyIdx() == 10012 || this.getDestiny().getDestinyIdx() == 10003 || this.getDestiny().getDestinyIdx() == 10004))
        )
        {
            return Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + temp + "/animator_" + temp + "_2");
        }
        return Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + temp + "/animator_" + temp);
    }

    public Sprite getBackSprite()
    {
        return Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + this.getName() + "_back");
    }
    public Sprite getShadowSprite()
    {
        if (this.getCurState() != 0)
        {
            return Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        return Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_character_shadow_" + this.getShadow().ToString());
    }

    public bool getPossible(int val)
    {
        if (Random.Range(0, 99) + this.magAtk >= 100 - val)
        {
            return true;
        }
        return false;
    }
    public int getMoney() {
        return this.money;
    }
    public int getShadow()
    {
        return this.shadow;
    }
    public bool getReviveUnit()
    {
        return reviveUnit;
    }
    public void setReviveUnit(bool input)
    {
        this.reviveUnit = input;
    }

    public Character_battle getCharacter_battle()
    {
        return this.character_battle;
    }
    public Dice getDiceTrue()
    {
        return this.dice;
    }
    public string getName()
    {
        return this.destiny.getName();
    }
    public Dice getDiceObj()
    {
        return this.dice;
    }

    public int throwDice()
    {
        return this.dice.throwDice();
    }
    public int throwDiceExcept() {
        return this.dice.throwDiceExcept();
    }
    public int needDice(int skillNum)
    {
        return destiny.getNeedDice(skillIdx[skillNum]);
    } 

    public void setDice(int diceIdx, int val)
    {
        this.dice.setCurDice(diceIdx, val);
    }
    public int getDice(int diceIdx)
    {
        return this.dice.getDiceNum(diceIdx);
    }
    public int getDice()
    {
        return this.dice.getNum();
    }
    public int getDiceDir()
    {
        return this.dice.getDir();
    }


    public void changeDiceNum(int idx, int val)
    {
        this.dice.setNum(idx, val);
    }

    public void changeEquip(int itemNum, int itemIdx)
    {
        item[itemNum] = new Item(itemManager.Instance.getItem(2, itemIdx));
        
    }
    public Item getItem(int idx)
    {
        return item[idx];
    }

    public Destiny getDestiny()
    {
        return destiny;
    }
    public int getCurState(){ return curState; }
    public int getHp() {return hp; }
    public int getArmor() {
        return this.character_battle.getArmor();
    }
    public int getMaxHp() { return maxHp; }
    public Skill skillUse(int selNum)
    {
        return destiny.findSkill(skillIdx[selNum]);
    }
    public string getSkillName(int selNum)
    {
        return destiny.findSkill(skillIdx[selNum]).getSkillName();
    }

    public int getSkillVal(int selNum, int idx) {
        return skillUse(selNum).getVal(idx);
    }

    
    public int getSkillIdx(int num)
    {
        //return destiny.getSkillIdx(skillIdx[num]);
        return skillIdx[num];
    }

    public abstract List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket);

    public virtual int getTargetChance(int idx)
    {
        return 0;
    }
    public int TakeSkillPacket(TakeSkillPacket takeSkillPacket) //return -2 : state변화만  -1 : 아무것도 해당 X 0 : 타격성공+생존 1 : 사망 2: 회피 3:버프 4: 방어구로 가드
    {
        if (takeSkillPacket.getSkillType() == 0 )//|| takeSkillPacket.getSkillType() == 1000)
        {
            if (this.getSpeed() >= Random.Range(1, 101)) { return 2; }
            if (this.character_battle.getArmor() > 0) {
                this.character_battle.setArmor(this.character_battle.getArmor() - 1);
                BattleManager.Instance.updateHpCover(takeSkillPacket.getTargetIdx(), 1);
                return 4;
            }
            this.hp -= takeSkillPacket.getVal();
            //Debug.Log("this damage is : " + takeSkillPacket.getVal());
            //Debug.Log("my remain Hp is : " + this.hp);

            if (this.hp <= 0)
            {
                if (this.reviveUnit && AdventureManager.Instance.getTutorial() != 0)
                {
                    jsonDataManager.Instance.tutorialRevive();
                    this.hp = 1; return 0;
                } //튜토리얼 용으로 하나 만들기.
                this.hp = 0;
                this.curState = 2;
                return 1;
            }
            return 0;
        }
        else if (takeSkillPacket.getSkillType() == 1)// || takeSkillPacket.getSkillType() == 1001) //회복인 경우
        {
            Debug.Log("Heal is " + takeSkillPacket.getVal());
            this.hp += takeSkillPacket.getVal();

            if (this.hp >= this.maxHp)
            {
                this.hp = this.maxHp;
            }
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 2) //공격력 업인 경우
        {
            this.character_battle.upgrade(2, takeSkillPacket.getVal());
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 3) //특수 변수 변화인경우
        {
            this.character_battle.setSpecialVal(takeSkillPacket.getVal()); // 변수를 해당 값으로 변화시킨다.
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 4) //마법감응력 업인 경우
        {
            this.character_battle.upgrade(4, takeSkillPacket.getVal());
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 5) //스피드 업인 경우
        {
            this.character_battle.upgrade(5, takeSkillPacket.getVal());
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 6) //방어구 획득인 경우
        {
            this.character_battle.upgrade(6, takeSkillPacket.getVal());
            BattleManager.Instance.updateHpCover(takeSkillPacket.getTargetIdx(), 0);
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 10)// HP 영구 제거
        {
            if (this.getSpeed() >= Random.Range(1, 101)) { return 2; }
            this.downGrade(1, takeSkillPacket.getVal());
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 11) //최대체력 업그레이드
        {
            this.upGrade(1, takeSkillPacket.getVal());
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 12) //공격력 업인 경우
        {
            this.downGrade(5, takeSkillPacket.getVal());
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 14) //마법감응력 업인 경우
        {
            this.downGrade(6, takeSkillPacket.getVal());
            return 3;
        }
        else if (takeSkillPacket.getSkillType() == 15) //스피드 업인 경우
        {
            this.downGrade(7, takeSkillPacket.getVal());
            return 3;
        }

        if (takeSkillPacket.getStateChange() >= 1 && takeSkillPacket.getStateChange() <= 6)
        {
            return -2;
        }
        return -1;
    }
    public int getPhyAtk(int opt)
    {
        if (opt == 0) return phyAtk;
        if (opt == 1) return character_battle.getAtk();
        return 0;
    }
    public int getMagAtk(int opt)
    {
        if (opt == 0) return magAtk;
        if (opt == 1) return character_battle.getMag();
        return 0;
    }
    public int getSpeed(int opt) {
        if (opt == 0) return speed;
        if (opt == 1) return character_battle.getSpeed();
        return 0;
    }
    public int getPhyAtk(){
        if (phyAtk + character_battle.getAtk() < 0) return 0;
        if (phyAtk + character_battle.getAtk() > AdventureManager.Instance.getAtkMaxVal()) return AdventureManager.Instance.getAtkMaxVal();
        return phyAtk + character_battle.getAtk(); 
    }
    public int getMagAtk() {
        if (magAtk + character_battle.getMag() < 0) return 0;
        if (magAtk + character_battle.getMag() > AdventureManager.Instance.getMagMaxVal()) return AdventureManager.Instance.getMagMaxVal();
        return magAtk + character_battle.getMag(); }
    public int getSpeed() {
        if (speed + character_battle.getSpeed() < 0) return 0;
        if (speed + character_battle.getSpeed() > AdventureManager.Instance.getSpdMaxVal()) return AdventureManager.Instance.getSpdMaxVal();
        return speed + character_battle.getSpeed(); 
    }
    public int damage(int damage)
    {
        this.hp -= damage;
        if (this.hp <= 0)
        {
            this.curState = 2;
            this.hp = 0;
            return 1;
        }
        return 0;
    }

    public int downGradeDamage(int damage)
    {
        this.hp -= damage;
        if (this.hp <= 0)
        {
            this.hp = 1;
            //if (this.reviveUnit) {this.hp = 0; return 0; }
            //this.curState = 2;
            //this.hp = 0;
            //return 1;
        }
        return 0;
    }

    public void setHp(int hp)
    {
        if (this.hp <=0 && hp > 0) this.curState = 0; //부활인 경우
        this.hp = hp;
        if (hp == 0 )
        {
            if(this.curState == 0) this.curState = 2;
        }
        
    }
    public int upGrade(int idx, int val)
    {  //0 : 체력 / 1: 최대체력  / 4:방어도 / 5:공격력 / 6:마법 감응력 / 7.스피드 
        if (idx == 0){
            this.hp += val;
            if (hp > maxHp) hp = maxHp;
        }
        if (idx == 1){
            int tempMaxHp = maxHp;
            maxHp += val;
            this.hp += maxHp - tempMaxHp; 
        }
        if (idx == 4)
        {
            //this.armor += val;
        }
        if (idx == 5)
        {
            this.phyAtk += val;
            if (this.phyAtk > AdventureManager.Instance.getAtkMaxVal())
            {
                fullUI.showFull(140);
                this.phyAtk = AdventureManager.Instance.getAtkMaxVal();
            }
            if (this.phyAtk < 0) this.phyAtk = 0;
        }
        if (idx == 6)
        {
            this.magAtk += val;
            if (this.magAtk > AdventureManager.Instance.getMagMaxVal())
            {
                fullUI.showFull(140);
                this.magAtk = AdventureManager.Instance.getMagMaxVal();
            }
                if (this.magAtk < 0) this.magAtk = 0;
        }
        if (idx == 7)
        {
            this.speed += val;
            if (this.speed > AdventureManager.Instance.getSpdMaxVal())
            {
                fullUI.showFull(140);
                this.speed = AdventureManager.Instance.getSpdMaxVal();
            }
            if (this.speed < 0) this.speed = 0;
        }
        return 0;
    }

    public int downGrade(int idx, int val)
    {  //0 : 체력 / 1: 최대체력  / 4:방어도 / 5:공격력 / 6:마법 감응력/ 7 : 스피드
        if (idx == 0) //체력이 줄었고 
        {
            if (downGradeDamage(val) == 1) return 1;
        }
        if (idx == 1)
        {
            this.maxHp -= val;
            if (maxHp < hp) hp = maxHp;
            if (maxHp <= 0)
            {
                //this.curState = 2;
                maxHp = 1;
                hp = 1; //return 1;
            }
        }
        if (idx == 4) {
          /*  this.armor -= val; 
            if (this.armor < 0) this.armor = 0;*/
        }
        if (idx == 5)
        {
            this.phyAtk -= val;
            if (this.phyAtk > AdventureManager.Instance.getAtkMaxVal()) this.phyAtk = AdventureManager.Instance.getAtkMaxVal();
            if (this.phyAtk < 0) this.phyAtk = 0;
        }
        if (idx == 6)
        {
            this.magAtk -= val;
            if (this.magAtk > AdventureManager.Instance.getMagMaxVal()) this.magAtk = AdventureManager.Instance.getMagMaxVal();
            if (this.magAtk < 0) this.magAtk = 0;
        }
        if (idx == 7)
        {
            this.speed -= val;
            if (this.speed > AdventureManager.Instance.getSpdMaxVal()) this.speed = AdventureManager.Instance.getSpdMaxVal();
            if (this.speed < 0) this.speed = 0;
        }
        return 0;
    }
    
}

