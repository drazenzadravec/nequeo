set(mgl_src
	addon.cpp axis.cpp base_cf.cpp base.cpp canvas_cf.cpp canvas.cpp cont.cpp crust.cpp
	complex.cpp complex_io.cpp fft.cpp data_gr.cpp
	data.cpp data_io.cpp data_ex.cpp data_png.cpp def_font.cpp
	export_2d.cpp export_3d.cpp eval.cpp evalp.cpp exec.cpp export.cpp
	fit.cpp font.cpp obj.cpp other.cpp parser.cpp pde.cpp pixel.cpp
	plot.cpp prim.cpp surf.cpp tex_table.cpp vect.cpp volume.cpp evalc.cpp
	s_hull/s_hull_pro.cpp window.cpp
)

set(mgl_hdr
	../include/mgl2/base_cf.h	../include/mgl2/fit.h		../include/mgl2/plot.h
	../include/mgl2/base.h		../include/mgl2/prim.h		../include/mgl2/canvas_cf.h
	../include/mgl2/font.h		../include/mgl2/canvas.h	../include/mgl2/surf.h
	../include/mgl2/mgl_cf.h	../include/mgl2/type.h		${MathGL_BINARY_DIR}/include/mgl2/config.h
${MathGL_BINARY_DIR}/include/mgl2/dllexport.h
	../include/mgl2/cont.h		../include/mgl2/mgl.h		../include/mgl2/vect.h
	../include/mgl2/data.h		../include/mgl2/volume.h	../include/mgl2/data_cf.h
	../include/mgl2/define.h	../include/mgl2/other.h		../include/mgl2/eval.h
	../include/mgl2/parser.h	../include/mgl2/addon.h		../include/mgl2/evalc.h
	s_hull/s_hull_pro.h			../include/mgl2/wnd.h		../include/mgl2/canvas_wnd.h
	../include/mgl2/thread.h	../include/mgl2/abstract.h	../include/mgl2/pde.h
)

add_definitions(-DMGL_SRC)

if(MGL_HAVE_PNG)
	set(prc_src prc/PRCbitStream.cc prc/PRCdouble.cc prc/oPRCFile.cc prc/writePRC.cc prc.cpp )
	set(prc_hdr prc/PRC.h prc/PRCbitStream.h prc/PRCdouble.h prc/oPRCFile.h prc/writePRC.h )

	set(mgl_src ${mgl_src} ${prc_src} )
	set(mgl_hdr ${mgl_hdr} ${prc_hdr} )
	include_directories(prc)
endif(MGL_HAVE_PNG)

if(MGL_HAVE_OPENGL)
	set(mgl_src ${mgl_src} opengl.cpp )
	set(mgl_hdr ${mgl_hdr} ../include/mgl2/opengl.h )
endif(MGL_HAVE_OPENGL)

include(GenerateExportHeader)
add_compiler_export_flags()
add_library(mgl SHARED ${mgl_src} ${mgl_hdr})
add_library(mgl-static STATIC ${mgl_src} ${mgl_hdr})
set_target_properties(mgl PROPERTIES CLEAN_DIRECT_OUTPUT 1)
set_target_properties(mgl-static PROPERTIES CLEAN_DIRECT_OUTPUT 1)
set_target_properties(mgl-static PROPERTIES COMPILE_FLAGS -DMGL_STATIC_DEFINE)
generate_export_header(mgl EXPORT_FILE_NAME ../include/mgl2/dllexport.h)

if(enable-mgl2)
	set_target_properties(mgl PROPERTIES OUTPUT_NAME "mgl2")
	set_target_properties(mgl-static PROPERTIES OUTPUT_NAME "mgl2")
else(enable-mgl2)
	set_target_properties(mgl-static PROPERTIES OUTPUT_NAME "mgl")
endif(enable-mgl2)

# if(MGL_HAVE_LTDL)
# 	target_link_libraries(mgl ${LTDL_LIB})
# 	include_directories(${LTDL_INCLUDE_DIR})
# endif(MGL_HAVE_LTDL)

if(MGL_HAVE_PDF)
	include_directories(${HPDF_INCLUDE_DIR})
	target_link_libraries(mgl ${HPDF_LIB})
endif(MGL_HAVE_PDF)

if(MGL_HAVE_PTHREAD)
	target_link_libraries(mgl ${CMAKE_THREAD_LIBS_INIT})
endif(MGL_HAVE_PTHREAD)

if(MGL_HAVE_JPEG)
	target_link_libraries(mgl ${JPEG_LIBRARIES})
	include_directories(${JPEG_INCLUDE_DIR})
endif(MGL_HAVE_JPEG)

if(MGL_HAVE_GIF)
	target_link_libraries(mgl ${GIF_LIBRARIES})
	include_directories(${GIF_INCLUDE_DIR})
endif(MGL_HAVE_GIF)

if(MGL_HAVE_HDF5)
	target_link_libraries(mgl ${HDF5_LIBRARIES})
	include_directories(${HDF5_INCLUDE_DIR})
endif(MGL_HAVE_HDF5)

if(MGL_HAVE_HDF4)
	target_link_libraries(mgl ${HDF4MF_LIB} ${HDF4_LIB})
	include_directories(${HDF4_INCLUDE_DIR})
endif(MGL_HAVE_HDF4)

if(MGL_HAVE_GSL)
	target_link_libraries(mgl ${GSL_LIB} ${GSL_CBLAS_LIB} )
	include_directories(${GSL_INCLUDE_DIR})
endif(MGL_HAVE_GSL)

if(MGL_HAVE_OPENGL)
	target_link_libraries(mgl ${OPENGL_LIBRARIES} )
	include_directories(${OPENGL_INCLUDE_DIR} )
endif(MGL_HAVE_OPENGL)

if(MGL_HAVE_PNG)
	target_link_libraries(mgl ${PNG_LIBRARIES} )
	include_directories(${PNG_INCLUDE_DIR})
endif(MGL_HAVE_PNG)

if(MGL_HAVE_ZLIB)
	target_link_libraries(mgl ${ZLIB_LIBRARIES} )
	include_directories(${ZLIB_INCLUDE_DIR})
endif(MGL_HAVE_ZLIB)

if(MGL_HAVE_MPI)
	add_library(mgl-mpi SHARED mpi.cpp ../include/mgl2/mpi.h)
	add_library(mgl-mpi-static STATIC mpi.cpp ../include/mgl2/mpi.h)
	set_target_properties(mgl-mpi PROPERTIES CLEAN_DIRECT_OUTPUT 1)
	set_target_properties(mgl-mpi PROPERTIES DEFINE_SYMBOL "mgl_EXPORTS")
	set_target_properties(mgl-mpi-static PROPERTIES OUTPUT_NAME "mgl-mpi")
	set_target_properties(mgl-mpi-static PROPERTIES CLEAN_DIRECT_OUTPUT 1)
	set_target_properties(mgl-mpi-static PROPERTIES COMPILE_FLAGS -DMGL_STATIC_DEFINE)

	if(enable-mgl2)
		set_target_properties(mgl-mpi PROPERTIES OUTPUT_NAME "mgl2-mpi")
		set_target_properties(mgl-mpi-static PROPERTIES OUTPUT_NAME "mgl2-mpi")
	else(enable-mgl2)
		set_target_properties(mgl-mpi-static PROPERTIES OUTPUT_NAME "mgl-mpi")
	endif(enable-mgl2)

	target_link_libraries(mgl-mpi ${MPI_LIBRARIES} )
#	include_directories(${MPI_C_INCLUDE_PATH})
	include_directories(${MPI_CXX_INCLUDE_PATH})

	set_target_properties(mgl-mpi PROPERTIES SOVERSION ${MathGL_SOVERSION})
	install(
		TARGETS mgl-mpi mgl-mpi-static
		RUNTIME DESTINATION bin
		ARCHIVE DESTINATION ${MGL_LIB_INSTALL_DIR}
		LIBRARY DESTINATION ${MGL_LIB_INSTALL_DIR}
	)
endif(MGL_HAVE_MPI)

if(M_LIB)
	target_link_libraries(mgl ${M_LIB})
endif(M_LIB)

set_target_properties(mgl PROPERTIES SOVERSION ${MathGL_SOVERSION})
install(
	TARGETS mgl mgl-static
	RUNTIME DESTINATION bin
	ARCHIVE DESTINATION ${MGL_LIB_INSTALL_DIR}
	LIBRARY DESTINATION ${MGL_LIB_INSTALL_DIR}
)
install(FILES ${MathGL_BINARY_DIR}/include/mgl2/dllexport.h DESTINATION ${MGL_INCLUDE_PATH})
