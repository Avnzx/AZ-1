.PHONY: view
view: parta makedocs

viewcurrent: parta
	evince parta.pdf & 

makedocs: parta
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
	dot parta/storyboard.dot -Tsvg > storyboard.svg
	inkscape parta/storyboard.svg -o parta/storyboard.png
	inkscape parta/context.svg -o parta/context.png
	mmdc -e png -i parta/flowchart.mmd -s 4
	latexmk -xelatex parta.tex
    
# partb:
