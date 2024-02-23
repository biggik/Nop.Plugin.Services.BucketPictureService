# Nop.Plugin.Misc.BucketPictureService
This is a nopCommerce plugin that generates thumb images in "buckets" instead of a flat file structure.

This works much better on Windows than the default implementation, and also better than the *Multiple thumb directories* setting under *Media Settings*, since that only uses the first three letters of the thumbnail image name, and the first 8 characters are the picture id.

This implementation uses the picture id, but by bucketing thumb images in a two-level hierarchy. The first level is the *thousand* part, e.g. all pictures with an id under 10000 will be in folder 0, all pictures between 10000 and 19999 in folder 10, etc.
The second level is the *hundred* part, which divides the 10000 numbers into 100 parts (so maximum of 100 subfolders).

Examples:
- Picture id is 123, then the thumb would be in a subfolder *0\100*
- Picture id is 13910, then the thumb would be in a subfolder *10\3900*
- Picture id is 20999, then the thumb would be in a subfolder *20\9900*


# Building
## Directory structure
The repository contains separate directories for each nopCommerce version, e.g. 4.60, 4.70, and so on.

## NopCommerce source
Each *Nop.Plugin.Misc.BucketPictureService* project contains references to nopCommerce projects. For convenience the nopCommerce files are references via drive *n:*. It is recommended to map n: to the location of the nopCommerce source. For example, if nopCommerce source files are available in *c:\dev\nopCommerce public releases*, and each version in a subdirectory named by the release (i.e. *nopCommerce 4.20*) then the following command will map n: to that location

    net use n: "\\mycomputer\c$\dev\nopCommerce public releases" /persistent:yes

where mycomputer is the name of the development machine.
 
 ## Build the projects
 To build the projects, either open the preferred solution (.sln) file, or execute the build-all.cmd file in the root directory. That command builds all versions both in debug and release mode.

The output of the build, whether from Visual Studio, or from the build-all.cmd file, goes to the *_build* directory in the root directory
