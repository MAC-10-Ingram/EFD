using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public void TestButton() {
        //To do
        PlayerPrefsX.SetBool(PrefsEntity.BattleEndFlag, true);
        LoadingManager.LoadSceneBattleToField();
        print("Test is done");
    }
}
