package main

import (
	"encoding/xml"
	"fmt"
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

func PrintHostProperties() {
	v := new(HostProperties)
	err := xml.Unmarshal([]byte(data), v)
	if err != nil {
		fmt.Printf("error: %v", err)
		return
	}
	fmt.Printf("v = %#v\n", v)
	fmt.Printf("HostProperties Tag :: %q\n", v.Tags[0].Key, v.Tags[0].Value)
	fmt.Printf("HostProperties Tag :: %q\n", v.Tags[1].Key, v.Tags[1].Value)
	//fmt.Printf("PhysicalScreen Width:: %q\n", media.PhysicalScreens[0].Width)
	//fmt.Printf("PhysicalScreen Height:: %q\n", media.PhysicalScreens[0].Height)

}
