========================================================================
    STATIC LIBRARY : NequeoIOCompression Project Overview
========================================================================

AppWizard has created this NequeoIOCompression library project for you.

This file contains a summary of what you will find in each of the files that
make up your NequeoIOCompression application.


NequeoIOCompression.vcxproj
    This is the main project file for VC++ projects generated using an Application Wizard.
    It contains information about the version of Visual C++ that generated the file, and
    information about the platforms, configurations, and project features selected with the
    Application Wizard.

NequeoIOCompression.vcxproj.filters
    This is the filters file for VC++ projects generated using an Application Wizard. 
    It contains information about the association between the files in your project 
    and the filters. This association is used in the IDE to show grouping of files with
    similar extensions under a specific node (for e.g. ".cpp" files are associated with the
    "Source Files" filter).


/////////////////////////////////////////////////////////////////////////////

StdAfx.h, StdAfx.cpp
    These files are used to build a precompiled header (PCH) file
    named NequeoIOCompression.pch and a precompiled types file named StdAfx.obj.

/////////////////////////////////////////////////////////////////////////////
Other notes:

AppWizard uses "TODO:" comments to indicate parts of the source code you
should add to or customize.

/////////////////////////////////////////////////////////////////////////////



using namespace boost::iostreams; 
filtering_ostream out; 
out.push(gzip_compressor()); 
out.push(file_sink("test.gz", std::ios::binary)); 
out << "This is a gz file\n"; 

This will create a gz file containing a single compressed file (called 'test'). 
 I can change the name of this internal file using a gzip_params struct and all 
is well. 

However, I want to create a single gzip file that contains multiple compressed 
files, each with a different filename.  Does anyone know how to achieve this? 




Boost IO streams GZIP Tutorials

First you will need to compile boost with IO streams with encoding into Gzip support. How to build Boost with IO streams encoding support is described here 

How to Gzip encode a string or a file with Boost IO streams

Based on official tutorial, shows how to encode into gzip. 

Code
#include <iostream>
#include <sstream>
#include <string>
#include <fstream>

#include <boost/iostreams/filtering_streambuf.hpp>
#include <boost/iostreams/copy.hpp>
#include <boost/iostreams/filter/gzip.hpp>
#include <boost/iostreams/stream.hpp>


int main(  )
{
        std::cout << "create data." << std::endl;
        std::string data_part_1 = "rock in clouds";
        std::string data_part_2 = "!";

        std::cout << "encode data:" << std::endl;
        {
                std::ofstream file( std::string( "hello world.gz" ).c_str(),  std::ofstream::binary);
                boost::iostreams::filtering_streambuf< boost::iostreams::input> in;
                in.push( boost::iostreams::gzip_compressor());
                std::stringstream data;
                data << data_part_1 << data_part_2;
                in.push(data);
                boost::iostreams::copy(in, file);
        }
                std::cout << "data encoded." << std::endl;
        std::cin.get();
        return 0;
}

Output
create data.
encode data:
data encoded.

And here is generated file 

How to Gzip encode entire folder with Boost IO streams

You will need a cross platform header only hpp class `lindenb::io::Tar` which you can download from here to put multiple files into tar archive and then gzip it. 

Code
#include <iostream>
#include <fstream>
#include <cstdlib>
#include <sstream>
#include <string>
#include <algorithm>

#include <boost/filesystem.hpp>
#include <boost/iostreams/filtering_streambuf.hpp>
#include <boost/iostreams/copy.hpp>
#include <boost/iostreams/filter/gzip.hpp>
#include <boost/iostreams/stream.hpp>

#include <lindenb/io/tarball.hpp>

void is_file( boost::filesystem::path p, lindenb::io::Tar & archive )
{
        std::string file_name_in_archive = p.relative_path().normalize().string();
        std::replace(file_name_in_archive.begin(), file_name_in_archive.end(), '\\', '/');
        file_name_in_archive = file_name_in_archive.substr(2, file_name_in_archive.size());
        archive.putFile(p.string().c_str(), file_name_in_archive.c_str());
}

void is_dir( boost::filesystem::path dir, lindenb::io::Tar & archive )
{
        if(!boost::filesystem::exists(dir))
        {
                return;
        }
        //create_file(dir, old_fs, new_fs);
        if (boost::filesystem::is_directory(dir))
        {
                boost::filesystem::directory_iterator dirIter( dir );
                boost::filesystem::directory_iterator dirIterEnd;

                while ( dirIter != dirIterEnd )
                {
                        if ( boost::filesystem::exists( *dirIter ) && !boost::filesystem::is_directory( *dirIter ) )
                        {
                                try
                                {
                                        is_file((*dirIter), archive);
                                }
                                catch(std::exception){}
                        }
                        else
                        {
                                try
                                {
                                        is_dir((*dirIter), archive);
                                }
                                catch(std::exception){}
                        }
                        ++dirIter;
                }
        }
}

int main(int argc,char** argv)
{
        std::stringstream out("pseudofile");
        lindenb::io::Tar tarball(out);
        is_dir("./myfiles", tarball);
        tarball.finish();
        {
                std::ofstream file( std::string( "hello folder.tar.gz" ).c_str(),  std::ofstream::binary);
                boost::iostreams::filtering_streambuf< boost::iostreams::input> in;
                in.push( boost::iostreams::gzip_compressor());
                in.push(out);
                boost::iostreams::copy(in, file);
        }
        std::cin.get();
        return 0;
}













Minizip does have an example programs to demonstrate its usage - the files are called minizip.c and miniunz.c. 

Update: I had a few minutes so I whipped up this quick, bare bones example for you. It's very smelly C, and I wouldn't use 
it without major improvements. Hopefully it's enough to get you going for now.

// uzip.c - Simple example of using the minizip API.
// Do not use this code as is! It is educational only, and probably
// riddled with errors and leaks!
#include <stdio.h>
#include <string.h>

#include "unzip.h"

#define dir_delimter '/'
#define MAX_FILENAME 512
#define READ_SIZE 8192

int main( int argc, char **argv )
{
    if ( argc < 2 )
    {
        printf( "usage:\n%s {file to unzip}\n", argv[ 0 ] );
        return -1;
    }

    // Open the zip file
    unzFile *zipfile = unzOpen( argv[ 1 ] );
    if ( zipfile == NULL )
    {
        printf( "%s: not found\n" );
        return -1;
    }

    // Get info about the zip file
    unz_global_info global_info;
    if ( unzGetGlobalInfo( zipfile, &global_info ) != UNZ_OK )
    {
        printf( "could not read file global info\n" );
        unzClose( zipfile );
        return -1;
    }

    // Buffer to hold data read from the zip file.
    char read_buffer[ READ_SIZE ];

    // Loop to extract all files
    uLong i;
    for ( i = 0; i < global_info.number_entry; ++i )
    {
        // Get info about current file.
        unz_file_info file_info;
        char filename[ MAX_FILENAME ];
        if ( unzGetCurrentFileInfo(
            zipfile,
            &file_info,
            filename,
            MAX_FILENAME,
            NULL, 0, NULL, 0 ) != UNZ_OK )
        {
            printf( "could not read file info\n" );
            unzClose( zipfile );
            return -1;
        }

        // Check if this entry is a directory or file.
        const size_t filename_length = strlen( filename );
        if ( filename[ filename_length-1 ] == dir_delimter )
        {
            // Entry is a directory, so create it.
            printf( "dir:%s\n", filename );
            mkdir( filename );
        }
        else
        {
            // Entry is a file, so extract it.
            printf( "file:%s\n", filename );
            if ( unzOpenCurrentFile( zipfile ) != UNZ_OK )
            {
                printf( "could not open file\n" );
                unzClose( zipfile );
                return -1;
            }

            // Open a file to write out the data.
            FILE *out = fopen( filename, "wb" );
            if ( out == NULL )
            {
                printf( "could not open destination file\n" );
                unzCloseCurrentFile( zipfile );
                unzClose( zipfile );
                return -1;
            }

            int error = UNZ_OK;
            do    
            {
                error = unzReadCurrentFile( zipfile, read_buffer, READ_SIZE );
                if ( error < 0 )
                {
                    printf( "error %d\n", error );
                    unzCloseCurrentFile( zipfile );
                    unzClose( zipfile );
                    return -1;
                }

                // Write data to file.
                if ( error > 0 )
                {
                    fwrite( read_buffer, error, 1, out ); // You should check return of fwrite...
                }
            } while ( error > 0 );

            fclose( out );
        }

        unzCloseCurrentFile( zipfile );

        // Go the the next entry listed in the zip file.
        if ( ( i+1 ) < global_info.number_entry )
        {
            if ( unzGoToNextFile( zipfile ) != UNZ_OK )
            {
                printf( "cound not read next file\n" );
                unzClose( zipfile );
                return -1;
            }
        }
    }

    unzClose( zipfile );

    return 0;
}