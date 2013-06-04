
#include "waos.h"


void memory_init()
{
}

void memory_stack_zero(waos_memory_stack *stack)
{
	memset(stack->addr, 0x00, WAOS_MEMORY_STACKSIZE);
}

void *memory_stack_base(waos_memory_stack *stack)
{
	return stack->addr + WAOS_MEMORY_STACKSIZE;
}

waos_memory_stack *memory_stack_alloc()
{
	waos_memory_stack *stack = malloc(sizeof(waos_memory_stack));
	memset(stack, 0x00, sizeof(waos_memory_stack));
	stack->addr = malloc(WAOS_MEMORY_STACKSIZE);
	
	if (!stack->addr)
	{
		return NULL;
	}
	
	memory_stack_zero(stack);
	return stack;
}

void memory_stack_free(waos_memory_stack *stack)
{
	memory_stack_zero(stack);
	free(stack->addr);
	free(stack);
}
