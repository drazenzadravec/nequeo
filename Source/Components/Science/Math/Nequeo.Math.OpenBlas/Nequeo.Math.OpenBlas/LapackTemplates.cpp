/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          LapackTemplates.cpp
*  Purpose :       Common Lapack functions.
*
*/

/*
Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#include "stdafx.h"
#include "GlobalOpenBlas.h"
#include "GlobalOpenBlas.cpp"

#include <cstring>

#include "cblas.h"
#include "lapacke.h"

/// <summary>
/// Computes the LU factorization of a general m-by-n matrix. A = P*L*U.
/// where P is a permutation matrix, L is lower triangular with unit diagonal elements (lower trapezoidal if m > n) and U is upper triangular (upper trapezoidal if m < n). The routine uses partial pivoting, with row interchanges.
/// </summary>
/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
/// <param name="a">Array, size at least max(1, lda*n) for column-major layout or max(1, lda*m) for row-major layout. Contains the matrix A.</param>
/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
/// <returns>This function returns a value info.
/// If info=0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// If info = i, uii is 0. The factorization has been completed, but U is exactly singular. Division by 0 will occur if you use the factor U for solving a system of linear equations.
/// </returns>
template<typename T, typename GETRF>
inline lapack_int lu_factor(lapack_int m, T a[], lapack_int ipiv[], GETRF getrf)
{
	auto info = getrf(LAPACK_COL_MAJOR, m, m, a, m, ipiv);
	shift_ipiv_down(m, ipiv);
	return info;
}

/// <summary>
/// Computes the inverse of an LU-factored general matrix.
/// The routine computes the inverse inv(A) of a general matrix A.
/// </summary>
/// <param name="n">The order of the matrix A; n ? 0.</param>
/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
/// <param name="work">The work matrix.</param>
/// <param name="lwork">The work length.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful. 
/// If info = -i, parameter i had an illegal value.
/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
/// </returns>
template<typename T, typename GETRF, typename GETRI>
inline lapack_int lu_inverse(lapack_int n, T a[], GETRF getrf, GETRI getri)
{
	try
	{
		auto ipiv = array_new<lapack_int>(n);
		auto info = getrf(LAPACK_COL_MAJOR, n, n, a, n, ipiv.get());

		if (info != 0)
		{
			return info;
		}

		info = getri(LAPACK_COL_MAJOR, n, a, n, ipiv.get());
		return info;
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Computes the inverse of an LU-factored general matrix.
/// The routine computes the inverse inv(A) of a general matrix A.
/// </summary>
/// <param name="n">The order of the matrix A; n ? 0.</param>
/// <param name="a">Overwritten by the n-by-n matrix inv(A). Array a(size max(1, lda*n)) contains the factorization of the matrix A, as returned by ?getrf: A = P*L*U. The second dimension of a must be at least max(1,n).</param>
/// <param name="ipiv">Array, size at least max(1,min(m, n)). The pivot indices; for 1 ? i ? min(m, n), row i was interchanged with row ipiv(i).</param>
/// <param name="work">The work matrix.</param>
/// <param name="lwork">The work length.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful. 
/// If info = -i, parameter i had an illegal value.
/// If info = i, the i-th diagonal element of the factor U is zero, U is singular, and the inversion could not be completed.
/// </returns>
template<typename T, typename GETRI>
inline lapack_int lu_inverse_factored(lapack_int n, T a[], lapack_int ipiv[], GETRI getri)
{
	shift_ipiv_up(n, ipiv);
	auto info = getri(LAPACK_COL_MAJOR, n, a, n, ipiv);
	shift_ipiv_down(n, ipiv);
	return info;
}

/// <summary>
/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
/// The routine solves for X the following systems of linear equations:
/// A*X = B if trans = 'N',
/// AT*X = B if trans = 'T',
/// AH*X = B if trans = 'C' (for complex matrices only).
/// </summary>
/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
/// <param name="a">Array of size max(1, lda*n).</param>
/// <param name="ipiv">Array, size at least max(1, n).</param>
/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// </returns>
template<typename T, typename GETRS>
inline lapack_int lu_solve_factored(lapack_int n, lapack_int nrhs, T a[], lapack_int ipiv[], T b[], GETRS getrs)
{
	shift_ipiv_up(n, ipiv);
	auto info = getrs(LAPACK_COL_MAJOR, 'N', n, nrhs, a, n, ipiv, b, n);
	shift_ipiv_down(n, ipiv);
	return info;
}

/// <summary>
/// Solves a system of linear equations with an LU-factored square coefficient matrix, with multiple right-hand sides.
/// The routine solves for X the following systems of linear equations:
/// A*X = B if trans = 'N',
/// AT*X = B if trans = 'T',
/// AH*X = B if trans = 'C' (for complex matrices only).
/// </summary>
/// <param name="n">The order of A; the number of rows in B(n ? 0).</param>
/// <param name="nrhs">The number of right-hand sides; nrhs ? 0.</param>
/// <param name="a">Array of size max(1, lda*n).</param>
/// <param name="b">Array of size max(1,ldb*nrhs) for column major layout, and max(1,ldb*n) for row major layout. The array b contains the matrix B whose columns are the right - hand sides for the systems of equations.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// </returns>
template<typename T, typename GETRF, typename GETRS>
inline lapack_int lu_solve(lapack_int n, lapack_int nrhs, T a[], T b[], GETRF getrf, GETRS getrs)
{
	try
	{
		auto clone = array_clone(n * n, a);
		auto ipiv = array_new<lapack_int>(n);
		auto info = getrf(LAPACK_COL_MAJOR, n, n, clone.get(), n, ipiv.get());

		if (info != 0)
		{
			return info;
		}

		return getrs(LAPACK_COL_MAJOR, 'N', n, nrhs, clone.get(), n, ipiv.get(), b, n);
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Computes the Cholesky factorization of a symmetric (Hermitian) positive-definite matrix.
/// The routine forms the Cholesky factorization of a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A:
/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
/// A = L*LT for real data, A = L*LH for complex data  if uplo='L'
/// </summary>
/// <param name="n">The order of matrix A; n ? 0.</param>
/// <param name="a">The upper or lower triangular part of a is overwritten by the Cholesky factor U or L, as specified by uplo. Array, size max(1, lda*n. The array a contains either the upper or the lower triangular part of the matrix A (see uplo).</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// If info = i, the leading minor of order i (and therefore the matrix A itself) is not positive-definite, and the factorization could not be completed. This may indicate an error in forming the matrix A.
/// </returns>
template<typename T, typename POTRF>
inline lapack_int cholesky_factor(lapack_int n, T* a, POTRF potrf)
{
	auto info = potrf(LAPACK_COL_MAJOR, 'L', n, a, n);
	auto zero = T();

	for (auto i = 0; i < n; ++i)
	{
		auto index = i * n;

		for (auto j = 0; j < n && i > j; ++j)
		{
			a[index + j] = zero;
		}
	}

	return info;
}

/// <summary>
/// Solves a system of linear equations with a Cholesky-factored symmetric (Hermitian) positive-definite coefficient matrix.
/// The routine solves for X the system of linear equations A*X = B with a symmetric positive-definite or, for complex data, Hermitian positive-definite matrix A, given the Cholesky factorization of A:
/// A = UT*U for real data, A = UH*U for complex data if uplo='U'
/// A = L*LT for real data, A = L*LH for complex data if uplo='L'
/// </summary>
/// <param name="n">The order of matrix A; n ? 0.</param>
/// <param name="nrhs">The number of right-hand sides (nrhs ? 0).</param>
/// <param name="a">Array A of size at least max(1, lda*n). The array a contains the factor U or L (see uplo)</param>
/// <param name="b">The array b contains the matrix B whose columns are the right-hand sides for the systems of equations. The size of b must be at least max(1, ldb*nrhs) for column major layout and max(1, ldb*n) for row major layout.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// </returns>
template<typename T, typename POTRF, typename POTRS>
inline lapack_int cholesky_solve(lapack_int n, lapack_int nrhs, T a[], T b[], POTRF potrf, POTRS potrs)
{
	try
	{
		auto clone = array_clone(n * n, a);
		auto info = potrf(LAPACK_COL_MAJOR, 'L', n, clone.get(), n);

		if (info != 0)
		{
			return info;
		}

		return potrs(LAPACK_COL_MAJOR, 'L', n, nrhs, clone.get(), n, b, n);
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Computes the QR factorization of a general m-by-n matrix.
/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
/// </summary>
/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
/// <param name="n">The number of columns in A (n ? 0).</param>
/// <param name="r">Overwritten by the factorization data as follows:
/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// </returns>
template<typename T, typename GEQRF, typename ORGQR>
inline lapack_int qr_factor(lapack_int m, lapack_int n, T r[], T tau[], T q[], GEQRF geqrf, ORGQR orgqr)
{
	auto info = geqrf(LAPACK_COL_MAJOR, m, n, r, m, tau);

	for (auto i = 0; i < m; ++i)
	{
		for (auto j = 0; j < m && j < n; ++j)
		{
			if (i > j)
			{
				q[j * m + i] = r[j * m + i];
			}
		}
	}

	if (info != 0)
	{
		return info;
	}

	//compute the q elements explicitly
	if (m <= n)
	{
		info = orgqr(LAPACK_COL_MAJOR, m, m, m, q, m, tau);
	}
	else
	{
		info = orgqr(LAPACK_COL_MAJOR, m, m, n, q, m, tau);
	}

	return info;
}

/// <summary>
/// Computes the QR factorization of a general m-by-n matrix.
/// Generates the real orthogonal matrix Q of the QR factorization formed by ?geqrf.
/// </summary>
/// <param name="m">The number of rows in the matrix A (m ? 0).</param>
/// <param name="n">The number of columns in A (n ? 0).</param>
/// <param name="q">Overwritten by n leading columns of the m-by-m orthogonal matrix Q.</param>
/// <param name="tau">Array, size at least max (1, min(m, n)). Contains scalars that define elementary reflectors for the matrix Qin its decomposition in a product of elementary reflectors (see Orthogonal Factorizations).</param>
/// <param name="r">Overwritten by the factorization data as follows:
/// The elements on and above the diagonal of the array contain the min(m, n) - by - n upper trapezoidal matrix R(R is upper triangular if m ? n); the elements below the diagonal, with the array tau, present the orthogonal matrix Q as a product of min(m, n) elementary reflectors(see Orthogonal Factorizations).
/// Array a of size max(1, lda*n) for column major layout and max(1, lda*m) for row major layout contains the matrix A. </param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// </returns>
template<typename T, typename GEQRF, typename ORGQR>
inline lapack_int qr_thin_factor(lapack_int m, lapack_int n, T q[], T tau[], T r[], GEQRF geqrf, ORGQR orgqr)
{
	auto info = geqrf(LAPACK_COL_MAJOR, m, n, q, m, tau);

	for (auto i = 0; i < n; ++i)
	{
		for (auto j = 0; j < n; ++j)
		{
			if (i <= j)
			{
				r[j * n + i] = q[j * m + i];
			}
		}
	}

	if (info != 0)
	{
		return info;
	}

	info = orgqr(LAPACK_COL_MAJOR, m, n, n, q, m, tau);
	return info;
}

/// <summary>
/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
/// </summary>
/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
/// <param name="n">The number of columns in C (n ? 0).</param>
/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
/// 0 ? k ? m if side ='L';
/// 0 ? k ? n if side ='R'.
/// </param>
/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// </returns>
template<typename T, typename GELS>
inline lapack_int qr_solve(lapack_int m, lapack_int n, lapack_int bn, T a[], T b[], T x[], GELS gels)
{
	try
	{
		auto clone_a = array_clone(m * n, a);
		auto clone_b = array_clone(m * bn, b);
		auto info = gels(LAPACK_COL_MAJOR, 'N', m, n, bn, clone_a.get(), m, clone_b.get(), m);

		if (info != 0)
		{
			return info;
		}

		copyBtoX(m, n, bn, clone_b.get(), x);
		return info;
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
/// </summary>
/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
/// <param name="n">The number of columns in C (n ? 0).</param>
/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
/// 0 ? k ? m if side ='L';
/// 0 ? k ? n if side ='R'.
/// </param>
/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
/// <param name="tau">The size of tau must be at least max(1, k).</param>
/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// </returns>
template<typename T, typename ORMQR, typename TRSM>
inline lapack_int qr_solve_factored(lapack_int m, lapack_int n, lapack_int bn, T r[], T b[], T tau[], T x[], ORMQR ormqr, TRSM trsm)
{
	try
	{
		auto clone_b = array_clone(m * bn, b);
		auto info = ormqr(LAPACK_COL_MAJOR, 'L', 'T', m, bn, n, r, m, tau, clone_b.get(), m);

		if (info != 0)
		{
			return info;
		}

		trsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, 1.0, r, m, clone_b.get(), m);
		copyBtoX(m, n, bn, clone_b.get(), x);
		return info;
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Multiplies a real matrix by the orthogonal matrix Q of the QR factorization formed by ?geqrf or ?geqpf
/// </summary>
/// <param name="m">The number of rows in the matrix C (m ? 0).</param>
/// <param name="n">The number of columns in C (n ? 0).</param>
/// <param name="bn">The number of elementary reflectors whose product defines the matrix Q. Constraints:
/// 0 ? k ? m if side ='L';
/// 0 ? k ? n if side ='R'.
/// </param>
/// <param name="r">The size of a is max(1, lda*k) for column major layout, max(1, lda*m) for row major layout and side = 'L', and max(1, lda*n) for row major layout and side = 'R'.</param>
/// <param name="b">Array c of size max(1, ldc*n) for column major layout and max(1, ldc*m) for row major layout contains the m-by-n matrix C. </param>
/// <param name="tau">The size of tau must be at least max(1, k).</param>
/// <param name="x">Overwritten by the product Q*C, QT*C, C*Q, or C*QT (as specified by side and trans).</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// </returns>
template<typename T, typename R, typename UNMQR, typename TRSM>
inline lapack_int complex_qr_solve_factored(lapack_int m, lapack_int n, lapack_int bn, T r[], T b[], T tau[], T x[], UNMQR unmqr, TRSM trsm)
{
	try
	{
		auto clone_b = array_clone(m * bn, b);
		auto info = unmqr(LAPACK_COL_MAJOR, 'L', 'C', m, bn, n, r, m, tau, clone_b.get(), m);

		if (info != 0)
		{
			return info;
		}

		T one = 1.0f;
		trsm(CblasColMajor, CblasLeft, CblasUpper, CblasNoTrans, CblasNonUnit, n, bn, reinterpret_cast<R*>(&one), reinterpret_cast<R*>(r), m, reinterpret_cast<R*>(clone_b.get()), m);
		copyBtoX(m, n, bn, clone_b.get(), x);
		return info;
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Computes the singular value decomposition of a general rectangular matrix.
/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
/// A = U*?*VT for real routines
/// A = U*?*VH for complex routines
/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
/// </summary>
/// <param name="compute_vectors">True to compute vectors; else false.</param>
/// <param name="m">The number of rows of the matrix A (m ? 0).</param>
/// <param name="n">The number of columns in A (n ? 0).</param>
/// <param name="a">Arrays: a(size at least max(1, lda*n) for column major layout and max(1, lda*m) for row major layout) is an array containing the m - by - n matrix A.</param>
/// <param name="s">Array, size at least max(1, min(m,n)). Contains the singular values of A sorted so that s[i] ? s[i + 1].</param>
/// <param name="u">Array u minimum size: 
/// jobu = 'A' max(1, ldu*m) max(1, ldu*m)
/// jobu = 'S' max(1, ldu*min(m, n)) max(1, ldu*m)
/// If jobu = 'A', u contains the m - by - m orthogonal / unitary matrix U.
/// If jobu = 'S', u contains the first min(m, n) columns of U(the left singular vectors stored column - wise).
/// If jobu = 'N' or 'O', u is not referenced.
/// </param>
/// <param name="v">Array v minimum size: 
/// jobu = 'A' max(1, ldvt*n) max(1, ldvt*n)
/// jobu = 'S' max(1, ldvt*min(m, n)) max(1, ldvt*n)
/// If jobvt = 'A', vt contains the n - by - n orthogonal / unitary matrix VT / VH.
/// If jobvt = 'S', vt contains the first min(m, n) rows of VT / VH(the right singular vectors stored row - wise).
/// If jobvt = 'N' or 'O', vt is not referenced.
/// </param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
/// </returns>
template<typename T, typename GESVD>
inline lapack_int svd_factor(bool compute_vectors, lapack_int m, lapack_int n, T a[], T s[], T u[], T v[], GESVD gesvd)
{
	try
	{
		auto job = compute_vectors ? 'A' : 'N';
		auto dim_s = min(m, n);
		auto superb = array_new<T>(max(2, dim_s) - 1);
		return gesvd(LAPACK_COL_MAJOR, job, job, m, n, a, m, s, u, m, v, n, superb.get());
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Computes the singular value decomposition of a general rectangular matrix.
/// The routine computes the singular value decomposition (SVD) of a real/complex m-by-n matrix A, optionally computing the left and/or right singular vectors. The SVD is written as
/// A = U*?*VT for real routines
/// A = U*?*VH for complex routines
/// where ? is an m - by - n matrix which is zero except for its min(m, n) diagonal elements, U is an m - by - m orthogonal / unitary matrix, and V is an n - by - n orthogonal / unitary matrix.The diagonal elements of ? are the singular values of A; they are real and non - negative, and are returned in descending order.The first min(m, n) columns of U and V are the left and right singular vectors of A.
/// Note that the routine returns VT(for real flavors) or VH(for complex flavors), not V.
/// </summary>
/// <param name="compute_vectors">True to compute vectors; else false.</param>
/// <param name="m">The number of rows of the matrix A (m ? 0).</param>
/// <param name="n">The number of columns in A (n ? 0).</param>
/// <param name="a">Arrays: a(size at least max(1, lda*n) for column major layout and max(1, lda*m) for row major layout) is an array containing the m - by - n matrix A.</param>
/// <param name="s">Array, size at least max(1, min(m,n)). Contains the singular values of A sorted so that s[i] ? s[i + 1].</param>
/// <param name="u">Array u minimum size: 
/// jobu = 'A' max(1, ldu*m) max(1, ldu*m)
/// jobu = 'S' max(1, ldu*min(m, n)) max(1, ldu*m)
/// If jobu = 'A', u contains the m - by - m orthogonal / unitary matrix U.
/// If jobu = 'S', u contains the first min(m, n) columns of U(the left singular vectors stored column - wise).
/// If jobu = 'N' or 'O', u is not referenced.
/// </param>
/// <param name="v">Array v minimum size: 
/// jobu = 'A' max(1, ldvt*n) max(1, ldvt*n)
/// jobu = 'S' max(1, ldvt*min(m, n)) max(1, ldvt*n)
/// If jobvt = 'A', vt contains the n - by - n orthogonal / unitary matrix VT / VH.
/// If jobvt = 'S', vt contains the first min(m, n) rows of VT / VH(the right singular vectors stored row - wise).
/// If jobvt = 'N' or 'O', vt is not referenced.
/// </param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
/// </returns>
template<typename T, typename R, typename GESVD>
inline lapack_int complex_svd_factor(bool compute_vectors, lapack_int m, lapack_int n, T a[], T s[], T u[], T v[], GESVD gesvd)
{
	try
	{
		auto dim_s = min(m, n);
		auto s_local = array_new<R>(dim_s);
		auto superb = array_new<R>(max(2, dim_s) - 1);
		auto job = compute_vectors ? 'A' : 'N';
		auto info = gesvd(LAPACK_COL_MAJOR, job, job, m, n, a, m, s_local.get(), u, m, v, n, superb.get());

		for (auto index = 0; index < dim_s; ++index)
		{
			s[index] = s_local.get()[index];
		}

		return info;
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Eigensolver interface for standard eigenvalue problem with dense matrices.
/// The routines compute all the eigenvalues and eigenvectors for standard eigenvalue problems, Ax = ?x, within a given search interval.
/// </summary>
/// <param name="isSymmetric">True if is symmetric; else false.</param>
/// <param name="n">Sets the size of the problem. n > 0.</param>
/// <param name="a">Array of dimension lda by n, contains either full matrix A or upper or lower triangular part of the matrix A, as specified by uplo.</param>
/// <param name="vectors">Vectors (size max(1, ldvr*mm) for column major layout and max(1, ldvr*n) for row major layout) 
/// If howmny = 'B' and side = 'R' or 'B', then vr must contain an n - by - n matrix Q(usually the matrix of Schur vectors returned by ? hseqr). .
/// If howmny = 'A' or 'S', then vr need not be set.
/// The array vr is not referenced if side = 'L'.
/// </param>
/// <param name="values">The result values.</param>
/// <param name="d">The resulting eigen factors.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
/// </returns>
template<typename T, typename R, typename GEES, typename TREVC>
inline lapack_int eigen_factor(lapack_int n, T a[], T vectors[], R values[], T d[], GEES gees, TREVC trevc)
{
	try
	{
		auto clone_a = array_clone(n * n, a);
		auto wr = array_new<T>(n);
		auto wi = array_new<T>(n);

		lapack_int sdim;
		lapack_int info = gees(LAPACK_COL_MAJOR, 'V', 'N', nullptr, n, clone_a.get(), n, &sdim, wr.get(), wi.get(), vectors, n);
		if (info != 0)
		{
			return info;
		}

		lapack_int m;
		info = trevc(LAPACK_COL_MAJOR, 'R', 'B', nullptr, n, clone_a.get(), n, nullptr, n, vectors, n, n, &m);
		if (info != 0)
		{
			return info;
		}

		for (auto index = 0; index < n; ++index)
		{
			values[index] = R(wr.get()[index], wi.get()[index]);
		}

		for (auto i = 0; i < n; ++i)
		{
			auto in = i * n;
			d[in + i] = wr.get()[i];

			if (wi.get()[i] > 0)
			{
				d[in + n + i] = wi.get()[i];
			}
			else if (wi.get()[i] < 0)
			{
				d[in - n + i] = wi.get()[i];
			}
		}
		return info;
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Eigensolver interface for standard eigenvalue problem with dense matrices.
/// The routines compute all the eigenvalues and eigenvectors for standard eigenvalue problems, Ax = ?x, within a given search interval.
/// </summary>
/// <param name="isSymmetric">True if is symmetric; else false.</param>
/// <param name="n">Sets the size of the problem. n > 0.</param>
/// <param name="a">Array of dimension lda by n, contains either full matrix A or upper or lower triangular part of the matrix A, as specified by uplo.</param>
/// <param name="vectors">Vectors (size max(1, ldvr*mm) for column major layout and max(1, ldvr*n) for row major layout) 
/// If howmny = 'B' and side = 'R' or 'B', then vr must contain an n - by - n matrix Q(usually the matrix of Schur vectors returned by ? hseqr). .
/// If howmny = 'A' or 'S', then vr need not be set.
/// The array vr is not referenced if side = 'L'.
/// </param>
/// <param name="values">The result values.</param>
/// <param name="d">The resulting eigen factors.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
/// </returns>
template<typename T, typename GEES, typename TREVC>
inline lapack_int eigen_complex_factor(lapack_int n, T a[], T vectors[], lapack_complex_double values[], T d[], GEES gees, TREVC trevc)
{
	try
	{
		auto clone_a = array_clone(n * n, a);
		auto w = array_new<T>(n);

		lapack_int sdim;
		lapack_int info = gees(LAPACK_COL_MAJOR, 'V', 'N', nullptr, n, clone_a.get(), n, &sdim, w.get(), vectors, n);
		if (info != 0)
		{
			return info;
		}

		lapack_int m;
		info = trevc(LAPACK_COL_MAJOR, 'R', 'B', nullptr, n, clone_a.get(), n, nullptr, n, vectors, n, n, &m);
		if (info != 0)
		{
			return info;
		}

		for (auto i = 0; i < n; ++i)
		{
			values[i] = w.get()[i];
			d[i * n + i] = w.get()[i];
		}

		return info;
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}

/// <summary>
/// Eigensolver interface for standard eigenvalue problem with dense matrices.
/// The routines compute all the eigenvalues and eigenvectors for standard eigenvalue problems, Ax = ?x, within a given search interval.
/// </summary>
/// <param name="isSymmetric">True if is symmetric; else false.</param>
/// <param name="n">Sets the size of the problem. n > 0.</param>
/// <param name="a">Array of dimension lda by n, contains either full matrix A or upper or lower triangular part of the matrix A, as specified by uplo.</param>
/// <param name="vectors">Vectors (size max(1, ldvr*mm) for column major layout and max(1, ldvr*n) for row major layout) 
/// If howmny = 'B' and side = 'R' or 'B', then vr must contain an n - by - n matrix Q(usually the matrix of Schur vectors returned by ? hseqr). .
/// If howmny = 'A' or 'S', then vr need not be set.
/// The array vr is not referenced if side = 'L'.
/// </param>
/// <param name="values">The result values.</param>
/// <param name="d">The resulting eigen factors.</param>
/// <returns>This function returns a value info.
/// If info = 0, the execution is successful.
/// If info = -i, parameter i had an illegal value.
/// If info = i, then if ?bdsqr did not converge, i specifies how many superdiagonals of the intermediate bidiagonal form B did not converge to zero (see the description of the superb parameter for details).
/// </returns>
template<typename R, typename T, typename SYEV>
inline lapack_int sym_eigen_factor(lapack_int n, T a[], T vectors[], lapack_complex_double values[], T d[], SYEV syev)
{
	try
	{
		auto clone_a = array_clone(n * n, a);
		auto w = array_new<R>(n);

		lapack_int info = syev(LAPACK_COL_MAJOR, 'V', 'U', n, clone_a.get(), n, w.get());
		if (info != 0)
		{
			return info;
		}

		memcpy(vectors, clone_a.get(), n*n * sizeof(T));

		for (auto index = 0; index < n; ++index)
		{
			values[index] = lapack_complex_double(w.get()[index]);
		}

		for (auto j = 0; j < n; ++j)
		{
			auto jn = j*n;

			for (auto i = 0; i < n; ++i)
			{
				if (i == j)
				{
					d[jn + i] = w.get()[i];
				}
			}
		}

		return info;
	}
	catch (std::bad_alloc&)
	{
		return INSUFFICIENT_MEMORY;
	}
}