
#include "waos.h"
#include "../user/user.h"


int main();
void waos_init();
void waos_boot();
void waos_null();
void waos_kernel();


int main()
{
	// task_kernel_work()���� LOCK �� UNLOCK �˴ϴ�.
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

	// ����
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



// TODO: timer_sleep() ��� �� ������� �ɸ��� ��츦
// ȸ���ϱ� ���� ��� (timer ���ͷ�Ʈ �߻� ����)
// �� �½�ũ������ �׻� unlock��.
//
// TODO: ����� �߻� �� ���� ���� ���ؽ�Ʈ ������ ����ؼ� ����� ��,
// ���� task ����� console�� ����ϴ� ������� �Ǵ� Ŀ�θ�� ��� �ʿ�
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
