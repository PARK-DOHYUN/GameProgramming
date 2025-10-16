#include <stdio.h>
#include <stdlib.h>
#include <conio.h>
#include <time.h>
#include <windows.h>

#define PERFECT_ZONE 3
#define GOOD_ZONE 6
#define BAD_ZONE 10
#define TIMING_BAR_LENGTH 40

// ���� �ڵ�
#define COLOR_RESET 7
#define COLOR_PERFECT 10  // ���� ���
#define COLOR_GOOD 14     // �����
#define COLOR_BAD 12      // ������
#define COLOR_NORMAL 8    // ȸ��
#define COLOR_PLAYER 11   // û�ϻ�
#define COLOR_AI 13       // ��ȫ��

// �¸� ����
#define WIN_POSITION_LEFT 12   // �������� 8ĭ �̵��ϸ� �¸�
#define WIN_POSITION_RIGHT 28  // ���������� 8ĭ �̵��ϸ� �¸�

typedef struct {
    int position;
    int direction;
    int speed;
} TimingBar;

void intro_game();
void gotoxy(int x, int y);
void set_color(int color);
void display_game_field(int rope_pos);
void display_score_board(int s_w[], int s_l[], int rope_pos);
void make_decision(int r_s, int s_w[], int s_l[]);
void game_control(int *r_s, int turn_count);
void init_timing_bar(TimingBar *bar);
void display_timing_bar(TimingBar *bar, int center);
int check_timing(TimingBar *bar, int center);
void display_timing_result(int timing_score, int is_player);
void draw_border();
void display_winner(int winner);
void clear_line(int y);
int ai_play();

int main(void)
{
    int score_win[2]={0}, score_loose[2]={0}, r_start;
    int turn_count = 0;
    
    srand(time(NULL));
    
    // �ܼ� â ũ�� ����
    system("mode con: cols=100 lines=35");
    
    intro_game();
    
    do
    {
        system("cls");
        draw_border();
        r_start = 20;
        turn_count = 0;
        
        display_score_board(score_win, score_loose, r_start);
        
        gotoxy(35, 32);
        set_color(14);
        printf("�ƹ� Ű�� ���� ��� ����!");
        set_color(COLOR_RESET);
        getch();
        clear_line(32);
        
        while((WIN_POSITION_LEFT < r_start) && (r_start < WIN_POSITION_RIGHT))
        {
            game_control(&r_start, turn_count);
            turn_count++;
            display_score_board(score_win, score_loose, r_start);
        }
        
        make_decision(r_start, score_win, score_loose);
        
    }while((score_win[0]<2) && (score_win[1]<2));
    
    // ���� ���� ǥ��
    system("cls");
    if(score_win[0] >= 2)
        display_winner(1);
    else
        display_winner(2);
    
    gotoxy(32, 30);
    set_color(COLOR_RESET);
    printf("������ �����մϴ�.");
    gotoxy(1, 34);
    return 0;
}

void intro_game()
{
    system("cls");
    
    // �ƽ�Ű ��Ʈ �ΰ�
    gotoxy(18, 3);
    set_color(COLOR_PLAYER);
    printf("  _____ _   _  ____ ");
    gotoxy(48, 3);
    set_color(COLOR_AI);
    printf("  ___  _____   __      ___    ____  ");
    
    gotoxy(18, 4);
    set_color(COLOR_PLAYER);
    printf(" |_   _| | | |/ ___|");
    gotoxy(48, 4);
    set_color(COLOR_AI);
    printf(" / _ \\|  ___|  \\ \\    / / \\  |  _ \\ ");
    
    gotoxy(18, 5);
    set_color(COLOR_PLAYER);
    printf("   | | | | | | |  _ ");
    gotoxy(48, 5);
    set_color(COLOR_AI);
    printf("| | | | |_    \\ \\ /\\ / / _ \\ | |_) |");
    
    gotoxy(18, 6);
    set_color(COLOR_PLAYER);
    printf("   | | | |_| | |_| |");
    gotoxy(48, 6);
    set_color(COLOR_AI);
    printf("| |_| |  _|    \\ V  V / ___ \\|  _ < ");
    
    gotoxy(18, 7);
    set_color(COLOR_PLAYER);
    printf("   |_|  \\___/ \\____|");
    gotoxy(48, 7);
    set_color(COLOR_AI);
    printf(" \\___/|_|       \\_/\\_/_/   \\_\\_| \\_\\");
    
    gotoxy(35, 9);
    set_color(14);
    printf("+================================+");
    gotoxy(35, 10);
    printf("|   Ÿ�̹� �� �ٴٸ��� ����      |");
    gotoxy(35, 11);
    printf("+================================+");
    set_color(COLOR_RESET);
    
    gotoxy(23, 13);
    printf("========================================================");
    
    gotoxy(30, 15);
    printf("> 3�� �ο��� 2�� ���� �̱� ���� ����!");
    
    gotoxy(30, 17);
    printf("> Ÿ�̹� �ٰ� �߾ӿ� �� �� �����̽��� ��������!");
    
    gotoxy(30, 18);
    printf("> AI�� ����մϴ�!");
    
    gotoxy(35, 21);
    set_color(COLOR_PERFECT);
    printf("* PERFECT");
    set_color(COLOR_RESET);
    printf(" - 3ĭ �̵�");
    
    gotoxy(35, 22);
    set_color(COLOR_GOOD);
    printf("O GOOD");
    set_color(COLOR_RESET);
    printf("    - 2ĭ �̵�");
    
    gotoxy(35, 23);
    set_color(COLOR_BAD);
    printf("^ BAD");
    set_color(COLOR_RESET);
    printf("     - 1ĭ �̵�");
    
    gotoxy(35, 24);
    set_color(8);
    printf("X MISS");
    set_color(COLOR_RESET);
    printf("    - �̵� ����");
    
    gotoxy(23, 27);
    printf("========================================================");
    
    gotoxy(30, 30);
    set_color(14);
    printf("�ƹ� Ű�� ���� ������ �����ϼ���...");
    set_color(COLOR_RESET);
    
    getch();
}

void set_color(int color)
{
    SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), color);
}

void gotoxy(int x, int y)
{
    COORD Pos = {x - 1, y - 1};
    SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), Pos);
}

void clear_line(int y)
{
    int i;
    gotoxy(1, y);
    for(i=0; i<100; i++)
        printf(" ");
}

void draw_border()
{
    int i;
    
    // ��� �׵θ�
    gotoxy(13, 3);
    set_color(15);
    printf("+");
    for(i=0; i<74; i++)
        printf("=");
    printf("+");
    
    // �ϴ� �׵θ� (���ھ�� ��)
    gotoxy(13, 10);
    printf("+");
    for(i=0; i<74; i++)
        printf("=");
    printf("+");
    
    set_color(COLOR_RESET);
}

void display_game_field(int rope_pos)
{
    int i;
    int display_start = 20;  // ȭ�� ���� ��ġ
    
    // �ٴٸ��� �ʵ� ���
    gotoxy(display_start, 16);
    set_color(8);
    printf("+");
    for(i=0; i<60; i++)
        printf("-");
    printf("+");
    
    gotoxy(display_start, 17);
    printf("|");
    
    // �� ǥ�� (rope_pos�� 10~30 ����, �߾��� 20)
    for(i=0; i<60; i++)
    {
        if(i == 30)  // �߾Ӽ�
        {
            set_color(15);
            printf("|");
        }
        else if(i == rope_pos + 10)  // ���� ��ġ (rope_pos 10~30�� ȭ�� 20~50���� ����)
        {
            set_color(14);
            printf("O");
        }
        else if(i >= 0 && i <= 10)  // Player ����
        {
            set_color(COLOR_PLAYER);
            printf("~");
        }
        else if(i >= 50 && i <= 60)  // AI ����
        {
            set_color(COLOR_AI);
            printf("~");
        }
        else
        {
            set_color(8);
            printf("=");
        }
    }
    
    set_color(8);
    printf("|");
    
    gotoxy(display_start, 18);
    printf("+");
    for(i=0; i<60; i++)
        printf("-");
    printf("+");
    
    // �¸� ���� ǥ��
    gotoxy(display_start + 5, 19);
    set_color(COLOR_PLAYER);
    printf("WIN");
    
    gotoxy(display_start + 52, 19);
    set_color(COLOR_AI);
    printf("WIN");
    
    set_color(COLOR_RESET);
}

void display_score_board(int s_w[], int s_l[], int rope_pos)
{
    // Player ����
    gotoxy(20, 5);
    set_color(COLOR_PLAYER);
    printf("+----------------+");
    gotoxy(20, 6);
    printf("|    PLAYER      |");
    gotoxy(20, 7);
    set_color(COLOR_RESET);
    printf("|  %d WIN  %d LOSE |", s_w[0], s_l[0]);
    gotoxy(20, 8);
    set_color(COLOR_PLAYER);
    printf("+----------------+");
    
    // �߾� VS ǥ��
    gotoxy(48, 5);
    set_color(15);
    printf("+====+");
    gotoxy(48, 6);
    printf("| VS |");
    gotoxy(48, 7);
    printf("+====+");
    
    // AI ����
    gotoxy(65, 5);
    set_color(COLOR_AI);
    printf("+----------------+");
    gotoxy(65, 6);
    printf("|      AI        |");
    gotoxy(65, 7);
    set_color(COLOR_RESET);
    printf("|  %d WIN  %d LOSE |", s_w[1], s_l[1]);
    gotoxy(65, 8);
    set_color(COLOR_AI);
    printf("+----------------+");
    
    set_color(COLOR_RESET);
    
    // ���� �ʵ� ǥ��
    display_game_field(rope_pos);
}

void make_decision(int r_s, int s_w[], int s_l[])
{
    int win;
    
    if (r_s <= WIN_POSITION_LEFT)
    {
        win = 1;
        s_w[0] += 1;
        s_l[1] += 1;
    }
    else if (r_s >= WIN_POSITION_RIGHT)
    {
        win = 2;
        s_w[1] += 1;
        s_l[0] += 1;
    }
    else
        win = 0;
    
    gotoxy(40, 28);
    printf("                                        ");
    gotoxy(35, 28);
    
    if (win == 1)
    {
        set_color(COLOR_PLAYER);
        printf("***** PLAYER ���� �¸�! *****");
    }
    else if (win == 2)
    {
        set_color(COLOR_AI);
        printf("*****   AI ���� �¸�!   *****");
    }
    
    set_color(COLOR_RESET);
    
    gotoxy(45, 30);
    printf("�ƹ� Ű�� ��������...");
    getch();
}

void display_winner(int winner)
{
    int i;
    
    gotoxy(25, 10);
    set_color(14);
    printf("+============================================+");
    for(i=11; i<=17; i++)
    {
        gotoxy(25, i);
        printf("|                                            |");
    }
    gotoxy(25, 18);
    printf("+============================================+");
    
    if(winner == 1)
    {
        gotoxy(38, 13);
        set_color(COLOR_PLAYER);
        printf("********************");
        gotoxy(38, 14);
        printf("   PLAYER �¸�!     ");
        gotoxy(38, 15);
        printf("********************");
    }
    else
    {
        gotoxy(38, 13);
        set_color(COLOR_AI);
        printf("********************");
        gotoxy(38, 14);
        printf("     AI �¸�!       ");
        gotoxy(38, 15);
        printf("********************");
    }
    
    set_color(COLOR_RESET);
    gotoxy(35, 20);
    if(winner == 1)
        printf("�����մϴ�!");
    else
        printf("������ �̱� �� �־��!");
}

void init_timing_bar(TimingBar *bar)
{
    bar->position = 0;
    bar->direction = 1;
    // �ӵ��� 1~3 ���̿��� �������� ���� (1�� ���� ����)
    bar->speed = 1 + (rand() % 3);
}

void display_timing_bar(TimingBar *bar, int center)
{
    int i;
    int start_x = 30;
    int y_pos = 23;
    
    // Ÿ�̹� �� ��
    gotoxy(start_x - 10, y_pos);
    set_color(15);
    printf("TIMING:");
    
    // Ÿ�̹� �� ���
    gotoxy(start_x, y_pos);
    printf("[");
    
    for(i=0; i<TIMING_BAR_LENGTH; i++)
    {
        int dist = abs(i - center);
        
        if(dist <= PERFECT_ZONE)
        {
            set_color(COLOR_PERFECT);
            printf("#");
        }
        else if(dist <= GOOD_ZONE)
        {
            set_color(COLOR_GOOD);
            printf("#");
        }
        else if(dist <= BAD_ZONE)
        {
            set_color(COLOR_BAD);
            printf("=");
        }
        else
        {
            set_color(COLOR_NORMAL);
            printf(".");
        }
    }
    set_color(COLOR_RESET);
    printf("]");
    
    // �̵��ϴ� Ŀ��
    gotoxy(start_x + 1 + bar->position, y_pos);
    set_color(15);
    printf("V");
    set_color(COLOR_RESET);
}

int check_timing(TimingBar *bar, int center)
{
    int distance = abs(bar->position - center);
    
    if(distance <= PERFECT_ZONE)
        return 3; // PERFECT
    else if(distance <= GOOD_ZONE)
        return 2; // GOOD
    else if(distance <= BAD_ZONE)
        return 1; // BAD
    else
        return 0; // MISS
}

void display_timing_result(int timing_score, int is_player)
{
    gotoxy(35, 25);
    printf("                              ");
    gotoxy(37, 25);
    
    if(is_player)
    {
        switch(timing_score)
        {
            case 3:
                set_color(COLOR_PERFECT);
                printf("* PERFECT! * (+3)");
                break;
            case 2:
                set_color(COLOR_GOOD);
                printf("O GOOD! O (+2)");
                break;
            case 1:
                set_color(COLOR_BAD);
                printf("^ BAD ^ (+1)");
                break;
            default:
                set_color(8);
                printf("X MISS X (0)");
                break;
        }
    }
    else  // AI ��
    {
        switch(timing_score)
        {
            case 3:
                set_color(COLOR_PERFECT);
                printf("AI: PERFECT! (+3)");
                break;
            case 2:
                set_color(COLOR_GOOD);
                printf("AI: GOOD! (+2)");
                break;
            case 1:
                set_color(COLOR_BAD);
                printf("AI: BAD (+1)");
                break;
            default:
                set_color(8);
                printf("AI: MISS (0)");
                break;
        }
    }
    set_color(COLOR_RESET);
}

int ai_play()
{
    // AI�� �Ƿ� (�������� ����)
    int random = rand() % 100;
    
    if(random < 25)  // 25% PERFECT
        return 3;
    else if(random < 60)  // 35% GOOD
        return 2;
    else if(random < 90)  // 30% BAD
        return 1;
    else  // 10% MISS
        return 0;
}

void game_control(int *r_s, int turn_count)
{
    TimingBar bar;
    int current_turn = turn_count % 2;  // 0: Player, 1: AI
    int timing_score, power;
    int i, center = TIMING_BAR_LENGTH / 2;
    int round_trip_count = 0;  // �պ� Ƚ�� ī��Ʈ
    int max_round_trips = 3;    // �ִ� 3�� �պ�
    
    init_timing_bar(&bar);
    
    if(current_turn == 0)  // Player ��
    {
        // ���� �� ǥ��
        clear_line(21);
        gotoxy(28, 21);
        set_color(COLOR_PLAYER);
        printf(">>> YOUR TURN - [�����̽�]�� �����ּ���! <<<");
        set_color(COLOR_RESET);
        
        // �ӵ� ǥ��
        gotoxy(38, 22);
        set_color(8);
        printf("(Speed: ");
        if(bar.speed == 1)
        {
            set_color(COLOR_GOOD);
            printf("SLOW");
        }
        else if(bar.speed == 2)
        {
            set_color(14);
            printf("NORMAL");
        }
        else
        {
            set_color(COLOR_BAD);
            printf("FAST");
        }
        set_color(8);
        printf(")");
        set_color(COLOR_RESET);
        
        // Ÿ�̹� �� �ִϸ��̼�
        timing_score = 0;  // �ʱⰪ�� MISS
        while(round_trip_count < max_round_trips)
        {
            display_timing_bar(&bar, center);
            
            if(_kbhit())
            {
                char key = _getch();
                if(key == ' ')
                {
                    timing_score = check_timing(&bar, center);
                    display_timing_result(timing_score, 1);
                    Sleep(1000);
                    break;
                }
            }
            
            bar.position += bar.direction * bar.speed;
            
            // ���� ��ȯ �� �պ� Ƚ�� ����
            if(bar.position >= TIMING_BAR_LENGTH - 1)
            {
                bar.position = TIMING_BAR_LENGTH - 1;
                bar.direction = -1;
                round_trip_count++;
            }
            else if(bar.position <= 0)
            {
                bar.position = 0;
                bar.direction = 1;
                round_trip_count++;
            }
            
            Sleep(30);
        }
        
        // 3�� �պ��ߴµ��� ������ �ʾ����� MISS
        if(round_trip_count >= max_round_trips)
        {
            timing_score = 0;
            display_timing_result(timing_score, 1);
            Sleep(1000);
        }
    }
    else  // AI ��
    {
        clear_line(21);
        gotoxy(35, 21);
        set_color(COLOR_AI);
        printf(">>> AI TURN <<<");
        set_color(COLOR_RESET);
        
        Sleep(500);  // AI �����ϴ� �ð�
        
        timing_score = ai_play();
        display_timing_result(timing_score, 0);
        Sleep(1500);
    }
    
    // Ÿ�ֿ̹� ���� �� ���
    if(timing_score == 3)
        power = 3;
    else if(timing_score == 2)
        power = 2;
    else if(timing_score == 1)
        power = 1;
    else
        power = 0;
    
    // �� �̵� (Player�� ��������, AI�� ����������)
    int direction = (current_turn == 0) ? -power : power;
    
    if(direction < 0)
    {
        for(i = *r_s; i > (*r_s + direction); i--)
        {
            display_game_field(i);
            Sleep(100);
        }
        *r_s = i + 1;
    }
    else if(direction > 0)
    {
        for(i = *r_s; i < (*r_s + direction); i++)
        {
            display_game_field(i);
            Sleep(100);
        }
        *r_s = i - 1;
    }
    
    // Ÿ�̹� �� UI �����
    clear_line(21);
    clear_line(22);
    clear_line(23);
    clear_line(25);
}
