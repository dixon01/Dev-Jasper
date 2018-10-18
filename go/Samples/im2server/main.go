package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
	"os"

	"github.com/gorilla/mux"
)

type InfomediaJson struct {
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

func LoadIm2(filename string) (InfomediaJson, error) {
	fmt.Println("loading media file ...")
	var im2 InfomediaJson
	im2File, err := os.Open(filename)
	defer im2File.Close()
	if err != nil {
		return im2, err
	}
	jsonParser := json.NewDecoder(im2File)
	err = jsonParser.Decode(&im2)
	fmt.Println("loading media file ...success ")
	return im2, err

}

var projects []InfomediaJson

func GetProjectEndPoint(w http.ResponseWriter, req *http.Request) {
	//params := mux.Vars(req)
	fmt.Println("calling GetProjectEndPoint ...")
	for i, item := range projects {
		if i == 0 {
			json.NewEncoder(w).Encode(item)
			return
		}
	}
	json.NewEncoder(w).Encode(&InfomediaJson{})
}

func GetProjectsDataEndPoint(w http.ResponseWriter, req *http.Request) {
	fmt.Println("calling GetProjectsDataEndPoint ...")
	json.NewEncoder(w).Encode(projects)
}

func CreateProjectEndPoint(w http.ResponseWriter, req *http.Request) {
	fmt.Println("calling CreateProjectEndPoint ...")
	// params := mux.Vars(req)
	var project InfomediaJson
	// _ = json.NewDecoder(req.Body).Decode(&project)
	// //	project.ProjectID = params["projectid"]
	// projects = append(projects, project)
	json.NewEncoder(w).Encode(project)
}
func DeleteProjectEndPoint(w http.ResponseWriter, req *http.Request) {
	fmt.Println("calling DeleteProjectEndPoint ...")
	// params := mux.Vars(req)
	// for index, item := range projects {
	// 	if index == 0 {
	// 		projects = append(projects[:index], projects[index+1:]...)
	// 		break
	// 	}
	// }
	json.NewEncoder(w).Encode(projects)
}
func TestEndPoint(w http.ResponseWriter, req *http.Request) {
	fmt.Fprintln(w, "testing server - server is running !!!")
}
func main() {
	fmt.Println("Strating application")
	log.Print("Starting Go Server at http://localhost:8011")
	router := mux.NewRouter()
	im2, _ := LoadIm2(".\\simple\\main.json")
	projects = append(projects, im2)
	PrintHostProperties()

	im2xml, _ := LoadIm2FromXMLTest(".\\simple\\test.im2")
	fmt.Printf("Im2 File Loaded: %q\n", im2xml)
	//	projectsXML = append(projectsXML, im2xml)
	//  LoadXMLAttributesAndValues()
	//  im2xml, _ := LoadIm2FromXML(".\\simple\\main.im2")
	//	fmt.Printf("Im2 File Loaded: %q\n", im2xml)

	router.HandleFunc("/test", TestEndPoint).Methods("GET")
	router.HandleFunc("/projects", GetProjectsDataEndPoint).Methods("GET")
	router.HandleFunc("/project/{projectid}", GetProjectEndPoint).Methods("GET")
	router.HandleFunc("/project/{projectid}", CreateProjectEndPoint).Methods("POST")
	router.HandleFunc("/project/{projectid}", DeleteProjectEndPoint).Methods("DELETE")
	log.Fatal(http.ListenAndServe(":8011", router))
}
