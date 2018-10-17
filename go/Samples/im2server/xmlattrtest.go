package main

import (
	"encoding/xml"
	"fmt"
)

type App struct {
	App_id   string `xml:"app_id,attr"`   // notice the capitalized field name here
	App_name string `xml:"app_name,attr"` // notice the capitalized field name here and the `xml:"app_name,attr"`
}

type AppList struct {
	XMLName xml.Name `xml:"applist"`
	Apps    []App    `xml:"app"`
}

func LoadXMLAttributesAndValues() {
	data := `
    <?xml version="1.0" encoding="UTF-8"?>
    <applist>
        <app app_id="1234" app_name="abc"/>
        <app app_id="5678" app_name="def"/>
    </applist>
    `

	var portfolio AppList
	err := xml.Unmarshal([]byte(data), &portfolio)
	if err != nil {
		fmt.Printf("error: %v", err)
		return
	}
	fmt.Printf("application ID:: %q\n", portfolio.Apps[0].App_id)     // the corresponding changes here for App
	fmt.Printf("application name:: %q\n", portfolio.Apps[0].App_name) // the corresponding changes here for App
}
