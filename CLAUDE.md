# Tetris Game — Claude Code 컨텍스트

## 프로젝트 개요
- **게임**: 클래식 테트리스 (2D 모바일)
- **플랫폼**: Android (Google Play Store 출시 목표)
- **엔진**: Unity 6000.3.10f1 (URP)
- **수익화**: 무료 + 광고 (개발 후반부에 적용)
- **타겟**: 어린이 포함 캐주얼 게이머

## 개발 원칙
- 처음 Unity를 배우는 사용자이므로 **단계별로 설명하며 진행**
- 각 기능 구현 시 왜 이렇게 하는지 이유를 함께 설명
- 과도한 추상화 금지 — 지금 필요한 것만 구현
- 변경사항은 항상 `docs/PROGRESS.md`에 반영

## 폴더 구조 (Assets/)
```
Assets/
├── Scenes/          # Unity 씬 파일
├── Scripts/         # C# 스크립트
│   ├── Core/        # 게임 핵심 로직 (Board, Piece, GameManager)
│   ├── UI/          # UI 관련 스크립트
│   └── Utils/       # 유틸리티
├── Prefabs/         # 재사용 가능한 GameObject 프리팹
├── Sprites/         # 2D 이미지 리소스
├── Audio/           # 효과음, 배경음악
└── Settings/        # URP 렌더 설정 (기존)
```

## 주요 문서
- `docs/GDD.md` — 게임 디자인 문서
- `docs/PROGRESS.md` — 개발 진행 현황 (체크리스트)
- `docs/ARCHITECTURE.md` — 씬·스크립트 설계

## 세션 시작 시 체크리스트
1. `docs/PROGRESS.md` 확인 → 현재 단계 파악
2. Unity MCP 연결 확인 (`mcpforunity://instances`)
3. 이전 단계 완료 여부 확인 후 다음 작업 진행

## 기술 스택
| 항목 | 선택 | 이유 |
|------|------|------|
| 렌더 파이프라인 | URP | 모바일 최적화, 프로젝트 기본값 |
| 입력 시스템 | Unity Input System | 터치/키보드 통합 처리 |
| UI | Unity UI (uGUI) | 2D 모바일 UI 표준 |
| 광고 (예정) | Google AdMob | 구글 플레이 연동 |
