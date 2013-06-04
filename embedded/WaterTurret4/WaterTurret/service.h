
#ifndef __SERVICE__
#define __SERVICE__


#include "serial.h"



void ServDeviceCheck(message_t *);
void ServEmergency(message_t *);

void ServPumpOn(message_t *);
void ServPumpOff(message_t *);

void ServValveOn(message_t *);
void ServValveOff(message_t *);

void ServWaterOn(message_t *);
void ServWaterOff(message_t *);

void ServPanGet(message_t *);
void ServPanSet(message_t *);

void ServTiltNozzleGet(message_t *);
void ServTiltNozzleSet(message_t *);

void ServTiltCameraGet(message_t *);
void ServTiltCameraSet(message_t *);


#endif
