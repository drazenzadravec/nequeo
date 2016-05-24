// TestACMLOpenMP.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <windows.h>
#include <assert.h>
#include <math.h>
#include <omp.h>

//int main()
//{
//	#pragma omp parallel num_threads(50)
//	{
//		int i = omp_get_thread_num();
//		printf_s("Hello from thread %d\n", i);
//	}
//
//    return 0;
//}





//int main() {
//	#pragma omp parallel num_threads(2)
//	{
//		#pragma omp single
//		// Only a single thread can read the input.
//		printf_s("read input\n");
//
//		// Multiple threads in the team compute the results.
//		printf_s("compute results\n");
//
//		#pragma omp single
//		// Only a single thread can write the output.
//		printf_s("write output\n");
//	}
//
//    return 0;
//}




#define NUM_THREADS 4
#define NUM_START 1
#define NUM_END 10

int main() {
	int i, nRet = 0, nSum = 0, nStart = NUM_START, nEnd = NUM_END;
	int nThreads = 0, nTmp = nStart + nEnd;
	unsigned uTmp = (unsigned((abs(nStart - nEnd) + 1)) *
		unsigned(abs(nTmp))) / 2;
	int nSumCalc = uTmp;

	if (nTmp < 0)
		nSumCalc = -nSumCalc;

	omp_set_num_threads(NUM_THREADS);

	#pragma omp parallel default(none) private(i) shared(nSum, nThreads, nStart, nEnd)
	{
		#pragma omp master
		nThreads = omp_get_num_threads();

		#pragma omp for
		for (i = nStart; i <= nEnd; ++i) 
		{
			#pragma omp atomic
			nSum += i;
		}
	}

	if (nThreads == NUM_THREADS) {
		printf_s("%d OpenMP threads were used.\n", NUM_THREADS);
		nRet = 0;
	}
	else {
		printf_s("Expected %d OpenMP threads, but %d were used.\n",
			NUM_THREADS, nThreads);
		nRet = 1;
	}

	if (nSum != nSumCalc) {
		printf_s("The sum of %d through %d should be %d, "
			"but %d was reported!\n",
			NUM_START, NUM_END, nSumCalc, nSum);
		nRet = 1;
	}
	else
		printf_s("The sum of %d through %d is %d\n",
			NUM_START, NUM_END, nSum);

	return 0;
}






//#define NUM_THREADS 4
//#define SUM_START   1
//#define SUM_END     10
//#define FUNC_RETS   {1, 1, 1, 1, 1}
//
//int bRets[5] = FUNC_RETS;
//int nSumCalc = ((SUM_START + SUM_END) * (SUM_END - SUM_START + 1)) / 2;
//
//int func1() { return bRets[0]; }
//int func2() { return bRets[1]; }
//int func3() { return bRets[2]; }
//int func4() { return bRets[3]; }
//int func5() { return bRets[4]; }
//
//int main()
//{
//	int nRet = 0,
//		nCount = 0,
//		nSum = 0,
//		i,
//		bSucceed = 1;
//
//	omp_set_num_threads(NUM_THREADS);
//
//#pragma omp parallel reduction(+ : nCount)
//	{
//		nCount += 1;
//
//#pragma omp for reduction(+ : nSum)
//		for (i = SUM_START; i <= SUM_END; ++i)
//			nSum += i;
//
//#pragma omp sections reduction(&& : bSucceed)
//		{
//#pragma omp section
//			{
//				bSucceed = bSucceed && func1();
//			}
//
//#pragma omp section
//			{
//				bSucceed = bSucceed && func2();
//			}
//
//#pragma omp section
//			{
//				bSucceed = bSucceed && func3();
//			}
//
//#pragma omp section
//			{
//				bSucceed = bSucceed && func4();
//			}
//
//#pragma omp section
//			{
//				bSucceed = bSucceed && func5();
//			}
//		}
//	}
//
//	printf_s("The parallel section was executed %d times "
//		"in parallel.\n", nCount);
//	printf_s("The sum of the consecutive integers from "
//		"%d to %d, is %d\n", 1, 10, nSum);
//
//	if (bSucceed)
//		printf_s("All of the the functions, func1 through "
//			"func5 succeeded!\n");
//	else
//		printf_s("One or more of the the functions, func1 "
//			"through func5 failed!\n");
//
//	if (nCount != NUM_THREADS)
//	{
//		printf_s("ERROR: For %d threads, %d were counted!\n",
//			NUM_THREADS, nCount);
//		nRet |= 0x1;
//	}
//
//	if (nSum != nSumCalc)
//	{
//		printf_s("ERROR: The sum of %d through %d should be %d, "
//			"but %d was reported!\n",
//			SUM_START, SUM_END, nSumCalc, nSum);
//		nRet |= 0x10;
//	}
//
//	if (bSucceed != (bRets[0] && bRets[1] &&
//		bRets[2] && bRets[3] && bRets[4]))
//	{
//		printf_s("ERROR: The sum of %d through %d should be %d, "
//			"but %d was reported!\n",
//			SUM_START, SUM_END, nSumCalc, nSum);
//		nRet |= 0x100;
//	}
//	return 0;
//}







//#define NUM_THREADS 4
//#define SLEEP_THREAD 1
//#define NUM_LOOPS 2
//
//enum Types {
//	ThreadPrivate,
//	Private,
//	FirstPrivate,
//	LastPrivate,
//	Shared,
//	MAX_TYPES
//};
//
//int nSave[NUM_THREADS][MAX_TYPES][NUM_LOOPS] = { { 0 } };
//int nThreadPrivate;
//
//#pragma omp threadprivate(nThreadPrivate)
//#pragma warning(disable:4700)
//
//
//int main()
//{
//	int nPrivate = NUM_THREADS;
//	int nFirstPrivate = NUM_THREADS;
//	int nLastPrivate = NUM_THREADS;
//	int nShared = NUM_THREADS;
//	int nRet = 0;
//	int i;
//	int j;
//	int nLoop = 0;
//
//	nThreadPrivate = NUM_THREADS;
//	printf_s("These are the variables before entry "
//		"into the parallel region.\n");
//	printf_s("nThreadPrivate = %d\n", nThreadPrivate);
//	printf_s("      nPrivate = %d\n", nPrivate);
//	printf_s(" nFirstPrivate = %d\n", nFirstPrivate);
//	printf_s("  nLastPrivate = %d\n", nLastPrivate);
//	printf_s("       nShared = %d\n\n", nShared);
//	omp_set_num_threads(NUM_THREADS);
//
//#pragma omp parallel copyin(nThreadPrivate) private(nPrivate) shared(nShared) firstprivate(nFirstPrivate)
//	{
//#pragma omp for schedule(static) lastprivate(nLastPrivate)
//		for (i = 0; i < NUM_THREADS; ++i) {
//			for (j = 0; j < NUM_LOOPS; ++j) {
//				int nThread = omp_get_thread_num();
//				assert(nThread < NUM_THREADS);
//
//				if (nThread == SLEEP_THREAD)
//					Sleep(100);
//				nSave[nThread][ThreadPrivate][j] = nThreadPrivate;
//				nSave[nThread][Private][j] = nPrivate;
//				nSave[nThread][Shared][j] = nShared;
//				nSave[nThread][FirstPrivate][j] = nFirstPrivate;
//				nSave[nThread][LastPrivate][j] = nLastPrivate;
//				nThreadPrivate = nThread;
//				nPrivate = nThread;
//				nShared = nThread;
//				nLastPrivate = nThread;
//				--nFirstPrivate;
//			}
//		}
//	}
//
//	for (i = 0; i < NUM_LOOPS; ++i) {
//		for (j = 0; j < NUM_THREADS; ++j) {
//			printf_s("These are the variables at entry of "
//				"loop %d of thread %d.\n", i + 1, j);
//			printf_s("nThreadPrivate = %d\n",
//				nSave[j][ThreadPrivate][i]);
//			printf_s("      nPrivate = %d\n",
//				nSave[j][Private][i]);
//			printf_s(" nFirstPrivate = %d\n",
//				nSave[j][FirstPrivate][i]);
//			printf_s("  nLastPrivate = %d\n",
//				nSave[j][LastPrivate][i]);
//			printf_s("       nShared = %d\n\n",
//				nSave[j][Shared][i]);
//		}
//	}
//
//	printf_s("These are the variables after exit from "
//		"the parallel region.\n");
//	printf_s("nThreadPrivate = %d (The last value in the "
//		"master thread)\n", nThreadPrivate);
//	printf_s("      nPrivate = %d (The value prior to "
//		"entering parallel region)\n", nPrivate);
//	printf_s(" nFirstPrivate = %d (The value prior to "
//		"entering parallel region)\n", nFirstPrivate);
//	printf_s("  nLastPrivate = %d (The value from the "
//		"last iteration of the loop)\n", nLastPrivate);
//	printf_s("       nShared = %d (The value assigned, "
//		"from the delayed thread, %d)\n\n",
//		nShared, SLEEP_THREAD);
//
//
//    return 0;
//}