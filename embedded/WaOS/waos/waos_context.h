
#ifndef _WAOS_CONTEXT
#define _WAOS_CONTEXT


// ���� ����ü
typedef struct
{
	char spl;
	char sph;
} waos_context;


void context_init(void);

// ���ͷ�Ʈ
// void SIG_OUTPUT_COMPARE1A(void)	__attribute__ ((signal, naked));
void SIG_OUTPUT_COMPARE0(void)	__attribute__ ((signal, naked));

// �ڹ����� CPU ��ȯ�� ���� ȣ��
void context_block(void)		__attribute__ ((naked));
void context_suspend(void)		__attribute__ ((naked));
void context_none(void)			__attribute__ ((naked));

#endif
