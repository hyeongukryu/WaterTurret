
#include "waos.h"

void util_delay_us (uint8_t time_us)
{
	register uint8_t i; 
	for(i = 0; i < time_us; i++)	// 4 cycle
	{
		asm volatile("PUSH	R0");	// 2 cycle
		asm volatile("POP 	R0");	// 2 cycle
		asm volatile("PUSH	R0");	// 2 cycle
		asm volatile("POP 	R0");	// 2 cycle
		asm volatile("PUSH	R0");	// 2 cycle
		asm volatile("POP 	R0");	// 2 cycle
		
		// 16 cycle = 1 us for 16MHz
	}
}

void util_delay_ms (int time_ms) 
{ 
	register int i; 
	for (i = 0; i < time_ms; i++) 
	{
		util_delay_us(250);
		util_delay_us(250);
		util_delay_us(250);
		util_delay_us(250);
	}
}
