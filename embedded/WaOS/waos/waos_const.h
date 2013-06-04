
#ifndef _WAOS_CONST
#define _WAOS_CONST


/*
	WAOS_MEMORY_STACKSIZE
작업에 기본 할당되는 스택의 크기를 바이트 단위로 지정합니다.
0번 작업(커널)에는 해당되지 않습니다.
*/
#define WAOS_MEMORY_STACKSIZE	128


/*
	WAOS_TASK_MAX
WaOS에서 사용 가능한 최대 작업 수를 지정합니다.
작업의 상태와 관련되는 대부분의 정보는 정적 할당되고
작업이 사용하는 스택은 동적으로 할당됩니다.
*/
#define WAOS_TASK_MAX 8

/*
	WAOS_TASK_STATUS
작업 스케줄링을 위해 사용되는 작업의 구체적인 상태를 나타납니다.
스케줄러 뿐만 아니라 작업의 생명을 관리하는 커널 요소에서도 사용합니다.
0x00은 값으로 가질 수 없습니다.

WAOS_TASK_STATUS_NEW
새로운 작업을 준비하는 중입니다. 작업 정보 생성은 완료되었지만
작업이 사용하기 위한 자원이 아직 할당되지 않았습니다.
스케줄러는 곧 0번 작업(커널)이 자원을 할당하도록 합니다.

WAOS_TASK_STATUS_RUNNING
작업이 정상적으로 실행 중인 상태입니다.

WAOS_TASK_STATUS_BLOCKED
작업이 일시적으로 중지된 상태입니다.
스케줄러는 0번 작업을 제외한 모든 작업을 라운드 로빈 스케줄링합니다.

WAOS_TASK_STATUS_SUSPENDED
작업이 스케줄링을 포기한 상태이며, 언제 다시 실행될 지 알 수 없습니다.
보통을 입출력이 필요하거나 타이머를 대기하기 위해 이 상태로 전환됩니다.

WAOS_TASK_STATUS_CLEAN
커널 부팅 과정에서 초기화되었거나, 실행 중인 작업이 완전히 종료되었습니다.
스택과 같은 작업 자원은 모두 해제되었습니다.

WAOS_TASK_STATUS_DIRTY
작업을 종료하고 삭제하는 중입니다.
스케줄러는 0번 작업(커널)이 자원을 해제하도록 우선 스케줄링합니다.

*/

#define	WAOS_TASK_STATUS_NEW		0x10
#define	WAOS_TASK_STATUS_RUNNING	0x20
#define WAOS_TASK_STATUS_BLOCKED	0x21
#define WAOS_TASK_STATUS_SUSPENDED	0x22
#define WAOS_TASK_STATUS_CLEAN		0x40
#define WAOS_TASK_STATUS_DIRTY		0x41

/*
	WAOS_SERIAL_UBRR
시스템에서 기본 제공하는 직렬 통신을 초기화하기 위한 상수입니다.
*/
#define WAOS_SERIAL_UBRR 25

/*
	WAOS_ERROR
오류 정보를 나타냅니다.

WAOS_ERROR_GOOD
최근에 발생한 에러가 없습니다.
작업이 초기화되면 기본으로 설정되는 오류이며, 문제가 없는 상태입니다.

WAOS_ERROR_TASK_LIMIT
작업을 생성하려고 하지만, 커널에 설정된 최대 작업 수 제한 때문에 그럴 수 없습니다.

WAOS_ERROR_MEMORY_LIMIT
작업을 생성하려고 하지만, 커널에서 메모리를 할당할 수 없습니다.

*/
#define WAOS_ERROR_GOOD				0x00
#define WAOS_ERROR_TASK_LIMIT 		0x10
#define WAOS_ERROR_MEMORY_LIMIT		0x11


#endif
