Stream.cs (Within the file) ******************************************************************************************************************************

FILENAME_STRUCTURE_BUFFER_SIZE = 100 bytes, The name of the file.
FILE_SIZE_STRUCTURE_BUFFER_SIZE = 12 bytes, The size of the file; including the structure data.

e.g. (Within the file) - At the top of the file
Apple.txt                                   45667
data in file
--
--
end data


StreamFileQueryHandler.cs (Within the query string) ******************************************************************************************************

F = The name of the file to download.
D = The name of the file to delete.
L = Get the list of files.

e.g. (Within the query string)
http://www.domain.com/folder/StreamFileQueryHandler.ashx?F=Apple.txt


StreamHandler.cs (Within the file) ***********************************************************************************************************************

OPERATION_STRUCTURE_BUFFER_SIZE = 2 bytes, The operation to perform.
FILENAME_STRUCTURE_BUFFER_SIZE = 98 bytes, The name of the file.
FILE_SIZE_STRUCTURE_BUFFER_SIZE = 12 bytes, The size of the file; including the structure data.
FILE_POSITION_STRUCTURE_BUFFER_SIZE = 12 bytes, The position within the file to start downloading/uploading from.

UP = Upload the file name within (FILENAME_STRUCTURE_BUFFER_SIZE).
DN = Download the file within (FILENAME_STRUCTURE_BUFFER_SIZE).
SZ = The size of the file within (FILENAME_STRUCTURE_BUFFER_SIZE).

e.g. (Within the file) - At the top of the file
UPApple.txt                                   45667       10000
data in file
--
--
end data


StreamQueryHandler.cs (Within the query string) **********************************************************************************************************

O = The operation to perform.
F = The file name to Get/Set.
L = The location (directory) of the file.
S = The file size to Get/Set.
R = The URI of the operation.
P = The position within the file to start downloading/uploading from.

UP = Upload file operation, F = The name of the file to create on the server, (optional L = one|two|three (equivalent directory structure 'one\two\three'), P = 10000 (start from position 10000 within the file)).
DN = Download file operation, F = The name of the file to get from the server, (optional L = one|two|three (equivalent directory structure 'one\two\three'), P = 10000 (start from position 10000 within the file)).
SZ = File size operation, F = The name of the file to get the size the server.
DL = Delete file operation, F = The name of the file to delete from the server.
DR = Download resource file operation, F = The name of the file to create on the server for the resourse, R = The resource name to download from the server.
UR = Upload resource file operation, F = The name of the file to upload to the resource, R = The resource name to upload to the server.

e.g. (Within the query string)
http://www.domain.com/folder/StreamQueryHandler.ashx?O=UP&F=Apple.txt
http://www.domain.com/folder/StreamQueryHandler.ashx?O=UP&F=Apple.txt&P=10000
http://www.domain.com/folder/StreamQueryHandler.ashx?O=DN&F=Apple.txt
http://www.domain.com/folder/StreamQueryHandler.ashx?O=DN&F=Apple.txt&P=10000
http://www.domain.com/folder/StreamQueryHandler.ashx?O=UP&F=Apple.txt&L=one|two|three
http://www.domain.com/folder/StreamQueryHandler.ashx?O=DN&F=Apple.txt&L=one|two|three
http://www.domain.com/folder/StreamQueryHandler.ashx?O=DR&F=Apple.txt&R=Http://www.company.com/folder/Apple.txt
http://www.domain.com/folder/StreamQueryHandler.ashx?O=UR&F=Apple.txt&R=Http://www.company.com/folder/Apple.txt