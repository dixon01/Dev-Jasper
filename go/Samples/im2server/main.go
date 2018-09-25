package main

import (
	"encoding/json"
	"fmt"
	"os"

	"github.com/sanity-io/litter"
)

type Infomedia struct {
	PhysicalScreens struct {
		PhysicalScreen struct {
			Name   string `json:"Name"`
			Type   string `json:"Type"`
			Width  string `json:"Width"`
			Height string `json:"Height"`
		} `json:"PhysicalScreen"`
	} `json:"PhysicalScreens"`
	VirtualDisplays struct {
		VirtualDisplay struct {
			Name         string `json:"Name"`
			CyclePackage string `json:"CyclePackage"`
			Width        string `json:"Width"`
			Height       string `json:"Height"`
		} `json:"VirtualDisplay"`
	} `json:"VirtualDisplays"`
	MasterPresentation struct {
		MasterCycles struct {
			MasterCycle struct {
				MasterSection struct {
					Duration string `json:"Duration"`
					Layout   string `json:"Layout"`
				} `json:"MasterSection"`
				Name string `json:"Name"`
			} `json:"MasterCycle"`
		} `json:"MasterCycles"`
		MasterEventCycles string `json:"MasterEventCycles"`
		MasterLayouts     struct {
			MasterLayout struct {
				PhysicalScreen struct {
					VirtualDisplay struct {
						X      string `json:"X"`
						Y      string `json:"Y"`
						ZIndex string `json:"ZIndex"`
						Ref    string `json:"Ref"`
					} `json:"VirtualDisplay"`
					Ref string `json:"Ref"`
				} `json:"PhysicalScreen"`
				Name string `json:"_Name"`
			} `json:"MasterLayout"`
		} `json:"MasterLayouts"`
	} `json:"MasterPresentation"`
	Evaluations struct {
		Evaluation []struct {
			StringCompare struct {
				Generic struct {
					Lang   string `json:"Lang"`
					Table  string `json:"Table"`
					Column string `json:"Column"`
					Row    string `json:"Row"`
				} `json:"Generic"`
				Value string `json:"Value"`
			} `json:"StringCompare,omitempty"`
			Name           string `json:"Name"`
			IntegerCompare struct {
				Generic struct {
					Lang   string `json:"Lang"`
					Table  string `json:"Table"`
					Column string `json:"Column"`
					Row    string `json:"Row"`
				} `json:"Generic"`
				Begin string `json:"Begin"`
				End   string `json:"End"`
			} `json:"IntegerCompare,omitempty"`
			CodeConversion struct {
				FileName string `json:"FileName"`
				UseImage string `json:"UseImage"`
			} `json:"CodeConversion,omitempty"`
		} `json:"Evaluation"`
	} `json:"Evaluations"`
	Cycles struct {
		StandardCycles struct {
			StandardCycle struct {
				StandardSection []struct {
					Duration string `json:"Duration"`
					Layout   string `json:"Layout"`
				} `json:"StandardSection"`
				Name string `json:"Name"`
			} `json:"StandardCycle"`
		} `json:"StandardCycles"`
		EventCycles string `json:"EventCycles"`
	} `json:"Cycles"`
	CyclePackages struct {
		CyclePackage struct {
			StandardCycles struct {
				StandardCycle struct {
					Ref string `json:"Ref"`
				} `json:"StandardCycle"`
			} `json:"StandardCycles"`
			EventCycles string `json:"EventCycles"`
			Name        string `json:"Name"`
		} `json:"CyclePackage"`
	} `json:"CyclePackages"`
	Pools   string `json:"Pools"`
	Layouts struct {
		Layout []struct {
			Resolution struct {
				Text []struct {
					Font struct {
						Face         string `json:"Face"`
						Height       string `json:"Height"`
						Weight       string `json:"Weight"`
						Italic       string `json:"Italic"`
						Color        string `json:"Color"`
						OutlineColor string `json:"OutlineColor"`
					} `json:"Font"`
					LastPosition struct {
						Location struct {
							X string `json:"X"`
							Y string `json:"Y"`
						} `json:"Location"`
						Size struct {
							Width  string `json:"Width"`
							Height string `json:"Height"`
						} `json:"Size"`
						X      string `json:"X"`
						Y      string `json:"Y"`
						Width  string `json:"Width"`
						Height string `json:"Height"`
					} `json:"LastPosition"`
					X           string `json:"X"`
					Y           string `json:"Y"`
					Width       string `json:"Width"`
					Height      string `json:"Height"`
					ZIndex      string `json:"ZIndex"`
					Align       string `json:"Align"`
					Overflow    string `json:"Overflow"`
					ScrollSpeed string `json:"ScrollSpeed"`
					Value       string `json:"Value"`
				} `json:"Text"`
				Video struct {
					X             string `json:"X"`
					Y             string `json:"Y"`
					Width         string `json:"Width"`
					Height        string `json:"Height"`
					ZIndex        string `json:"ZIndex"`
					VideoURI      string `json:"VideoUri"`
					FallbackImage string `json:"FallbackImage"`
				} `json:"Video"`
				Image struct {
					X        string `json:"X"`
					Y        string `json:"Y"`
					Width    string `json:"Width"`
					Height   string `json:"Height"`
					ZIndex   string `json:"ZIndex"`
					Filename string `json:"Filename"`
				} `json:"Image"`
				Width  string `json:"Width"`
				Height string `json:"Height"`
			} `json:"Resolution"`
			Name string `json:"Name"`
		} `json:"Layout"`
	} `json:"Layouts"`
	Fonts    string `json:"Fonts"`
	XmlnsXsi string `json:"xmlns:xsi"`
	XmlnsXsd string `json:"xmlns:xsd"`
	Version  string `json:"Version"`
	Created  string `json:"Created"`
}

func LoadIm2(filename string) (Infomedia, error) {
	var im2 Infomedia
	im2File, err := os.Open(filename)
	defer im2File.Close()
	if err != nil {
		return im2, err
	}
	jsonParser := json.NewDecoder(im2File)
	err = jsonParser.Decode(&im2)
	return im2, err

}

func main() {
	fmt.Println("Strating application")
	im2, _ := LoadIm2(".\\simple\\main.1.json")
	//fmt.Printf("Screens %s, version %s \n", im2.PhysicalScreens.PhysicalScreen.Height, im2.Version)
	litter.Dump(im2)
}
