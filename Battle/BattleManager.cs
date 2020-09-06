using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class BattleManager : MonoBehaviour
{    
    public List<GameObject> myBattleChars; //플레이어 캐릭터들
    public List<BattleChar> myBC;//플레이어 캐릭터들의 BattleChar
    public List<BattleChar> myDeadBC;//플레이어 캐릭터들의 죽은 BattleChar

    public List<GameObject> enemys; //적 캐릭터들
    public List<BattleChar> enemyBC;//적 캐릭터들의 BattleChar
    public List<BattleChar> enemyDeadBC;//적 캐릭터들의 죽은 BattleChar


    public List<GameObject> readyPlayer; //행동력이 꽉찬 캐릭터들
    public List<BattleChar> readyBC; //행동력이 꽉찬 캐릭터들의 BattleChar



    public GameObject skWIndow; //스킬창
    public GameObject[] buttons; //스킬창의 버튼들

    public GameObject[,] myField; //플레이어 캐릭터들이 배치되는 장소
    public GameObject[,] enemyField; //적 캐릭터들이 배치되는 ㅂ아소

    public int[,] myPosition; //타일 인데스 번호
    public int myCharNum; //플레이어 캐릭터들의 갯수
    public int[,] enemyPos; //타일 인덱스 번호
    public int enemyNum; //적 캐릭터들의 갯수

    //더미 데이터들
    public GameObject floor; //임시 발판
    public GameObject dummy; //임시 플레이어 캐릭터
    public GameObject enemyCube; //임시 적 캐릭터

    public float floorStep;

    //스킬 관련 정보들
    public Skill sklSelected; //선택된 스킬
    Skill nullSkl;

    public Vector3 charSize;

    Skill normal; //평타
    BattleChar player;//스킬 시전자
    TARGETTAG tg;//스킬 대상 분류
    int targetNum;//스킬 대상의 갯수
    public List<BattleChar> target;//스킬 시전 대상
    public List<Skill> skills;//스킬 목록

    //현재 층
    public int floorLevel;

    //도발로 보호받는 대상 전달용
    List<BattleChar> guarded;

    public int animStack; //출력중인 애니메이션 갯수

    public enum BattleState {INIT, READY, START, TARGET, SPELL, ANIM, END, OVER };
    //INIT : 본격적인 전투시작 전 페이즈
    //READY : 준비된 캐릭터가 없는 경우, 모든 캐릭터의 AP를 Update할것
    //START : 준비된 캐릭터에 맞추어 설정하는 단계, 스턴 확인
    //TARGET : 스킬과 대상을 선택, 도발 확인
    //SPELL : 스킬을 적용되는 단계
    //END : 스킬을 사용한 이후 단계, 각종 특수 상태 데미지 확인
    //ANIM : 애니메이션 재생중
    //OVER : 전투 종료단계

    public BattleState curState; //현재 턴의 상태
    RaycastHit hit; //히트스캔을 위해서 사용

    public Camera mainCam;
    Vector3 camOrigin;
    float camSize;

    Vector3 centerPos;
    Vector3 originPos;



    // Start is called before the first frame update
    void Start()
    {
        myBattleChars = new List<GameObject>();
        enemys = new List<GameObject>();
        readyPlayer = new List<GameObject>();
        target = new List<BattleChar>();

        myBC = new List<BattleChar>();
        enemyBC = new List<BattleChar>();
        readyBC = new List<BattleChar>();

        myDeadBC = new List<BattleChar>();
        enemyDeadBC = new List<BattleChar>();

        myField = new GameObject[3, 3];
        enemyField = new GameObject[3, 3];

        buttons = new GameObject[5];

        myPosition = new int[myCharNum, 2];
        enemyPos = new int[enemyNum, 2];

        skills = new List<Skill>();

        guarded = new List<BattleChar>();

        List<DataChar> DCList = new List<DataChar>();

        nullSkl = new Skill(new SkillSet());
        sklSelected = nullSkl;

        charSize = new Vector3(2.2f, 2.2f, 2.2f);

        mainCam = Camera.main;
        camOrigin = mainCam.transform.position;
        camSize = mainCam.orthographicSize;
        floorStep = 1.5f;

        //버튼 객체들을 저장
        for (int i = 0; i < 5; i++) {
            buttons[i] = skWIndow.transform.GetChild(i).gameObject;
        }

        //플레이어, AI 정보 불러오기

        //필드 구성
        for (int i = 0; i<3; i++) {
            for (int j = 0; j<3; j++) {
                myField[i, j] = Instantiate(floor, new Vector3(i * floorStep, 0, j * floorStep), Quaternion.identity);
                myField[i, j].name = "my floor("+i+","+j+")";
                enemyField[i, j] = Instantiate(floor, new Vector3(i * floorStep + 11.5f, 0, j * floorStep), Quaternion.identity);
                enemyField[i, j].name = "enemy floor(" + i + "," + j + ")";
            }
        }


        centerPos = (myField[1, 1].transform.position + enemyField[1, 1].transform.position) / 2;

        //불러온 정보를 기반으로 발판에 캐릭터 생성 및 리스트에 추가
        //플레이어 캐릭터 생성
        //for (int i = 0; i < myCharNum; i++) {
        //    GameObject tmp = Instantiate(dummy, myField[i, i].transform.position, Quaternion.identity);
        //    tmp.tag = "FRIEND";
        //    BattleChar bc = GetComponent<BattleChar>();
        //    bc.xPos = i;
        //    bc.yPos = i;
        //    bc.floor = myField[i, i];
        //    bc.tg = TARGETTAG.FRIEND;
        //    bc.body = tmp;


        //    myBattleChars.Add(tmp);
        //    myBC.Add(bc);
        //    tmp.name = "friend knight " + i +" ";
        //}

        DCList.Add(DataChar.getKnight());
        DCList.Add(DataChar.getArcher());
        DCList.Add(DataChar.getThief());
        DCList.Add(DataChar.getMage());
        DCList.Add(DataChar.getPriest());
        DCList.Add(DataChar.getNormalEnemy());
        DCList.Add(DataChar.getNormalEnemy());
        DCList.Add(DataChar.getNormalEnemy());



        //적 캐릭터 생성
        for (int i = 0; i < enemyNum; i++)
        {
            GameObject tmp = Instantiate(enemyCube, enemyField[i, i].transform.position, Quaternion.identity);
            tmp.transform.localScale = charSize;
            tmp.tag = "ENEMY";
            tmp.GetComponent<BattleChar>().xPos = i;
            tmp.GetComponent<BattleChar>().yPos = i;
            tmp.GetComponent<BattleChar>().floor = enemyField[i, i];
            tmp.GetComponent<BattleChar>().tg = TARGETTAG.ENEMY;
            tmp.GetComponent<BattleChar>().body = tmp;



            enemys.Add(tmp);
            while (!tmp.GetComponent<BattleChar>()) enemyBC.Add(tmp.GetComponent<BattleChar>());
            tmp.name = "enemy knight " + i + " ";
            tmp.GetComponent<BattleChar>().DC = DCList[5 + i];
        }

       

        //버튼에 이벤트 리스너 부착
        buttons[0].GetComponent<Button>().onClick.AddListener(delegate () { sklSelected = normal ; curState = BattleState.START; });
        buttons[1].GetComponent<Button>().onClick.AddListener(delegate () { sklSelected = skills[0]; curState = BattleState.START; });
        buttons[2].GetComponent<Button>().onClick.AddListener(delegate () { sklSelected = skills[1]; curState = BattleState.START; });
        buttons[3].GetComponent<Button>().onClick.AddListener(delegate () { sklSelected = skills[2]; curState = BattleState.START; });
        buttons[4].GetComponent<Button>().onClick.AddListener(delegate () { sklSelected = skills[3]; curState = BattleState.START; });

        skWIndow.SetActive(false);

        hit = new RaycastHit();
        curState = BattleState.INIT;
        animStack = 0;

        testSpawnChar(DCList[0], 2, 2, DCList[0].cls);
        testSpawnChar(DCList[1], 1, 1, DCList[1].cls);
        testSpawnChar(DCList[2], 0, 0, DCList[2].cls);
        testSpawnChar(DCList[3], 0, 2, DCList[3].cls);
        testSpawnChar(DCList[4], 2, 0, DCList[4].cls);


        Invoke( "afterStart", 0.001f);
    }

    void afterStart() {
        foreach (BattleChar bc in myBC)
        {
            bc.changeAnimControl(bc.DC.cls);
        }
    }

    public void player2center() {
        player.transform.position = centerPos;
    }

    public void player2target(Vector3 target) {
        Vector3 tmp = Vector3.zero;

        tmp.z = target.z;//상하
        tmp.x = target.x;//좌우
        tmp.y += 0.1f;

        tmp.x += (target.x - originPos.x > 0) ? -floorStep : floorStep;

        player.transform.position = tmp;
    }

    public void player2origin()
    {
        player.transform.position = originPos;
    }

    public void zoomPlayerCam() {
        Vector3 pos = new Vector3(player.transform.position.x, Mathf.Tan(Mathf.Deg2Rad * 60) * Mathf.Abs(player.transform.position.z - mainCam.transform.position.z), mainCam.transform.position.z);

        //camMove(pos,1.7f);
        StartCoroutine(zoomEffect(pos,1.7f, 0.2f ));
    }

    public void resetCam() {
        camMove(camOrigin, camSize);
    }

    IEnumerator zoomEffect(Vector3 pos, float size, float time) {
        for (float f = 0.0f; f <= 1.0f; f += Time.deltaTime/time) {

            camMove(camOrigin * (1.0f - f) + pos * f ,camSize * (1.0f - f) + size * f );

            yield return null;
        }

    }

    //카메라를 움직이는 함수
    void camMove(Vector3 pos, float size) {
        print("cam move to "+ pos);
        mainCam.orthographicSize = size;
        mainCam.transform.position = pos;
    }

    void testSpawnChar(DataChar dc, int i, int j, string name) {
        GameObject tmp = Instantiate(dummy, myField[i, j].transform.position, Quaternion.identity);
        tmp.transform.localScale = charSize;
        tmp.tag = "FRIEND";

        tmp.GetComponent<BattleChar>().xPos = i;
        tmp.GetComponent<BattleChar>().yPos = j;
        tmp.GetComponent<BattleChar>().floor = myField[i, j];
        tmp.GetComponent<BattleChar>().tg = TARGETTAG.FRIEND;
        tmp.GetComponent<BattleChar>().body = tmp;


        myBattleChars.Add(tmp);
        while (!tmp.GetComponent<BattleChar>()) { }
        myBC.Add(tmp.GetComponent<BattleChar>());
        tmp.name = name;
        tmp.GetComponent<BattleChar>().DC = dc;
        tmp.GetComponent<BattleChar>().initDC();

    }

    void testSpawnEnemy(DataChar dc, int i, int j, string name)
    {
        GameObject tmp = Instantiate(dummy, enemyField[i, j].transform.position, Quaternion.identity);
        tmp.transform.localScale = charSize;
        tmp.tag = "FRIEND";

        tmp.GetComponent<BattleChar>().xPos = i;
        tmp.GetComponent<BattleChar>().yPos = j;
        tmp.GetComponent<BattleChar>().floor = enemyField[i, j];
        tmp.GetComponent<BattleChar>().tg = TARGETTAG.FRIEND;
        tmp.GetComponent<BattleChar>().body = tmp;


        enemys.Add(tmp);
        while (!tmp.GetComponent<BattleChar>()) { }
        enemyBC.Add(tmp.GetComponent<BattleChar>());
        tmp.name = name;
        tmp.GetComponent<BattleChar>().DC = dc;

    }





    // Update is called once per frame
    void Update()
    {
        switch (curState) {
            case BattleState.INIT:
                //전투 시작전 준비 페이즈

                //모든 캐릭터 스킬 초기화
                foreach (BattleChar bc in myBC)
                {
                    while (bc.DC.charSkillSet == null) { }
                    bc.DC.charSkillSet.initSkillSet();
                }
                foreach (BattleChar bc in enemyBC)
                {
                    while (bc.DC.charSkillSet == null) { }
                    bc.DC.charSkillSet.initSkillSet();
                }

                curState++;
                break;

            case BattleState.OVER:

                foreach (BattleChar bc in myBC)
                {
                    bc.DC.charSkillSet.resetSkillSet();
                }

                print("Battle Over!");//yes//전투 결과를 업데이트한다. 이동씬으로 복귀한다.


                break;

                
            case BattleState.READY: 
                //전투 전 페이즈
                //AP가 꽉찬 캐릭터 체크/ 전멸한 팀이 있는지 체크/ AP 업데이트
                //아군 또는 적군 유닛이 전멸하였는가?
                if ((myBattleChars.Count <= 0 || enemys.Count <= 0))
                {
                    curState = BattleState.OVER;
                    break;
                }
                //no//전투를 지속한다.

                //행동력이 꽉찬 캐릭터가 있는가?
                //yes//스킬사용
                else if (readyPlayer.Count > 0) {
                    curState++;
                }
                //no//행동력이 꽉찰때까지 업데이트
                else
                {
                    //모든 살아있는 캐릭터의 행동력 올리기 / 행동력이 꽉찬 캐릭터는 readyPlayer List에 추가
                    foreach (GameObject T in myBattleChars.ToArray())
                    {
                        BattleChar srt = T.GetComponent<BattleChar>();
                        if (srt != null)
                        {
                            if (srt.getHP() > 0.0f)
                            {
                                srt.UpdateAP();
                                if (srt.IsFullAP())
                                {
                                    readyPlayer.Add(T);
                                    readyBC.Add(srt);
                                    srt.UpdateBuff();
                                }
                            }
                        }
                    }
                    foreach (GameObject T in enemys.ToArray())
                    {
                        BattleChar srt = T.GetComponent<BattleChar>();
                        if (srt != null)
                        {
                            if (srt.getHP() > 0.0f)
                            {
                                srt.UpdateAP();
                                if (srt.IsFullAP())
                                {
                                    readyPlayer.Add(T);
                                    readyBC.Add(srt);
                                    srt.UpdateBuff();
                                }

                            }
                        }
                    }

                    //행동력이 꽉찬 캐릭터들을 스피드 순으로 정렬
                    readyPlayer.Sort(delegate (GameObject a, GameObject b) { return a.GetComponent<BattleChar>().getSPD().CompareTo(b.GetComponent<BattleChar>().getSPD()); });
                    readyBC.Sort(delegate (BattleChar a, BattleChar b) { return a.getSPD().CompareTo(b.getSPD()); });
                }

                break;
            case BattleState.START: 
                //전투 시작, 배틀매니저와 캐릭터를 셋업
                player = readyPlayer[0].GetComponent<BattleChar>();
                target.Clear();
                ClearTile();
                originPos = player.transform.position;

                if (player.stateSturn != null)
                {
                    if (player.stateSturn.getTurn() > 0)
                    {
                        player.curAP = 0;
                        curState = BattleState.END;
                        break;
                    }
                }
                else if (player.stateCount != null) {
                    if (player.stateCount.getTurn() > 0)
                    {
                        player.curAP = 0;
                        curState = BattleState.END;
                        break;
                    }
                }

                // 스킬 ID를 스킬 객체에 전달
                skills.AddRange(player.DC.charSkillSet.set);
                for (int i = 0; i < 4; i++) {
                    if (skills[i].type == SKILLTYPE.PAS)
                    {
                        //패시브
                        buttons[i + 1].GetComponent<Button>().interactable = false;
                    }
                    else if (!skills[i].testMP()) {
                        //마나가 부족한 경우
                        buttons[i + 1].GetComponent<Button>().interactable = false;
                    }
                    else
                    {
                        //액티브 스킬이고 마나가 충분한 경우
                        if (skills[i].targetTag == TARGETTAG.FRIENDDEAD)
                        {
                            if (myDeadBC.Count <= 0)
                            {
                                buttons[i + 1].GetComponent<Button>().interactable = false;
                                break;
                            }
                        }

                        if (skills[i].targetTag == TARGETTAG.ENEMYDEAD)
                        {
                            if (myDeadBC.Count <= 0)
                            {
                                buttons[i + 1].GetComponent<Button>().interactable = false;
                                break;
                            }
                        }


                        buttons[i + 1].GetComponent<Button>().interactable = true;
                    }
                }
                normal = player.DC.charSkillSet.normal;

                //버튼 이미지 스킬 아이디에 맞추어 변경하기
                buttons[0].GetComponent<Image>().sprite = Resources.Load<Sprite>(normal.image);

                for (int i = 0; i < 4; i++) { 
                    buttons[1+i].GetComponent<Image>().sprite = Resources.Load<Sprite>(skills[i].image);
                }

                //현재 스킬 사용중인 캐릭터 보여주기용
                

                //스킬 사용창 활성화
                if(player.tg == TARGETTAG.FRIEND)skWIndow.SetActive(true);

                curState++;
                break;
            case BattleState.TARGET:
                //스킬 선택 및 공격할 타겟 선택
                ClearTile();

                player.floor.GetComponent<MeshRenderer>().material.color = Color.yellow;
                player.floor.SetActive(true);

                //플레이어 캐릭터 
                if (player.tg == TARGETTAG.FRIEND) 
                {

                    //스킬 선택하기 in 스킬 메뉴
                    //스킬 사용 가능 타겟 표시하기 및 설정
                    if (!sklSelected.Equals(nullSkl))
                    {
                        //print(sklSelected.level);
                        switch (sklSelected.targetTag) { 
                            case TARGETTAG.ENEMY:
                                foreach (GameObject obj in enemys.ToArray())
                                {
                                    obj.GetComponent<BattleChar>().getTarget().floor.GetComponent<MeshRenderer>().material.color = Color.red;
                                    obj.GetComponent<BattleChar>().getTarget().floor.SetActive(true);
                                }
                                targetNum = sklSelected.targetNum;
                                tg = sklSelected.targetTag;

                                break;

                            case TARGETTAG.ENEMYALL:
                                foreach (GameObject obj in enemyField)
                                {
                                    obj.GetComponent<MeshRenderer>().material.color = Color.red;

                                    obj.SetActive(true);
                                }
                                targetNum = enemys.Count;
                                tg = TARGETTAG.ENEMY;

                                break;

                            case TARGETTAG.FRIEND:
                                foreach (GameObject obj in myBattleChars.ToArray())
                                {
                                    obj.GetComponent<BattleChar>().floor.GetComponent<MeshRenderer>().material.color = Color.green;

                                    obj.GetComponent<BattleChar>().getTarget().floor.SetActive(true);
                                }
                                targetNum = sklSelected.targetNum;
                                tg = TARGETTAG.FRIEND;

                                break;

                            case TARGETTAG.FRIENDALL:
                                foreach (GameObject obj in myField)
                                {
                                    obj.GetComponent<MeshRenderer>().material.color = Color.green;
                                    obj.SetActive(true);
                                }
                                targetNum = myBattleChars.Count;
                                tg = TARGETTAG.FRIEND;

                                break;

                            case TARGETTAG.SELF:
                                player.floor.GetComponent<MeshRenderer>().material.color = Color.green;
                                player.getTarget().floor.SetActive(true);
                                targetNum = 1;
                                tg = TARGETTAG.FRIEND;

                                break;

                            case TARGETTAG.FRIENDDEAD:
                                foreach (BattleChar bc in myDeadBC)
                                {
                                    bc.floor.GetComponent<MeshRenderer>().material.color = Color.green;
                                    bc.getTarget().floor.SetActive(true);
                                }

                                break;

                            case TARGETTAG.ENEMYDEAD:
                                foreach (BattleChar bc in enemyDeadBC)
                                {
                                    bc.floor.GetComponent<MeshRenderer>().material.color = Color.red;
                                    bc.getTarget().floor.SetActive(true);
                                }

                                break;

                            case TARGETTAG.NONE:

                                break;

                        }

                        //타겟 선택하기
                        SelectTarget(target);

                        //타겟 선택이 끝나면
                        if (target.Count == targetNum)
                        {
                            curState++;//다음 단계로 이동
                            skWIndow.SetActive(false); // 스킬창 닫기
                            ClearTile(); //발판 색 초기화
                            break;
                        }
                    }

                }
                //적 캐릭터 
                else
                {
                    // 스킬 선택
                    sklSelected = ((EnemySkillSet)player.DC.charSkillSet).getSkill();

                    // 타겟 선택
                    target.AddRange( ((EnemyActiveSkills)sklSelected).getTarget() );

                    // 테스트 용도로 적 캐릭터 스킬도 사용자가 선택함, 실제 사용시 AI를 구현 적용해야됨
                    //if (!sklSelected.Equals(nullSkl))
                    //{
                    //    //print(sklSelected.level);
                    //    switch (sklSelected.targetTag)
                    //    {
                    //        case TARGETTAG.ENEMY:
                    //            foreach (GameObject obj in enemys.ToArray())
                    //            {
                    //                obj.GetComponent<BattleChar>().getTarget().floor.GetComponent<MeshRenderer>().material.color = Color.red;
                    //                obj.GetComponent<BattleChar>().getTarget().floor.SetActive(true);
                    //            }
                    //            targetNum = sklSelected.targetNum;
                    //            tg = sklSelected.targetTag;

                    //            break;

                    //        case TARGETTAG.ENEMYALL:
                    //            foreach (GameObject obj in enemyField)
                    //            {
                    //                obj.GetComponent<MeshRenderer>().material.color = Color.red;
                    //                obj.SetActive(true);
                    //            }
                    //            targetNum = enemys.Count;
                    //            tg = TARGETTAG.ENEMY;

                    //            break;

                    //        case TARGETTAG.FRIEND:
                    //            foreach (GameObject obj in myBattleChars.ToArray())
                    //            {
                    //                obj.GetComponent<BattleChar>().floor.GetComponent<MeshRenderer>().material.color = Color.green;
                    //                obj.GetComponent<BattleChar>().getTarget().floor.SetActive(true);
                    //            }
                    //            targetNum = sklSelected.targetNum;
                    //            tg = TARGETTAG.FRIEND;

                    //            break;

                    //        case TARGETTAG.FRIENDALL:
                    //            foreach (GameObject obj in myField)
                    //            {
                    //                obj.GetComponent<MeshRenderer>().material.color = Color.green;
                    //                obj.SetActive(true);
                    //            }
                    //            targetNum = myBattleChars.Count;
                    //            tg = TARGETTAG.FRIEND;

                    //            break;

                    //        case TARGETTAG.SELF:
                    //            player.floor.GetComponent<MeshRenderer>().material.color = Color.green;
                    //            player.getTarget().floor.SetActive(true);
                    //            targetNum = 1;
                    //            tg = TARGETTAG.FRIEND;

                    //            break;

                    //        case TARGETTAG.NONE:

                    //            break;

                    //    }

                    //    //타겟 선택하기
                    //    SelectTarget(target);

                        //타겟 선택이 끝나면
                        if (target.Count == sklSelected.targetNum)
                        {
                            curState++;//다음 단계로 이동
                            skWIndow.SetActive(false); // 스킬창 닫기
                            ClearTile(); //발판 색 초기화
                            break;
                        }
                    //}
                }

                break;
            case BattleState.SPELL:
                //선택된 스킬이 실제로 적용되는 단계
                //zoomPlayerCam();

                if (animStack <= 0)
                {
                    print(player.name +" use "+sklSelected.name);
                    sklSelected.useSkill(target);
                }
                else {
                    curState++;
                    //curState = BattleState.END;
                }
                break;

            case BattleState.ANIM: 
                //애니메이션 출력 되는 단계
                if (animStack <= 0) curState = BattleState.END;

                break;

            case BattleState.END: //전투의 끝, 배틀 매니저 초기화
                resetCam();
                player2origin();


                player.StateUpdate();
                player.EndBuff();
                player.curAP = 0;
                print(player.body.name +"'s turn is end.");
                readyPlayer.Remove(player.body);
                readyBC.Remove(player);

                sklSelected = nullSkl;
                target.Clear();
                skills.Clear();
                targetNum = 10;


                curState = BattleState.READY;

                break;
        }


    }

    //배틀 매니저에 등록되어 있는 캐릭터가 죽는 경우를 처리하기 위한 ㅎ마수
    public void Death(BattleChar bc) {
        if (bc.tg == TARGETTAG.ENEMY) {
            bc.tg = TARGETTAG.ENEMYDEAD;
            enemys.Remove(bc.transform.gameObject);
            enemyBC.Remove(bc);
            enemyDeadBC.Add(bc);
            enemyNum--;
        }
        else {
            bc.tg = TARGETTAG.FRIENDDEAD;
            myBattleChars.Remove(bc.transform.gameObject);
            myBC.Remove(bc);
            myDeadBC.Add(bc);
            myCharNum--;
        }


        readyPlayer.Remove(bc.transform.gameObject);
        readyBC.Remove(bc);
    }

    public void Revive(BattleChar bc) {
        bc.animator.enabled = true;
        bc.AP.active();
        bc.MP.active();
        bc.HP.active();
        if (bc.tg == TARGETTAG.ENEMYDEAD)
        {
            bc.tg = TARGETTAG.ENEMY;
            enemys.Add(bc.transform.gameObject);
            enemyBC.Add(bc);
            enemyDeadBC.Remove(bc);
            enemyNum++;
        }
        else
        {
            bc.tg = TARGETTAG.FRIEND;
            myBattleChars.Add(bc.transform.gameObject);
            myBC.Add(bc);
            myDeadBC.Remove(bc);
            myCharNum++;
        }

        bc.render.color = Color.white;
    }

    //타일 색을 초기화하는 함수
    void ClearTile() {
        Color tmp = Color.white;

        foreach (GameObject obj in enemyField)
        {
            obj.GetComponent<MeshRenderer>().material.color = tmp;
            obj.SetActive(false);
        }

        foreach (GameObject obj in myField)
        {
            obj.GetComponent<MeshRenderer>().material.color = tmp;
            obj.SetActive(false);
        }
    }

    //스킬을 사용할 타겟을 선택하는 함수
    void SelectTarget( List<BattleChar> list) {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            BattleChar tmp;

            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                print(hit.transform.tag);
                tmp = hit.transform.GetComponent<BattleChar>();

                switch (sklSelected.targetTag) {
                    case TARGETTAG.ENEMY:
                        if (tmp.tg == sklSelected.targetTag) list.Add(tmp);

                        break;
                    case TARGETTAG.ENEMYALL:
                        foreach (GameObject o in enemys) { list.Add(o.GetComponent<BattleChar>()); }

                        break;
                    case TARGETTAG.FRIEND:
                        if (tmp.tg == sklSelected.targetTag) list.Add(tmp);

                        break;
                    case TARGETTAG.FRIENDALL:
                        foreach (GameObject o in myBattleChars) { list.Add(o.GetComponent<BattleChar>()); }

                        break;
                    case TARGETTAG.NONE:


                        break;
                    case TARGETTAG.SELF:
                        if (tmp == player) list.Add(tmp);

                        break;
                }


                
                print(list.Count);
            }
        }

    }

    //void Destroy()
    //{
    //    buttons[0].GetComponent<Button>().onClick.RemoveListener(delegate () { sklSelected = normal; curState = BattleState.START; });
    //    buttons[1].GetComponent<Button>().onClick.RemoveListener(delegate () { sklSelected = skills[0]; curState = BattleState.START; });
    //    buttons[2].GetComponent<Button>().onClick.RemoveListener(delegate () { sklSelected = skills[1]; curState = BattleState.START; });
    //    buttons[3].GetComponent<Button>().onClick.RemoveListener(delegate () { sklSelected = skills[2]; curState = BattleState.START; });
    //    buttons[3].GetComponent<Button>().onClick.RemoveListener(delegate () { sklSelected = skills[3]; curState = BattleState.START; });
    //}

    //도발 상태인 타겟이 공격할 타겟보다 가까이에 위치한지 체크함 
    public List<BattleChar> getGuarded(BattleChar c) {
        guarded.Clear();
        foreach (GameObject o in myBattleChars) {
            BattleChar t = o.GetComponent<BattleChar>();

            if (t.yPos == c.yPos && t.xPos < c.xPos) guarded.Add(t);
            
        }


        return guarded;
    }

    //피격시 피격효과 적용
    public void StartHit(float time, BattleChar bc, Color color) {
        animStack++;
        curState = BattleState.ANIM;
        StartCoroutine(HitEffect(time, bc, color));
    }

    //피격효과
    IEnumerator HitEffect(float time, BattleChar bc, Color color)
    {
        if (bc != null) bc.render.color = color;

        yield return new WaitForSeconds(time);
        
        if(bc != null) bc.render.color = Color.white;
        animStack--;

    }

    //사망시 사망효과 적용
    public void StartDie(float time, BattleChar bc)
    {
        animStack++;
        curState = BattleState.ANIM;
        bc.animator.enabled = false;
        StartCoroutine(DieEffect(time, bc));
        Death(bc);
    }

    //사망효과
    IEnumerator DieEffect(float time, BattleChar bc)
    {
        Color color = Color.white;

        for (float f = 1.0f; f >= 0.1f; f -= Time.deltaTime/(time * 0.9f))
        {
            color.a = f;
            if (bc != null) bc.render.color = color;
            yield return null;

        }

        bc.AP.deActive();
        bc.MP.deActive();
        bc.HP.deActive();

        animStack--;

    }


}
