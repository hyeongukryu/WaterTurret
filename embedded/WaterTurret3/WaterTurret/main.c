
#include "common.h"
#include "init.h"
#include "serial.h"
#include "control.h"

int main()
{
	cli();

	init_port();
	init_timer();
	init_control();

	init_serial();

	sei();

	for(;;)
	{
		serial_loop();
	}

	return 0;
}
