using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChaChange : MonoBehaviour
{
    [SerializeField] List<Transform> cha_0_part = new List<Transform>();
    [SerializeField] List<Transform> cha_1_part = new List<Transform>();

    List<List<Transform>> chaPartList = new List<List<Transform>>();
    

    public PlayerChaStatus currentChaStatus;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        chaPartList = new List<List<Transform>>() 
        {
            cha_0_part,
            cha_1_part,
        };

        PlayerChaChangeOn(PlayerChaStatus.PlayerCha_0);
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    public void PlayerChaChangeOn(int status) 
    {
        PlayerChaChangeOn((PlayerChaStatus)status);
    }

    public void PlayerChaChangeOn(PlayerChaStatus playerChaStatus)
    {
        currentChaStatus = playerChaStatus;

        for (int i = 0; i < chaPartList.Count; i++)
        {
            List<Transform> cha_partList = chaPartList[i];

            for (int z = 0; z < cha_partList.Count; z++)
            {
                cha_partList[z].gameObject.SetActive(i == (int)currentChaStatus);
            }
            
        }
    }



}

public enum PlayerChaStatus
{
    PlayerCha_0 = 0,
    PlayerCha_1,
}
