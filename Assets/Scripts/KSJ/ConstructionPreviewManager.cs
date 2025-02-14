using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eLayerMask
{
    Building = 1 << 9,
    Ground = 1 << 13,
    Essence = 1 << 14,
    Friendly = 1 <<16,

    ConstructionChk = Building | Essence | Friendly
}

public enum eConditionConstructionPreview
{
    Buildable, NearbyBuilding, NonBuildable, NearbyNoEssence
}

public enum eBuilding
{
    Alter, WhiteTower, Workshop, MaxCount
}

public class ConstructionPreviewManager : MonoBehaviour
{
    [Header ("Preview Object Setting")]
    [SerializeField] private BasePreviewObject _alter;
    [SerializeField] private BasePreviewObject _whiteTower;
    [SerializeField] private BasePreviewObject _workshop;

    [Header("Mouse Snap Setting")]
    [SerializeField] private float snapDistanceUnit;

    [Header("PreviewObject Color Setting")]
    [SerializeField] private Color _buildableColor;
    [SerializeField] private Color _nonBuildableColor;

    public BasePreviewObject alter { get => _alter; }
    public BasePreviewObject whiteTower { get => _whiteTower; }
    public BasePreviewObject workshop { get => _workshop; }




    private BasePreviewObject[] previewObjectArray = new BasePreviewObject[(int)eBuilding.MaxCount];
    private eBuilding _nowPreviewBuilding;

    private bool _isPreviewMode;


    public Color buildableColor { get => _buildableColor; }
    public Color nonBuildableColor { get => _nonBuildableColor; }
    public bool isPreviewMode { get => _isPreviewMode; }
    public eBuilding nowPreviewBuilding { get => _nowPreviewBuilding; }

    private Vector3 snapPosition;

    RaycastHit hit;
    Ray ray;

    IEnumerator previewPositioning;

    //singleton
    private static ConstructionPreviewManager mInstance;
    public static ConstructionPreviewManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = FindObjectOfType<ConstructionPreviewManager>();
            }
            return mInstance;
        }
    }

    private void Awake()
    {
        previewObjectArray[(int)eBuilding.Alter] = alter;
        previewObjectArray[(int)eBuilding.WhiteTower] = whiteTower;
        previewObjectArray[(int)eBuilding.Workshop] = workshop;

        _isPreviewMode = false;
    }


    #region 버튼 조작용 함수 제작
    public void OnAlterConstructionPreview()
    {
        ConstructionPreview(eBuilding.Alter, 600);
    }

    public void OnWhiteTowerConstructionPreview()
    {
        ConstructionPreview(eBuilding.WhiteTower, 500);
    }

    public void OnWorkshopConstructionPreview()
    {
        ConstructionPreview(eBuilding.Workshop, 0);
    }

    private void ConstructionPreview(eBuilding buildingType, int price)
    {
        if (StageManager.Instance.ChkEssence(price))
        {
            if(!WhiteFreaksManager.Instance.idleFreaksCount.Equals(0))
                OnConstructionPreview(buildingType);
            else
                SystemMassage.Instance.PrintSystemMassage("명령을 수행할 화이트프릭스가 없습니다.");
        }
        else
            SystemMassage.Instance.PrintSystemMassage("필요한 정수가 부족합니다.");
    }

    public void OnConstructionPreview(eBuilding buildingType)
    {
        if (_nowPreviewBuilding.Equals(buildingType))
        {
            if (!isPreviewMode)
            {
                ConstructionPreview(true);
            }
        }
        else
        {
            if (isPreviewMode)
            {
                ConstructionPreview(false);
            }

            _nowPreviewBuilding = buildingType;
            ConstructionPreview(true);
        }
    }
    #endregion


    public void ConstructionPreview(bool value)
    {
        if(!isPreviewMode.Equals(value))
        {
            _isPreviewMode = value;
            BuildingManager.Instance.BuildingRangeON(value);
            previewObjectArray[(int)_nowPreviewBuilding].SetActive(value);
        }
    }

    public Vector3 PreviewPosition()
    {
        return previewObjectArray[(int)_nowPreviewBuilding].PreviewPosition();
    }

    public bool ChkConstructionArea()
    {
        return previewObjectArray[(int)_nowPreviewBuilding].canBuild;
    }

    public void PrintMessage()
    {
        PrintMessage(previewObjectArray[(int)_nowPreviewBuilding].conditionConstructionPreview);
    }

    private void PrintMessage(eConditionConstructionPreview condition)
    {

        //추후 알림메세지 입력예정
        switch (condition)
        {
            case eConditionConstructionPreview.Buildable:
                Debug.Log("건설 가능한데 왜 여기로 오지?");
                break;

            case eConditionConstructionPreview.NearbyBuilding:
                SystemMassage.Instance.PrintSystemMassage("해당 위치에는 건설이 불가합니다.");
                Debug.Log("다른 건물 인근에 건설할 수 없습니다.");
                break;

            case eConditionConstructionPreview.NonBuildable:
                SystemMassage.Instance.PrintSystemMassage("해당 위치에는 건설이 불가합니다.");
                Debug.Log("건설 가능한 위치가 아닙니다.");
                break;

            case eConditionConstructionPreview.NearbyNoEssence:
                SystemMassage.Instance.PrintSystemMassage("해당 위치에는 건설이 불가합니다.");
                Debug.Log("인근에 자원이 없습니다.");
                break;

            default:
                break;
        }
    }

    #region 마우스 위치 스냅 구현
    public Vector3 SnapPosition(Vector3 buildingPosition, Vector3 position)
    {
        if (buildingPosition.x > position.x)
        {
            if(position.x < 0.0f)
                snapPosition.x = ((int)((position.x + (snapDistanceUnit * 0.5f)) / snapDistanceUnit) - 0.5f) * snapDistanceUnit;
            else
                snapPosition.x = ((int)((position.x + (snapDistanceUnit * 0.5f)) / snapDistanceUnit) + 0.5f) * snapDistanceUnit;
        }
        else if (buildingPosition.x < position.x)
        {
            if (position.x < 0.0f)
                snapPosition.x = ((int)((position.x - (snapDistanceUnit * 0.5f)) / snapDistanceUnit) - 0.5f) * snapDistanceUnit;
            else
                snapPosition.x = ((int)((position.x - (snapDistanceUnit * 0.5f)) / snapDistanceUnit) + 0.5f) * snapDistanceUnit;
        }
        else
            snapPosition.x = buildingPosition.x;

        if (buildingPosition.z > position.z)
        {
            if (position.z < 0.0f)
                snapPosition.z = ((int)((position.z + (snapDistanceUnit * 0.5f)) / snapDistanceUnit) - 0.5f) * snapDistanceUnit;
            else
                snapPosition.z = ((int)((position.z + (snapDistanceUnit * 0.5f)) / snapDistanceUnit) + 0.5f) * snapDistanceUnit;
        }
        else if (buildingPosition.z < position.z)
        {
            if (position.z < 0.0f)
                snapPosition.z = ((int)((position.z - (snapDistanceUnit * 0.5f)) / snapDistanceUnit) - 0.5f) * snapDistanceUnit;
            else
                snapPosition.z = ((int)((position.z - (snapDistanceUnit * 0.5f)) / snapDistanceUnit) + 0.5f) * snapDistanceUnit;
        }
        else
            snapPosition.z = buildingPosition.z;

        snapPosition.y = position.y;

        return snapPosition;
    }
    #endregion
}
