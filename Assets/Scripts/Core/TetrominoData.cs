using UnityEngine;

// ScriptableObject: Unity 에디터에서 데이터를 에셋 파일로 저장할 수 있는 클래스
// 게임 오브젝트 없이 데이터만 독립적으로 관리할 수 있어 편리함
[CreateAssetMenu(fileName = "TetrominoData", menuName = "Tetris/Tetromino Data")]
public class TetrominoData : ScriptableObject
{
    // 이 블록에 사용할 스프라이트 이미지
    public Sprite sprite;

    // 블록 4칸의 상대 좌표 배열
    // Vector2Int(열, 행) 형태로, 피벗(중심)으로부터의 오프셋
    public Vector2Int[] cells;
}

// 7종 테트로미노 타입 열거형
// 이 값으로 어떤 블록인지 구분함
public enum TetrominoType
{
    I, O, T, S, Z, J, L
}
