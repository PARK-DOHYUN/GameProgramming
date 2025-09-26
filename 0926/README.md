
## 🔢 숫자의 변환과 표현

숫자를 자리 수별로 분리해 출력하는 방법은 두 가지가 있습니다:
1. **문자열 처리**
2. **정수형 처리 (나눗셈/나머지 연산)**

### 문자열 처리 예제
```c
#include <stdio.h>
#include <string.h>

int main(void) {
    char number[20];
    int length, i;
    printf("금액을 입력하고 Enter>");
    scanf("%s", number);
    length = strlen(number);

    printf("높은 단위부터 출력\n");
    for(i = 0; i < length; i++)
        printf("%c\n", number[i]);

    printf("낮은 단위부터 출력\n");
    for(i = length - 1; i >= 0; i--)
        printf("%c\n", number[i]);
    return 0;
}
```
정수형 처리 (높은 단위부터 출력)
```c

#include <math.h>

void serial_number(long number) {
    int num, i, length = 0;
    length = (int)(log10(number) + 1);  // 자리수 계산
    for(i = length; i >= 1; i--) {
        num = number / (long)pow(10, i-1);
        printf("%ld\n", num);
        number = number - num * (long)pow(10, i-1);
    }
}
```
정수형 처리 (낮은 단위부터 출력)
```c
void reverse_number(long number) {
    while(number > 0) {
        printf("%ld\n", number % 10);
        number /= 10;
    }
}
```
🔄 재귀적 호출을 이용한 숫자 출력
높은 단위부터 출력
```c
void serial_number(long number) {
    if (number > 0) {
        serial_number(number / 10);
        printf("%ld\n", number % 10);
    }
}
```
낮은 단위부터 출력
```c
void reverse_number(long number) {
    printf("%ld\n", number % 10);
    if ((number / 10) > 0)
        reverse_number(number / 10);
}
```
🧮 디지털 숫자 출력
숫자 0~9를 5행 4열의 배열로 표현하여, 1은 ■, 0은 공백으로 출력합니다.
입력된 정수는 각 자리마다 배열을 참조해 디지털 숫자로 출력합니다.

핵심 코드
```c
void digit_print(int dim[], int line) {
    int i;
    for(i = line*4; i <= line*4+3; i++) {
        if (dim[i] == 1) printf("■");
        else printf("  ");
    }
    printf("  ");
}

void number_check(int k, int line) {
    if (k >= 1) {
        number_check(k / 10, line);
        switch(k % 10) {
            case 0: digit_print(zero, line); break;
            case 1: digit_print(one, line); break;
            case 2: digit_print(two, line); break;
            case 3: digit_print(three, line); break;
            case 4: digit_print(four, line); break;
            case 5: digit_print(five, line); break;
            case 6: digit_print(six, line); break;
            case 7: digit_print(seven, line); break;
            case 8: digit_print(eight, line); break;
            case 9: digit_print(nine, line); break;
        }
    }
}
```
🎚️ 슬라이드 바 (Slide Bar)
슬라이드 바는 키보드 방향키로 제어할 수 있는 UI 요소입니다.

수평 슬라이드 바
```c
void draw_horizontal_slide(int x, int y, int length, char *s) {
    int real_length = length / 2;
    gotoxy(1, y);
    draw_rectangle(real_length+1, 1);
    gotoxy(x+2, y+1);
    printf("%s", s);
    gotoxy(real_length*2+2, y-1);
    printf("%2d", x);
}
```
수직 슬라이드 바
```c
void draw_vertical_slide(int x, int y, int length, char *s) {
    gotoxy(x, 1);
    draw_rectangle(1, length);
    gotoxy(x+2, y+1);
    printf("%s", s);
    gotoxy(x+6, length+1);
    printf("%2d", y);
}
```
방향키 제어
```c
void move_arrow_key(char key, int *x1, int *y1, int x_b, int y_b) {
    switch(key) {
        case 72: *y1 = (*y1 > 1) ? *y1-1 : 1; break;   // 위
        case 80: *y1 = (*y1 < y_b) ? *y1+1 : y_b; break; // 아래
        case 75: *x1 = (*x1 > 1) ? *x1-1 : 1; break;   // 왼쪽
        case 77: *x1 = (*x1 < x_b) ? *x1+1 : x_b; break; // 오른쪽
    }
}
```
⬛ 도형의 이동과 회전
3×3 행렬로 표현된 도형을 방향키로 이동, 스페이스 키로 회전시킵니다.

회전 함수
```c
void rotation_right(int m[][3]) {
    int i, j, temp[3][3];
    for(i=0;i<3;i++)
        for(j=0;j<3;j++)
            temp[j][2-i] = m[i][j];
    for(i=0;i<3;i++)
        for(j=0;j<3;j++)
            m[i][j] = temp[i][j];
}
```
도형 출력
```c
void print_shape(int m[][3]) {
    int i, j;
    for(i=0;i<3;i++) {
        gotoxy(x, y+i);
        for(j=0;j<3;j++) {
            if (m[i][j] == 1) printf("□");
            else printf("  ");
        }
        printf("\n");
    }
}
```
