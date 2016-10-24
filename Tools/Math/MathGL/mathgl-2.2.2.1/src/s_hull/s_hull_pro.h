#ifndef _structures_h
#define _structures_h




// for FILE

#include <stdlib.h>
#include <vector>
#include <set>



/*
   for use in s_hull_pro.cpp

S-hull-pro, Copyright (c) 2012
Dr David SInclair
Cambridge, UK

email david@s-hull.org


*/

// Global replace int->long by A.Balakin 7 August 2012 -- 64bit version can handle huge data arrays

struct Triad
{
	long a,b, c;
	long ab, bc, ac;  // adjacent edges index to neighbouring triangle.
	float ro, R,C;
	//std::set<long> idx;
	Triad() {a=b=c=ab=bc=ac=0;	ro=R=C=0;}	// added by A.Balakin 6 July 2012 -- uninitialised variable
	Triad(long x, long y) : a(x), b(y),c(0), ab(-1), bc(-1), ac(-1), ro(-1), R(0), C(0) {}
	Triad(long x, long y, long z) : a(x), b(y), c(z),  ab(-1), bc(-1), ac(-1), ro(-1), R(0), C(0) {}
	Triad(const Triad &p) : a(p.a), b(p.b), c(p.c), ab(p.ab), bc(p.bc), ac(p.ac), ro(p.ro), R(p.R), C(p.C) {}

	Triad &operator=(const Triad &p)
	{
		a = p.a;
		b = p.b;
		c = p.c;

		ab = p.ab;
		bc = p.bc;
		ac = p.ac;

		ro = p.ro;
		R = p.R;
		C = p.C;

		return *this;
	}
};



/* point structure for s_hull only.
   has to keep track of triangle ids as hull evolves.
*/


struct Shx
{
	long id, trid;
	float r,c, tr,tc ;
	float ro;
	Shx() {r=c=tr=tc=ro=0;	id=trid=0;}	// added by A.Balakin 6 July 2012 -- uninitialised variable
	Shx(float a, float b) : id(-1), r(a), c(b), tr(0.0), tc(0.0), ro(0.0)
	{	trid=0;	}		// added by A.Balakin 6 July 2012 -- uninitialised variable
	Shx(float a, float b, float x) : id(-1), r(a), c(b), tr(0), tc(0), ro(x)
	{	trid=0;	}	// added by A.Balakin 6 July 2012 -- uninitialised variable
	Shx(const Shx &p) : id(p.id), trid(p.trid), r(p.r), c(p.c), tr(p.tr), tc(p.tc), ro(p.ro) {}

	Shx &operator=(const Shx &p)
	{
		id = p.id;
		trid = p.trid;
		r = p.r;
		c = p.c;
		tr = p.tr;
		tc = p.tc;
		ro = p.ro;
		return *this;
	}

};


// sort into descending order (for use in corner responce ranking).
inline bool operator<(const Shx &a, const Shx &b)
{
	if( a.ro == b.ro)
		return a.r < b.r;
	return a.ro <  b.ro;
}


struct Dupex
{
	long id;
	float r,c;

	Dupex() {}
	Dupex(float a, float b) : id(-1), r(a), c(b) {}
	Dupex(float a, float b, long x) : id(x), r(a), c(b) {}
	Dupex(const Dupex &p) : id(p.id),  r(p.r), c(p.c) {}

	Dupex &operator=(const Dupex &p)
	{
		id = p.id;
		r = p.r;
		c = p.c;
		return *this;
	}
};



// sort into descending order (for use in corner responce ranking).
inline bool operator<(const Dupex &a, const Dupex &b)
{
	if( a.r == b.r)
		return a.c < b.c;
	return a.r <  b.r;
}





// from s_hull.C


long s_hull_pro( std::vector<Shx> &pts, std::vector<Triad> &triads);
void circle_cent2(float r1,float c1, float r2,float c2, float r3,float c3,float &r,float &c, float &ro2);
void circle_cent4(float r1,float c1, float r2,float c2, float r3,float c3,float &r,float &c, float &ro2);
void write_Shx(std::vector<Shx> &pts, char * fname);
void write_Triads(std::vector<Triad> &ts, char * fname);
long Cline_Renka_test(float &Ax, float &Ay, float &Bx, float &By, float &Cx, float &Cy, float &Dx, float &Dy);
long T_flip_pro( std::vector<Shx> &pts, std::vector<Triad> &triads, std::vector<long> &slump, long numt, long start, std::set<long> &ids);
long T_flip_pro_idx( std::vector<Shx> &pts, std::vector<Triad> &triads, std::vector<long> &slump, std::set<long> &ids);

long read_Shx(std::vector<Shx> &pts, char * fname);


size_t de_duplicate( std::vector<Shx> &pts,  std::vector<size_t> &outx );

#endif
