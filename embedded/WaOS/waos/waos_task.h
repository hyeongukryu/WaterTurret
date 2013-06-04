
#ifndef _WAOS_TASK
#define _WAOS_TASK


// 작업 구조체
typedef struct
{
	unsigned char id;
	char status;
#ifndef WAOS_FLAG_NO_ERROR
	char last_error;
#endif
	waos_context context;
	waos_memory_stack *stack;
	unsigned int tick_sleep;
	void (*entry_point)(void);
} waos_task;


/*
	void task_init();
작업 관리자를 초기화합니다.

	waos_task *task_create(void (*entry_point)(void));
entry_point에서 시작하고나 하는 새로운 작업을 생성합니다.
작업은 0번 작업(커널)에 의해 사용 가능한 상태가 된 이후부터 실행됩니다.
	
	void task_schedule();
작업 스케줄링을 수행합니다.

	void task_kernel();
0번 작업에서 작업 관련 작업을 수행합니다.

	static void task_begin();
새로운 작업을 시작하고 작업이 종료되면 정리합니다.
내부에서 entry_point를 직접 호출합니다.

	static void task_remove();
현재 작업을 삭제합니다. 작업은 WAOS_TASK_STATUS_DIRTY 상태가 되고
커널에 의해 WAOS_TASK_STATUS_CLEAN 상태가 됩니다.

*/

void task_init();
waos_task *task_create(void (*entry_point)(void));
waos_task *task_create_locked(void (*entry_point)(void));
void task_schedule();

void task_kernel_work();
void task_tick();

void task_begin();
void task_remove();

#endif
