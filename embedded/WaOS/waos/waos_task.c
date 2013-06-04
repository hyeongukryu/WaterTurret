
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


// TODO : �۾��� �켱 ������ ���Ͽ�, ������ kernel �۾����� �Ѿ�� �ʵ���
// kernel �� ���� priority�� �Ҵ��ϰų�, ���ο� �۾� (new, clean, dirty, remove)��
// ���� �ӽ� priority alloc. �ʿ�.

// TODO : task scheduler ���� ������ ���� �۾� (WAOS_TASK_STATUS_BLOCKED)��
// ã�� ���� ���, init �۾��� �������� ���ϸ� (null) kernel panic.

// TODO : null(init, main()) �۾����� context_block() �ݺ�,
// ���� ����(util_delay_ms() �迭), timer_sleep() �� ���� Ÿ�缺 �ִ�
// ������ �ϰų�, �׷��� ������ on-demand �� ó���ϰų�,
// ������ Ÿ�ӿ� ���� (preprocessor �Ǵ� hard coding ����� ����)
// -> ���� �Ǵ� ���, �Լ� ������, �Լ� �̸� ġȯ ���



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
				// ���� �Ҵ� ����
				// TODO: ���� ó�� �Ǵ� fatal error (Ŀ�� �д�)
			}

			char *stack_base = memory_stack_base(task->stack);

			stack_base -= 1;
			*stack_base = LOW_BIT(task_begin);

			stack_base -= 1;
			*stack_base = HIGH_BIT(task_begin);

			// ��������: SREG ���� ���� (TODO)
			stack_base -= 33;

			// stack pointer ��ġ ����
			stack_base -= 1;
			task->context.spl = LOW_BIT(stack_base);
			task->context.sph = HIGH_BIT(stack_base);

			task->status = WAOS_TASK_STATUS_BLOCKED;
		}
	}
}
