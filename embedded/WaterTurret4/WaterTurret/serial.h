
#ifndef __SERIAL__
#define __SERIAL__


#define UBRR			51

#define SEQUENCE_TYPE	int
#define TYPE_TYPE		char
#define TYPE_LENGTH		char
#define TYPE_DATA		int

#define TX_SEQUENCE(x)	serial_tx_2(x)
#define TX_TYPE(x)		serial_tx(x)
#define TX_LENGTH(x)	serial_tx(x)
#define TX_DATA(x)		serial_tx_2(x)

#define RX_SEQUENCE()	serial_rx_2()
#define RX_TYPE()		serial_rx()
#define RX_LENGTH()		serial_rx()
#define RX_DATA()		serial_rx_2()

typedef struct
{
	SEQUENCE_TYPE sequence;
	TYPE_TYPE type;
	TYPE_LENGTH length;
	TYPE_DATA data[8];
} message_t;


#define MESSAGE_OK		42
#define MESSAGE_FAIL	100
#define MESSAGE_TYPE	101


void init_serial();

void serial_loop();

#endif
