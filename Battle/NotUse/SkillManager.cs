using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    delegate void skill(GameObject u, List<GameObject> t, int lv);
    delegate string info(string str);
    delegate void counter(GameObject u, GameObject t, int lv, float damage);
    List<skill> skills = new List<skill>();
    List<info> infos = new List<info>();
    List<counter> counters = new List<counter>();

    public GameObject Buff;

    // Start is called before the first frame update
    void Start() // skills.add(함수명)로 스킬 추가
    {
        //skills.Add(Attak1);
        //infos.Add(Attak1);

        //skills.Add(Buff1);
        //infos.Add(Buff1);

        //counters.Add(KnightActive5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spell(GameObject u, List<GameObject> t, int id, int lv) {
        skills[id](u, t, lv);
    }

    public string GetInfo(int id, string str) {
        return infos[id](str);
    }


    //스킬 양식
    //void 스킬명(GameObject u, GameObject t) //id = 스킬 ID, 스킬 설명 { 스킬 내용 }
    //string 스킬명(string str) {// 양수,음수로 대상 피아구분, 절댓값으로 스킬 적용 객수 구분}

    //id = 0/ 공격 예시
    //void Attak1(GameObject u, List<GameObject> t, int lv) { 
    //    BattleChar bcU = u.GetComponent<BattleChar>();
    //    foreach (GameObject obj in t) {
    //        BattleChar bcT = obj.GetComponent<BattleChar>();
    //        bcT.GetDamage(bcU.phyATK, bcU.mgcATK, bcU.acc);
    //    }
        
    //}

    //string Attak1(string str) {
    //    if (str.Equals("NUM"))
    //    {
    //        return "1";
    //    }
    //    else if (str.Equals("TAG")) // 피아식별용
    //    {
    //        return "ENEMY"; //ENEMY or FRIEND or SELF
    //    }
    //    else if (str.Equals("IMG")) //스킬 버튼 이미지용/ Resources.Load() 사용
    //    {
    //        return "/Resources/ATTAK1.bmp";
    //    }

    //    else return "ERR";
    //}

    ////id = 1 / 버프 예시
    //void Buff1(GameObject u, List<GameObject> t, int lv)
    //{ 
    //    GameObject bufObj = Instantiate(Buff, u.transform);
    //    Buff src = bufObj.GetComponent<Buff>();
    //    BattleChar bcU = u.GetComponent<BattleChar>();

    //    src.turn = 3;
    //    src.buffMgcDEF = 0.1f;
    //    src.buffPhyDEF = 0.1f;
    //    src.buffPhyATK = -0.1f;

    //    foreach (GameObject obj in t)
    //    {
    //        BattleChar bcT = obj.GetComponent<BattleChar>();
    //        bcT.buffStack.Add(bufObj);
    //    }

    //}

    //string Buff1(string str)
    //{
    //    if (str.Equals("NUM"))
    //    {
    //        return "1";
    //    }
    //    else if (str.Equals("TAG")) // 피아식별용
    //    {
    //        return "SELF"; //ENEMY or FRIEND or SELF
    //    }
    //    else if (str.Equals("IMG")) //스킬 버튼 이미지용/ Resources.Load() 사용
    //    {
    //        return "/Resources/ATTAK1.bmp";
    //    }

    //    else return "ERR";
    //}


    ////기사 액티브 스킬
    ////id = 2 / 기사 액티스 스킬 1
    //void KnightActive1(GameObject u, List<GameObject> t, int lv)
    //{
    //    BattleChar bcU = u.GetComponent<BattleChar>();
    //    bcU.curMP -= 5 + 2 * lv;
    //    foreach (GameObject obj in t)
    //    {
    //        BattleChar bcT = obj.GetComponent<BattleChar>();
    //        float dam = bcT.GetDamage(bcU.phyATK * (1.2f + 0.05f*lv), 0, bcU.acc);

    //        if (Random.Range(0.0f, 100.0f) < 30 + lv) bcT.SturnTurn = 1 ;

    //        if (bcT.CountTurn > 0) counters[bcT.CountID](obj, u, bcT.CountLv, dam);
    //    }

    //}

    //string KnightActive1(string str)
    //{
    //    if (str.Equals("NUM"))
    //    {
    //        return "1";
    //    }
    //    else if (str.Equals("TAG")) // 피아식별용
    //    {
    //        return "ENEMY";  //ENEMY or FRIEND or SELF or ENEMYALL or FRIENDALL
    //    }
    //    else if (str.Equals("IMG")) //스킬 버튼 이미지용/ Resources.Load() 사용
    //    {
    //        return "/Resources/ATTAK1.bmp";
    //    }

    //    else return "ERR";
    //}

    ////id = 3 / 기사 액티스 스킬 2
    //void KnightActive2(GameObject u, List<GameObject> t, int lv)
    //{
    //    GameObject bufObj = Instantiate(Buff, u.transform);
    //    Buff src = bufObj.GetComponent<Buff>();
    //    BattleChar bcU = u.GetComponent<BattleChar>();
    //    bcU.curMP -= 3 + 1 * lv;

    //    src.turn = 3;
    //    src.buffMgcDEF = 0.1f + 0.05f * lv;
    //    src.buffPhyDEF = 0.1f + 0.05f * lv;
    //    src.buffPhyATK = -(0.1f + 0.01f * lv);

    //    bcU.TauntTurn = 3;

    //    foreach (GameObject obj in t)
    //    {
    //        BattleChar bcT = obj.GetComponent<BattleChar>();
    //        bcT.buffStack.Add(bufObj);
    //    }

    //}

    //string KnightActive2(string str)
    //{
    //    if (str.Equals("NUM"))
    //    {
    //        return "1";
    //    }
    //    else if (str.Equals("TAG")) // 피아식별용
    //    {
    //        return "SELF";  //ENEMY or FRIEND or SELF or ENEMYALL or FRIENDALL
    //    }
    //    else if (str.Equals("IMG")) //스킬 버튼 이미지용/ Resources.Load() 사용
    //    {
    //        return "/Resources/ATTAK1.bmp";
    //    }

    //    else return "ERR";
    //}

    ////id = 4 / 기사 액티스 스킬 3
    //void KnightActive3(GameObject u, List<GameObject> t, int lv)
    //{
    //    GameObject bufObj = Instantiate(Buff, u.transform);
    //    Buff src = bufObj.GetComponent<Buff>();
    //    BattleChar bcU = u.GetComponent<BattleChar>();
    //    bcU.curMP -= 5 + 3 * lv;

    //    src.turn = 1;
    //    src.buffPhyATK = -(0.1f + 0.02f * lv) ;

    //    bcU.TauntTurn = 3;

    //    foreach (GameObject obj in t)
    //    {
    //        BattleChar bcT = obj.GetComponent<BattleChar>();
    //        bcT.buffStack.Add(bufObj);
    //    }

    //}

    //string KnightActive3(string str)
    //{
    //    if (str.Equals("NUM"))
    //    {
    //        return "9";
    //    }
    //    else if (str.Equals("TAG")) // 피아식별용
    //    {
    //        return "ENEMYALL"; //ENEMY or FRIEND or SELF or ENEMYALL or FRIENDALL
    //    }
    //    else if (str.Equals("IMG")) //스킬 버튼 이미지용/ Resources.Load() 사용
    //    {
    //        return "/Resources/ATTAK1.bmp";
    //    }

    //    else return "ERR";
    //}

    ////id = 5 / 기사 액티스 스킬 4
    //void KnightActive4(GameObject u, List<GameObject> t, int lv)
    //{
    //    GameObject bufObj = Instantiate(Buff, u.transform);
    //    Buff src = bufObj.GetComponent<Buff>();
    //    BattleChar bcU = u.GetComponent<BattleChar>();
    //    bcU.curMP -= 2 + 2 * lv;

    //    src.turn = 3;
    //    src.buffMgcDEF = -(0.2f + 0.02f * lv);
    //    src.buffPhyDEF = -(0.2f + 0.02f * lv);
    //    src.buffPhyATK = 0.2f + 0.04f * lv;

    //    foreach (GameObject obj in t)
    //    {
    //        BattleChar bcT = obj.GetComponent<BattleChar>();
    //        bcT.buffStack.Add(bufObj);
    //    }

    //}

    //string KnightActive4(string str)
    //{
    //    if (str.Equals("NUM"))
    //    {
    //        return "9";
    //    }
    //    else if (str.Equals("TAG")) // 피아식별용
    //    {
    //        return "ENEMYALL"; //ENEMY or FRIEND or SELF or ENEMYALL or FRIENDALL
    //    }
    //    else if (str.Equals("IMG")) //스킬 버튼 이미지용/ Resources.Load() 사용
    //    {
    //        return "/Resources/ATTAK1.bmp";
    //    }

    //    else return "ERR";
    //}

    ////id = 6, counterID = 0 / 기사 액티스 스킬 5
    //void KnightActive5(GameObject u, List<GameObject> t, int lv)
    //{
    //    GameObject bufObj = Instantiate(Buff, u.transform);
    //    Buff src = bufObj.GetComponent<Buff>();
    //    BattleChar bcU = u.GetComponent<BattleChar>();
    //    bcU.curMP -= 6 + 6 * lv;

    //    foreach (GameObject obj in t)
    //    {
    //        BattleChar bcT = obj.GetComponent<BattleChar>();
    //        bcT.CountTurn = 3;
    //        bcT.CountID = 0;
    //        bcT.CountLv = lv;
    //    }

    //}

    //string KnightActive5(string str)
    //{
    //    if (str.Equals("NUM"))
    //    {
    //        return "1";
    //    }
    //    else if (str.Equals("TAG")) // 피아식별용
    //    {
    //        return "SELF"; //ENEMY or FRIEND or SELF or ENEMYALL or FRIENDALL
    //    }
    //    else if (str.Equals("IMG")) //스킬 버튼 이미지용/ Resources.Load() 사용
    //    {
    //        return "/Resources/ATTAK1.bmp";
    //    }

    //    else return "ERR";
    //}


    //void KnightActive5(GameObject u, GameObject t, int lv, float damage) {
    //    GameObject bufObj = Instantiate(Buff, u.transform);
    //    Buff src = bufObj.GetComponent<Buff>();
    //    BattleChar bcU = u.GetComponent<BattleChar>();
    //    bcU.curMP -= 6 + 6 * lv;

       
    //    BattleChar bcT = t.GetComponent<BattleChar>();
    //    bcT.GetDamage(0, damage * (bcU.mgcATK * 5 + 0.5f * lv), bcU.acc);
        
    //}

}
