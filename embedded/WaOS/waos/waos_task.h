
#ifndef _WAOS_TASK
#define _WAOS_TASK


// �۾� ����ü
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
�۾� �����ڸ� �ʱ�ȭ�մϴ�.

	waos_task *task_create(void (*entry_point)(void));
entry_point���� �����ϰ� �ϴ� ���ο� �۾��� �����մϴ�.
�۾��� 0�� �۾�(Ŀ��)�� ���� ��� ������ ���°� �� ���ĺ��� ����˴ϴ�.
	
	void task_schedule();
�۾� �����ٸ��� �����մϴ�.

	void task_kernel();
0�� �۾����� �۾� ���� �۾��� �����մϴ�.

	static void task_begin();
���ο� �۾��� �����ϰ� �۾��� ����Ǹ� �����մϴ�.
���ο��� entry_point�� ���� ȣ���մϴ�.

	static void task_remove();
���� �۾��� �����մϴ�. �۾��� WAOS_TASK_STATUS_DIRTY ���°� �ǰ�
Ŀ�ο� ���� WAOS_TASK_STATUS_CLEAN ���°� �˴ϴ�.

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
