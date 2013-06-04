
#ifndef _WAOS_MEMORY
#define _WAOS_MEMORY


// 스택 형식
typedef struct
{
	void *addr;
} waos_memory_stack;


void memory_init(void);

waos_memory_stack *memory_stack_alloc();
void memory_stack_zero(waos_memory_stack *);
void* memory_stack_base(waos_memory_stack *); 
void memory_stack_free(waos_memory_stack *);

#endif
