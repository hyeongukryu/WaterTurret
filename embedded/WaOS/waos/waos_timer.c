
#include "waos.h"

// unsigned int timer_tick;


void timer_init()
{
//	timer_tick = 0;
}

void timer_tick_event()
{
//	timer_tick++;
	task_tick();
}

/*
unsigned int timer_current_tick_get()
{
	return timer_tick;
}
*/

extern waos_task *task_current;

void timer_sleep(unsigned int tick)
{
	UTIL_LOCK();

	task_current->tick_sleep = tick;

	context_suspend();

	UTIL_UNLOCK();
}
