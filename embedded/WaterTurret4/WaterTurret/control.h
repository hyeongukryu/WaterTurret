
#ifndef __CONTROL__
#define __CONTROL_


#include "common.h"

#ifdef __AVR_ATmega128__

#define PAN_PORT		PORTF
#define PAN_DDR			DDRF

#define PAN_ENABLE()		(PAN_PORT = PAN_PORT|0x80)
#define PAN_DISABLE()		(PAN_PORT = PAN_PORT&0x7f)
#define PAN_STEP_M0()		(PAN_PORT = (PAN_PORT&0xcf)|0x00)
#define PAN_STEP_M1()		(PAN_PORT = (PAN_PORT&0xcf)|0x10)
#define PAN_STEP_M2()		(PAN_PORT = (PAN_PORT&0xcf)|0x20)
#define PAN_STEP_M3()		(PAN_PORT = (PAN_PORT&0xcf)|0x30)
#define PAN_CLOCK()			(PAN_PORT = PAN_PORT^0x01)
#define PAN_CW()			(PAN_PORT = PAN_PORT&0xfd)
#define PAN_CCW()			(PAN_PORT = PAN_PORT|0x02)

#define TILT_INIT		1500
#define TILT_NOZZLE		OCR1A
#define TILT_CAMERA		OCR1B

#define PUMP_ON()		PORTD |= (1 << PD0)
#define PUMP_OFF()		PORTD &= ~(1 << PD0)

#define VALVE_ON()		PORTD |= (1 << PD1)
#define VALVE_OFF()		PORTD &= ~(1 << PD1)


#else

#define PAN_PORT		PORTA
#define PAN_DDR			DDRA

#define PAN_ENABLE()
#define PAN_DISABLE()

#define PAN_MICRO_MASK	0x0C
#define PAN_STEP_M0()	(PAN_PORT = (PAN_PORT & ~PAN_MICRO_MASK))
#define PAN_STEP_M1()	(PAN_PORT = (PAN_PORT & ~PAN_MICRO_MASK) | 0x04)
#define PAN_STEP_M2()	(PAN_PORT = (PAN_PORT & ~PAN_MICRO_MASK) | 0x08)
#define PAN_STEP_M3()	(PAN_PORT = (PAN_PORT & ~PAN_MICRO_MASK) | 0x0C)

#define PAN_CLOCK()		(PAN_PORT = PAN_PORT ^ 0x02)
#define PAN_CW()		(PAN_PORT = PAN_PORT & 0xFE)
#define PAN_CCW()		(PAN_PORT = PAN_PORT | 0x01)


#define TILT_INIT		1500
#define TILT_NOZZLE		OCR1A
#define TILT_CAMERA		OCR1B

#define PUMP_ON()		PORTD &= ~(1 << PD7)
#define PUMP_OFF()		PORTD |= (1 << PD7)

#define VALVE_ON()		PORTC &= ~(1 << PC7)
#define VALVE_OFF()		PORTC |= (1 << PC7)

#endif

void init_control();

void set_pan(int);
void wait_pan();
int get_pan();



#endif
