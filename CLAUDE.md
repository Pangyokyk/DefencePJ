# CLAUDE.md

이 파일은 Claude Code(claude.ai/code)가 이 저장소에서 작업할 때 따를 지침을 제공합니다.

## 커뮤니케이션

- **사용자와는 항상 한국어로 대화한다.**

## 프로젝트

Unity 6 게임 프로젝트(버전 6000.3.18f1), Universal Render Pipeline(URP) 사용. 타워 디펜스 게임으로 추정 — AI Navigation(2.0.13)이 설치되어 있음. 대상 플랫폼: Windows Standalone 64-bit.

## Unity 관련 규칙

- **`Library/`, `Temp/`, `Logs/` 안의 파일은 절대 편집하지 않는다** — Unity가 자동 생성하며 임포트/재빌드 시 삭제됨. 영구적인 코드와 에셋은 모두 `Assets/` 또는 `Packages/` 아래에 위치한다.
- **스크립트 위치**: 런타임 스크립트는 `Assets/Scripts/` 아래에 배치(디렉토리 없으면 생성). 에디터 전용 스크립트는 런타임 빌드에서 제외되도록 반드시 `Editor/` 폴더 안에 배치.
- **어셈블리 정의**: 암묵적인 Assembly-CSharp(런타임)과 Assembly-CSharp-Editor(에디터 전용) 어셈블리를 사용한다. 명시적으로 요청받지 않는 한 `.asmdef` 파일을 생성하지 않는다.
- **씬 등록**: 새 씬은 런타임에 로드할 수 있도록 반드시 **File → Build Settings → Scenes In Build**에 추가해야 한다.
- **URP 머티리얼**: URP 호환 셰이더(Lit, Unlit 등)를 사용할 것 — Built-in 파이프라인 셰이더는 런타임에 분홍색으로 표시됨.

## 언어 및 스타일

- C# 9.0, .NET Framework 4.7.1 타겟
- `AllowUnsafeBlocks` 비활성화 — `unsafe` 코드 사용 금지
- 경고 0169(미사용 필드)와 USG0001은 `.csproj`에 이미 suppress 설정됨
- 인스펙터에 노출할 private 필드는 public으로 만들지 말고 `[SerializeField]`를 사용할 것

## 설치된 패키지

주요 패키지 (`Packages/manifest.json`에서 전체 목록 확인 가능):
- `com.unity.render-pipelines.universal` 17.3.0 — URP 렌더링
- `com.unity.inputsystem` 1.19.0 — 새 입력 시스템; 입력 바인딩은 `Assets/InputSystem_Actions.inputactions`
- `com.unity.ai.navigation` 2.0.13 — NavMesh / AI 경로탐색
- `com.unity.test-framework` 1.6.0 — Unity Test Framework (EditMode 및 PlayMode 테스트)
- `com.unity.timeline` 1.8.12 — 타임라인 시퀀스
- `com.unity.visualscripting` 1.9.11 — 비주얼 스크립팅 노드

## 테스트

Unity Test Framework가 설치되어 있으나 테스트는 아직 없음. 테스트는 `Assets/Tests/` 아래에 위치(디렉토리 없으면 생성). EditMode 테스트에는 `[Test]`, 프레임 단계가 필요한 PlayMode 테스트에는 `IEnumerator`와 함께 `[UnityTest]`를 사용.

## 렌더 파이프라인 설정

- `Assets/Settings/PC_RPAsset.asset` — PC 렌더러
- `Assets/Settings/Mobile_RPAsset.asset` — 모바일 렌더러
- 별도의 렌더러가 구성되어 있음; 명시적으로 요청받지 않는 한 통합하지 않는다.

## 버전 관리

이 프로젝트는 git을 사용한다. Unity에 적합한 `.gitignore`와 `.gitattributes`(UnityYAMLMerge 및 LFS 설정 포함)가 프로젝트 루트에 있다. GitHub 원격 저장소: https://github.com/Pangyokyk/DefencePJ
