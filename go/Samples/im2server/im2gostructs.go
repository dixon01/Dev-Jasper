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

func LoadIm2FromXML(filename string) (Infomedia, error) {
	fmt.Println("loading media file ...")
	var im2 Infomedia
	im2File, err := ioutil.ReadFile(filename)

	if err != nil {
		return im2, err
	}

	//im2string := string(im2File) // convert content to a 'string'
	//fmt.Printf("Media content: %q\n", im2string)
	var media Infomedia
	im2, err1 := UnmarshalInfomedia([]byte(im2File))
	if err1 != nil {
		fmt.Printf("error: %v", err1)
		return im2, err1
	}
	fmt.Printf("Im2 data: %q\n", media)
	//fmt.Printf("PhysicalScreen Type:: %q\n", media.PhysicalScreens[0].Type)
	//fmt.Printf("PhysicalScreen Width:: %q\n", media.PhysicalScreens[0].Width)
	//fmt.Printf("PhysicalScreen Height:: %q\n", media.PhysicalScreens[0].Height)

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
	Version            string             `xml:"@Version"`
	Created            string             `xml:"@Created"`
	PhysicalScreens    PhysicalScreens    `xml:"PhysicalScreens"`
	VirtualDisplays    VirtualDisplays    `xml:"VirtualDisplays"`
	MasterPresentation MasterPresentation `xml:"MasterPresentation"`
	Evaluations        []Evaluation       `xml:"Evaluations"`
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
	Name           string                     `xml:"@Name"`
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
	Name           string          `xml:"@Name"`
	StringCompare  *StringCompare  `xml:"StringCompare,omitempty"`
	IntegerCompare *IntegerCompare `xml:"IntegerCompare,omitempty"`
	CodeConversion *CodeConversion `xml:"CodeConversion,omitempty"`
}

type CodeConversion struct {
	FileName string `xml:"@FileName"`
	UseImage string `xml:"@UseImage"`
}

type IntegerCompare struct {
	Begin   string  `xml:"@Begin"`
	End     string  `xml:"@End"`
	Generic Generic `xml:"Generic"`
}

type Generic struct {
	Lang   string `xml:"@Lang"`
	Table  string `xml:"@Table"`
	Column string `xml:"@Column"`
	Row    string `xml:"@Row"`
}

type StringCompare struct {
	Value   string  `xml:"@Value"`
	Generic Generic `xml:"Generic"`
}

type Layout struct {
	Name       string     `xml:"@Name"`
	Resolution Resolution `xml:"Resolution"`
}

type Resolution struct {
	Width  string `xml:"@Width"`
	Height string `xml:"@Height"`
	Text   []Text `xml:"Text"`
	Video  Video  `xml:"Video"`
	Image  *Image `xml:"Image,omitempty"`
}

type Image struct {
	X        string `xml:"@X"`
	Y        string `xml:"@Y"`
	Width    string `xml:"@Width"`
	Height   string `xml:"@Height"`
	ZIndex   string `xml:"@ZIndex"`
	Filename string `xml:"@Filename"`
}

type Text struct {
	X            string       `xml:"@X"`
	Y            string       `xml:"@Y"`
	Width        string       `xml:"@Width"`
	Height       string       `xml:"@Height"`
	ZIndex       string       `xml:"@ZIndex"`
	Align        string       `xml:"@Align"`
	Overflow     string       `xml:"@Overflow"`
	ScrollSpeed  string       `xml:"@ScrollSpeed"`
	Value        string       `xml:"@Value"`
	Font         Font         `xml:"Font"`
	LastPosition LastPosition `xml:"LastPosition"`
}

type Font struct {
	Face         string `xml:"@Face"`
	Height       string `xml:"@Height"`
	Weight       string `xml:"@Weight"`
	Italic       string `xml:"@Italic"`
	Color        string `xml:"@Color"`
	OutlineColor string `xml:"@OutlineColor"`
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
	X             string `xml:"@X"`
	Y             string `xml:"@Y"`
	Width         string `xml:"@Width"`
	Height        string `xml:"@Height"`
	ZIndex        string `xml:"@ZIndex"`
	VideoURI      string `xml:"@VideoUri"`
	FallbackImage string `xml:"@FallbackImage"`
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
	Name          string  `xml:"@Name"`
	MasterSection Section `xml:"MasterSection"`
}

type MasterLayouts struct {
	MasterLayout MasterLayout `xml:"MasterLayout"`
}

type MasterLayout struct {
	Name           string         `xml:"@Name"`
	PhysicalScreen PhysicalScreen `xml:"PhysicalScreen"`
}

// type PhysicalScreen struct {
// 	Ref            string         `xml:"@Ref"`
// 	VirtualDisplay VirtualDisplay `xml:"VirtualDisplay"`
// }

// type VirtualDisplay struct {
// 	X      string `xml:"@X"`
// 	Y      string `xml:"@Y"`
// 	ZIndex string `xml:"@ZIndex"`
// 	Ref    string `xml:"@Ref"`
// }

type PhysicalScreens struct {
	PhysicalScreen VirtualDisplayClass `xml:"PhysicalScreen"`
}

type VirtualDisplayClass struct {
	Name         string  `xml:"@Name"`
	Width        string  `xml:"@Width"`
	Height       string  `xml:"@Height"`
	CyclePackage *string `xml:"@CyclePackage,omitempty"`
}

type VirtualDisplays struct {
	VirtualDisplay VirtualDisplayClass `xml:"VirtualDisplay"`
}
