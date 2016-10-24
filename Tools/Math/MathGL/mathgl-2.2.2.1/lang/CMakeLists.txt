set(src_imp_dep
../include/mgl2/type.h
../include/mgl2/data.h
../include/mgl2/mgl.h
)

set(mgl_clean_files "")

SET_SOURCE_FILES_PROPERTIES(mathgl.i numpy.i PROPERTIES CPLUSPLUS ON)

if(MGL_HAVE_PYTHON)
	include_directories(${PYTHON_INCLUDE_DIR})
	execute_process(
		COMMAND ${PYTHON_EXECUTABLE} -c "from distutils.sysconfig import get_python_lib; print get_python_lib(prefix='${CMAKE_INSTALL_PREFIX}')"
		OUTPUT_VARIABLE MGL_PYTHON_SITE_PACKAGES
		OUTPUT_STRIP_TRAILING_WHITESPACE)
	set(SWIG_MODULE_mathgl_EXTRA_DEPS numpy.i ${src_imp_dep})
	SWIG_ADD_MODULE(mathgl python mathgl.i)
	SWIG_LINK_LIBRARIES(mathgl mgl ${PYTHON_LIBRARIES})
	set_target_properties(_mathgl PROPERTIES BUILD_WITH_INSTALL_RPATH ON)
	add_custom_command(OUTPUT mathgl.pyc
		COMMAND ${PYTHON_EXECUTABLE}
		ARGS -c \"from py_compile import compile\; compile('mathgl.py') \"
		WORKING_DIRECTORY ${CMAKE_BINARY_DIR}/lang
		DEPENDS _mathgl
	)
	add_custom_target(mgl_python_module ALL DEPENDS _mathgl mathgl.pyc)

	install(FILES ${MathGL_BINARY_DIR}/lang/mathgl.py ${MathGL_BINARY_DIR}/lang/mathgl.pyc DESTINATION ${MGL_PYTHON_SITE_PACKAGES})
	install (TARGETS _mathgl LIBRARY DESTINATION ${MGL_PYTHON_SITE_PACKAGES})
	set(mgl_clean_files ${mgl_clean_files} mathgl.py)
endif(MGL_HAVE_PYTHON)

if(MGL_HAVE_LUA)
	include_directories(${LUA_INCLUDE_DIR})
	set(SWIG_MODULE_mgl-lua_EXTRA_DEPS numpy.i ${src_imp_dep})
	SWIG_ADD_MODULE(mgl-lua lua mathgl.i)
	SWIG_LINK_LIBRARIES(mgl-lua mgl ${LUA_LIBRARIES})
	set_target_properties(mgl-lua PROPERTIES PREFIX "" BUILD_WITH_INSTALL_RPATH ON)

	install (TARGETS mgl-lua LIBRARY DESTINATION ${MGL_LIB_INSTALL_DIR})
endif(MGL_HAVE_LUA)

if(MGL_HAVE_OCTAVE)
	execute_process(COMMAND ${oct_prog} -p CANONICAL_HOST_TYPE
	OUTPUT_VARIABLE oct_host
	OUTPUT_STRIP_TRAILING_WHITESPACE)
	execute_process(COMMAND ${oct_prog} -p API_VERSION
	OUTPUT_VARIABLE oct_api
	OUTPUT_STRIP_TRAILING_WHITESPACE)
	execute_process(COMMAND ${oct_prog} -p OCTINCLUDEDIR
	OUTPUT_VARIABLE oct_inc
	OUTPUT_STRIP_TRAILING_WHITESPACE)
message(STATUS "${oct_prog} ${oct_host} ${oct_api}")
	set(oct_arch ${oct_host}-${oct_api})
	set(pkg_name mathgl)
	set(mgl_pkg_dir ${MathGL_BINARY_DIR}/lang/${pkg_name}/inst/${oct_arch})

	include_directories(${oct_inc} ${oct_inc}/..)
	SET(SWIG_MODULE_mgl-oct_EXTRA_DEPS numpy.i ${src_imp_dep})
	SWIG_ADD_MODULE(mgl-oct octave mathgl.i)
	SWIG_LINK_LIBRARIES(mgl-oct mgl)
	set_target_properties(mgl-oct PROPERTIES OUTPUT_NAME mathgl PREFIX "" SUFFIX ".oct" BUILD_WITH_INSTALL_RPATH ON)
	get_target_property(mgl-oct_filename mgl-oct LOCATION)
	add_custom_command(OUTPUT mathgl.tar.gz
		COMMAND ${CMAKE_COMMAND} -E make_directory ${mgl_pkg_dir}
		COMMAND ${CMAKE_COMMAND} -E copy ${MathGL_SOURCE_DIR}/COPYING ${MathGL_BINARY_DIR}/lang/${pkg_name}
		COMMAND ${CMAKE_COMMAND} -E copy ${MathGL_SOURCE_DIR}/lang/DESCRIPTION ${MathGL_BINARY_DIR}/lang/${pkg_name}
		COMMAND ${CMAKE_COMMAND} -E copy ${MathGL_SOURCE_DIR}/lang/INDEX ${MathGL_BINARY_DIR}/lang/${pkg_name}
		COMMAND ${CMAKE_COMMAND} -E copy ${MathGL_SOURCE_DIR}/lang/PKG_ADD_template ${MathGL_BINARY_DIR}/lang/${pkg_name}
		COMMAND ${CMAKE_COMMAND} -E copy ${mgl-oct_filename} ${mgl_pkg_dir}
		COMMAND ${oct_tar} cpzf mathgl.tar.gz ${pkg_name}
		WORKING_DIRECTORY ${CMAKE_BINARY_DIR}/lang
		DEPENDS mgl-oct
	)
	add_custom_target(mgl_octave_module ALL DEPENDS mathgl.tar.gz)

	install(FILES ${CMAKE_BINARY_DIR}/lang/mathgl.tar.gz DESTINATION ${MGL_DATA_PATH})
	if(enable-octave-install)
		install(CODE "execute_process(
			COMMAND ${oct_exec} -q --eval \"pkg install mathgl.tar.gz\"
			WORKING_DIRECTORY ${CMAKE_BINARY_DIR}/lang )")
	endif(enable-octave-install)
	set(mgl_clean_files ${mgl_clean_files} mathgl)
endif(MGL_HAVE_OCTAVE)
set_directory_properties(PROPERTIES ADDITIONAL_MAKE_CLEAN_FILES "${mgl_clean_files}")
