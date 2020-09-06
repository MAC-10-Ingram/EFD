using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Equip {

    public int level;
    public string img; // 장비 이미지 URL
    public string name; // 장비이름

    public EquipType type; // 장비종류 : 투구 - 갑옷 - 신발 - 무기 - 보조무기
    public EquipClass cls; // 직업제한 : 도적 - 궁수 - 법사 - 기사 - 사제
    public EquipGrade grade; // 장비등급 : 노말 - 레어 - 유니크 - 미스틱

    public float maxHP; // 체력
    public float maxMP; // 마나
    public float incMP; // 마나회복
    public float incAP; // 속도회복
    public float phyATK; // 물리공격력
    public float phyDEF; // 물리방어력
    public float macATK; // 마법공격력
    public float macDEF; // 마법방어력
    public float healEFC; // 회복효율
    public float critical; // 치명타확률

    public Equip() {
        level = 1;
    }

    public bool isUpgrade() {
        if (level < 10) return true;


        return false;
    }

    public void upgrade() {
        if (isUpgrade())
        {
            level++;
        }
    }

    public float getHP() { return maxHP * (0.1f * level); }
    public float getMP() { return maxMP * (0.1f * level); }
    public float getIncMP() { return incMP * (0.1f * level); }
    public float getIncAP() { return incAP * (0.1f * level); }
    public float getPhyATK() { return phyATK * (0.1f * level); }
    public float getPhyDEF() { return phyDEF * (0.1f * level); }
    public float getMgcATK() { return macATK * (0.1f * level); }
    public float getMgcDEF() { return macDEF * (0.1f * level); }
    public float getHealEFC() { return healEFC * (0.1f * level); }
    public float getCritical() { return critical * (0.1f * level); }
}
[System.Serializable]
public class EquipHead : Equip
{
    public EquipHead() : base() { 
        
    }
}

[System.Serializable]
public class EquipBody : Equip
{
    public EquipBody() : base()
    {

    }
}
[System.Serializable]
public class EquipFoot : Equip
{
    public EquipFoot() : base()
    {

    }
}
[System.Serializable]
public class EquipWeapon : Equip
{
    public EquipWeapon() : base()
    {

    }
}
[System.Serializable]
public class EquipSubWeapon : Equip
{
    public EquipSubWeapon() : base()
    {

    }
}


[System.Serializable]
public class EquipSet {
    float total;

    public EquipHead head;
    public EquipBody body;
    public EquipFoot foot;
    public EquipWeapon weapon;
    public EquipSubWeapon subWeapon;

    Equip[] set;

    public EquipSet(String cls) {
        set = new Equip[5];

        head = new EquipHead(); //EquipManager.GetEquip(NormalHead + cls);
        body = new EquipBody(); //EquipManager.GetEquip(NormalBody) + cls;
        foot = new EquipFoot(); //EquipManager.GetEquip(NormalFoot + cls);
        weapon = new EquipWeapon(); //EquipManager.GetEquip(NormalWeapon + cls);
        subWeapon = new EquipSubWeapon(); //EquipManager.GetEquip(NormalSubWeapon + cls);

        set[0] = head;
        set[1] = body;
        set[2] = foot;
        set[3] = weapon;
        set[4] = subWeapon;
    }

    public float getHP() {
        total = 0;
        foreach (Equip e in set) {
            total += e.getHP();
        }

        return total; 
    }

    public float getMP()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getMP();
        }

        return total;
    }
    public float getIncMP()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getIncMP();
        }

        return total;
    }
    public float getIncAP()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getIncAP();
        }

        return total;
    }
    public float getPhyATK()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getPhyATK();
        }

        return total;
    }
    public float getPhyDEF()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getPhyDEF();
        }

        return total;
    }
    public float getMgcATK()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getMgcATK();
        }

        return total;
    }
    public float getMgcDEF()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getMgcDEF();
        }

        return total;
    }
    public float getHealEFC()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getHealEFC();
        }

        return total;
    }
    public float getCritical()
    {
        total = 0;
        foreach (Equip e in set)
        {
            total += e.getCritical();
        }

        return total;
    }

}