package main

import (
	"encoding/xml"
	"fmt"
)

var (
	xmlData = `
<?xml version="1.0" encoding="UTF-8"?>
<system name="SystemA123" enabled="true" open="9" close="15" timeZone="America/Denver" freq="4h" dailyMax="1" override="false">
    <hosts>
        <host address="10.1.2.3">
            <command>"free -mo</command>
            <command>"cat /proc/cpuinfo | grep processor"</command>
            <command>"ifconfig eth0 down"</command>
            <command>"shutdown -r now"</command>
            <command>"cat /proc/loadavg"</command>
        </host>
        <host address="10.1.2.4">
			<command>"free -mo</command>
			<command>"cat /proc/cpuinfo | grep processor"</command>
			<command>"ifconfig eth0 down"</command>
			<command>"shutdown -r now"</command>
			<command>"cat /proc/loadavg"</command>
		</host>
	</hosts>
</system>
`
)

type Host struct {
	XMLName  xml.Name      `xml:"host"`
	IPaddr   string        `xml:"address,attr"`
	Commands []HostCommand `xml:"commands>command"`
}

type SystemConfig struct {
	XMLName   xml.Name `xml:"system"`
	SysName   string   `xml:"name,attr"`
	Enabled   bool     `xml:"enabled,attr"`
	OpenHour  int      `xml:"open,attr"`
	CloseHour int      `xml:"close,attr"`
	TimeZone  string   `xml:"timeZone,attr"`
	Frequency string   `xml:"freq,attr"` //will use time.ParseDuration to read the interval specified here
	DailyMax  int      `xml:"dailyMax,attr"`
	Override  bool     `xml:"override,attr"`
	Hosts     []Host   `xml:"hosts>host"`
}

type HostCommand struct {
	XMLName xml.Name `xml:"command"`
	Command string   `xml:",chardata"`
}

func main() {
	conf, _ := ReadSystemConfig(xmlData)
	fmt.Printf("%#v\n", conf)
	fmt.Printf(" host ip: %q\n", conf.Hosts[0].IPaddr)
	data, _ := WriteSystemConfig(conf)
	fmt.Printf("%#v\n", data)
}

func ReadSystemConfig(data string) (*SystemConfig, error) {
	sysConf := &SystemConfig{}
	if err := xml.Unmarshal([]byte(data), sysConf); err != nil {
		return nil, err
	}
	return sysConf, nil
}

func WriteSystemConfig(sysConf *SystemConfig) (string, error) {
	dataBytes, err := xml.Marshal(sysConf)
	if err != nil {
		return "", err
	}
	return string(dataBytes), nil
}
