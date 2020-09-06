using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEditor.PackageManager;
using UnityEngine;

public class NormalEnemyAttack : EnemyAttack
{

    public NormalEnemyAttack(SkillSet set) : base(set)
    {
        
    }

    public override void initSkill()
    {
        base.initSkill();
        setTag(set.user.tg);
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        foreach (BattleChar bc in target)
        {
            bct = bc.getTarget();
            bct.setLastAttack(set.user);
            bct.SetPhysicalDamage(set.user, 1.0f, level, coef * set.user.getFinalDamage() * CriticalTest());
        }
    }
}

public class SkillNormalEnemy : EnemySkillSet
{
    public SkillNormalEnemy() : base()
    {

        List<int> skillSeed = new List<int>();
        normal = new NormalEnemyAttack(this);
        actSkills = new List<EnemyActiveSkills>();

        act.Add(new NormalEnemyActive1(this));
        act.Add(new NormalEnemyActive2(this));
        act.Add(new NormalEnemyActive3(this));
        act.Add(new NormalEnemyActive4(this));
        act.Add(new NormalEnemyActive5(this));
        act.Add(new NormalEnemyActive6(this));
        act.Add(new NormalEnemyActive7(this));

        pas.Add(new NomalEnemyPassive1(this));
        pas.Add(new NomalEnemyPassive2(this));
        pas.Add(new NomalEnemyPassive3(this));
        pas.Add(new NomalEnemyPassive4(this));
        pas.Add(new NomalEnemyPassive5(this));



        for (int i = 0; i < 11; i++)
        {
            skillSeed.Add(i);
        }

        for (int i = 0; i < 4; i++)
        {
            int index = Random.Range(0, skillSeed.Count);
            switch (skillSeed[index])
            {
                case 1:
                    set.Add(act[0]);
                    actSkills.Add((EnemyActiveSkills)act[0]);
                    break;
                case 2:
                    set.Add(act[1]);
                    actSkills.Add((EnemyActiveSkills)act[1]);
                    break;
                case 3:
                    set.Add(act[2]);
                    actSkills.Add((EnemyActiveSkills)act[2]);
                    break;
                case 4:
                    set.Add(act[3]);
                    actSkills.Add((EnemyActiveSkills)act[3]);
                    break;
                case 5:
                    set.Add(act[4]);
                    actSkills.Add((EnemyActiveSkills)act[4]);
                    break;
                case 6:
                    set.Add(act[5]);
                    actSkills.Add((EnemyActiveSkills)act[5]);
                    break;
                case 7:
                    set.Add(act[6]);
                    actSkills.Add((EnemyActiveSkills)act[6]);
                    break;
                case 8:
                    if (Random.Range(0, 2) == 0)
                    {
                        set.Add(pas[0]);
                    }
                    else
                    {
                        set.Add(pas[1]);
                    }

                    break;
                case 9:
                    set.Add(pas[2]);
                    break;
                case 10:
                    set.Add(pas[3]);
                    break;
                case 0:
                    set.Add(pas[4]);
                    break;
            }
            skillSeed.RemoveAt(index);
        }

    }

    public override void setSkillSet()
    {
        
    }
}

public class NormalEnemyActives : EnemyActiveSkills
{
    //public new SkillArcher set;

    public NormalEnemyActives(SkillNormalEnemy set) : base(set)
    {

    }

    public virtual float CriticalTest(BattleChar user)
    {
        float critValue = base.CriticalTest();

        if (critValue > 1.0f && ((NormalEnemyPassives)set.pas[3]).isInit)
        {
            Bleed bd = new Bleed(3, set.user, user, user.DC.getMaxHP() * 0.05f);
            user.stateBleed.Add(bd);
        }

        return critValue;
    }

    public override List<BattleChar> getTarget()
    {
        BattleChar tmp;
        targetList.AddRange(set.user.BM.myBC);
        if (((NormalEnemyPassives)set.pas[1]).isInit) {
            targetList.Sort(delegate(BattleChar A, BattleChar B) {
                if (A.getHP() > B.getHP()) return 1;
                else if (A.getHP() < B.getHP()) return -1;
                else return 0;
            });

            tmp = targetList[0];
            targetList.Clear();
            targetList.Add(tmp);

            return targetList;
        }

        if (((NormalEnemyPassives)set.pas[2]).isInit)
        {
            targetList.Sort(delegate (BattleChar A, BattleChar B) {
                if (A.debuffStack.Count > B.debuffStack.Count) return -1;
                else if (A.debuffStack.Count < B.debuffStack.Count) return 1;
                else return 0;
            });

            tmp = targetList[0];
            targetList.Clear();
            targetList.Add(tmp);

            return targetList;
        }

        return base.getTarget();
    }
}

public class NormalEnemyPassives : PassiveSkills
{
    //public new SkillArcher set;
    public bool isInit;

    public NormalEnemyPassives(SkillNormalEnemy set) : base(set)
    {

    }
}

public class NormalEnemyActive1 : NormalEnemyActives
{
    public NormalEnemyActive1(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        skillType = ENEMYSKILLTYPE.NORMAL;
        coef = 1;
        turnUse = 3;

        key = "act1";
        name = "act1";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            DamCoef = 1.5f;
            bct = bc.getTarget();

            bct.setLastAttack(set.user);
            bct.SetPhysicalDamage(set.user, 1.0f, level, coef * set.user.getFinalDamage() * CriticalTest(bct));
            
            if (Random.value <= (0.5))
            {
                if (bct.stateSturn == null) bct.stateSturn = new Sturn(1, set.user, bct);
                else bct.stateSturn.setSturn(1, set.user, bct);
            }
        }

        targetList.Clear();
    }

    public override List<BattleChar> getTarget()
    {
        return base.getTarget();
    }
}

public class NormalEnemyActive2 : NormalEnemyActives
{
    List<Poison> state;
    List<BattleChar> target;
    public NormalEnemyActive2(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        skillType = ENEMYSKILLTYPE.NORMAL;
        coef = 1;
        turnUse = 3;

        key = "act2";
        name = "act2";

        state = new List<Poison>();
        target = new List<BattleChar>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar g in target)
        {
            bct = g.getTarget();
            bct.setLastAttack(set.user);

            if (this.target.Exists(x => x == bct))
            {
                state.ToArray()[this.target.IndexOf(bct)].setTurn(3);
            }
            else
            {
                Poison t = new Poison(3, set.user, bct, 0.6f * set.user.GetMgcATK());
                state.Add(t);
                this.target.Add(bct);
                bct.statePoison.Add(t);
            }
        }

        targetList.Clear();
    }
}

public class NormalEnemyActive3 : NormalEnemyActives
{
    public NormalEnemyActive3(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        skillType = ENEMYSKILLTYPE.NORMAL;
        coef = 1;
        turnUse = 4;

        key = "normalAttack";
        name = "act3";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        BattleChar coEnemy = set.user.BM.enemyBC[Random.Range(0, set.user.BM.enemyNum)];
        coEnemy.animator.SetBool(key, true);
        coEnemy.StartCoroutine(coEnemy.IsAnimEnd(key));

        
        foreach (BattleChar bc in target)
        {
            DamCoef = 1.0f;
            bct = bc.getTarget();
            bct.setLastAttack(set.user);

            bct.setLastAttack(set.user);
            bct.getTarget().SetPhysicalDamage(set.user, DamCoef, 1, 1 * set.user.getFinalDamage() * CriticalTest(bct));
            bct.getTarget().SetPhysicalDamage(coEnemy, DamCoef, 1, 1 * coEnemy.getFinalDamage() * CriticalTest(bct));
        }

        targetList.Clear();

    }

    public override List<BattleChar> getTarget()
    {
        return base.getTarget();
    }
}

public class NormalEnemyActive4 : NormalEnemyActives
{
    Buff buff;
    public NormalEnemyActive4(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.ACT;
        skillType = ENEMYSKILLTYPE.BUFF;
        coef = 1;
        turnUse = 4;

        key = "act4";
        name = "act4";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        if (buff == null)
        {
            buff = new Buff();
            buff.turn = 2;
            buff.user = set.user;
            buff.buffCrit = 1.3f;

            set.user.buffStack.Add(buff);
        }
        else {
            buff.turn = 2;
        }

        targetList.Clear();

    }

    public override List<BattleChar> getTarget()
    {
        targetList.Add(set.user);
        return targetList;
    }
}

public class NormalEnemyActive5 : NormalEnemyActives
{
    float totalHP;
    public NormalEnemyActive5(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMYALL;
        type = SKILLTYPE.ACT;
        skillType = ENEMYSKILLTYPE.CONDITION;
        coef = 1;
        turnUse = 4;

        key = "act5";
        name = "act5";
        totalHP = 0;
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target) {
            totalHP += bc.DC.curHP;
        }

        foreach (BattleChar bc in target)
        {
            bc.DC.curHP = totalHP / bc.BM.enemyNum;
        }

        targetList.Clear();

    }

    public override bool checkConditoin()
    {
        foreach (BattleChar bc in set.user.BM.enemyBC) {
            if (bc.getHP() / bc.DC.getMaxHP() < 0.5f) return true;
        }

        return false;
    }

    public override List<BattleChar> getTarget()
    {
        return set.user.BM.enemyBC;
    }
}

public class NormalEnemyActive6 : NormalEnemyActives
{
    List<Buff> debuffs;
    List<BattleChar> target;
    public NormalEnemyActive6(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        skillType = ENEMYSKILLTYPE.BUFF;
        coef = 1;
        turnUse = 3;

        key = "act6";
        name = "act6";

        debuffs = new List<Buff>();
        target = new List<BattleChar>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar g in target)
        {
            bct = g.getTarget();
            bct.setLastAttack(set.user);

            if (this.target.Exists(x => x == bct))
            {
                setBuff(debuffs.ToArray()[this.target.IndexOf(bct)]);
            }
            else
            {
                Buff t = new Buff();
                t.user = set.user;
                setBuff(t);

                debuffs.Add(t);
                this.target.Add(bct);
                bct.debuffStack.Add(t);
            }
        }

        targetList.Clear();

    }

    void setBuff(Buff bf) {

        bf.buffCrit = 1.0f;
        bf.buffPhyATK = 1.0f;
        bf.buffPhyDEF = 1.0f;
        bf.buffMgcATK = 1.0f;
        bf.buffMgcDEF = 1.0f;
        bf.buffIncAP = 1.0f;
        bf.turn = 2;

        switch (Random.Range(0,3)) { 
            case 0:
                bf.buffPhyATK = .9f;
                bf.buffMgcATK = .9f;
                break;
            case 1:
                bf.buffPhyDEF = .9f;
                bf.buffMgcDEF = .9f;
                break;
            case 2:
                bf.buffCrit = .9f;
                bf.buffIncAP = .9f;
                break;
        }
    }
}

public class NormalEnemyActive7 : NormalEnemyActives
{
    public NormalEnemyActive7(SkillNormalEnemy set) : base(set)
    {
        targetNum = 5;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.ACT;
        skillType = ENEMYSKILLTYPE.NORMAL;
        coef = 1;
        turnUse = 5;

        key = "act7";
        name = "act7";
    }

    public override float CriticalTest(BattleChar user)
    {
        float critValue = base.CriticalTest(user);

        if (critValue > 1.0f)
        {
            if (user.stateSturn == null)
            {
                user.stateSturn = new Sturn(2, set.user, user);
            }
            else { 
                user.stateSturn.setTurn(2); 
            }
        }

        return critValue;
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target) {
            bc.SetMagicDamage(set.user, 0.5f, 1, set.user.getFinalDamage() * CriticalTest(bc));
        }

        targetList.Clear();
    }

    public override List<BattleChar> getTarget()
    {
        return set.user.BM.myBC;
    }
}

public class NomalEnemyPassive1 : NormalEnemyPassives {
    public NomalEnemyPassive1(SkillNormalEnemy set) : base(set) {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.PAS;

        coef = 1;


    }

    public override void initSkill()
    {
        isInit = true;
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }
}

public class NomalEnemyPassive2 : NormalEnemyPassives
{
    public NomalEnemyPassive2(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.PAS;

        coef = 1;


    }

    public override void initSkill()
    {
        isInit = true;
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }
}

public class NomalEnemyPassive3 : NormalEnemyPassives
{
    public NomalEnemyPassive3(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.PAS;

        coef = 1;


    }

    public override void initSkill()
    {
        isInit = true;
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }
}

public class NomalEnemyPassive4 : NormalEnemyPassives
{
    public Buff selfBuff;
    public NomalEnemyPassive4(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.PAS;

        coef = 1;
    }

    public override void initSkill()
    {
        isInit = true;
        selfBuff = new Buff();
        selfBuff.user = set.user;
        selfBuff.step = 0;
        selfBuff.turn = 100;
        set.user.buffStack.Add(selfBuff);
        set.user.phyDamagedStack.Add(new NormalEnemyPassive4PhyDamaged(set, 100));
        set.user.mgcDamagedStack.Add(new NormalEnemyPassive4MgcDamaged(set, 100));
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }
}

public class NormalEnemyPassive4PhyDamaged : CounterSkills{
    public NormalEnemyPassive4PhyDamaged(SkillSet set, int turn) : base(set, turn) {
        
    }

    public override int updateTurn()
    {
        return turn;
    }

    public override void useSkill(BattleChar target)
    {
        base.useSkill(target);
        ((NomalEnemyPassive4)set.pas[4]).selfBuff.buffPhyDAM = 0.9f;
        ((NomalEnemyPassive4)set.pas[4]).selfBuff.buffMgcDAM = 1.0f;
    }
}

public class NormalEnemyPassive4MgcDamaged : CounterSkills
{
    public NormalEnemyPassive4MgcDamaged(SkillSet set, int turn) : base(set, turn)
    {

    }

    public override int updateTurn()
    {
        return turn;
    }

    public override void useSkill(BattleChar target)
    {
        base.useSkill(target);
        ((NomalEnemyPassive4)set.pas[4]).selfBuff.buffPhyDAM = 1.0f;
        ((NomalEnemyPassive4)set.pas[4]).selfBuff.buffMgcDAM = 0.9f;
    }
}

public class NomalEnemyPassive5 : NormalEnemyPassives
{
    Buff selfBuff;
    public NomalEnemyPassive5(SkillNormalEnemy set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.PAS;

        coef = 1;

    }

    public override void initSkill()
    {
        isInit = true;
        selfBuff = new NormalEnemyPassive5Buff(set.user);
        set.user.buffStack.Add(selfBuff);
    }

    public override void resetSkill()
    {
        isInit = false;
    }

    public override void useSkill(List<BattleChar> target)
    {

    }
}
class NormalEnemyPassive5Buff : Buff {
    public NormalEnemyPassive5Buff(BattleChar user) {
        turn = 100;
        this.user = user;

        buffPhyATK = 1.04f;
        buffMgcATK = 1.04f;
        buffPhyDEF = 1.04f;
        buffMgcDEF = 1.04f;
        buffCrit = 1.04f;
        buffIncAP = 1.04f;
    }

    public override int updateTurn()
    {
        buffPhyATK *= 1.04f;
        buffMgcATK *= 1.04f;
        buffPhyDEF *= 1.04f;
        buffMgcDEF *= 1.04f;
        buffCrit *= 1.04f;
        buffIncAP *= 1.04f;
        return turn;
    }
}
