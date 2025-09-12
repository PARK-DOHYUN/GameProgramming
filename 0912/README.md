🎮 게임 프로그래밍 

경성대학교 소프트웨어학과 – 게임 프로그래밍 기초 강의 정리

이 문서는 콘솔 환경에서 C언어로 게임의 기본 모듈을 구현하는 방법을 다룹니다.
(개발 환경: Orwell Dev-C++
)

📌 주요 내용
1. 커서 제어

gotoxy(x, y) 함수를 사용하여 콘솔 화면에서 원하는 위치로 커서를 이동 가능

예제: 특정 좌표에 "Hello" 출력

#include <stdio.h>
#include <windows.h>
void gotoxy(int x, int y) {
    COORD Pos = {x - 1, y - 1};
    SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), Pos);
}

2. 화면 제어

system("cls")로 화면 지우기

getch(), getche(), getchar() 를 활용한 입력 처리

구구단 출력 예제 포함

3. 키 입력 처리

아스키 코드 vs 스캔 코드 개념

getch()를 이용한 화살표 키 처리

move_arrow_key() 함수로 위/아래/좌/우 이동 구현

4. 도형 그리기

텍스트 모드에서 확장 완성형 문자를 이용하여 사각형/격자 출력

draw_square(int size) 함수로 크기 조절 가능

5. 메뉴 만들기

1단계 메뉴 : 햄버거 / 스파게티 선택

2단계 서브 메뉴 : 예) 치킨버거, 치즈버거 / 토마토 스파게티, 크림 스파게티

printf("간식 만들기\n\n");
printf("1. 햄버거\n");
printf("2. 스파게티\n");
printf("3. 프로그램 종료\n\n");

6. 난수 생성

rand()와 srand(time(NULL))을 이용

범위 내 난수, 주사위 굴리기, 로또 번호 생성 예제 포함

중복 없는 난수 + 정렬 구현 (selection_sort() 사용)

7. 가변 인수 (Variable Arguments)

<stdarg.h> 라이브러리 활용

printf, scanf와 같은 함수의 원리 이해

va_list, va_start, va_arg, va_end 사용 예제

double sum(int count, ...) {
    double total=0, number;
    va_list ap;
    va_start(ap, count);
    for(int i=0;i<count;i++) {
        number = va_arg(ap, double);
        total += number;
    }
    va_end(ap);
    return total;
}

📖 참고자료
