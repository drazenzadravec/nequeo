/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          MP4TransportFileServerMedia.cpp
*  Purpose :       MP4 Media Server class.
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

#include "MP4TransportFileServerMedia.h"

/// <summary>
/// MP4 live media server.
/// </summary>
MP4TransportFileServerMedia::MP4TransportFileServerMedia() {}

/// <summary>
/// Create the MP4 SMS.
/// </summary>
/// <param name="sms">The current session.</param>
/// <param name="env">The user environment.</param>
/// <param name="fileName">The filename.</param>
/// <param name="reuseSource">The resource reuse.</param>
void MP4TransportFileServerMedia::createNewMP4SMS(ServerMediaSession* sms, UsageEnvironment& env, char const* fileName, Boolean reuseSource)
{
	// get the info about file
	char * command = (char *)malloc(150 * sizeof(char));
	command = strcat(command, "mp4info  ");
	command = strcat(command, fileName);
	command = strcat(command, " >temp ");
	puts(command);
	system(command);

	// parsing code
	FILE * fTemp = fopen("temp", "r"), *temp = NULL;

	// If the file has been created.
	if (fTemp != NULL)
	{
		char c, ext[4];
		ext[0] = '.';
		ext[1] = 't';
		ext[3] = '\0';

		char * Word = (char *)malloc(100 * sizeof(char));
		int flagLine = 0, lineCount = 0, flagWord = 0, wordCount = 0, i = 0, flagCodec = 0, streamCount = 1;

		// Until end.
		while (!feof(fTemp)) 
		{
			if (lineCount != 3) 
			{
				c = getc(fTemp);
			}

			if (flagLine == 1) 
			{
				flagLine = 0;

				if ((c > 48) && (c < 59)) 
				{
					// get inside the stream numbers only ...
					flagWord = 1;

					while ((c != '\n') && (!feof(fTemp))) 
					{
						c = getc(fTemp);

						if (flagWord == 1) 
						{
							i = 0;
							while ((c != ' ') && (c != '\t') && (!feof(fTemp))) 
							{
								Word[i] = tolower(c);
								i++;
								c = getc(fTemp);
							}
							Word[i] = '\0';

							if ((strcmp("video", Word) == 0) || (strcmp("audio", Word) == 0)) 
							{
								flagCodec = 1;
								wordCount = 0;
								int i;
								for (i = 0; i < 100; i++) 
								{
									Word[i] = '\n';
								}
								ext[2] = '0' + streamCount;
								strcpy(Word, "mp4creator -extract=");
								Word[20] = streamCount + 48;
								Word[21] = ' ';
								Word[22] = '\0';
								Word = strcat(Word, fileName);
								puts(Word);

								command = strcpy(command, fileName);
								command = strcat(command, ext);
								temp = fopen(command, "r");
								if (temp == NULL) 
								{
									env << "creating files";
									system(Word);
								}
								puts(command);
								streamCount++;
							}
							if ((flagCodec == 1) && (wordCount == 1)) 
							{
								if (strcmp("h264", Word) == 0) 
								{
									// error cant play H.264 files.
									goto cleanup;
								}
							}
							if ((flagCodec == 1) && (wordCount == 2)) 
							{
								flagCodec = 0;

								///////////////////////////////enter the code here////////////////////////////////
								if (strcmp("aac", Word) == 0) 
								{
									puts("aac found");
									sms->addSubsession(ADTSAudioFileServerMediaSubsession
										::createNew(env, command, reuseSource));
									puts(ext);
								}
								else if (strcmp("simple", Word) == 0) 
								{
									puts("m4e found");
									sms->addSubsession(MPEG4VideoFileServerMediaSubsession
										::createNew(env, command, reuseSource));
									puts(ext);
								}
								else if (strcmp("h264", Word) == 0) 
								{
									puts("m4e found");
									sms->addSubsession(MPEG4VideoFileServerMediaSubsession
										::createNew(env, command, reuseSource));
									puts(ext);
								}
								else if (strcmp("amr", Word) == 0) 
								{
									puts("amr found");
									sms->addSubsession(AMRAudioFileServerMediaSubsession
										::createNew(env, command, reuseSource));
									puts(ext);
								}
							}
							flagWord = 0; ////////// the word flag is reset
						}
						if ((c == '\t') || (c == ' ')) 
						{
							wordCount++;
							flagWord = 1; // the word flag set for getting next word.
						}
					}

					flagWord = 0;
					wordCount = 0;
					goto out;
				}
			}
		out:
			if (c == '\n') 
			{
				lineCount++;
				if (lineCount > 2) 
				{
					flagLine = 1;
				}
			}
		}

	cleanup:
		// Close the file.
		fclose(fTemp);
		fclose(temp);
		return;
	}
}