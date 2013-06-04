
#include "waos.h"

extern waos_task *task_current;

#ifndef WAOS_FLAG_NO_ERROR

void error_init()
{
}

void error_last_error_set(char error)
{
	task_current->last_error = error; 
}

char error_last_error_get()
{
	return task_current->last_error;
}

#endif
