
#include "waos.h"
#include "../user/user.h"


int main();
void waos_init();
void waos_boot();
void waos_null();
void waos_kernel();


int main()
{
	// task_kernel_work()에서 LOCK 후 UNLOCK 됩니다.
	UTIL_LOCK();

	waos_init();
	waos_boot();
	waos_kernel();

	return 0;
}

void waos_init()
{

#ifndef WAOS_FLAG_NO_ERROR
	error_init();
#endif

	memory_init();
	serial_init();
	timer_init();
	port_init();

	// 순서
	task_init();
	context_init();
}

void waos_boot()
{
//	printf("\nWaOS "WAOS_VERSION"\n");

//	printf("Starting 'swap' task...\n");
	task_create_locked(waos_null);

//	printf("Starting 'user' task...\n");
	task_create_locked(User);
}



// TODO: timer_sleep() 사용 시 데드락에 걸리는 경우를
// 회피하기 위해 사용 (timer 인터럽트 발생 안함)
// 이 태스크에서는 항상 unlock됨.
//
// TODO: 데드락 발생 시 실행 중인 컨텍스트 정보를 출력해서 디버그 후,
// 현재 task 목록을 console로 출력하는 유저모드 또는 커널모드 기능 필요
//
void waos_null()
{
	UTIL_UNLOCK();

	while(1)
	{
		context_block();
	}
}

void waos_kernel()
{
	while (1)
	{
		task_kernel_work();

		context_suspend();
	}
}
