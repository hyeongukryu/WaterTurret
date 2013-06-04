
#ifndef _WAOS_CONTEXT
#define _WAOS_CONTEXT


// 문맥 구조체
typedef struct
{
	char spl;
	char sph;
} waos_context;


void context_init(void);

// 인터럽트
// void SIG_OUTPUT_COMPARE1A(void)	__attribute__ ((signal, naked));
void SIG_OUTPUT_COMPARE0(void)	__attribute__ ((signal, naked));

// 자발적인 CPU 반환을 위해 호출
void context_block(void)		__attribute__ ((naked));
void context_suspend(void)		__attribute__ ((naked));
void context_none(void)			__attribute__ ((naked));

#endif
