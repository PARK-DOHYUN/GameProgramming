
# 📘 게임 프로그래밍 (C 언어)

**과목:** 게임프로그래밍  
**언어:** C  
**학교:** 경성대학교  

---

## 🃏 카드 프로그래밍

트럼프 카드는 ♠ ◆ ♥ ♣ 4종류, 각 13장으로 총 52장이 사용됩니다.  
이를 구조체를 이용해 구현합니다.

### 카드 구조체
```c
typedef struct {
    int order;     // 카드 우선순위 (♠=0, ◆=1, ♥=2, ♣=3)
    char shape[4]; // 카드 모양 (♠◆♥♣)
    char number[3];// 카드 숫자 (A,2~10,J,Q,K)
} Card;
카드 초기화
c
void init_cards(Card cards[]) {
    char *shapes[] = {"♠", "◆", "♥", "♣"};
    for (int i = 0; i < 4; i++) {
        for (int j = 1; j <= 13; j++) {
            int idx = i * 13 + (j - 1);
            cards[idx].order = i;
            strcpy(cards[idx].shape, shapes[i]);
            switch (j) {
                case 1:  strcpy(cards[idx].number, "A"); break;
                case 11: strcpy(cards[idx].number, "J"); break;
                case 12: strcpy(cards[idx].number, "Q"); break;
                case 13: strcpy(cards[idx].number, "K"); break;
                default: sprintf(cards[idx].number, "%d", j); break;
            }
        }
    }
}
카드 출력
c
void print_cards(Card cards[], int size) {
    for (int i = 0; i < size; i++) {
        printf("%s%-2s ", cards[i].shape, cards[i].number);
        if ((i + 1) % 13 == 0) printf("\n");
    }
}
카드 섞기
c
void shuffle(Card cards[], int size) {
    srand(time(NULL));
    for (int i = 0; i < size; i++) {
        int rnd;
        do {
            rnd = rand() % size;
        } while (rnd == i); // 같은 인덱스 방지
        Card temp = cards[i];
        cards[i] = cards[rnd];
        cards[rnd] = temp;
    }
}
```

🎵 음계와 피아노 건반
기준음: 라(A4) = 440Hz

12평균율 사용 → 옥타브를 12개 반음으로 나눔

calc_frequency 함수로 각 음 주파수 계산

Windows 환경에서는 Beep 함수로 소리 출력 가능

주파수 계산 함수
```c
#include <math.h>

int calc_frequency(int octave, int index) {
    double base = 440.0; // A4 = 440Hz
    double ratio = pow(2.0, 1.0 / 12.0); // 12평균율
    return (int)(base * pow(ratio, index + (octave - 4) * 12));
}
```
도레미파솔라시도 출력
```c
#include <windows.h>
#include <conio.h>

int main() {
    int index[] = {0, 2, 4, 5, 7, 9, 11, 12}; // 도레미파솔라시도
    for (int i = 0; i < 8; i++) {
        int freq = calc_frequency(4, index[i]);
        Beep(freq, 500); // 0.5초 출력
    }
    return 0;
}
```
숫자 키 입력에 따라 음 출력
```c
#include <conio.h>
#include <windows.h>

int main() {
    int index[] = {0, 2, 4, 5, 7, 9, 11, 12}; 
    char ch;
    while ((ch = getch()) != 27) { // Esc 누르면 종료
        int key = ch - '1'; // '1' 입력 시 index[0]
        if (key >= 0 && key < 8) {
            int freq = calc_frequency(4, index[key]);
            Beep(freq, 400);
        }
    }
    return 0;
}
```
📂 자료 구조
연결 리스트 (Linked List)
연결 리스트는 노드들이 포인터로 연결된 자료 구조입니다.
게임 replay 기능 구현에 활용할 수 있습니다.

```c
typedef struct Node {
    char data;
    struct Node *next;
} Node;

Node* insert(Node *head, char data) {
    Node *newNode = (Node*)malloc(sizeof(Node));
    newNode->data = data;
    newNode->next = head;
    return newNode;
}

void printList(Node *head) {
    Node *curr = head;
    while (curr != NULL) {
        printf("%c -> ", curr->data);
        curr = curr->next;
    }
    printf("NULL\n");
}
```
스택 (Stack)
스택은 LIFO(Last In, First Out) 구조입니다.
추가/삭제는 항상 top에서 일어납니다.

```c
typedef struct StackNode {
    int data;
    struct StackNode *next;
} StackNode;

StackNode* push(StackNode *top, int data) {
    StackNode *newNode = (StackNode*)malloc(sizeof(StackNode));
    newNode->data = data;
    newNode->next = top;
    return newNode;
}

StackNode* pop(StackNode *top) {
    if (top == NULL) return NULL;
    StackNode *temp = top;
    printf("Popped: %d\n", temp->data);
    top = top->next;
    free(temp);
    return top;
}
```
🔡 데이터 정렬
문자열을 정렬할 때 대소문자 차이를 제거하고 비교해야 합니다.

```c
void upper_to_lower(char *str) {
    for (int i = 0; str[i]; i++) {
        if ('A' <= str[i] && str[i] <= 'Z')
            str[i] += 32; // 소문자로 변환
    }
}

void sort_words(char arr[][50], int n) {
    char temp[50];
    for (int i = 0; i < n-1; i++) {
        for (int j = i+1; j < n; j++) {
            char a[50], b[50];
            strcpy(a, arr[i]);
            strcpy(b, arr[j]);
            upper_to_lower(a);
            upper_to_lower(b);
            if (strcmp(a, b) > 0) {
                strcpy(temp, arr[i]);
                strcpy(arr[i], arr[j]);
                strcpy(arr[j], temp);
            }
        }
    }
}
```
