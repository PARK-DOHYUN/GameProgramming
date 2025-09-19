// push, pop - Node이용

#include <stdio.h>
#include <malloc.h>
#include <stdlib.h>

struct Node
{
    int data;
    struct Node *next;
};

struct Stack
{
    struct Node *top;
};

void push(struct Stack *stack, int data)
{
    if (stack->top == NULL) // 스택이 비어있을 때
    {
        struct Node *newNode = (struct Node *)malloc(sizeof(struct Node));
        newNode->data = data;
        newNode->next = NULL;
        stack->top = newNode;
    }
    else // 스택이 비어있지 않을 때
    {
        struct Node *newNode = (struct Node *)malloc(sizeof(struct Node));
        newNode->data = data;
        newNode->next = stack->top;
        stack->top = newNode;
    }
}

void pop(struct Stack *stack)
{
    if (stack->top == NULL) // 스택이 비어있을 때
    {
        printf("Stack is empty. Cannot pop.\n");
        return;
    }
    else // 스택이 비어있지 않을 때
    {
        struct Node *temp = stack->top;
        stack->top = stack->top->next;
        free(temp);
    }
}

void ReleaseStack(struct Stack *stack)
{
    while (stack->top != NULL)
    {
        pop(stack);
    }
}
void ShowStack(struct Stack *stack)
{
    struct Node *current = stack->top;
    while (current != NULL)
    {
        printf("%d\n", current->data);
        current = current->next;
    }
}

int main(void)
{
    struct Stack stack;
    stack.top = NULL;

    push(&stack, 10);
    push(&stack, 20);
    push(&stack, 30);

    ShowStack(&stack);

    pop(&stack);
    printf("After pop:\n");
    ShowStack(&stack);

    ReleaseStack(&stack);
    return 0;
}
