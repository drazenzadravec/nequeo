include(GenerateExportHeader)
add_compiler_export_flags()

function(libmglqt mglqt)
	add_library(mgl-${mglqt} SHARED ${MGL_QT_FILES})
	add_library(mgl-${mglqt}-static STATIC ${MGL_QT_FILES})
	set_target_properties(mgl-${mglqt} PROPERTIES SOVERSION ${MathGL_SOVERSION})
	set_target_properties(mgl-${mglqt} PROPERTIES CLEAN_DIRECT_OUTPUT 1)
	set_target_properties(mgl-${mglqt} PROPERTIES DEFINE_SYMBOL "mgl_EXPORTS")
	set_target_properties(mgl-${mglqt}-static PROPERTIES CLEAN_DIRECT_OUTPUT 1)
	set_target_properties(mgl-${mglqt}-static PROPERTIES COMPILE_FLAGS -DMGL_STATIC_DEFINE)
	target_link_libraries(mgl-${mglqt} mgl)
	qt5_use_modules(mgl-${mglqt} Core Widgets Gui PrintSupport)
	qt5_use_modules(mgl-${mglqt}-static Core Widgets Gui PrintSupport)
	if(enable-mgl2)
		set_target_properties(mgl-${mglqt} PROPERTIES OUTPUT_NAME "mgl2-${mglqt}")
		set_target_properties(mgl-${mglqt}-static PROPERTIES OUTPUT_NAME "mgl2-${mglqt}")
	else(enable-mgl2)
		set_target_properties(mgl-${mglqt}-static PROPERTIES OUTPUT_NAME "mgl-${mglqt}")
	endif(enable-mgl2)
	install(
		TARGETS mgl-${mglqt} mgl-${mglqt}-static
		RUNTIME DESTINATION bin
		ARCHIVE DESTINATION ${MGL_LIB_INSTALL_DIR}
		LIBRARY DESTINATION ${MGL_LIB_INSTALL_DIR}
	)
endfunction(libmglqt mglqt)

if(enable-qt5)
	include(../../cmake-qt5.txt)
	qt5_wrap_cpp(MGL_MOC_FILES ../../include/mgl2/qmathgl.h)
	set(MGL_QT_FILES ../qt.cpp ${MGL_MOC_FILES} ../../include/mgl2/qt.h ../../include/mgl2/qmathgl.h)
	libmglqt(qt5)
	if(enable-qt5asqt)
		libmglqt(qt)
	endif(enable-qt5asqt)

	if(MGL_HAVE_FLTK)
		include_directories(${FLTK_INCLUDE_DIR})

		set(MGL_WND_SRC ${MGL_QT_FILES} ../fltk.cpp ../../include/mgl2/fltk.h)

		add_library(mgl-wnd SHARED ${MGL_WND_SRC})
		add_library(mgl-wnd-static STATIC ${MGL_WND_SRC})

		set_target_properties(mgl-wnd PROPERTIES SOVERSION ${MathGL_SOVERSION})
		set_target_properties(mgl-wnd PROPERTIES CLEAN_DIRECT_OUTPUT 1)
		set_target_properties(mgl-wnd PROPERTIES DEFINE_SYMBOL "mgl_EXPORTS")
		set_target_properties(mgl-wnd-static PROPERTIES CLEAN_DIRECT_OUTPUT 1)
		set_target_properties(mgl-wnd-static PROPERTIES COMPILE_FLAGS -DMGL_STATIC_DEFINE)

		if(enable-mgl2)
			set_target_properties(mgl-wnd PROPERTIES OUTPUT_NAME "mgl2-wnd")
			set_target_properties(mgl-wnd-static PROPERTIES OUTPUT_NAME "mgl2-wnd")
		else(enable-mgl2)
			set_target_properties(mgl-wnd-static PROPERTIES OUTPUT_NAME "mgl-wnd")
		endif(enable-mgl2)

		target_link_libraries(mgl-wnd mgl)
		qt5_use_modules(mgl-wnd Core Widgets Gui PrintSupport)
		qt5_use_modules(mgl-wnd-static Core Widgets Gui PrintSupport)
		target_link_libraries(mgl-wnd ${FLTK_LIBRARIES})

		install(
			TARGETS mgl-wnd mgl-wnd-static
			RUNTIME DESTINATION bin
			ARCHIVE DESTINATION ${MGL_LIB_INSTALL_DIR}
			LIBRARY DESTINATION ${MGL_LIB_INSTALL_DIR}
		)
	endif(MGL_HAVE_FLTK)
else(enable-qt5)
	set(MGL_HAVE_QT5 0)
endif(enable-qt5)
