
#ifndef _WAOS_CONST
#define _WAOS_CONST


/*
	WAOS_MEMORY_STACKSIZE
�۾��� �⺻ �Ҵ�Ǵ� ������ ũ�⸦ ����Ʈ ������ �����մϴ�.
0�� �۾�(Ŀ��)���� �ش���� �ʽ��ϴ�.
*/
#define WAOS_MEMORY_STACKSIZE	128


/*
	WAOS_TASK_MAX
WaOS���� ��� ������ �ִ� �۾� ���� �����մϴ�.
�۾��� ���¿� ���õǴ� ��κ��� ������ ���� �Ҵ�ǰ�
�۾��� ����ϴ� ������ �������� �Ҵ�˴ϴ�.
*/
#define WAOS_TASK_MAX 8

/*
	WAOS_TASK_STATUS
�۾� �����ٸ��� ���� ���Ǵ� �۾��� ��ü���� ���¸� ��Ÿ���ϴ�.
�����ٷ� �Ӹ� �ƴ϶� �۾��� ������ �����ϴ� Ŀ�� ��ҿ����� ����մϴ�.
0x00�� ������ ���� �� �����ϴ�.

WAOS_TASK_STATUS_NEW
���ο� �۾��� �غ��ϴ� ���Դϴ�. �۾� ���� ������ �Ϸ�Ǿ�����
�۾��� ����ϱ� ���� �ڿ��� ���� �Ҵ���� �ʾҽ��ϴ�.
�����ٷ��� �� 0�� �۾�(Ŀ��)�� �ڿ��� �Ҵ��ϵ��� �մϴ�.

WAOS_TASK_STATUS_RUNNING
�۾��� ���������� ���� ���� �����Դϴ�.

WAOS_TASK_STATUS_BLOCKED
�۾��� �Ͻ������� ������ �����Դϴ�.
�����ٷ��� 0�� �۾��� ������ ��� �۾��� ���� �κ� �����ٸ��մϴ�.

WAOS_TASK_STATUS_SUSPENDED
�۾��� �����ٸ��� ������ �����̸�, ���� �ٽ� ����� �� �� �� �����ϴ�.
������ ������� �ʿ��ϰų� Ÿ�̸Ӹ� ����ϱ� ���� �� ���·� ��ȯ�˴ϴ�.

WAOS_TASK_STATUS_CLEAN
Ŀ�� ���� �������� �ʱ�ȭ�Ǿ��ų�, ���� ���� �۾��� ������ ����Ǿ����ϴ�.
���ð� ���� �۾� �ڿ��� ��� �����Ǿ����ϴ�.

WAOS_TASK_STATUS_DIRTY
�۾��� �����ϰ� �����ϴ� ���Դϴ�.
�����ٷ��� 0�� �۾�(Ŀ��)�� �ڿ��� �����ϵ��� �켱 �����ٸ��մϴ�.

*/

#define	WAOS_TASK_STATUS_NEW		0x10
#define	WAOS_TASK_STATUS_RUNNING	0x20
#define WAOS_TASK_STATUS_BLOCKED	0x21
#define WAOS_TASK_STATUS_SUSPENDED	0x22
#define WAOS_TASK_STATUS_CLEAN		0x40
#define WAOS_TASK_STATUS_DIRTY		0x41

/*
	WAOS_SERIAL_UBRR
�ý��ۿ��� �⺻ �����ϴ� ���� ����� �ʱ�ȭ�ϱ� ���� ����Դϴ�.
*/
#define WAOS_SERIAL_UBRR 25

/*
	WAOS_ERROR
���� ������ ��Ÿ���ϴ�.

WAOS_ERROR_GOOD
�ֱٿ� �߻��� ������ �����ϴ�.
�۾��� �ʱ�ȭ�Ǹ� �⺻���� �����Ǵ� �����̸�, ������ ���� �����Դϴ�.

WAOS_ERROR_TASK_LIMIT
�۾��� �����Ϸ��� ������, Ŀ�ο� ������ �ִ� �۾� �� ���� ������ �׷� �� �����ϴ�.

WAOS_ERROR_MEMORY_LIMIT
�۾��� �����Ϸ��� ������, Ŀ�ο��� �޸𸮸� �Ҵ��� �� �����ϴ�.

*/
#define WAOS_ERROR_GOOD				0x00
#define WAOS_ERROR_TASK_LIMIT 		0x10
#define WAOS_ERROR_MEMORY_LIMIT		0x11


#endif
