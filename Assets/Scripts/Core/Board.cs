using UnityEngine;

public class Board : MonoBehaviour
{
    public static readonly int Width = 10;
    public static readonly int Height = 20;

    // 각 칸에 놓인 블록의 Transform을 저장하는 2D 배열
    // null = 빈 칸, 값이 있으면 블록이 차있는 칸
    private Transform[,] _grid = new Transform[Width, Height];

    // 보드의 왼쪽 하단이 (-5, -10)이 되도록 오프셋 설정
    public static readonly Vector2Int BoardOffset = new Vector2Int(-5, -10);

    // 특정 위치가 보드 안에 있고 비어있는지 확인
    public bool IsValidPosition(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= Width) return false;
        if (pos.y < 0) return false;
        if (pos.y >= Height) return true; // 스폰 영역(보드 위)은 허용
        return _grid[pos.x, pos.y] == null;
    }

    // 월드 좌표 → 보드 좌표 변환
    public Vector2Int WorldToBoard(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x) - BoardOffset.x;
        int y = Mathf.RoundToInt(worldPos.y) - BoardOffset.y;
        return new Vector2Int(x, y);
    }

    // 보드 좌표 → 월드 좌표 변환
    public Vector3 BoardToWorld(Vector2Int boardPos)
    {
        return new Vector3(boardPos.x + BoardOffset.x, boardPos.y + BoardOffset.y, 0);
    }

    // 블록 조각을 보드에 고정시킴
    public void PlacePiece(Transform piece)
    {
        foreach (Transform block in piece)
        {
            Vector2Int pos = WorldToBoard(block.position);
            if (pos.y < Height)
                _grid[pos.x, pos.y] = block;
        }
    }

    // 꽉 찬 줄을 모두 제거하고 제거된 줄 수 반환
    public int ClearLines()
    {
        int linesCleared = 0;
        for (int y = 0; y < Height; y++)
        {
            if (IsLineFull(y))
            {
                DeleteLine(y);
                DropLinesAbove(y);
                y--;
                linesCleared++;
            }
        }
        return linesCleared;
    }

    // 게임 오버 판정: 최상단 줄에 블록이 있으면 게임 오버
    public bool IsGameOver()
    {
        for (int x = 0; x < Width; x++)
        {
            if (_grid[x, Height - 1] != null)
                return true;
        }
        return false;
    }

    // 보드를 완전히 초기화 (게임 재시작 시 사용)
    public void ClearAll()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                DestroyCell(x, y);
            }
        }
    }

    // --- 내부 메서드 ---

    private bool IsLineFull(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            if (_grid[x, y] == null) return false;
        }
        return true;
    }

    private void DeleteLine(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            DestroyCell(x, y);
        }
    }

    private void DestroyCell(int x, int y)
    {
        if (_grid[x, y] != null)
        {
            Destroy(_grid[x, y].gameObject);
            _grid[x, y] = null;
        }
    }

    private void DropLinesAbove(int deletedY)
    {
        for (int y = deletedY + 1; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_grid[x, y] != null)
                {
                    _grid[x, y].position += Vector3.down;
                    _grid[x, y - 1] = _grid[x, y];
                    _grid[x, y] = null;
                }
            }
        }
    }
}
