using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StateTag {STURN, COUNT, TAUNT, SLOW, BLEED, BURN, POISON};
public class SpecialState// : MonoBehaviour
{
    protected int turn;
    protected StateTag tg;
    protected BattleChar target;
    protected BattleChar user;
        
    public SpecialState(int turn, BattleChar user, BattleChar target) {
        this.turn = turn;
        this.target = target;
        this.user = user;
    }

    public virtual int UpdateState() {
        return --turn;
    }

    public int getTurn() { return turn; }

    public int setTurn(int turn) {
        return this.turn = turn;
    }
}

public class Sturn : SpecialState {

    public Sturn(int turn, BattleChar user, BattleChar target) : base(turn, user, target) {
        this.tg = StateTag.STURN;
    }

    public void setSturn(int turn, BattleChar user, BattleChar target) {
        this.turn = turn;
        this.user = user;
        this.target = target;
    }
}

public class Counter : SpecialState {

    float damage;
    int lv;
    float coef;
    public Counter(int turn, BattleChar user, BattleChar target, int lv, float coef) : base(turn, user, target)
    {
        this.tg = StateTag.COUNT;
        damage = 0;
        this.lv = lv;
        this.coef = coef;
    }

    public float CriticalTest()
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

    public void setCounter(int turn) {
        this.turn = turn;
        resetDamage();
    }

    public void setTarget(BattleChar target) {
        this.target = target;
    }

    public float getDamage() {
        return damage;
    }
    public float resetDamage() {
        return damage = 0;
    }

    public float addDamage(float dam) {
        return damage += dam;
    }

    public override int UpdateState()
    {
        if (base.UpdateState() == 0)
        {
            BattleChar tmp;

            tmp = user.BM.enemys.ToArray()[Random.Range(0, user.BM.enemyNum)].GetComponent<BattleChar>();


            user.BM.player2target(tmp.transform.position);

            tmp.SetMagicDamage(user, getDamage() * (5 + 0.5f * lv) / 100.0f, lv, coef * user.getFinalDamage() * CriticalTest());

            ((KnightPassive3)user.DC.charSkillSet.pas[2]).pas3(tmp);

            user.animator.SetBool("NormalAttack", true);
            user.StartCoroutine(user.IsAnimEnd("NormalAttack"));

            resetDamage();
            return 0;
        }
        else {
            return getTurn();
        }
        
    }
}

public class Taunt : SpecialState
{

    public Taunt(int turn, BattleChar user, BattleChar target) : base(turn, user, target)
    {
        this.tg = StateTag.TAUNT;
    }

    public BattleChar getUser() {
        return user;
    }
}

public class Slow : SpecialState
{
    float incAP;

    public Slow(int turn, BattleChar user, BattleChar target, float incAP) : base(turn, user, target)
    {
        this.tg = StateTag.SLOW;
        this.incAP = incAP;
    }

    public float getIncAP() {
        if (turn > 0) return incAP;
        else return 1;
    
    }

    public override int UpdateState()
    {
        return --turn;
    }
}

public class Bleed : SpecialState
{
    float damage;

    public Bleed(int turn, BattleChar user, BattleChar target, float damage) : base(turn, user, target)
    {
        this.tg = StateTag.BLEED;
        this.damage = damage;
    }

    public override int UpdateState()
    {
        target.SetHP(target.getHP() - damage);
        target.setLastAttack(user);
        return --turn;
    }
}

public class Burn : SpecialState
{
    float damage;

    public Burn(int turn, BattleChar user, BattleChar target, float damage) : base(turn, user, target)
    {
        this.tg = StateTag.BURN;
        this.damage = damage;
    }

    public override int UpdateState()
    {
        target.SetHP(target.getHP() - damage);
        target.setLastAttack(user);
        return --turn;
    }
}

public class Poison : SpecialState
{
    float damage;

    public Poison(int turn, BattleChar user, BattleChar target, float damage) : base(turn, user, target)
    {
        this.tg = StateTag.POISON;
        this.damage = damage;
    }

    public override int UpdateState()
    {
        target.SetHP(target.getHP() - damage);
        target.setLastAttack(user);
        return --turn;
    }
}

