//
//  NetworkBridge.h
//  NetworkPlugin
//
//  Created by TienKM on 30/6/25.
//
#include <stdbool.h>
#ifdef __cplusplus
extern "C" {
#endif
void startListening(void);
bool getCurrentNetworkState(void);
#ifdef __cplusplus
}
#endif
