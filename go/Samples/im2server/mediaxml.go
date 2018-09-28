package main

import (
	"encoding/xml"
	"fmt"
	"os"
)

type InfomediaXML struct {
	PhysicalScreens struct {
		PhysicalScreen struct {
			Name   string `xml:"Name"`
			Type   string `xml:"Type"`
			Width  string `xml:"Width"`
			Height string `xml:"Height"`
		} `xml:"PhysicalScreen"`
	} `xml:"PhysicalScreens"`
	VirtualDisplays struct {
		VirtualDisplay struct {
			Name         string `xml:"Name"`
			CyclePackage string `xml:"CyclePackage"`
			Width        string `xml:"Width"`
			Height       string `xml:"Height"`
		} `xml:"VirtualDisplay"`
	} `xml:"VirtualDisplays"`
	MasterPresentation struct {
		MasterCycles struct {
			MasterCycle struct {
				MasterSection struct {
					Duration string `xml:"Duration"`
					Layout   string `xml:"Layout"`
				} `xml:"MasterSection"`
				Name string `xml:"Name"`
			} `xml:"MasterCycle"`
		} `xml:"MasterCycles"`
		MasterEventCycles string `xml:"MasterEventCycles"`
		MasterLayouts     struct {
			MasterLayout struct {
				PhysicalScreen struct {
					VirtualDisplay struct {
						X      string `xml:"X"`
						Y      string `xml:"Y"`
						ZIndex string `xml:"ZIndex"`
						Ref    string `xml:"Ref"`
					} `xml:"VirtualDisplay"`
					Ref string `xml:"Ref"`
				} `xml:"PhysicalScreen"`
				Name string `xml:"_Name"`
			} `xml:"MasterLayout"`
		} `xml:"MasterLayouts"`
	} `xml:"MasterPresentation"`
	Evaluations struct {
		Evaluation []struct {
			StringCompare struct {
				Generic struct {
					Lang   string `xml:"Lang"`
					Table  string `xml:"Table"`
					Column string `xml:"Column"`
					Row    string `xml:"Row"`
				} `xml:"Generic"`
				Value string `xml:"Value"`
			} `xml:"StringCompare,omitempty"`
			Name           string `xml:"Name"`
			IntegerCompare struct {
				Generic struct {
					Lang   string `xml:"Lang"`
					Table  string `xml:"Table"`
					Column string `xml:"Column"`
					Row    string `xml:"Row"`
				} `xml:"Generic"`
				Begin string `xml:"Begin"`
				End   string `xml:"End"`
			} `xml:"IntegerCompare,omitempty"`
			CodeConversion struct {
				FileName string `xml:"FileName"`
				UseImage string `xml:"UseImage"`
			} `xml:"CodeConversion,omitempty"`
		} `xml:"Evaluation"`
	} `xml:"Evaluations"`
	Cycles struct {
		StandardCycles struct {
			StandardCycle struct {
				StandardSection []struct {
					Duration string `xml:"Duration"`
					Layout   string `xml:"Layout"`
				} `xml:"StandardSection"`
				Name string `xml:"Name"`
			} `xml:"StandardCycle"`
		} `xml:"StandardCycles"`
		EventCycles string `xml:"EventCycles"`
	} `xml:"Cycles"`
	CyclePackages struct {
		CyclePackage struct {
			StandardCycles struct {
				StandardCycle struct {
					Ref string `xml:"Ref"`
				} `xml:"StandardCycle"`
			} `xml:"StandardCycles"`
			EventCycles string `xml:"EventCycles"`
			Name        string `xml:"Name"`
		} `xml:"CyclePackage"`
	} `xml:"CyclePackages"`
	Pools   string `xml:"Pools"`
	Layouts struct {
		Layout []struct {
			Resolution struct {
				Text []struct {
					Font struct {
						Face         string `xml:"Face"`
						Height       string `xml:"Height"`
						Weight       string `xml:"Weight"`
						Italic       string `xml:"Italic"`
						Color        string `xml:"Color"`
						OutlineColor string `xml:"OutlineColor"`
					} `xml:"Font"`
					LastPosition struct {
						Location struct {
							X string `xml:"X"`
							Y string `xml:"Y"`
						} `xml:"Location"`
						Size struct {
							Width  string `xml:"Width"`
							Height string `xml:"Height"`
						} `xml:"Size"`
						X      string `xml:"X"`
						Y      string `xml:"Y"`
						Width  string `xml:"Width"`
						Height string `xml:"Height"`
					} `xml:"LastPosition"`
					X           string `xml:"X"`
					Y           string `xml:"Y"`
					Width       string `xml:"Width"`
					Height      string `xml:"Height"`
					ZIndex      string `xml:"ZIndex"`
					Align       string `xml:"Align"`
					Overflow    string `xml:"Overflow"`
					ScrollSpeed string `xml:"ScrollSpeed"`
					Value       string `xml:"Value"`
				} `xml:"Text"`
				Video struct {
					X             string `xml:"X"`
					Y             string `xml:"Y"`
					Width         string `xml:"Width"`
					Height        string `xml:"Height"`
					ZIndex        string `xml:"ZIndex"`
					VideoURI      string `xml:"VideoUri"`
					FallbackImage string `xml:"FallbackImage"`
				} `xml:"Video"`
				Image struct {
					X        string `xml:"X"`
					Y        string `xml:"Y"`
					Width    string `xml:"Width"`
					Height   string `xml:"Height"`
					ZIndex   string `xml:"ZIndex"`
					Filename string `xml:"Filename"`
				} `xml:"Image"`
				Width  string `xml:"Width"`
				Height string `xml:"Height"`
			} `xml:"Resolution"`
			Name string `xml:"Name"`
		} `xml:"Layout"`
	} `xml:"Layouts"`
	Fonts    string `xml:"Fonts"`
	XmlnsXsi string `xml:"xmlns:xsi"`
	XmlnsXsd string `xml:"xmlns:xsd"`
	Version  string `xml:"Version"`
	Created  string `xml:"Created"`
}

func LoadXML(filename string) (InfomediaXML, error) {
	fmt.Println("loading XML media file ...")
	var im2xml InfomediaXML
	im2File, err := os.Open(filename)
	defer im2File.Close()
	if err != nil {
		return im2xml, err
	}
	xmlParser := xml.NewDecoder(im2File)
	err = xmlParser.Decode(&im2xml)
	if err != nil {
		fmt.Println("Error loading media xml file. ")
		return im2xml, err
	}
	fmt.Println("loading media xml file ...success ")
	return im2xml, err

}
