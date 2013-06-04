
#ifndef _WAOS_PORT
#define _WAOS_PORT


/*
#define PORT_CLOCK_LED_PORT			PORTA
#define PORT_CLOCK_LED_DDR			DDRA
#define PORT_CLOCK_LED_BIT			PA0
*/

#define PORT_STATUS_A_LED_PORT		PORTG
#define PORT_STATUS_A_LED_DDR		DDRG
#define PORT_STATUS_A_LED_BIT		PG0

#define PORT_STATUS_B_LED_PORT		PORTG
#define PORT_STATUS_B_LED_DDR		DDRG
#define PORT_STATUS_B_LED_BIT		PG1




void port_init();

void port_status_led_on(char led);
void port_status_led_off(char led);
// void port_status_led_toggle(char led);

// void port_clock_led();

#endif
