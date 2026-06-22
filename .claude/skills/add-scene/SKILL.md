---
name: add-scene
description: 이 프로젝트에 새 Unity 씬을 올바르게 추가하기 위한 체크리스트와 안내. 파일 위치, Build Settings 등록, URP 볼륨 설정, 라이팅을 다룬다. 새 게임플레이 레벨, 메뉴, 로딩 화면을 만들 때 사용한다.
---

새 Unity 씬을 추가할 때 아래 단계를 안내합니다.

## 1. 씬 파일 생성

Unity 에디터에서:
- **File → New Scene** — "Basic (URP)" 템플릿 선택 (URP 호환 디렉셔널 라이트와 스카이박스가 포함된 상태로 시작)
- **File → Save As** → `Assets/Scenes/<씬이름>.unity`로 저장
- `.meta` 파일이 자동 생성됨 — 반드시 git에 커밋해야 함

## 2. Build Settings 등록

여기에 등록되지 않은 씬은 **`SceneManager.LoadScene`으로 런타임에 로드할 수 없음**:
- **File → Build Settings → Add Open Scenes** (또는 `.unity` 파일을 Scenes In Build 목록으로 드래그)
- 씬 인덱스를 확인해 둘 것 — `SceneManager.LoadScene(0)`은 인덱스로 로드하므로 인덱스 변경에 취약하지 않도록 이름으로 로드하는 것을 권장

## 3. URP 볼륨 프로파일

각 씬에는 포스트 프로세싱을 위한 볼륨이 있어야 함:
- 씬 하이어라키에 **Volume** 게임 오브젝트 생성
- **Volume Profile** 에셋 할당 (`Assets/Settings/DefaultVolumeProfile.asset`을 복제해서 시작점으로 사용)
- **Mode**를 Global로 설정하여 씬 전체에 효과 적용
- 이 설정이 없으면 블룸, 컬러 그레이딩 등 포스트 프로세싱이 적용되지 않음

## 4. 라이팅 설정

- **Directional Light** 하나를 메인 라이트로 설정 (그림자를 사용한다면 "MainLight" 태그 지정)
- **Window → Rendering → Lighting** 에서 씬의 라이트맵 설정 구성
- 베이크드 라이팅을 사용한다면 **Generate Lighting** 클릭 — 베이크드 라이트맵은 씬 이름으로 된 폴더에 저장됨

## 5. 카메라 설정

- **Universal Additional Camera Data** 컴포넌트가 있는 **Camera**가 최소 하나 이상 있어야 함 (URP 사용 시 자동 추가)
- 카메라의 **Renderer**를 `Assets/Settings/`에 있는 적절한 렌더러(PC_RPAsset 또는 Mobile_RPAsset)로 설정

## 6. DefencePJ 씬 관련 팁

- 이 씬에서 NavMesh AI를 사용한다면 워크어블 지오메트리를 배치한 후 **Window → AI → Navigation** 을 열어 NavMesh를 베이크
- 게임플레이 씬이라면 지면 메시에 AI Navigation Surface 컴포넌트를 추가한 후 베이크

## 7. 완료 전 체크리스트

- [ ] 씬이 `Assets/Scenes/`에 저장됨
- [ ] Build Settings에 추가됨
- [ ] Volume Profile 할당됨
- [ ] 카메라가 최소 하나 이상 있음
- [ ] `.unity` 파일 옆에 `.meta` 파일이 존재함
- [ ] NavMesh 사용 씬의 경우: NavMesh 베이크 완료
