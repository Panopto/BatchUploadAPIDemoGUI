Panopto-BatchUploadAPIDemoGUI
=====================

Panopto Batch Upload API Demo GUI

This program uses Panopto RESTful API and C# to accomplish upload to server.

This program will need AWSSDK to run as it uses Amazon S3 Services.

AWSSDK can be downloaded from here: http://aws.amazon.com/s3/

This program is explained below:

Upload file in specified MODE where FILE column holds URI to session with same name as file in folder with ID as FOLDERID on SERVER logged in with USERNAME and USERPASSWORD

	SERVER: Target server
	USERNAME: User name to access the server
	USERPASSWORD: Password for the user
	FOLDERID: ID of the destination folder
	MODE: URI type (Type of file location)
	FILE: Target upload file path relative to current directory
