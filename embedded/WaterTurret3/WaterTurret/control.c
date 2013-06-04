
#include "common.h"
#include "control.h"


volatile int current_step = 0;
volatile int aimed_step = 0;

static void move_pan();


void init_control()
{
	// Pump - Valve
	PUMP_OFF();
	VALVE_OFF();

#ifdef __AVR_ATmega128__

	// Timer 0 : Pan Motor
	TCCR0 = (1 << WGM01) | (1 << CS02) | (1 << CS01) | (1 << CS00);
	TCNT0 = 0x00;
	OCR0 = 62;
	TIMSK |= 1 << OCIE0;	

#else

	// Timer 0 : Pan Motor
	TCCR0 = (1 << WGM01) | (1 << CS02);
	TCNT0 = 0x00;
	OCR0 = 62;
	TIMSK |= 1 << OCIE0;	

#endif

	// Timer 1 : Tilt Motor
	TCNT1 = 0;
	ICR1 = 20000;
	TILT_NOZZLE = TILT_INIT;
	TILT_CAMERA = TILT_INIT;

	TCCR1A = (1 << COM1A1) | (1 << COM1B1);
	TCCR1B = (1 << WGM13) | (1 << CS11);
	TIMSK |= 0;
	TIFR = 0;

	// Pan
	PAN_ENABLE();
}

ISR(SIG_OUTPUT_COMPARE0)
{
	move_pan();
}


static void move_pan()
{
	if (current_step != aimed_step)
	{
		int diff = current_step - aimed_step;

		if (diff > 0)
		{
			PAN_CCW();
			PAN_CLOCK();
			current_step--;
		}
		else
		{
			PAN_CW();
			PAN_CLOCK();
			current_step++;
		}
	}
}

void set_pan(int step)
{
	aimed_step = step;
}

void wait_pan()
{
	while (current_step != aimed_step);
}

int get_pan()
{
	return current_step;
}
