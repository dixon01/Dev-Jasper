// Access and Modify Structs
package main

import "fmt"

type Simple struct {
   Key string
}


func main() {
var Simples []Simple
	
	for i := 0; i < 10000; i++ {
		 randomString  := Simple {Key: fmt.Sprintf("String = %d", i) }
		Simples = append(Simples, randomString )
	}
	fmt.Println("Made 10000 random strings like", Simples )
}


## Get all presentations in a folder recuresively

package main

import (
	"fmt"
	"log"
	"os"
	"path/filepath"
)

func visit(files *[]string) filepath.WalkFunc {
	return func(path string, info os.FileInfo, err error) error {
		if err != nil {
			log.Fatal(err)
		}
		if info.IsDir() {
			*files = append(*files, path)
		}

		return nil
	}
}


func main() {
	var presentations []string

	presentationsRoot := "../"
	err := filepath.Walk(presentationsRoot, visit(&presentations))
	if err != nil {
		panic(err)
	}
	for _, file := range presentations {

		fmt.Println(file)
	}
}

## Get all presentations in a folder not recuresively
var presentations []string

	presentationsRoot := "../"
	files, err := ioutil.ReadDir(presentationsRoot)
	if err != nil {
		log.Fatal(err)
	}

	for _, f := range files {
		if f.IsDir() {
			//*files = append(*files, path)
			presentations = append(presentations, f.Name())
		}
	}
	fmt.Println(presentations)