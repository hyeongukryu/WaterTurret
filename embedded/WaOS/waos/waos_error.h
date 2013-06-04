
#ifndef _WAOS_ERROR
#define _WAOS_ERROR

#ifndef WAOS_FLAG_NO_ERROR

void error_init(void);
void error_last_error_set(char);
char error_last_error_get();

#endif

#endif
