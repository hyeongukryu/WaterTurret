
#include "waos.h"


void port_init()
{
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
	PORTG = 0x00;
	DDRG = 0x00;

//	PORT_CLOCK_LED_DDR |= (1 << PORT_CLOCK_LED_BIT);
	PORT_STATUS_A_LED_DDR |= (1 << PORT_STATUS_A_LED_BIT);
	PORT_STATUS_B_LED_DDR |= (1 << PORT_STATUS_B_LED_BIT);
}

void port_status_led_on(char led)
{
	if (led == 0)
	{
		PORT_STATUS_A_LED_PORT |= (1 << PORT_STATUS_A_LED_BIT);
	}
	else if (led == 1)
	{
		PORT_STATUS_B_LED_PORT |= (1 << PORT_STATUS_B_LED_BIT);
	}
}

void port_status_led_off(char led)
{
	if (led == 0)
	{
		PORT_STATUS_A_LED_PORT &= ~(1 << PORT_STATUS_A_LED_BIT);
	}
	else if (led == 1)
	{
		PORT_STATUS_B_LED_PORT &= ~(1 << PORT_STATUS_B_LED_BIT);
	}
}

/*
void port_status_led_toggle(char led)
{
	if (led == 0)
	{
		PORT_STATUS_A_LED_PORT ^= (1 << PORT_STATUS_A_LED_BIT);
	}
	else if (led == 1)
	{
		PORT_STATUS_B_LED_PORT ^= (1 << PORT_STATUS_B_LED_BIT);
	}
}

void port_clock_led()
{
	PORT_CLOCK_LED_PORT ^= (1 << PORT_CLOCK_LED_BIT);
}
*/
