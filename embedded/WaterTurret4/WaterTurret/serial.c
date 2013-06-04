
#include "common.h"
#include "serial.h"
#include "service.h"

void serial_tx(char data);
char serial_rx();

void serial_tx_2(int data);
int serial_rx_2();

void serial_loop();
void serial_process(message_t *);



void init_serial()
{

#ifdef __AVR_ATmega128__

	UBRR0H = (unsigned char)(UBRR >> 8);
	UBRR0L = (unsigned char)UBRR;

	UCSR0A = 0x00;
	UCSR0C = (1 << UCSZ1) | (1 << UCSZ0);
	UCSR0B = (1 << RXEN) | (1 << TXEN);

#else
#endif
}

void serial_tx(char data)
{
#ifdef __AVR_ATmega128__

	while (!(UCSR0A & (1 << UDRE)));
	UDR0 = data;

#else
#endif
}

char serial_rx()
{
#ifdef __AVR_ATmega128__

	while (!(UCSR0A & (1 << RXC)));
	return UDR0;

#else
#endif
}

void serial_tx_2(int data)
{
	// Big Endian
	serial_tx((data >> 8) & 0xFF);
	serial_tx(data & 0xFF);
}

int serial_rx_2()
{
	// Big Endian
	int data = (int)serial_rx() << 8;
	return data | serial_rx();
}

void serial_loop()
{
	message_t message;

	message.sequence = RX_SEQUENCE();
	message.type = RX_TYPE();
	message.length = RX_LENGTH();
	for (int i = 0; i < message.length; i++)
	{
		message.data[i] = RX_DATA();
	}

	serial_process(&message);

	TX_SEQUENCE(message.sequence);
	TX_TYPE(message.type);
	TX_LENGTH(message.length);
	for (int i = 0; i < message.length; i++)
	{
		TX_DATA(message.data[i]);
	}
}

void serial_process(message_t *message)
{
	char type = message->type;
	message->type = MESSAGE_OK;
	message->length = 0;

	switch(type)
	{
	case 10:	ServDeviceCheck(message);	break;
	case 11:	ServEmergency(message);		break;
	
	case 20:	ServPanGet(message);		break;
	case 21:	ServPanSet(message);		break;
	
	case 30:	ServTiltNozzleGet(message);	break;
	case 31:	ServTiltNozzleSet(message);	break;
	
	case 40:	ServTiltCameraGet(message);	break;
	case 41:	ServTiltCameraSet(message);	break;
	
	case 50:	ServPumpOn(message);		break;
	case 51:	ServPumpOff(message);		break;
	case 52:	ServValveOn(message);		break;
	case 53:	ServValveOff(message);		break;

	default:
		message->type = MESSAGE_TYPE;
		message->length = 0;
	}
}
