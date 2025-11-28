# 언리얼 엔진 5 기초 가이드

## 📑 목차
- [EP1: 언리얼 엔진 설치 및 실행](#ep1-언리얼-엔진-설치-및-실행)
- [EP2: 프로젝트 생성과 기본 가이드](#ep2-프로젝트-생성과-기본-가이드)
- [EP3: 블루프린트 기초](#ep3-블루프린트-기초)
- [EP4: C++ 개발 기초](#ep4-c-개발-기초)
- [EP5: 샘플 & 애셋 사용법](#ep5-샘플--애셋-사용법)

---

## EP1: 언리얼 엔진 설치 및 실행

### 🔧 설치 과정

#### 1. Epic Games 웹사이트 접속
- Epic Games 검색 후 접속
- 상단의 `Download` 또는 `Download Launcher` 클릭

#### 2. 계정 생성/로그인
- Epic Games 계정 생성 또는 소셜 계정 연동

#### 3. 런처 실행
- 기본적으로 News 탭이 표시됨
- 좌측 메뉴에서 `UnrealEngine` 선택
- `Library` 항목에서 설치된 엔진 버전 확인

#### 4. 엔진 다운로드
- 엔진이 설치되어 있지 않다면 우측 상단의 노란색 `Launch` 버튼 클릭
- 설치 경로와 파일 이름 지정
- `Options` 버튼에서 필요한 기능 선택/해제 가능
- `Install` 클릭하여 설치 완료

### 💡 TIP: 기존 프로젝트 찾기
- Library 탭 하단의 **My Projects** 섹션에서 기존 프로젝트 확인 가능
- ⚠️ **주의**: 자동 저장 위치가 아닌 다른 곳에 저장한 프로젝트는 인식되지 않음

---

## EP2: 프로젝트 생성과 기본 가이드

### 🎮 새 프로젝트 생성

1. **런처에서 Launch 버튼 클릭**
2. **Unreal Project Browser 화면 대기**
3. **GAMES 카테고리 선택**
4. **템플릿 선택 및 프로젝트 설정**
   - 원하는 템플릿 선택
   - 프로젝트 이름 입력
   - `Create` 클릭

### 🖥️ 레이아웃 설정
- UE4 레이아웃과 UE5 레이아웃 중 선택 가능
- 상단 메뉴: `창` → `레이아웃`에서 변경

### 📐 화면 구성

| 영역 | 단축키 | 설명 |
|------|--------|------|
| **뷰포트 (Viewport)** | - | 게임 화면을 보고 오브젝트를 배치하는 공간 |
| **메뉴바/툴바** | - | UE의 기본 동작 제어 |
| **아웃라이너 (Outliner)** | - | 뷰포트 내 오브젝트의 계층 구조 |
| **디테일 (Detail)** | - | 오브젝트 & 스크립트의 세부 설정 |
| **콘텐츠 브라우저** | `Ctrl + Space` | 게임 자료 저장 공간 |
| **액터 배치** | - | UE 기본 오브젝트/스크립트 |

### 🎯 기본 조작법

#### 뷰포트 이동
- **마우스 우클릭 + WASD**로 화면 이동
- 스냅 On/Off 및 크기 조정 가능
- 카메라 속도 조절 가능

#### 오브젝트 조작
- `Q`: 선택 모드
- `W`: 이동 모드
- `E`: 회전 모드
- `R`: 스케일 모드

### 🗺️ 레벨 생성

1. Content Browser의 `Content` 폴더로 이동
2. 우클릭 → 폴더 생성
3. 폴더 내부에서 우클릭 → `Level` 클릭
4. 더블클릭하여 레벨 열기

### 🌅 환경 설정

#### 방법 1: 환경 라이트 믹서
1. 상단 메뉴에서 `환경 라이트 믹서` 열기
2. 필요한 라이트 요소 생성

#### 방법 2: 액터 배치
1. `액터 배치` → `라이트` 선택
2. 필요한 라이트 컴포넌트 추가:

| 컴포넌트 | 설명 |
|----------|------|
| **Sky Light** | 간접광 표현 |
| **Directional Light** | 태양광 시뮬레이션 (방향 조절 가능) |
| **Sky Atmosphere** | 대기 시뮬레이션 |
| **Volumetric Cloud** | 구름 생성 |
| **Exponential Height Fog** | 안개 생성 |

> 💡 **TIP**: Directional Light를 선택하고 `E`키로 회전하면 시간대 변화 확인 가능

---

## EP3: 블루프린트 기초

### 📘 블루프린트 종류

#### 1. 레벨 블루프린트
- 상단 툴바 → `블루프린트` → `Open Level Blueprint`
- 현재 레벨에만 적용되는 스크립트

#### 2. 일반 블루프린트
- 콘텐츠 브라우저에서 우클릭
- `Blueprint Class` → `Actor` 선택
- 재사용 가능한 독립적인 스크립트

### 🖼️ 블루프린트 레이아웃

| 영역 | 설명 |
|------|------|
| **이벤트 그래프** | 블루프린트 로직을 작성하는 공간 |
| **상태창** | 함수, 변수, 이벤트 등을 관리 |
| **디테일** | 선택한 요소의 상세 설정 |

### 🎓 실습: 키보드 입력으로 화면에 문구 띄우기

#### 필요한 요소
- Input Mapping Context
- Input Action
- Print String
- EnhancedInputAction

#### 작업 순서

##### 1. Input Action 생성
```
1. Content/ThirdPerson/Input 폴더로 이동
2. 우클릭 → Input → Input Action 생성
3. 이름: IA_Click (또는 원하는 이름)
```

##### 2. 키 매핑 설정
```
1. ThirdPerson/Input/IMC_Default 더블클릭
2. 새 매핑 추가
3. 생성한 IA_Click 선택
4. 원하는 키 설정 (예: N키)
```

##### 3. 레벨 블루프린트 작성
```
1. 뷰포트에서 우클릭 → 레벨 블루프린트 열기
2. IA_Click 이벤트 추가
3. Print String 노드 연결
4. In String에 출력할 텍스트 입력
```

##### 4. 테스트
```
1. 플레이 버튼 클릭
2. 설정한 키를 눌러 화면에 메시지 표시 확인
```

---

## EP4: C++ 개발 기초

### 🛠️ Visual Studio 설치

#### 1. Visual Studio 다운로드
- [Visual Studio 공식 사이트](https://visualstudio.microsoft.com/)에서 Community 버전 다운로드

#### 2. 워크로드 설치
```
1. Visual Studio Installer 실행
2. 수정 클릭
3. "C++을 사용한 게임 개발" 선택
4. 추가 구성 요소:
   - v143 빌드 도구 C++ v14.38 (x86, x64)
   - MSVC v143 - VS 2022 C++ x64/x86 빌드 도구
```

#### 3. UE5와 연동
```
1. UE5 프로젝트에서 Tools → Visual Studio 열기
2. .NET 데스크톱 개발 워크로드 설치 (필요시)
```

### 🐛 버그 수정

#### 문제: Visual Studio 재설치 후 빌드 크래시
```
LINK : fatal error LNK1181: 'legacy_stdio_wide_specifiers.lib' 파일을 열 수 없습니다.
```

#### 해결 방법
1. 프로젝트 폴더의 `Saved` 파일 삭제
2. UE5 런처와 Visual Studio 종료 후 재부팅

### 🎓 실습: 화면에 문구 띄우기 (C++ 버전)

#### 1. C++ 클래스 생성
```
Tools → 새로운 C++ 클래스 → Actor 선택 → 이름 입력 (예: MyActor)
```

#### 2. 기본 Actor 클래스 구조

**MyActor.h**
```cpp
#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "MyActor.generated.h"

UCLASS()
class PROJECTNAME_API AMyActor : public AActor
{
    GENERATED_BODY()
    
public:
    AMyActor();

protected:
    virtual void BeginPlay() override;

public:
    virtual void Tick(float DeltaTime) override;
    
    // 커스텀 함수
    void LogMessage();
};
```

**MyActor.cpp**
```cpp
#include "MyActor.h"

AMyActor::AMyActor()
{
    PrimaryActorTick.bCanEverTick = true;
}

void AMyActor::BeginPlay()
{
    Super::BeginPlay();
    LogMessage();
}

void AMyActor::Tick(float DeltaTime)
{
    Super::Tick(DeltaTime);
}

void AMyActor::LogMessage()
{
    if (GEngine)
    {
        GEngine->AddOnScreenDebugMessage(-1, 5.f, FColor::Green, TEXT("Hello, World!"));
    }
}
```

#### 3. 블루프린트 생성
```
1. C++ 클래스 우클릭
2. MyActor 기반 블루프린트 클래스 생성
3. 뷰포트에 배치
4. 플레이하여 테스트
```

### 📚 주요 함수 설명

| 함수 | 설명 |
|------|------|
| `BeginPlay()` | 오브젝트 생성 시 한 번만 실행 |
| `Tick(float DeltaTime)` | 매 프레임마다 실행 |

### 🔍 주요 헤더 파일

| 헤더 | 설명 |
|------|------|
| `CoreMinimal.h` | 언리얼 핵심 기능 (타입, 매크로, 로그 등) |
| `GameFramework/Actor.h` | Actor 클래스 기능 |
| `*.generated.h` | 언리얼 리플렉션 시스템용 (항상 마지막에 위치) |

---

## EP5: 샘플 & 애셋 사용법

### 📦 샘플 프로젝트 가져오기
```
1. Epic Games 런처 실행
2. 상단 메뉴에서 "샘플" 클릭
3. 원하는 콘텐츠 예제 선택
4. "내 라이브러리 추가" 클릭
5. Library 탭으로 이동
6. "프로젝트 생성" 클릭하여 다운로드
```

### 🎨 애셋 다운로드
```
1. Epic Games 런처에서 "Fab" 클릭
2. 상위 카테고리 → "모든 애셋" 선택
3. Tag 탭에서 가격 → "무료" 체크
4. 원하는 애셋 선택 후 "구독"
5. 애셋을 추가할 프로젝트 선택
6. 프로젝트의 콘텐츠 브라우저에서 폴더 형식으로 확인
```

### 🎓 실습: 애셋으로 레벨 꾸미기
```
1. 무료 애셋 다운로드
2. 콘텐츠 브라우저에서 애셋 찾기
3. 뷰포트로 드래그하여 배치
4. W, E, R 키로 위치, 회전, 크기 조정
5. 다양한 애셋을 조합하여 레벨 완성
```

---

## 📋 템플릿 종류

### 🎮 GAMES 카테고리

| 템플릿 | 설명 |
|--------|------|
| **Blank** | 빈 프로젝트 (코드 없음) |
| **First Person** | 1인칭 시점 게임 템플릿 |
| **Third Person** | 3인칭 시점 게임 템플릿 |
| **Top Down** | 상부 시점 게임 템플릿 |
| **Handheld AR** | 모바일 AR 게임 템플릿 |
| **Virtual Reality** | VR 게임 템플릿 |
| **Vehicle** | 차량 레이싱 게임 템플릿 |

### 🎬 FILM / VIDEO & LIVE EVENTS 카테고리

| 템플릿 | 설명 |
|--------|------|
| **Virtual Production** | 모션 캡처, 실시간 시각 효과 (영상 제작용) |
| **InCameraVFX** | 실시간 렌더링, CG 가상 배경 삽입 |
| **nDisplay** | 다수의 디스플레이를 활용한 그래픽 제작 |
| **Motion Design** | 3D 모션 그래픽 제작 |
| **DMX** | 공연장 조명 제어용 프로토콜 |

### 🏗️ ARCHITECTURE 카테고리

| 템플릿 | 설명 |
|--------|------|
| **Archvis** | 건축 시각화 템플릿 |
| **Collab Viewer** | 협업용 건축 설계 시뮬레이터 |
| **Design Configurator** | 사용자 맞춤형 시뮬레이터 |
| **Handheld AR** | 건축 특화 모바일 AR |

### 🚗 AUTOMOTIVE 카테고리

| 템플릿 | 설명 |
|--------|------|
| **Photo Studio** | 자동차 홍보용 포토그래픽 |
| **Production Configuration** | 커스터마이징 가능한 실시간 처리 시스템 |

---

## 🔗 유용한 링크

- [UE5 공식 문서](https://docs.unrealengine.com/)
- [UE5 포럼](https://forums.unrealengine.com/)
- [UE5 마켓플레이스](https://www.unrealengine.com/marketplace/)
- [Fab (애셋 스토어)](https://www.fab.com/)
- [UE5 로드맵](https://portal.productboard.com/epicgames/1-unreal-engine-public-roadmap)

---

## 📝 저작권 및 라이선스

이 가이드는 교육 목적으로 작성되었습니다.  
**Unreal Engine**은 Epic Games의 등록 상표입니다.

---

## 📌 추가 팁

### 자주 사용하는 단축키

| 단축키 | 기능 |
|--------|------|
| `F` | 선택한 오브젝트로 포커스 |
| `Alt + 마우스 드래그` | 오브젝트 복제 |
| `Ctrl + D` | 오브젝트 복제 |
| `Delete` | 오브젝트 삭제 |
| `Ctrl + S` | 저장 |
| `Ctrl + Z` | 실행 취소 |
| `Ctrl + Y` | 다시 실행 |
| `End` | 오브젝트를 바닥에 정렬 |

### 프로젝트 관리 팁

- 정기적으로 백업하기
- 의미 있는 이름으로 폴더 구조화
- Git 등 버전 관리 시스템 활용
- 큰 용량의 애셋은 별도 관리

---

**제작**: [Your Name]  
**최종 수정일**: 2024년  
**버전**: 1.0
