using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TARGETTAG{NONE, SELF, FRIEND, FRIENDALL, FRIENDDEAD, ENEMY, ENEMYALL, ENEMYDEAD};
public enum SKILLTYPE {ACT, PAS};

public enum ENEMYSKILLTYPE {BUFF,  CONDITION, NORMAL}

[System.Serializable]
public class Skill
{
    public int targetNum;
    public TARGETTAG targetTag;
    public SKILLTYPE type;
    public string image;
    [System.NonSerialized]
    public BattleChar bct;
    public string name;
    public string explain;


    public int level;
    public float coef;

    [System.NonSerialized]
    public SkillSet set;

    protected float DamCoef;

    public string key;

    public Skill(SkillSet set) {
        this.set = set;
        key = "skill";
        DamCoef = 1.0f;
    }

    public virtual void setTag(TARGETTAG tg) {
        targetTag = tg;
    }

    public virtual float CriticalTest() {
        if (Random.value <= set.user.getCritical())
        {
            return 1.5f;
        }
        else {
            return 1.0f;
        }
    }

    public virtual void useSkill(List<BattleChar> target)
    {
        
    }

    public virtual bool testMP()
    {
        return false;
    }

    public virtual void initSkill() {
        
    }

    public virtual void resetSkill() {
        
    }

    public void skillAnim(string key) {
        set.user.animator.SetBool(key, true);
        set.user.StartCoroutine(set.user.IsAnimEnd(key));
    }
}

[System.Serializable]
public class ActiveSkills : Skill { 
    public int basicMana;
    public int incMana;

    public ActiveSkills( SkillSet set) : base(set) {
        type = SKILLTYPE.ACT;
    }

    public int needMP() { return basicMana + incMana * level; }

    public override bool testMP()
    {
        if (set.user.DC.curMP - needMP() < 0) return false;
        else return true;
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        skillAnim(key);
        set.user.setMP(set.user.getMP() - needMP());
    }
}

public class CounterSkills : ActiveSkills
{
    public int turn;

    public CounterSkills(SkillSet set, int turn) : base(set)
    {
        this.turn = turn;
    }

    public virtual int updateTurn()
    {
        return --turn;
    }

    public virtual int setTurn(int turn)
    {
        return this.turn = turn;
    }

    public virtual void useSkill(BattleChar target) {
        skillAnim(key);
    }
}

public class EnemyActiveSkills : ActiveSkills {
    public int turn;
    public int turnUse;
    public List<BattleChar> targetList;
    public ENEMYSKILLTYPE skillType;

    public EnemyActiveSkills( SkillSet set) : base(set)
    {
        turn = 0;
        targetList = new List<BattleChar>();
    }

    public virtual List<BattleChar> getTarget() {
        targetList.Clear();
        targetList.Add(set.user.BM.myBC[Random.Range(0, set.user.BM.myBC.Count)]);
        return targetList;
    }

    public virtual bool checkConditoin() {
        return false;
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        setTurn(turnUse);
    }

    public override bool testMP()
    {
        if (turn-- > 0) return false;
        else return true;
    }

    public void setTurn(int i) {
        turn = i;
    }
}

[System.Serializable]
public class PassiveSkills : Skill {
    public PassiveSkills( SkillSet set) : base(set)
    {
        type = SKILLTYPE.PAS;
    }
}

[System.Serializable]
public class NormalAttack : ActiveSkills
{

    public NormalAttack( SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.ACT;
        image = "PATH";

        level = 1;
        coef = 1;

        basicMana = 0;
        incMana = 0;

        key = "normalAttack";
        name = "Normal Attack";
    }


}

public class EnemyAttack : EnemyActiveSkills
{

    public EnemyAttack(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        image = "PATH";


        turn = 0;

        key = "normalAttack";
    }


}

[System.Serializable]
public class SkillSet {
    public List<Skill> set;
    public List<Skill> act;
    public List<Skill> pas;
    public Skill normal;

    [System.NonSerialized]
    public BattleChar user;

    //public ActiveSkills act1;
    //public ActiveSkills act2;
    //public ActiveSkills act3;
    //public ActiveSkills act4;
    //public ActiveSkills act5;

    //public PassiveSkills pas1;
    //public PassiveSkills pas2;
    //public PassiveSkills pas3;
    //public PassiveSkills pas4;
    //public PassiveSkills pas5;

    public string name;
    public string explain;

    public SkillSet() {
        set = new List<Skill>();
        act = new List<Skill>();
        pas = new List<Skill>();
    }

    public virtual void setSkillSet() {
        set.Clear();
    }

    public void setUser(BattleChar user) {
        this.user = user;
    }

    public virtual float BuffedDamage(float damage, BattleChar target) {
        return damage;
    }

    public virtual void initSkillSet() {
        normal.set = this;

        foreach (Skill k in set) {
            k.initSkill();
            k.set = this;
        }
    }

    public virtual void resetSkillSet() {
        foreach (Skill k in set)
        {
            k.resetSkill();
        }
    }
}

public class EnemySkillSet : SkillSet {
    public List<EnemyActiveSkills> actSkills;
    public EnemyActiveSkills selectedSkill;

    public EnemySkillSet() : base()
    { 
        
    }

    public virtual EnemyActiveSkills getSkill() {
        selectedSkill = (EnemyActiveSkills)normal;

        for (int i = 0; i < actSkills.Count; i++)
        {
            if (!actSkills[i].testMP())
            {
                //턴이 부족한 경우
                continue;
            }

            //스킬 사용 컨디션에 부합하지 않은경우
            if (actSkills[i].skillType == ENEMYSKILLTYPE.CONDITION) {
                if (!actSkills[i].checkConditoin()) continue;
            }

            if (selectedSkill.skillType == actSkills[i].skillType)
            {
                if (selectedSkill != (EnemyActiveSkills)normal) selectedSkill = (Random.Range(0, 2) == 0 ? selectedSkill : actSkills[i]);
                else selectedSkill = actSkills[i];
            }
            else
            {
                if (selectedSkill.skillType > actSkills[i].skillType)
                {
                    selectedSkill = actSkills[i];
                }
            }
        }

        return selectedSkill;
    }

}
