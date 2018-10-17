package main

import (
	"encoding/xml"
	"fmt"
)

type PhysicalScreenList struct {
	XMLName         xml.Name         `xml:"PhysicalScreenList"`
	PhysicalScreens []PhysicalScreen `xml:"PhysicalScreen"`
}

type PhysicalScreen struct {
	Name   string `xml:"Name,attr"`
	Type   string `xml:"Type,attr"`
	Width  string `xml:"Width,attr"`
	Height string `xml:"Height,attr"`
}

// type AppList struct {
// 	XMLName xml.Name `xml:"applist"`
// 	Apps    []App    `xml:"app"`
// }

func LoadIm2XML() {
	var data = `
		<?xml version="1.0" encoding="UTF-8"?>
    <PhysicalScreenList>
		<PhysicalScreen Name="1366x768" Type="TFT" Width="1366" Height="768" />
		<PhysicalScreen Name="1366x768" Type="TFT" Width="1366" Height="768" />
    </PhysicalScreenList>
		`

	// data := `
	// <?xml version="1.0" encoding="UTF-8"?>
	// <applist>
	//     <app app_id="1234" app_name="abc"/>
	//     <app app_id="5678" app_name="def"/>
	// </applist>
	// `

	var media PhysicalScreenList
	err := xml.Unmarshal([]byte(data), &media)
	if err != nil {
		fmt.Printf("error: %v", err)
		return
	}
	fmt.Printf("PhysicalScreen Name:: %q\n", media.PhysicalScreens[0].Name)
	fmt.Printf("PhysicalScreen Type:: %q\n", media.PhysicalScreens[0].Type)
	fmt.Printf("PhysicalScreen Width:: %q\n", media.PhysicalScreens[0].Width)
	fmt.Printf("PhysicalScreen Height:: %q\n", media.PhysicalScreens[0].Height)
}
