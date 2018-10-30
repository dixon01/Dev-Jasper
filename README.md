# granite-jasper
Jasper Cross Functional Team Repository

For more information about Jasper visit the Wiki Page

https://github.com/apollovideo/granite-jasper/wiki


## Git Commands Update
1. Clone jasper repo $ git clone https://github.com/apollovideo/granite-jasper.git

2. To Untrack Files $ git rm --cached FILENAME

3. More commands here >> https://orga.cat/posts/most-useful-git-commands

### UI Submodule
The UI is being developed in [apollovideo/granite-jasper-ui](https://github.com/apollovideo/granite-jasper-ui) and is included in granite-jasper as a Git submodule.

To initiate the submodule on your local, run:

`git submodule init`

From then on, to update/load the files within the /ui folder:

`git submodule update`

Pull requests to update this repo's version of the UI will automatically provide helpful links to the changes that were made in apollovideo/granite-jasper-ui.

Documentation on how to test/demo/review the UI itself can be found in the [apollovideo/granite-jasper-ui README](https://github.com/apollovideo/granite-jasper-ui/blob/master/README.md). Any of the instructions you find there need to be run from within the /ui directory.
