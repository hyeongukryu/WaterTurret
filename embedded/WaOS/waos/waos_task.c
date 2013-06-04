
#include "waos.h"


waos_task task_tasks[WAOS_TASK_MAX];

waos_task
	*task_current, *task_kernel,
	*begin, *end;


void task_init()
{
	for (unsigned char i = 0; i < WAOS_TASK_MAX; i++)
	{
		waos_task *task = &task_tasks[i];
		memset(task, 0x00, sizeof(waos_task));

		task->id = i;
		task->status = WAOS_TASK_STATUS_CLEAN;
#ifndef WAOS_FLAG_NO_ERROR
		task->last_error = WAOS_ERROR_GOOD;
#endif
		task->tick_sleep = 0;
	}
	
	task_kernel = &task_tasks[0];
	task_kernel->status = WAOS_TASK_STATUS_RUNNING;
	task_current = task_kernel;

	begin = &task_tasks[0];
	end = &task_tasks[WAOS_TASK_MAX];
}

void task_tick()
{
	for (waos_task *task = begin; task != end; task++)
	{
		if (task->status == WAOS_TASK_STATUS_SUSPENDED && task->tick_sleep > 0)
		{
			task->tick_sleep--;

			if (task->tick_sleep == 0)
			{
				task->status = WAOS_TASK_STATUS_BLOCKED;
			}
		}
	}
}

waos_task *task_create_locked(void (*entry_point)(void))
{
	waos_task *task = begin;
	for (; task != end; task++)
	{
		if (task->status == WAOS_TASK_STATUS_CLEAN)
		{
#ifndef WAOS_FLAG_NO_ERROR	
			task->last_error = WAOS_ERROR_GOOD;
#endif
			task->entry_point = entry_point;

			task->status = WAOS_TASK_STATUS_NEW;
			
			break;
		}
	}

	if (task == end)
	{
#ifndef WAOS_FLAG_NO_ERROR
		error_last_error_set(WAOS_ERROR_TASK_LIMIT);
#endif
	
		return NULL;
	}

	return task;
}

waos_task *task_create(void (*entry_point)(void))
{
	UTIL_LOCK();

	waos_task *task = task_create_locked(entry_point);

	UTIL_UNLOCK();

	return task;
}

void task_remove(waos_task *task)
{
	task->status = WAOS_TASK_STATUS_DIRTY;
}

void task_schedule()
{
	for (waos_task *task = begin + 1; task != end; task++)
	{
		if (task->status == WAOS_TASK_STATUS_NEW
			|| task->status == WAOS_TASK_STATUS_DIRTY)
		{
			task_kernel->status = WAOS_TASK_STATUS_BLOCKED;
			task_current = task_kernel;
			return;
		}
	}

	for (waos_task *task = task_current + 1; task != end; task++)
	{
		if (task->status == WAOS_TASK_STATUS_BLOCKED)
		{
			task->status = WAOS_TASK_STATUS_RUNNING;
			task_current = task;
			return;
		}
	}

	for (waos_task *task = begin + 1; task <= task_current; task++)
	{
		if (task->status == WAOS_TASK_STATUS_BLOCKED)
		{
			task->status = WAOS_TASK_STATUS_RUNNING;
			task_current = task;
			return;
		}
	}
	
	task_kernel->status = WAOS_TASK_STATUS_BLOCKED;
	task_current = task_kernel;
}


// TODO : 작업의 우선 순위를 정하여, 무조건 kernel 작업으로 넘어가지 않도록
// kernel 에 정적 priority를 할당하거나, 새로운 작업 (new, clean, dirty, remove)에
// 대한 임시 priority alloc. 필요.

// TODO : task scheduler 에서 적절한 다음 작업 (WAOS_TASK_STATUS_BLOCKED)를
// 찾지 못한 경우, init 작업을 실행하지 못하면 (null) kernel panic.

// TODO : null(init, main()) 작업에서 context_block() 반복,
// 무한 루프(util_delay_ms() 계열), timer_sleep() 중 가장 타당성 있는
// 동작을 하거나, 그러한 동작은 on-demand 로 처리하거나,
// 컴파일 타임에 결정 (preprocessor 또는 hard coding 방법을 통해)
// -> 변수 또는 상수, 함수 포인터, 함수 이름 치환 사용



void task_begin()
{
	UTIL_UNLOCK();

	task_current->entry_point();

	UTIL_LOCK();

	task_remove(task_current);

	context_none();
}



void task_kernel_work()
{
	UTIL_LOCK();

	waos_task
		*begin = &task_tasks[0],
		*end = &task_tasks[WAOS_TASK_MAX];

	for (waos_task *task = begin + 1; task != end; task++)
	{
		if (task->status == WAOS_TASK_STATUS_DIRTY)
		{
			memory_stack_free(task->stack);
			task->tick_sleep = 0;
			task->status = WAOS_TASK_STATUS_CLEAN;
		}

		if (task->status == WAOS_TASK_STATUS_NEW)
		{
			task->stack = memory_stack_alloc();

			if (task->stack == NULL)
			{
				// 스택 할당 실패
				// TODO: 오류 처리 또는 fatal error (커널 패닉)
			}

			char *stack_base = memory_stack_base(task->stack);

			stack_base -= 1;
			*stack_base = LOW_BIT(task_begin);

			stack_base -= 1;
			*stack_base = HIGH_BIT(task_begin);

			// 레지스터: SREG 상태 무시 (TODO)
			stack_base -= 33;

			// stack pointer 위치 정렬
			stack_base -= 1;
			task->context.spl = LOW_BIT(stack_base);
			task->context.sph = HIGH_BIT(stack_base);

			task->status = WAOS_TASK_STATUS_BLOCKED;
		}
	}
}
