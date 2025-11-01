# 🧭 Git & GitHub 완벽 가이드 ✨

> 이 문서는 **Git**과 **GitHub**을 처음 배우는 사람을 위한 학습 요약본입니다.
> 《Do it\! 깃 & 깃허브》 교재의 01\~08장과 NEW 특집 내용을 기반으로 정리했습니다.

-----

## 📘 01. 깃 시작하기

### 🔹 깃의 핵심 기능

  * **버전 관리:** 파일의 변경 이력 추적 및 복원
  * **백업:** 다른 컴퓨터나 서버에 복제
  * **협업:** 여러 사람이 동시에 파일 작업 가능

> 💡 **버전 → 백업 → 협업** 순서로 이해해야 함

### 🔹 깃 프로그램 종류

| 프로그램 | 특징 |
| :--- | :--- |
| **GitHub Desktop** | GUI 기반, 초보자 친화적 |
| **TortoiseGit** | 윈도우 탐색기 통합형 |
| **SourceTree** | 고급 기능 지원 |
| **CLI (Command Line)** | 명령어 기반, 개발자 선호 |

### 🔹 깃 설치 (Windows)

1.  [https://git-scm.com](https://git-scm.com) 방문
2.  `Download for Windows` 클릭
3.  설치 시 **기본값** 사용
4.  편집기: **Vim**
5.  기본 브랜치: **main**
6.  설치 확인:

<!-- end list -->

```bash
git --version
```

-----

## 📗 02. 깃으로 버전 관리하기

### 🔹 깃의 3가지 영역

| 구분 | 설명 |
| :--- | :--- |
| **Working Tree** | 실제 파일이 존재하는 폴더 |
| **Stage** | 커밋 대기 중인 파일 |
| **Repository** | 커밋(버전)이 저장되는 공간 |

### 🔹 기본 명령어

```bash
git init               # 저장소 생성
git status             # 상태 확인
git add <파일>           # 스테이징 (Stage 영역으로 이동)
git commit -m "메시지"   # 커밋 (Repository에 저장)
git log                # 커밋 이력 보기
```

### 🔹 빠른 커밋

```bash
git commit -am "메시지" # 수정된 파일을 Stage에 올리고 바로 커밋 (새 파일에는 사용 불가)
```

### 🔹 되돌리기

```bash
git restore <파일>      # Working Tree의 변경 취소
git reset --hard HEAD  # 최근 커밋으로 복구 (Stage와 Working Tree 모두 버림)
```

-----

## 📘 03. 깃과 브랜치

### 🔹 브랜치란?

독립적인 작업 공간을 만들어 여러 기능을 **병렬 개발**할 수 있도록 하는 도구.

### 🔹 주요 명령어

```bash
git branch              # 브랜치 목록 확인
git branch feature-login # 브랜치 생성
git switch feature-login # 브랜치 전환
git merge feature-login  # 현재 브랜치에 feature-login 브랜치의 내용 병합
git branch -d feature-login # 브랜치 삭제
```

### 🔹 브랜치 핵심 개념

  * **main(master):** 기본 브랜치
  * **HEAD:** 현재 브랜치가 가리키는 커밋
  * **Merge(병합):** 분기한 내용을 다시 합치기

> ⚠️ **충돌(conflict)** 발생 시 직접 수정 필요

-----

## 📙 04. 깃허브 시작하기

### 🔹 저장소 종류

| 구분 | 설명 |
| :--- | :--- |
| **Local Repository** | 내 컴퓨터에 있는 저장소 |
| **Remote Repository** | 깃허브 서버에 있는 저장소 |

### 🔹 주요 개념

  * **Push:** 로컬 → 원격 업로드
  * **Pull:** 원격 → 로컬 다운로드
  * **Clone:** 원격 저장소 복제
  * **Sync:** 두 저장소를 동일 상태로 유지

### 🔹 원격 저장소 연결

```bash
git remote add origin <주소>  # 원격 저장소 등록 (별칭 'origin')
git push -u origin main      # 로컬 main 브랜치를 origin에 푸시하고 추적 설정
```

-----

## 📒 05. 깃허브로 협업하기

### 🔹 원격 저장소 복제

```bash
git clone <주소> # 원격 저장소를 내 컴퓨터에 복제하여 Local Repository 생성
```

### 🔹 협업 기본 흐름

1.  원격 저장소를 `git clone`
2.  로컬에서 수정 후 `git commit`
3.  `git push`로 원격에 업로드
4.  다른 사람은 `git pull`로 갱신

### 🔹 원격 브랜치 확인

```bash
git log --oneline
```

> `(HEAD -> main, origin/main)` : 로컬과 원격이 동일함을 의미

-----

## 📔 06. 깃허브에서 다른 사람과 소통하기

### 🔹 프로필 관리

  * 이름, 사진, 자기소개, 링크 추가 가능
  * 활동 기록은 초록색 **컨트리뷰션 그래프**로 표시됨

### 🔹 README.md 파일

  * 저장소 설명 문서 (**자동 인식됨**)
  * **마크다운 문법**으로 작성

#### 📄 기본 마크다운 문법 예시

```markdown
# 제목 1
## 제목 2
**굵게** / *기울임* / ***굵고 기울임***
- 순서 없는 목록
1. 순서 있는 목록
[링크](https://github.com)
```

-----

## 📕 07. VS Code로 깃과 깃허브 다루기

### 🔹 VS Code에서 깃 설정하기

1.  새 폴더 생성 → [**리포지토리 초기화**] 클릭
2.  소스 제어 탭에서 변경사항 확인
3.  **스테이징** → **커밋** → **푸시**

### 🔹 터미널에서 설정 (최초 1회)

```bash
git config --global user.name "이름"
git config --global user.email "이메일"
```

### 🔹 커밋 메시지 작성

VS Code 상단의 **✓ 아이콘**을 클릭해 저장소 상태 갱신

-----

## 📔 08. 깃허브에 이력서 사이트 & 블로그 만들기

### 🔹 HTML/CSS로 사이트 준비

  * 첫 화면은 반드시 `index.html`
  * 이미지나 링크가 정상 작동하는지 확인

### 🔹 GitHub Pages 배포

1.  저장소 생성 후 파일 업로드
2.  `Settings` → `Pages` → `Branch: main` 선택 → `Save`
3.  사이트 주소:

<!-- end list -->

```
https://<사용자명>.github.io/<저장소명>/
```

> 💡 **깃허브 저장소**를 **웹사이트**처럼 사용할 수 있음\!

-----

## 🧩 NEW. 깃허브의 새로운 서비스와 기능

### 🔹 Codespaces

  * 깃허브 내에서 **VS Code 환경을 바로 실행**
  * **클라우드 개발 환경** 제공
  * 확장 기능과 터미널 모두 사용 가능

### 🔹 GitHub.dev

  * 깃허브 저장소 주소를 `github.com` → `github.dev`로 바꾸면 **웹 VS Code 실행**

### 🔹 Copilot (코파일럿)

  * **AI 기반 코드 자동 완성 서비스**
  * 무료 체험 가능 ([https://copilot.github.com](https://copilot.github.com))

-----

## 🧾 참고 링크

  * Git 공식 사이트
  * GitHub
  * Do it\! Git & GitHub 영상 강의

-----
