user@DESKTOP-O8C3BFH MINGW64 /c/dev/j/granite-jasper/go/Samples/TestIm2.go (master)
$ docker build . -t natraj:im2server
Sending build context to Docker daemon  20.41MB
Step 1/2 : FROM golang:onbuild
# Executing 3 build triggers
 ---> Running in 5ae3764b4d34
+ exec go get -v -d
github.com/gorilla/mux (download)
Removing intermediate container 5ae3764b4d34
 ---> Running in f2c82f5d8df4
+ exec go install -v
github.com/gorilla/mux
app
Removing intermediate container f2c82f5d8df4
 ---> 5fb580609ee9
Step 2/2 : EXPOSE 8081
 ---> Running in 08ce1ad7db0b
Removing intermediate container 08ce1ad7db0b
 ---> 886e7d0ff8b9
Successfully built 886e7d0ff8b9
Successfully tagged natraj:im2server
SECURITY WARNING: You are building a Docker image from Windows against a non-Windows Docker host. All files and directories added to build context will have '-rwxr-xr-x' permissions. It is recommended to double check and reset permissions for sensitive files and directories.

user@DESKTOP-O8C3BFH MINGW64 /c/dev/j/granite-jasper/go/Samples/TestIm2.go (master)
$ docker run --publish 8011:8011 -t natraj:im2server
+ exec app
Strating application
2018/10/28 18:51:06 Starting Go Server at http://localhost:8011
loading media file ...
loading media file ...
loading media file ...
loading media file ...
[]
calling GetProjectsDataEndPoint ...