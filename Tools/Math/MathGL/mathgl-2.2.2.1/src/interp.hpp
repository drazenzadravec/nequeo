//-----------------------------------------------------------------------------
template <class Treal> void mglFillP(long x,long y, const Treal *a,long nx,long ny,Treal _p[4][4])
{
	Treal sx[4]={0,0,0,0},sy[4]={0,0,0,0},f[4]={0,0,0,0},d[4]={0,0,0,0};
	if(x<0 || y<0 || x>nx-2 || y>ny-2)
	{
		memset(_p[0],0,4*sizeof(Treal));
		memset(_p[1],0,4*sizeof(Treal));
		memset(_p[2],0,4*sizeof(Treal));
		memset(_p[3],0,4*sizeof(Treal));
		return;
	}
	// �������� �������
	f[0]=a[x+nx*y];		f[1]=a[x+nx*(y+1)];
	if(nx>1)	{	f[2]=a[x+1+nx*y];	f[3]=a[x+1+nx*(y+1)];	}
	else		{	f[2] = f[0];	f[3] = f[1];	}
	// ����������� �� x
	if(nx>1)
	{
		if(x==0)
		{
			sx[0]=a[x+1+y*nx]-a[x+nx*y];
			sx[1]=a[x+1+nx*(y+1)]-a[x+nx*(y+1)];
		}
		else
		{
			sx[0]=(a[x+1+nx*y]-a[x-1+nx*y])/mreal(2);
			sx[1]=(a[x+1+nx*(y+1)]-a[x-1+nx*(y+1)])/mreal(2);
		}
	}
	if(x==nx-2)
	{
		sx[2]=a[x+1+nx*y]-a[x+nx*y];
		sx[3]=a[x+1+nx*(y+1)]-a[x+nx*(y+1)];
	}
	else
	{
		sx[2]=(a[x+2+nx*y]-a[x+nx*y])/mreal(2);
		sx[3]=(a[x+2+nx*(y+1)]-a[x+nx*(y+1)])/mreal(2);
	}
	// ����������� �� y
	if(y==0)
	{
		sy[0]=a[x+nx*(y+1)]-a[x+nx*y];
		sy[2]=a[x+1+nx*(y+1)]-a[x+1+nx*y];
	}
	else
	{
		sy[0]=(a[x+nx*(y+1)]-a[x+nx*(y-1)])/mreal(2);
		sy[2]=(a[x+1+nx*(y+1)]-a[x+1+nx*(y-1)])/mreal(2);
	}
	if(y==ny-2)
	{
		sy[1]=a[x+nx*(y+1)]-a[x+nx*y];
		sy[3]=a[x+1+nx*(y+1)]-a[x+1+nx*y];
	}
	else
	{
		sy[1]=(a[x+nx*(y+2)]-a[x+nx*y])/mreal(2);
		sy[3]=(a[x+1+nx*(y+2)]-a[x+1+nx*y])/mreal(2);
	}
	// ������������ �����������
	if(nx>1)
	{
		// ������ d[0]
		if(y==0 && x==0)
			d[0]=(a[x+1+nx*(y+1)]-a[x+nx*(y+1)]-a[x+1+nx*y]+a[x+nx*y]);
		else if(y==0)
			d[0]=(a[x+1+nx*(y+1)]-a[x-1+nx*(y+1)]-a[x+1+nx*y]+a[x-1+nx*y])/mreal(2);
		else if(x==0)
			d[0]=(a[x+1+nx*(y+1)]-a[x+nx*(y+1)]-a[x+1+nx*(y-1)]+a[x+nx*(y-1)])/mreal(2);
		else
			d[0]=(a[x+1+nx*(y+1)]-a[x-1+nx*(y+1)]-a[x+1+nx*(y-1)]+a[x-1+nx*(y-1)])/mreal(4);
		// ������ d[1]
		if(y==ny-2 && x==0)
			d[1]=(a[x+1+nx*(y+1)]-a[x+nx*(y+1)]-a[x+1+nx*y]+a[x+nx*y]);
		else if(y==ny-2)
			d[1]=(a[x+1+nx*(y+1)]-a[x-1+nx*(y+1)]-a[x+1+nx*y]+a[x-1+nx*y])/mreal(2);
		else if(x==0)
			d[1]=(a[x+1+nx*(y+2)]-a[x+nx*(y+2)]-a[x+1+nx*y]+a[x+nx*y])/mreal(2);
		else
			d[1]=(a[x+1+nx*(y+2)]-a[x-1+nx*(y+2)]-a[x+1+nx*y]+a[x-1+nx*y])/mreal(4);
		// ������ d[2]
		if(y==0 && x==nx-2)
			d[2]=(a[x+1+nx*(y+1)]-a[x+nx*(y+1)]-a[x+1+nx*y]+a[x+nx*y]);
		else if(y==0)
			d[2]=(a[x+2+nx*(y+1)]-a[x+nx*(y+1)]-a[x+2+nx*y]+a[x+nx*y])/mreal(2);
		else if(x==nx-2)
			d[2]=(a[x+1+nx*(y+1)]-a[x+nx*(y+1)]-a[x+1+nx*(y-1)]+a[x+nx*(y-1)])/mreal(2);
		else
			d[2]=(a[x+2+nx*(y+1)]-a[x+nx*(y+1)]-a[x+2+nx*(y-1)]+a[x+nx*(y-1)])/mreal(4);
		// ������ d[3]
		if(y==ny-2 && x==nx-2)
			d[3]=(a[x+1+nx*(y+1)]-a[x+nx*(y+1)]-a[x+1+nx*y]+a[x+nx*y]);
		else if(y==ny-2)
			d[3]=(a[x+2+nx*(y+1)]-a[x+nx*(y+1)]-a[x+2+nx*y]+a[x+nx*y])/mreal(2);
		else if(x==nx-2)
			d[3]=(a[x+1+nx*(y+2)]-a[x+nx*(y+2)]-a[x+1+nx*y]+a[x+nx*y])/mreal(2);
		else
			d[3]=(a[x+2+nx*(y+2)]-a[x+nx*(y+2)]-a[x+2+nx*y]+a[x+nx*y])/mreal(4);
	}
	// ��������� ������������ ��������
	_p[0][0]=f[0];		_p[1][0]=sx[0];
	_p[2][0]=mreal(3)*(f[2]-f[0])-mreal(2)*sx[0]-sx[2];
	_p[3][0]=sx[0]+sx[2]+mreal(2)*(f[0]-f[2]);
	_p[0][1]=sy[0];		_p[1][1]=d[0];
	_p[2][1]=mreal(3)*(sy[2]-sy[0])-mreal(2)*d[0]-d[2];
	_p[3][1]=d[0]+d[2]+mreal(2)*(sy[0]-sy[2]);
	_p[0][2]=mreal(3)*(f[1]-f[0])-mreal(2)*sy[0]-sy[1];
	_p[1][2]=mreal(3)*(sx[1]-sx[0])-mreal(2)*d[0]-d[1];
	_p[2][2]=mreal(9)*(f[0]-f[1]-f[2]+f[3])+mreal(6)*(sy[0]-sy[2]+sx[0]-sx[1])+
		mreal(3)*(sx[2]-sx[3]+sy[1]-sy[3])+mreal(2)*(d[1]+d[2])+mreal(4)*d[0]+d[3];
	_p[3][2]=mreal(6)*(f[1]+f[2]-f[0]-f[3])+mreal(3)*(sx[1]-sx[0]+sx[3]-sx[2])+
		mreal(4)*(sy[2]-sy[0])+mreal(2)*(sy[3]-sy[1]-d[0]-d[2])-d[1]-d[3];
	_p[0][3]=mreal(2)*(f[0]-f[1])+sy[0]+sy[1];
	_p[1][3]=mreal(2)*(sx[0]-sx[1])+d[0]+d[1];
	_p[2][3]=mreal(6)*(f[1]+f[2]-f[0]-f[3])+mreal(3)*(sy[2]-sy[1]+sy[3]-sy[0])+
		mreal(4)*(sx[1]-sx[0])+mreal(2)*(sx[3]-sx[2]-d[0]-d[1])-d[2]-d[3];
	_p[3][3]=d[0]+d[1]+d[2]+d[3]+mreal(4)*(f[0]-f[1]-f[2]+f[3])+
		mreal(2)*(sx[0]-sx[1]+sx[2]-sx[3]+sy[0]-sy[2]+sy[1]-sy[3]);
}
//-----------------------------------------------------------------------------
template <class Treal> void mglFillP(long x, const Treal *a,long nx,Treal _p[4])
{
	if(x<0 || x>nx-2)
	{
		memset(_p,0,4*sizeof(Treal));
		return;
	}
	Treal s[2],f[2];
	// �������� �������
	f[0]=a[x];		f[1]=a[x+1];
	// ����������� �� x
	if(x==0)	s[0]=a[x+1]-a[x];
	else		s[0]=(a[x+1]-a[x-1])/mreal(2);
	if(x==nx-2)	s[1]=a[x+1]-a[x];
	else		s[1]=(a[x+2]-a[x])/mreal(2);
	// ��������� ������������ ��������
	_p[0]=f[0];		_p[1]=s[0];
	_p[2]=mreal(3)*(f[1]-f[0])-mreal(2)*s[0]-s[1];
	_p[3]=s[0]+s[1]+mreal(2)*(f[0]-f[1]);
}
//-----------------------------------------------------------------------------
template <class Treal> Treal mglLineart(const Treal *a, long nx, long ny, long nz, mreal x, mreal y, mreal z)
{
	if(!a || nx<1 || ny<1 || nz<1)	return 0;
	register long i0;
	long kx,ky,kz;
	Treal b=0,dx,dy,dz,b1,b0;
	if(x<0 || y<0 || z<0 || x>nx-1 || y>ny-1 || z>nz-1)
		return 0;
	if(nz>1 && z!=floor(z))		// 3d interpolation
	{
		kx=long(x);	ky=long(y);	kz=long(z);
		dx = x-mreal(kx);	dy = y-mreal(ky);	dz = z-mreal(kz);

		i0 = kx+nx*(ky+ny*kz);
		b0 = a[i0]*(mreal(1)-dx-dy+dx*dy) + dx*(mreal(1)-dy)*a[i0+1] +
			dy*(mreal(1)-dx)*a[i0+nx] + dx*dy*a[i0+nx+1];
		i0 = kx+nx*(ky+ny*(kz+1));
		b1 = a[i0]*(mreal(1)-dx-dy+dx*dy) + dx*(mreal(1)-dy)*a[i0+1] +
			dy*(mreal(1)-dx)*a[i0+nx] + dx*dy*a[i0+nx+1];
		b = b0 + dz*(b1-b0);
	}
	else if(ny>1 && y!=floor(y))	// 2d interpolation
	{
		kx=long(x);	ky=long(y);
		dx = x-kx;	dy=y-ky;
		i0 = kx+nx*ky;
		b = a[i0]*(mreal(1)-dx-dy+dx*dy) + dx*(mreal(1)-dy)*a[i0+1] +
			dy*(mreal(1)-dx)*a[i0+nx] + dx*dy*a[i0+nx+1];
	}
	else if(nx>1 && x!=floor(x))	// 1d interpolation
	{
		kx = long(x);
		b = a[kx] + (x-kx)*(a[kx+1]-a[kx]);
	}
	else						// no interpolation
		b = a[long(x+nx*(y+ny*z))];
	return b;
}
//-----------------------------------------------------------------------------
template <class Treal> Treal mglSpline3t(const Treal *a, long nx, long ny, long nz, mreal x, mreal y, mreal z, Treal *dx=0, Treal *dy=0, Treal *dz=0)
{
	if(!a || nx<1 || ny<1 || nz<1)	return 0;
	Treal _p[4][4];
	register long i,j;
	Treal fx=1, fy=1;
	long kx=long(x),ky=long(y),kz=long(z);
	Treal b=0;
	x = x>0 ?(x<nx-1 ? x:nx-1):0;
	y = y>0 ?(y<ny-1 ? y:ny-1):0;
	z = z>0 ?(z<nz-1 ? z:nz-1):0;
	//	if(x<0 || y<0 || z<0 || x>nx-1 || y>ny-1 || z>nz-1)		return 0;
	if(dx)	*dx=0;	if(dy)	*dy=0;	if(dz)	*dz=0;
	if(kx>nx-2)	kx = nx-2;	if(kx<0) 	kx = 0;
	if(ky>ny-2)	ky = ny-2;	if(ky<0) 	ky = 0;
	if(kz>nz-2)	kz = nz-2;	if(kz<0) 	kz = 0;
//	if(nz>1 && z!=kz)		// 3d interpolation
	if(nz>1)		// 3d interpolation
	{
		Treal b1[4]={0,0,0,0},  x1[4]={0,0,0,0},  y1[4]={0,0,0,0};
		long kk=1;
		if(kz==0)	{	kk=0;	}
		else if(nz>3 && kz==nz-2)	{	kk=2;	}
		for(long k=0;k<4;k++)
		{
			if(kz+k-kk<nz && kz+k-kk>=0)
				mglFillP(kx, ky, a+(kz+k-kk)*nx*ny, nx, ny, _p);
			else
			{
				memset(_p[0],0,4*sizeof(Treal));
				memset(_p[1],0,4*sizeof(Treal));
				memset(_p[2],0,4*sizeof(Treal));
				memset(_p[3],0,4*sizeof(Treal));
			}
			for(i=0,fx=1;i<4;i++)
			{
				for(j=0,fy=1;j<4;j++)
				{	b1[k] += fy*fx*_p[i][j];	fy *= y-ky;	}
				for(j=1,fy=1;j<4;j++)
				{	y1[k] += mreal(j)*fy*fx*_p[i][j];	fy *= y-ky;	}
				fx *= x-kx;
			}
			for(i=1,fx=1;i<4;i++)
			{
				for(j=0,fy=1;j<4;j++)
				{	x1[k] += mreal(i)*fy*fx*_p[i][j];	fy *= y-ky;	}
				fx *= x-kx;
			}
		}
		mglFillP(kk, b1, nz>3 ? 4:3, _p[0]);
		mglFillP(kk, x1, nz>3 ? 4:3, _p[1]);
		mglFillP(kk, y1, nz>3 ? 4:3, _p[2]);
		for(i=0,fx=1,b=0;i<4;i++)
		{
			b += fx*_p[0][i];
			if(dx)	*dx += fx*_p[1][i];
			if(dy)	*dy += fx*_p[2][i];
			fx *= z-kz;
		}
		if(dz)	for(i=1,fx=1;i<4;i++)
		{	*dz += mreal(i)*fx*_p[0][i];	fx *= z-kz;	}
	}
//	else if(ny>1 && y!=ky)	// 2d interpolation
	else if(ny>1)	// 2d interpolation
	{
		mglFillP(kx, ky, a+kz*nx*ny, nx, ny, _p);
		for(i=0,fx=1,b=0;i<4;i++)
		{
			for(j=0,fy=1;j<4;j++)
			{	b += fy*fx*_p[i][j];	fy *= y-ky;	}
			if(dy)	for(j=1,fy=1;j<4;j++)
			{	*dy+= mreal(j)*fy*fx*_p[i][j];	fy *= y-ky;	}
			fx *= x-kx;
		}
		if(dx)	for(i=1,fx=1;i<4;i++)
		{
			for(j=0,fy=1;j<4;j++)
			{	*dx+= mreal(i)*fy*fx*_p[i][j];	fy *= y-ky;	}
			fx *= x-kx;
		}
	}
//	else if(nx>1 && x!=kx)	// 1d interpolation
	else if(nx>1)	// 1d interpolation
	{
		mglFillP(kx, a+(ky+ny*kz)*nx, nx, _p[0]);
		for(i=0,fx=1,b=0;i<4;i++)
		{	b += fx*_p[0][i];	fx *= x-kx;	}
		if(dx)	for(i=1,fx=1;i<4;i++)
		{	*dx+= mreal(i)*fx*_p[0][i];	fx *= x-kx;	}
	}
	else					// no interpolation
		b = a[kx+nx*(ky+ny*kz)];
	return b;
}
//-----------------------------------------------------------------------------
template <class Treal> Treal mglSpline3st(const Treal *a, long nx, long ny, long nz, mreal x, mreal y, mreal z)
{
	if(!a || nx<1 || ny<1 || nz<1)	return 0;
	Treal _p[4][4];
	register long i,j;
	Treal fx=1, fy=1;
	long kx=long(x),ky=long(y),kz=long(z);
	Treal b=0;
	x = x>0 ?(x<nx-1 ? x:nx-1):0;
	y = y>0 ?(y<ny-1 ? y:ny-1):0;
	z = z>0 ?(z<nz-1 ? z:nz-1):0;
	//	if(x<0 || y<0 || z<0 || x>nx-1 || y>ny-1 || z>nz-1)		return 0;
	if(kx>nx-2)	kx = nx-2;	if(kx<0) 	kx = 0;
	if(ky>ny-2)	ky = ny-2;	if(ky<0) 	ky = 0;
	if(kz>nz-2)	kz = nz-2;	if(kz<0) 	kz = 0;
//	if(nz>1 && z!=kz)		// 3d interpolation
	if(nz>1)		// 3d interpolation
	{
		Treal b1[4]={0,0,0,0},  x1[4]={0,0,0,0},  y1[4]={0,0,0,0};
		long kk=1;
		if(kz==0)	{	kk=0;	}
		else if(nz>3 && kz==nz-2)	{	kk=2;	}
		for(long k=0;k<4;k++)
		{
			if(kz+k-kk<nz && kz+k-kk>=0)
				mglFillP(kx, ky, a+(kz+k-kk)*nx*ny, nx, ny, _p);
			else
			{
				memset(_p[0],0,4*sizeof(Treal));
				memset(_p[1],0,4*sizeof(Treal));
				memset(_p[2],0,4*sizeof(Treal));
				memset(_p[3],0,4*sizeof(Treal));
			}
			for(i=0,fx=1;i<4;i++)
			{
				for(j=0,fy=1;j<4;j++)
				{	b1[k] += fy*fx*_p[i][j];	fy *= y-ky;	}
				fx *= x-kx;
			}
		}
		mglFillP(kk, b1, nz>3 ? 4:3, _p[0]);
		mglFillP(kk, x1, nz>3 ? 4:3, _p[1]);
		mglFillP(kk, y1, nz>3 ? 4:3, _p[2]);
		for(i=0,fx=1,b=0;i<4;i++)
		{
			b += fx*_p[0][i];
			fx *= z-kz;
		}
	}
//	else if(ny>1 && y!=ky)	// 2d interpolation
	else if(ny>1)	// 2d interpolation
	{
		mglFillP(kx, ky, a+kz*nx*ny, nx, ny, _p);
		for(i=0,fx=1,b=0;i<4;i++)
		{
			for(j=0,fy=1;j<4;j++)
			{	b += fy*fx*_p[i][j];	fy *= y-ky;	}
			fx *= x-kx;
		}
	}
//	else if(nx>1 && x!=kx)	// 1d interpolation
	else if(nx>1)	// 1d interpolation
	{
		mglFillP(kx, a+(ky+ny*kz)*nx, nx, _p[0]);
		for(i=0,fx=1,b=0;i<4;i++)
		{	b += fx*_p[0][i];	fx *= x-kx;	}
	}
	else					// no interpolation
		b = a[kx+nx*(ky+ny*kz)];
	return b;
}
//-----------------------------------------------------------------------------
template <class Treal> Treal mglSpline1t(const Treal *a, long nx, mreal x, Treal *dx=0)
{
	Treal _p[4];
	long kx=long(x);
	Treal b=0;
	x = x>0 ?(x<nx-1 ? x:nx-1):0;
	if(kx>nx-2)	kx = nx-2;	if(kx<0) 	kx = 0;
	if(nx>1)	// 1d interpolation
	{
		mglFillP(kx, a, nx, _p);
		b = _p[0]+(x-kx)*(_p[1]+(x-kx)*(_p[2]+(x-kx)*_p[3]));
		if(dx)	*dx = _p[1]+(x-kx)*(mreal(2)*_p[2]+mreal(3)*(x-kx)*_p[3]);
	}
	else		// no interpolation
	{	b = a[0];	if(dx)	*dx=0;	}
	return b;
}
//-----------------------------------------------------------------------------
template <class Treal> Treal mglSpline1st(const Treal *a, long nx, mreal x)
{
	Treal _p[4];
	long kx=long(x);
	Treal b=0;
	x = x>0 ?(x<nx-1 ? x:nx-1):0;
	if(kx>nx-2)	kx = nx-2;	if(kx<0) 	kx = 0;
	if(nx>1)	// 1d interpolation
	{
		mglFillP(kx, a, nx, _p);
		b = _p[0]+(x-kx)*(_p[1]+(x-kx)*(_p[2]+(x-kx)*_p[3]));
	}
	else		// no interpolation
		b = a[0];
	return b;
}
//-----------------------------------------------------------------------------
