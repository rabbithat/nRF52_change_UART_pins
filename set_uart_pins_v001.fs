$50000504 CONSTANT _NRF_GPIO__OUT  \ set by default to pin P0.24.  Want it to be P0.06
$50000508 CONSTANT _NRF_GPIO__OUTSET \ needed to set the GPIO_OUT register
$50000510 CONSTANT _NRF_GPIO__IN   \ set by default to pin p0.25.  Want it to be P0.08
$50000514 CONSTANT _NRF_GPIO__DIR   
$50000518 CONSTANT _NRF_GPIO__DIRSET \ needed to set the GPIO_DIR register 
$50000700 CONSTANT _NRF_GPIO__PIN_CNF[0] \ for reference
$50000718 CONSTANT _NRF_GPIO__PIN_CNF[6] \ we want pin P0.06 as the output pin
$50000720 CONSTANT _NRF_GPIO__PIN_CNF[8] \ we want pin P0.08 as the input pin
$50000760 CONSTANT _NRF_GPIO__PIN_CNF[24] \ pin P0.24 is the default output pin.  Default value = 0x1
$50000764 CONSTANT _NRF_GPIO__PIN_CNF[25] \ pin P0.25 is the default input pin.  Default value = 0xC


: SET_PIN_P0.6  %0001 _NRF_GPIO__PIN_CNF[6] ! ;  \ Set pin P0.06 to be an output pin.
: SET_PIN_P0.8  %1100 _NRF_GPIO__PIN_CNF[8] ! ;  \ set as input with pulldown resistor
: SET_GPIO_DIRSET %1000000 _NRF_GPIO__DIRSET ! ;  \ Set pin P0.06 to be an output pin.
: SET_GPIO_OUTSET %1000000 _NRF_GPIO__OUTSET ! ;  \ Set pin P0.06 to be an output pin.
\ : SET_GPIO SET_GPIO_DIRSET SET_PIN_P0.6 SET_PIN_P0.8 SET_GPIO_OUTSET SET_GPIO_IN ; \ handle all GPIO register changes

0 variable <newTxPin>
0 variable <newRxPin>
0 variable <newTxPinAddress>
0 variable <newRxPinAddress>
0 variable <txPinMask>

( pin# -- gpioAddress )
: gpioAddressOfPin 4 * _NRF_GPIO__PIN_CNF[0] + ;

( pin# -- )
: setTxPinMask 1 swap lshift ;

(  --  )
: setUartTxPinAsOutputPin %0001 <newTxPinAddress> @ ! <txPinMask> @ dup _NRF_GPIO__DIRSET ! _NRF_GPIO__OUTSET ! ;


: setUartRxPinAsInputPin %1100 <newTxPinAddress> @ ! ;



$40002500 constant _NRF_UART__ENABLE  \ Note: UART must be disabled before changing TXD and RXD pin assignments
: UART_ENABLE! #4 _NRF_UART__ENABLE ! ;  \ enable the UART
: UART_DISABLE! 0 _NRF_UART__ENABLE ! ; \ disable the UART

$40002508 CONSTANT _NRF_UART__PSEL.RTS  \ set by default to no pin at all
$4000250C CONSTANT _NRF_UART__PSEL.TXD  \ set by default to pin P0.24.  Want it to be P0.06
$40002510 CONSTANT _NRF_UART__PSEL.CTS  \ set by default to no pin at all
$40002514 CONSTANT _NRF_UART__PSEL.RXD  \ set by default to pin P0.25.  Want it to be P0.08

: SET_UART_PSEL.TXD  #6 _NRF_UART__PSEL.TXD ! ;  \ Change from pin P0.24 (default) to P0.06
: SET_UART_PSEL.RXD  #8 _NRF_UART__PSEL.RXD ! ;  \ Change from pin P0.25 (default) to P0.08
: SET_UART SET_UART_PSEL.TXD SET_UART_PSEL.RXD  ; \ handle all UART register changes

\ : CHANGE_UART_PINS UART_DISABLE!  SET_GPIO SET_UART UART_ENABLE! ;  \ the one word that does it all
\ : INIT CHANGE_UART_PINS ;  \ automatically make the change on every reset

( pin# -- )
: makeNewUartTxPin UART_DISABLE! dup <newTxPin> ! dup gpioAddressOfPin <newTxPinAddress> ! dup setTxPinMask <txPinMask> ! _NRF_UART__PSEL.TXD ! setUartTxPinAsOutputPin UART_ENABLE! ;

( pin# -- )
: makeNewUartRxPin UART_DISABLE! dup <newRxPin> ! dup gpioAddressOfPin <newRxPinAddress> ! _NRF_UART__PSEL.RXD !  setUartRxPinAsInputPin UART_ENABLE! ;

\ : init #29 makeNewUartRxPin #31 makeNewUartTxPin ; \ for dongle

