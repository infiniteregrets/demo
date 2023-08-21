package main

import (
	"fmt"
	"log"
	"net/http"
	"os"
)

const (
	outputFileName = "/tmp/output.txt"
)

func main() {
	http.HandleFunc("/api/v1", handleRequestV1)
	http.HandleFunc("/api/v2", handleRequestV2)

	log.Println("Starting server on port 8080...")
	if err := http.ListenAndServe(":8080", nil); err != nil {
		log.Fatal(err)
	}
}

// set next: "header_filter": "service-log: true"

func handleRequestV1(w http.ResponseWriter, r *http.Request) {
	logRequest(w, r)
	fmt.Fprint(w, "API v1 endpoint")
}

func handleRequestV2(w http.ResponseWriter, r *http.Request) {
	logRequest(w, r)
	fmt.Fprint(w, "API v2 endpoint")
}

func logRequest(w http.ResponseWriter, r *http.Request) {
	if r.Header.Get("service-log") == "true" {
		file, err := os.OpenFile(outputFileName, os.O_APPEND|os.O_CREATE|os.O_WRONLY, 0644)
		if err != nil {
			log.Println("Error opening output file:", err)
			return
		}
		defer file.Close()

		logString := fmt.Sprintf("Request from %s: %s\n", r.RemoteAddr, r.URL.Path)
		if _, err := file.WriteString(logString); err != nil {
			log.Println("Error writing to output file:", err)
		}
	}
}
