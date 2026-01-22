#include "performance.h"
#include <stdlib.h>
#include <time.h>
#include <math.h>
#include <windows.h>
#include <string.h>  


#define _CRT_SECURE_NO_WARNINGS	
#define	 MAX_NUMBER_OF_CACHE_LEVELS 5

typedef struct {
	char producerName[13];
	char brand[49];
	int family;
	int model;
	int stepping;
	int type;
}CPU_INFO;



typedef struct {
	int physicalAddressesNumberBits;
	int virtualAddressesNumberBits;
}ADDRESS_SIZE;



typedef struct {
	int baseFrequency;
	int maxFrequency;
	int magistralaFrequency;
}FREQUENCY;


void get_cpuid(int cpuInfo[4], int eaxVal) {
	_asm {
		mov eax, eaxVal
		cpuid
		mov edi, cpuInfo
		mov[edi], eax
		mov[edi + 4], ebx
		mov[edi + 8], ecx
		mov[edi + 12], edx
	}
}


void get_cpuid_plus_ecx(int cpuInfo[4], int eaxVal, int ecxVal) {
	_asm {
		mov eax, eaxVal
		mov ecx, ecxVal
		cpuid
		mov edi, cpuInfo
		mov[edi], eax
		mov[edi + 4], ebx
		mov[edi + 8], ecx
		mov[edi + 12], edx

	}
}


void cpu_get_producer_name(char* producerName) {
	int cpuInfo[4];
	get_cpuid(cpuInfo, 0);

	int eax = cpuInfo[0];
	int ebx = cpuInfo[1];
	int ecx = cpuInfo[2];
	int edx = cpuInfo[3];

	memcpy(producerName + 0, &ebx, 4);
	memcpy(producerName + 4, &edx, 4);
	memcpy(producerName + 8, &ecx, 4);

	producerName[12] = '\0';

}


void cpu_get_basics(int* family, int* model, int* stepping, int* type) {
	int cpuInfo[4];
	get_cpuid(cpuInfo, 1);


	int eax = cpuInfo[0];

	*stepping = eax & 0xF;
	*model = (eax >> 4) & 0xF; //model se afla pe 4 to 7
	*family = (eax >> 8) & 0xF;
	*type = (eax >> 12) & 0x3;
}


void cpu_get_brand(char* brand) {
	int info[4];

	get_cpuid(info, 0x80000002);
	memcpy(brand + 0, info, 16);


	get_cpuid(info, 0x80000003);
	memcpy(brand + 16, info, 16);

	get_cpuid(info, 0x80000004);
	memcpy(brand + 32, info, 16);

	brand[48] = '\0';

}

/*

CPU_INFO get_cpu_information() {
	CPU_INFO information;
	cpu_get_producer_name(information.producerName);
	cpu_get_basics(&information.family, &information.model, &information.stepping, &information.type);
	cpu_get_brand(information.brand);

	return information;
}*/


CACHE_MEMORY get_cache_per_level(int ecxIndex) {
	CACHE_MEMORY cache;
	int cacheInfo[4];

	get_cpuid_plus_ecx(cacheInfo, 4, ecxIndex);

	int eax = cacheInfo[0];
	int ebx = cacheInfo[1];
	int ecx = cacheInfo[2];
	int edx = cacheInfo[3];

	cache.type = eax & 0x1F;
	cache.level = (eax >> 5) & 0x7;
	cache.selfInitializing = (eax >> 8) & 0x1;
	cache.fullyAssociative = (eax >> 9) & 0x1;
	cache.numberThreads = ((eax >> 14) & 0xFFF) + 1;
	cache.numberCores = ((eax >> 26) & 0x3F) + 1;

	cache.sizeLine = (ebx & 0xFFF) + 1;
	cache.numberLinePartitions = ((ebx >> 12) & 0x3FF) + 1;
	cache.numberAsociativityPaths = ((ebx >> 22) & 0x3FF) + 1;

	cache.totalNumberOfSets = ecx + 1;

	cache.complexIndexing = (edx >> 2) & 1;
	cache.isInclusiveCache = (edx >> 1) & 1;

	return cache;
}




int get_cache_at_all_levels(CACHE_MEMORY caches[MAX_NUMBER_OF_CACHE_LEVELS]) {
	int cnt = 0;

	for (int i = 0; i < MAX_NUMBER_OF_CACHE_LEVELS; i++) {
		CACHE_MEMORY cache = get_cache_per_level(i);

		if (cache.type == 0) {
			break;
		}

		caches[cnt] = cache;
		cnt++;
	}

	return cnt;

}


ADDRESS_SIZE get_address_size() {
	int addressInfo[4];
	ADDRESS_SIZE address;

	get_cpuid(addressInfo, 0x80000008);

	int eax = addressInfo[0];

	address.physicalAddressesNumberBits = eax & 0xFF;
	address.virtualAddressesNumberBits = (eax >> 8) & 0xFF;

	return address;
}




FREQUENCY get_frequences() {
	FREQUENCY freq;
	int freqInfo[4];

	get_cpuid(freqInfo, 0x16);

	int eax = freqInfo[0];
	int ebx = freqInfo[1];
	int ecx = freqInfo[2];

	freq.baseFrequency = eax;
	freq.maxFrequency = ebx;
	freq.magistralaFrequency = ecx;

	return freq;

}


SYSINFO get_system_information() {
	SYSINFO info;

	SYSTEM_INFO windowsSystemInfo;
	GetSystemInfo(&windowsSystemInfo); //scrie in structura windows 

	MEMORYSTATUSEX memInfo;
	GlobalMemoryStatusEx(&memInfo);//info mem ram

	if (windowsSystemInfo.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_AMD64) {
		strcpy_s(info.architecture, sizeof(info.architecture), "x64");
	}

	else if (windowsSystemInfo.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_INTEL) {
		strcpy_s(info.architecture, sizeof(info.architecture), "x86");
	}

	else if (windowsSystemInfo.wProcessorArchitecture == PROCESSOR_ARCHITECTURE_ARM) {
		strcpy_s(info.architecture, sizeof(info.architecture), "ARM");
	}

	else {
		strcpy_s(info.architecture, sizeof(info.architecture), "Unknown");
	}

	info.logicalProcessors = windowsSystemInfo.dwNumberOfProcessors;
	info.totalRamMb = (unsigned long long)(memInfo.ullTotalPhys / (1024ULL * 1024ULL));

	return info;
}




void cycle_mem_access(int* array, int n) {
	for (int i = 0; i < n; i++) {
		array[i] = i;
	}

	for (int i = n - 1; i > 0; i--) {
		int j = rand() % (i + 1);
		int tmp = array[i];
		array[i] = array[j];
		array[j] = tmp;
	}

	for (int i = 0; i < n - 1; i++) {
		array[array[i]] = array[i + 1];
	}

	array[array[n - 1]] = array[0];
} 



int memory_access(int* array, int n, int steps) {
	int idx = 0;
	for (int i = 0; i < steps; i++) {
		idx = array[idx];
	}
	return idx;
}


double test_memory_random_access(int n, int stepsPerElement) {
	int* array = (int*)malloc(n * sizeof(int));
	if (array == NULL) {
		return -1.0;
	}

	srand((unsigned int)time(NULL));
	cycle_mem_access(array, n);

	int totalSteps = n * stepsPerElement;

	clock_t start = clock();
	memory_access(array, n, totalSteps);
	clock_t end = clock();

	free(array);


	return (double)(end - start) * 1000.0 / CLOCKS_PER_SEC;
}




void gauss_int(int** a, int* b, int n)
{
	int i, j, k;
	int c, sum;
	int* x = (int*)malloc(n * sizeof(int));
	if (x == NULL) return;

	for (j = 0; j < n; j++)
	{
		for (i = 0; i < n; i++)
		{
			if (i > j)
			{
				if (a[j][j] == 0)
					continue;

				c = a[i][j] / a[j][j];

				for (k = 0; k < n; k++)
				{
					a[i][k] = a[i][k] - c * a[j][k];
				}

				b[i] = b[i] - c * b[j];
			}
		}
	}

	x[n - 1] = b[n - 1] / a[n - 1][n - 1];

	for (i = n - 2; i >= 0; i--)
	{
		sum = 0;
		for (j = i + 1; j < n; j++)
		{
			sum = sum + a[i][j] * x[j];
		}
		if (a[i][i] != 0)
			x[i] = (b[i] - sum) / a[i][i];
	}

	for (i = 0; i < n; i++)
		b[i] = x[i];

	free(x);
}



double test_gauss_int_performance(int n)
{
	int** a = (int**)malloc(n * sizeof(int*));
	int* b = (int*)malloc(n * sizeof(int));

	if (a == NULL || b == NULL)
	{
		free(a);
		free(b);
		return -1.0;
	}

	for (int i = 0; i < n; i++)
	{
		a[i] = (int*)malloc(n * sizeof(int));
		if (a[i] == NULL)
		{
			for (int k = 0; k < i; k++)
				free(a[k]);
			free(a);
			free(b);
			return -1.0;
		}
	}

	for (int i = 0; i < n; i++)
	{
		for (int j = 0; j < n; j++)
		{
			if (i == j)
				a[i][j] = n + 5;
			else
				a[i][j] = (i + j) % 7 + 1;
		}
		b[i] = (i * 3 + 1) % 11;
	}

	clock_t start = clock();
	gauss_int(a, b, n);
	clock_t end = clock();

	for (int i = 0; i < n; i++)
		free(a[i]);
	free(a);
	free(b);

	return (double)(end - start) * 1000.0 / CLOCKS_PER_SEC;
}


void jacobi_loop(double* current, double* next, int width, int height) {
	for (int i = 1; i < height - 1; i++) {
		for (int j = 1; j < width - 1; j++) {
			int idx = i * width + j;

			next[idx] =(current[idx - width] + current[idx + width] + current[idx - 1] + current[idx + 1]) / 4.0;
		}
	}
}



double test_floating_numbers_performance(int width, int height, int iterations) {
	int size = width * height;

	double* current = (double*)malloc(size * sizeof(double));
	double* next = (double*)malloc(size * sizeof(double));

	if (current == NULL || next == NULL) {
		free(current);
		free(next);
		return -1.0;
	}

	for (int i = 0; i < size; i++) {
		current[i] = 1.0;
		next[i] = 0.0;
	}

	clock_t start = clock();

	for (int it = 0; it < iterations; it++) {
		jacobi_loop(current, next, width, height);

		double* temp = current;
		current = next;
		next = temp;
	}

	clock_t end = clock();

	free(current);
	free(next);

	return (double)(end - start) * 1000.0 / CLOCKS_PER_SEC;
}



void cpu_get_address_size(int* physBits, int* virtBits) {
	if (!physBits || !virtBits) return;

	ADDRESS_SIZE a = get_address_size();
	*physBits = a.physicalAddressesNumberBits;
	*virtBits = a.virtualAddressesNumberBits;
}




int cpu_get_frequencies(int* baseFreq, int* maxFreq, int* busFreq) {
	if (!baseFreq || !maxFreq || !busFreq) return 0;

	FREQUENCY f = get_frequences();
	*baseFreq = f.baseFrequency;
	*maxFreq = f.maxFrequency;
	*busFreq = f.magistralaFrequency;

	return (*baseFreq || *maxFreq || *busFreq) ? 1 : 0;
}



int cpu_get_cache_all_levels(CACHE_MEMORY* outCaches, int maxCaches) {
	if (!outCaches || maxCaches <= 0) return 0;

	CACHE_MEMORY tmp[MAX_NUMBER_OF_CACHE_LEVELS];
	int cnt = get_cache_at_all_levels(tmp);

	if (cnt > maxCaches) cnt = maxCaches;
	memcpy(outCaches, tmp, cnt * sizeof(CACHE_MEMORY));
	return cnt;
}


void sys_get_information(SYSINFO* info) {
	if (!info) return;
	*info = get_system_information();
}
