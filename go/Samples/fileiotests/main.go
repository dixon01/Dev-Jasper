package main

import (
	"fmt"
	"io/ioutil"
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

	// err := filepath.Walk(presentationsRoot, visit(&presentations))
	// if err != nil {
	// 	panic(err)
	// }
	// for _, file := range presentations {

	// 	fmt.Println(file)
	// }
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
}
