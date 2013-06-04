
#include "waos.h"


waos_context *context_current = NULL;	// 현재 문맥
extern waos_task *task_current;			// 현재 작업


#define CONTEXT_BACKUP()						\
asm volatile (									\
	"push	r0							\n\t"	\
	"in		r0, __SREG__				\n\t"	\
	"cli								\n\t"	\
	"push	r0							\n\t"	\
	"push	r1							\n\t"	\
	"clr	r1							\n\t"	\
	"push	r2							\n\t"	\
	"push	r3							\n\t"	\
	"push	r4							\n\t"	\
	"push	r5							\n\t"	\
	"push	r6							\n\t"	\
	"push	r7							\n\t"	\
	"push	r8							\n\t"	\
	"push	r9							\n\t"	\
	"push	r10							\n\t"	\
	"push	r11							\n\t"	\
	"push	r12							\n\t"	\
	"push	r13							\n\t"	\
	"push	r14							\n\t"	\
	"push	r15							\n\t"	\
	"push	r16							\n\t"	\
	"push	r17							\n\t"	\
	"push	r18							\n\t"	\
	"push	r19							\n\t"	\
	"push	r20							\n\t"	\
	"push	r21							\n\t"	\
	"push	r22							\n\t"	\
	"push	r23							\n\t"	\
	"push	r24							\n\t"	\
	"push	r25							\n\t"	\
	"push	r26							\n\t"	\
	"push	r27							\n\t"	\
	"push	r28							\n\t"	\
	"push	r29							\n\t"	\
	"push	r30							\n\t"	\
	"push	r31							\n\t"	\
	"lds	r26, context_current		\n\t"	\
	"lds	r27, context_current + 1	\n\t"	\
	"in		r0, __SP_L__				\n\t"	\
	"st		x+, r0						\n\t"	\
	"in		r0, __SP_H__				\n\t"	\
	"st		x+, r0						\n\t"	\
);

#define CONTEXT_RESTORE()						\
asm volatile (									\
	"lds	r26, context_current		\n\t"	\
	"lds	r27, context_current + 1	\n\t"	\
	"ld		r28, x+						\n\t"	\
	"out	__SP_L__, r28				\n\t"	\
	"ld		r29, x+						\n\t"	\
	"out	__SP_H__, r29				\n\t"	\
	"pop	r31							\n\t"	\
	"pop	r30							\n\t"	\
	"pop	r29							\n\t"	\
	"pop	r28							\n\t"	\
	"pop	r27							\n\t"	\
	"pop	r26							\n\t"	\
	"pop	r25							\n\t"	\
	"pop	r24							\n\t"	\
	"pop	r23							\n\t"	\
	"pop	r22							\n\t"	\
	"pop	r21							\n\t"	\
	"pop	r20							\n\t"	\
	"pop	r19							\n\t"	\
	"pop	r18							\n\t"	\
	"pop	r17							\n\t"	\
	"pop	r16							\n\t"	\
	"pop	r15							\n\t"	\
	"pop	r14							\n\t"	\
	"pop	r13							\n\t"	\
	"pop	r12							\n\t"	\
	"pop	r11							\n\t"	\
	"pop	r10							\n\t"	\
	"pop	r9							\n\t"	\
	"pop	r8							\n\t"	\
	"pop	r7							\n\t"	\
	"pop	r6							\n\t"	\
	"pop	r5							\n\t"	\
	"pop	r4							\n\t"	\
	"pop	r3							\n\t"	\
	"pop	r2							\n\t"	\
	"pop	r1							\n\t"	\
	"pop	r0							\n\t"	\
	"out	__SREG__, r0				\n\t"	\
	"pop	r0							\n\t"	\
);

#define CONTEXT_UPDATE()	\
	context_current = &task_current->context


void context_init()
{
	CONTEXT_UPDATE();

	// 타이머 0
	TCCR0 = (1 << WGM01) | (1 << CS02) | (1 << CS01);
	OCR0 = 62;
	TCNT0 = 0x00;
	ASSR = 0x00;	
	TIMSK = 1 << OCIE0;

/*
	// 타이머 1을 사용하기 전에 인터럽트 처리 루틴을 변경하거나
	// 알맞은 전처리기 작업을 사용해야 합니다.
	TCCR1A = 0;
	TCCR1C = 0;
	OCR1A = 62;
	TCNT1 = 0;
	TCCR1B = (1 << WGM12) | (1 << CS12);
	TIMSK = 1 << OCIE1A;
*/
}

//void SIG_OUTPUT_COMPARE1A(void)
void SIG_OUTPUT_COMPARE0(void)
{	
	UTIL_UNLOCK();
	CONTEXT_BACKUP();
	UTIL_LOCK();

	task_current->status = WAOS_TASK_STATUS_BLOCKED;

	timer_tick_event();

	task_schedule();

	CONTEXT_UPDATE();

	CONTEXT_RESTORE();
	
	asm volatile ("reti");
}

void context_block(void)
{
	CONTEXT_BACKUP();

	task_current->status = WAOS_TASK_STATUS_BLOCKED;

	task_schedule();

	CONTEXT_UPDATE();

	CONTEXT_RESTORE();

	asm volatile ("ret");
}

void context_suspend(void)
{
	CONTEXT_BACKUP();

	task_current->status = WAOS_TASK_STATUS_SUSPENDED;

	task_schedule();

	CONTEXT_UPDATE();

	CONTEXT_RESTORE();

	asm volatile ("ret");
}

void context_none(void)
{
	CONTEXT_BACKUP();

	task_schedule();

	CONTEXT_UPDATE();

	CONTEXT_RESTORE();

	asm volatile ("ret");
}
