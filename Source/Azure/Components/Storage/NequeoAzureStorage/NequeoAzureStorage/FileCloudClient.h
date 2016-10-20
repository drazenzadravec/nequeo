/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          FileCloudClient.h
*  Purpose :       Cloud client provider class.
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

#pragma once

#include "stdafx.h"

#include "Global.h"
#include "AzureAccount.h"

#include <was\file.h>

#undef CreateDirectory
#undef DeleteFile

namespace Nequeo {
	namespace Azure {
		namespace Storage
		{
			///	<summary>
			///	Cloud client provider.
			///	</summary>
			class EXPORT_NEQUEO_AZURE_STORAGE_API FileCloudClient
			{
			public:
				///	<summary>
				///	Cloud client provider.
				///	</summary>
				/// <param name="account">The Azure account.</param>
				FileCloudClient(const AzureAccount& account);

				///	<summary>
				///	Cloud client provider destructor.
				///	</summary>
				~FileCloudClient();

				/// <summary>
				/// Has the client been initialised.
				/// </summary>
				/// <returns>True if is initialised; else false.</returns>
				inline bool IsInitialised() const
				{
					return _isInitialised;
				}

				///	<summary>
				///	Get the file client.
				///	</summary>
				/// <returns>The file client.</returns>
				const azure::storage::cloud_file_client& FileClient() const;

				///	<summary>
				///	Initialise the File client.
				///	</summary>
				void Initialise();

				///	<summary>
				///	Initialise the File client.
				///	</summary>
				/// <param name="default_request_options">Default file request options.</param>
				void Initialise(const azure::storage::file_request_options& default_request_options);

				///	<summary>
				///	Create a Share.
				///	</summary>
				/// <param name="shareName">The Share name to create.</param>
				/// <returns>True if created; else false.</returns>
				bool CreateShare(const utility::string_t& shareName) const;

				///	<summary>
				///	Delete a Share.
				///	</summary>
				/// <param name="shareName">The Share name to delete.</param>
				/// <returns>True if deleted; else false.</returns>
				bool DeleteShare(const utility::string_t& shareName) const;

				///	<summary>
				///	Get the Share usage size.
				///	</summary>
				/// <param name="shareName">The Share name to get usage size.</param>
				/// <returns>The usage size within the share.</returns>
				const int32_t GetShareSize(const utility::string_t& shareName) const;

				///	<summary>
				///	Set the Share usage size.
				///	</summary>
				/// <param name="shareName">The Share name to set usage size.</param>
				/// <param name="quota">The share quota size to set (GB - gigabytes).</param>
				void SetShareSize(const utility::string_t& shareName, utility::size64_t quota);

				///	<summary>
				///	Create a directory.
				///	</summary>
				/// <param name="shareName">The Share name.</param>
				/// <param name="directoryName">The directory name.</param>
				/// <returns>True if created; else false.</returns>
				bool CreateDirectory(const utility::string_t& shareName, const utility::string_t& directoryName) const;

				///	<summary>
				///	Delete a directory.
				///	</summary>
				/// <param name="shareName">The Share name.</param>
				/// <param name="directoryName">The directory name.</param>
				/// <returns>True if deleted; else false.</returns>
				bool DeleteDirectory(const utility::string_t& shareName, const utility::string_t& directoryName) const;

				///	<summary>
				///	Create a sub-directory.
				///	</summary>
				/// <param name="shareName">The Share name.</param>
				/// <param name="directoryName">The directory name.</param>
				/// <param name="subDirectoryName">The sub-directory name.</param>
				/// <returns>True if created; else false.</returns>
				bool CreateSubDirectory(const utility::string_t& shareName, const utility::string_t& directoryName, const utility::string_t& subDirectoryName) const;

				///	<summary>
				///	Delete a sub-directory.
				///	</summary>
				/// <param name="shareName">The Share name.</param>
				/// <param name="directoryName">The directory name.</param>
				/// <param name="subDirectoryName">The sub-directory name.</param>
				/// <returns>True if deleted; else false.</returns>
				bool DeleteSubDirectory(const utility::string_t& shareName, const utility::string_t& directoryName, const utility::string_t& subDirectoryName) const;

				///	<summary>
				///	Get the file directory reference.
				///	</summary>
				/// <param name="shareName">The Share name.</param>
				/// <returns>The directory reference</returns>
				azure::storage::cloud_file_directory GetRootDirectoryReference(const utility::string_t& shareName);

				///	<summary>
				///	Get the file directory reference.
				///	</summary>
				/// <param name="shareName">The Share name.</param>
				/// <param name="directoryName">The directory name.</param>
				/// <returns>The directory reference</returns>
				azure::storage::cloud_file_directory GetDirectoryReference(const utility::string_t& shareName, const utility::string_t& directoryName);

				///	<summary>
				///	Get the file directory reference.
				///	</summary>
				/// <param name="shareName">The Share name.</param>
				/// <param name="directoryName">The directory name.</param>
				/// <param name="subDirectoryName">The sub-directory name.</param>
				/// <returns>The directory reference</returns>
				azure::storage::cloud_file_directory GetSubDirectoryReference(const utility::string_t& shareName, 
					const utility::string_t& directoryName, const utility::string_t& subDirectoryName);

				///	<summary>
				///	Get the file directory reference.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="subDirectoryName">The sub-directory name.</param>
				/// <returns>The directory reference</returns>
				azure::storage::cloud_file_directory GetSubDirectoryReference(
					azure::storage::cloud_file_directory& directory, const utility::string_t& subDirectoryName);

				///	<summary>
				///	Get the list of files.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <returns>The list of files.</returns>
				std::vector<utility::string_t> ListFiles(azure::storage::cloud_file_directory& directory);

				///	<summary>
				///	Get the list of directories.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <returns>The list of directories.</returns>
				std::vector<utility::string_t> ListDirectories(azure::storage::cloud_file_directory& directory);

				///	<summary>
				///	Get the list of files.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <returns>The list of files.</returns>
				Concurrency::task<std::vector<utility::string_t>> ListFilesAsync(azure::storage::cloud_file_directory& directory);

				///	<summary>
				///	Get the list of directories.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <returns>The list of directories.</returns>
				Concurrency::task<std::vector<utility::string_t>> ListDirectoriesAsync(azure::storage::cloud_file_directory& directory);

				///	<summary>
				///	Upload a file to the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="localFileName">The path and name of the local file to upload.</param>
				void UploadFile(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, const utility::string_t& localFileName);

				///	<summary>
				///	Upload a file to the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="localFileName">The path and name of the local file to upload.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> UploadFileAsync(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, const utility::string_t& localFileName);

				///	<summary>
				///	Upload a file to the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="inputStream">The input stream containing the file to upload.</param>
				void UploadStreamFile(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, Concurrency::streams::istream& inputStream);

				///	<summary>
				///	Upload a file to the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="inputStream">The input stream containing the file to upload.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> UploadStreamFileAsync(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, Concurrency::streams::istream& inputStream);

				///	<summary>
				///	Upload a file to the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="text">The text to upload.</param>
				void UploadTextFile(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, const utility::string_t& text);

				///	<summary>
				///	Upload a file to the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="text">The text to upload.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> UploadTextFileAsync(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, const utility::string_t& text);

				///	<summary>
				///	Download a file from the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <returns>The downloaded text.</returns>
				utility::string_t DownloadTextFile(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName);

				///	<summary>
				///	Download a file from the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<utility::string_t> DownloadTextFileAsync(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName);

				///	<summary>
				///	Download a file from the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="localFileName">The path and name of the local file to upload.</param>
				void DownloadFile(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, const utility::string_t& localFileName);

				///	<summary>
				///	Download a file from the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="localFileName">The path and name of the local file to upload.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> DownloadFileAsync(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, const utility::string_t& localFileName);

				///	<summary>
				///	Download a file from the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="outputStream">The output stream containing the downloaded file.</param>
				void DownloadStreamFile(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, Concurrency::streams::ostream& outputStream);

				///	<summary>
				///	Download a file from the share.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="outputStream">The output stream containing the downloaded file.</param>
				/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
				Concurrency::task<void> DownloadStreamFileAsync(azure::storage::cloud_file_directory& directory,
					const utility::string_t& fileName, Concurrency::streams::ostream& outputStream);

				///	<summary>
				///	Delete a file.
				///	</summary>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <returns>True if deleted; else false.</returns>
				bool DeleteFile(azure::storage::cloud_file_directory& directory, const utility::string_t& fileName) const;

				///	<summary>
				///	Generate a shared access signature for a file or file share.
				///	</summary>
				/// <param name="shareName">The Share name.</param>
				/// <param name="directory">The directory reference.</param>
				/// <param name="fileName">The file name.</param>
				/// <param name="policyName">A policy text.</param>
				/// <param name="contentSAS">A content SAS (Shared Access Signatures (SAS)).</param>
				/// <param name="downloadedText">The returned downloaded text.</param>
				/// <param name="credentialsURL">The returned credentials URL access text.</param>
				/// <param name="permission">The access permission type.</param>
				/// <param name="minutes">The access permission expire time.</param>
				void GenerateSharedAccessSignature(
					const utility::string_t& shareName, 
					azure::storage::cloud_file_directory& directory, 
					const utility::string_t& fileName,
					const utility::string_t& policyName,
					const utility::string_t& contentSAS,
					utility::string_t& downloadedText,
					utility::string_t& credentialsURL,
					uint8_t permission = azure::storage::file_shared_access_policy::permissions::write | azure::storage::file_shared_access_policy::permissions::read,
					unsigned int minutes = 90);

			private:
				bool _disposed;
				bool _isInitialised;

				AzureAccount _account;
				azure::storage::cloud_file_client _client;
			};
		}
	}
}