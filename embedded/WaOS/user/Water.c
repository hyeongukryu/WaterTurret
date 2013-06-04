
#include "user.h"

#define WATER_PORT			PORTD
#define WATER_DDR			DDRD

#define WATER_PUMP_BIT		PD0
#define WATER_VALVE_BIT		PD1


void Water()
{
	UTIL_LOCK();
	WATER_PORT = 0x00;
	WATER_DDR = 0xFF;
	UTIL_UNLOCK();
}

void WaterEmergency()
{
	WATER_PORT = 0x00;
}

void WaterPumpOn(Message *m)
{
	port_status_led_on(0);
	m->Type = MESSAGE_OK;
	m->Length = 0;
	WATER_PORT |= 1 << WATER_PUMP_BIT;
}

void WaterPumpOff(Message *m)
{
	port_status_led_off(0);
	m->Type = MESSAGE_OK;
	m->Length = 0;
	WATER_PORT &= ~(1 << WATER_PUMP_BIT);
}

void WaterValveOn(Message *m)
{
	port_status_led_on(1);
	m->Type = MESSAGE_OK;
	m->Length = 0;
	WATER_PORT |= 1 << WATER_VALVE_BIT;
}

void WaterValveOff(Message *m)
{
	port_status_led_off(1);
	m->Type = MESSAGE_OK;
	m->Length = 0;
	WATER_PORT &= ~(1 << WATER_VALVE_BIT);
}
