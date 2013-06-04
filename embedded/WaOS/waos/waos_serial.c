
#include "waos.h"

FILE waos_serial_rx = FDEV_SETUP_STREAM(NULL, serial_rx0, _FDEV_SETUP_READ);
FILE waos_serial_tx = FDEV_SETUP_STREAM(serial_tx0, NULL, _FDEV_SETUP_WRITE);

void serial_init()
{	
	// 16M, 38.4k, 0.2%
	UBRR0H = WAOS_SERIAL_UBRR >> 8;
	UBRR0L = WAOS_SERIAL_UBRR;

	UCSR0A = 0x00;
	UCSR0C = (1 << UCSZ01) | (1 << UCSZ00);
	UCSR0B = (1 << RXEN0) | (1 << TXEN0);

	// 표준 입출력
	stdin = &waos_serial_rx;
	stdout = &waos_serial_tx;
}

int serial_tx0(char data, FILE *stream)
{
	/*
	if(data == '\n')
	{
		serial_tx0('\r', stream);
	}
	*/

	while (!(UCSR0A & (1 << UDRE0)));
	UDR0 = data;
	return 0;
}

int serial_rx0(FILE *stream)
{
	while (!(UCSR0A & (1 << RXC0)));
	return UDR0;
}
