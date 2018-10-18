package main

import (
	"encoding/xml"
	"fmt"
	"io/ioutil"
)

type InfomediaXML struct {
	XMLName         xml.Name         `xml:"Infomedia"`
	PhysicalScreens []PhysicalScreen `xml:"PhysicalScreen"`
	//	VirtualDisplays []VirtualDisplay `xml:"VirtualDisplay"`
}

type PhysicalScreen struct {
	Name   string `xml:"Name,attr"`
	Type   string `xml:"Type,attr"`
	Width  string `xml:"Width,attr"`
	Height string `xml:"Height,attr"`
}

type VirtualDisplay struct {
	Name         string `xml:"Name,attr"`
	CyclePackage string `xml:"CyclePackage,attr"`
	Width        string `xml:"Width,attr"`
	Height       string `xml:"Height,attr"`
}

func LoadIm2FromXMLTest(filename string) (InfomediaXML, error) {
	fmt.Println("loading media file ...")
	var im2 InfomediaXML
	im2File, err := ioutil.ReadFile(filename)
	if err != nil {
		return im2, err
	}

	im2string := string(im2File) // convert content to a 'string'
	fmt.Printf("Media content: %q\n", im2string)
	media := new(InfomediaXML)
	err1 := xml.Unmarshal([]byte(im2File), media)
	if err1 != nil {
		fmt.Printf("error: %v", err1)
		return im2, err1
	}
	fmt.Printf("print objects ...  %q\n", media)
	fmt.Printf("PhysicalScreen Name:: %q\n", media.PhysicalScreens[0].Name)
	fmt.Printf("PhysicalScreen Type:: %q\n", media.PhysicalScreens[0].Type)
	fmt.Printf("PhysicalScreen Width:: %q\n", media.PhysicalScreens[0].Width)
	fmt.Printf("PhysicalScreen Height:: %q\n", media.PhysicalScreens[0].Height)

	return im2, err1
}

// func LoadIm2XML() {
//  var data = `
//      <?xml version="1.0" encoding="UTF-8"?>
//     <Infomedia>
//      <PhysicalScreen Name="1366x768" Type="TFT" Width="1366" Height="768" />
//      <PhysicalScreen Name="1440x900" Type="TFT" Width="1440" Height="900" />
//     </Infomedia>
//      `

//  var media InfomediaXML
//  err := xml.Unmarshal([]byte(data), &media)
//  if err != nil {
//      fmt.Printf("error: %v", err)
//      return
//  }
//  fmt.Printf("PhysicalScreen Name:: %q\n", media.PhysicalScreens[0].Name)
//  fmt.Printf("PhysicalScreen Type:: %q\n", media.PhysicalScreens[0].Type)
//  fmt.Printf("PhysicalScreen Width:: %q\n", media.PhysicalScreens[0].Width)
//  fmt.Printf("PhysicalScreen Height:: %q\n", media.PhysicalScreens[0].Height)
// }
