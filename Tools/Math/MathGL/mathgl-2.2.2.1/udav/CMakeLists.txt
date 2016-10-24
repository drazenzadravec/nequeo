if(enable-qt)

#set(mgl_wnd_src window.cpp)
set(udav_src anim_dlg.cpp find_dlg.cpp mem_pnl.cpp prop_dlg.cpp textedit.cpp args_dlg.cpp
	help_pnl.cpp newcmd_dlg.cpp text_pnl.cpp calc_dlg.cpp hint_dlg.cpp
	open_dlg.cpp qmglsyntax.cpp udav_wnd.cpp dat_pnl.cpp info_dlg.cpp opt_dlg.cpp setup_dlg.cpp
	files_dlg.cpp plot_pnl.cpp style_dlg.cpp data_dlg.cpp tree_pnl.cpp)

set(udav_moc_hdr anim_dlg.h files_dlg.h info_dlg.h opt_dlg.h text_pnl.h args_dlg.h
	find_dlg.h mem_pnl.h plot_pnl.h setup_dlg.h udav_wnd.h calc_dlg.h help_pnl.h newcmd_dlg.h
	prop_dlg.h style_dlg.h dat_pnl.h open_dlg.h textedit.h hint_dlg.h data_dlg.h)

set(udav_rc udav.qrc)

if(WIN32)
	set(udav_src ${udav_src} udav.rc)
endif(WIN32)

if(MGL_HAVE_HDF5)
#	target_link_libraries(mgl ${HDF5_LIBRARIES})
	include_directories(${HDF5_INCLUDE_DIR})
endif(MGL_HAVE_HDF5)

if(MGL_HAVE_GSL)
#	target_link_libraries(mgl ${GSL_LIB} ${GSL_CBLAS_LIB} )
	include_directories(${GSL_INCLUDE_DIR})
endif(MGL_HAVE_GSL)

if(enable-qt5)
	include(../cmake-qt5.txt)
	qt5_add_resources(udav_rc_src ${udav_rc} )
	qt5_wrap_cpp(udav_moc_src ${udav_moc_hdr} )
else(enable-qt5)
	include(../cmake-qt4.txt)
	qt4_add_resources(udav_rc_src ${udav_rc} )
	qt4_wrap_cpp(udav_moc_src ${udav_moc_hdr} )
endif(enable-qt5)
add_executable(udav ${udav_src} ${udav_moc_src} ${udav_rc_src})
#set_target_properties(udav PROPERTIES COMPILE_FLAGS "${CMAKE_CXX_FLAGS} -pthread")
if(enable-qt5)
	target_link_libraries(udav mgl-qt5)
	qt5_use_modules(udav Core Widgets Gui PrintSupport)
else(enable-qt5)
	target_link_libraries(udav mgl-qt4)
	target_link_libraries(udav ${QT_LIBRARIES})
endif(enable-qt5)

if(MGL_HAVE_PTHREAD)
	target_link_libraries(udav ${CMAKE_THREAD_LIBS_INIT})
endif(MGL_HAVE_PTHREAD)

install(
	TARGETS udav
	RUNTIME DESTINATION bin
)
install(FILES udav.desktop DESTINATION ${CMAKE_INSTALL_PREFIX}/share/applications/)
install(FILES udav.png DESTINATION ${CMAKE_INSTALL_PREFIX}/share/pixmaps/)
install(FILES udav_ru.qm DESTINATION ${CMAKE_INSTALL_PREFIX}/share/udav/)

endif(enable-qt)
