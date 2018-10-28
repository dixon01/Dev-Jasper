// To parse and unparse this xml data, add this code to your project and do:
//
//    infomedia, err := UnmarshalInfomedia(bytes)
//    bytes, err = infomedia.Marshal()

// https://itnext.io/building-restful-web-api-service-using-golang-chi-mysql-d85f427dee54
//https://semaphoreci.com/community/tutorials/building-and-testing-a-rest-api-in-go-with-gorilla-mux-and-postgresql
//https://github.com/jtbonhomme/go-rest-api-boilerplate

// Docker Commands
// Build 
// $ docker build . -t natraj:im2server
// Run 
// $ docker run --publish 8011:8011 -t natraj:im2server
package main

import (
	"encoding/json"
	"encoding/xml"
	"fmt"
	"io"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"path/filepath"
	"strconv"

	"github.com/gorilla/mux"
)

type Project struct {
	XMLName   xml.Name  `xml:"ProjectManager",omitonempty`
	Name      string    `xml:Name`
	ShortName string    `xml:ShortName,omitonempty`
	Media     Infomedia `xml:Infomedia`
}

var projects []Project

func GetPresentationsDataEndPoint(w http.ResponseWriter, req *http.Request) {
	//Allow CORS here By * or specific origin
	w.Header().Set("Access-Control-Allow-Origin", "*")

	w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
	//params := mux.Vars(req)
	fmt.Println("calling GetPresentationsDataEndPoint ...")
	presentations := []string{}
	for _, item := range projects {
		presentations = append(presentations, item.ShortName)
	}
	json.NewEncoder(w).Encode(presentations)
}
func GetProjectEndPoint(w http.ResponseWriter, req *http.Request) {
	//Allow CORS here By * or specific origin
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
	params := mux.Vars(req)
	id := params["projectidname"]
	fmt.Println("calling GetProjectEndPoint ...")
	for i, item := range projects {
		_, err := strconv.Atoi(id)
		if err == nil {
			fmt.Println("calling GetProjectEndPoint using project id")
			if strconv.Itoa(i) == id {
				json.NewEncoder(w).Encode(item)
				return
			}
		} else {
			if item.ShortName == id {
				fmt.Println("calling GetProjectEndPoint using project name %s , %s", item.Name, item.ShortName)
				json.NewEncoder(w).Encode(item)
				return
			}
		}
	}
	json.NewEncoder(w).Encode(&Infomedia{})
}

func GetProjectsDataEndPoint(w http.ResponseWriter, req *http.Request) {
	//Allow CORS here By * or specific origin
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
	fmt.Println("calling GetProjectsDataEndPoint ...")
	json.NewEncoder(w).Encode(projects)
}

func CreateProjectEndPoint(w http.ResponseWriter, req *http.Request) {

	fmt.Println("calling CreateProjectEndPoint ...")
	params := mux.Vars(req)
	projectname := params["projectidname"]
	dirnameofproject := "." + string(filepath.Separator) + "Presentations" + string(filepath.Separator) + projectname
	fmt.Println("calling CreateProjectEndPoint ...", dirnameofproject)
	newProject := new(Infomedia)
	os.Mkdir(dirnameofproject, 0777)
	//  display on screen what we are about to save
	xmlString, err := xml.MarshalIndent(newProject, "", "    ")
	if err != nil {
			fmt.Println(err)
	}
 	fmt.Printf("%s \n", string(xmlString))
	// everything ok now, write to file.
	filename := dirnameofproject + string(filepath.Separator) + "main.im2"
	file, _ := os.Create(filename)

	xmlWriter := io.Writer(file)

	enc := xml.NewEncoder(xmlWriter)
	enc.Indent("  ", "    ")
	if err := enc.Encode(newProject); err != nil {
			fmt.Printf("error: %v\n", err)
	}

	json.NewEncoder(w).Encode(newProject)
}
func DeleteProjectEndPoint(w http.ResponseWriter, req *http.Request) {
	//Allow CORS here By * or specific origin
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
	params := mux.Vars(req)
	projectname := params["projectidname"]
	dirnameofproject := "." + string(filepath.Separator) + "Presentations" + string(filepath.Separator) + projectname
	fmt.Println("calling DeleteProjectEndPoint ", dirnameofproject)
	os.RemoveAll(dirnameofproject)
	json.NewEncoder(w).Encode(projects)
}
func TestEndPoint(w http.ResponseWriter, req *http.Request) {
	fmt.Fprintln(w, "testing server - server is running !!!")
}

func LoadProjects() {
	var presentations []string

	presentationsRoot := ".\\Presentations"
	files, err := ioutil.ReadDir(presentationsRoot)
	if err != nil {
		presentationsRoot := "./Presentations"
		files, err = ioutil.ReadDir(presentationsRoot)
		if err != nil {
			log.Fatal("Cannot read presentations dir %v " , err)
		}
	}

	for _, f := range files {
		if f.IsDir() {
			//*files = append(*files, path)
			//	presentations = append(presentations, fmt.Sprintf("%s\\%s\\main.im2", presentationsRoot, f.Name()))
			pres := fmt.Sprintf("%s\\%s\\main.im2", presentationsRoot, f.Name())
			im2Temp, _ := LoadIm2FromXML(pres)
			project := Project{Name: pres, ShortName: f.Name(), Media: im2Temp}
			projects = append(projects, project)
		}
	}
	fmt.Println(presentations)

	// for _, pres := range presentations {
	// 	im2Temp, _ := LoadIm2FromXML(pres)
	// 	project := Project{Name: pres, Media: im2Temp}
	// 	projects = append(projects, project)
	// }
}

func main() {

	fmt.Println("Strating application")
	log.Print("Starting Go Server at http://localhost:8011")
	router := mux.NewRouter()
	LoadProjects()
	router.HandleFunc("/test", TestEndPoint).Methods("GET")
	router.HandleFunc("/projects", GetProjectsDataEndPoint).Methods("GET", "OPTIONS")
	router.HandleFunc("/presentations", GetPresentationsDataEndPoint).Methods("GET", "OPTIONS")
	router.HandleFunc("/project/{projectidname}", GetProjectEndPoint).Methods("GET", "OPTIONS")
	router.HandleFunc("/project/{projectidname}", CreateProjectEndPoint).Methods("POST", "OPTIONS")
	router.HandleFunc("/project/{projectidname}", DeleteProjectEndPoint).Methods("DELETE", "OPTIONS")
	log.Fatal(http.ListenAndServe(":8011", router))

}

func PrintIm2Data(im2xmlData Infomedia) {
	fmt.Printf("Created : %q\n", im2xmlData.Created)
	fmt.Printf("Version %q\n", im2xmlData.Version)
	fmt.Printf("Name %q\n", im2xmlData.PhysicalScreens.PhysicalScreen.Name)

	fmt.Printf("Height %q\n", im2xmlData.PhysicalScreens.PhysicalScreen.Height)
	fmt.Printf("Width %q\n", im2xmlData.PhysicalScreens.PhysicalScreen.Width)

	fmt.Printf("Cycle Package Name %q\n", im2xmlData.CyclePackages.CyclePackage.Name)
	fmt.Printf("Cycle Package Standard Cycle Ref %q\n",
		im2xmlData.CyclePackages.CyclePackage.StandardCycles.StandardCycle.Ref)

	fmt.Printf("Master Presentation %q\n", im2xmlData.MasterPresentation.MasterLayouts.MasterLayout.Name)
	fmt.Printf("Master Presentation Ref(%q) => %q x %q\n",
		im2xmlData.MasterPresentation.MasterLayouts.MasterLayout.PhysicalScreen.VirtualDisplay.Ref,
		im2xmlData.MasterPresentation.MasterLayouts.MasterLayout.PhysicalScreen.VirtualDisplay.X,
		im2xmlData.MasterPresentation.MasterLayouts.MasterLayout.PhysicalScreen.VirtualDisplay.Y)

	fmt.Printf("Master Cycle Name %q, Duration %q , Layout %q\n",
		im2xmlData.MasterPresentation.MasterCycles.MasterCycle.Name,
		im2xmlData.MasterPresentation.MasterCycles.MasterCycle.MasterSection.Duration,
		im2xmlData.MasterPresentation.MasterCycles.MasterCycle.MasterSection.Layout)

	fmt.Printf("Layout Cycle Name = %q, Resolution %q x %q\n",
		im2xmlData.Layouts[0].Name, im2xmlData.Layouts[0].Resolution.Height,
		im2xmlData.Layouts[0].Resolution.Height)

	fmt.Printf("Layout Cycle Resolution Name = %q, Text.Font %q, ScrollSpeed %q, Align %q, LastPosition %q, Overflow %q \n",
		im2xmlData.Layouts[0].Name,
		im2xmlData.Layouts[0].Resolution.Text[0].Font,
		im2xmlData.Layouts[0].Resolution.Text[0].ScrollSpeed,
		im2xmlData.Layouts[0].Resolution.Text[0].Align,
		im2xmlData.Layouts[0].Resolution.Text[0].LastPosition,
		im2xmlData.Layouts[0].Resolution.Text[0].Overflow)

	fmt.Printf("Layout Cycle Resolution Name = %q, Image %q, Video %q \n",
		im2xmlData.Layouts[0].Name,
		im2xmlData.Layouts[0].Resolution.Image.Filename,
		im2xmlData.Layouts[0].Resolution.Video.VideoURI)
	fmt.Printf("VirtualDisplay.Name %q\n", im2xmlData.VirtualDisplays.VirtualDisplay.Name)
	fmt.Printf("VirtualDisplay %q x %q\n", im2xmlData.VirtualDisplays.VirtualDisplay.Height,
		im2xmlData.VirtualDisplays.VirtualDisplay.Width)

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
	}else {
		fmt.Println("loading media file success...")
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
	Evaluations        []Evaluation       `xml:"Evaluations>Evaluation"`
	Cycles             Cycles             `xml:"Cycles"`
	CyclePackages      CyclePackages      `xml:"CyclePackages"`
	Pools              []Pool             `xml:"Pools>Pool"`
	Layouts            []Layout           `xml:"Layouts>Layout"`
	Fonts              []FontElement      `xml:"Fonts>FontElement"`
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
	Ref string `xml:"Ref,attr"`
}

type EventCycles struct {
	EventCycle EventCycle `xml:"EventCycle"`
}
type Cycles struct {
	StandardCycles StandardCycles `xml:"StandardCycles"`
	EventCycles    []EventCycle   `xml:"EventCycles"`
}

type StandardCycles struct {
	StandardCycle EventCycle `xml:"StandardCycle"`
}

type StandardCycle struct {
	Name            string    `xml:"Name,attr"`
	StandardSection []Section `xml:"StandardSection"`
}

type EventCycle struct {
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
	Generic Generic `json:"Generic"`
}

type Section struct {
	Duration string `xml:"Duration,attr"`
	Layout   string `xml:"Layout,attr"`
}

type Evaluation struct {
	Name           string          `xml:"Name,attr"`
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
