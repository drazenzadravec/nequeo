/* Company :       Nequeo Pty Ltd, http://www.nequeo.com.au/
*  Copyright :     Copyright © Nequeo Pty Ltd 2016 http://www.nequeo.com.au/
*
*  File :          FileCloudClient.cpp
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

#include "stdafx.h"

#include "FileCloudClient.h"

using namespace Nequeo::Azure::Storage;

///	<summary>
///	Cloud client provider.
///	</summary>
/// <param name="account">The Azure account.</param>
FileCloudClient::FileCloudClient(const AzureAccount& account) : _disposed(false), _isInitialised(false), _account(account)
{
}

///	<summary>
///	Cloud client provider.
///	</summary>
FileCloudClient::~FileCloudClient()
{
	if (!_disposed)
	{
		_disposed = true;
		_isInitialised = false;
	}
}

///	<summary>
///	Get the file client.
///	</summary>
/// <returns>The file client.</returns>
const azure::storage::cloud_file_client& FileCloudClient::FileClient() const
{
	return _client;
}

///	<summary>
///	Initialise the File client.
///	</summary>
void FileCloudClient::Initialise()
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_file_client();
		_isInitialised = true;
	}
}

///	<summary>
///	Initialise the File client.
///	</summary>
/// <param name="default_request_options">Default file request options.</param>
void FileCloudClient::Initialise(const azure::storage::file_request_options& default_request_options)
{
	// If not initialised;
	if (!_isInitialised)
	{
		_client = _account._account.create_cloud_file_client(default_request_options);
		_isInitialised = true;
	}
}

///	<summary>
///	Create a Share.
///	</summary>
/// <param name="shareName">The Share name to create.</param>
/// <returns>True if created; else false.</returns>
bool FileCloudClient::CreateShare(const utility::string_t& shareName) const
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	// Create the share if it doesn't exist.
	return share.create_if_not_exists();
}

///	<summary>
///	Delete a Share.
///	</summary>
/// <param name="shareName">The Share name to delete.</param>
/// <returns>True if deleted; else false.</returns>
bool FileCloudClient::DeleteShare(const utility::string_t& shareName) const
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	// Delete the share if it exists.
	return share.delete_share_if_exists();
}

///	<summary>
///	Get the Share usage size.
///	</summary>
/// <param name="shareName">The Share name to get usage size.</param>
/// <returns>The usage size within the share.</returns>
const int32_t FileCloudClient::GetShareSize(const utility::string_t& shareName) const
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	if (share.exists())
	{
		// Return the share usage.
		return share.download_share_usage();
	}
	else
	{
		// Share does not exist.
		return 0;
	}
}

///	<summary>
///	Set the Share usage size.
///	</summary>
/// <param name="shareName">The Share name to set usage size.</param>
/// <param name="quota">The share quota size to set (GB - gigabytes).</param>
void FileCloudClient::SetShareSize(const utility::string_t& shareName, utility::size64_t quota)
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	if (share.exists())
	{
		// Set the quota.
		share.resize(quota);
	}
}

///	<summary>
///	Create a directory.
///	</summary>
/// <param name="shareName">The Share name.</param>
/// <param name="directoryName">The directory name.</param>
/// <returns>True if created; else false.</returns>
bool FileCloudClient::CreateDirectory(const utility::string_t& shareName, const utility::string_t& directoryName) const
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	//Get a reference to the root directory for the share.
	azure::storage::cloud_file_directory root_dir = share.get_directory_reference(directoryName);

	// Return value is true if the share did not exist and was successfully created.
	return root_dir.create_if_not_exists();
}

///	<summary>
///	Delete a directory.
///	</summary>
/// <param name="shareName">The Share name.</param>
/// <param name="directoryName">The directory name.</param>
/// <returns>True if deleted; else false.</returns>
bool FileCloudClient::DeleteDirectory(const utility::string_t& shareName, const utility::string_t& directoryName) const
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	//Get a reference to the root directory for the share.
	azure::storage::cloud_file_directory root_dir = share.get_directory_reference(directoryName);

	// Delete the directory if it exists.
	return root_dir.delete_directory_if_exists();
}

///	<summary>
///	Create a sub-directory.
///	</summary>
/// <param name="shareName">The Share name.</param>
/// <param name="directoryName">The directory name.</param>
/// <param name="subDirectoryName">The sub-directory name.</param>
/// <returns>True if created; else false.</returns>
bool FileCloudClient::CreateSubDirectory(const utility::string_t& shareName, const utility::string_t& directoryName, const utility::string_t& subDirectoryName) const
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	//Get a reference to the root directory for the share.
	azure::storage::cloud_file_directory root_dir = share.get_directory_reference(directoryName);

	// Create a subdirectory.
	azure::storage::cloud_file_directory subdirectory = root_dir.get_subdirectory_reference(subDirectoryName);
	return subdirectory.create_if_not_exists();
}

///	<summary>
///	Delete a sub-directory.
///	</summary>
/// <param name="shareName">The Share name.</param>
/// <param name="directoryName">The directory name.</param>
/// <param name="subDirectoryName">The sub-directory name.</param>
/// <returns>True if deleted; else false.</returns>
bool FileCloudClient::DeleteSubDirectory(const utility::string_t& shareName, const utility::string_t& directoryName, const utility::string_t& subDirectoryName) const
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	//Get a reference to the root directory for the share.
	azure::storage::cloud_file_directory root_dir = share.get_directory_reference(directoryName);

	// Create a subdirectory.
	azure::storage::cloud_file_directory subdirectory = root_dir.get_subdirectory_reference(subDirectoryName);
	return subdirectory.delete_directory_if_exists();
}

///	<summary>
///	Get the file directory reference.
///	</summary>
/// <param name="shareName">The Share name.</param>
/// <returns>The directory reference</returns>
azure::storage::cloud_file_directory FileCloudClient::GetRootDirectoryReference(const utility::string_t& shareName)
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);
	return share.get_root_directory_reference();
}

///	<summary>
///	Get the file directory reference.
///	</summary>
/// <param name="shareName">The Share name.</param>
/// <param name="directoryName">The directory name.</param>
/// <returns>The directory reference</returns>
azure::storage::cloud_file_directory FileCloudClient::GetDirectoryReference(const utility::string_t& shareName, const utility::string_t& directoryName)
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);
	return share.get_directory_reference(directoryName);
}

///	<summary>
///	Get the file directory reference.
///	</summary>
/// <param name="shareName">The Share name.</param>
/// <param name="directoryName">The directory name.</param>
/// <param name="subDirectoryName">The sub-directory name.</param>
/// <returns>The directory reference</returns>
azure::storage::cloud_file_directory FileCloudClient::GetSubDirectoryReference(const utility::string_t& shareName,
	const utility::string_t& directoryName, const utility::string_t& subDirectoryName)
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);
	azure::storage::cloud_file_directory root_dir = share.get_directory_reference(directoryName);
	return root_dir.get_subdirectory_reference(subDirectoryName);
}

///	<summary>
///	Get the file directory reference.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="subDirectoryName">The sub-directory name.</param>
/// <returns>The directory reference</returns>
azure::storage::cloud_file_directory FileCloudClient::GetSubDirectoryReference(
	azure::storage::cloud_file_directory& directory, const utility::string_t& subDirectoryName)
{
	return directory.get_subdirectory_reference(subDirectoryName);
}

///	<summary>
///	Get the list of files.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <returns>The list of files.</returns>
std::vector<utility::string_t> FileCloudClient::ListFiles(azure::storage::cloud_file_directory& directory)
{
	std::vector<utility::string_t> files;

	// Output URI of each item.
	azure::storage::list_file_and_diretory_result_iterator end_of_results;

	// For each file.
	for (auto it = directory.list_files_and_directories(); it != end_of_results; ++it)
	{
		// If file.
		if (it->is_file())
		{
			files.push_back(it->as_file().uri().primary_uri().to_string());
		}
	}

	// Return the files.
	return files;
}

///	<summary>
///	Get the list of directories.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <returns>The list of directories.</returns>
std::vector<utility::string_t> FileCloudClient::ListDirectories(azure::storage::cloud_file_directory& directory)
{
	std::vector<utility::string_t> directories;

	// Output URI of each item.
	azure::storage::list_file_and_diretory_result_iterator end_of_results;

	// For each directory.
	for (auto it = directory.list_files_and_directories(); it != end_of_results; ++it)
	{
		// Is directory.
		if (it->is_directory())
		{
			directories.push_back(it->as_directory().uri().primary_uri().to_string());
		}
	}

	// Return the directories.
	return directories;
}

///	<summary>
///	Get the list of files.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <returns>The list of files.</returns>
Concurrency::task<std::vector<utility::string_t>> FileCloudClient::ListFilesAsync(azure::storage::cloud_file_directory& directory)
{
	// Create a new internal task.
	return Concurrency::create_task([this, &directory]() -> std::vector<utility::string_t>
	{
		return ListFiles(directory);
	});
}

///	<summary>
///	Get the list of directories.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <returns>The list of directories.</returns>
Concurrency::task<std::vector<utility::string_t>> FileCloudClient::ListDirectoriesAsync(azure::storage::cloud_file_directory& directory)
{
	// Create a new internal task.
	return Concurrency::create_task([this, &directory]() -> std::vector<utility::string_t>
	{
		return ListDirectories(directory);
	});
}

///	<summary>
///	Upload a file to the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="localFileName">The path and name of the local file to upload.</param>
void FileCloudClient::UploadFile(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, const utility::string_t& localFileName)
{
	// Upload a file from a file.
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	file.upload_from_file(localFileName);
}

///	<summary>
///	Upload a file to the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="localFileName">The path and name of the local file to upload.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> FileCloudClient::UploadFileAsync(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, const utility::string_t& localFileName)
{
	// Upload a file from a file.
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	return file.upload_from_file_async(localFileName);
}

///	<summary>
///	Upload a file to the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="inputStream">The input stream containing the file to upload.</param>
void FileCloudClient::UploadStreamFile(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, Concurrency::streams::istream& inputStream)
{
	// Upload a file from a file.
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	file.upload_from_stream(inputStream);
}

///	<summary>
///	Upload a file to the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="inputStream">The input stream containing the file to upload.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> FileCloudClient::UploadStreamFileAsync(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, Concurrency::streams::istream& inputStream)
{
	// Upload a file from a file.
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	return file.upload_from_stream_async(inputStream);
}

///	<summary>
///	Upload a file to the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="text">The text to upload.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
void FileCloudClient::UploadTextFile(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, const utility::string_t& text)
{
	// Upload a file from a file.
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	file.upload_text(text);
}

///	<summary>
///	Upload a file to the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="text">The text to upload.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> FileCloudClient::UploadTextFileAsync(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, const utility::string_t& text)
{
	// Upload a file from a file.
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	return file.upload_text_async(text);
}

///	<summary>
///	Download a file from the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <returns>The downloaded text.</returns>
utility::string_t FileCloudClient::DownloadTextFile(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName)
{
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	return file.download_text();
}

///	<summary>
///	Download a file from the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<utility::string_t> FileCloudClient::DownloadTextFileAsync(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName)
{
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	return file.download_text_async();
}

///	<summary>
///	Download a file from the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="localFileName">The path and name of the local file to upload.</param>
void FileCloudClient::DownloadFile(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, const utility::string_t& localFileName)
{
	azure::storage::cloud_file file = directory.get_file_reference(fileName);

	// Save data contents to a file.
	concurrency::streams::container_buffer<std::vector<uint8_t>> buffer;
	concurrency::streams::ostream output_stream(buffer);
	file.download_to_stream(output_stream);

	std::ofstream outfile(localFileName, std::ofstream::binary);
	std::vector<unsigned char>& data = buffer.collection();

	outfile.write((char *)&data[0], buffer.size());
	outfile.close();
}

///	<summary>
///	Download a file from the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="localFileName">The path and name of the local file to upload.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> FileCloudClient::DownloadFileAsync(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, const utility::string_t& localFileName)
{
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	return file.download_to_file_async(localFileName);
}

///	<summary>
///	Download a file from the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="outputStream">The output stream containing the downloaded file.</param>
void FileCloudClient::DownloadStreamFile(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, Concurrency::streams::ostream& outputStream)
{
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	file.download_to_stream(outputStream);
}

///	<summary>
///	Download a file from the share.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <param name="outputStream">The output stream containing the downloaded file.</param>
/// <returns>A <see cref="Concurrency::task"/> object that represents the current operation.</returns>
Concurrency::task<void> FileCloudClient::DownloadStreamFileAsync(azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName, Concurrency::streams::ostream& outputStream)
{
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	return file.download_to_stream_async(outputStream);
}

///	<summary>
///	Delete a file.
///	</summary>
/// <param name="directory">The directory reference.</param>
/// <param name="fileName">The file name.</param>
/// <returns>True if deleted; else false.</returns>
bool FileCloudClient::DeleteFile(azure::storage::cloud_file_directory& directory, const utility::string_t& fileName) const
{
	azure::storage::cloud_file file = directory.get_file_reference(fileName);
	return file.delete_file_if_exists();
}

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
void FileCloudClient::GenerateSharedAccessSignature(
	const utility::string_t& shareName, 
	azure::storage::cloud_file_directory& directory,
	const utility::string_t& fileName,
	const utility::string_t& policyName,
	const utility::string_t& contentSAS,
	utility::string_t& downloadedText,
	utility::string_t& credentialsURL,
	uint8_t permission,
	unsigned int minutes)
{
	// Retrieve a reference to a share.
	azure::storage::cloud_file_share share = _client.get_share_reference(shareName);

	// If share exists.
	if (share.exists())
	{
		// Create and assign a policy
		utility::string_t policy_name = policyName;
		azure::storage::file_shared_access_policy sharedPolicy = azure::storage::file_shared_access_policy();

		//set permissions to expire in xx minutes.
		sharedPolicy.set_expiry(utility::datetime::utc_now() + utility::datetime::from_minutes(minutes));

		//give read and write permissions
		sharedPolicy.set_permissions(permission);

		//set permissions for the share
		azure::storage::file_share_permissions permissions;

		//retrieve the current list of shared access policies
		azure::storage::shared_access_policies<azure::storage::file_shared_access_policy> policies;

		//add the new shared policy
		policies.insert(std::make_pair(policy_name, sharedPolicy));

		//save the updated policy list
		permissions.set_policies(policies);
		share.upload_permissions(permissions);

		//Retrieve the root directory and file references
		azure::storage::cloud_file file = directory.get_file_reference(fileName);

		// Generate a SAS for a file in the share 
		//  and associate this access policy with it.       
		utility::string_t sas_token = file.get_shared_access_signature(sharedPolicy);

		// Create a new CloudFile object from the SAS, and write some text to the file.     
		azure::storage::cloud_file file_with_sas(azure::storage::storage_credentials(sas_token).transform_uri(file.uri().primary_uri()));
		utility::string_t text = contentSAS;
		file_with_sas.upload_text(text);

		// Download and print URL with SAS.
		downloadedText = file_with_sas.download_text();
		credentialsURL = azure::storage::storage_credentials(sas_token).transform_uri(file.uri().primary_uri()).to_string();
	}
}