기획서 링크 : https://docs.google.com/document/d/1XNY8ATNEtwMKyMukeV8BUeJsLT2HTG19R69q0W4weGI/edit?tab=t.0

유니티 모듈 : 데스크탑,모바일 꼭 설치해야함

시나리오 링크 : https://docs.google.com/spreadsheets/d/156fvxlO_H1pA03kz_f6NPqZxSZ7FuM6Qk09v6qHuyzM/edit?gid=0#gid=0

저작권 관련 이슈 : https://github.com/Mapilae/Comterny/issues/26
# Comterny

**버전:** 1.0  
**개발자:** Comterny 팀 ([기여자 목록](https://github.com/Mapilae/Comterny/graphs/contributors))  
**플랫폼:** Unity 6  

## 프로젝트 여는 방법 (개발용)
에디터 버전은 **6000.3.6f1** 을 사용합니다.

### Unity Hub로 열기
1. Unity Hub → **Projects → Add → Add project from disk**
2. `Assets`, `Packages`, `ProjectSettings` 가 바로 들어있는 **프로젝트 루트 폴더**를 선택합니다.  
   (상위 폴더나 `Assets` 폴더를 고르면 `Please add a valid project` 오류가 납니다.)

### Hub 없이 바로 열기 (PowerShell)
Hub에서 인식이 안 될 때는 에디터 실행 파일로 직접 엽니다. 경로는 각자 환경에 맞게 수정하세요.
```powershell
& "<Unity 설치 경로>\6000.3.6f1\Editor\Unity.exe" -projectPath "<프로젝트 경로>\Comterny"

# 예시
& "C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe" -projectPath "E:\codingstudy\Comterny"
```
처음 열 때는 `Library` 폴더를 생성하느라 시간이 걸립니다.

### .sln 파일 생성
`.sln` / `.csproj` 는 git에 올리지 않으므로 직접 생성해야 합니다.
1. **Edit → Preferences → External Tools** 에서 External Script Editor 를 선택 (Visual Studio / Rider / VS Code)
2. **Regenerate project files** 클릭 → 프로젝트 루트에 `Comterny.sln` 생성

### 시작(디폴트) 씬 설정
처음 프로젝트를 열면 빈 `Untitled` 씬이 열립니다. 마지막으로 연 씬 정보는 `Library` 폴더에 저장되는데, 이 폴더는 git에 올라가지 않기 때문입니다.

**1. 에디터에 MainMenu 씬 띄우기**
- `Assets/01. Scenes/MainMenu.unity` 를 더블클릭해서 엽니다.
- 한 번 열면 이후부터는 에디터가 마지막 씬을 기억합니다.

**2. Play 시 항상 MainMenu부터 시작하기**
- `Assets/Editor/PlayFromFirstScene.cs` 가 Play 버튼을 누르면 현재 열린 씬과 관계없이 **Build Settings 의 첫 번째 씬 (MainMenu)** 부터 실행하도록 설정합니다. (기본값: 켜짐)
- 현재 열린 씬에서 바로 테스트하려면 메뉴 **Tools → Play From First Scene** 을 눌러 끄면 됩니다. (체크 표시로 상태 확인)

**3. 빌드 시 시작 씬 변경**
- **File → Build Profiles (Build Settings) → Scene List** 에서 맨 위(인덱스 0)에 있는 씬이 게임 시작 씬입니다.
- 순서를 바꾸면 Play 시작 씬(2번)도 같이 바뀝니다.
- 현재 순서: `MainMenu` → `MemoryDept` → `BusStop` → `CentralProcessingDept` → `SystemSoftware` → `GoToOS` → `OSBossCombat`

## 소개
Comterny는 탑뷰 방식으로 진행되는 교육용 퍼즐 게임으로, 컴퓨터 내부 세계를 돌아다니며 버그를 잡고 스택, 큐, 이진 트리, 다익스트라, 논리 게이트, 패리티 비트 같은 컴퓨터 과학 개념을 배우는 게임입니다.

## 설치 방법
현재 별도의 배포 파일은 없습니다. 아래 [빌드 방법](#빌드-방법)을 따라 직접 빌드해서 실행합니다.

## 기본 조작법
| 키 | 동작 |
|----|------|
| W | 위로 이동 |
| A | 왼쪽으로 이동 |
| S | 아래로 이동 |
| D | 오른쪽으로 이동 |
| Shift / 왼쪽 Ctrl (누른 채 이동) | 달리기 |
| Space | 상호작용, 대사 넘기기 |
| 마우스 클릭 / 휠 | 대사 넘기기 |
| ESC | 일시정지 메뉴 열기/닫기 |

모바일 빌드에서는 화면의 조이스틱으로 이동합니다.

## 게임 방식
- **탑뷰 방식**으로 진행되며, 캐릭터를 조작해 메모리부, 중앙처리부 등 컴퓨터 내부를 본뜬 지역을 차례로 이동합니다.
- 대화와 미니게임을 진행하면서 개념을 익힙니다.
- 미니게임: 스택, 큐, 이진 트리, 깊이 우선 탐색, 다익스트라, 논리 게이트, 패리티 비트, 이진수 덧셈, 10진수-2진수 변환 등

## 설정 및 옵션
일시정지 메뉴에서 다음을 사용할 수 있습니다.
- **배경음 볼륨** 조절
- **미니게임 건너뛰기**: 진행 중인 미니게임이 있을 때만 활성화됩니다.

## 빌드 방법
### 1. Unity 6에서 프로젝트 열기
- Unity 6을 실행하고 `Comterny` 프로젝트를 엽니다.

### 2. 빌드 설정
1. **파일(File) > 빌드 설정(Build Settings)** 메뉴로 이동합니다.
2. `플랫폼(Platform)`을 선택 (예: Windows, macOS, Android 등).
3. `플랫폼 전환(Switch Platform)` 버튼을 클릭하여 선택한 플랫폼으로 변경합니다.
4. `씬 추가(Add Open Scenes)` 버튼을 눌러 필요한 씬을 빌드에 포함합니다.

### 3. 플레이어 설정 조정
1. **편집(Edit) > 프로젝트 설정(Project Settings)**으로 이동합니다.
2. **플레이어(Player) 설정**에서 **회사 이름, 제품 이름, 아이콘, 해상도 설정** 등을 조정합니다.

### 4. 빌드 실행
1. `빌드(Build)` 버튼을 클릭하여 빌드 파일을 생성합니다.
2. 실행 파일이 생성된 폴더에서 게임을 실행하여 정상적으로 동작하는지 확인합니다.

Android 서명 키(`*.keystore`)는 저장소에 올리지 않습니다. 팀 내부에서 따로 전달받아 **Project Settings → Player → Publishing Settings** 에서 지정합니다.

## 문제 해결
| 문제 | 해결 방법 |
|------|----------|
| Unity Hub에서 `Please add a valid project` 오류 | `Assets`, `Packages`, `ProjectSettings` 가 바로 들어있는 프로젝트 루트 폴더를 선택합니다. |
| 프로젝트를 열면 `Untitled` 씬이 보임 | [시작(디폴트) 씬 설정](#시작디폴트-씬-설정)을 참고합니다. |
| 프로젝트 루트에 `.sln` 파일이 없음 | [.sln 파일 생성](#sln-파일-생성)을 참고합니다. |

## 문의 및 지원
버그 제보와 문의는 [GitHub Issues](https://github.com/Mapilae/Comterny/issues)에 남겨 주세요.

## 라이선스
별도의 라이선스를 지정하지 않았습니다. 저작권 관련 논의는 [이슈 #26](https://github.com/Mapilae/Comterny/issues/26)을 참고하세요.
