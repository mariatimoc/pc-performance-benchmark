#pragma once

#ifdef __cplusplus
extern "C" {
#endif


#define MAX_NUMBER_OF_CACHE_LEVELS 5
#define EXPORT __declspec(dllexport)

typedef struct {
	//din eax 
	int type;
	int level;
	int selfInitializing;
	int fullyAssociative;
	int numberThreads;
	int numberCores;

	//din ebx
	int sizeLine;
	int numberLinePartitions;
	int numberAsociativityPaths;

	//din ecx
	int totalNumberOfSets;

	//din edx
	int isInclusiveCache;
	int complexIndexing;

}CACHE_MEMORY;


typedef struct {
	char architecture[20];
	unsigned int logicalProcessors;
	unsigned long long totalRamMb;
} SYSINFO;


	EXPORT void cpu_get_producer_name(char* producerName);
	EXPORT void cpu_get_brand(char* brand);
	EXPORT void cpu_get_basics(int* family, int* model, int* stepping, int* type);
	EXPORT void cpu_get_address_size(int* physBits, int* virtBits);
	EXPORT int  cpu_get_frequencies(int* baseMHz, int* maxMHz, int* busMHz);
	EXPORT int  cpu_get_cache_all_levels(CACHE_MEMORY* outCaches, int maxCaches);

	EXPORT void sys_get_information(SYSINFO* outInfo);
	EXPORT double test_memory_random_access(int n, int stepsPerElement);
	EXPORT double test_gauss_int_performance(int n);
	EXPORT double test_floating_numbers_performance(int width, int height, int iterations);

	

#ifdef __cplusplus}
#endif
