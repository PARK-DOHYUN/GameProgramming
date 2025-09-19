#include <stdio.h>
#include <math.h>
#include <conio.h>
#include <windows.h>

// 음계 index (도, 레, 미, 파, 솔, 라, 시, 도)
int index_table[] = {0, 2, 4, 5, 7, 9, 11, 12};

// 주파수 계산 함수
double calc_frequency(int octave, int index)
{
    return 440.0 * pow(2.0, (index - 9 + (octave - 4) * 12) / 12.0);
}

// 직접 연주 모드
void practice_piano(void)
{
    int freq[8], code, i;

    // 각 음계의 주파수 미리 계산
    for (i = 0; i < 8; i++)
    {
        freq[i] = (int)calc_frequency(4, index_table[i]);
    }

    printf("🎹 직접 연주 모드 (1~8: 도~도, ESC: 종료)\n");

    do
    {
        code = getch();
        if ('1' <= code && code <= '8')
        {
            code = code - '1'; // '1' → 0, '2' → 1, ...
            Beep(freq[code], 300);
        }
    } while (code != 27); // ESC 키(27) 누르면 종료
}

// 학교종이 땡땡땡 자동 연주
void play_song(void)
{
    int freq[8], i;
    for (i = 0; i < 8; i++)
    {
        freq[i] = (int)calc_frequency(4, index_table[i]);
    }

    int beat = 400; // 한 박자(ms)

    // 도=0, 레=1, 미=2, 파=3, 솔=4, 라=5, 시=6, 도=7
    int song[] = {
        0, 0, 4, 4, 5, 5, 4, // 도도솔솔라라솔
        3, 3, 2, 2, 1, 1, 0, // 파파미미레레도
        4, 4, 3, 3, 2, 2, 1, // 솔솔파파미미레
        4, 4, 3, 3, 2, 2, 1, // 솔솔파파미미레
        0, 0, 4, 4, 5, 5, 4, // 도도솔솔라라솔
        3, 3, 2, 2, 1, 1, 0  // 파파미미레레도
    };
    int length = sizeof(song) / sizeof(song[0]);

    printf("🔔 학교종이 땡땡땡 자동 연주 시작!\n");
    for (i = 0; i < length; i++)
    {
        Beep(freq[song[i]], beat);
        Sleep(50); // 음 사이 간격
    }
    printf("✅ 연주 완료!\n");
}

int main(void)
{
    int menu;

    while (1)
    {
        printf("\n=== 메뉴 선택 ===\n");
        printf("1. 직접 연주 모드\n");
        printf("2. 학교종이 땡땡땡 자동 연주\n");
        printf("3. 종료\n");
        printf("선택: ");
        scanf("%d", &menu);

        if (menu == 1)
        {
            practice_piano();
        }
        else if (menu == 2)
        {
            play_song();
        }
        else if (menu == 3)
        {
            break;
        }
        else
        {
            printf("잘못된 입력입니다.\n");
        }
    }

    return 0;
}
