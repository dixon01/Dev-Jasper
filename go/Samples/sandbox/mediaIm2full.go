// To parse and unparse this JSON data, add this code to your project and do:
//
//    infomedia, err := UnmarshalInfomedia(bytes)
//    bytes, err = infomedia.Marshal()

package main

import (
	"bytes"
	"encoding/json"
	"encoding/xml"
	"errors"
	"fmt"
	"io/ioutil"
	"log"
)

func UnmarshalInfomedia(data []byte) (Infomedia, error) {
	var r Infomedia
	err := xml.Unmarshal(data, &r)
	return r, err
}

func (r *Infomedia) Marshal() ([]byte, error) {
	return xml.Marshal(r)
}

func LoadProjects() {
	var presentations []string

	presentationsRoot := "..\\TestIm2.go\\Presentations"
	files, err := ioutil.ReadDir(presentationsRoot)
	if err != nil {
		log.Fatal(err)
	}

	for _, f := range files {
		if f.IsDir() {
			//*files = append(*files, path)
			presentations = append(presentations, fmt.Sprintf("%s\\%s\\main.im2", presentationsRoot, f.Name()))
		}
	}
	fmt.Println(presentations)

	for _, pres := range presentations {
		im2Temp, _ := LoadIm2FromXML(pres)
		projects = append(projects, im2Temp)
	}
}

var projects []Infomedia

func main() {
	fmt.Println("Strating application")
	log.Print("Starting Go Server at http://localhost:8011")

	LoadProjects()
}

func PrintIm2Data(im2xmlData Infomedia) {
	fmt.Printf("Created : %q\n", im2xmlData.Created)
	fmt.Printf("Version %q\n", im2xmlData.Version)
	fmt.Printf("Name %q\n", im2xmlData.PhysicalScreens)

	//	fmt.Printf("Height %q\n", im2xmlData.PhysicalScreens.PhysicalScreen.Height)
	//	fmt.Printf("Width %q\n", im2xmlData.PhysicalScreens.PhysicalScreen.Width)

	//	fmt.Printf("Cycle Package Name %q\n", im2xmlData.CyclePackages.CyclePackage.Name)
	//	fmt.Printf("Cycle Package Standard Cycle Ref %q\n",
	//		im2xmlData.CyclePackages.CyclePackage.StandardCycles.StandardCycle.Ref)

	fmt.Printf("Master Presentation %q\n", im2xmlData.MasterPresentation.MasterLayouts.MasterLayout.Name)
	//	fmt.Printf("Master Presentation Ref(%q) => %q x %q\n",
	//		im2xmlData.MasterPresentation.MasterLayouts.MasterLayout.PhysicalScreen.VirtualDisplay.Ref,
	//		im2xmlData.MasterPresentation.MasterLayouts.MasterLayout.PhysicalScreen.VirtualDisplay.X,
	//		im2xmlData.MasterPresentation.MasterLayouts.MasterLayout.PhysicalScreen.VirtualDisplay.Y)

	fmt.Printf("Master Cycle Name %q, Duration %q , Layout %q\n",
		im2xmlData.MasterPresentation.MasterCycles.MasterCycle.Name,
		im2xmlData.MasterPresentation.MasterCycles.MasterCycle.MasterSection.Duration,
		im2xmlData.MasterPresentation.MasterCycles.MasterCycle.MasterSection.Layout)

	//	fmt.Printf("Layout Cycle Name = %q, Resolution %q x %q\n",
	//		im2xmlData.Layouts[0].Name, im2xmlData.Layouts[0].Resolution.Height,
	//		im2xmlData.Layouts[0].Resolution.Height)

	// fmt.Printf("Layout Cycle Resolution Name = %q, Text.Font %q, ScrollSpeed %q, Align %q, LastPosition %q, Overflow %q \n",
	// 	im2xmlData.Layouts[0].Name,
	// 	im2xmlData.Layouts[0].Resolution.Text[0],
	// 	im2xmlData.Layouts[0].Resolution.Text[0].ScrollSpeed,
	// 	im2xmlData.Layouts[0].Resolution.Text[0].Align,
	// 	im2xmlData.Layouts[0].Resolution.Text[0].LastPosition,
	// 	im2xmlData.Layouts[0].Resolution.Text[0].Overflow)

	// fmt.Printf("Layout Cycle Resolution Name = %q, Image %q, Video %q \n",
	// 	im2xmlData.Layouts[0].Name,
	// 	im2xmlData.Layouts[0].Resolution.Image.Filename,
	// 	im2xmlData.Layouts[0].Resolution.Video.VideoURI)
	// fmt.Printf("VirtualDisplay.Name %q\n", im2xmlData.VirtualDisplays.VirtualDisplay.Name)
	// fmt.Printf("VirtualDisplay %q x %q\n", im2xmlData.VirtualDisplays.VirtualDisplay.Height,
	// 	im2xmlData.VirtualDisplays.VirtualDisplay.Width)

	fmt.Printf("Evaluations 0 Name %q\n", im2xmlData.Evaluations[0].Name)
	fmt.Printf("Evaluations StringCompare.Value %q\n", im2xmlData.Evaluations[0].StringCompare.Value)
	fmt.Printf("Evaluations Column %q\n", im2xmlData.Evaluations[0].StringCompare.Generic.Column)
	fmt.Printf("Evaluations Lang %q\n", im2xmlData.Evaluations[0].StringCompare.Generic.Lang)
	fmt.Printf("Evaluations Table %q\n", im2xmlData.Evaluations[0].StringCompare.Generic.Table)

	fmt.Printf("Evaluations CodeConversion.FileName %q\n", im2xmlData.Evaluations[6].CodeConversion.FileName)
	fmt.Printf("Evaluations CodeConversion.UseImage %q\n", im2xmlData.Evaluations[6].CodeConversion.UseImage)
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

type Infomedia struct {
	Version            string              `xml:"Version,attr"`
	Created            string              `xml:"Created,attr"`
	PhysicalScreens    []map[string]string `xml:"PhysicalScreens"`
	VirtualDisplays    []map[string]string `xml:"VirtualDisplays"`
	MasterPresentation MasterPresentation  `xml:"MasterPresentation"`
	Evaluations        []Evaluation        `xml:"Evaluations>Evaluation"`
	Cycles             Cycles              `xml:"Cycles"`
	CyclePackages      []CyclePackage      `xml:"CyclePackages"`
	Pools              []Pool              `xml:"Pools>Pool"`
	Layouts            []Layout            `xml:"Layouts>Layout"`
	Fonts              []FontElement       `xml:"Fonts>FontElement"`
}
type Pool struct {
	Name          string `xml:"Name,attr"`
	BaseDirectory string `xml:"BaseDirectory,attr"`
}

type Filename struct {
	CSVMapping CSVMapping `xml:"CsvMapping"`
}

type CSVMapping struct {
	FileName     string  `xml:"FileName,attr"`
	OutputFormat string  `xml:"OutputFormat,attr"`
	DefaultValue Trigger `xml:"DefaultValue,attr"`
	Match        Match   `xml:"Match,attr"`
}
type Match struct {
	Column  string  `json:"Column,attr"`
	Generic Generic `json:"Generic"`
}
type FontElement struct {
	Path string `xml:"Path,attr"`
}

type CyclePackage struct {
	Name           string               `xml:"Name,attr"`
	StandardCycles *StandardCyclesUnion `xml:"StandardCycles"`
	EventCycles    *EventCyclesUnion    `xml:"EventCycles"`
}

type EventCycle struct {
	Ref string `xml:"Ref,attr"`
}

type EventCyclesClass struct {
	EventCycle EventCycle `xml:"EventCycle"`
}

type StandardCyclesClass struct {
	StandardCycle EventCycle `xml:"StandardCycle"`
}

type Cycles struct {
	StandardCycles []StandardCycle    `xml:"StandardCycles"`
	EventCycles    []CyclesEventCycle `xml:"EventCycles"`
}

type CyclesEventCycle struct {
	Name            string             `xml:"Name,attr"`
	Enabled         *EventCycleEnabled `xml:"Enabled,omitempty"`
	Trigger         Trigger            `xml:"Trigger"`
	StandardSection Section            `xml:"StandardSection"`
}

type EventCycleEnabled struct {
	Evaluation *EventCycle `xml:"Evaluation,omitempty"`
	Not        *Trigger    `xml:"Not,omitempty"`
}

type Trigger struct {
	Generic Generic `xml:"Generic"`
}

type Generic struct {
	Lang   string `xml:"Lang,attr"`
	Table  string `xml:"Table,attr"`
	Column string `xml:"Column,attr"`
	Row    string `xml:"Row,attr"`
}

type Section struct {
	Duration string `xml:"Duration,attr"`
	Layout   string `xml:"Layout,attr"`
}

type StandardCycle struct {
	Name            string                `xml:"Name,attr"`
	StandardSection *StandardSection      `xml:"StandardSection"`
	Enabled         *StandardCycleEnabled `xml:"Enabled,omitempty"`
}

type StandardCycleEnabled struct {
	Or EnabledOr `xml:"Or"`
}

type EnabledOr struct {
	Not        Not          `xml:"Not"`
	Evaluation []EventCycle `xml:"Evaluation"`
}

type Not struct {
	Evaluation EventCycle `xml:"Evaluation"`
}

type Evaluation struct {
	Name           string                   `xml:"Name,attr"`
	And            *AndUnion                `xml:"And"`
	Or             []OrElement              `xml:"Or"`
	IntegerCompare *IntegerCompare          `xml:"IntegerCompare,omitempty"`
	StringCompare  *EvaluationStringCompare `xml:"StringCompare,omitempty"`
	CodeConversion *CodeConversion          `xml:"CodeConversion,omitempty"`
	Not            *EvaluationNot           `xml:"Not,omitempty"`
	Format         *Format                  `xml:"Format,omitempty"`
	Generic        *Generic                 `xml:"Generic,omitempty"`
}

type AndElement struct {
	Value         *string           `xml:"Value,omitempty,attr"`
	Generic       *Generic          `xml:"Generic,omitempty"`
	StringCompare *AndStringCompare `xml:"StringCompare,omitempty"`
}

type AndStringCompare struct {
	Value   string  `xml:"Value,attr"`
	Generic Generic `xml:"Generic"`
}

type PurpleAnd struct {
	Not           AndNot           `xml:"Not"`
	StringCompare AndStringCompare `xml:"StringCompare"`
}

type AndNot struct {
	StringCompare AndStringCompare `xml:"StringCompare"`
}

type CodeConversion struct {
	FileName string `xml:"FileName,attr"`
	UseImage string `xml:"UseImage,attr"`
}

type Format struct {
	Format string `xml:"Format,attr"`
}

type IntegerCompare struct {
	Begin   string  `xml:"Begin,attr"`
	End     string  `xml:"End,attr"`
	Generic Generic `xml:"Generic"`
}

type EvaluationNot struct {
	StringCompare *AndStringCompare `xml:"StringCompare,omitempty"`
	Generic       *Generic          `xml:"Generic,omitempty"`
}

type OrElement struct {
	Value         *string           `xml:"Value,omitempty,attr"`
	Generic       *Generic          `xml:"Generic,omitempty"`
	StringCompare *AndStringCompare `xml:"StringCompare,omitempty"`
	Lang          *string           `xml:"Lang,omitempty,attr"`
	Table         *string           `xml:"Table,omitempty,attr"`
	Column        *string           `xml:"Column,omitempty,attr"`
	Row           *string           `xml:"Row,omitempty,attr"`
	IgnoreCase    *string           `xml:"IgnoreCase,omitempty,attr"`
}

type EvaluationStringCompare struct {
	Value      string  `xml:"Value,attr"`
	Generic    Generic `xml:"Generic"`
	IgnoreCase *string `xml:"IgnoreCase,omitempty,attr"`
}

type Fonts struct {
	Font FontsFont `xml:"Font"`
}

type FontsFont struct {
	Path string `xml:"Path,attr"`
}

type Layout struct {
	Name       string           `xml:"Name,attr"`
	Resolution *ResolutionUnion `xml:"Resolution"`
}

type ResolutionElement struct {
	Width  string         `xml:"Width,attr"`
	Height string         `xml:"Height,attr"`
	Text   []TextElement  `xml:"Text"`
	Image  []ImageElement `xml:"Image"`
}

type ImageElement struct {
	X        string   `xml:"X,attr"`
	Y        string   `xml:"Y,attr"`
	Width    string   `xml:"Width,attr"`
	Height   string   `xml:"Height,attr"`
	ZIndex   string   `xml:"ZIndex,attr"`
	Filename string   `xml:"Filename,attr"`
	Visible  *Trigger `xml:"Visible,omitempty"`
	Scaling  *Scaling `xml:"Scaling,omitempty,attr"`
}

type TextElement struct {
	X            string       `xml:"X,attr"`
	Y            string       `xml:"Y,attr"`
	Width        string       `xml:"Width,attr"`
	Height       string       `xml:"Height,attr"`
	ZIndex       string       `xml:"ZIndex,attr"`
	Align        Align        `xml:"Align,attr"`
	VAlign       *VAlign      `xml:"VAlign,omitempty,attr"`
	Overflow     Scaling      `xml:"Overflow,attr"`
	ScrollSpeed  string       `xml:"ScrollSpeed,attr"`
	Value        *string      `xml:"Value,omitempty,attr"`
	Font         TextFont     `xml:"Font"`
	LastPosition LastPosition `xml:"LastPosition"`
	Visible      *Not         `xml:"Visible,omitempty"`
	TextValue    *Trigger     `xml:"Value,omitempty"`
}
type Visible struct {
	Date       *Date          `xml:"Date,omitempty"`
	Evaluation *StandardCycle `xml:"Evaluation,omitempty"`
	Not        *VisibleNot    `xml:"Not,omitempty"`
	And        *VisibleAnd    `xml:"And,omitempty"`
}
type VisibleNot struct {
	Evaluation StandardCycle `xml:"Evaluation"`
}
type VisibleAnd struct {
	Not        VisibleNot    `xml:"Not"`
	Evaluation StandardCycle `xml:"Evaluation"`
}

type Date struct {
	Begin string `xml:"Begin,attr"`
	End   string `xml:"End,attr"`
}
type TextFont struct {
	Face         Face   `xml:"Face,attr"`
	Height       string `xml:"Height,attr"`
	Weight       string `xml:"Weight,attr"`
	Italic       string `xml:"Italic,attr"`
	Color        Color  `xml:"Color,attr"`
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

type PurpleResolution struct {
	Width       string       `xml:"Width,attr"`
	Height      string       `xml:"Height,attr"`
	Text        *TextUnion   `xml:"Text"`
	Image       *ImageUnion  `xml:"Image"`
	AudioOutput *AudioOutput `xml:"AudioOutput,omitempty"`
}

type AudioOutput struct {
	Volume       string          `xml:"Volume,attr"`
	Priority     string          `xml:"Priority,attr"`
	AudioFile    *AudioFileUnion `xml:"AudioFile"`
	Enabled      *Not            `xml:"Enabled,omitempty"`
	TextToSpeech *TextToSpeech   `xml:"TextToSpeech,omitempty"`
}

type AudioFileElement struct {
	Filename string           `xml:"Filename,attr"`
	Enabled  AudioFileEnabled `xml:"Enabled"`
}

type AudioFileEnabled struct {
	StringCompare EvaluationStringCompare `xml:"StringCompare"`
}

type PurpleAudioFile struct {
	Filename string `xml:"Filename,attr"`
}

type TextToSpeech struct {
	Voice string `xml:"Voice,attr"`
	Value Not    `xml:"Value"`
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
	Name           string           `xml:"Name,attr"`
	PhysicalScreen []PhysicalScreen `xml:"PhysicalScreen"`
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

type Scaling string

const (
	Clip  Scaling = "Clip"
	Scale Scaling = "Scale"
	Wrap  Scaling = "Wrap"
)

type Align string

const (
	Center Align = "Center"
	Left   Align = "Left"
)

type Color string

const (
	Ffff0000  Color = "#FFFF0000"
	Ffffff    Color = "#FFFFFF"
	The000000 Color = "#000000"
	The0Ecd99 Color = "#0ecd99"
	The545358 Color = "#545358"
)

type Face string

const (
	Arial      Face = "Arial"
	CourierNew Face = "Courier New"
)

type VAlign string

const (
	Middle VAlign = "Middle"
)

type EventCyclesUnion struct {
	EventCycleArray  []EventCycle
	EventCyclesClass *EventCyclesClass
}

func (x *EventCyclesUnion) UnmarshalJSON(data []byte) error {
	x.EventCycleArray = nil
	x.EventCyclesClass = nil
	var c EventCyclesClass
	object, err := unmarshalUnion(data, nil, nil, nil, nil, true, &x.EventCycleArray, true, &c, false, nil, false, nil, false)
	if err != nil {
		return err
	}
	if object {
		x.EventCyclesClass = &c
	}
	return nil
}

func (x *EventCyclesUnion) MarshalJSON() ([]byte, error) {
	return marshalUnion(nil, nil, nil, nil, x.EventCycleArray != nil, x.EventCycleArray, x.EventCyclesClass != nil, x.EventCyclesClass, false, nil, false, nil, false)
}

type StandardCyclesUnion struct {
	EventCycleArray     []EventCycle
	StandardCyclesClass *StandardCyclesClass
}

func (x *StandardCyclesUnion) UnmarshalJSON(data []byte) error {
	x.EventCycleArray = nil
	x.StandardCyclesClass = nil
	var c StandardCyclesClass
	object, err := unmarshalUnion(data, nil, nil, nil, nil, true, &x.EventCycleArray, true, &c, false, nil, false, nil, false)
	if err != nil {
		return err
	}
	if object {
		x.StandardCyclesClass = &c
	}
	return nil
}

func (x *StandardCyclesUnion) MarshalJSON() ([]byte, error) {
	return marshalUnion(nil, nil, nil, nil, x.EventCycleArray != nil, x.EventCycleArray, x.StandardCyclesClass != nil, x.StandardCyclesClass, false, nil, false, nil, false)
}

type StandardSection struct {
	Section      *Section
	SectionArray []Section
}

func (x *StandardSection) UnmarshalJSON(data []byte) error {
	x.SectionArray = nil
	x.Section = nil
	var c Section
	object, err := unmarshalUnion(data, nil, nil, nil, nil, true, &x.SectionArray, true, &c, false, nil, false, nil, false)
	if err != nil {
		return err
	}
	if object {
		x.Section = &c
	}
	return nil
}

func (x *StandardSection) MarshalJSON() ([]byte, error) {
	return marshalUnion(nil, nil, nil, nil, x.SectionArray != nil, x.SectionArray, x.Section != nil, x.Section, false, nil, false, nil, false)
}

type AndUnion struct {
	AndElementArray []AndElement
	PurpleAnd       *PurpleAnd
}

func (x *AndUnion) UnmarshalJSON(data []byte) error {
	x.AndElementArray = nil
	x.PurpleAnd = nil
	var c PurpleAnd
	object, err := unmarshalUnion(data, nil, nil, nil, nil, true, &x.AndElementArray, true, &c, false, nil, false, nil, false)
	if err != nil {
		return err
	}
	if object {
		x.PurpleAnd = &c
	}
	return nil
}

func (x *AndUnion) MarshalJSON() ([]byte, error) {
	return marshalUnion(nil, nil, nil, nil, x.AndElementArray != nil, x.AndElementArray, x.PurpleAnd != nil, x.PurpleAnd, false, nil, false, nil, false)
}

type ResolutionUnion struct {
	PurpleResolution       *PurpleResolution
	ResolutionElementArray []ResolutionElement
}

func (x *ResolutionUnion) UnmarshalJSON(data []byte) error {
	x.ResolutionElementArray = nil
	x.PurpleResolution = nil
	var c PurpleResolution
	object, err := unmarshalUnion(data, nil, nil, nil, nil, true, &x.ResolutionElementArray, true, &c, false, nil, false, nil, false)
	if err != nil {
		return err
	}
	if object {
		x.PurpleResolution = &c
	}
	return nil
}

func (x *ResolutionUnion) MarshalJSON() ([]byte, error) {
	return marshalUnion(nil, nil, nil, nil, x.ResolutionElementArray != nil, x.ResolutionElementArray, x.PurpleResolution != nil, x.PurpleResolution, false, nil, false, nil, false)
}

type AudioFileUnion struct {
	AudioFileElementArray []AudioFileElement
	PurpleAudioFile       *PurpleAudioFile
}

func (x *AudioFileUnion) UnmarshalJSON(data []byte) error {
	x.AudioFileElementArray = nil
	x.PurpleAudioFile = nil
	var c PurpleAudioFile
	object, err := unmarshalUnion(data, nil, nil, nil, nil, true, &x.AudioFileElementArray, true, &c, false, nil, false, nil, false)
	if err != nil {
		return err
	}
	if object {
		x.PurpleAudioFile = &c
	}
	return nil
}

func (x *AudioFileUnion) MarshalJSON() ([]byte, error) {
	return marshalUnion(nil, nil, nil, nil, x.AudioFileElementArray != nil, x.AudioFileElementArray, x.PurpleAudioFile != nil, x.PurpleAudioFile, false, nil, false, nil, false)
}

type ImageUnion struct {
	ImageElement      *ImageElement
	ImageElementArray []ImageElement
}

func (x *ImageUnion) UnmarshalJSON(data []byte) error {
	x.ImageElementArray = nil
	x.ImageElement = nil
	var c ImageElement
	object, err := unmarshalUnion(data, nil, nil, nil, nil, true, &x.ImageElementArray, true, &c, false, nil, false, nil, false)
	if err != nil {
		return err
	}
	if object {
		x.ImageElement = &c
	}
	return nil
}

func (x *ImageUnion) MarshalJSON() ([]byte, error) {
	return marshalUnion(nil, nil, nil, nil, x.ImageElementArray != nil, x.ImageElementArray, x.ImageElement != nil, x.ImageElement, false, nil, false, nil, false)
}

type TextUnion struct {
	TextElement      *TextElement
	TextElementArray []TextElement
}

func (x *TextUnion) UnmarshalJSON(data []byte) error {
	x.TextElementArray = nil
	x.TextElement = nil
	var c TextElement
	object, err := unmarshalUnion(data, nil, nil, nil, nil, true, &x.TextElementArray, true, &c, false, nil, false, nil, false)
	if err != nil {
		return err
	}
	if object {
		x.TextElement = &c
	}
	return nil
}

func (x *TextUnion) MarshalJSON() ([]byte, error) {
	return marshalUnion(nil, nil, nil, nil, x.TextElementArray != nil, x.TextElementArray, x.TextElement != nil, x.TextElement, false, nil, false, nil, false)
}

func unmarshalUnion(data []byte, pi **int64, pf **float64, pb **bool, ps **string, haveArray bool, pa interface{}, haveObject bool, pc interface{}, haveMap bool, pm interface{}, haveEnum bool, pe interface{}, nullable bool) (bool, error) {
	if pi != nil {
		*pi = nil
	}
	if pf != nil {
		*pf = nil
	}
	if pb != nil {
		*pb = nil
	}
	if ps != nil {
		*ps = nil
	}

	dec := json.NewDecoder(bytes.NewReader(data))
	dec.UseNumber()
	tok, err := dec.Token()
	if err != nil {
		return false, err
	}

	switch v := tok.(type) {
	case json.Number:
		if pi != nil {
			i, err := v.Int64()
			if err == nil {
				*pi = &i
				return false, nil
			}
		}
		if pf != nil {
			f, err := v.Float64()
			if err == nil {
				*pf = &f
				return false, nil
			}
			return false, errors.New("Unparsable number")
		}
		return false, errors.New("Union does not contain number")
	case float64:
		return false, errors.New("Decoder should not return float64")
	case bool:
		if pb != nil {
			*pb = &v
			return false, nil
		}
		return false, errors.New("Union does not contain bool")
	case string:
		if haveEnum {
			return false, xml.Unmarshal(data, pe)
		}
		if ps != nil {
			*ps = &v
			return false, nil
		}
		return false, errors.New("Union does not contain string")
	case nil:
		if nullable {
			return false, nil
		}
		return false, errors.New("Union does not contain null")
	case json.Delim:
		if v == '{' {
			if haveObject {
				return true, xml.Unmarshal(data, pc)
			}
			if haveMap {
				return false, xml.Unmarshal(data, pm)
			}
			return false, errors.New("Union does not contain object")
		}
		if v == '[' {
			if haveArray {
				return false, xml.Unmarshal(data, pa)
			}
			return false, errors.New("Union does not contain array")
		}
		return false, errors.New("Cannot handle delimiter")
	}
	return false, errors.New("Cannot unmarshal union")

}

func marshalUnion(pi *int64, pf *float64, pb *bool, ps *string, haveArray bool, pa interface{}, haveObject bool, pc interface{}, haveMap bool, pm interface{}, haveEnum bool, pe interface{}, nullable bool) ([]byte, error) {
	if pi != nil {
		return xml.Marshal(*pi)
	}
	if pf != nil {
		return xml.Marshal(*pf)
	}
	if pb != nil {
		return xml.Marshal(*pb)
	}
	if ps != nil {
		return xml.Marshal(*ps)
	}
	if haveArray {
		return xml.Marshal(pa)
	}
	if haveObject {
		return xml.Marshal(pc)
	}
	if haveMap {
		return xml.Marshal(pm)
	}
	if haveEnum {
		return xml.Marshal(pe)
	}
	if nullable {
		return xml.Marshal(nil)
	}
	return nil, errors.New("Union must not be null")
}
