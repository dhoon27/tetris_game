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
│   ├── Board.cs            # 10×20 그리드 상태 관리, 줄 제거
│   ├── Piece.cs            # 현재 조작 중인 테트로미노 (이동, 회전)
│   ├── TetrominoData.cs    # 7종 블록 모양/색상 데이터 (ScriptableObject)
│   ├── GhostPiece.cs       # 착지 위치 미리보기 (반투명 회색 블록)
│   ├── GameManager.cs      # 게임 상태 (시작/게임오버), 이벤트 발행
│   ├── PauseManager.cs     # 일시정지 상태 + timeScale 관리
│   ├── ScoreManager.cs     # 점수/레벨/줄 수 계산, 최고 점수 저장
│   └── AudioManager.cs     # BGM/SFX 재생 (DontDestroyOnLoad)
├── UI/
│   ├── HUD.cs              # 점수, 레벨, 줄 수 실시간 업데이트
│   ├── NextPieceDisplay.cs # 다음 블록 미리보기 (Canvas UI Image 기반)
│   ├── PauseUI.cs          # 일시정지 패널 (재개/메인메뉴 버튼)
│   ├── GameOverUI.cs       # 게임 오버 패널 (재시도/메인메뉴 버튼)
│   └── MainMenuUI.cs       # 메인 메뉴 (PLAY 버튼)
├── Input/
│   └── TouchInputHandler.cs  # 드래그 연속 이동/탭 회전/하드 드롭, raycastTarget 토글
└── Utils/
    ├── SafeArea.cs         # 노치/라운드 코너 대응 RectTransform 조정
    └── CameraFit.cs        # 화면 비율 기반 orthoSize + 카메라 위치 자동 조정
```

## 게임 오브젝트 계층 구조 (Game 씬)

```
Game (씬)
├── Main Camera           ← Camera + CameraFit.cs
├── Global Light 2D
├── Board                 ← Board.cs
│   ├── Border            # SpriteRenderer, 위치 (-0.5, -0.5), 스케일 (10.4, 20.4)
│   └── Background        # SpriteRenderer, 위치 (-0.5, -0.5), 스케일 (10, 20)
├── GameManager           ← GameManager.cs
├── ScoreManager          ← ScoreManager.cs
├── PauseManager          ← PauseManager.cs
├── AudioManager          ← AudioManager.cs
├── EventSystem           ← InputSystemUIInputModule
└── Canvas (Screen Space - Overlay, 1080×1920 기준, matchWidthOrHeight 0.5)
    ├── SafeAreaPanel     ← SafeArea.cs, Image(α=0, raycastTarget), TouchInputHandler.cs
    │   ├── PauseButton   # 앵커 top-right 모서리 (-15, -15), 90×90
    │   ├── ScoreBox      # 배경 박스 (앵커 top-left, 240×110)
    │   │   └── ScoreText # fontSize 36, "SCORE\n{값}"
    │   ├── LevelBox      # 배경 박스 (앵커 top-center, 180×110)
    │   │   └── LevelText # fontSize 36, "LEVEL\n{값}"
    │   ├── NextBox       # 배경 박스 (앵커 top-right, 200×150)
    │   │   ├── NextLabelText  # fontSize 28, "NEXT"
    │   │   └── NextPieceContainer ← NextPieceDisplay.cs (UI Image 블록)
    │   └── BestBox       # 배경 박스 (앵커 top-left, ScoreBox 아래, 240×90)
    │       └── BestScoreText # fontSize 28, "BEST\n{값}"
    ├── PausePanel        # 반투명 오버레이 (scale 1, 비활성 기본)
    │   ├── PausedText
    │   ├── ResumeButton
    │   └── MainMenuButton
    └── GameOverPanel     # 반투명 오버레이 (scale 1, 비활성 기본)
        ├── GameOverText
        ├── FinalScoreText
        ├── NewBestText
        ├── RetryButton
        └── MainMenuButton
```

## 핵심 데이터 흐름

```
[Touch Input] → TouchInputHandler (스와이프/탭 감지)
                    ↓
              GameManager.cs (TryMovePiece / RotatePiece / HardDrop)
                    ↓
              Piece.cs (이동/회전 요청)
                    ↓
              Board.cs (IsValidPosition 충돌 검사 → 허용/거부)
                    ↓
              GhostPiece.cs (LateUpdate에서 착지 위치 자동 계산)
                    ↓
              Piece.cs (블록 착지 시) → Board.cs (PlacePiece + ClearLines)
                    ↓
              GameManager.OnPieceLocked → ScoreManager (점수/레벨 갱신)
                    ↓
              HUD.cs (UI 텍스트 갱신)
```

## 좌표계
- **보드 좌표**: (0,0)~(9,19) — Board.cs 내부 그리드 인덱스
- **월드 좌표**: BoardOffset = (-5, -10) 적용 → 셀 중심이 (-5,-10)~(4,9)
- **블록 셀 영역 중심**: (-0.5, -0.5) — Border/Background의 기준 위치
- **블록 스폰 위치**: 보드 좌표 (4, 18) → 월드 (-1, 8)

## 카메라 & 레이아웃 설계

### CameraFit 마진 시스템
CameraFit.cs는 화면 비율에 관계없이 보드 주변에 UI 마진을 확보:
- **상단 22%**: BEST / SCORE / LEVEL / NEXT 박스 + PauseButton 영역
- **하단 2%**: 최소 여백
- **좌측 2%**: 최소 여백
- **우측 2%**: 최소 여백

UI가 모두 상단에 배치되므로 좌우/하단은 대칭 마진으로 보드를 중앙 정렬.

orthoSize와 카메라 위치를 Start()에서 자동 계산하므로,
씬 에디터의 카메라 값은 런타임에 덮어써짐.

### 오버레이 터치 처리
Pause/GameOver 패널이 뜨면:
1. `TouchInputHandler.SetInputActive(false)` 호출
2. SafeAreaPanel Image의 `raycastTarget = false` → 터치 가로채기 중지
3. 오버레이 버튼(Resume, Retry 등)이 정상 터치 수신

## 주요 설계 결정
| 결정 | 이유 |
|------|------|
| SpriteRenderer 배열 (Tilemap X) | 초보자가 이해하기 쉬운 구조 |
| ScriptableObject 블록 데이터 | 데이터와 로직 분리, 에디터에서 수정 편리 |
| Input System + IPointerHandler | Android 터치 안정성 (EnhancedTouch보다 신뢰성↑) |
| NextPieceDisplay Canvas UI 방식 | 월드 스페이스는 좁은 폰에서 화면 밖으로 나감 |
| CameraFit 마진 기반 배치 | 다양한 화면 비율(9:16~9:21)에서 UI 겹침 방지 |
| Border/Background 위치 (-0.5, -0.5) | 셀 중심 기준 배치 — 블록이 경계를 넘지 않음 |
