# 개발 진행 현황

> 마지막 업데이트: 2026-03-17

## 현재 단계: Phase 4 진행 중 (4-2 완료, 고스트 피스 추가)

---

## Phase 1 — 핵심 게임플레이
> 목표: 터치로 플레이 가능한 테트리스 완성 (모바일 기준)

- [x] 1-1. 씬 & 카메라 세팅
- [x] 1-2. 게임 보드 (10×20 그리드) 구현
- [x] 1-3. 테트로미노 7종 정의 (모양 + 색상)
- [x] 1-4. 블록 생성 & 낙하 (자동 드롭)
- [x] 1-5. 블록 이동 (좌/우)
- [x] 1-6. 블록 회전 (SRS 또는 심플 회전)
- [x] 1-7. 소프트 드롭 / 하드 드롭
- [x] 1-8. 블록 착지 & 보드에 고정
- [x] 1-9. 줄 제거 로직
- [x] 1-10. 게임 오버 판정
- [x] 1-11. 터치 입력 (스와이프 이동, 탭 회전, 하드 드롭)

## Phase 1.5 — 게임플레이 보조 기능
- [x] 고스트 피스: 블록이 떨어질 위치를 반투명 회색 블록으로 미리 표시
- [x] 드래그 연속 이동: 터치 드래그로 블록이 부드럽게 좌우/아래 이동

## Phase 2 — UI
> 목표: 메뉴, 점수, 게임오버 화면 완성

- [x] 2-1. 점수 / 레벨 / 줄 수 표시
- [x] 2-2. 다음 블록 미리보기
- [x] 2-3. 메인 메뉴 씬
- [x] 2-4. 일시정지 기능
- [x] 2-5. 게임 오버 화면
- [x] 2-6. 최고 점수 저장 (PlayerPrefs)

## Phase 3 — 오디오
- [x] 3-1. BGM 루프 (메인메뉴 전용, bgm_menu.mp3)
- [ ] 3-2. 효과음 (이동, 착지, 줄 제거, 게임 오버)

## Phase 4 — 모바일 최적화
- [x] 4-1. 화면 비율 대응 (Safe Area)
- [x] 4-2. Android 빌드 세팅
- [ ] 4-3. 성능 프로파일링

## Phase 5 — 출시 준비
- [ ] 5-1. Google AdMob 연동
- [ ] 5-2. Google Play 스토어 등록
- [ ] 5-3. 아이콘 & 스플래시 스크린
- [ ] 5-4. 최종 빌드 & 서명

---

## 완료된 작업 로그
| 날짜 | 작업 내용 |
|------|-----------|
| 2026-03-15 | 프로젝트 문서 초기화 (CLAUDE.md, GDD.md, PROGRESS.md, ARCHITECTURE.md) |
| 2026-03-15 | Phase 2-1: ScoreManager, HUD, Canvas UI 구현 (점수/레벨/줄 수 표시) |
| 2026-03-15 | Phase 2-2: NextPieceDisplay 구현, GameManager 다음 블록 예약 로직 추가 |
| 2026-03-15 | Phase 2-3: MainMenu 씬 생성, PLAY 버튼, Build Settings 등록 |
| 2026-03-15 | Phase 2-4: PauseManager, PauseUI 구현 (일시정지/재개/메인메뉴 버튼) |
| 2026-03-15 | Phase 2-5: GameOverUI 구현 (GAME OVER 패널, 최종 점수, RETRY/MAIN MENU) |
| 2026-03-15 | Phase 2-6: 최고 점수 PlayerPrefs 저장, HUD BEST 표시, 게임오버 시 NEW BEST! |
| 2026-03-15 | Phase 3-1: AudioManager 구현, 메인메뉴 BGM 루프 재생 (bgm_menu.mp3) |
| 2026-03-15 | Phase 4-1: SafeArea 스크립트 구현, Game/MainMenu 씬 SafeAreaPanel 적용 |
| 2026-03-15 | Phase 4-2: AndroidBuildSetup Editor 스크립트 (APK/Mono/ARMv7+ARM64, minAPI 25) |
| 2026-03-15 | Bugfix: AndroidBuildSetup IL2CPP+ARM64로 변경, 패키지명 com.dh.tetrisgame |
| 2026-03-15 | Bugfix: TouchInputHandler EnhancedTouch→Touchscreen.current (Android 터치 미작동 수정) |
| 2026-03-15 | BuildScript.cs 추가 (Tools > Build Android APK 메뉴) |
| 2026-03-15 | UI 레이아웃 수정: LevelText/PauseButton 겹침 해결, SafeAreaPanel scale 1로 복구, CanvasScaler MatchWidthOrHeight 0.5 적용 |
| 2026-03-16 | Bugfix: Pause/GameOver 터치 미작동 — SafeAreaPanel raycastTarget 토글 + Time.unscaledTime 적용 |
| 2026-03-16 | Bugfix: 블록 경계 이탈 — Border/Background 위치를 블록 셀 중심 (-0.5, -0.5)으로 재정렬 |
| 2026-03-16 | Bugfix: UI가 보드 안에 겹침 — CameraFit을 마진 기반으로 재설계 (상12%/하5%/우15%/좌2%) |
| 2026-03-16 | NextPieceDisplay를 월드 스페이스→Canvas UI(Image)로 전환, 우측 마진 영역에 배치 |
| 2026-03-16 | PausePanel/GameOverPanel scale 0.75→1.0 변경 (전체 화면 덮어 터치 차단 강화) |
| 2026-03-16 | UI 리디자인: LINES 삭제, NEXT를 상단으로 이동, BEST 별도 박스, 텍스트 축소(36/28), 배경 박스 4개 추가 |
| 2026-03-17 | feat: 고스트 피스 — 블록 착지 위치를 반투명 회색 블록으로 미리 표시 (GhostPiece.cs 추가) |
| 2026-03-17 | feat: 드래그 연속 이동 — 스와이프 1회→IDragHandler 기반 실시간 드래그 이동으로 개선 |
| 2026-03-17 | fix: UI 겹침 4건 — CameraFit topMargin 26%, GameOver/Pause VerticalLayoutGroup, 버튼 autoSizing |
| 2026-03-17 | feat: GameOver SCORE/BEST 나란히 배치 — HorizontalLayoutGroup 기반 런타임 UI 생성 |
