// Basiscode voor het starten van eender welk project op een Nucleo-F091RC met Nucleo Extension Shield V2.
//
// OPM:
//	- via 'Project -> Manage -> Select software packs' kies je bij Keil::STM32F0xx_DFP voor versie 2.0.0.
//	- via 'Options for Target -> C/C++' zet je de compiler op C11, optimizations op default en warnings op AC5-like.
// 
// Versie: 20230206

// Includes.
#include "stm32f091xc.h"
#include "stdio.h"
#include "stdbool.h"
#include "leds.h"
#include "buttons.h"
#include "usart2.h"
#include "ad.h"

// Functie prototypes.
void SystemClock_Config(void);
void InitIo(void);
void WaitForMs(uint32_t timespan);

// Variabelen aanmaken. 
// OPM: het keyword 'static', zorgt ervoor dat de variabele enkel binnen dit bestand gebruikt kan worden.
static uint8_t count = 0;
static volatile uint32_t ticks = 0;
static uint8_t led = 0;
static uint8_t receivedByte = 0;
static uint8_t receivedByteleds = 0;
static uint8_t receivedBytedruk = 0;
static char text[101];
static uint16_t adcValue = 0;


// Entry point.
int main(void)
{
	// Initialisaties.
	SystemClock_Config();
	InitIo();
	InitButtons();
	InitLeds();
	InitUsart2(9600);
	InitAd();
	
	// Clock voor GPIOF inschakelen.
	RCC->AHBENR = RCC->AHBENR | RCC_AHBENR_GPIOFEN;
	
	
	// Clock voor GPIOC inschakelen.
	RCC->AHBENR = RCC->AHBENR | RCC_AHBENR_GPIOCEN;	
	
	// PC2 en PC3 en PC4 als uitgang zetten	
	GPIOC->MODER = (GPIOC->MODER & ~GPIO_MODER_MODER2) | GPIO_MODER_MODER2_0;
	GPIOC->MODER = (GPIOC->MODER & ~GPIO_MODER_MODER3) | GPIO_MODER_MODER3_0;
	GPIOC->MODER = (GPIOC->MODER & ~GPIO_MODER_MODER4) | GPIO_MODER_MODER4_0;
	
	
	// Laten weten dat we opgestart zijn, via de USART2 (USB).
	StringToUsart2("Reboot\r\n");
	
	// Oneindige lus starten.
	while (1)
	{	
		GPIOC->ODR = GPIOC->ODR & ~GPIO_ODR_4;
		if(SW1Active())
		{
			led = 1;	
		}
		
		if(SW2Active())
		{
			led = 2;	
		}
		
		if(SW3Active())
		{
			led = 3;	
		}
		
		if(SW4Active())
		{
			led = 4;	
		}
		
		GPIOC->ODR = GPIOC->ODR | GPIO_ODR_14;
		
		
		adcValue = (GetAdValue() >> 4);
		
		sprintf(text, "%d,%d\r\n", led, adcValue);
		StringToUsart2(text);
		
		WaitForMs(100);
		
		
		if((USART2->ISR & USART_ISR_RXNE) == USART_ISR_RXNE)
		{
			receivedByte = USART2->RDR;
			ByteToLeds(receivedByte);
			
			if (receivedByte >= 128)
			{
				GPIOC->ODR = GPIOC->ODR | GPIO_ODR_4;  // PC4 inschakelen
				WaitForMs(200);                        // wacht 200 ms
				GPIOC->ODR = GPIOC->ODR & ~GPIO_ODR_4; // PC4 uitschakelen
				ByteToLeds(receivedByte);
				
			}
			
			if (receivedByte >= 32  && receivedByte <= 63)
			{ 
          GPIOC->ODR = GPIOC->ODR | GPIO_ODR_2;		// PC2 inschakelen
		      GPIOC->ODR = GPIOC->ODR & ~GPIO_ODR_3;  // PC3 uitschakelen
			}
			
			
			if (receivedByte >= 64 && receivedByte <=127)
			{ 
          GPIOC->ODR = GPIOC->ODR | GPIO_ODR_3;		// PC3 inschakelen
		      GPIOC->ODR = GPIOC->ODR & ~GPIO_ODR_2;  // PC2 uitschakelen   
			}
			
			if (receivedByte <= 31)
			{ 
          GPIOC->ODR = GPIOC->ODR & ~GPIO_ODR_3;		// PC3 uitschakelen  
		      GPIOC->ODR = GPIOC->ODR & ~GPIO_ODR_2;    // PC2 uitschakelen  
			}
		}
		
		WaitForMs(100);
	}
	
	// Terugkeren zonder fouten... (unreachable).
	return 0;
}

// Functie om extra IO's te initialiseren.
void InitIo(void)
{

}

// Handler die iedere 1ms afloopt. Ingesteld met SystemCoreClockUpdate() en SysTick_Config().
void SysTick_Handler(void)
{
	ticks++;
}

// Wachtfunctie via de SysTick.
void WaitForMs(uint32_t timespan)
{
	uint32_t startTime = ticks;
	
	while(ticks < startTime + timespan);
}

// Klokken instellen. Deze functie niet wijzigen, tenzij je goed weet wat je doet.
void SystemClock_Config(void)
{
	RCC->CR |= RCC_CR_HSITRIM_4;														// HSITRIM op 16 zetten, dit is standaard (ook na reset).
	RCC->CR  |= RCC_CR_HSION;																// Internal high speed oscillator enable (8MHz)
	while((RCC->CR & RCC_CR_HSIRDY) == 0);									// Wacht tot HSI zeker ingeschakeld is
	
	RCC->CFGR &= ~RCC_CFGR_SW;															// System clock op HSI zetten (SWS is status geupdatet door hardware)	
	while((RCC->CFGR & RCC_CFGR_SWS) != RCC_CFGR_SWS_HSI);	// Wachten to effectief HSI in actie is getreden
	
	RCC->CR &= ~RCC_CR_PLLON;																// Eerst PLL uitschakelen
	while((RCC->CR & RCC_CR_PLLRDY) != 0);									// Wacht tot PLL zeker uitgeschakeld is
	
	RCC->CFGR |= RCC_CFGR_PLLSRC_HSI_PREDIV;								// 01: HSI/PREDIV selected as PLL input clock
	RCC->CFGR2 |= RCC_CFGR2_PREDIV_DIV2;										// prediv = /2		=> 4MHz
	RCC->CFGR |= RCC_CFGR_PLLMUL12;													// PLL multiplied by 12 => 48MHz
	
	FLASH->ACR |= FLASH_ACR_LATENCY;												//  meer dan 24 MHz, dus latency op 1 (p 67)
	
	RCC->CR |= RCC_CR_PLLON;																// PLL inschakelen
	while((RCC->CR & RCC_CR_PLLRDY) == 0);									// Wacht tot PLL zeker ingeschakeld is

	RCC->CFGR |= RCC_CFGR_SW_PLL; 													// PLLCLK selecteren als SYSCLK (48MHz)
	while((RCC->CFGR & RCC_CFGR_SWS) != RCC_CFGR_SWS_PLL);	// Wait until the PLL is switched on
		
	RCC->CFGR |= RCC_CFGR_HPRE_DIV1;												// SYSCLK niet meer delen, dus HCLK = 48MHz
	RCC->CFGR |= RCC_CFGR_PPRE_DIV1;												// HCLK niet meer delen, dus PCLK = 48MHz	
	
	SystemCoreClockUpdate();																// Nieuwe waarde van de core frequentie opslaan in SystemCoreClock variabele
	SysTick_Config(48000);																	// Interrupt genereren. Zie core_cm0.h, om na ieder 1ms een interrupt 
																													// te hebben op SysTick_Handler()
}
