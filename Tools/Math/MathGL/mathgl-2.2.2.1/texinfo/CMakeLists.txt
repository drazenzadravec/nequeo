	set(MGL_EXTRA light)
	set(MGL_PNG alpha area aspect axial axis barh bars belt boxplot
		box boxs candle chart cloud colorbar combined cones conta contd
		contfa contf contf_xyz cont contv cont_xyz curvcoor cut dat_diff
		dat_extra densa dens dens_xyz dew dots error error2 fall fit flow fonts
		fog grad hist inplot label legend loglog map mark mesh mirror
		molecule parser pde pipe plot primitives projection qo2d radar region schemes
		several_light solve stem step stereo stfa style surf3a surf3c surf3
		surfa surfc surf table tape tens ternary textmark text ticks tile tiles
		torus traj triangulation triplot tube type0 type1 type2 vect vecta venn
		projection5 mask correl refill ohlc )
	set(MGL_PNG_N )
	set(MGL_PNG_S )
	set(MGL_PNG_J )
	set(MGL_PNG_D )

	set(MGL_OUT ${CMAKE_BINARY_DIR}/texinfo)
	set(MGL_TEX ${CMAKE_SOURCE_DIR}/texinfo)
	file(MAKE_DIRECTORY ${MGL_OUT}/udav)
	file(MAKE_DIRECTORY ${MGL_OUT}/png)
	file(MAKE_DIRECTORY ${MGL_OUT}/small)
	file(MAKE_DIRECTORY ${MGL_OUT}/json)
	file(MAKE_DIRECTORY ${MGL_OUT}/pdf)
	file(COPY ${MGL_TEX}/qt.png ${MGL_TEX}/fltk.png ${MGL_TEX}/classes.png ${MGL_TEX}/emblem_sm.png ${MGL_TEX}/datadvance.png DESTINATION ${MGL_OUT})
	file(COPY ${MGL_TEX}/index.html ${MGL_TEX}/json.html ${MGL_TEX}/mathgl.js DESTINATION ${MGL_OUT})

	set(UDAV_IMG udav_arg.png udav_calc.png udav_cmd.png udav_data.png
		udav_gen_set.png udav_help.png udav_light.png udav_main.png udav_opt.png
		udav_pen.png udav_prop.png udav_sch.png udav_txt.png udav_var.png)
	foreach(SAMPLE ${UDAV_IMG})
		file(COPY ${MGL_TEX}/udav/${SAMPLE} DESTINATION ${MGL_OUT}/udav/)
	endforeach(SAMPLE)

	foreach(SAMPLE ${MGL_PNG})
		set(MGL_PNG_N ${MGL_PNG_N} ${MGL_OUT}/png/${SAMPLE}.png)
		add_custom_command(OUTPUT ${MGL_OUT}/png/${SAMPLE}.png
			COMMAND mgl_example -kind=${SAMPLE} -web
			DEPENDS mgl_example
			WORKING_DIRECTORY ${MGL_OUT}/png )
		set(MGL_PNG_S ${MGL_PNG_S} ${MGL_OUT}/small/${SAMPLE}-sm.png)
		add_custom_command(OUTPUT ${MGL_OUT}/small/${SAMPLE}-sm.png
			COMMAND mgl_example -kind=${SAMPLE} -mini
			DEPENDS mgl_example
			WORKING_DIRECTORY ${MGL_OUT}/small )
if(MGL_HAVE_DOC_JSON)
		set(MGL_PNG_J ${MGL_PNG_J} ${MGL_OUT}/json/${SAMPLE}.json)
		add_custom_command(OUTPUT ${MGL_OUT}/json/${SAMPLE}.json
			COMMAND mgl_example -json -kind=${SAMPLE}
#			COMMAND ${CMAKE_BINARY_DIR}/examples/mgl_example -json -kind=${SAMPLE}
			DEPENDS mgl_example
			WORKING_DIRECTORY ${MGL_OUT}/json )
endif(MGL_HAVE_DOC_JSON)
if(MGL_HAVE_DOC_PRC)
		set(MGL_PNG_D ${MGL_PNG_D} ${MGL_OUT}/pdf/${SAMPLE}.pdf)
		add_custom_command(OUTPUT ${MGL_OUT}/pdf/${SAMPLE}.pdf
			COMMAND mgl_example -pdf -kind=${SAMPLE}
			DEPENDS mgl_example
			WORKING_DIRECTORY ${MGL_OUT}/pdf )
endif(MGL_HAVE_DOC_PRC)
	endforeach(SAMPLE)

	foreach(SAMPLE ${MGL_EXTRA})
		set(MGL_PNG_N ${MGL_PNG_N} ${MGL_OUT}/png/${SAMPLE}.png)
		add_custom_command(OUTPUT ${MGL_OUT}/png/${SAMPLE}.png
			COMMAND mgl_example -kind=${SAMPLE} -web
			DEPENDS mgl_example
			WORKING_DIRECTORY ${MGL_OUT}/png )
		set(MGL_PNG_S ${MGL_PNG_S} ${MGL_OUT}/small/${SAMPLE}-sm.png)
		add_custom_command(OUTPUT ${MGL_OUT}/small/${SAMPLE}-sm.png
			COMMAND mgl_example -kind=${SAMPLE} -mini
			DEPENDS mgl_example
			WORKING_DIRECTORY ${MGL_OUT}/small )
	endforeach(SAMPLE)

	set(list_texi_files_en overview_en.texi example_en.texi ex_mgl_en.texi parse_en.texi formats_en.texi udav_en.texi symbols_en.texi core_en.texi concept_en.texi widget_en.texi data_en.texi other_en.texi appendix_en.texi fdl.texi version.texi time.texi time_big.texi )
	set(list_texi_files_ru overview_ru.texi example_ru.texi ex_mgl_ru.texi parse_ru.texi formats_ru.texi udav_ru.texi symbols_ru.texi core_ru.texi concept_ru.texi widget_ru.texi data_ru.texi other_ru.texi appendix_ru.texi fdl.texi version.texi time.texi time_big.texi )

	add_custom_command(OUTPUT ${MGL_OUT}/mathgl_en.info
		COMMAND ${findmi} ${MGL_TEX}/mathgl_en.texi
		DEPENDS ${list_texi_files_en} mathgl_en.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)
	add_custom_command(OUTPUT ${MGL_OUT}/mathgl_ru.info
		COMMAND ${findmi} ${MGL_TEX}/mathgl_ru.texi
		DEPENDS ${list_texi_files_ru} mathgl_ru.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)

	add_custom_command(OUTPUT ${MGL_OUT}/mathgl_en.html
		COMMAND ${findth} ${th_opt} -I ${MGL_OUT} --no-split ${MGL_TEX}/mathgl_en.texi
		DEPENDS ${list_texi_files_en} mathgl_en.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)
	add_custom_command(OUTPUT ${MGL_OUT}/mgl_en.html
		COMMAND ${findth} ${th_opt} -I ${MGL_OUT} --no-split ${MGL_TEX}/mgl_en.texi
		DEPENDS ${list_texi_files_en} mgl_en.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)
	add_custom_command(OUTPUT ${MGL_OUT}/mathgl_ru.html
		COMMAND ${findth} ${th_opt} -I=${MGL_OUT} --no-split ${MGL_TEX}/mathgl_ru.texi
		DEPENDS ${list_texi_files_ru} mathgl_ru.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)
	add_custom_command(OUTPUT ${MGL_OUT}/mgl_ru.html
		COMMAND ${findth} ${th_opt} -I=${MGL_OUT} --no-split ${MGL_TEX}/mgl_ru.texi
		DEPENDS ${list_texi_files_ru} mgl_ru.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)

	add_custom_command(OUTPUT ${site_en}
		COMMAND ${findth} ${th_opt} -I=${MGL_OUT}/png --split=node -o doc_en ${MGL_TEX}/doc_en.texi
		DEPENDS ${list_texi_files_en} doc_en.texi ${MGL_PNG_N} web_en.texi ${MGL_PNG_S} ${MGL_PNG_J} ${MGL_PNG_D}
		WORKING_DIRECTORY ${MGL_OUT}
	)
	add_custom_command(OUTPUT ${site_ru}
		COMMAND ${findth} ${th_opt} -I=${MGL_OUT}/png --split=node -o doc_ru ${MGL_TEX}/doc_ru.texi
		DEPENDS ${list_texi_files_ru} doc_ru.texi ${MGL_PNG_N} web_ru.texi ${MGL_PNG_S} ${MGL_PNG_J} ${MGL_PNG_D}
		WORKING_DIRECTORY ${MGL_OUT}
	)

	add_custom_command(OUTPUT ${MGL_OUT}/mathgl_en.pdf
		COMMAND ${findtp} ${MGL_TEX}/mathgl_en.texi
		COMMAND ${findtp} ${MGL_TEX}/mathgl_en.texi
		COMMAND ${findtp} ${MGL_TEX}/mathgl_en.texi
		DEPENDS ${list_texi_files_en} mathgl_en.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)
	add_custom_command(OUTPUT ${MGL_OUT}/mgl_en.pdf
		COMMAND ${findtp} ${MGL_TEX}/mgl_en.texi
		COMMAND ${findtp} ${MGL_TEX}/mgl_en.texi
		COMMAND ${findtp} ${MGL_TEX}/mgl_en.texi
		DEPENDS ${list_texi_files_en} mgl_en.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)

	add_custom_command(OUTPUT ${MGL_OUT}/mathgl_ru.pdf
		COMMAND ${findtp} ${MGL_TEX}/mathgl_ru.texi
		COMMAND ${findtp} ${MGL_TEX}/mathgl_ru.texi
		COMMAND ${findtp} ${MGL_TEX}/mathgl_ru.texi
		DEPENDS ${list_texi_files_ru} mathgl_ru.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)
	add_custom_command(OUTPUT ${MGL_OUT}/mgl_ru.pdf
		COMMAND ${findtp} ${MGL_TEX}/mgl_ru.texi
		COMMAND ${findtp} ${MGL_TEX}/mgl_ru.texi
		COMMAND ${findtp} ${MGL_TEX}/mgl_ru.texi
		DEPENDS ${list_texi_files_en} mgl_ru.texi ${MGL_PNG_N}
		WORKING_DIRECTORY ${MGL_OUT}
	)

	if(MGL_HAVE_DOC_INFO)
		add_custom_target(doc_info ALL
			DEPENDS ${MGL_OUT}/mathgl_en.info
			DEPENDS ${MGL_OUT}/mathgl_ru.info
		)
# TODO: try to install all mathgl*.info* in future!!!
		install(FILES ${MGL_OUT}/mathgl_en.info ${MGL_OUT}/mathgl_en.info-1 ${MGL_OUT}/mathgl_en.info-2 ${MGL_OUT}/mathgl_en.info-3 DESTINATION ${MGL_INFO_PATH})
	endif(MGL_HAVE_DOC_INFO)
	
	if(MGL_HAVE_DOC_HTML)
		add_custom_target(doc_html ALL
		DEPENDS ${MGL_OUT}/mgl_en.html
		DEPENDS ${MGL_OUT}/mathgl_en.html
		DEPENDS ${MGL_OUT}/mgl_ru.html
		DEPENDS ${MGL_OUT}/mathgl_ru.html
		)
		install(FILES ${MGL_TEX}/qt.png ${MGL_TEX}/fltk.png ${MGL_TEX}/classes.png ${MGL_TEX}/datadvance.png DESTINATION ${MGL_DOC_PATH})
		install(FILES ${MGL_OUT}/mathgl_en.html ${MGL_OUT}/mgl_en.html ${MGL_OUT}/mathgl_ru.html ${MGL_OUT}/mgl_ru.html DESTINATION ${MGL_DOC_PATH})
		install(DIRECTORY ${MGL_OUT}/png DESTINATION ${MGL_DOC_PATH})
		install(DIRECTORY ${MGL_OUT}/udav DESTINATION ${MGL_DOC_PATH})
	endif(MGL_HAVE_DOC_HTML)

	if(MGL_HAVE_DOC_SITE)
		add_custom_target(doc_site ALL
		DEPENDS ${site_en}
		DEPENDS ${site_ru}
		)
	endif(MGL_HAVE_DOC_SITE)

	if(MGL_HAVE_DOC_PDF_EN)
		add_custom_target(doc_en ALL
			DEPENDS ${MGL_OUT}/mgl_en.pdf
			DEPENDS ${MGL_OUT}/mathgl_en.pdf
		)
		install(FILES ${MGL_OUT}/mathgl_en.pdf DESTINATION ${MGL_DOC_PATH})
	endif(MGL_HAVE_DOC_PDF_EN)
	
	if(MGL_HAVE_DOC_PDF_RU)
		add_custom_target(doc_ru ALL
#			DEPENDS ${MGL_OUT}/mgl_ru.pdf
#			DEPENDS ${MGL_OUT}/mathgl_ru.pdf
		)
#		install(FILES ${MGL_OUT}/mathgl_ru.pdf DESTINATION ${MGL_DOC_PATH})
	endif(MGL_HAVE_DOC_PDF_RU)

	set_directory_properties(PROPERTIES ADDITIONAL_MAKE_CLEAN_FILES "doc_en;doc_ru;./mathgl*.*;./mgl*.*")

	install(FILES ${MGL_TEX}/mglconv.1 ${MGL_TEX}/mglview.1 ${MGL_TEX}/udav.1 ${MGL_TEX}/mgl.cgi.1 DESTINATION ${MGL_MAN_PATH}/man1)
	install(FILES ${MGL_TEX}/mgl.5 DESTINATION ${MGL_MAN_PATH}/man5)
