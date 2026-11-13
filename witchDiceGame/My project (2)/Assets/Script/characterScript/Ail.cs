using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CowTree : Character
{
    public CowTree(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class GoomBang : Character
{
    public GoomBang(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class Bear : Character
{
    public Bear(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class Nicky : Character
{
    public Nicky(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class Arwen : Character
{
    public Arwen(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class Wayne : Character
{
    public Wayne(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class GuildMaster : Character
{
    public GuildMaster(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class Tem : Character
{
    public Tem(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class FireArm : Character
{
    public FireArm(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
public class FrameArm : Character
{
    public FrameArm(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {

            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}

public class Ail_Adult : Character
{
    public Ail_Adult(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {
            
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 6, 2));
        }

        return packets;
    }

}
