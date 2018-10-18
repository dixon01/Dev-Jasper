// To parse and unparse this xml data, add this code to your project and do:
//
//    infomedia, err := UnmarshalInfomedia(bytes)
//    bytes, err = infomedia.Marshal()

package main

import (
	"encoding/xml"
	"fmt"
	"io/ioutil"
)

func main() {
	fmt.Println("Strating application")
	im2xml, _ := LoadIm2FromXML(".\\main.im2")
	fmt.Printf("Im2 File Loaded: %q\n", im2xml)

	fmt.Printf("Created : %q\n", im2xml.Created)
	fmt.Printf("Version %q\n", im2xml.Version)
	fmt.Printf("Name %q\n", im2xml.PhysicalScreens.PhysicalScreen.Name)
	fmt.Printf("Cycle Package Name %q\n", im2xml.CyclePackages.CyclePackage.Name)
	fmt.Printf("Height %q\n", im2xml.PhysicalScreens.PhysicalScreen.Height)
	fmt.Printf("Width %q\n", im2xml.PhysicalScreens.PhysicalScreen.Width)

	fmt.Printf("Evaluations 0 %q\n", im2xml.Evaluations[0].Name)
	fmt.Printf("Evaluations 1 %q\n", im2xml.Evaluations[1].Name)
	fmt.Printf("Evaluations 2 %q\n", im2xml.Evaluations[2].Name)
}

func LoadIm2FromXML(filename string) (Infomedia, error) {
	fmt.Println("loading media file ...")
	var im2 Infomedia
	im2File, err := ioutil.ReadFile(filename)

	if err != nil {
		return im2, err
	}

	//im2string := string(im2File) // convert content to a 'string'
	//fmt.Printf("Media content: %q\n", im2string)
	//var media Infomedia
	im2, err1 := UnmarshalInfomedia([]byte(im2File))
	if err1 != nil {
		fmt.Printf("error: %v", err1)
		return im2, err1
	}

	return im2, err1
}

func UnmarshalInfomedia(data []byte) (Infomedia, error) {
	var r Infomedia
	err := xml.Unmarshal(data, &r)
	return r, err
}

func (r *Infomedia) Marshal() ([]byte, error) {
	return xml.Marshal(r)
}

type Infomedia struct {
	XMLName            xml.Name           `xml:"Infomedia"`
	Version            string             `xml:"Version,attr"`
	Created            string             `xml:"Created,attr"`
	PhysicalScreens    PhysicalScreens    `xml:"PhysicalScreens"`
	VirtualDisplays    VirtualDisplays    `xml:"VirtualDisplays"`
	MasterPresentation MasterPresentation `xml:"MasterPresentation"`
	Evaluations        []*Evaluation      `xml:"Evaluations"`
	Cycles             Cycles             `xml:"Cycles"`
	CyclePackages      CyclePackages      `xml:"CyclePackages"`
	Pools              []interface{}      `xml:"Pools"`
	Layouts            []Layout           `xml:"Layouts"`
	Fonts              []interface{}      `xml:"Fonts"`
}

type CyclePackages struct {
	CyclePackage CyclePackage `xml:"CyclePackage"`
}

type CyclePackage struct {
	Name           string                     `xml:"Name,attr"`
	StandardCycles CyclePackageStandardCycles `xml:"StandardCycles"`
	EventCycles    []interface{}              `xml:"EventCycles"`
}

type CyclePackageStandardCycles struct {
	StandardCycle PurpleStandardCycle `xml:"StandardCycle"`
}

type PurpleStandardCycle struct {
	Ref string `xml:"@Ref"`
}

type Cycles struct {
	StandardCycles CyclesStandardCycles `xml:"StandardCycles"`
	EventCycles    []interface{}        `xml:"EventCycles"`
}

type CyclesStandardCycles struct {
	StandardCycle FluffyStandardCycle `xml:"StandardCycle"`
}

type FluffyStandardCycle struct {
	Name            string    `xml:"@Name"`
	StandardSection []Section `xml:"StandardSection"`
}

type Section struct {
	Duration string `xml:"@Duration"`
	Layout   string `xml:"@Layout"`
}

type Evaluation struct {
	XMLName        xml.Name        `xml:"Evaluation"`
	StringCompare  *StringCompare  `xml:"StringCompare,omitempty"`
	IntegerCompare *IntegerCompare `xml:"IntegerCompare,omitempty"`
	CodeConversion *CodeConversion `xml:"CodeConversion,omitempty"`
}

type CodeConversion struct {
	FileName string `xml:"FileName,attr"`
	UseImage string `xml:"UseImage,attr"`
}

type IntegerCompare struct {
	Begin   string  `xml:"Begin,attr"`
	End     string  `xml:"End,attr"`
	Generic Generic `xml:"Generic,attr"`
}

type Generic struct {
	Lang   string `xml:"Lang,attr"`
	Table  string `xml:"Table,attr"`
	Column string `xml:"Column,attr"`
	Row    string `xml:"Row,attr"`
}

type StringCompare struct {
	Value   string  `xml:"Value,attr"`
	Generic Generic `xml:"Generic"`
}

type Layout struct {
	Name       string     `xml:"Name,attr"`
	Resolution Resolution `xml:"Resolution"`
}

type Resolution struct {
	Width  string `xml:"Width,attr"`
	Height string `xml:"Height,attr"`
	Text   []Text `xml:"Text"`
	Video  Video  `xml:"Video"`
	Image  *Image `xml:"Image,omitempty"`
}

type Image struct {
	X        string `xml:"X,attr"`
	Y        string `xml:"Y,attr"`
	Width    string `xml:"Width,attr"`
	Height   string `xml:"Height,attr"`
	ZIndex   string `xml:"ZIndex,attr"`
	Filename string `xml:"Filename,attr"`
}

type Text struct {
	X            string       `xml:"X,attr"`
	Y            string       `xml:"Y,attr"`
	Width        string       `xml:"Width,attr"`
	Height       string       `xml:"Height,attr"`
	ZIndex       string       `xml:"ZIndex,attr"`
	Align        string       `xml:"Align,attr"`
	Overflow     string       `xml:"Overflow,attr"`
	ScrollSpeed  string       `xml:"ScrollSpeed,attr"`
	Value        string       `xml:"Value,attr"`
	Font         Font         `xml:"Font"`
	LastPosition LastPosition `xml:"LastPosition"`
}

type Font struct {
	Face         string `xml:"Face,attr"`
	Height       string `xml:"Height,attr"`
	Weight       string `xml:"Weight,attr"`
	Italic       string `xml:"Italic,attr"`
	Color        string `xml:"Color,attr"`
	OutlineColor string `xml:"OutlineColor,attr"`
}

type LastPosition struct {
	Location Location `xml:"Location"`
	Size     Size     `xml:"Size"`
	X        string   `xml:"X"`
	Y        string   `xml:"Y"`
	Width    string   `xml:"Width"`
	Height   string   `xml:"Height"`
}

type Location struct {
	X string `xml:"X"`
	Y string `xml:"Y"`
}

type Size struct {
	Width  string `xml:"Width"`
	Height string `xml:"Height"`
}

type Video struct {
	X             string `xml:"X,attr"`
	Y             string `xml:"Y,attr"`
	Width         string `xml:"Width,attr"`
	Height        string `xml:"Height,attr"`
	ZIndex        string `xml:"ZIndex,attr"`
	VideoURI      string `xml:"VideoUri,attr"`
	FallbackImage string `xml:"FallbackImage,attr"`
}

type MasterPresentation struct {
	MasterCycles      MasterCycles  `xml:"MasterCycles"`
	MasterEventCycles []interface{} `xml:"MasterEventCycles"`
	MasterLayouts     MasterLayouts `xml:"MasterLayouts"`
}

type MasterCycles struct {
	MasterCycle MasterCycle `xml:"MasterCycle"`
}

type MasterCycle struct {
	Name          string  `xml:"Name,attr"`
	MasterSection Section `xml:"MasterSection"`
}

type MasterLayouts struct {
	MasterLayout MasterLayout `xml:"MasterLayout"`
}

type MasterLayout struct {
	Name           string         `xml:"Name,attr"`
	PhysicalScreen PhysicalScreen `xml:"PhysicalScreen"`
}

type PhysicalScreen struct {
	Ref            string         `xml:"Ref,attr"`
	VirtualDisplay VirtualDisplay `xml:"VirtualDisplay"`
}

type VirtualDisplay struct {
	X      string `xml:"X,attr"`
	Y      string `xml:"Y,attr"`
	ZIndex string `xml:"ZIndex,attr"`
	Ref    string `xml:"Ref,attr"`
}

type PhysicalScreens struct {
	PhysicalScreen VirtualDisplayClass `xml:"PhysicalScreen"`
}

type VirtualDisplayClass struct {
	Name         string  `xml:"Name,attr"`
	Width        string  `xml:"Width,attr"`
	Height       string  `xml:"Height,attr"`
	CyclePackage *string `xml:"CyclePackage,attr,omitempty"`
}

type VirtualDisplays struct {
	VirtualDisplay VirtualDisplayClass `xml:"VirtualDisplay"`
}
