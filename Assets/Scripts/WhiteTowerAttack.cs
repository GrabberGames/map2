﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WhiteTowerAttack : Stat, InterfaceRange, ITarget
{

    public ParticleSystem fx_whiteTower;

    // Stat _stat;

    private float AttackPerSeconds = 4f;

    private Vector3 bulletSpawnPosition;
    private GameObject NoBuildRange;

    private GameController gameController;
    private List<FreaksController> blackFreaks = new List<FreaksController>();
    bool isAttack = false;


    public AudioSource SFXWhiteTowerDestroy;
    public AudioSource SFXWhiteTowerAttack;
    public AudioSource SFXWhiteTowerComplete;

    [SerializeField] private GameObject circle;
    public void OpenCircle()
    {
        circle.SetActive(true);
    }

    public void CloseCircle()
    {
        circle.SetActive(false);
    }

    protected override void Init()
    {
        base.Init();
  
        blackFreaks = gameController.GetAliveBlackFreaksList();
        // Debug.Log("base.HP : " + base.HP);
        bulletSpawnPosition = new Vector3(transform.position.x, transform.position.y + 18.98f, transform.position.z - 0.29f);
        NoBuildRange = transform.GetChild(2).gameObject;
    


    }
    public override void DeadSignal()
    {
        if (HP <= 0)
            StartCoroutine(Dissolve());

     
    }

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        Init();
        SetHpBar();
    }

    //�Ҹ� Ȯ���ϱ����� update��

    void Update()
    {        
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this.gameObject)
                {
                    OpenCircle();
                }
                else
                {
                    CloseCircle();
                }
            }
        }
        
   
        for (int i = 0; i < blackFreaks.Count; i++)
        {


            if ((blackFreaks[i].gameObject.transform.position - transform.position).sqrMagnitude < 2025f)
            {
                if (isAttack)
                    return;
                else
                {
                    StartCoroutine(FindInAttackRange(blackFreaks[i].gameObject));
                    isAttack = true;
                }
            }

        }



    }

    GameObject bullet;
    IEnumerator FindInAttackRange(GameObject blackFreaks)
    {

        bullet = BulletPooling.instance.GetObject("WhiteTowerBullet");
        bullet.GetComponent<WhiteTowerBullet>().InitSetting(PD, blackFreaks, bulletSpawnPosition);
        bullet.SetActive(true);
        fx_whiteTower.Play(true);
        SFXWhiteTowerAttack.Play();
        yield return new WaitForSeconds(fx_whiteTower.main.startDelayMultiplier);

        fx_whiteTower.Play(false);

        yield return new WaitForSeconds(AttackPerSeconds - fx_whiteTower.main.startDelayMultiplier);
        isAttack = false;
    }



    IEnumerator Dissolve()
    {


        MeshRenderer Sr1 = transform.gameObject.GetComponent<MeshRenderer>();
        MeshRenderer Sr2 = transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        float threshold1;
        float threshold2;

        threshold1 = Sr1.material.GetFloat("_Dissolve");
        threshold2 = Sr2.material.GetFloat("_Dissolve");

        SFXWhiteTowerDestroy.Play();
        for (int i = 60; i <= 100; i++)
        {
            threshold1 = i / 100f;
            Sr1.material.SetFloat("_Dissolve", threshold1);
            Sr2.material.SetFloat("_Dissolve", threshold1);



            yield return YieldInstructionCache.WaitForSeconds(0.03f);
        }

        BuildingPooling.instance.ReturnObject(this.gameObject);
        BarPooling.instance.ReturnObject(hpBar);
       // GetComponentInParent<Building>().ReturnBuildingPool();

    }




    public void BuildingRangeON(bool check)
    {
        if (NoBuildRange != null)
        {
            if (check)
                NoBuildRange.SetActive(true);
            else
                NoBuildRange.SetActive(false);
        }
    }


    public GameObject hpBar;
    public Vector3 hpBarOffset = new Vector3(0, 5, 0);
    private RectTransform rect;
    private Image hpBarImage;
    private Text hpBarText;
    void SetHpBar()
    {
        hpBar = BarPooling.instance.GetObject(BarPooling.bar_name.ally_bar);
        rect = (RectTransform)hpBar.transform;
        rect.sizeDelta = new Vector2(100, 21);
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];
        hpBarImage.rectTransform.sizeDelta = rect.sizeDelta;
        hpBarText = hpBar.GetComponentsInChildren<Text>()[0];
        hpBarText.text = HP.ToString();
        var _hpbar = hpBar.GetComponent<HpBar>();
        _hpbar.target = this.gameObject;
        _hpbar.offset = hpBarOffset;
        _hpbar.what = HpBar.targets.WhiteTower;
    }

    public override void OnAttackSignal()
    {
        hpBarImage.fillAmount = HP / MAX_HP;
        if (HP >= 0)
            hpBarText.text = HP.ToString();
    }


}