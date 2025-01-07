cmake_minimum_required(VERSION 3.16)

project(VoicebankCreator VERSION 0.1 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(Qt6 REQUIRED COMPONENTS Quick Multimedia yaml-cpp boolinq)

qt_standard_project_setup(REQUIRES 6.5)

qt_add_executable(appVoicebankCreator
	main.cpp
)

qt_add_qml_module(appVoicebankCreator
	URI VoicebankCreator
	VERSION 1.0
	QML_FILES
	Main.qml
	RESOURCES
	RESOURCES vcpkg.json
	QML_FILES Controls/PlayerToolBar.qml
	SOURCES mediaformatprovider.h mediaformatprovider.cpp
	SOURCES includes.h
)

# Qt for iOS sets MACOSX_BUNDLE_GUI_IDENTIFIER automatically since Qt 6.1.
# If you are developing for iOS or macOS you should consider setting an
# explicit, fixed bundle identifier manually though.
set_target_properties(appVoicebankCreator PROPERTIES
#    MACOSX_BUNDLE_GUI_IDENTIFIER com.example.appVoicebankCreator
MACOSX_BUNDLE_BUNDLE_VERSION ${PROJECT_VERSION}
MACOSX_BUNDLE_SHORT_VERSION_STRING ${PROJECT_VERSION_MAJOR}.${PROJECT_VERSION_MINOR}
MACOSX_BUNDLE TRUE
WIN32_EXECUTABLE TRUE
)

target_link_libraries(appVoicebankCreator PRIVATE
	Qt6::Quick
	Qt6::Multimedia
	yaml-cpp
	boolinq
)

# target_link_libraries(MediaPlayerApp PRIVATE
#     Qt6::Core
#     Qt6::Svg
#     Qt6::Quick
#     Qt6::Multimedia
#     MediaControlsplugin
#     MediaPlayerplugin
#     Configplugin
# )

target_compile_definitions(appVoicebankCreator PUBLIC
	QT_USE_QSTRINGBUILDER
)

include(GNUInstallDirs)
install(TARGETS appVoicebankCreator
	BUNDLE DESTINATION .
	LIBRARY DESTINATION ${CMAKE_INSTALL_LIBDIR}
	RUNTIME DESTINATION ${CMAKE_INSTALL_BINDIR}
)
