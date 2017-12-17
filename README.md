# PhotoReorder *(DRAFT VERSION)*
The developer (_JNAttila_) is not responsible for the use of any application from this repository!

## Overview
First of all, this is a *playground*! Every code, each commit in this repository is a result of a game. A game, which is named: DEVELOPMENT!

Secondly. Here are some code what started it's life as a C# project with SVN source control. I used it at home, on my own developer environment. All comments are on native Hungarian (sorry). The Java version (and maybe improved version) of these application are going to be under GIT source control from the first step.

## The project
There are two applications at this project. Both application are working mainly with photo folders.
* a photo reorganizer
* a folder comparer

### PhotoReorder
Try to imagine a general user with a general digital camera with a big enough memory card. The user is made lots of photos on an event. Few days later there is an other event and a lots of new photos, and so on...

On one day the user copy all pictures from the card to the PC, BUT (!!!) the images will be not deleted from the memory card! Several new events, a lots of new photos and a new copy of some earlier images. The user is general, so he or she hasn't any professional process to archive images. Just copy some images into a folder. A few weeks later some new images and several old images will be copied into a new folder.

After a few iteration ~1.0GB photo will need to have ~1.3...1.7GB or more disk-space, because of redundancy. Different folders will contain partially the same photos with lots of duplicated images. It is hard to overview, and much harder to optimize these folders and this folder structure.

In this "theoretical" case, two questions can be came alive:
* which image is copied
* where i placed the image earlier

The *PhotoReorder* can give a really basic solution to this question. You need to give the path of the photo folder and press a button. The application will read all images from the folder and each sub-folder. It reads a few EXIF informations from images. For example: *Camera maker*, *Camera model*, *Date taken*. The application is going to make a new folder with name `__Album__` into the given folder.

There is going to be an [`__Album__`]**/**[*Camera maker*]**/**[*Camera model*]**/**[*Date taken*] folder structure. Each photo from the source folder will place into the right target folder, according to it's EXIF information.

All images without EXIF informations will be copy into a separated folder, like "other" images. A photo without EXIF information is almost impossible, if You think to general photos from a general digital camera without image modifications, or maybe with small rework.

Optionally the application can not just copy, but also move images into the right places.

This description will be better, when I read all code of this application. Please be patient with me.

### FolderComparer
It can happen, that you have some folders with exactly the same files. It would be good to decrease the redundancy, maybe there is a solution.

This application needs only a root-folder, it is going to analyze, and creates a report with the folders contains the same files. The report is only a simple TXT file, so You need to copy the path and open it in `Windows Explorer` or with your favorite file browser.

The application searches only for redundancy. Any further steps depends on You.
