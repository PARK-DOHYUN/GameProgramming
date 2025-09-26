#include <stdio.h>
#include <string.h>
#include <conio.h>
#include <math.h>
void serial_number(long number);
void reverse_number(long number);
void rec_serial_number(long number);
void rec_reverse_number(long number);

int main(void)
{
    char str[20];
    long number;
    printf("문자열을 입력하세요: ");
    scanf("%s", str);
    char_serial_number(str);
    char_reverse_number(str);
    printf("정수를 입력하세요: ");
    scanf("%ld", &number);
    serial_number(number);
    reverse_number(number);
    rec_serial_number(number);
    rec_reverse_number(number);

    return 0;
}
// 문자형 정순으로
void char_serial_number(char *number)
{
    int i, length;
    length = strlen(number);
    for (i = 0; i < length; i++)
        printf("%c\n", number[i]);
    printf("\n");
}
// 문자형 역순으로
void char_reverse_number(char *number)
{
    int i, length;
    length = strlen(number);
    for (i = length - 1; i >= 0; i--)
        printf("%c\n", number[i]);
    printf("\n");
}

// 정수형 정순으로
void serial_number(long number)
{
    int num;
    int i, length = 0;
    length = (int)(log10(number) + 1); // 최대 자리수 계산
    for (i = length; i >= 1; i--)
    {
        num = number / (long)pow(10, i - 1);
        printf("%ld\n", num);
        number = number - num * (long)pow(10, i - 1);
    }
    printf("\n");
}
// 정수형 역순으로
void reverse_number(long number)
{
    while (number > 0)
    {
        printf("%ld\n", number % 10);
        number /= 10;
    }
}
// 재귀함수 정순으로
void rec_serial_number(long number)
{
    if (number > 0)
    {
        serial_number(number / 10);
        printf("%ld\n", number % 10);
    }
    else
        return;
}
// 재귀함수 역순으로
void rec_reverse_number(long number)
{
    printf("%ld\n", number % 10);
    if ((number / 10) > 0)
        reverse_number(number / 10);
    else
        return;
}