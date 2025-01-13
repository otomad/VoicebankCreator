cmake_minimum_required(VERSION 3.16)

project(VoicebankCreator VERSION 0.1 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD_REQUIRED ON)

include(FetchContent)

find_package(Qt6 REQUIRED COMPONENTS Quick Multimedia LinguistTools)

qt_standard_project_setup(REQUIRES 6.5
	I18N_TRANSLATED_LANGUAGES zh_CN
)

qt_add_executable(${PROJECT_NAME}
	main.cpp
	application.rc
)

qt_add_translations(${PROJECT_NAME}
	TS_FILE_BASE ${PROJECT_NAME}
	TS_FILE_DIR Translations
)

if (QT_KNOWN_POLICY_QTP0004)
	qt_policy(SET QTP0004 NEW)
endif ()

set_source_files_properties(Constants.qml PROPERTIES QT_QML_SINGLETON_TYPE TRUE)
set_source_files_properties(Components/Images.qml PROPERTIES QT_QML_SINGLETON_TYPE TRUE)
set_source_files_properties(Components/PlayerSliderPopupSingletonService.qml PROPERTIES QT_QML_SINGLETON_TYPE TRUE)

qt_add_qml_module(${PROJECT_NAME}
	URI ${PROJECT_NAME}
	VERSION 1.0
	QML_FILES
		Main.qml
		Constants.qml
		Components/PlayerToolBar.qml
		Components/PlayerPreview.qml
		Components/PlayerToolButton.qml
		Components/PlayerPlaybackButton.qml
		Components/Images.qml
		Components/ErrorPopup.qml
		Components/PlayerSliderPopup.qml
		Components/PlayerSliderPopupSingletonService.qml
	SOURCES
		mediaformatprovider.h mediaformatprovider.cpp
		includes.h
		boolinq.h
		backdropwindowhelper.h backdropwindowhelper.cpp
		audiopreview.h audiopreview.cpp
	RESOURCES
		Icons/open.svg
		Icons/pause.svg
		Icons/play.svg
		Icons/speed_low.svg
		Icons/speed_medium.svg
		Icons/speed_high.svg
		Icons/volume0.svg
		Icons/volume1.svg
		Icons/volume2.svg
		Icons/volume3.svg
		Icons/mute.svg
		Icons/error.svg
)

# Qt for iOS sets MACOSX_BUNDLE_GUI_IDENTIFIER automatically since Qt 6.1.
# If you are developing for iOS or macOS you should consider setting an
# explicit, fixed bundle identifier manually though.
set_target_properties(${PROJECT_NAME} PROPERTIES
	MACOSX_BUNDLE_GUI_IDENTIFIER com.github.otomad.${PROJECT_NAME}
	MACOSX_BUNDLE_BUNDLE_VERSION ${PROJECT_VERSION}
	MACOSX_BUNDLE_SHORT_VERSION_STRING ${PROJECT_VERSION_MAJOR}.${PROJECT_VERSION_MINOR}
	MACOSX_BUNDLE TRUE
	WIN32_EXECUTABLE TRUE
)

target_link_libraries(${PROJECT_NAME} PRIVATE
	Qt6::Quick
	Qt6::Multimedia
)

target_link_libraries(${PROJECT_NAME} PRIVATE
	user32.lib
	dwmapi.lib
)

# target_link_libraries(MediaPlayerApp PRIVATE
# 	Qt6::Core
# 	Qt6::Svg
# 	Qt6::Quick
# 	Qt6::Multimedia
# 	MediaControlsplugin
# 	MediaPlayerplugin
# 	Configplugin
# )

FetchContent_Declare(
	yaml-cpp
	GIT_REPOSITORY https://github.com/jbeder/yaml-cpp.git
	GIT_TAG master
)
FetchContent_MakeAvailable(yaml-cpp)

target_link_libraries(${PROJECT_NAME} PUBLIC
	yaml-cpp::yaml-cpp
)

target_compile_definitions(${PROJECT_NAME} PUBLIC
	QT_USE_QSTRINGBUILDER
)

include(GNUInstallDirs)
install(TARGETS ${PROJECT_NAME}
	BUNDLE DESTINATION .
	LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
	RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)

get_target_property(qmake_exec_filepath Qt6::qmake IMPORTED_LOCATION)
get_filename_component(qt_exec_bin_dir "${qmake_exec_filepath}" DIRECTORY)
find_program(windeployqt_exec_filepath windeployqt HINTS "${qt_exec_bin_dir}")
add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD
	COMMAND "${CMAKE_COMMAND}" -E
	env PATH="${qt_exec_bin_dir}" "${windeployqt_exec_filepath}"
	"$<TARGET_FILE:${PROJECT_NAME}>"
	COMMENT "Running windeployqt..."
)
