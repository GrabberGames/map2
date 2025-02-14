﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    [Header("Alter")]
    [SerializeField] private AlterController altercontroller;
    [SerializeField] private GameObject alter;
    [SerializeField] private AudioSource SFXBuildingSelect;


    private GameObject essenceSpot;

    public GameObject Alter
    {
        get
        {
            if (alter == null)
            {
                alter = GameObject.Find("alter");
                // _alter = GameObject.FindGameObjectWithTag("alter");
            }
            return alter;
        }
        set
        {
            alter = value;
            AlterIsChange.Invoke(alter);

            return;


        }
    }
    private void Awake()
    {
        SFXBuildingSelect = Instantiate(SFXBuildingSelect);
    }
    
    

    public Action<GameObject> AlterIsChange = null;




    private List<BuildingRange> buildingRangeList = new List<BuildingRange>();




    private static BuildingManager mInstance;
    public static BuildingManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<BuildingManager>();
            }
            return mInstance;
        }
    }
    // test�뵵
    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SetBuildPosition("Alter", new Vector3(0,0,0));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            SetBuildPosition("Whitetower", new Vector3(0, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SetBuildPosition("Workshop", new Vector3(0, 0, 0));
        }
       
        if (Input.GetKeyDown(KeyCode.B))
        {
           BuildingRangeON(true);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            BuildingRangeON(false);
        }


    } */



    public void SetBuildPosition(eBuilding buildingType, Vector3 pos, GameObject essence)
    {
        SetEssenceSpot(essence);
        SetBuildPosition(buildingType, pos);
    }
    
    GameObject go;
    public void SetBuildPosition(eBuilding buildingType, Vector3 pos)
    {
        // SFXBuildingSelect.SetScheduledStartTime(0.06);
        SFXBuildingSelect.time = 0.5f;
        SFXBuildingSelect.Play();

        switch (buildingType)
        {
            case eBuilding.Alter:
                go = BuildingPooling.instance.GetObject("building");
                go.GetComponent<Building>().build_num = 0;
                go.transform.position = pos;
                go.SetActive(true);
                go.GetComponent<Building>().Init();

                break;
            case eBuilding.WhiteTower:
                go = BuildingPooling.instance.GetObject("building");
                go.transform.position = pos;
                go.GetComponent<Building>().build_num = 1;
                //whiteTowerList.Add(go.transform.GetChild(1).gameObject.GetComponent<WhiteTowerAttack>());
                //buildingRangeList.Add(go.transform.GetChild(3).gameObject.GetComponent<BuildingRange>());
               // buildingRangeList.Add(go.GetComponent<Building>().NoBuild_range.gameObject.GetComponent<BuildingRange>());
                go.SetActive(true);

                go.GetComponent<Building>().Init();

                break;
            case eBuilding.Workshop:
                
                go = BuildingPooling.instance.GetObject("building");
                go.GetComponent<Building>().build_num = 2;
                go.transform.position = pos;
                go.SetActive(true);
                go.GetComponent<Building>().Init();


                break;
            default:
                break;
        }
    }


    InterfaceRange interfaceRange;
    public void BuildingRangeON(bool check) //건물들 rangeON
    {
        interfaceRange = altercontroller.GetComponent<InterfaceRange>();
        interfaceRange.BuildingRangeON(check);

        for (int i = 0; i < buildingRangeList.Count; i++)
        {
            if (buildingRangeList[i] != null)
            {
                interfaceRange = buildingRangeList[i].GetComponent<InterfaceRange>();
                interfaceRange.BuildingRangeON(check);


            }
            else
            {
                buildingRangeList.RemoveAt(i); //buildingRange 소멸되었을 경우
            }
        }

    }

    public Vector3 GetAlterPosition() //Alter 위치 전달
    {
        return alter.gameObject.transform.position;
    }

    public float GetAlterBuildRadius() //알터 건설가능범위 radius 전달
    {
        float radius = (alter.gameObject.transform.GetChild(2).gameObject.transform.localScale.x) / 2;
        return radius;
    }

    public float GetAlterNoBuildRadius() //알터 건설 불가능범위 radius 전달
    {
        float radius = ((alter.gameObject.transform.GetChild(2).gameObject.transform.localScale.x) * (float)0.3) / 2;
        return radius;
    }


    private void SetEssenceSpot(GameObject go) //자원지 오브젝트 받아옴
    {
        essenceSpot = go;
    }

    public GameObject GetEssenceSpot() //자원지 오브젝트 설정
    {
        return essenceSpot;
    }


    public List<BuildingRange> GetbuildingRangeList()
    {
        return buildingRangeList;
    }

}