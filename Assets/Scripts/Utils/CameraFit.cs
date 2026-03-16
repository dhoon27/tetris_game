using UnityEngine;

/// <summary>
/// 보드 주위에 UI용 마진을 확보하면서 카메라 orthoSize와 위치를 자동 조정.
/// 화면 비율이 달라도 SCORE/NEXT 등 UI가 보드 밖에 표시되도록 보장.
/// </summary>
public class CameraFit : MonoBehaviour
{
    [Header("보드 경계 (월드 좌표)")]
    [SerializeField] private float boardLeft = -5.7f;
    [SerializeField] private float boardRight = 4.7f;
    [SerializeField] private float boardBottom = -10.7f;
    [SerializeField] private float boardTop = 9.7f;

    [Header("UI 마진 (화면 비율)")]
    [Tooltip("상단: BEST/SCORE/LEVEL/NEXT/PAUSE 영역")]
    [SerializeField] private float topMargin = 0.22f;

    [Tooltip("하단: 최소 여백")]
    [SerializeField] private float bottomMargin = 0.02f;

    [Tooltip("우측: 최소 여백")]
    [SerializeField] private float rightMargin = 0.02f;

    [Tooltip("좌측: 최소 여백")]
    [SerializeField] private float leftMargin = 0.02f;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Start()
    {
        Fit();
    }

    private void Fit()
    {
        float aspect = (float)Screen.width / Screen.height;

        float boardWidth = boardRight - boardLeft;
        float boardHeight = boardTop - boardBottom;

        // 보드가 차지할 수 있는 화면 비율 (나머지는 UI 마진)
        float availW = 1f - leftMargin - rightMargin;
        float availH = 1f - topMargin - bottomMargin;

        // 가로/세로 각각에서 필요한 orthoSize 계산
        float orthoForWidth = (boardWidth / availW) / (2f * aspect);
        float orthoForHeight = (boardHeight / availH) / 2f;

        float ortho = Mathf.Max(orthoForWidth, orthoForHeight);
        _cam.orthographicSize = ortho;

        // 카메라 위치: 마진 비율에 따라 보드를 화면 내에 배치
        float visW = 2f * ortho * aspect;
        float visH = 2f * ortho;

        // 좌측 마진을 비율로 분배 → 카메라 X 위치 결정
        float leftWorld = leftMargin / (leftMargin + rightMargin) * (visW - boardWidth);
        float camX = boardLeft - leftWorld + visW / 2f;

        // 하단 마진을 비율로 분배 → 카메라 Y 위치 결정
        float bottomWorld = bottomMargin / (bottomMargin + topMargin) * (visH - boardHeight);
        float camY = boardBottom - bottomWorld + visH / 2f;

        transform.position = new Vector3(camX, camY, transform.position.z);
    }
}
