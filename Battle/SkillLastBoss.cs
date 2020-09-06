using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class LastEnemyActives : EnemyActiveSkills
{
    public LastEnemyActives(SkillSet set) : base(set)
    {

    }
}

public class LastEnemyPassives : PassiveSkills
{
    public LastEnemyPassives(SkillSet set) : base(set)
    {

    }
}

public class SkillLastBoss : EnemySkillSet
{
    public SkillLastBoss() : base() {
        normal = new NormalEnemyAttack(this);

        actSkills = new List<EnemyActiveSkills>();

        act.Add( new LastBossActive1(this));
        act.Add(new LastBossActive2(this));
        act.Add(new LastBossActive3(this));

        pas.Add(new LastBossPassive1(this));
        pas.Add(new LastBossPassive2(this));

        set.Add(act[1]);
        set.Add(act[2]);
        set.Add(act[3]);
        set.Add(pas[1]);
        set.Add(pas[2]);

        actSkills.Add((EnemyActiveSkills)act[1]);
        actSkills.Add((EnemyActiveSkills)act[2]);
        actSkills.Add((EnemyActiveSkills)act[3]);
    }

    public override void setSkillSet()
    {
        set.Clear();

        
    }
}

public class LastBossActive1 : EnemyActiveSkills {
    List<Slow> slow;
    List<BattleChar> players;

    public LastBossActive1(SkillSet set) : base(set) {
        targetNum = 5;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 5;

        key = "normalAttack";

        slow = new List<Slow>();
        players = new List<BattleChar>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        foreach (BattleChar bc in target) {
            bct = bc.getTarget();
            bct.setLastAttack(set.user);
            bct.SetMagicDamage(set.user, 1.0f, 1, set.user.getCritical() * set.user.getFinalDamage());

            if (Random.value <= 0.4f)
            {
                bct.SetMagicDamage(set.user, 1.0f, 1, CriticalTest() * set.user.getFinalDamage());

                if (players.Exists(x => x == bct))
                {
                    slow.ToArray()[players.IndexOf(bct)].setTurn(1);
                }
                else {
                    Slow tmp = new Slow(1, set.user, bct, 0.8f);

                    bct.stateSlow.Add(tmp);
                    slow.Add(tmp);
                    players.Add(bct);

                }


                if (bct.stateSturn == null)
                {
                    bct.stateSturn = new Sturn(2, set.user, bct);
                }
                else {
                    bct.stateSturn.setTurn(2);
                }
                
            }
        }

    }

    public override List<BattleChar> getTarget()
    {
        return set.user.BM.myBC;
    }
}

public class LastBossActive2 : EnemyActiveSkills
{

    public LastBossActive2(SkillSet set) : base(set)
    {
        targetNum = 2;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 3;

        key = "normalAttack";

    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        foreach (BattleChar bc in target)
        {
            bct = bc.getTarget();
            bct.setLastAttack(set.user);

            if (bc != bct)
            {
                bct.SetPhysicalDamage(set.user, 1.65f, 1, CriticalTest() * set.user.getFinalDamage());
            }
            else
            {
                bct.SetPhysicalDamage(set.user, 1.5f, 1, CriticalTest() * set.user.getFinalDamage());

            }

            
        }

    }

    public override List<BattleChar> getTarget()
    {
        targetList.Clear();
        targetList.AddRange(set.user.BM.myBC);
        targetList.Sort(delegate (BattleChar A, BattleChar B) {
            if (A.getHP() > B.getHP()) return 1;
            else if (A.getHP() < B.getHP()) return -1;
            else return 0;
        });

        return targetList.GetRange(0,2);
    }
}

public class LastBossActive3 : EnemyActiveSkills
{
    LastBossAct3Buff buff;
    LastBossAct3Kill kill;
    public LastBossActive3(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 6;

        key = "normalAttack";

        

    }

    public override void initSkill()
    {
        base.initSkill();
        kill = new LastBossAct3Kill(set);
        set.user.killStack.Add(kill);
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        foreach (BattleChar bc in target)
        {
            bc.SetHP(bc.DC.curHP + bc.DC.getMaxHP() * 0.2f);

            if (buff == null)
            {
                buff = new LastBossAct3Buff();
                buff.buffMgcDEF = 2.0f;
                buff.buffPhyDEF = 2.0f;
                buff.turn = 3;
                buff.user = set.user;
                buff.kill = kill;
            }
            else {
                buff.turn = 3;
            }

            if (set.user.stateSturn == null)
            {
                set.user.stateSturn = new Sturn(3, set.user, set.user);
                
            }
            else {
                set.user.stateSturn.setTurn(3);
            }

        }

    }
}

public class LastBossAct3Buff : Buff {
    public LastBossAct3Kill kill;

    public override int updateTurn()
    {
        int tmp = base.updateTurn();
        if (tmp == 0) {
            kill.enableKill();

            foreach(BattleChar bc in user.BM.myBC){
                bc.setLastAttack(user);
                bc.SetHP(bc.DC.curHP - user.DC.curHP * 0.9f);
            }

            kill.disableKill();
        }

        return tmp;
    }
}
public class LastBossAct3Kill : ActiveSkills {
    bool active;
    public LastBossAct3Kill(SkillSet set) : base(set) {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.ACT;
        coef = 1;
        disableKill();
    }

    public void enableKill() {
        active = true;
    }

    public void disableKill() {
        active = false;
    }

    public override void useSkill(List<BattleChar> target)
    {
        set.user.SetHP(set.user.getHP() + set.user.DC.getMaxHP() * 0.05f);
    }
}

public class LastBossPassive1 : PassiveSkills {
    LastBossPas1Buff buff;
    public LastBossPassive1(SkillSet set) : base(set) { 
        
    }

    public override void initSkill()
    {
        base.initSkill();
        buff = new LastBossPas1Buff();
        buff.step = 0;
        buff.turn = 100;
        set.user.eDebuffStack.Add(buff);
    }
}

public class LastBossPas1Buff : Buff{
    public override int updateTurn()
    {
        foreach (BattleChar bc in user.BM.myBC) {
            bc.SetHP(bc.getHP() - bc.DC.getMaxHP() * 0.02f);
            bc.setMP(bc.getMP() - bc.DC.getMaxMP() * 0.02f);
        }

        return base.updateTurn();
    }
}

public class LastBossPassive2 : PassiveSkills
{
    LastBossPas1Buff buff;
    public LastBossPassive2(SkillSet set) : base(set)
    {

    }

    public override void initSkill()
    {
        base.initSkill();
        buff = new LastBossPas1Buff();
        buff.step = 0;
        buff.turn = 100;
        set.user.eDebuffStack.Add(buff);
    }
}

public class LastBossPas2Buff : Buff
{
    float phyDam = 0.0f;
    float mgcDam = 0.0f;
    bool enableBuff = false;
    public override int updateTurn()
    {
        if (enableBuff) {
            phyDam = 0.0f;
            mgcDam = 0.0f;
            enableBuff = false;
        }

        if (user.stigma >= 5) {
            user.stigma = 0;
            phyDam = 0.5f;
            mgcDam = 0.5f;
            enableBuff = true;
        }

        buffIncAP = 1.0f + user.stigma * 0.03f;
        buffCrit = 1.0f + user.stigma * 0.03f;
        buffPhyATK = phyDam + 1.0f + user.stigma * 0.03f;
        buffPhyDEF = 1.0f + user.stigma * 0.03f;
        buffMgcATK = mgcDam + 1.0f + user.stigma * 0.03f;
        buffMgcDEF = 1.0f + user.stigma * 0.03f;

        return base.updateTurn();
    }
}

public class SkillLastEnemy1 : EnemySkillSet
{
    public SkillLastEnemy1() : base()
    {
        normal = new NormalEnemyAttack(this);
        actSkills = new List<EnemyActiveSkills>();

        act.Add( new LastEnemyActive1(this));
        act.Add( new LastEnemyActive2(this));

        set.Add(act[1]);
        set.Add(act[2]);

        actSkills.Add((EnemyActiveSkills)act[1]);
        actSkills.Add((EnemyActiveSkills)act[2]);
    }

    public override void setSkillSet()
    {
        set.Clear();


        
    }
}

public class LastEnemyActive1 : EnemyActiveSkills {
    public LastEnemyActive1(SkillSet set) : base(set) {
        targetNum = 5;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 3;

        key = "normalAttack";

    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        foreach (BattleChar bc in target)
        {
            foreach (Taunt t in bc.stateTaunt) {
                t.setTurn(0);
            }
        }

    }

    public override List<BattleChar> getTarget()
    {
        return set.user.BM.myBC;
    }
}

public class LastEnemyActive2 : EnemyActiveSkills
{
    float total;
    public LastEnemyActive2(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 5;

        key = "normalAttack";
        total = 0;

    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        total = 0;

        foreach (BattleChar bc in set.user.BM.myBC)
        {
            total += bc.getMP() * 0.2f;
            bc.setMP(bc.getMP() * 0.8f);
        }

        foreach (BattleChar bc in target) {
            bc.SetHP(bc.getHP() + total);
        }

    }

    public override List<BattleChar> getTarget()
    {
        return set.user.BM.myBC;
    }
}

public class SkillLastEnemy2 : EnemySkillSet
{

    BattleChar boss;
    public SkillLastEnemy2() : base()
    {
        normal = new NormalEnemyAttack(this);
        actSkills = new List<EnemyActiveSkills>();

        act.Add( new LastEnemyActive3(this));
        pas.Add( new LastEnemyPassive1(this));

        set.Add(act[3]);
        set.Add(pas[1]);

        actSkills.Add((EnemyActiveSkills)act[3]);
    }

    public override void setSkillSet()
    {
        set.Clear();

        

        
    }

    public void setBoss(BattleChar boss) {
        this.boss = boss;
        ((LastEnemyPassive1)pas[1]).setBoss(boss);
    }
}

public class LastEnemyActive3 : EnemyActiveSkills
{
    List<LastEnemyAct3Buff> bomb;
    List<BattleChar> bomber;

    public LastEnemyActive3(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 4;

        key = "normalAttack";

        bomb = new List<LastEnemyAct3Buff>();
        bomber = new List<BattleChar>();

    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        foreach (BattleChar bc in target)
        {

            bct = bc.getTarget();
            bct.setLastAttack(set.user);

            if (bomber.Exists(x => x == bct)) {
                bomb.ToArray()[bomber.IndexOf(bct)].turn = 3;
            } else {
                LastEnemyAct3Buff b = new LastEnemyAct3Buff();
                b.turn = 3;
                b.user = set.user;
                b.target = bct;

                bct.debuffStack.Add(b);
                bomber.Add(bct);
                bomb.Add(b);

            }

        }

    }
}

public class LastEnemyAct3Buff : Buff {
    public BattleChar target;

    float CriticalTest()
    {
        if (Random.value <= user.getCritical())
        {
            return 1.5f;
        }
        else
        {
            return 1.0f;
        }
    }

    public override int updateTurn()
    {
        int tmp = base.updateTurn();
        if (tmp == 0) {
            target.SetMagicDamage(user, 2.0f, 1, user.getFinalDamage() * CriticalTest());
        }
        return tmp;
    }
}

public class LastEnemyPassive1 : PassiveSkills
{
    LastEnemyAct4Buff buff;
    BattleChar boss;

    public LastEnemyPassive1(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.ENEMY;
        type = SKILLTYPE.PAS;
        coef = 1;

        key = "normalAttack";

    }

    public void setBoss(BattleChar boss) {
        this.boss = boss;
    }

    public override void initSkill()
    {
        if (buff == null)
        {
            buff = new LastEnemyAct4Buff();
            buff.turn = 100;
            buff.step = 0;
            buff.user = set.user;
            buff.target = boss;

            boss.eDebuffStack.Add(buff);

        }

        base.initSkill();
    }
}

public class LastEnemyAct4Buff : Buff{
    public BattleChar target;
    public override void endTurn()
    {
        if(user.getHP() > 0)target.stigma++;
    }
}
