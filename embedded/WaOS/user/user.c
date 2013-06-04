
#include "user.h"


const char FndTable[10] = {63, 6, 91, 79, 102, 109, 125, 39, 127, 103};
unsigned char FndData[4] = {0,0,0,0};

void Fnd()
{
	unsigned char digit = 0;

	DDRA = 0xFF;
	DDRC = 0x0F;
	DDRG = 0x03;

	while(1)
	{
		digit++;
	    digit %= 4;
		PORTC = 0x0F;
		PORTA = FndTable[FndData[digit]];
		PORTC = ~(0x01 << digit);

		context_block();
	}
}

int count = 0;

void Counter()
{
	while(1)
	{
		timer_sleep(1000);
		
		count++;
		count %= 10000;

		UTIL_LOCK();
        FndData[0] = (count % 10000) / 1000;
	    FndData[1] = (count % 1000) / 100;
        FndData[2] = (count % 100) / 10;
        FndData[3] = count % 10;
		UTIL_UNLOCK();
	}
}

void User()
{
	task_create(Motor);
	task_create(Water);

	task_create(Console);
	task_create(Fnd);
	task_create(Counter);
}
