
#include "user.h"

#define PAN_PORT	PORTF
#define PAN_DDR		DDRF

#define PAN_ENABLE()		(PAN_PORT = PAN_PORT|0x80)
#define PAN_DISABLE()		(PAN_PORT = PAN_PORT&0x7f)
#define PAN_STEP_M0()		(PAN_PORT = (PAN_PORT&0xcf)|0x00)
#define PAN_STEP_M1()		(PAN_PORT = (PAN_PORT&0xcf)|0x10)
#define PAN_STEP_M2()		(PAN_PORT = (PAN_PORT&0xcf)|0x20)
#define PAN_STEP_M3()		(PAN_PORT = (PAN_PORT&0xcf)|0x30)
#define PAN_LEFT_CLOCK()	(PAN_PORT = PAN_PORT^0x01)
#define PAN_RIGHT_CLOCK()	(PAN_PORT = PAN_PORT^0x04)
#define PAN_LEFT_CW()		(PAN_PORT = PAN_PORT&0xfd)
#define PAN_LEFT_CCW()		(PAN_PORT = PAN_PORT|0x02)
#define PAN_RIGHT_CW()		(PAN_PORT = PAN_PORT&0xf7)
#define PAN_RIGHT_CCW()		(PAN_PORT = PAN_PORT|0x08)

#define TILT_DDR		DDRB
#define TILT_INIT		1500
#define TILT_NOZZLE		OCR1A
#define TILT_CAMERA		OCR1B


int panCurrent = 0;


void Motor()
{
	UTIL_LOCK();

	PAN_DDR = 0x00;
	PAN_ENABLE();
	PAN_STEP_M0();
	PAN_DDR = 0xFF;

	TILT_DDR = 0x00;

	TCNT1 = 0;
	ICR1 = 20000;
	TILT_NOZZLE = TILT_INIT;
	TILT_CAMERA = TILT_INIT;

	TCCR1A = (1 << COM1A1) | (1 << COM1B1);
	TCCR1B = (1 << WGM13) | (1 << CS11);
	TCCR1C = 0x00;

	TILT_DDR = 0xFF;

	UTIL_UNLOCK();
}

void MotorEmergency()
{
	PAN_DISABLE();
	TILT_DDR = 0x00;
}


void MotorPanMoveLockedAsync(int step, int sleep)
{
	UTIL_UNLOCK();

	if (step > 0)
	{
		PAN_LEFT_CW();
		PAN_RIGHT_CW();	
	}
	else if (step < 0)
	{
		PAN_LEFT_CCW();
		PAN_RIGHT_CCW();
	}

	panCurrent += step;
	
	while (step != 0)
	{
		PAN_LEFT_CLOCK();
		PAN_RIGHT_CLOCK();

		if (step > 0)
		{
			step--;
		}
		else if (step < 0)
		{
			step++;
		}

		timer_sleep(sleep);
	}

	UTIL_LOCK();
}

void MotorPanGet(Message *m)
{
	ConsoleMessageHelper(m, MESSAGE_OK, 1);
	m->Data[0] = panCurrent;
}

void MotorPanSet(Message *m)
{
	ConsoleMessageHelper(m, MESSAGE_OK, 0);
	MotorPanMoveLockedAsync(m->Data[0] - panCurrent, m->Data[1]);
}

void MotorTiltNozzleGet(Message *m)
{
	ConsoleMessageHelper(m, MESSAGE_OK, 1);
	m->Data[0] = TILT_NOZZLE;
}

void MotorTiltNozzleSet(Message *m)
{
	ConsoleMessageHelper(m, MESSAGE_OK, 0);
	UTIL_LOCK();
	TILT_NOZZLE = m->Data[0];
	UTIL_UNLOCK();
}


void MotorTiltCameraGet(Message *m)
{
	ConsoleMessageHelper(m, MESSAGE_OK, 1);
	m->Data[0] = TILT_CAMERA;
}

void MotorTiltCameraSet(Message *m)
{
	m->Type = MESSAGE_OK;
	m->Length = 0;
	UTIL_LOCK();
	TILT_CAMERA = m->Data[0];
	UTIL_UNLOCK();
}
