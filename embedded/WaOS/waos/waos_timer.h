
#ifndef _WAOS_TIMER
#define _WAOS_TIMER

// 타이머 관리자
void timer_init();
void timer_tick_event();
// unsigned int timer_current_tick_get();
void timer_sleep(unsigned int);


#endif
