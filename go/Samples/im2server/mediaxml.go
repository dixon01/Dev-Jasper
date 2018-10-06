package main

import (
	"encoding/xml"
)

var data = `<HostProperties>
<tag name="HOST_END">Thu Feb 20 12:38:24 2014</tag>
<tag name="patch-summary-total-cves">4</tag>
<tag name="cpe-1">cpe:/a:openbsd:openssh:5.6 -&gt; OpenBSD OpenSSH 5.6</tag>
<tag name="cpe-0">cpe:/o:vmware:esx_server</tag>
<tag name="system-type">hypervisor</tag>
<tag name="operating-system">VMware ESXi</tag>
<tag name="mac-address">00:00:00:00:00:00</tag>
<tag name="traceroute-hop-0">172.28.28.29</tag>
<tag name="host-ip">172.28.28.29</tag>
<tag name="host-fqdn">foobar.com</tag>
<tag name="HOST_START">Thu Feb 20 12:30:14 2014</tag>
</HostProperties>`

type HostProperties struct {
	XMLName xml.Name `xml:HostProperties"`
	Tags    []Tag    `xml:"tag"`
}

type Tag struct {
	Key   string `xml:"name,attr"`
	Value string `xml:",chardata"`
}

// func LoadXML(filename string) (InfomediaXML, error) {
// 	fmt.Println("loading XML media file ...")
// 	var im2xml InfomediaXML
// 	im2File, err := os.Open(filename)
// 	defer im2File.Close()
// 	if err != nil {
// 		return im2xml, err
// 	}
// 	xmlParser := xml.NewDecoder(im2File)
// 	err = xmlParser.Decode(&im2xml)
// 	if err != nil {
// 		fmt.Println("Error loading media xml file. ")
// 		return im2xml, err
// 	}
// 	fmt.Println("loading media xml file ...success ")
// 	return im2xml, err

// }
