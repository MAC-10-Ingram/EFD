using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleChar : MonoBehaviour
{

    public DataChar DC;//캐릭터의 정보들을 담고 있는 객체

    [Header("- Action Point")]
    public float curAP;//현재AP
    float buffIncAP; //AP 증가 속도 버프
    float stateIncAP; //AP 증가 통제 변수

    //[Header("- Physical Stat")]
    float buffPhyATK;//물리뎀 버프, 디버프 by 스킬
    float buffPhyDEF;//물리방 버프, 디버프 by 스킬
    float buffPhyDAM;//물리뎀

    //[Header("- Magical Stat")]
    float buffMgcATK;//마법뎀 버프, 디버프 by 스킬
    float buffMgcDEF;//마법방 버프, 디버프 by 스킬
    float buffMgcDAM;//마법뎀

    //[Header("- Speed, Accuracy, Avoid")]
    float buffCrit; //크리티컬 확률 버프
    float buffFinalDam; //최종 데미지 버프

    //GameObject panel;
    public GameObject body; //캐릭터 객체
    public SpriteRenderer render; //스프라이트, 애니메이션 통제용


    //특수 상태
    //기절, 도발, 속도감소, 출혈, 화상
    [Header("- State 상태")]
    public Sturn stateSturn;
    public Counter stateCount;
    public List<Taunt> stateTaunt;
    public int tauntTurn;

    public List<Slow> stateSlow;
    public List<Burn> stateBurn;
    public List<Bleed> stateBleed;
    public List<Poison> statePoison;

    [Header("- Position")]
    public int xPos;
    public int yPos;

    //궁수용 계수들, 추후에 분리 에정


    public List<Buff> buffStack;//모든 버프
    public List<Buff> debuffStack;//디버프
    public List<Buff> eDebuffStack;//반영구 디버프
    public int stigma; //낙인

    public bool healSwitch; //회복 가능 여부


    public BattleManager BM; //전투 관리용 객체

    public GameObject floor;//필드의 바닥 객체

    public TARGETTAG tg; //피아 식별용 태그


    BattleChar lastAttacked;// 마지막에 자신을 공격한 타겟
    BattleChar attackLast; // 자신이 마지막에 공격한 타겟
    public List<Skill> killStack; // 상대편을 죽였을때 발동되는 스킬들
    public List<CounterSkills> damagedStack; // 공격 받을시 사용되는 스킬들
    public List<CounterSkills> phyDamagedStack; // 공격 받을시 사용되는 스킬들
    public List<CounterSkills> mgcDamagedStack; // 공격 받을시 사용되는 스킬들


    public Animator animator;// 애니메이션 출력용
                             //public string animState;

    public enum ATK_TYPE { PHY, MGC };

    public ProgressBar AP;
    public ProgressBar MP;
    public ProgressBar HP;



    void Start()
    {
        buffStack = new List<Buff>();
        debuffStack = new List<Buff>();
        eDebuffStack = new List<Buff>();
        stigma = 0;

        BM = GameObject.Find("BattleManager").GetComponent<BattleManager>();
        animator = GetComponentInChildren<Animator>();

        stateBurn = new List<Burn>();
        stateBleed = new List<Bleed>();
        statePoison = new List<Poison>();
        stateSlow = new List<Slow>();
        stateTaunt = new List<Taunt>();
        tauntTurn = 0;
        healSwitch = true;

        killStack = new List<Skill>();
        damagedStack = new List<CounterSkills>();
        phyDamagedStack = new List<CounterSkills>();
        mgcDamagedStack = new List<CounterSkills>();

        //각종 버프들 / 0으로 초기화
        ClearBuff();

        //행동력 / 최댓값으로 초기화
        curAP = 1.0f;
        stateIncAP = 1.0f;

        render = this.GetComponentInChildren<SpriteRenderer>();
        lastAttacked = null;

        initDC();
    }

    public void initDC() {
        DC.charSkillSet.setUser(this);
        //DC.charSkillSet.setSkillSet();
        //eDebuffStack.Add(DC.charEquptSet.getBuff());
    }

    public void changeAnimControl(string key) {
        animator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("battle/" + key + "/sprite/idle/AnimControl");
    }

    // Update is called once per frame
    void Update()
    {
        AP.setProgress(curAP);
        MP.setProgress(DC.curMP/getMaxMP());
        HP.setProgress(DC.curHP/getMaxHP());
    }

    //보호중인 캐릭터를 반환
    BattleChar getTaunt()
    {
        BattleChar taunter = null;

        foreach (Taunt t in stateTaunt)
        {
            if (t.getUser() != null && t.getTurn() > 0) taunter = t.getUser();
        }

        return taunter;
    }

    //타겟을 반환하는 함수, 보호중인 캐릭터가 없으면 스스로를 반환
    public BattleChar getTarget()
    {
        BattleChar tmp = getTaunt();
        if (tmp == null) return this;
        else return tmp;
    }

    //턴 종료떄 호출, 특수상태들을 업데이트
    public void StateUpdate()
    {
        //기절
        if (stateSturn != null) {
            if (stateSturn.getTurn() > 0) stateSturn.UpdateState();
        }

        //반격
        if (stateCount != null) {
            if (stateCount.getTurn() > 0) stateCount.UpdateState();
        }

        //속도 감소
        stateIncAP = 1.0f;
        foreach (Slow s in stateSlow) {
            if (s.getTurn() > 0) {
                stateIncAP *= s.getIncAP();
                s.UpdateState();
            }
        }

        //도발
        tauntTurn--;
        foreach (Taunt t in stateTaunt) {
            if (t.getTurn() > 0) t.UpdateState();
        }

        //중독 데미지
        foreach (Poison p in statePoison)
        {
            if (p.getTurn() > 0) p.UpdateState();
        }

        //화상 데미지
        foreach (Burn b in stateBurn)
        {
            if (b.getTurn() > 0) b.UpdateState();
        }

        //출혈 데미지
        foreach (Bleed b in stateBleed)
        {
            if (b.getTurn() > 0) b.UpdateState();
        }
    }

    //턴 시작시 호출
    //자신이 가지고 있는 모든 버프들을 스스로에게 적용

    void applyBuff(List<Buff> b) {
        foreach (Buff tmp in b)
        {
            if (tmp.turn < 0)
            {
                break;
            }
            buffMgcATK *= tmp.buffMgcATK;
            buffMgcDEF *= tmp.buffMgcDEF;
            buffPhyATK *= tmp.buffPhyATK;
            buffPhyDEF *= tmp.buffPhyDEF;
            buffIncAP *= tmp.buffIncAP;
            buffPhyDAM *= tmp.getPhyDam();
            buffMgcDAM *= tmp.getMgcDam();
            buffCrit *= tmp.buffCrit;
            buffFinalDam *= tmp.buffFinalDam;

            tmp.updateTurn();

        }
    }

    public void UpdateBuff()
    {
        ClearBuff();

        applyBuff(buffStack);
        applyBuff(debuffStack);
        applyBuff(eDebuffStack);

        foreach (CounterSkills tmp in damagedStack)
        {
            tmp.updateTurn();
        }

        foreach (CounterSkills tmp in phyDamagedStack)
        {
            tmp.updateTurn();
        }

        foreach (CounterSkills tmp in mgcDamagedStack)
        {
            tmp.updateTurn();
        }
    }

    public void EndBuff() {
        foreach (Buff tmp in buffStack)
        {
            if (tmp.turn < 0)
            {
                break;
            }

            tmp.endTurn();
        }

        foreach (Buff tmp in debuffStack)
        {
            if (tmp.turn < 0)
            {
                break;
            }

            tmp.endTurn();
        }

        foreach (Buff tmp in eDebuffStack)
        {
            if (tmp.turn < 0)
            {
                break;
            }

            tmp.endTurn();
        }
    }

    //적용된 버프들을 모두 초기화
    public void ClearBuff()
    {
        buffMgcATK = 1.0f;
        buffMgcDEF = 1.0f;
        buffPhyATK = 1.0f;
        buffPhyDEF = 1.0f;
        buffIncAP = 1.0f;
        buffMgcDAM = 1.0f;
        buffPhyDAM = 1.0f;
        buffCrit = 1.0f;
        buffFinalDam = 1.0f;
    }

    //행동력 업데이트
    public void UpdateAP()
    {
        if (curAP + GetIncAP() > 1.0f) curAP = 1.0f;
        else curAP += GetIncAP();
    }

    //현재 행동력을 반환
    public float GetAP()
    {
        return curAP;
    }

    //행동력이 꽉찼는지 확인
    public bool IsFullAP()
    {
        return (curAP >= 1.0f) ? true : false;
    }

    //행동력 초기화
    public void ClearAP()
    {
        curAP = .0f;
    }

    //공격을 받았을시 호출
    public void Damaged(BattleChar user) {
        foreach (CounterSkills ct in damagedStack)
        {
            ct.useSkill(user);
        }
        BM.StartHit(1.0f, this, new Vector4(1, 0.5f, 0.5f, 1));
    }

    public void Healed() {
        BM.StartHit(1.0f, this, new Vector4(0.5f, 1, 0.5f, 1));
    }

    public void poisoned() {
        BM.StartHit(1.0f, this, new Vector4(1, 0.5f, 1, 1));
    }

    public void buffed() {
        BM.StartHit(1.0f, this, new Vector4(0.5f, 0.5f, 1, 1));
    }

    public void DamagedMgc(BattleChar user) {
        Damaged(user);
        foreach (CounterSkills ct in mgcDamagedStack) {
            ct.useSkill(user);
        }
    }

    public void DamagedPhy(BattleChar user) {
        Damaged(user);
        foreach (CounterSkills ct in phyDamagedStack)
        {
            ct.useSkill(user);
        }
    }

    //마법 데미지
    public float SetMagicDamage(BattleChar user, float damCoef, int lv, float coef)
    {float totalDamage = .0f;

        totalDamage = (user.GetMgcATK() * damCoef * (1 + (lv * 0.005f))) - (user.GetMgcATK() * damCoef * (GetMgcDEF() / (50 + (100 - BM.floorLevel) * 1.5f + GetMgcDEF())) * 1.4f) * coef;
        totalDamage *= buffMgcDAM;

        if (stateCount != null) { stateCount.addDamage(totalDamage); }

        print(this.transform.name + " is damaged " + totalDamage);
        if (SetHP(getHP() - totalDamage) > 0) { DamagedMgc(user); }

        return totalDamage;
    }

    //물리 데미지
    public float SetPhysicalDamage(BattleChar user, float damCoef, int lv, float coef)
    {
        float totalDamage = .0f;

        totalDamage = (user.GetPhyATK() * damCoef * (1 + (lv * 0.005f))) - (user.GetPhyATK() * damCoef * (GetPhyDEF() / (50 + (100 - BM.floorLevel) * 1.5f + GetPhyDEF())) * 1.4f) * coef;
        totalDamage *= buffPhyDAM;

        if (stateCount != null) stateCount.addDamage(totalDamage);

        print(this.transform.name + " is damaged " + totalDamage);
        if (SetHP(getHP() - totalDamage) > 0) { DamagedPhy(user); }

        return totalDamage;
    }

    public float getSPD()
    {
        return DC.getSPD();
    }

    public float getHP()
    {
        return DC.curHP;
    }

    public float getMaxHP() {
        return DC.getMaxHP();
    }

    public float getMaxMP() {
        return DC.getMaxMP();
    }

    public float SetHP(float newHP)
    {
        if (!healSwitch) {
            if (newHP > DC.curHP) {
                return DC.curHP;
            }
        }

        if (newHP > getMaxHP()) return DC.curHP = getMaxHP();
        else if (newHP <= 0.0f) { Death(); return -1; }
        else return DC.curHP = newHP;
    }

    void Death() {
        BM.StartDie(1.0f, this);
        lastAttacked.killScore();
    }

    public float GetPhyATK()
    {
        return DC.getPhyATK() * buffPhyATK + DC.charEquipSet.getPhyATK();
    }

    public float GetPhyDEF()
    {
        return DC.getPhyDEF() * buffPhyDEF + DC.charEquipSet.getPhyDEF();
    }

    public float GetMgcATK()
    {
        return DC.getMgcATK() * buffMgcATK + DC.charEquipSet.getMgcATK();
    }

    public float GetMgcDEF()
    {
        return DC.getMgcDEF() * buffMgcDEF + DC.charEquipSet.getMgcDEF();
    }

    public float GetIncAP() {
        return DC.getIncAP() * (buffIncAP + DC.charEquipSet.getIncAP()) * stateIncAP * Time.deltaTime;
    }

    public float getCritical() {
        return DC.critical * (buffCrit + DC.charEquipSet.getCritical());
    }

    public float setMP(float newMP) {
        if (newMP < 0) return DC.curMP = 0;
        else if (newMP > getMaxMP()) return DC.curMP = getMaxMP();
        else return DC.curMP = newMP;
    }

    public float getMP() { return DC.curMP; }

    public BattleChar setLastAttack(BattleChar bc) {
        return lastAttacked = bc;
    }

    public BattleChar getLastAttack() {
        return lastAttacked;
    }

    //상대편을 죽였을때 호출되는 스킬들을 발동시키는 트리거
    public void killScore() {
        foreach (Skill k in killStack) {
            k.useSkill(BM.enemyBC);
        }
    }

    //캐릭터 애니메이션 출력용
    public IEnumerator IsAnimEnd(string animState) {
        BM.animStack++;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(animState))
        {
            //전환 중일 때 실행되는 부분
            //print(animState +" is ready");
            yield return null;
        }

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            //애니메이션 재생 중 실행되는 부분
            BM.curState = BattleManager.BattleState.ANIM;
            yield return null;
        }

        
        //애니메이션 완료 후 실행되는 부분
        animator.SetBool(animState, false);
        animState = "Idle";
        BM.animStack--;
    }

    public float getFinalDamage() { return buffFinalDam; }
}
