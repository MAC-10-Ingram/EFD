using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidEnemyActives : EnemyActiveSkills {
    public MidEnemyActives(SkillSet set) : base(set) { 
        
    }
}

public class MidEnemyPassives : PassiveSkills {
    public MidEnemyPassives(SkillSet set) : base(set) { 
        
    }
}

//중간 보스
public class SkillMidBoss : EnemySkillSet
{
    public SkillMidBoss() : base()
    {
        normal = new NormalEnemyAttack(this);
        actSkills = new List<EnemyActiveSkills>();

        act.Add( new MidBossActive1(this));
        act.Add( new MidBossActive2(this));

        pas.Add( new MidBossPassive1(this));
        pas.Add( new MidBossPassive3(this));

        set.Add(act[0]);
        set.Add(act[1]);
        set.Add(pas[0]);
        set.Add(pas[1]);

        actSkills.Add((EnemyActiveSkills)act[0]);
        actSkills.Add((EnemyActiveSkills)act[1]);
    }

    public override void setSkillSet()
    {
        set.Clear();

        
    }
}

public class MidBossActive1 : MidEnemyActives
{
    public MidBossActive1(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 3;

        key = "normalAttack";
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        int xpos=-1, ypos=-1;
        foreach (BattleChar bc in target) {
            bct = bc.getTarget();
            bct.setLastAttack(set.user);

            xpos = bct.xPos;
            ypos = bct.yPos;

            bct.SetPhysicalDamage(set.user, 1.25f, 1, set.user.getFinalDamage() * CriticalTest());
        }

        foreach (BattleChar bc in set.user.BM.myBC) {
            if (ypos == bc.yPos) {
                if (bc.xPos < xpos) {
                    bc.SetMagicDamage(set.user, 0.5f, 1, set.user.getFinalDamage() * CriticalTest()); ;
                }
            }
        }

    }
}

public class MidBossActive2 : MidEnemyActives
{
    List<Buff> gainHP;
    List<BattleChar> target;

    public MidBossActive2(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 6;

        key = "normalAttack";

        gainHP = new List<Buff>();
        target = new List<BattleChar>();
    }

    public override void useSkill(List<BattleChar> target)
    {

        base.useSkill(target);
        foreach (BattleChar bc in target)
        {
            bct = bc.getTarget();
            bct.setLastAttack(set.user);

            if (this.target.Exists(x => x == bct))
            {
                gainHP.ToArray()[this.target.IndexOf(bct)].turn = 5;
            }
            else
            {
                Buff t = new MidBossAct2Buff();
                t.turn = 5;
                t.user = set.user;
                gainHP.Add(t);
                target.Add(bct);
                bct.debuffStack.Add(t);
            }
        }

    }
}

public class MidBossAct2Buff : Buff {
    public override int updateTurn()
    {
        user.SetHP(user.getHP() + user.DC.getMaxHP() * 0.05f);
        return base.updateTurn();
    }
}

public class MidBossPassive1 : MidEnemyPassives {
    public MidBossPassive1(SkillSet set) : base(set) {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "PATH";

        coef = 1;
    }

    public override void initSkill()
    {
        Buff b = new MidBossAct2Buff();
        b.user = set.user;
        set.user.eDebuffStack.Add(b);
        //set.user.buffStack.Add(this);
    }
}

public class MidBossPas1Buff : Buff
{
    int num;
    public override int updateTurn()
    {
        if (user.DC.curHP >= user.DC.getMaxHP() * 0.5)
        {
            buffIncAP = 1.0f;
            buffPhyATK = 1.0f;
            buffMgcATK = 1.0f;

            num = user.buffStack.Count;
            user.SetHP(user.getHP() + user.DC.getMaxHP() * 0.05f * num);

            foreach (BattleChar bc in user.BM.myBC)
            {
                bc.SetHP(user.getHP() - user.DC.getMaxHP() * 0.03f * num);
            }
        }
        else {
            buffIncAP = 1.2f;
            buffPhyATK = 1.2f;
            buffMgcATK = 1.2f;
        }


        return base.updateTurn();

    }
}

public class MidBossPassive3 : PassiveSkills {
    public MidBossPassive3(SkillSet set) : base(set) {
        targetNum = 1;
        targetTag = TARGETTAG.SELF;
        type = SKILLTYPE.PAS;
        image = "PATH";

        coef = 1;
    }

    public override void initSkill()
    {
        Buff b = new MidBossPas3Buff();
        b.user = set.user;
        b.step = 0;
        set.user.eDebuffStack.Add(b);
        //set.user.buffStack.Add(this);
    }
}

public class MidBossPas3Buff : Buff
{
    int stigma;
    float buffCoef;
    public override int updateTurn()
    {
        stigma = user.stigma;
        buffCoef = stigma * 0.1f + 1.0f;
        if (stigma < 15)
        {
            buffPhyATK = buffCoef;
            buffPhyDEF = buffCoef;
            buffMgcATK = buffCoef;
            buffMgcDEF = buffCoef;
            buffCrit = buffCoef;
            buffIncAP = buffCoef;
        }
        else
        {
            user.stigma = 0;
            BattleChar bc = user.BM.myBC[Random.Range(0, user.BM.myCharNum)];

            bc.SetMagicDamage(user, 3.0f, 1, 1.0f);
        }


        return base.updateTurn();

    }
}


//중간 쫄 1
public class SkillMidEnemy1 : EnemySkillSet {
    BattleChar boss;
    public SkillMidEnemy1() : base() {
        normal = new NormalEnemyAttack(this);
        actSkills = new List<EnemyActiveSkills>();

        act.Add( new MidEnemyActive1(this));
        act.Add( new MidEnemyActive2(this));

        set.Add(act[0]);
        set.Add(act[1]);

        actSkills.Add((EnemyActiveSkills)act[0]);
        actSkills.Add((EnemyActiveSkills)act[1]);

    }

    public override void setSkillSet()
    {
        set.Clear();
    }

    public void setBoss(BattleChar boss)
    {
        this.boss = boss;
        ((MidEnemyActive1)act[0]).setBoss(boss);
    }
}

public class MidEnemyActive1 : EnemyActiveSkills {
    List<MidEnemyAct1Buff> BossBuffs;
    BattleChar boss;

    public MidEnemyActive1(SkillSet set) : base(set) {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 3;

        key = "normalAttack";
        BossBuffs = new List<MidEnemyAct1Buff>();
    }

    public void setBoss(BattleChar boss) {
        this.boss = boss;
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);
        if (BossBuffs.Count < 3) {
            for (int i = 0; i< 3; i++) {
                MidEnemyAct1Buff b = new MidEnemyAct1Buff();
                b.user = set.user;
                b.setBuff();
                b.turn = 3;
                set.user.buffStack.Add(b);
                BossBuffs.Add(b);

            }
        }
        else {
            foreach (MidEnemyAct1Buff b in BossBuffs)
            {
                b.setBuff();
            }
        }
        

    }
}

public class MidEnemyAct1Buff : Buff {
    public void setBuff() {
        ClearBuff();
        switch (Random.Range(0,5)) {
            case 0:
                buffPhyATK = 1.1f;
                break;
            case 1:
                buffMgcATK = 1.1f;
                break;
            case 2:
                buffPhyDEF = 1.1f;
                break;
            case 3:
                buffMgcDEF = 1.1f;
                break;
            case 4:
                buffIncAP = 1.1f;
                break;
        }
    }

    void ClearBuff()
    {
        buffMgcATK = 1.0f;
        buffMgcDEF = 1.0f;
        buffPhyATK = 1.0f;
        buffPhyDEF = 1.0f;
        buffIncAP = 1.0f;
    }
}

public class MidEnemyActive2 : EnemyActiveSkills
{
    List<BattleChar> TRG;
    List<MidEnemyAct2Buff> deb;
    public MidEnemyActive2(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIENDALL;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 3;

        key = "normalAttack";

        TRG = new List<BattleChar>();
        deb = new List<MidEnemyAct2Buff>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach (BattleChar bc in target)
        {
            if (TRG.Exists(x => x == bc))
            {
                deb.ToArray()[target.IndexOf(bc)].turn = 2;
            }
            else
            {

                TRG.Add(bc);

                MidEnemyAct2Buff d = new MidEnemyAct2Buff();
                d.user = set.user;
                d.target = bc;
                d.turn = 2;

                deb.Add(d);
                bc.debuffStack.Add(d);
            }
        }

    }

    public override List<BattleChar> getTarget()
    {
        return set.user.BM.myBC;
    }
}

public class MidEnemyAct2Buff : Buff
{
    public BattleChar target;
    public override int updateTurn()
    {
        int tmp = base.updateTurn();

        if (tmp <= 0)
        {
            target.healSwitch = true;
        }
        else
        {
            target.healSwitch = false;
        }
        return tmp;
    }
}

//중간 쫄 2
public class SkillMidEnemy2 : EnemySkillSet
{
    BattleChar boss;
    public SkillMidEnemy2() : base()
    {
        normal = new NormalEnemyAttack(this);
        actSkills = new List<EnemyActiveSkills>();

        act.Add( new MidEnemyActive3(this));
        pas.Add( new MidEnemyPassive1(this));

        set.Add(act[0]);
        set.Add(pas[0]);

        actSkills.Add((EnemyActiveSkills)act[0]);

    }

    public override void setSkillSet()
    {
        set.Clear();
    }

    public void setBoss(BattleChar boss)
    {
        this.boss = boss;
        ((LastEnemyPassive1)pas[1]).setBoss(boss);
    }
}



public class MidEnemyActive3 : EnemyActiveSkills
{
    List<BattleChar> TRG;
    List<MidEnemyAct3Buff> def1;
    List<MidEnemyAct3Buff> def2;

    public MidEnemyActive3(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.ACT;
        coef = 1;
        turnUse = 3;

        key = "normalAttack";

        TRG = new List<BattleChar>();
        def1 = new List<MidEnemyAct3Buff>();
        def2 = new List<MidEnemyAct3Buff>();
    }

    public override void useSkill(List<BattleChar> target)
    {
        base.useSkill(target);

        foreach(BattleChar bc in target)
        {

            bct = bc.getTarget();
            bct.setLastAttack(set.user);

            if (TRG.Exists(x => x == bc))
            {
                def1.ToArray()[target.IndexOf(bct)].turn = 2;
                def1.ToArray()[target.IndexOf(bct)].setBuff();
                def2.ToArray()[target.IndexOf(bct)].turn = 2;
                def1.ToArray()[target.IndexOf(bct)].setBuff();
            }
            else
            {

                TRG.Add(bct);

                MidEnemyAct3Buff d = new MidEnemyAct3Buff();
                d.user = set.user;
                d.turn = 2;
                def1.Add(d);
                bct.debuffStack.Add(d);

                d = new MidEnemyAct3Buff();
                d.user = set.user;
                d.turn = 2;
                def2.Add(d);
                bct.debuffStack.Add(d);
            }
        }
        

    }
}

public class MidEnemyAct3Buff : Buff
{
    public void setBuff()
    {
        ClearBuff();
        switch (Random.Range(0, 5))
        {
            case 0:
                buffPhyATK = 0.9f;
                break;
            case 1:
                buffMgcATK = 0.9f;
                break;
            case 2:
                buffPhyDEF = 0.9f;
                break;
            case 3:
                buffMgcDEF = 0.9f;
                break;
            case 4:
                buffIncAP = 0.9f;
                break;
        }
    }

    void ClearBuff()
    {
        buffMgcATK = 1.0f;
        buffMgcDEF = 1.0f;
        buffPhyATK = 1.0f;
        buffPhyDEF = 1.0f;
        buffIncAP = 1.0f;
    }
}

public class MidEnemyPassive1 : PassiveSkills
{
    BattleChar boss;
    public MidEnemyPassive1(SkillSet set) : base(set)
    {
        targetNum = 1;
        targetTag = TARGETTAG.FRIEND;
        type = SKILLTYPE.PAS;
        coef = 1;

        key = "normalAttack";
    }

    public override void initSkill()
    {
        base.initSkill();

        foreach (BattleChar bc in set.user.BM.myBC) {
            MidEnemyPas1Buff d = new MidEnemyPas1Buff();
            d.boss = boss;
            d.user = set.user;
            d.step = 0;
            bc.eDebuffStack.Add(d);
        }
    }
    
    public void setBoss(BattleChar boss) {
        this.boss = boss;
    }
}

public class MidEnemyPas1Buff : Buff{
    public BattleChar boss;
    
    public override void endTurn()
    {
        boss.stigma++;
    }
}

