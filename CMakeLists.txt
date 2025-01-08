cmake_minimum_required(VERSION 3.16)

project(VoicebankCreator VERSION 0.1 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD_REQUIRED ON)

include(FetchContent)

find_package(Qt6 REQUIRED COMPONENTS Quick Multimedia LinguistTools)

qt_standard_project_setup(REQUIRES 6.5
	I18N_TRANSLATED_LANGUAGES zh_CN
)

qt_add_executable(VoicebankCreator
	main.cpp
)

qt_add_translations(VoicebankCreator
	TS_FILE_BASE VoicebankCreator
	TS_FILE_DIR Translations
)

if (QT_KNOWN_POLICY_QTP0004)
	qt_policy(SET QTP0004 NEW)
endif ()

set_source_files_properties(Constants.qml PROPERTIES QT_QML_SINGLETON_TYPE TRUE)
set_source_files_properties(Components/Images.qml PROPERTIES QT_QML_SINGLETON_TYPE TRUE)

qt_add_qml_module(VoicebankCreator
	URI VoicebankCreator
	VERSION 1.0
	QML_FILES
		Main.qml
		Constants.qml
		Components/PlayerToolBar.qml
		Components/PlayerPreview.qml
		Components/PlayerToolButton.qml
		Components/PlayerPlaybackButton.qml
		Components/Images.qml
	SOURCES
		mediaformatprovider.h mediaformatprovider.cpp
		includes.h
		boolinq.h
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
)

# Qt for iOS sets MACOSX_BUNDLE_GUI_IDENTIFIER automatically since Qt 6.1.
# If you are developing for iOS or macOS you should consider setting an
# explicit, fixed bundle identifier manually though.
set_target_properties(VoicebankCreator PROPERTIES
	MACOSX_BUNDLE_GUI_IDENTIFIER com.github.otomad.VoicebankCreator
	MACOSX_BUNDLE_BUNDLE_VERSION ${PROJECT_VERSION}
	MACOSX_BUNDLE_SHORT_VERSION_STRING ${PROJECT_VERSION_MAJOR}.${PROJECT_VERSION_MINOR}
	MACOSX_BUNDLE TRUE
	WIN32_EXECUTABLE TRUE
)

target_link_libraries(VoicebankCreator PRIVATE
	Qt6::Quick
	Qt6::Multimedia
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

target_link_libraries(VoicebankCreator PUBLIC
	yaml-cpp::yaml-cpp
)

target_compile_definitions(VoicebankCreator PUBLIC
	QT_USE_QSTRINGBUILDER
)

include(GNUInstallDirs)
install(TARGETS VoicebankCreator
	BUNDLE DESTINATION .
	LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
	RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)
