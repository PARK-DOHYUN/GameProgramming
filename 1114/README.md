# Unity RollingBall 프로젝트

Unity를 활용한 기초 3D 게임 프로젝트입니다. 경사면을 굴러내려가는 공을 만들어보는 간단한 예제입니다.

## 📋 프로젝트 정보

- **프로젝트명**: RollingBall
- **Unity 버전**: 2021.3.7f1 이상
- **프로젝트 유형**: 3D Core
- **제작일**: 2022.07.24

## 🎯 학습 목표

- Unity 기본 인터페이스 사용법
- 3D 오브젝트 생성 및 배치
- Transform 컴포넌트 활용
- 물리 엔진(Rigidbody, Collider) 적용
- Material을 통한 오브젝트 속성 변경

## 🚀 시작하기

### 필수 요구사항

- Unity Hub
- Unity Editor 2021.3.7f1 이상

### 설치 방법

1. [Unity 공식 웹사이트](https://unity.com/)에서 Unity Hub 다운로드
2. Unity Hub에서 Unity Editor 설치
3. 새 프로젝트 생성 (3D Core 템플릿 선택)
4. 프로젝트명을 "RollingBall"로 설정

## 🎮 프로젝트 구성

### 1. 바닥 (Floor)
```
Position: (0, 0, 0)
Scale: (10, 0.5, 5)
```

### 2. 벽 (Walls)
- **Wall01**: Position (-5.3, 4.6, 0), Scale (1, 10, 5)
- **Wall02**: Position (5.3, 4.6, 0), Scale (1, 10, 5)
- **Wall03**: Position (0, 4.6, 2), Scale (10, 10, 1)

### 3. 경사면 (Slopes)
- **Slope01**: Position (-1, 8.57, 1), Rotation (0, 0, -10), Scale (8, 0.3, 2)
- **Slope02**: Position (-1.5, 3.9, 1), Rotation (0, 0, -10), Scale (8, 0.3, 2)
- **Slope03**: Position (-3, 0.35, 1), Rotation (0, 0, -10), Scale (5, 0.3, 2)
- **Slope04**: Position (0.9, 6.2, 1), Rotation (0, 0, 8), Scale (8, 0.3, 2.2)
- **Slope05**: Position (0.9, 2.2, 1), Rotation (0, 0, 8), Scale (8, 0.3, 2.2)

### 4. 공 (Ball)
```
Position: (-4.4, 9.58, 0.7)
Scale: (0.4, 0.4, 0.4)
Components: Rigidbody, Sphere Collider (Bounce Material)
```

### 5. 카메라 및 조명
- **Main Camera**: Position (0, 8, -11.9), Rotation (18, 0, 0)
- **Directional Light**: Position (0, 0, -11.9), Rotation (10, 0, 0)

## 🛠️ 주요 컴포넌트

### Rigidbody
- Use Gravity 활성화
- 공에 중력 효과 적용

### Physic Material (Bounce)
- Bounciness: 0.6
- 공의 탄성 설정

### Material (Ball)
- 원하는 색상으로 공 색상 변경

## ⌨️ 주요 단축키

| 단축키 | 기능 |
|--------|------|
| `Q` | Hand Tool (화면 이동) |
| `W` | Move Tool (오브젝트 이동) |
| `E` | Rotate Tool (오브젝트 회전) |
| `R` | Scale Tool (크기 조절) |
| `F` | 선택한 오브젝트 포커스 |
| `Ctrl + D` | 오브젝트 복제 |

## 🎮 실행 방법

1. Unity Editor에서 프로젝트 열기
2. Game 뷰로 전환
3. 상단의 Play 버튼(▶) 클릭
4. 공이 경사면을 따라 굴러내려가는 것을 확인

## 📚 참고 자료

- [Unity 공식 웹사이트](https://unity.com/)
- [Unity 공식 문서](https://docs.unity3d.com/)
- (초보자를 위한) 유니티 5 입문, 아라카와 다쿠야, 아사노 유이치 지음, 윤준 옮김, 한빛미디어

## 📄 라이선스

이 프로젝트는 교육 목적으로 제작되었습니다.

## 👨‍🏫 제작

경성대학교 게임 프로그래밍 수업 자료
