SHELL := /bin/bash

makedocs: 
	md2html --github log.md > log.html 
view: makedocs
	firefox log.html	
clean:
	rm *.html
