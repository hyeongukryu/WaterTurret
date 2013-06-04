
#include "service.h"
#include "control.h"


void ServDeviceCheck(message_t *m)
{
}

void ServEmergency(message_t *m)
{
	PAN_DISABLE();
	VALVE_OFF();
	PUMP_OFF();
}

void ServPumpOn(message_t *m)
{
	PUMP_ON();
}

void ServPumpOff(message_t *m)
{
	PUMP_OFF();
}

void ServValveOn(message_t *m)
{
	VALVE_ON();
}

void ServValveOff(message_t *m)
{
	VALVE_OFF();
}


void ServPanGet(message_t *m)
{
	wait_pan();
	m->length = 1;
	m->data[0] = get_pan();
}

void ServPanSet(message_t *m)
{
	wait_pan();
	set_pan(m->data[0]);
	
	// sync
	wait_pan();
}

void ServTiltNozzleGet(message_t *m)
{
	m->length = 1;
	m->data[0] = TILT_NOZZLE;
}

void ServTiltNozzleSet(message_t *m)
{
	TILT_NOZZLE = m->data[0];
}

void ServTiltCameraGet(message_t *m)
{
	m->length = 1;
	m->data[0] = TILT_CAMERA;
}

void ServTiltCameraSet(message_t *m)
{
	TILT_CAMERA = m->data[0];
}
