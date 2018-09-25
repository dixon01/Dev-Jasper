Dynamic Content HowTo
=====================
This short document describes how dynamic content creation is supposed to work.

There are three parties involved in the creation of dynamic content:
a) icenter.media needs to define where dynamic content is to be placed (layout and cycles).
b) The background system must provide the dynamic content when it was updated on a remote system (e.g. a web server providing the live information)
c) The imotion software must know how to handle the dynamic content coming from the background system. This is usually done by Composer or Protran.

1. Dynamic Content Types
------------------------
There are two types of dynamic content:
a) File-based content generated on the background system
b) On-the-fly (textual) information

An example for file-based content is a CMS (e.g. ScreenGate by GateMedia) that
provides media files (images and videos) that are updated regularly.
Most updates on the CMS will result in additional/new files being transferred to the Units.

An example for on-the-fly textual information is the RSS ticker text.
Updates to the ticker will only result in a (Medi) message being sent to the Units.

2. Export of DynamicContent by icenter.media
--------------------------------------------
When icenter.media exports UpdateParts that should show dynamic content,
it needs to fill the UpdatePart.DynamicContent structure with an object of type
Gorba.Center.Common.ServiceModel.Dynamic.DynamicContentInfo. This object needs
to define all parts that require dynamic content. This is done this way so that
dynamic content is delivered to the Unit only while that given content is also
defined in the respective UpdatePart.

3. Generation of dynamic content by the Background System
---------------------------------------------------------
The DynamicDataManager (with the help of UpdateGroupDynamicDataControllers) uses
the UpdatePart.DynamicContent structure to generate a dynamic content provider
for each part defined in the DynamicContent. This is a 1:1 mapping from
DynamicContentPartBase to DynamicContentProviderBase (each part has one provider).

The DynamicContentProviderBase implementation (called content providers hereafter)
will then generate all content from the configuration given to it at creation.
The content provider may or may not require some files on the Unit. If it
requires files, it needs to generate the resources for those files and then
provide the list of files in CreateFolderStructure(). This list will mostly be
empty for on-the-fly information (see above) and will always contain files
for file-based dynamic content.

A content provider for an RSS feed ticker might very well return an emtpy folder 
structure and simply send (Medi) messages directly to Protran for the feed
content changes.

A content provider for CMS data will most probably use the .wm2 file structure
to generate its content. An example can be seen in the ScreenGateContentProvider.

It is strongly recommended that content providers always generate the same
output for the same input (don't add "random" data). This guarantees that Units
are not updated when there was no update to dynamic content (but for example
the Background System was restarted).

4. Use of dynamic content by imotion software
---------------------------------------------
The imotion software has to handle dynamic content differently depending on its
kind. The .wm2 file structure together with the WebmediaSection already provide
a good toolset for cycles with dynamic layout data (media files and other info).
Protran is the application to handle messages from a content provider for
on-the-fly text information.