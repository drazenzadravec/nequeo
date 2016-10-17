/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2015 http://www.nequeo.com.au/
*
*  File :          CallMethodData.cpp
*  Purpose :       SIP CallMethodData class.
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

#include "CallMethodData.h"

using namespace Nequeo::Net::PjSip;

///	<summary>
///	Call method data.
///	</summary>
CallMethodData::CallMethodData() :
	_disposed(false)
{
}

///	<summary>
///	Call method data.
///	</summary>
CallMethodData::~CallMethodData()
{
	if (!_disposed)
	{
		_disposed = true;
	}
}

/// <summary>
/// Call initialise tone generator.
/// </summary>
/// <param name="call_id">The call id.</param>
/// <returns>The stream info.</returns>
call_data* CallMethodData::call_init_tonegen(pjsua_call_id call_id)
{
	pj_pool_t *pool;
	struct call_data *cd;
	pjsua_call_info ci;

	pool = pjsua_pool_create("NequeoCallMethodData", 512, 512);
	cd = PJ_POOL_ZALLOC_T(pool, struct call_data);
	cd->pool = pool;

	pjmedia_tonegen_create(cd->pool, 8000, 1, 160, 16, 0, &cd->tonegen);
	pjsua_conf_add_port(cd->pool, cd->tonegen, &cd->toneslot);

	pjsua_call_get_info(call_id, &ci);
	pjsua_conf_connect(cd->toneslot, ci.conf_slot);

	pjsua_call_set_user_data(call_id, (void*)cd);

	return cd;
}

/// <summary>
/// Call play digit tone.
/// </summary>
/// <param name="call_id">The call id.</param>
/// <param name="digits">The digits.</param>
void CallMethodData::call_play_digit(pjsua_call_id call_id, const char *digits)
{
	pjmedia_tone_digit d[16];
	unsigned i, count = strlen(digits);
	struct call_data *cd;

	cd = (struct call_data*) pjsua_call_get_user_data(call_id);
	if (!cd)
		cd = call_init_tonegen(call_id);

	if (count > PJ_ARRAY_SIZE(d))
		count = PJ_ARRAY_SIZE(d);

	pj_bzero(d, sizeof(d));
	for (i = 0; i<count; ++i) {
		d[i].digit = digits[i];
		d[i].on_msec = 100;
		d[i].off_msec = 200;
		d[i].volume = 0;
	}

	pjmedia_tonegen_play_digits(cd->tonegen, count, d, 0);
}

/// <summary>
/// Call de-initialise tone generator.
/// </summary>
/// <param name="call_id">The call id.</param>
void CallMethodData::call_deinit_tonegen(pjsua_call_id call_id)
{
	struct call_data *cd;

	cd = (struct call_data*) pjsua_call_get_user_data(call_id);
	if (!cd)
		return;

	pjsua_conf_remove_port(cd->toneslot);
	pjmedia_port_destroy(cd->tonegen);
	pj_pool_release(cd->pool);

	pjsua_call_set_user_data(call_id, NULL);
}