#include <stdio.h>
#include <stdlib.h>
#include <conio.h>
#include <windows.h>
#include <time.h>

#define BOARD_WIDTH 12
#define BOARD_HEIGHT 22
#define SHAPE_SIZE 4

// 게임 보드
int board[BOARD_HEIGHT][BOARD_WIDTH];
int prev_board[BOARD_HEIGHT][BOARD_WIDTH]; // 이전 보드 상태 저장

// 테트리스 블록들 (7가지)
int shapes[7][4][4] = {
    // I블록
    {{0, 0, 0, 0}, {1, 1, 1, 1}, {0, 0, 0, 0}, {0, 0, 0, 0}},
    // O블록
    {{0, 0, 0, 0}, {0, 1, 1, 0}, {0, 1, 1, 0}, {0, 0, 0, 0}},
    // T블록
    {{0, 0, 0, 0}, {0, 1, 0, 0}, {1, 1, 1, 0}, {0, 0, 0, 0}},
    // S블록
    {{0, 0, 0, 0}, {0, 1, 1, 0}, {1, 1, 0, 0}, {0, 0, 0, 0}},
    // Z블록
    {{0, 0, 0, 0}, {1, 1, 0, 0}, {0, 1, 1, 0}, {0, 0, 0, 0}},
    // J블록
    {{0, 0, 0, 0}, {1, 0, 0, 0}, {1, 1, 1, 0}, {0, 0, 0, 0}},
    // L블록
    {{0, 0, 0, 0}, {0, 0, 1, 0}, {1, 1, 1, 0}, {0, 0, 0, 0}}};

// 현재 블록 정보
int current_shape[4][4];
int current_x = 4, current_y = 0;
int score = 0, prev_score = -1;
int lines_cleared = 0, prev_lines = -1;

// 함수 선언
void gotoxy(int x, int y);
void hide_cursor();
void init_board();
void print_board();
void print_info();
void copy_shape(int shape_index);
void rotation_right();
int check_collision(int dx, int dy, int rotated[4][4]);
void place_block();
int clear_lines();
void new_block();
void game_over();
int kbhit_timeout();

int main(void)
{
    srand(time(NULL));
    hide_cursor(); // 커서 숨기기
    init_board();
    new_block();

    system("cls");
    print_board();
    print_info();

    int fall_time = 0;
    char key;

    while (1)
    {
        int need_refresh = 0;

        // 자동 낙하
        if (fall_time++ > 10) // 약 1초마다 낙하
        {
            if (!check_collision(0, 1, current_shape))
            {
                current_y++;
            }
            else
            {
                place_block();
                if (clear_lines() > 0)
                {
                    score += 100;
                    lines_cleared++;
                }
                new_block();
                if (check_collision(0, 0, current_shape))
                {
                    game_over();
                    break;
                }
            }
            fall_time = 0;
            need_refresh = 1;
        }

        // 키 입력 처리
        if (kbhit())
        {
            key = getch();
            switch (key)
            {
            case 27: // ESC
                goto game_end;
            case 72: // 위쪽 화살표 (회전)
                rotation_right();
                need_refresh = 1;
                break;
            case 75: // 왼쪽 화살표
                if (!check_collision(-1, 0, current_shape))
                {
                    current_x--;
                    need_refresh = 1;
                }
                break;
            case 77: // 오른쪽 화살표
                if (!check_collision(1, 0, current_shape))
                {
                    current_x++;
                    need_refresh = 1;
                }
                break;
            case 80: // 아래쪽 화살표 (빠른 낙하)
                if (!check_collision(0, 1, current_shape))
                {
                    current_y++;
                    need_refresh = 1;
                }
                break;
            case 32: // 스페이스바 (한번에 떨어뜨리기)
                while (!check_collision(0, 1, current_shape))
                    current_y++;
                need_refresh = 1;
                break;
            }
        }

        // 화면 갱신이 필요할 때만 다시 그리기
        if (need_refresh)
        {
            print_board();
            print_info();
        }

        Sleep(100);
    }

game_end:
    gotoxy(1, BOARD_HEIGHT + 5);
    printf("게임을 종료합니다.\n");
    return 0;
}

void gotoxy(int x, int y)
{
    COORD Pos = {x - 1, y - 1};
    SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), Pos);
}

void hide_cursor()
{
    CONSOLE_CURSOR_INFO cursor_info;
    cursor_info.bVisible = FALSE; // 커서를 보이지 않게 함
    cursor_info.dwSize = 1;
    SetConsoleCursorInfo(GetStdHandle(STD_OUTPUT_HANDLE), &cursor_info);
}

void init_board()
{
    int i, j;
    for (i = 0; i < BOARD_HEIGHT; i++)
    {
        for (j = 0; j < BOARD_WIDTH; j++)
        {
            if (j == 0 || j == BOARD_WIDTH - 1 || i == BOARD_HEIGHT - 1)
            {
                board[i][j] = 1;       // 벽
                prev_board[i][j] = -1; // 다른 값으로 초기화하여 변화 감지
            }
            else
            {
                board[i][j] = 0;       // 빈 공간
                prev_board[i][j] = -1; // 다른 값으로 초기화하여 변화 감지
            }
        }
    }
}

void print_board()
{
    int i, j;

    // 현재 블록을 임시로 보드에 그리기
    int temp_board[BOARD_HEIGHT][BOARD_WIDTH];
    for (i = 0; i < BOARD_HEIGHT; i++)
        for (j = 0; j < BOARD_WIDTH; j++)
            temp_board[i][j] = board[i][j];

    // 현재 블록 추가 (벽이 아닌 빈 공간에만)
    for (i = 0; i < 4; i++)
    {
        for (j = 0; j < 4; j++)
        {
            if (current_shape[i][j] == 1)
            {
                int board_x = current_x + j;
                int board_y = current_y + i;
                if (board_y >= 0 && board_y < BOARD_HEIGHT &&
                    board_x >= 0 && board_x < BOARD_WIDTH &&
                    board[board_y][board_x] == 0) // 빈 공간인 경우만
                {
                    temp_board[board_y][board_x] = 2;
                }
            }
        }
    }

    // 변경된 부분만 출력 (더 부드러운 화면 갱신)
    for (i = 0; i < BOARD_HEIGHT; i++)
    {
        for (j = 0; j < BOARD_WIDTH; j++)
        {
            if (temp_board[i][j] != prev_board[i][j])
            {
                gotoxy(j * 2 + 1, i + 1);
                if ((j == 0 || j == BOARD_WIDTH - 1 || i == BOARD_HEIGHT - 1))
                    printf("??"); // 벽 (더 두껍게)
                else if (temp_board[i][j] == 1)
                    printf("■"); // 고정된 블록
                else if (temp_board[i][j] == 2)
                    printf("□"); // 현재 움직이는 블록
                else
                    printf("  "); // 빈 공간
            }
        }
    }

    // 이전 상태 저장
    for (i = 0; i < BOARD_HEIGHT; i++)
        for (j = 0; j < BOARD_WIDTH; j++)
            prev_board[i][j] = temp_board[i][j];
}

void print_info()
{
    // 점수가 변경되었을 때만 다시 그리기
    if (score != prev_score)
    {
        gotoxy(30, 4);
        printf("점수: %d    ", score); // 공백으로 이전 텍스트 지우기
        prev_score = score;
    }

    // 라인 수가 변경되었을 때만 다시 그리기
    if (lines_cleared != prev_lines)
    {
        gotoxy(30, 5);
        printf("라인: %d    ", lines_cleared); // 공백으로 이전 텍스트 지우기
        prev_lines = lines_cleared;
    }

    // 처음 한번만 고정 정보 출력
    static int first_time = 1;
    if (first_time)
    {
        gotoxy(30, 2);
        printf("=== 테트리스 ===");
        gotoxy(30, 7);
        printf("조작법:");
        gotoxy(30, 8);
        printf("↑: 회전");
        gotoxy(30, 9);
        printf("←→: 이동");
        gotoxy(30, 10);
        printf("↓: 빠른 낙하");
        gotoxy(30, 11);
        printf("Space: 즉시 낙하");
        gotoxy(30, 12);
        printf("ESC: 종료");
        first_time = 0;
    }
}

void copy_shape(int shape_index)
{
    int i, j;
    for (i = 0; i < 4; i++)
        for (j = 0; j < 4; j++)
            current_shape[i][j] = shapes[shape_index][i][j];
}

void rotation_right()
{
    int i, j;
    int temp[4][4];

    // 회전된 모양을 임시 배열에 저장
    for (i = 0; i < 4; i++)
        for (j = 0; j < 4; j++)
            temp[j][3 - i] = current_shape[i][j];

    // 충돌 체크
    if (!check_collision(0, 0, temp))
    {
        // 충돌이 없으면 회전 적용
        for (i = 0; i < 4; i++)
            for (j = 0; j < 4; j++)
                current_shape[i][j] = temp[i][j];
    }
}

int check_collision(int dx, int dy, int shape[4][4])
{
    int i, j;
    for (i = 0; i < 4; i++)
    {
        for (j = 0; j < 4; j++)
        {
            if (shape[i][j] == 1)
            {
                int new_x = current_x + j + dx;
                int new_y = current_y + i + dy;

                // 경계 체크
                if (new_x < 0 || new_x >= BOARD_WIDTH ||
                    new_y < 0 || new_y >= BOARD_HEIGHT)
                    return 1;

                // 다른 블록과의 충돌 체크
                if (board[new_y][new_x] == 1)
                    return 1;
            }
        }
    }
    return 0;
}

void place_block()
{
    int i, j;
    for (i = 0; i < 4; i++)
    {
        for (j = 0; j < 4; j++)
        {
            if (current_shape[i][j] == 1)
            {
                int board_x = current_x + j;
                int board_y = current_y + i;
                if (board_y >= 0 && board_y < BOARD_HEIGHT &&
                    board_x >= 0 && board_x < BOARD_WIDTH)
                {
                    board[board_y][board_x] = 1;
                }
            }
        }
    }
}

int clear_lines()
{
    int cleared = 0;
    int i, j, k;

    for (i = BOARD_HEIGHT - 2; i >= 1; i--) // 바닥과 천장 제외
    {
        int full = 1;
        for (j = 1; j < BOARD_WIDTH - 1; j++) // 벽 제외
        {
            if (board[i][j] == 0)
            {
                full = 0;
                break;
            }
        }

        if (full)
        {
            // 라인 제거 및 위쪽 블록들 아래로 이동
            for (k = i; k > 1; k--)
            {
                for (j = 1; j < BOARD_WIDTH - 1; j++)
                    board[k][j] = board[k - 1][j];
            }
            // 맨 위쪽 라인 비우기
            for (j = 1; j < BOARD_WIDTH - 1; j++)
                board[1][j] = 0;

            cleared++;
            i++; // 같은 라인을 다시 체크
        }
    }
    return cleared;
}

void new_block()
{
    current_x = 4;
    current_y = 0;
    copy_shape(rand() % 7);
}

void game_over()
{
    gotoxy(5, BOARD_HEIGHT / 2);
    printf("게임 오버!");
    gotoxy(5, BOARD_HEIGHT / 2 + 1);
    printf("최종 점수: %d", score);
    gotoxy(5, BOARD_HEIGHT / 2 + 2);
    printf("아무 키나 누르세요...");
    getch();
}

int kbhit_timeout()
{
    return kbhit();
}