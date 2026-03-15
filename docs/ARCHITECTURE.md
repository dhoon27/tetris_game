# 기술 아키텍처

## 씬 구성
| 씬 이름 | 역할 |
|---------|------|
| `MainMenu` | 타이틀 화면, 시작 버튼 |
| `Game` | 메인 게임플레이 |

## 스크립트 구조

```
Scripts/
├── Core/
│   ├── Board.cs          # 10×20 그리드 상태 관리, 줄 제거
│   ├── Piece.cs          # 현재 조작 중인 테트로미노 (이동, 회전)
│   ├── TetrominoData.cs  # 7종 블록 모양/색상 데이터 (ScriptableObject)
│   └── GameManager.cs    # 게임 상태 (시작/일시정지/게임오버), 점수
├── UI/
│   ├── HUD.cs            # 점수, 레벨, 줄 수 실시간 업데이트
│   ├── NextPieceDisplay.cs # 다음 블록 미리보기
│   └── GameOverUI.cs     # 게임 오버 패널
└── Input/
    └── TouchInputHandler.cs  # 스와이프/탭 감지 → 이동/회전/드롭 변환
```

## 게임 오브젝트 계층 구조 (Game 씬)

```
Game (씬)
├── Main Camera
├── GameManager          ← GameManager.cs
├── Board                ← Board.cs
│   └── Grid (10×20 타일맵 또는 SpriteRenderer 배열)
├── PieceSpawner         ← Piece.cs 동적 생성
└── UI (Canvas)
    ├── HUD              ← HUD.cs
    ├── NextPiecePanel   ← NextPieceDisplay.cs
    ├── PausePanel
    └── GameOverPanel    ← GameOverUI.cs
```

## 핵심 데이터 흐름

```
[Input] → Piece.cs (이동/회전 요청)
              ↓
         Board.cs (충돌 검사 → 허용/거부)
              ↓
         Piece.cs (블록 착지 시) → Board.cs (고정 + 줄 제거)
              ↓
         GameManager.cs (점수 계산, 레벨업, 게임오버 판정)
              ↓
         HUD.cs (UI 갱신)
```

## 좌표계
- Unity 2D 기준: 좌하단 (0,0), 우상단 (9,19)
- 블록 스폰 위치: x=4, y=20 (보이지 않는 버퍼 영역)

## 주요 설계 결정
| 결정 | 이유 |
|------|------|
| Tilemap 대신 SpriteRenderer 배열 사용 | 초보자가 이해하기 쉬운 구조 |
| ScriptableObject로 블록 데이터 관리 | 데이터와 로직 분리, 에디터에서 수정 편리 |
| Input System 패키지 사용 | 키보드 + 터치 통합 처리 |
