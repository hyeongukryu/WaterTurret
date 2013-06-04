
#ifndef _MOTOR
#define _MOTOR


void Motor();
void MotorEmergency();

void MotorPanGet(Message *);
void MotorPanSet(Message *);

void MotorTiltNozzleGet(Message *);
void MotorTiltNozzleSet(Message *);

void MotorTiltCameraGet(Message *);
void MotorTiltCameraSet(Message *);
void MotorTiltCameraAdd(Message *);

#endif
