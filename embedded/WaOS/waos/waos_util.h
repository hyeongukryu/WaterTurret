
#ifndef _WAOS_UTIL
#define _WAOS_UTIL


#define HIGH_BIT(x) (((int)x >> 8) & 0xFF)
#define LOW_BIT(x) (((int)x) & 0xFF)

void util_delay_ms(int ms);
void util_delay_us(uint8_t us);


#define UTIL_LOCK()		cli();
#define UTIL_UNLOCK()	sei();




#endif
