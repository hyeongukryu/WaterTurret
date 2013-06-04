
#include "common.h"
#include "init.h"
#include "control.h"

void init_port()
{

#ifdef __AVR_ATmega128__

	PORTA = 0x00;
	DDRA = 0x00;
	PORTB = 0x00;
	DDRB = 0x00;
	PORTC = 0x00;
	DDRC = 0x00;
	PORTD = 0x00;
	DDRD = 0x00;
	PORTE = 0x00;
	DDRE = 0x00;
	PORTF = 0x00;
	DDRF = 0x00;

	// Pan Motor
	PAN_DDR |= 0xFF;

	// Tilt Motor
	DDRB |= 0xFF;

	// Pump, Valve
	DDRD |= 0xFF;

	TIMSK = 0x00;

#else
#endif
}


void init_timer()
{
	// Timer 2 : FND
	TCCR2 = (1 << WGM21) | (1 << CS22);
	TCNT2 = 0x00;
	OCR2 = 62;
	TIMSK |= 1 << OCIE2;
	
	DDRA = 0xFF;
	DDRC = 0x0F;
}

int time = 0;
int second = 0;
unsigned char digit = 0;

const char FndTable[10] = {63, 6, 91, 79, 102, 109, 125, 39, 127, 103};
unsigned char FndData[4] = {0,0,0,0};

ISR(SIG_OUTPUT_COMPARE2)
{
	if (second++ == 1000)
	{
		second = 0;
		if(++time == 10000)
		{
			time = 0;
		}

		FndData[0] = (time % 10000) / 1000;
	    FndData[1] = (time % 1000) / 100;
        FndData[2] = (time % 100) / 10;
        FndData[3] = time % 10;
	}

	if(++digit == 4)
	{
		digit = 0;
	}
	
	PORTC = 0x0F;
	PORTA = FndTable[FndData[digit]];
	PORTC = ~(0x01 << digit);	
}
