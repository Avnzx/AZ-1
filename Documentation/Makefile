SHELL := /bin/bash

makedocs:
	md2html --github log.md > log.html
	#dot organisation.dot -Tsvg > organisation.svg
	#dot highlevel.dot -Tsvg > highlevel.svg
view: makedocs
	firefox log.html
clean:
	rm *.html
	rm *.svg

# for the assignment
parta:
	dot parta/context.dot -Tsvg > context.svg
    
partb:
